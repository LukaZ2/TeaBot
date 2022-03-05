using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaBot.Client;

namespace TeaBot.Packet
{
    internal class LoginStartPacket : ClientToServerPacket
    {
        public LoginStartPacket(string username)
        {
            Append(username);
        }
        public override int Id()
        {
            return 0x00;
        }
        public override ClientState State()
        {
            return ClientState.LOGIN;
        }
    }
}
