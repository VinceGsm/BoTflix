using BoTflix.Service;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using log4net;
using System.Diagnostics;
using System.Reflection;

namespace BoTflix.Module
{
    // Your module must be public and inherit ModuleBase to be discovered by AddModulesAsync.    
    public class PrefixModule : ModuleBase<SocketCommandContext>
    {
        private readonly MessageService _messageService;        
        private readonly JellyfinService _jellyfinService;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PrefixModule(MessageService messageService, JellyfinService jellyfinService)
        {
            _jellyfinService = jellyfinService;
            _messageService = messageService;            
        }


        [Command("Jellyfin")]        
        [Discord.Commands.Summary("Active et partage un lien d'accès au server Jellyfin")]
        public async Task JellyfinAsync()
        {
            string message = string.Empty;
            SocketUserMessage userMsg = Context.Message;
            SocketGuildUser author = userMsg.Author as SocketGuildUser;

            if (author.Roles.First(x => x.Id == Helper._idJellyfinRole) == null)
                return;

            var reference = new MessageReference(userMsg.Id);

            if (Helper.IsJellyfinCorrectChannel(Context.Channel))
            {
                if (Process.GetProcessesByName("ngrok").Any()) //already in use
                {
                    await _messageService.AddReactionAlarm(userMsg);
                    await _messageService.SendJellyfinAlreadyInUse(Context.Channel, reference);
                }

                log.Info($"JellyfinAsync by {author}");

                //check NAS
                if (Pinger.Ping())
                {
                    _messageService._client.SetGameAsync(name: ": $Jellyfin", streamUrl: Helper.statusLink, type: ActivityType.Streaming);
                    List<RestMessage> pinneds = Context.Channel.GetPinnedMessagesAsync().Result.ToList();
                    _messageService.UnPinLastJelly(pinneds);
                    userMsg.PinAsync();
                    
                    await _messageService.AddReactionVu(userMsg);

                    // Jellyfin
                    _jellyfinService.Activate();

                    //activation NGrok + récupération du lien http
                    string ngrokUrl = await _jellyfinService.GetNgrokUrl();
                    log.Info($"ngrokUrl = {ngrokUrl}");

                    var builder = _messageService.MakeJellyfinMessageBuilder(userMsg, ngrokUrl);
                    Embed embed = builder.Build();

                    if (Helper.IsSundayToday())                    
                        message = $"{Helper.GetLuffyEmote()}";                                            
                    else
                        message = $"{Helper.GetPepeSmokeEmote()}";

                    await Context.Channel.SendMessageAsync(message, false, embed, null, null, reference);
                    await _messageService.AddDoneReaction(userMsg);
                }
                else
                {
                    await _messageService.AddReactionAlarm(userMsg);
                    await _messageService.JellyfinNotAvailable(Context.Channel, reference);
                }
            }
            else
            {
                await _messageService.AddReactionAlarm(userMsg);
                await _messageService.SendJellyfinNotAuthorizeHere(Context.Channel, reference);
            }
            log.Info($"JellyfinAsync done");
        }
    }
}
