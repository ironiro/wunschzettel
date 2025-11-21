# Wunschzettel

Minimal ASP.NET Core Razor Pages app to create and open wishlists. Each wishlist receives a 4-digit numeric ID which can be used to re-open it.

Run locally

```bash
dotnet build
dotnet run --project wunschzettel/wunschzettel.csproj
# open https://localhost:5001 or http://localhost:5000
```

Development notes

- Wishlists are persisted to `data/wishlists.json` inside the app folder.
- The app exposes a page `/Wishlist/{id}` to view a wishlist (e.g. `/Wishlist/1234`).

CI / GitHub

This repository includes a GitHub Actions workflow that builds the project on push. To publish an artifact or container you can extend the workflow — see `.github/workflows/build.yml`.

Next steps you may want me to do:

- Choose a color palette (I can apply it to the CSS).
- Add ability to add/edit gifts inside a wishlist.
- Add authentication / protection for private wishlists.
- Add a deployment workflow (Docker image push or Azure Web App deploy).

Docker & GitHub Container Registry

Build locally with Docker:

```bash
docker build -f wunschzettel/wunschzettel/Dockerfile -t wunschzettel:local .
docker run --rm -p 8080:80 wunschzettel:local
# open http://localhost:8080
```

To push to GitHub Container Registry from CI, the workflow `.github/workflows/docker-publish.yml` builds and pushes an image to `ghcr.io/<owner>/wunschzettel:latest` on push to `main`.
