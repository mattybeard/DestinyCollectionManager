using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollectionManagerSite.Models
{
    public class FactionDetails
    {
        public string FactionName { get; set; }
        public List<MissingItemModel> Items { get; set; } 
    }
}