using NgrokApi;
using System.Diagnostics;

namespace BoTflix.Service
{
    public class JellyfinService
    {
        private static readonly string _ngrokBatPath = @"C:\Program Files\Ngrok\ngrok.bat";
        private static readonly string _jellyfinPath = @"C:\Program Files\Jellyfin\jellyfin_10.8.4\jellyfin.exe";        

        internal async Task<string> GetNgrokUrl()
        {
            if (!Process.GetProcessesByName("ngrok").Any())
            {
                Helper.StartProcess(_ngrokBatPath);
                Thread.Sleep(1000); // wait 1sec
            }

            string res = await GetJellyfinUrl();
            return res;
        }

        private async Task<string> GetJellyfinUrl()
        {
            var ngrok = new Ngrok(Environment.GetEnvironmentVariable("NGROK_API_KEY"));

            Tunnel jellyfinTunnel = await ngrok.Tunnels.List().FirstAsync();
            return jellyfinTunnel.PublicUrl;
        }

        /// <summary>
        /// Check if there is any process already running, then start Jellyfin
        /// </summary>
        internal void Activate()
        {
            if (!Process.GetProcessesByName("jellyfin").Any())
            {
                Helper.StartProcess(_jellyfinPath);
                Thread.Sleep(4000); // wait 4sec
            }
        }
    }
}