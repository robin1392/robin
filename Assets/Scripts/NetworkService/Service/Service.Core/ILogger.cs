namespace Service.Core
{
    public interface ILogger
    {
        void Info(string text);
        void Fatal(string text);
        void Error(string text);
        void Warn(string text);
        void Debug(string text);      
    }
}