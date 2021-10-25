using Microsoft.Extensions.Logging;

namespace api.cars.dealer.Common
{
    public interface IErrorHandler<out TCategoryName>
    {
        void LogTrace(string message);
    }

    public class ErrorHandler<T> : IErrorHandler<T>
    {
        private readonly ILogger<T> _logger;

        public ErrorHandler(ILogger<T> logger) => _logger = logger;

        public void LogTrace(string message)
        {
            _logger.LogTrace(message);
        }
    }
}
