using System.Collections.Concurrent;

namespace APICatalogo.Logging
{
    public class CustomLoggerProvider(CustomLoggerProviderConfiguration config) : ILoggerProvider
    {
        readonly CustomLoggerProviderConfiguration loggerConfig = config;

        readonly ConcurrentDictionary<string, CustomerLogger> loggers =
                   new();

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, name => new CustomerLogger(name, loggerConfig));
        }

        public void Dispose()
        {
            loggers.Clear();
            GC.SuppressFinalize(this); 
        }
    }
}
