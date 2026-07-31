# CodeCareer.AiChat

Отдельный Web API для AI-чата по конспектам.

## Запуск

```bash
dotnet run --project CodeCareer.AiChat --launch-profile https
```

Слушает `https://localhost:7301` (как в `CodeCareer/appsettings.json` → `AiChat:BaseUrl`).

## Секреты LLM

```bash
dotnet user-secrets set "Llm:ApiKey" "sk-..." --project CodeCareer.AiChat
dotnet user-secrets set "Llm:Model" "gpt-4o-mini" --project CodeCareer.AiChat
```

Без ключа API отвечает в демо-режиме (чтобы проверить UI).

## Контракт

`POST /api/chat` — body: `noteId`, `noteTitle`, `noteContext`, `messages[]`  
`GET /health`
