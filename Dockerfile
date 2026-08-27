FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CodeCareer.sln .
COPY CodeCareer/CodeCareer.csproj CodeCareer/
COPY CodeCareer/package.json CodeCareer/
COPY CodeCareer/tailwind.config.js CodeCareer/

RUN dotnet restore CodeCareer/CodeCareer.csproj

COPY CodeCareer/ CodeCareer/
WORKDIR /src/CodeCareer

RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs \
    && npm ci \
    && npm run build:css \
    && dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
HEALTHCHECK CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "CodeCareer.dll"]
