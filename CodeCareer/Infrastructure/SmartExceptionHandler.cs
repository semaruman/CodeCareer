using Microsoft.AspNetCore.Diagnostics;

namespace CodeCareer.Infrastructure
{
    public class SmartExceptionHandler : IExceptionHandler
    {
        private readonly ILogger _logger;

        public SmartExceptionHandler(ILogger<SmartExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken token
            )
        {
            _logger.LogError("Произошла ошибка в {Method}: {Path}",
                context.Request.Method,
                context.Request.Path
                );

            //если клиент прервал запрос, то не отправляю ответ
            if (token.IsCancellationRequested)
            {
                return false;
            }

            switch (exception)
            {
                case ArgumentException argumentExceprion:
                    context.Response.StatusCode = 400; //BadRequest
                    await context.Response.WriteAsync($"Некорректные данные: {argumentExceprion.Message}");
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = 401; //Unauthorized
                    await context.Response.WriteAsync("Пользователь не авторизован");
                    break;

                default:
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Что-то пошло не так");
                    break;
            }
            return true;
        }
    }
}
