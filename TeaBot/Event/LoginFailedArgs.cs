using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaBot.DataType;

namespace TeaBot.Event
{
    internal class LoginFailedArgs : EventArgs
    {
        public Chat reason;
    }
}
