using System.Collections.Generic;
using System.Windows.Forms;

namespace fazo_client_cs
{
    public class Conf
    {
        public string Host { get; set; }
        public Shortcut Shortcut { get; set; } 
    }

    public class Shortcut
    {
        public Keys Key { get; set; }
        public List<Keys> Modifiers { get; set; }
    }
}