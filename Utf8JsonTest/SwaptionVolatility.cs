using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utf8JsonTest
{
    [MessagePackObject]
    [Serializable]
    public class SwaptionVolatility
    {
        [Key(0)]
        public string Ccy { get; set; }

        [Key(1)]
        public string Tenor { get; set; }

        [Key(2)]
        public string Expiry { get; set; }

        [Key(3)]
        public double Value { get; set; }
    }
}
