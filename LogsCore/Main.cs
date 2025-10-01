using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using Discord;
using Discord.WebSocket;
using Discord.Webhook;
using LogsApi;

namespace LogsCore;

public class PluginConfig : BasePluginConfig
{
    public string DiscordBotToken { get; set; } = string.Empty;
    public string DiscordWebhookUrl { get; set; } = string.Empty;
    public string VkToken { get; set; } = string.Empty;
}

public class Main : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "LogsCore";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "iks__";
    public override string ModuleDescription => "Module for all you're logs";

    public static BasePlugin Instance = null!;

    private PluginCapability<ILogsApi> _pluginCapability { get; } = new("logs:core");
    public PluginConfig Config { get; set; } = null!;
    public static PluginConfig sConfig { get; set; } = null!;
    private static DiscordSocketClient? _client;
    private static DiscordWebhookClient? _webhookClient;
    private Api _api = null!;

    public override void Load(bool hotReload)
    {
        Instance = this;
        _api = new();
        Capabilities.RegisterPluginCapability(_pluginCapability, () => _api);
        LogsCoreUtils.Api = _api;

        if (!string.IsNullOrEmpty(Config.DiscordBotToken))
        {
            Task.Run(async () => { await StartBot(); });
        }
        if (!string.IsNullOrEmpty(Config.DiscordWebhookUrl))
        {
            _webhookClient = new DiscordWebhookClient(Config.DiscordWebhookUrl);
            Console.WriteLine("LogsCore DISCORD: webhook client initialized");
        }
    }

    private async Task StartBot()
    {
        try
        {
            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.MessageContent
            };
            _client = new DiscordSocketClient(config);

            var token = Config.DiscordBotToken;
            _client.Log += OnLog;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            Console.WriteLine("Bot started");

            await Task.Delay(-1);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task OnLog(LogMessage message)
    {
        Console.WriteLine("LogsCore DISCORD: " + message.Message);
    }

    public static async Task SendEmbed(Embed embed, ulong channelId = 0)
    {

        if (_client != null && channelId != 0)
        {
            Console.WriteLine("Send embed with discord bot");
            var channel = await _client.GetChannelAsync(channelId) as IMessageChannel;
            if (channel == null) return;
            await channel.SendMessageAsync(embed: embed);
        }
        if (_webhookClient != null)
        {
            Console.WriteLine("Send embed with webhook");
            await _webhookClient.SendMessageAsync(embeds: new[] { embed });
        }
    }

    public override void Unload(bool hotReload)
    {
        if (_client != null)
        {
            Task.Run(async () => { await _client.StopAsync(); });
        }

        _webhookClient?.Dispose();
    }

    public void OnConfigParsed(PluginConfig config)
    {
        Config = config;
        sConfig = config;
    }
}

public class Api : ILogsApi
{
    public Dictionary<string, ILogger> Loggers => new();

    public ILogger CreateBaseLogger(string loggerName, ConsoleColor consoleColor = ConsoleColor.DarkCyan)
    {
        return new BaseLogger(loggerName, consoleColor);
    }

    public DiscordEmbed? GetEmbedFromConfig(string eName, string[] keys, params object[] values)
    {
        throw new NotImplementedException();
    }

    public void RegisterLogger(ILogger logger)
    {
        if (Loggers.ContainsKey(logger.LoggerName))
        {
            throw new Exception("Logger with same name already registered");
        }
        Loggers.Add(logger.LoggerName, logger);
    }
}
