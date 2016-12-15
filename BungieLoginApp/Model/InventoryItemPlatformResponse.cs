using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BungieLoginApp.Model
{
    public class InventoryItemPlatformResponse
    {
        public InventoryItemResponse Response { get; set; }
        public int ErrorCode { get; set; }
        public int ThrottleSeconds { get; set; }
        public string ErrorStatus { get; set; }
        public string Message { get; set; }
        public MessageData MessageData { get; set; }
    }

    public class InventoryItemResponse
    {
        public InventoryItemData data { get; set; }
    }

    public class InventoryItemData
    {
        public long requestedId { get; set; }
        public InventoryItem inventoryItem { get; set; }
    }

    public class InventoryItem
    {
        public long itemHash { get; set; }
        public string itemName { get; set; }
        public string itemDescription { get; set; }
        public string icon { get; set; }
        public bool hasIcon { get; set; }
        public string secondaryIcon { get; set; }
        public string actionName { get; set; }
        public bool hasAction { get; set; }
        public bool deleteOnAction { get; set; }
        public string tierTypeName { get; set; }
        public int tierType { get; set; }
        public string itemTypeName { get; set; }
        public long bucketTypeHash { get; set; }
        public long primaryBaseStatHash { get; set; }
        public Stats stats { get; set; }
        public List<long> perkHashes { get; set; }
        public int specialItemType { get; set; }
        public long talentGridHash { get; set; }
        public bool hasGeometry { get; set; }
        public long statGroupHash { get; set; }
        public List<object> itemLevels { get; set; }
        public int qualityLevel { get; set; }
        public bool equippable { get; set; }
        public bool instanced { get; set; }
        public long rewardItemHash { get; set; }
        public int itemType { get; set; }
        public int itemSubType { get; set; }
        public int classType { get; set; }
        public List<long> itemCategoryHashes { get; set; }
        public List<long> sourceHashes { get; set; }
        public bool nonTransferrable { get; set; }
        public int exclusive { get; set; }
        public int maxStackSize { get; set; }
        public int itemIndex { get; set; }
        public List<long> setItemHashes { get; set; }
        public string tooltipStyle { get; set; }
        public long questlineItemHash { get; set; }
        public bool needsFullCompletion { get; set; }
        public List<long> objectiveHashes { get; set; }
        public bool allowActions { get; set; }
        public long questTrackingUnlockValueHash { get; set; }
        public long bountyResetUnlockHash { get; set; }
        public long uniquenessHash { get; set; }
        public bool showActiveNodesInTooltip { get; set; }
        public long hash { get; set; }
        public int index { get; set; }
        public bool redacted { get; set; }
    }
}
