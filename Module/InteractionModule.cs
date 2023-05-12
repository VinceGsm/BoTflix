using BoTflix.Service;
using Discord;
using Discord.Interactions;
using log4net;
using System.Reflection;

namespace BoTflix.Module
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        private const ulong _idReadRulesRole = 847048535799234560;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MessageService _messageService;

        public InteractionModule(MessageService messageService)
        {
            _messageService = messageService;
        }

        [RequireRole(roleId: _idReadRulesRole)]
        [SlashCommand("ping",        // Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            "Mets à jour le statut de BoTflix",    // Descriptions can have a max length of 100.
            false, RunMode.Async)]
        public async Task HandlePingInteraction()
        {
            var user = Context.User;
            log.Info($"HandlePing IN by {user.Username}");

            string message = $"{Helper.GetGreeting()}```Je suis à {_messageService._client.Latency}ms de Zderland !```";

            //NAS online?           
            if (Pinger.Ping())
                _messageService._client.SetGameAsync(name: ": $Jellyfin", streamUrl: Helper.statusLink, type: ActivityType.Streaming);
            else
                message += "NAS offline, retry later";

            await RespondAsync(message, ephemeral: true);
            log.Info("HandlePing OUT");
        }
    }
}