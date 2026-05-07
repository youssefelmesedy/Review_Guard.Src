namespace Review_Guard.Infrastructure.Logging;

public class ColoredConsoleFormatter : ConsoleFormatter
{
    public ColoredConsoleFormatter() : base("colored") { }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);

        if (message is null)
            return;

        var color = GetColor(logEntry.LogLevel);

        Console.ForegroundColor = color;

        var log = new StringBuilder();

        log.Append($"[{DateTime.Now:HH:mm:ss}] ");
        log.Append($"[{logEntry.LogLevel}] ");
        log.Append($"{logEntry.Category}: ");
        log.Append(message);

        if (logEntry.Exception is not null)
        {
            log.Append($" | EXCEPTION: {logEntry.Exception.Message}");
        }

        textWriter.WriteLine(log.ToString());

        Console.ResetColor();
    }

    private static ConsoleColor GetColor(LogLevel level) => level switch
    {
        LogLevel.Information => ConsoleColor.Green,
        LogLevel.Warning => ConsoleColor.Yellow,
        LogLevel.Error => ConsoleColor.Red,
        LogLevel.Critical => ConsoleColor.DarkRed,
        LogLevel.Debug => ConsoleColor.Cyan,
        LogLevel.Trace => ConsoleColor.Gray,
        _ => ConsoleColor.White
    };
}