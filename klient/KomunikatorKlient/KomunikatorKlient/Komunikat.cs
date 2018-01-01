using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomunikatorKlient
{
    class Komunikat
    {
        public string recipient;
        public string sender;
        public string type;
        public string content;

        public Komunikat(string r, string s, string t, string c)
        {
            recipient = r;
            sender = s;
            type = t;
            content = c;
        }
    }
}
