using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using System.Text;

namespace Review_Guard.API.Logging;

public class ColoredConsoleFormatter : ConsoleFormatter
{
    public ColoredConsoleFormatter()
        : base("colored")
    {
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(
            logEntry.State,
            logEntry.Exception);

        if (string.IsNullOrWhiteSpace(message))
            return;

        var time = DateTime.Now.ToString("HH:mm:ss");
        var level = logEntry.LogLevel.ToString().ToUpper();
        var category = logEntry.Category ?? "Unknown";

        // تقليل طول الـ namespace
        category = category
                .Replace("Microsoft.EntityFrameworkCore", "EF")
                .Replace("Review_Guard", "RG");

        var builder = new StringBuilder();

        builder.Append($"[{time}] ");
        builder.Append($"[{level,-5}] ");
        builder.Append($"[{category,-20}] ");
        builder.Append($"{message}\n");

        if (logEntry.Exception is not null)
        {
            builder.AppendLine();
            builder.Append($"EXCEPTION: {logEntry.Exception}");
        }

        Console.ForegroundColor = GetColor(logEntry.LogLevel);

        Console.WriteLine(builder.ToString());

        Console.ResetColor();
    }

    private static ConsoleColor GetColor(LogLevel level) =>
        level switch
        {
            LogLevel.Trace => ConsoleColor.Gray,
            LogLevel.Debug => ConsoleColor.Cyan,
            LogLevel.Information => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.DarkRed,
            _ => ConsoleColor.White
        };
}