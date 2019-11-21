namespace Durty.OBS.Watcher.Contracts
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error
    }
    public interface ILogger
    {
        void Write(LogLevel logLevel, string message);
    }
}
