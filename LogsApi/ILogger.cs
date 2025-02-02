namespace LogsApi;

public interface ILogger
{
    public string LoggerName {get;}
    public bool CanLogToFile { get; set;}
    public void LogToFile(string message);
    public void LogToConsole(string message);
    public void LogToVk(string message, long chatId, string vkBotToken = "");
    public void LogToDiscord(DiscordEmbed embed, ulong channelId);
    public bool CanLogToConsole { get; set; } 
    public bool CanLogToVk { get; set; } 
    public bool CanLogToDiscord { get; set; } 
    public void LogToAll(string message, long vkChatId = 0, string vkBotToken = "", DiscordEmbed? discordEmebed = null, ulong discordChannel = 0);
}