using System.Collections.Generic;

namespace CollectionManagerSite.Areas.D1.Models
{
    public class MissingItemModel
    {
        public MissingItemModel()
        {
            Obtainable = true;
        }

        public string Type { get; set; }
        public string Faction { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string SecondaryIcon { get; set; }
        public bool Obtainable { get; set; }
      
        public IEnumerable<long> UnlockHashes { get; set; }
        public long Hash { get; set; }
    }
}