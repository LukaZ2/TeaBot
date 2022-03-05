using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using TeaBot.Packet;
using Ionic.Zlib;
using TeaBot.Client;
using System.Security.Cryptography;

namespace TeaBot.Net
{
    public class NetworkClient
    {
        public TcpClient tcpClient;
        public MinecraftClient minecraftClient;
        public AesStream encryptedStream;
        public bool encrypted;
        public int compressionThreshold;
        public PacketReceiveHandler receiveHandler;
        public object streamLock = new object();
        public NetworkClient(string host, int port, MinecraftClient minecraftClient)
        {
            this.tcpClient = new TcpClient(host, port);
            tcpClient.ReceiveBufferSize = 1024 * 1024;
            tcpClient.ReceiveTimeout = 30000;
            this.minecraftClient = minecraftClient;
            compressionThreshold = 0;
            receiveHandler = new PacketReceiveHandler(this);
        }

        public void WritePacket(ClientToServerPacket packet)
        {
            lock (streamLock)
            {
                if (packet.State() != minecraftClient.clientState)
                {
                    throw new InvalidOperationException($"Cannot send packet {packet.Id} while in state {minecraftClient.clientState}. (Expected {packet.State})");
                }
                byte[] packetRaw = packet.GetPacket();
                if (encrypted)
                {
                    encryptedStream.Write(packetRaw, 0, packetRaw.Length);
                    return;
                }
                if (packet is EncryptionResponsePacket) EnableEncryption(((EncryptionResponsePacket)packet).keyRaw);
                tcpClient.Client.Send(packetRaw);
            }
        }

        public int ReadVarInt()
        {
            uint result = 0;
            int length = 0;
            while (true)
            {
                byte current = ReadByte();
                result |= (current & 0x7Fu) << length++ * 7;
                if (length > 5)
                    throw new InvalidDataException("VarInt too long!");
                if ((current & 0x80) != 128)
                    break;
            }
            return (int)result;
        }
        public int ReadVarInt(out int length)
        {
            uint result = 0;
            length = 0;
            while (true)
            {
                byte current = ReadByte();
                result |= (current & 0x7Fu) << length++ * 7;
                if (length > 5)
                    throw new InvalidDataException("VarInt too long!");
                if ((current & 0x80) != 128)
                    break;
            }
            return (int)result;
        }

        public byte ReadByte()
        {
            int result = -1;
            
            if(encrypted) { result = encryptedStream.ReadByte(); } 
            else result = tcpClient.GetStream().ReadByte();

            if (result == -1) throw new EndOfStreamException();
            return (byte)result;
        }
        public byte[] ReadBytes(int length)
        {
            var result = new byte[length];
            if(length == 0) return result;
            int n = length;
            while (true)
            {
                if(encrypted) { n -= encryptedStream.Read(result, length - n, n); }
                else n -= tcpClient.GetStream().Read(result, length - n, n);
                
                if (n == 0) break;
                Thread.Sleep(1);
            }
            return result;
        }

        public void EnableEncryption(byte[] key)
        {
            lock(streamLock)
            {
                encryptedStream = new AesStream(tcpClient.GetStream(), key);
                encrypted = true;
            }
        }

        public byte[] CompressWhenEnabled(byte[] data)
        {
            if (compressionThreshold == 0) return data;
            return new byte[1];
        }

        
    }
}
