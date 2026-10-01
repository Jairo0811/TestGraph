# Security Policy

## Supported version

Security fixes target the current `main` branch and the latest tagged TestGraph release candidate.

## Reporting a vulnerability

Please report security issues privately through GitHub's security reporting features when available. Do not publish credentials, tokens, private connection strings, or exploit details in public issues.

Useful reports include:

- affected endpoint or component;
- reproduction steps;
- expected and actual behavior;
- potential impact;
- proposed mitigation, when known.

## Security baseline

TestGraph V1 applies:

- password hashing instead of plaintext password storage;
- JWT signature, issuer, audience and lifetime validation;
- authenticated ownership checks for saved projects;
- configurable CORS origins;
- request-body size limits;
- global fixed-window rate limiting;
- security response headers;
- deterministic analysis with no arbitrary code execution.

The development JWT key and local SQL Server connection string are placeholders. Production deployments must supply secrets and production database configuration outside source control.
