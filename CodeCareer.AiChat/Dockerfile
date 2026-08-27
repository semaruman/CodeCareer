FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY CodeCareer.AiChat/CodeCareer.AiChat.csproj CodeCareer.AiChat/
RUN dotnet restore CodeCareer.AiChat/CodeCareer.AiChat.csproj
COPY CodeCareer.AiChat/ CodeCareer.AiChat/
WORKDIR /src/CodeCareer.AiChat
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
HEALTHCHECK CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "CodeCareer.AiChat.dll"]
