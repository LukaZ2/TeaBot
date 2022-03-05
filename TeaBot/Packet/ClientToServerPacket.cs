using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaBot.Client;

namespace TeaBot.Packet
{
    public abstract class ClientToServerPacket
    {

        public List<byte> data;
        public MinecraftClient client;

        public ClientToServerPacket()
        {
            data = new List<byte>();
        }
        public byte[] GetPacket()
        {
            byte[] packet = TeaBot.Concat(TeaBot.GetVarInt(Id()),data.ToArray());
            return TeaBot.Concat(TeaBot.GetVarInt(packet.Length), packet);
        }

        public byte[] GetPacketRaw()
        {
            return TeaBot.Concat(TeaBot.GetVarInt(Id()), data.ToArray());
        }

        public byte[] CompressIfPossible(int threshold)
        {
            return new byte[0];
        }

        public abstract int Id();
        public abstract ClientState State();
        public virtual void OnPacketSent(MinecraftClient client) { }

        public void Append(byte[] byteArray)
        {
            foreach (byte b in byteArray)
            {
                data.Add(b);
            }
        }

        public void Append(byte singleByte)
        {
            data.Add(singleByte);
        }

        public void Append(int i)
        {
            Append(TeaBot.GetVarInt(i));
        }

        public void Append(string s)
        {
            Append(TeaBot.GetString(s));
        }

        public void Append(ushort s)
        {
            Append(TeaBot.GetUShort(s));
        }
    }
}
