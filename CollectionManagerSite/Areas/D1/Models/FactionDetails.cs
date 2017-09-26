using System.Collections.Generic;

namespace CollectionManagerSite.Areas.D1.Models
{
    public class FactionDetails
    {
        public string FactionName { get; set; }
        public List<MissingItemModel> Items { get; set; } 
    }
}