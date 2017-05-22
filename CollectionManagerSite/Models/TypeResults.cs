using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollectionManagerSite.Models
{
    public class TypeResults
    {
        public string ItemType { get; set; }
        public List<FactionDetails> ForSale { get; set; }
        public List<FactionDetails> Needed { get; set; }

        public TypeResults()
        {
            ForSale = new List<FactionDetails>();
            Needed = new List<FactionDetails>();
        }
    }
}