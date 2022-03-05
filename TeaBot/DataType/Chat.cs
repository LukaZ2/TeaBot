using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaBot.DataType
{
    public class Chat
    {
        public string text;
        public bool bold;
        public bool italic;
        public bool underlined;
        public bool strikedthrough;
        public bool obfuscated;
        public string color;
        public string insertion;
        public ClickEvent clickEvent;
        public HoverEvent hoverEvent;
        public Chat[] extra;
    }
}
