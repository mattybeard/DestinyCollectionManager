using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BungieDatabaseClient.D2;

namespace CollectionManagerSite.Models
{
    public class CompleteTypeResults
    {
        public TypeResults Shaders { get; set; }
        public DateTime ShadersExpiryDate { get; set; }
        public TypeResults Emblems { get; set; }
        public TypeResults Ships { get; set; }
        public TypeResults Sparrows { get; set; }
        public List<InventoryEmblem> GotEmblems { get; set; }
        public List<InventoryEmblem> NeededEmblems { get; set; }
        public List<InventoryEmblem> UnobtainableEmblems { get; set; }

        public CompleteTypeResults()
        {
            Shaders = new TypeResults();
            Emblems = new TypeResults();
            Ships = new TypeResults();
            Sparrows = new TypeResults();
        }
    }
}