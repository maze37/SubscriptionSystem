using System.Data;
using Core.Database;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;
using SharedKernel;
using SubscriptionService.Domain;

namespace SubscriptionService.Infrastructure.Database;

/// <inheritdoc/>
public class TransactionManager : ITransactionManager
{
    private readonly AppDbContext _context;
    private readonly ILogger<TransactionManager> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public TransactionManager(
        AppDbContext context,
        ILogger<TransactionManager> logger,
        ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public async Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            IncreaseVersions();

            await _context.SaveChangesAsync(cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(
                ex,
                "Сущность была изменена параллельной транзакцией");

            return GeneralErrors.ConcurrencyConflict();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
                return GeneralErrors.UniqueConstraintViolation(pgEx.ConstraintName);

            _logger.LogError(
                ex,
                "Ошибка PostgreSQL {SqlState}: {Message}",
                pgEx.SqlState,
                pgEx.MessageText);

            return GeneralErrors.DatabaseError();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "DbUpdateException: {Message}, Inner: {Inner}",
                ex.Message,
                ex.InnerException?.Message);

            return GeneralErrors.DatabaseError();
        }
    }

    public async Task<Result<ITransactionScope, Error>> BeginTransactionAsync(
        CancellationToken cancellationToken = default,
        IsolationLevel? level = null)
    {
        try
        {
            var transaction = await _context.Database
                .BeginTransactionAsync(level ?? IsolationLevel.ReadCommitted, cancellationToken);

            var logger = _loggerFactory.CreateLogger<TransactionScope>();
            var transactionScope = new TransactionScope(transaction.GetDbTransaction(), logger);

            return transactionScope;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to begin transaction");
            return GeneralErrors.DatabaseError();
        }
    }

    private void IncreaseVersions()
    {
        var entries = _context.ChangeTracker
            .Entries<IVersionedEntity>()
            .Where(entry => entry.State == EntityState.Modified);

        foreach (var entry in entries)
            entry.Entity.IncreaseVersion();
    }
}
