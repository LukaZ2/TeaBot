using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Security.Cryptography;
using TeaBot.Packet;
using TeaBot.Net;
using TeaBot.Event;

namespace TeaBot.Client
{
    public class MinecraftClient
    {
        public string? username;
        public ClientState clientState;
        public int version;
        public NetworkClient? networkClient;

        public MinecraftClient(int version) //47 for 1.8.*
        {
            clientState = ClientState.IDLE;
            this.version = version;
        }

        public void Connect(string host, int port, string username)
        {
            networkClient = new NetworkClient(host, port, this);
            RegisterDefaults();
            HandshakePacket handshakePacket = new HandshakePacket(version, host, port, HandshakePacket.HandshakeState.LOGIN);
            networkClient.WritePacket(handshakePacket);
            LoginStartPacket loginStartPacket = new LoginStartPacket(username!);
            networkClient.WritePacket(loginStartPacket);
        }

        public void Connect(string host, int port, string email, string password)
        {
            //Get session and username
            string username = "Undefined";
            Connect(host, port, username);
        }

        public void Disconnect()
        {
            if (networkClient == null) throw new InvalidOperationException("Not connected.");
            networkClient.tcpClient.Close();
        }
        
        private void RegisterDefaults()
        {
            if (networkClient == null) throw new InvalidOperationException("Not connected.");
            networkClient.receiveHandler.OnLoginFailed += (a) => 
            {
                networkClient.tcpClient.Close();
            };
            networkClient.receiveHandler.OnEncryptionRequest += (a) =>
            {
                EncryptionRequestArgs args = (EncryptionRequestArgs)a;
                RSACryptoServiceProvider rsa = CryptoHandler.DecodeRSAPublicKey(args.key);
                byte[] secret = CryptoHandler.GenerateAESPrivateKey();
                byte[] secretLenght = TeaBot.GetVarInt(secret.Length);
                byte[] verifyToken = args.vt;
                byte[] verifyTokenLenght = TeaBot.GetVarInt(verifyToken.Length);
                byte[] packetData = TeaBot.Concat(secretLenght, secret, verifyTokenLenght, verifyToken);
                EncryptionResponsePacket packet = new EncryptionResponsePacket(rsa.Encrypt(packetData, false), secret);
                networkClient.WritePacket(packet);
            };
            networkClient.receiveHandler.OnSetCompression += (a) =>
            {
                SetCompressionArgs args = (SetCompressionArgs)a;
                networkClient.compressionThreshold = args.threshold;
            };
        }

    }
}
