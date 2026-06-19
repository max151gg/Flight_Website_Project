using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace SkyPathWSClient
{
    // Gives the whole app ONE shared HttpClient (the Singleton pattern).
    // Reusing one client instead of creating a new one for every request avoids
    // running out of network connections. ApiClient gets this client through Instance.
    public class SkyPathHttpClient
    {
        private static HttpClient createClient()
        {
            SocketsHttpHandler handler = new SocketsHttpHandler();
            handler.PooledConnectionLifetime = TimeSpan.FromMinutes(10);
            handler.ConnectTimeout = TimeSpan.FromSeconds(15);
            return new HttpClient(handler);
        }

        private static readonly HttpClient httpClient = createClient();
        private SkyPathHttpClient() { }
        public static HttpClient Instance
        {
            get
            {
                return httpClient;
            }
        }
    }
}
