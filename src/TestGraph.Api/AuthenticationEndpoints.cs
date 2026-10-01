using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TestGraph.Domain.Identity;
using TestGraph.Infrastructure.Persistence;

namespace TestGraph.Api;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/auth/register", async (
            RegisterRequest request,
            TestGraphDbContext db,
            IPasswordHasher<UserAccount> passwordHasher,
            IConfiguration configuration,
            CancellationToken ct) =>
        {
            var email = request.Email?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest(new { error = "Email and password are required." });
            }

            if (request.Password.Length < 8)
            {
                return Results.BadRequest(new { error = "Password must contain at least 8 characters." });
            }

            if (await db.Users.AnyAsync(user => user.Email == email, ct))
            {
                return Results.Conflict(new { error = "An account with that email already exists." });
            }

            var user = new UserAccount(Guid.NewGuid(), email, "pending");
            user.SetPasswordHash(passwordHasher.HashPassword(user, request.Password));

            db.Users.Add(user);
            await db.SaveChangesAsync(ct);

            return Results.Ok(CreateTokenResponse(user, configuration));
        });

        endpoints.MapPost("/api/auth/login", async (
            LoginRequest request,
            TestGraphDbContext db,
            IPasswordHasher<UserAccount> passwordHasher,
            IConfiguration configuration,
            CancellationToken ct) =>
        {
            var email = request.Email?.Trim().ToLowerInvariant();

            var user = await db.Users.SingleOrDefaultAsync(item => item.Email == email, ct);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verification == PasswordVerificationResult.Failed)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(CreateTokenResponse(user, configuration));
        });

        endpoints.MapGet("/api/auth/me", (ClaimsPrincipal principal) =>
        {
            if (principal.Identity?.IsAuthenticated != true)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(new
            {
                id = principal.FindFirstValue(ClaimTypes.NameIdentifier),
                email = principal.FindFirstValue(ClaimTypes.Email)
            });
        }).RequireAuthorization();

        return endpoints;
    }

    private static object CreateTokenResponse(UserAccount user, IConfiguration configuration)
    {
        var issuer = configuration["Jwt:Issuer"] ?? "TestGraph";
        var audience = configuration["Jwt:Audience"] ?? "TestGraph.Web";
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");

        var expiresAt = DateTimeOffset.UtcNow.AddHours(8);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            ],
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            user = new { user.Id, user.Email }
        };
    }
}

public sealed record RegisterRequest(string Email, string Password);
public sealed record LoginRequest(string Email, string Password);
