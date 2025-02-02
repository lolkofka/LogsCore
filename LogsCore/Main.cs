using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using Discord;
using Discord.WebSocket;
using LogsApi;

namespace LogsCore;

public class PluginConfig : BasePluginConfig
{
    public string DiscordBotToken { get; set; } = string.Empty;
    public string VkToken { get; set; } = string.Empty;
    public Dictionary<string, Dictionary<string, DiscordEmbed>> Embeds {get; set;} = new()
    {
        {"example_plugin", new () {{"example_field", new DiscordEmbed()}}}
    };  
}

public class Main : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "LogsCore";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "iks__";
    public override string ModuleDescription => "Module for all you're logs";

    public static BasePlugin Instance = null!;

    private PluginCapability<ILogsApi> _pluginCapability { get; } = new("logs:core");
    public PluginConfig Config {get; set;} = null!;
    private static DiscordSocketClient _client = null!;
    private Api _api = null!;


    public override void Load(bool hotReload)
    {
        Instance = this;
        _api = new();
        Capabilities.RegisterPluginCapability(_pluginCapability, () => _api);
        LogsCoreUtils.Api = _api;

        if (Config.DiscordBotToken != string.Empty)
        Task.Run(async () => {
            await StartBot();
        });
    }

    private async Task StartBot()
    {
        try
        {
            var config = new DiscordSocketConfig() {
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

    public static async Task SendEmbed(Embed embed, ulong channelId)
    {
        Console.WriteLine("Send embed");
        var channel = await _client.GetChannelAsync(channelId) as IMessageChannel;
        if (channel == null)
        {
            return;
        }
        await channel.SendMessageAsync(embed: embed);
    }
    public override void Unload(bool hotReload)
    {
        if (Config.DiscordBotToken != string.Empty)
        {
            Task.Run(async () => {
                await _client.StopAsync();
            });
        }
    }

    public void OnConfigParsed(PluginConfig config)
    {
        Config = config;
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
