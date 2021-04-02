namespace Service.Core
{
    public interface ILog
    {
        void Info(string text);
        void Fatal(string text);
        void Error(string text);
        void Warn(string text);
        void Debug(string text);      
    }

    public static class Logger
    {
        static ILog _logger = null;

        public static void Init(ILog logger)
        {
            _logger = logger;
        }

        public static bool IsActive()
        {
            return (_logger != null);
        }

        public static void Info(string log)
        {
            if (_logger == null)
            {
                return;
            }
            
            _logger.Info(log);
        }

        public static void Fatal(string log)
        {
            if (_logger == null)
            {
                return;
            }

            _logger.Fatal(log);
        }

        public static void Error(string log)
        {
            if (_logger == null)
            {
                return;
            }

            _logger.Error(log);
        }

        public static void Warn(string log)
        {
            if (_logger == null)
            {
                return;
            }

            _logger.Error(log);
        }

        public static void Debug(string log)
        {
            if (_logger == null)
            {
                return;
            }

            _logger.Debug(log);
        }
    }
}