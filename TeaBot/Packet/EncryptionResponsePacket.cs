using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaBot.Client;
using System.Security.Cryptography;

namespace TeaBot.Packet
{
    internal class EncryptionResponsePacket : ClientToServerPacket
    {

        public byte[] keyRaw;
        public EncryptionResponsePacket(byte[] encryptedPacket, byte[] keyRaw)
        {
            Append(encryptedPacket);
            this.keyRaw = keyRaw;
        }
        public override int Id()
        {
            return 0x01;
        }

        public override ClientState State()
        {
            return ClientState.LOGIN;
        }
    }
}
