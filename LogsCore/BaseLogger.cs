using Discord;
using LogsApi;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace LogsCore;
public class BaseLogger (string loggerName, ConsoleColor consoleColor = ConsoleColor.DarkCyan) : ILogger
{
    public string LoggerName => loggerName;

    public bool CanLogToFile {get; set;} = false;
    public bool CanLogToConsole {get; set;} = false;
    public bool CanLogToVk {get; set;} = false;
    public bool CanLogToTg {get; set;} = false;
    public bool CanLogToDiscord {get; set;} = false;

    public void LogToAll(string message, long vkChatId = 0, string vkBotToken = "", DiscordEmbed? discordEmebed = null, ulong discordChannel = 0)
    {
        if (CanLogToConsole)
            LogToConsole(message);
        if (CanLogToTg)
            LogToConsole(message);
        if (CanLogToVk)
            LogToVk(message, vkChatId);
        if (CanLogToDiscord)
            LogToDiscord(discordEmebed!, discordChannel);
        if (CanLogToFile)
            LogToFile(message);
    }

    public void LogToConsole(string message)
    {
        if (!CanLogToConsole) return;
        Console.ForegroundColor = consoleColor;
        Console.WriteLine($"[{LoggerName}] {GetDateString()} -> {message}");
        Console.ResetColor();
    }

    public void LogToDiscord(DiscordEmbed embed, ulong channelId)
    {
        Task.Run(async () => {
            try
            {
                var e = new EmbedBuilder();
                foreach (var f in embed.GetFields())
                {
                    if (f.Value.Trim() == "")
                        f.Value = "᲼";
                    if (f.Name.Trim() == "")
                        f.Name = "᲼";
    
                    e.AddField(f.Name, f.Value, f.InLine);
                }
                if (embed.Author != null)
                {
                    var a = embed.GetAuthor()!;
                    e.WithAuthor(a.Name, a.IconUrl, a.Url);
                }

                if (embed.Title != null)
                    e.WithTitle(embed.ReplaceKeyValues(embed.Title));
                if (embed.ThumbnailUrl != null)
                    e.WithThumbnailUrl(embed.ReplaceKeyValues(embed.ThumbnailUrl));
                if (embed.Timestamp != null)
                    e.WithTimestamp((DateTimeOffset)embed.Timestamp);
                if (embed.Url != null)
                    e.WithUrl(embed.ReplaceKeyValues(embed.Url));
                if (embed.ImageUrl != null)
                    e.WithImageUrl(embed.ReplaceKeyValues(embed.ImageUrl));
                if (embed.Description != null)
                    e.WithDescription(embed.ReplaceKeyValues(embed.Description));

                if (embed.Footer != null)
                {
                    var fb = new EmbedFooterBuilder();
                    if (embed.Footer.Text != null)
                        fb.WithText(embed.ReplaceKeyValues(embed.Footer.Text));
                    if (embed.Footer.IconUrl != null)
                        fb.WithText(embed.ReplaceKeyValues(embed.Footer.IconUrl));
                    e.WithFooter(fb);
                }

                e.WithColor(embed.Color.R, embed.Color.G, embed.Color.B);
                await Main.SendEmbed(e.Build(), channelId);
            }
            catch (Exception err)
            {
                LogToConsole(err.ToString());
            }
        });
    }

    public static string GetDateString()
    {
        return $"[{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}]";
    }

    public void LogToFile(string message)
    {
        var path = $"{Main.Instance.ModuleDirectory}/Logs/{loggerName}.txt";
        var msg = $"{GetDateString()} -> {message} \n";
        Task.Run(async () => {
            await File.AppendAllTextAsync(path, msg);
        });
    }

    public void LogToVk(string message, long chatId, string vkBotToken = "")
    {
        // ===
        Task.Run(async () =>
        {
            if (vkBotToken == "")
            {
                vkBotToken = Main.sConfig.VkToken;
            }
            var apiAuthParams = new ApiAuthParams
            {
                AccessToken = vkBotToken,
                Settings = Settings.Messages
            };
            var api = new VkApi();
            await api.AuthorizeAsync(apiAuthParams);

            try
            {
                await api.Messages.SendAsync(new MessagesSendParams
                {
                    RandomId = new Random().Next(),
                    PeerId = chatId,
                    Message = message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LogsCore]: Vk Message error: {ex.Message}");
            }
        });
    }
    
}