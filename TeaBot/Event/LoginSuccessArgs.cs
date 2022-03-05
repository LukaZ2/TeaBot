using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaBot.Event
{
    internal class LoginSuccessArgs : EventArgs
    {
        public string uuid;
        public string username;
    }
}
