using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaBot.Client;

namespace TeaBot.Packet
{
    internal class HandshakePacket : ClientToServerPacket
    {

        bool login = false;
        public HandshakePacket(int protocolVersion, string host, int port, HandshakeState state)
        {
            Append(protocolVersion);
            Append(host);
            Append((ushort)port);
            switch(state)
            {
                case HandshakeState.LOGIN: 
                    Append(2);
                    login = true;
                    break;
                case HandshakeState.STATUS: Append(1);
                    break;
                default:
                    break;
            }
        }

        public override int Id()
        {
            return 0x00;
        }
        public override ClientState State()
        {
            return ClientState.IDLE;
        }
        public enum HandshakeState
        {
            LOGIN, STATUS
        }
        public override void OnPacketSent(MinecraftClient client)
        {
            if(login) client.clientState = ClientState.LOGIN;
        }
    }
}
