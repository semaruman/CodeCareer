# Production Readiness Report

## Completed

### Security (P0)
- [x] Password hashing via `PasswordHasher<UserModel>` with legacy plaintext migration on login
- [x] Removed hardcoded `adminPassword` — Admin uses `Role = Admin` + `[Authorize(Policy = "AdminOnly")]`
- [x] `UseAuthentication()` / `UseAuthorization()` in pipeline
- [x] `[Authorize]` on protected actions; `[AllowAnonymous]` on public pages
- [x] Global CSRF via `AutoValidateAntiforgeryToken`
- [x] Rate limiting: login, AI chat, code submit, write content
- [x] AiChat protected by `X-Internal-Api-Key` + rate limit
- [x] Security headers middleware (X-Content-Type-Options, Referrer-Policy, etc.)
- [x] HTTPS redirection + HSTS (non-Development)
- [x] Secrets via configuration/env only

### Database
- [x] Unified EF Core `ApplicationDbContext` for all entities
- [x] EF migrations (`InitialProductionSchema`)
- [x] Removed `EnsureCreated()` and runtime DDL from startup
- [x] Normalized: `user_subscriptions`, `publication_tags`, `user_skill_tags`
- [x] New tables: `submissions`, `notifications`, `user_achievements`
- [x] FK, indexes in EF model
- [x] `LegacyDataMigrator` for old denormalized columns

### Judge
- [x] `ICodeJudge` + `Judge0CodeJudge` (real Judge0 API, no fake accept)
- [x] `SubmitSolution` action — removed manual MarkTaskSolved / CheckSample progress
- [x] Submissions history on task page + `Submissions` action
- [x] Progress updated only on `Accepted`

### Product
- [x] Post edit/delete with ownership checks
- [x] Comment delete (own) + admin moderation
- [x] Notifications (follow, comment)
- [x] Achievements (extensible keys)
- [x] Avatar upload via `IFileStorage` / `LocalFileStorage`
- [x] Markdown HTML sanitization

### Infrastructure
- [x] Health check `/health` (DB)
- [x] 404/500 error pages
- [x] Structured logging middleware + RequestId
- [x] Docker compose: web + mysql + aichat + judge0
- [x] Multi-stage Dockerfile, non-root user, healthcheck
- [x] Tailwind build pipeline (npm in Docker/CI; CDN fallback in Dev only)

### Tests & CI
- [x] 31 tests (28 unit + 3 integration)
- [x] GitHub Actions CI (build, test, publish, tailwind)

### Cleanup
- [x] Removed legacy JSON services and ADO.NET services
- [x] Fixed `asp-area="Users"` → `User`
- [x] Removed separate admin password flow

## Security mechanisms

| Mechanism | Implementation |
|-----------|----------------|
| Password storage | ASP.NET Identity `PasswordHasher` |
| Auth | Cookie authentication, 7-day sliding |
| Authorization | Roles + policies |
| CSRF | Global antiforgery validation |
| Rate limit | ASP.NET Core Rate Limiting |
| XSS (notes) | HtmlSanitizer on rendered Markdown |
| AI abuse | Internal API key + rate limit + prompt length cap |

## Database migrations

Run: `dotnet ef database update --project CodeCareer`

Migration: `20250827000000_InitialProductionSchema` (generated name may vary — see `Migrations/` folder)

## Judge flow

1. User submits code on `/User/Learning/Solve`
2. `Judge0CodeJudge` POSTs to Judge0 `/submissions`
3. Polls until complete; runs public + hidden test cases
4. Saves `SubmissionModel`; on `Accepted` → `ProgressService.MarkTaskSolved`

## Tests

| Suite | Count |
|-------|-------|
| Unit (Security, Domain, Judge) | 28 |
| Integration (WebApplicationFactory) | 3 |
| **Total** | **31** |

## CI

`.github/workflows/ci.yml` — restore, tailwind build, build, test (MySQL service), publish.

## Docker services

| Service | Port |
|---------|------|
| web | 8080 |
| mysql | 3306 |
| aichat | 7300 |
| judge0 | 2358 |

## Remaining limitations

1. **HtmlSanitizer 8.1.870** — known moderate CVE; upgrade when patched version available
2. **Integration tests locally** — Learning page returns 500 without MySQL (routing test only); CI runs with MySQL service
3. **Frontend track judge** — `IFrontendChallengeValidator` extension point only; CodePen embed remains for HTML tasks
4. **Tailwind local build** — requires Node.js; Dev falls back to CDN if `tailwind.css` missing
5. **Judge0** — requires privileged Docker mode; first startup can be slow
6. **Legacy password column** — `LegacyDataMigrator` copies to `password_hash`; old column may remain until manual cleanup migration
7. **External URL health checks** for AiChat/Judge removed (package not added); use `/health` DB check + compose depends_on

## Commands to run

```bash
cp .env.example .env
docker compose up --build
dotnet test CodeCareer.sln
dotnet ef database update --project CodeCareer/CodeCareer
```
