using System;
using Durty.OBS.Watcher.Contracts;

namespace Durty.OBS.Watcher.Loggers
{
    public class ConsoleLogger
        : ILogger
    {
        public void Write(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Console.WriteLine($"[DEBUG]\t{message}");
                    break;
                case LogLevel.Info:
                    Console.WriteLine($"[INFO]\t{message}");
                    break;
                case LogLevel.Warn:
                    Console.WriteLine($"[WARN]\t{message}");
                    break;
                case LogLevel.Error:
                    Console.WriteLine($"[ERROR]\t{message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}
