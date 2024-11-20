using Serilog;

namespace FlightsAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при обработке запроса {Path}.", context.Request.Path);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Произошла внутренняя ошибка сервера.");
            }
        }
    }
}
