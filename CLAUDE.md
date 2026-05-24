# Project — dotnet-vsa-template

See `backend/CLAUDE.md` for all backend architecture, tech choices, and coding conventions.

## Workflow rules

- **Always branch before executing plans.** Before running any implementation plan, check `git branch --show-current`. If on `main` or `master`, create a feature branch first (`git checkout -b feat/<name>`). Never commit plan output directly to `main`.
