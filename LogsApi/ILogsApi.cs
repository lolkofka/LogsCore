namespace LogsApi;
public static class LogsCoreUtils 
{
    public static ILogsApi Api = null!;
}
public interface ILogsApi
{
    public Dictionary<string, ILogger> Loggers { get; }
    /// <summary>
    /// Get BaseLogger realisation
    /// </summary>
    /// <param name="loggerName">Logger Name</param>
    /// <returns>BaseLogger from Core</returns>
    public ILogger CreateBaseLogger(string loggerName, ConsoleColor consoleColor = ConsoleColor.DarkCyan);
    public void RegisterLogger(ILogger logger);
    public DiscordEmbed? GetEmbedFromConfig(string eName, string[] keys, params object[] values);
}
