using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace CodeCareer.Infrastructure
{
    public class SimpleDbInterceptor : DbCommandInterceptor
    {
        private readonly ILogger _logger;

        public SimpleDbInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            _logger.LogDebug("SQL: {sql}", command.CommandText);
            return result;
        }

        public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
        {
            //Если запросы медленные
            if (eventData.Duration.TotalMilliseconds > 100)
            {
                _logger.LogWarning("Медленный SQL ({Duration}мс): {Sql}",
                    eventData.Duration.TotalMilliseconds,
                    command.CommandText);
            }

            return result;
        }

    }
}
