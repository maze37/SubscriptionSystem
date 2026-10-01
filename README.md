# SubscriptionService

REST API для управления пользователями, тарифными планами и подписками. 
Проект показывает доменную модель подписки, работу со счетами (`Invoice`), 
   атомарное сохранение и защиту от конкурентных изменений. 
Реального списания денег и интеграции с платёжным провайдером пока нет:
   `POST /api/subscriptions/{id}/activate` имитирует подтверждение оплаты счёта.

## Возможности

- Регистрация пользователя и создание тарифного плана.
- Получение активных планов.
- Создание подписки с 14-дневным триалом или счётом на первый платёж.
- Выставление счёта при смене тарифа; новый тариф применяется после подтверждения оплаты.
- Отмена подписки в конце текущего периода.
- Просмотр подписки и её счетов.
- Повторное подтверждение оплаченного счёта без повторного изменения периода.

## Технологии и структура

.NET 10, ASP.NET Core Web API, EF Core, PostgreSQL, Serilog, Seq и Docker Compose. Команды и запросы разделены по CQRS-стилю; handlers зарегистрированы через DI. `Result` и общие интерфейсы приходят из пакетов `Mazeland.*` в GitHub Packages.

```text
src/
  SubscriptionService.Domain/          User, Plan, Subscription, Invoice, Value Objects
  SubscriptionService.Application/     команды, запросы, handlers, DTO, интерфейсы репозиториев
  SubscriptionService.Infrastructure/  AppDbContext, EF-конфигурации, миграции, репозитории
  SubscriptionService.Web/             controllers, DI, конфигурация HTTP-приложения
tests/SubscriptionService.Tests/       доменные и PostgreSQL integration-тесты
```

Путь запроса: `Controller → Handler → Domain → Repository/EF Core → SaveChangesAsync()`. 

Для команд с одним сохранением EF Core выполняет изменения атомарно в транзакции. 
`Version` используется как concurrency token; 
Уникальные индексы защищают email и правило одной действующей подписки на пользователя.

## Запуск через Docker Compose

Нужны Docker с Compose и доступ к пакетам `Mazeland.*` в GitHub Packages. 
Для применения EF-миграций с хоста дополнительно нужны .NET 10 SDK и `dotnet-ef` версии 10.

GitHub Packages NuGet [требует personal access token (classic)] с разрешением `read:packages`;

1. Скопируйте шаблон и впишите свой токен в локальный `.env`:

   ```bash
   cp .env.example .env
   ```

   ```dotenv
   GITHUB_TOKEN=ваш_токен
   APP_PORT=5001
   ```

2. Поднимите сервисы:

   ```bash
   docker compose up -d --build
   ```

3. Примените миграции к локальной PostgreSQL. Приложение не запускает их автоматически:

   ```bash
   dotnet tool install --global dotnet-ef --version '10.*'
   dotnet ef database update \
     --project src/SubscriptionService.Infrastructure/SubscriptionService.Infrastructure.csproj \
     --startup-project src/SubscriptionService.Web/SubscriptionService.Web.csproj
   ```

После миграции доступны:

| Сервис | Адрес |
| --- | --- |
| Swagger UI | <http://localhost:5001/swagger> |
| Seq UI | <http://localhost:8083> |
| PostgreSQL с хоста | `localhost:25434`, база `subscription_system_db`, пользователь `postgres` |

## Локальный запуск API

Если зависимости NuGet уже доступны на хосте, можно поднять только инфраструктуру и запустить Web-проект под отладчиком:

```bash
docker compose up -d postgres seq
dotnet restore
dotnet ef database update \
  --project src/SubscriptionService.Infrastructure/SubscriptionService.Infrastructure.csproj \
  --startup-project src/SubscriptionService.Web/SubscriptionService.Web.csproj
dotnet run --project src/SubscriptionService.Web/SubscriptionService.Web.csproj --launch-profile http
```

## Маршруты и сценарий

| Метод | Путь | Действие |
| --- | --- | --- |
| `POST` | `/api/users` | зарегистрировать пользователя |
| `POST` | `/api/plans` | создать тарифный план |
| `GET` | `/api/plans/active` | получить активные планы |
| `POST` | `/api/subscriptions` | создать подписку |
| `GET` | `/api/subscriptions/{id}` | получить подписку и счета |
| `POST` | `/api/subscriptions/{id}/change-plan` | выставить счёт на новый план |
| `POST` | `/api/subscriptions/{id}/activate` | подтвердить оплату счёта |
| `POST` | `/api/subscriptions/{id}/cancel` | запланировать отмену |

Для проверки полного сценария создайте пользователя и план, затем подписку с `withTrial: false`. 
Она получит статус `PendingPayment`, а её счёт — `Pending`. 
Возьмите `invoices[0].id` из `GET /api/subscriptions/{id}` и 
   передайте его в `POST /api/subscriptions/{id}/activate` телом `{"invoiceId":"..."}`.
После этого счёт будет `Paid`, а подписка — `Active`.

## Возможные следующие шаги

- Интеграция с платёжным провайдером и проверка webhook.
- Настройка секретов, паролей, доступа к Seq и миграций для развёртывания вне локальной машины.
