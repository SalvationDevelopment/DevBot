using System.Net;

namespace DevBot
{
    public class ServerInfos
    {
        public IPAddress Address { get; private set; }
        public int ChatPort { get; private set; }

        public bool Retrieve()
        {
#if DEBUG
            Address = IPAddress.Parse("91.250.87.52");
#else
            Address = IPAddress.Loopback;
#endif
            ChatPort = 8933;
            return true;
        }
    }
}