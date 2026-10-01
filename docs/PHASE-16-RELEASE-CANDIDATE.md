# Phase 16 — Release Candidate

## Goal

Phase 16 packages the completed V1 roadmap into a release candidate suitable for final verification before the stable v1.0.0 release.

## Version

Release candidate:

`1.0.0-rc.1`

The version is aligned across .NET build metadata, frontend package metadata and the API health endpoint.

## Release assets and process

The repository now includes:

- CHANGELOG.md;
- release-candidate checklist;
- tag-triggered release validation workflow;
- explicit known limitations;
- security policy;
- all Phase 0–16 documentation.

## Release validation

Tags matching `v*` run:

- .NET restore;
- Release build;
- backend tests;
- frontend dependency installation;
- frontend lint;
- frontend production build.

## Stable-release rule

The RC should only be promoted to v1.0.0 after:

- all stacked phase PRs are merged in dependency order;
- main CI is green;
- SQL Server persistence smoke tests pass in the intended environment;
- authentication is validated with production-grade secrets;
- export flows are manually verified;
- known limitations are accepted or resolved.

## Definition of Done

- [x] RC version assigned.
- [x] Changelog created.
- [x] Release checklist created.
- [x] Tag validation workflow created.
- [x] Known limitations documented.
- [x] README roadmap closed through Phase 16.
- [x] Stable-release promotion criteria documented.
