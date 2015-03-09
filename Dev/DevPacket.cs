using System.Text;
using DevBot.Dev.Enums;

namespace DevBot.Dev
{
    public class DevPacket
    {
        public int Id { get; private set; }
        public byte[] Data { get; private set; }

        public DevPacket(int id)
        {
            Id = id;
            Data = null;
        }

        public DevPacket(int id, byte[] data)
        {
            Id = id;
            Data = data;
        }

        public DevPacket(int id, string data)
        {
            Id = id;
            Data = Encoding.UTF8.GetBytes(data);
        }

        public static bool IsLarge(DevClientPacket packet)
        {
            switch (packet)
            {
                case DevClientPacket.GameList:
                case DevClientPacket.UserList:
                case DevClientPacket.FriendList:
                case DevClientPacket.TeamList:
                case DevClientPacket.ChannelList:
                case DevClientPacket.ChannelUsers:
                    return true;
            }
            return false;
        }

        public static bool IsOneByte(DevClientPacket packet)
        {
            switch (packet)
            {
                case DevClientPacket.LoginFailed:
                case DevClientPacket.Banned:
                case DevClientPacket.Kicked:
                case DevClientPacket.RegisterAccept:
                case DevClientPacket.RegisterFailed:
                case DevClientPacket.Pong:
                case DevClientPacket.RefuseDuelRequest:
                    return true;
            }
            return false;
        }
    }
}