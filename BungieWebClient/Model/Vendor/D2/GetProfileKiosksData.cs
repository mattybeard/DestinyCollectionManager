using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BungieWebClient.Model.Vendor.D2
{
    public class GetProfileKiosksData
    {
        public Dictionary<long, GetProfileKioskItem[]> kioskItems { get; set; }
    }
}
