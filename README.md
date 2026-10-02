# TikTok Clone

English | [Українська](README.uk.md)

A short-video social application built with React and ASP.NET Core, with a separate worker for asynchronous video processing.

## Features

- Home and following feeds, HLS playback, search, and shareable video links.
- Video uploads, processing progress, and a personal video studio.
- Profiles, follows, likes, comments and replies, favorites, and reposts.
- Direct messages, message privacy settings, and real-time notifications.
- Email/password and Google sign-in, email confirmation, password recovery, and session management.
- Content reports, administrator moderation, and English/Ukrainian localization.

## Architecture

| Location | Purpose |
| --- | --- |
| `front/` | React 19, TypeScript, Vite, Tailwind CSS, Redux Toolkit / RTK Query, i18next |
| `back/Api/` | .NET 10 HTTP API, authentication, Swagger, SignalR |
| `back/Application/`, `back/Domain/`, `back/Contracts/` | Application logic, domain entities, shared contracts |
| `back/Infrastructure/`, `back/Persistence/` | Integrations, EF Core, PostgreSQL, database migrations |
| `back/VideoProcessor/` | FFmpeg video processing via RabbitMQ / MassTransit |
| `back/Application.Tests/` | Backend application tests |
| `tools/` | Jenkins, Terraform, deployment configuration |

PostgreSQL stores application data, Redis supports caching, and RabbitMQ connects the API and video worker. SignalR delivers chat, notifications, and processing updates. Development uses shared local media storage; production uses Amazon S3.

## Local Docker setup

Install Docker with the Compose plugin. From the repository root:

```bash
docker compose -f compose.local.yaml up --build -d
```

This starts the frontend, API, worker, PostgreSQL, Redis, and RabbitMQ. The first build downloads dependencies and may take several minutes. The API automatically applies migrations and seeds development users from `back/Api/Helpers/Users.json`. Prepared seed videos can be placed in `back/Api/SeedVideos/`.

| Service | Address |
| --- | --- |
| Web application | http://localhost |
| API health | http://localhost:8080/health |
| Swagger UI (development) | http://localhost:8080/swagger |
| RabbitMQ management | http://localhost:15672 (`guest` / `guest`) |
| PostgreSQL | `localhost:5432` |
| Redis | `localhost:6379` |

Local PostgreSQL credentials are `postgres` / `mypassword`, database `tiktok_clone`. Redis uses `localredispass`. These values are defined in `compose.local.yaml` for local development.

For Google sign-in, set `GOOGLE_CLIENT_ID` and `GOOGLE_CLIENT_SECRET` in the root `.env` and register the frontend origin with Google. Email delivery requires `SMTP_PASSWORD` and API `SMTP` settings matching your provider (host, port, username). Placeholder credentials do not enable these integrations. The local stack does not require AWS credentials.

```bash
docker compose -f compose.local.yaml logs -f api video_processor
docker compose -f compose.local.yaml down
```

`down` preserves named volumes, including the database and uploaded media. Adding `-v` deletes those volumes.

## Frontend development

Use Node.js 22 and npm, matching the frontend Docker build. Keep the Docker backend services running and create `front/.env`:

```dotenv
VITE_API_BASE_URL=http://localhost:8080
VITE_GOOGLE_CLIENT_ID=your-google-client-id
```

```bash
cd front
npm ci
npm run dev
```

Open the URL printed by Vite, normally http://localhost:5173. Register this origin too for Google sign-in. Values prefixed with `VITE_` are included in the browser bundle: never put secrets in them. Restart Vite after changing them, or rebuild the frontend image for Docker.

## Backend development

Install the .NET 10 SDK to build or run backend projects outside Docker. Each service has `appsettings.Development.json`; environment variables override settings using double underscores, such as `ConnectionStrings__DefaultConnection`.

For host development, stop the Docker API and worker to avoid port conflicts and competing queue consumers; leave `db`, `redis`, and `rabbitMQ` running. Use the API's `http` launch profile (port 8080), set `DOTNET_ENVIRONMENT=Development` for the worker, and configure both services with the same absolute `LocalStorage__RootPath`. Set the API's `Redis__ConnectionString=localhost:6379,password=localredispass`. When using Vite, set `Frontend__Url=http://localhost:5173` so password-reset links point to the right frontend.

The worker requires FFmpeg. Its Docker image installs FFmpeg; development startup also attempts to download binaries to the temporary directory. If processing fails, inspect worker logs and check binary availability. For manual installation, add FFmpeg and FFprobe to `PATH` and verify with `ffmpeg -version` and `ffprobe -version`.

## Checks and localization

From `front/`:

```bash
npm run check:locales
npm run lint
npm run build
```

From the repository root:

```bash
dotnet test back/Application.Tests/Application.Tests.csproj
```

Translations are in `front/src/locales/en/en.json` and `front/src/locales/uk/uk.json`. Add keys to both files and preserve interpolation variables such as `{{count}}`. The locale check compares nested keys, value types, and placeholders. Language detection and the English fallback are configured in `front/src/i18n.ts`.

## Deployment

`compose.server.yaml` is a starting point requiring environment-specific configuration. Use `.env_server_example` as a variable reference and explicitly pass your configured file with `--env-file .env_server`. Review `front/nginx.server.conf` (domain and TLS certificate paths), API production settings, S3 bucket/region and credentials, SMTP, database, Redis, RabbitMQ, and JWT settings.

Production administrator seeding reads `AdminAccount__Login` and `AdminAccount__Password`. Ensure both are passed through the API service's Compose environment: adding variables to a Compose `.env` file alone does not forward them to containers. The example's `AdminAccount_Password` spelling needs a double underscore for .NET.

Keep secrets out of version control, provide TLS certificates at the configured `certbot/` paths, and review exposed ports and backup configuration for your environment.
