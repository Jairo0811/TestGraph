# TestGraph v1.0.0-rc.1 — Release Candidate

## Purpose

This release candidate closes the original Phase 0–16 V1 roadmap and packages the academic-to-product evolution of TestGraph for final verification before a stable v1.0.0 tag.

## Included capabilities

- structured TGPL source analysis;
- lexical and syntactic diagnostics;
- AST construction;
- deterministic CFG generation;
- interactive CFG visualization;
- cyclomatic complexity;
- basis paths;
- adjacency matrices;
- manual and assisted structural test design;
- SQL Server project persistence;
- optional user accounts with Guest Mode preserved;
- academic samples;
- JSON/CSV/PNG/Markdown exports;
- CI and security hardening.

## Release-candidate checks

Before promoting to v1.0.0:

1. CI must pass on the release candidate commit.
2. Validate the Scholarship sample returns V(G)=7.
3. Validate basis-path count equals V(G) on supported samples.
4. Validate frontend production build and graph interaction.
5. Validate register/login/me flow with a non-development JWT key.
6. Validate SQL Server project creation and saved analysis retrieval.
7. Validate JSON, CSV, PNG and Markdown exports.
8. Confirm no secrets or production credentials are committed.
9. Review known TGPL V1 limitations.
10. Create the stable tag only after all release blockers are closed.

## Known limitations

Matrix/array syntax is not part of TGPL V1. The two original matrix exercises remain explicitly preserved as non-runnable reference samples until that language capability is introduced.

PDF reports remain outside the stable V1 requirement.
