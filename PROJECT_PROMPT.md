# Refactoring Prompt (Template for Other Projects)

Ты senior .NET архитектор.  
Нужно отрефакторить существующий проект в стиле чистой, понятной архитектуры как в SubscriptionService.

## Цель

- Максимально читаемый код.
- Явные сценарии use-case.
- Предсказуемая обработка ошибок.
- Минимум магии, максимум явных шагов.

## Архитектурные правила

1. Раздели на слои:
- Domain: сущности, value objects, инварианты, фабрики `Create(...)`, бизнес-правила.
- Application: Commands/Queries, Handlers, Validators, интерфейсы репозиториев/сервисов.
- Infrastructure: EF Core/репозитории/транзакции/внешние зависимости.
- Presentation (API): контроллеры, middleware, маппинг HTTP <-> Application.
- Shared: `Result/Error/Envelope/базовые абстракции`.

2. Result pattern везде:
- Методы use-case и domain-фабрик возвращают `Result<T, Error>` (или `Result<Error>` для внутренних шагов).
- Не бросай исключения для бизнес-ошибок.
- Ошибка содержит: `Code`, `Message`, `Type`, `InvalidField?`.
- `ErrorType`: `Validation`, `NotFound`, `Conflict`, `Failure`.

3. Единый API-ответ (Envelope):
- Успех: `{ result, errors: null, timeGenerated }`
- Ошибка: `{ result: null, errors, timeGenerated }`
- Централизованный маппинг:
  - `Validation=400`
  - `NotFound=404`
  - `Conflict=409`
  - `Failure=500`

4. Handler-пайплайн (строгий порядок):
- Validate command.
- Проверка существования/уникальности в репозиториях.
- Вызов domain factory/method.
- Persist через repository + `transactionManager.SaveChangesAsync()`.
- Маппинг DB constraint/DbUpdateException -> доменная `Conflict/Failure` ошибка.
- Возврат response DTO.

5. Маппинг (без лишней магии):
- Request -> Command в контроллере.
- Domain/Result -> Response DTO в handler.
- Error/ErrorList -> HTTP response через extension/middleware.
- Без AutoMapper, если маппинг простой (предпочтительно явный ручной код).

6. Валидация:
- FluentValidation для команд.
- Ошибки валидации конвертируй в `Error` (`code/message/field`).

7. Domain-модель:
- Value Objects создаются через `Create(...)` и возвращают `Result`.
- Сущности не допускают невалидного состояния.
- Инварианты проверяются в Domain, не в контроллерах.

8. Инфраструктура:
- Репозитории тонкие, без бизнес-логики.
- `TransactionManager` (или UnitOfWork) перехватывает `DbUpdateException` и возвращает `Error`.
- DI через extension methods (`AddApplication`, `AddInfrastructure` и т.д.).

9. Логирование:
- Serilog как основной logger.
- Seq как централизованный sink.
- Технические ошибки логируются в middleware/infrastructure.
- Бизнес-этапы и результаты логируются в handlers.

10. Результат рефакторинга:
- Единообразные контракты `ICommand/ICommandHandler/IQuery/IQueryHandler`.
- Отсутствие бизнес-исключений в обычном flow.
- Консистентный envelope и status codes во всех endpoints.

## Что сделать в ответе

1) Кратко опиши текущие проблемы кода.  
2) Дай целевую структуру папок/слоёв.  
3) Покажи пошаговый план рефакторинга.  
4) Приведи ключевые до/после примеры:
- Result pattern
- Handler pipeline
- Error -> HTTP mapping
- Request/Command/Response mapping
5) Если нужно — предложи минимальные контракты: `Error`, `Envelope`, `ITransactionManager`, `ICommandHandler`.  
6) В конце дай checklist проверок, что рефакторинг выполнен корректно.

## Стиль ответа

- Production-ready, но максимально читаемый для команды.
- Без “магии”, только явные шаги.
- Если меняешь поведение, объясни почему это безопасно.
