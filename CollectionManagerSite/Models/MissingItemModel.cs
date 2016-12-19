using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollectionManagerSite.Models
{
    public class MissingItemModel
    {
        public string Type { get; set; }
        public string Section { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }

        public IEnumerable<long> UnlockHashes { get; set; }
        public long Hash { get; set; }
    }
}