using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace playgroungClient
{
    [Serializable]
    public class SendInfo
    {
        public string message { get; set; }
        public long filesize { get; set; }
        public string filename { get; set; }

    }
}
