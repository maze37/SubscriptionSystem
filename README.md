# SubscriptionService

REST API для управления подписками пользователей на тарифные планы.

## Что умеет сервис

- регистрация пользователей
- создание тарифных планов
- получение активных планов
- создание подписки (с триалом и без)
- смена плана подписки
- отмена подписки
- активация подписки после оплаты счёта

## Технологии

- .NET 10 / ASP.NET Core Web API
- EF Core + PostgreSQL
- MediatR (CQRS-стиль)
- FluentValidation
- Serilog + Seq
- Docker Compose

## Архитектура (Clean + DDD)

```text
src/
  SharedKernel/                      # Result/Error, базовые абстракции DDD
  SubscriptionService.Domain/        # Aggregates + ValueObjects + бизнес-правила
  SubscriptionService.Application/   # Commands/Queries + Handlers + Validators + Abstractions
  SubscriptionService.Infrastructure/# EF Core, Repositories, UnitOfWork
  SubscriptionService.Web/           # Controllers, Middleware, Endpoint contracts/mapping
```

## Result / Error подход

В проекте используется явный поток ошибок через `Result`:

- `Result<TValue, Error>` — операция возвращает данные или ошибку
- `Result<Error>` — операция без payload (внутренние шаги)

`Error` содержит:

- `errorCode` (машиночитаемый код)
- `errorMessage` (понятный текст)
- `type` (`Validation`, `NotFound`, `Conflict`, `Failure`, ...)
- `invalidField` (опционально)

### Как это проходит по слоям

1. `Domain` проверяет инварианты и возвращает `Result`, не бросает бизнес-исключения.
2. `Application` в handler идёт строгим пайплайном:
   - валидация команды
   - проверка наличия/уникальности
   - вызов доменной фабрики/метода
   - сохранение через репозиторий + `IUnitOfWork.SaveChangesAsync()`
   - маппинг инфраструктурной ошибки в `Error`
3. `Web` маппит `ErrorType -> HTTP` и отдаёт единый envelope.

## Единый формат ответа API

Успех:

```json
{
  "result": {},
  "errors": null,
  "timeGenerated": "2026-01-01T00:00:00Z"
}
```

Ошибка:

```json
{
  "result": null,
  "errors": [
    {
      "errorCode": "subscription.not_found",
      "errorMessage": "Подписка не найдена.",
      "type": 2,
      "invalidField": null
    }
  ],
  "timeGenerated": "2026-01-01T00:00:00Z"
}
```

Маппинг в HTTP:

- `Validation` -> `400`
- `NotFound` -> `404`
- `Conflict` -> `409`
- `Failure` -> `500`

## Быстрый старт

```bash
docker compose up -d --build
```

### Swagger:

- API: `http://localhost:5001/swagger`
- Seq UI: `http://localhost:8081`

## Важные файлы

- Compose: [compose.yaml](/Users/maze/Projects/SubscriptionService/compose.yaml)
- Program startup: [Program.cs](/Users/maze/Projects/SubscriptionService/src/SubscriptionService.Web/Program.cs)
- Error/Result mapping: [ControllerResultExtensions.cs](/Users/maze/Projects/SubscriptionService/src/SubscriptionService.Web/Extensions/ControllerResultExtensions.cs)

