# Phase 16 — Release Candidate

## Goal

Phase 16 packages the completed V1 roadmap into a release candidate suitable for final verification before the stable v1.0.0 release.

## Release Candidate

The validated release candidate is:

`1.0.0-rc.1`

The version was aligned across .NET build metadata, frontend package metadata and the API health endpoint.

## Validation result

The `v1.0.0-rc.1` tag triggered the dedicated Release workflow. The release validation completed its substantive gates successfully:

- .NET restore;
- Release build;
- backend tests;
- frontend dependency installation;
- frontend lint;
- frontend production build.

The same release-candidate commit had already passed the integrated `main` CI pipeline after the RC regression fixes were merged.

## Stable promotion

The repository is now prepared for the stable version:

`1.0.0`

Stable promotion updates the version metadata and changelog without changing the deterministic analysis algorithms validated by the RC.

## Release assets and process

The repository includes:

- CHANGELOG.md;
- release-candidate checklist;
- tag-triggered release validation workflow;
- explicit known limitations;
- security policy;
- all Phase 0–16 documentation.

## Known V1 limitations

- TGPL V1 does not support array/matrix syntax; the two original matrix exercises remain preserved reference samples.
- PDF report generation remains post-V1.
- production deployment secrets and SQL Server configuration must be supplied outside source control.

## Definition of Done

- [x] RC version assigned.
- [x] Changelog created.
- [x] Release checklist created.
- [x] Tag validation workflow created.
- [x] Known limitations documented.
- [x] README roadmap closed through Phase 16.
- [x] Stable-release promotion criteria documented.
- [x] RC tag created and pushed.
- [x] RC backend restore/build/tests validated.
- [x] RC frontend install/lint/build validated.
- [x] Stable v1.0.0 promotion branch prepared.
