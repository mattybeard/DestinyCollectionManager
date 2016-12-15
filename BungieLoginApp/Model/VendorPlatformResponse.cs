using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BungieLoginApp.Model
{
    public class VendorPlatformResponse
    {
        public VendorResponse Response { get; set; }
        public int ErrorCode { get; set; }
        public int ThrottleSeconds { get; set; }
        public string ErrorStatus { get; set; }
        public string Message { get; set; }
        public MessageData MessageData { get; set; }
    }

    public class VendorResponse
    {
        public VendorData data { get; set; }
        //public Definitions definitions { get; set; }
    }

    public class VendorData
    {
        public long vendorHash { get; set; }
        public string nextRefreshDate { get; set; }
        public bool enabled { get; set; }
        public List<SaleItemCategory> saleItemCategories { get; set; }
        public List<object> inventoryBuckets { get; set; }
        public bool canPurchase { get; set; }
    }

    public class SaleItemCategory
    {
        public int categoryIndex { get; set; }
        public string categoryTitle { get; set; }
        public List<SaleItem> saleItems { get; set; }
    }

    public class SaleItem
    {
        public Item item { get; set; }
        public long vendorItemIndex { get; set; }
        public int itemStatus { get; set; }
        public List<long> requiredUnlockFlags { get; set; }
        public List<UnlockStatus> unlockStatuses { get; set; }
        public List<long> failureIndexes { get; set; }
    }

    public class UnlockStatus
    {
        public long unlockFlagHash { get; set; }
        public bool isSet { get; set; }
    }


    //public class Definitions
    //{
    //    public VendorDetails vendorDetails { get; set; }
    //    public Buckets buckets { get; set; }
    //    public Factions factions { get; set; }
    //    public Progressions progressions { get; set; }
    //    public Events events { get; set; }
    //    public VendorCategories vendorCategories { get; set; }
    //    public Items items { get; set; }
    //    public Stats124 stats { get; set; }
    //    public Perks perks { get; set; }
    //    public TalentGrids talentGrids { get; set; }
    //    public StatGroups statGroups { get; set; }
    //    public ProgressionMappings progressionMappings { get; set; }
    //    public ItemCategories itemCategories { get; set; }
    //    public Sources sources { get; set; }
    //    public Objectives objectives { get; set; }
    //    public DamageTypes damageTypes { get; set; }
    //    public MaterialRequirements materialRequirements { get; set; }
    //    public UnlockValues unlockValues { get; set; }
    //    public Locations locations { get; set; }
    //    public Destinations destinations { get; set; }
    //    public Activities activities { get; set; }
    //    public Books books { get; set; }
    //    public Places places { get; set; }
    //    public ActivityTypes activityTypes { get; set; }
    //    public ActivityBundles activityBundles { get; set; }
    //    public VendorSummaries vendorSummaries { get; set; }
    //    public Flags flags { get; set; }
    //}
}
