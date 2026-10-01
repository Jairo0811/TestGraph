# Phase 12 — Authentication

## Goal

Phase 12 implements TestGraph's planned **Guest Mode + Optional Account** model.

Guest users can continue using deterministic analysis endpoints without authentication. Accounts add an identity layer for saved projects, history and later cloud-oriented features.

## Account model

The first account model stores:

- user ID;
- normalized email;
- password hash;
- creation date.

Plain-text passwords are never persisted.

## Authentication

The API uses:

- ASP.NET Core password hashing;
- signed JWT bearer tokens;
- authenticated `/api/auth/me` endpoint.

## Endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/me`

Analysis endpoints remain public for Guest Mode.

## Security baseline

- minimum 8-character password;
- duplicate email rejection;
- salted framework password hashing;
- JWT signature validation;
- issuer/audience validation;
- finite token expiry.

The development signing key in `appsettings.json` is explicitly a local-development placeholder and must be replaced through environment configuration or user-secrets outside local development.

## Definition of Done

- [x] Persistent user account.
- [x] Secure password hashing.
- [x] Registration.
- [x] Login.
- [x] JWT issuance.
- [x] JWT validation.
- [x] Authenticated profile endpoint.
- [x] Guest analysis remains available.
- [x] Authentication documentation.

## Next

**Phase 13 — Academic Samples**
