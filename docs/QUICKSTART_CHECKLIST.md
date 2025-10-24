# Quickstart Checklist

Use this checklist to validate the setup end-to-end on a fresh environment.

## Prerequisites
- .NET 8 SDK installed
- PostgreSQL running locally (default port 5432)
- Connection string configured via User Secrets or appsettings.Development.json

## 1) Restore and Build
- Run: dotnet restore
- Run: dotnet build -c Debug
- Expectation: Build succeeds with no errors

## 2) Database
- Ensure database 'estantevirtual' exists
- Run migrations from API project context (or ensure they already applied)

## 3) Run Tests
- Run: dotnet test
- Expectation: All tests pass (unit + integration)

## 4) Start API
- Start EstanteVirtual.Api (Development)
- Verify Swagger is available at http://localhost:5009/swagger (in Dev via launchSettings)

## 5) Start Blazor
- Start EstanteVirtual.Web
- App should open at http://localhost:5248

## 6) Manual Flow
- Add a book (title, author, optional cover URL)
- Verify it appears on the home gallery
- Click the book to open details
- Add or update a review (rating + optional text)
- Return to home and verify the card shows rating

## 7) CORS Check
- Confirm that API accepts requests from http://localhost:5248

## 8) Performance Check (Optional)
- Seed 100 books (via API or script)
- Load home page; ensure it loads within ~2s

If all checks pass, the Quickstart validation is successful.
