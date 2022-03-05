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
            return TeaBot.Concat(TeaBot.GetVarInt(Id()),data.ToArray());
        }

        public byte[] CompressIfPossible(int threshold)
        {
            if(threshold < 0)
            {
                byte[] packet = GetPacket();
                return TeaBot.Concat(TeaBot.GetVarInt(packet.Length), packet);
            }
            byte[] data = GetPacket();
            if(data.Length < threshold)
            {
                byte[] uncompressedLenght = TeaBot.GetVarInt(TeaBot.GetVarInt(0).Length
                    + data.Length);
                return TeaBot.Concat(uncompressedLenght,TeaBot.GetVarInt(0) , data);
            }
            byte[] dataLenght = TeaBot.GetVarInt(data.Length);
            byte[] compressed = TeaBot.Compress(data);
            byte[] packetLenght = TeaBot.GetVarInt(dataLenght.Length + compressed.Length);
            return TeaBot.Concat(packetLenght, dataLenght, compressed);
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
