using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaBot.Client;
using TeaBot.Packet;
using TeaBot.DataType;
using TeaBot.Event;
using TeaBot.Net;

namespace TeaBot.Client
{
    public class PacketReceiveHandler
    {

        public NetworkClient client;
        public PacketReceiveHandler(NetworkClient client)
        {
            this.client = client;
            Thread t = new Thread(new ThreadStart(PacketReceivedListener));
            t.Name = "TeaBot-Wrapper packet listener";
            t.Start();
        }

        private void PacketReceivedListener()
        {
            while(true)
            {
                int lenght = client.ReadVarInt();
                int packetIdLenght;
                int packetId = client.ReadVarInt(out packetIdLenght);
                byte[] packetData = client.ReadBytes(lenght-packetIdLenght);
                HandlePacket(packetId, packetData);
            }
        }

        private void HandlePacket(int packetId, byte[] data)
        {
            switch(client.minecraftClient.clientState)
            {
                case ClientState.LOGIN:
                    HandleLoginStatePacket(packetId, data);
                    break;
                case ClientState.PLAYING:
                    HandlePlayStatePacket(packetId, data);
                    break;
                default:
                    throw new InvalidOperationException("Packet received while in idle state. This error is not because of you (The developer of this wrapper messed up.).");
            }
        }

        private void HandleLoginStatePacket(int packetId, byte[] data)
        {
            Queue<byte> dataQueue = new Queue<byte>(data);
            switch(packetId)
            {
                case 0x00:
                    LoginFailedArgs loginFailedArgs = new LoginFailedArgs();
                    loginFailedArgs.reason = TeaBot.GetChat(TeaBot.ReadNextString(dataQueue));
                    OnLoginFailed(loginFailedArgs);
                    break;
                case 0x01:
                    EncryptionRequestArgs encryptionRequestArgs = new EncryptionRequestArgs();
                    encryptionRequestArgs.serverId = TeaBot.ReadNextString(dataQueue);
                    encryptionRequestArgs.keyLenght = TeaBot.ReadNextVarInt(dataQueue);
                    encryptionRequestArgs.key = TeaBot.ReadNextByteArray(dataQueue);
                    encryptionRequestArgs.vtLenght = TeaBot.ReadNextVarInt(dataQueue);
                    encryptionRequestArgs.vt = TeaBot.ReadNextByteArray(dataQueue);
                    OnEncryptionRequest(encryptionRequestArgs);
                    break;
                case 0x02:
                    LoginSuccessArgs loginSuccessArgs = new LoginSuccessArgs();
                    loginSuccessArgs.uuid = TeaBot.ReadNextString(dataQueue);
                    loginSuccessArgs.username = TeaBot.ReadNextString(dataQueue);
                    OnLoginFailed(loginSuccessArgs);
                    break;
                case 0x03:
                    SetCompressionArgs setCompressionArgs = new SetCompressionArgs();
                    setCompressionArgs.threshold = TeaBot.ReadNextVarInt(dataQueue);
                    OnSetCompression(setCompressionArgs);
                    break;
                default:
                    Console.WriteLine($"[TeaBot] Invalid packet received while login: {packetId} (Ignored)");
                    break;
            }
        }

        public void HandlePlayStatePacket(int packetId, byte[] data)
        {
            switch(packetId)
            {
                default:
                    Console.WriteLine($"[TeaBot] Invalid packet received while playing: {packetId} (Ignored)");
                    break;
            }
        }

        // EVENT

        public delegate void EventHandler(EventArgs e);

        public event EventHandler OnLoginFailed = (a) => { };
        public event EventHandler OnEncryptionRequest = (a) => { };
        public event EventHandler OnLoginSuccess = (a) => { };
        public event EventHandler OnSetCompression = (a) => { };
    }
}
