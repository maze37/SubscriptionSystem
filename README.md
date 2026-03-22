# Subscription Service

REST API для управления подписками пользователей. Построен на Clean Architecture и DDD.

## О проекте

Сервис позволяет пользователям подписываться на тарифные планы, управлять подписками и отслеживать историю платежей. Реализован полный жизненный цикл подписки — от триального периода до отмены и продления.

## Стек технологий

- **.NET 10 / C#**
- **ASP.NET Core** — Web API
- **Entity Framework Core 10** — ORM, Code First миграции
- **PostgreSQL** — база данных
- **MediatR** — CQRS паттерн
- **FluentValidation** — валидация входных данных
- **Swagger** — документация API
- **Docker / Docker Compose** — контейнеризация

## Архитектура

Проект построен по принципам **Clean Architecture** и **DDD**:
```
src/
├── SharedKernel/                    # Базовые DDD строительные блоки
│   ├── Base/                        # Entity, AggregateRoot, ValueObject
│   ├── Result/                      # Result Pattern, Error
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
    └── Program.cs
```

## Доменная модель

**`Plan`** — тарифный план. Администратор создаёт планы (Basic/Pro/Enterprise) с ценой и периодом оплаты.

**`User`** — пользователь. Хранит email и флаг использования триала — триал можно использовать только один раз.

**`Subscription`** — главный агрегат. Управляет жизненным циклом подписки и содержит историю счетов (`Invoice`).

**`Invoice`** — счёт на оплату (Entity внутри Subscription). Каждый период создаётся новый счёт.

### Статусы подписки
```
Trial → Active → Cancelled
              ↘ Expired
```

### Бизнес-правила

- Нельзя иметь две активные подписки одновременно
- Триальный период — только один раз на аккаунт (14 дней)
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