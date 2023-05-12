using BoTflix.Module;
using BoTflix.Service;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text;


namespace BoTflix.Run
{
    public class Program
    {
        private DiscordSocketClient _client;        
        private readonly string _token = Environment.GetEnvironmentVariable("BoTflix_Token");


        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public Program(DiscordSocketClient client = null)
        {         
            _client = client ?? new DiscordSocketClient(
                new DiscordSocketConfig { MessageCacheSize = 100, AlwaysDownloadUsers = true, GatewayIntents = GatewayIntents.All }
             );
            _client.SetGameAsync(name: "/ping", type: ActivityType.Streaming);            
        }

        public async Task MainAsync()
        {
            LoadLogConfig();

            IServiceProvider serviceProvider = BuildServiceProvider();

            var sCommands = serviceProvider.GetRequiredService<InteractionService>();
            await serviceProvider.GetRequiredService<InteractionHandler>().InitializeInteractionAsync();
            Console.WriteLine("InitializeInteractionAsync : done");

            var pCommands = serviceProvider.GetRequiredService<PrefixHandler>();
            pCommands.AddModule<PrefixModule>();
            await pCommands.InitializeCommandsAsync();

            Console.WriteLine("InitializeCommandsAsync: done");

            // When guild data has finished downloading (+state : Ready)
            _client.Ready += async () =>
            {
                await sCommands.RegisterCommandsToGuildAsync(UInt64.Parse(Helper.GetZderLandId()));
                await _client.DownloadUsersAsync(_client.Guilds); // DL all user
            };

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        /// Inject Services
        /// </summary>
        /// <returns></returns>
        public IServiceProvider BuildServiceProvider()
        {
            IServiceCollection services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(new MessageService(_client))
                .AddSingleton(new LogService(_client))                
                .AddSingleton(new JellyfinService())
                .AddSingleton(x => new InteractionService(_client))
                .AddSingleton<InteractionHandler>()
                .AddSingleton(x => new CommandService())
                .AddSingleton<PrefixHandler>()
                ;

            return services.BuildServiceProvider();
        }

        private static void LoadLogConfig()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
    }
}