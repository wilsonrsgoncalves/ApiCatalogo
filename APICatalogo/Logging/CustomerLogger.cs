namespace APICatalogo.Logging
{
    public class CustomerLogger(string name, CustomLoggerProviderConfiguration config) : ILogger
    {
        private readonly string loggerName = name;
        private readonly CustomLoggerProviderConfiguration loggerConfig = config;

        IDisposable? ILogger.BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == loggerConfig.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string mensagem = $"{logLevel}: {eventId.Id} - {formatter(state, exception)}";

            EscreverTextoNoArquivo(mensagem);
        }

        private static void EscreverTextoNoArquivo(string mensagem)
        {
            string caminhoArquivoLog = @"C:\Diversos\logs\Catalogo_Log.txt";
            using StreamWriter streamWriter = new(caminhoArquivoLog, true);
            try
            {
                streamWriter.WriteLine(mensagem);
                streamWriter.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}