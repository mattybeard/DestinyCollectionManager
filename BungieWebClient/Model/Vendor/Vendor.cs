using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BungieWebClient.Model.Character;
using BungieWebClient.Model.Membership;

namespace BungieWebClient.Model.Vendor
{
    public class VendorPlatformResponse
    {
        public VendorResponse Response { get; set; }
        public int ErrorCode { get; set; }
        public int ThrottleSeconds { get; set; }
        public string ErrorStatus { get; set; }
        public string Message { get; set; }
        //public MessageData MessageData { get; set; }
    }

    public class VendorResponse
    {
        public VendorData data { get; set; }
        //public Definitions definitions { get; set; }
    }

    public class VendorData
    {
        //public long vendorHash { get; set; }
        public string nextRefreshDate { get; set; }
        //public bool enabled { get; set; }
        public List<SaleItemCategory> saleItemCategories { get; set; }
        //public List<object> inventoryBuckets { get; set; }
        //public bool canPurchase { get; set; }
    }

    public class SaleItemCategory
    {
        //public int categoryIndex { get; set; }
        public string categoryTitle { get; set; }
        public List<SaleItem> saleItems { get; set; }
    }

    public class SaleItem
    {
        public Item item { get; set; }
        //public long vendorItemIndex { get; set; }
        //public int itemStatus { get; set; }
        //public List<long> requiredUnlockFlags { get; set; }
        public List<UnlockStatus> unlockStatuses { get; set; }
        public List<long> failureIndexes { get; set; }
    }

    public class UnlockStatus
    {
        public long unlockFlagHash { get; set; }
        public bool isSet { get; set; }
    }
}
