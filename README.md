# Subscription Service

REST API для управления подписками пользователей. Построен на Clean Architecture и DDD.

## О проекте

Сервис позволяет пользователям подписываться на тарифные планы, управлять подписками и отслеживать историю платежей. Реализован полный жизненный цикл подписки — от триального периода до отмены и продления.

## Стек технологий

- **.NET 10 / C#**
- **ASP.NET Core** - Web API
- **Entity Framework Core 10** - ORM
- **PostgreSQL** - база данных
- **MediatR** - CQRS паттерн
- **FluentValidation** - валидация входных данных
- **Swagger** - документация API
- **Docker / Docker Compose** - контейнеризация

## Архитектура

Проект построен по принципам **Clean Architecture** и **DDD**:
```
src/
├── SharedKernel/                    # Базовые DDD строительные блоки
│   ├── Base/                        # Entity, AggregateRoot, ValueObject
│   ├── Result/                      # Result<TValue, Error>, Result<Error>, Error
│   └── Exceptions/                  # DomainException
│
├── SubscriptionService.Domain/      # Доменный слой
│   ├── Aggregates/
│   │   ├── Plan/                    # Агрегат тарифного плана
│   │   ├── Subscription/            # Агрегат подписки + Invoice (Entity)
│   │   └── User/                    # Агрегат пользователя
│   ├── ValueObjects/                # Money, PlanName, UserEmail
│   └── Enums/                       # SubscriptionStatus, InvoiceStatus, BillingPeriod
│
├── SubscriptionService.Application/ # Прикладной слой
│   ├── Abstractions/                # Интерфейсы репозиториев, IUnitOfWork
│   ├── Behaviors/                   # ValidationBehavior (MediatR Pipeline)
│   ├── DTOs/                        # SubscriptionResponse, PlanResponse
│   └── UseCases/
│       ├── Users/                   # RegisterUser
│       ├── Plans/                   # CreatePlan, GetActivePlans
│       └── Subscriptions/           # CreateSubscription, Cancel, ChangePlan, Activate
│
├── SubscriptionService.Infrastructure/ # Инфраструктурный слой
│   ├── Persistence/                 # AppDbContext, EF Core конфигурации
│   ├── Repositories/                # UserRepository, PlanRepository, SubscriptionRepository
│   └── UnitOfWork.cs
│
└── SubscriptionService.Web/         # Web слой
    ├── Controllers/                 # UsersController, PlansController, SubscriptionsController
    ├── Middlewares/                 # ExceptionMiddleware, RequestLoggingMiddleware
    ├── Contracts/                   # EndpointEnvelope, EndpointResult
    └── Program.cs
```

## Обработка ошибок и Result pattern

В приложении используется типизированный результат:

- `Result<TValue, Error>` — для операций с полезной нагрузкой
- `Result<Error>` — для операций без payload

Ошибки возвращаются явно как `Error` (с `ErrorType`), а не через исключения в application-слое.

## Формат HTTP-ответов (Envelope)

Все контроллеры возвращают единый envelope для фронтенда:

```json
{
  "success": true,
  "data": { },
  "error": null,
  "traceId": "0H..."
}
```

Для ошибок:

```json
{
  "success": false,
  "data": null,
  "error": {
    "errorCode": "409",
    "errorMessage": "..."
  },
  "traceId": "0H..."
}
```

Маппинг `ErrorType -> HTTP status`:

- `Validation`, `Null` -> `400 BadRequest`
- `NotFound` -> `404 NotFound`
- `Conflict` -> `409 Conflict`
- `Forbidden` -> `403 Forbidden`
- прочие -> `500 InternalServerError`

## Доменная модель

**`Plan`** - тарифный план. Администратор создаёт планы (Basic/Pro/Enterprise) с ценой и периодом оплаты.

**`User`** - пользователь. Хранит email и флаг использования триала - триал можно использовать только один раз.

**`Subscription`** - главный агрегат. Управляет жизненным циклом подписки и содержит историю счетов (`Invoice`).

**`Invoice`** - счёт на оплату (Entity внутри Subscription). Каждый период создаётся новый счёт.

### Статусы подписки
```
Trial -> Active -> Cancelled
              -> Expired
```

### Бизнес-правила

- Нельзя иметь две активные подписки одновременно
- Триальный период - только один раз на аккаунт (14 дней)
- Отмена подписки сохраняет доступ до конца периода
- Нельзя отменить уже истекшую подписку

## API эндпоинты

| Метод | Путь | Описание |
|-------|------|----------|
| `POST` | `/api/users` | Зарегистрировать пользователя |
| `POST` | `/api/plans` | Создать тарифный план |
| `GET` | `/api/plans/active` | Получить активные планы |
| `POST` | `/api/subscriptions` | Создать подписку |
| `GET` | `/api/subscriptions/{id}` | Получить подписку |
| `POST` | `/api/subscriptions/{id}/cancel` | Отменить подписку |
| `POST` | `/api/subscriptions/{id}/change-plan` | Сменить план |
| `POST` | `/api/subscriptions/{id}/activate` | Активировать после оплаты |

### Коды успешных ответов

- `POST /api/users`, `POST /api/plans`, `POST /api/subscriptions` -> `201 Created` + envelope
- `GET /api/plans/active`, `GET /api/subscriptions/{id}` -> `200 OK` + envelope
- `POST /api/subscriptions/{id}/cancel`, `change-plan`, `activate` -> `200 OK` + envelope

## Запуск

#### **Не забудьте изменить "YOUR_PASSWORD" в appsettings.json**

**Требования:** Docker, .NET 10 SDK
```bash
# Клонировать репозиторий
git clone https://github.com/maze37/SubscriptionSystem.git
cd SubscriptionSystem

# Запустить PostgreSQL
docker compose up -d

# Применить миграции
dotnet ef database update \
  --project src/SubscriptionService.Infrastructure \
  --startup-project src/SubscriptionService.Web

# Запустить приложение
dotnet run --project src/SubscriptionService.Web
```
