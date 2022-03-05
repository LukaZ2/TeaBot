using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaBot.Event
{
    internal class EncryptionRequestArgs : EventArgs
    {
        public string serverId;
        public int keyLenght;
        public byte[] key;
        public int vtLenght;
        public byte[] vt;
    }
}
