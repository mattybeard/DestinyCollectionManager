using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BungieDatabaseClient.D2;

namespace CollectionManagerSite.Models
{
    public class CompleteResults
    {
        public string Console { get; set; }
        public string GamingUsername { get; set; }
        public bool DualAccount { get; set; }
        public UserInfo Info { get; set; }
        public List<InventoryEmblem> Emblems { get; set; }
        public List<InventorySparrow> Sparrows { get; set; }
        public List<InventoryShip> Ships { get; set; }
    }
}