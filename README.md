# ResumeRadar

Paste a job posting. See how you match.

ResumeRadar compares your resume against job descriptions using AI. Upload your resume once, then paste any job posting to get a match score, skill gap analysis, and tailored talking points.

## Tech Stack

- **Backend:** ASP.NET Core 8, EF Core, SQLite
- **Frontend:** Vue 3, TypeScript, Vite, Tailwind CSS
- **AI:** Claude API (Anthropic)
- **Auth:** GitHub OAuth
- **PDF Parsing:** PdfPig

## Local Development

### Prerequisites

- .NET 8 SDK
- Node.js 18+
- A GitHub OAuth app (Settings > Developer settings > OAuth Apps)
- An Anthropic API key

### Setup

1. Clone the repo and configure secrets:

```bash
cd api/ResumeRadar.Api
cp appsettings.json appsettings.Development.json
# Edit appsettings.Development.json with your GitHub OAuth and Anthropic API keys
```

2. Start the API:

```bash
cd api/ResumeRadar.Api
dotnet run
```

3. In a second terminal, start the frontend:

```bash
cd client
npm install
npm run dev
```

4. Open `http://localhost:5173`

### Running Tests

```bash
cd api/ResumeRadar.Api.Tests
dotnet test
```

## Deployment

Designed for Azure App Service (F1 free tier). The API serves the built Vue SPA from `wwwroot/`.

```bash
cd client && npm run build
cd ../api/ResumeRadar.Api
rm -rf wwwroot && cp -r ../../client/dist wwwroot
dotnet publish -c Release
```

## Architecture

The API handles authentication, rate limiting (10 analyses/day per user), PDF text extraction, and Claude API orchestration. Resume text is stored per-user in SQLite. Analysis results (match score, strength matches, skill gaps, tailored talking points) are persisted for history.

SQLite is used for simplicity and zero-infrastructure deployment. The EF Core data layer is designed for easy migration to Azure SQL or PostgreSQL.
