using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using BungieWebClient;
using BungieWebClient.Model.Advisors;
using BungieWebClient.Model.InventoryItem;
using BungieWebClient.Model.Vendor;
using CollectionManagerSite.Models;
using Newtonsoft.Json;
using Item = BungieWebClient.Model.Character.Item;
using SaleItem = BungieWebClient.Model.Vendor.SaleItem;

namespace CollectionManagerSite.Controllers
{
    public class DetailsController : ApiController
    {
        private static AdvisorsEndpoint EvaCache { get; set; }
        private static AdvisorsEndpoint AmandaCache { get; set; }
        private static AdvisorsEndpoint PetraCache { get; set; }
        private static AdvisorsEndpoint VanguardQuartermasterCache { get; set; }
        private static AdvisorsEndpoint CrucibleQuartermasterCache { get; set; }
        private static AdvisorsEndpoint IronLordCache { get; set; }
        private static DateTime CacheExpiry { get; set; }
        private bool CacheExpired
        {
            get
            {
                if (EvaCache == null || AmandaCache == null || PetraCache == null || VanguardQuartermasterCache == null || CrucibleQuartermasterCache == null)
                    return true;

                if (CacheExpiry < DateTime.Now)
                    return true;

                if (DateTime.Now.Hour == 10 && DateTime.Now.Minute < 30)
                    return true;

                return false;
            }
        }
        public BungieClient WebClient { get; set; }
        public string GetCollectionStatus(int id, string type)
        {
            var consoleChoice = id;

            WebClient = new BungieClient(System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"].Value, System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"].Value);
            WebClient.RefreshAccessToken();
            WebClient.GetUserDetails();

            var characterIds = consoleChoice == 1 ? WebClient.XboxCharacterIds : WebClient.PsCharacterIds;
            if (CacheExpired)
            {
                EvaCache = GetVendorMetadata(134701236);
                AmandaCache = GetVendorMetadata(459708109);
                PetraCache = GetVendorMetadata(1410745145);
                VanguardQuartermasterCache = GetVendorMetadata(2668878854);
                CrucibleQuartermasterCache = GetVendorMetadata(3658200622);
                IronLordCache = GetVendorMetadata(2648860054);
                CacheExpiry = DateTime.Now.AddMinutes(30);
            }

            var resultsToGet = new[] {"Shaders", "Emblems","Sparrows","Ships"};
            var temporaryResults = new ConcurrentBag<TypeResults>();
            if (!string.IsNullOrEmpty(type))
            {
                temporaryResults.Add(CalculateItemResults(type, characterIds, consoleChoice));
            }
            else
            {
                Parallel.ForEach(resultsToGet, t =>
                {
                    temporaryResults.Add(CalculateItemResults(t, characterIds, consoleChoice));
                });
            }

            var results = new CompleteTypeResults()
            {
                Emblems = temporaryResults.FirstOrDefault(t => t.ItemType == "Emblems") ?? new TypeResults(),
                Shaders = temporaryResults.FirstOrDefault(t => t.ItemType == "Shaders") ?? new TypeResults(),
                ShadersExpiryDate = EvaCache.Response.data.vendor.nextRefreshDate,
                Sparrows = temporaryResults.FirstOrDefault(t => t.ItemType == "Sparrows") ?? new TypeResults(),
                Ships = temporaryResults.FirstOrDefault(t => t.ItemType == "Ships") ?? new TypeResults()
            };

            return JsonConvert.SerializeObject(results);
        }

        private TypeResults CalculateItemResults(string type, string[] characterIds, int consoleChoice)
        {
            var results = new TypeResults() { ItemType = type };
            if (type == "Shaders")
            {
                var shaders = GetVendorItems(characterIds, "Shader", 2420628997, consoleChoice).GroupBy(f => f.Faction).OrderBy(a => a.Key);
                foreach (var shaderGroup in shaders)
                    results.Needed.Add(new FactionDetails() { FactionName = shaderGroup.Key, Items = shaderGroup.ToList() });

                results.ForSale.AddRange(GetCurrentlyForSale(EvaCache, results.Needed, "Shaders", "Shaders"));
                results.ForSale.AddRange(GetCurrentlyForSale(PetraCache, results.Needed, "Queen's Wrath: Rank 2", "Shaders"));
                results.ForSale.AddRange(GetCurrentlyForSale(PetraCache, results.Needed, "Queen's Wrath: Rank 3", "Shaders"));
                results.ForSale.AddRange(GetCurrentlyForSale(IronLordCache, results.Needed, "Shaders", "Shaders"));
            }

            if (type == "Emblems")
            {
                var emblems = GetVendorItems(characterIds, "Emblem", 3301500998, consoleChoice).GroupBy(f => f.Faction).OrderBy(a => a.Key); ;
                foreach (var emblemGroup in emblems)
                    results.Needed.Add(new FactionDetails() { FactionName = emblemGroup.Key, Items = emblemGroup.ToList() });

                results.ForSale.AddRange(GetCurrentlyForSale(EvaCache, results.Needed, "Emblems", "Emblems"));
                results.ForSale.AddRange(GetCurrentlyForSale(PetraCache, results.Needed, "Queen's Wrath: Rank 1", "Emblems"));
                results.ForSale.AddRange(GetCurrentlyForSale(IronLordCache, results.Needed, "Emblems", "Emblems"));
            }

            if (type == "Sparrows")
            {
                var sparrows = GetVendorItems(characterIds, "Sparrow", 44395194, consoleChoice).GroupBy(f => f.Faction).OrderBy(a => a.Key); ;
                foreach (var sparrowGroup in sparrows)
                    results.Needed.Add(new FactionDetails() { FactionName = sparrowGroup.Key, Items = sparrowGroup.ToList() });

                results.ForSale.AddRange(GetCurrentlyForSale(AmandaCache, results.Needed, "Vehicles", "Sparrows"));
                results.ForSale.AddRange(GetCurrentlyForSale(VanguardQuartermasterCache, results.Needed, "Vehicles", "Sparrows"));
                results.ForSale.AddRange(GetCurrentlyForSale(CrucibleQuartermasterCache, results.Needed, "Vehicles", "Sparrows"));
            }

            if (type == "Ships")
            {
                var ships = GetVendorItems(characterIds, "Ship", 2244880194, consoleChoice).GroupBy(f => f.Faction).OrderBy(a => a.Key); ;
                foreach (var shipGroup in ships)
                    results.Needed.Add(new FactionDetails() {FactionName = shipGroup.Key, Items = shipGroup.ToList()});

                results.ForSale.AddRange(GetCurrentlyForSale(AmandaCache, results.Needed, "Ship Blueprints", "Ships"));
            }

            return results;
        }

        private List<MissingItemModel> GetVendorItems(string[] characterIds, string type, long vendorId, int membershipType)
        {
            var firstLoop = true;
            var itemsNeeded = new Dictionary<string, List<SaleItem>>();
            if (type == "Emblem")
                AddAdditionalEmblems(itemsNeeded);
            if (type == "Ship")
                AddAdditionalShips(itemsNeeded);
            if (type == "Shader")
                AddAdditionalShaders(itemsNeeded);
            if (type == "Sparrow")
                AddAdditionalSparrows(itemsNeeded);

            foreach (var character in characterIds)
            {
                var itemsCollection = WebClient.RunGetAsync<VendorPlatformResponse>($"Platform/Destiny/{membershipType}/MyAccount/Character/{character}/Vendor/{vendorId}/");
                if (itemsCollection.ErrorCode > 1)
                    throw new InvalidOperationException($"Problem getting your {type}s.");

                foreach (var category in itemsCollection.Response.data.saleItemCategories)
                {
                    if (!itemsNeeded.ContainsKey(category.categoryTitle) || firstLoop)
                    {
                        if (!itemsNeeded.ContainsKey(category.categoryTitle))
                            itemsNeeded.Add(category.categoryTitle, new List<SaleItem>());

                        foreach (var item in category.saleItems)
                        {
                            // We might have manually added in here already
                            var thisGroup = itemsNeeded[category.categoryTitle];
                            if (thisGroup.All(g => g.item.itemHash != item.item.itemHash))
                            {
                                if (item.unlockStatuses.Any(i => !i.isSet))
                                    itemsNeeded[category.categoryTitle].Add(item);
                                else if (item.failureIndexes.Any())
                                    itemsNeeded[category.categoryTitle].Add(item);
                            }
                        }

                    }
                    //else
                    //{
                    var unlocked = category.saleItems.Where(i => !i.failureIndexes.Any() && (!i.unlockStatuses.Any() || i.unlockStatuses.All(s => s.isSet))).Select(i => i.item.itemHash);
                    itemsNeeded[category.categoryTitle].RemoveAll(i => unlocked.Contains(i.item.itemHash));
                    //}

                    if (itemsNeeded.ContainsKey("Promotional"))
                    {
                        unlocked = category.saleItems.Where(i => !i.unlockStatuses.Any() || i.unlockStatuses.All(s => s.isSet)).Select(i => i.item.itemHash);
                        itemsNeeded["Promotional"].RemoveAll(i => unlocked.Contains(i.item.itemHash));
                    }
                }

                firstLoop = false;
            }

            var results = new ConcurrentBag<MissingItemModel>();
            foreach (var group in itemsNeeded)
            {
                if (group.Value.Any())
                {
                    Parallel.ForEach(group.Value, item =>
                    {

                        var inventoryItem = WebClient.RunGetAsync<InventoryItemPlatformResponse>($"/Platform/Destiny/Manifest/InventoryItem/{item.item.itemHash}/");
                        var result = new MissingItemModel()
                        {
                            Type = type,
                            Faction = group.Key,
                            Hash = inventoryItem.Response.data.inventoryItem.itemHash,
                            Name = inventoryItem.Response.data.inventoryItem.itemName,
                            Icon = inventoryItem.Response.data.inventoryItem.icon,
                            SecondaryIcon = inventoryItem.Response.data.inventoryItem.secondaryIcon ?? "",
                            UnlockHashes = item.unlockStatuses.Select(i => i.unlockFlagHash)
                        };

                        results.Add(result);
                    });                    
                }
            }

            return results.ToList();
        }


        private AdvisorsEndpoint GetVendorMetadata(long vendorId)
        {
            var vendorDetails = WebClient.RunGetAsync<AdvisorsEndpoint>($"Platform/Destiny/Vendors/{vendorId}/Metadata/");

            return vendorDetails;
        }

        private List<FactionDetails> GetCurrentlyForSale(AdvisorsEndpoint allAdvisorItems, List<FactionDetails> missingItems, string itemCategory, string type)
        {
            var currentEvaSaleItems = allAdvisorItems.Response.data.vendor.saleItemCategories.FirstOrDefault(sic => sic.categoryTitle == itemCategory);
            if (currentEvaSaleItems == null)
                return new List<FactionDetails>();

            var currentEvaItems = currentEvaSaleItems.saleItems.Select(si => si.item.itemHash).ToList();
            var items = missingItems.SelectMany(c => c.Items).Where(ms => currentEvaItems.Contains(ms.Hash)).GroupBy(c => c.Faction);

            return items.Select(i => new FactionDetails()
            {
                FactionName = i.Key,
                Items = i.ToList()
            }).ToList();
        }

        private void AddAdditionalEmblems(Dictionary<string, List<SaleItem>> itemsNeeded)
        {
            var community = new Tuple<string, long[]>("Community", new long[] { 1592588002 });
            var promotional = new Tuple<string, long[]>("Promotional", new long[] { 1443409301, 1443409300, 3253299356, 3253299357, 2686650663, 1409854025, 1409854024,
                                                                                            1426631718, 1426631716, 1426631717, 1443409298, 1426631713, 1426631719, 1426631722, 1426631723,
                                                                                            1443409303, 1443409302, 1325965974, 1443409309,  1325965972, 1325965971, 1325965977, 1325965973, 70035233, 1443409299,
                                                                                            1342743555, 1443409308, 1409854029, 1592588001, 1342743553,  1592588000, 1426631714,
                                                                                            1592588012, 1592588004, 1592588005, 1592588006, 1443409297, 1443409296,
                                                                                             1409854023, 1592588003,  1409854030, 1426631715, 1325965969, 3983828201  });
            var raid = new Tuple<string, long[]>("Raid", new long[] { 3269301481, 2372257459, 2372257458, 2372257457, 2372257456, 2372257463, 185564349, 185564348, 185564351, 185564350, 185564345 });
            var srl = new Tuple<string, long[]>("SRL", new long[] { 1777175508 });
            var holiday = new Tuple<string, long[]>("Holiday", new long[] { 3347001814 });
            var riseOfIron = new Tuple<string, long[]>("Rise of Iron", new long[] { 3983828200, 3659569693 });
            var trials = new Tuple<string, long[]>("Trials of Osiris", new long[] { 3347001815, 664737060 });
            var classEmblems = new Tuple<string, long[]>("Class", new long[] { 1600609906, 1600609907, 2840347248, 2840347249, 3357380906, 3357380907 });
            var crucible = new Tuple<string, long[]>("Crucible", new long[] { 2326927634, 2326927635 });
            var houseOfWolves = new Tuple<string, long[]>("House of Wolves", new long[] { 1459499349 });
            var ttk = new Tuple<string, long[]>("The Taken King", new long[] { 1527273555 });


            ConvertGroupToItems(itemsNeeded, community);
            ConvertGroupToItems(itemsNeeded, promotional);
            ConvertGroupToItems(itemsNeeded, raid);
            ConvertGroupToItems(itemsNeeded, srl);
            ConvertGroupToItems(itemsNeeded, holiday);
            ConvertGroupToItems(itemsNeeded, riseOfIron);
            ConvertGroupToItems(itemsNeeded, trials);
            ConvertGroupToItems(itemsNeeded, classEmblems);
            ConvertGroupToItems(itemsNeeded, crucible);
            ConvertGroupToItems(itemsNeeded, houseOfWolves);
            ConvertGroupToItems(itemsNeeded, ttk);
        }


        private void AddAdditionalShips(Dictionary<string, List<SaleItem>> itemsNeeded)
        {
            var ttk = new Tuple<string, long[]>("The Taken King", new long[] { 3644912838 });
            var crucible = new Tuple<string, long[]>("Crucible", new long[] { 1096884848, 1096884849, 1096884850, 1096884851, 1096884852, 1096884853, 1096884854, 1096884855, 1096884860, 1096884861, 1609120940, 1609120941 });
            var promo = new Tuple<string, long[]>("Promotional", new long[] { 1539265118, 2390487995 });

            ConvertGroupToItems(itemsNeeded, ttk);
            ConvertGroupToItems(itemsNeeded, crucible);
            ConvertGroupToItems(itemsNeeded, promo);
        }

        private void AddAdditionalSparrows(Dictionary<string, List<SaleItem>> itemsNeeded)
        {
            var basic = new Tuple<string, long[]>("Basic", new long[] { 1614459137 });
            var ttk = new Tuple<string, long[]>("Promotional", new long[] { 201885898, 2854740184, 2854740186, 2854740187, 3703598457, 3876794566 });
            var riseOfIron = new Tuple<string, long[]>("Rise of Iron", new long[] { 3703598456 });

            ConvertGroupToItems(itemsNeeded, basic);
            ConvertGroupToItems(itemsNeeded, ttk);
            ConvertGroupToItems(itemsNeeded, riseOfIron);



        }

        private void AddAdditionalShaders(Dictionary<string, List<SaleItem>> itemsNeeded)
        {
            var raid = new Tuple<string, long[]>("Raid", new long[] { 898062439, 898062438, 3176299289, 3176299291 });
            var ttk = new Tuple<string, long[]>("The Taken King", new long[] { 1158121103, 3056359297, 4211402863 });
            var activities = new Tuple<string, long[]>("Activities", new long[] { 202245948 });
            var vendor = new Tuple<string, long[]>("Vendor", new long[] { 194424264 });
            var promo = new Tuple<string, long[]>("Promotional", new long[] { 194424265, 1759332257, 2561402280, 2561402281, 2561402282, 2561402283, 2561402286, 3671454769, 2874472095 });
            var holiday = new Tuple<string, long[]>("Holiday", new long[] { 1759332262, 3859658560, 3859658561, 3859658562 });

            ConvertGroupToItems(itemsNeeded, raid);
            ConvertGroupToItems(itemsNeeded, ttk);
            ConvertGroupToItems(itemsNeeded, activities);
            ConvertGroupToItems(itemsNeeded, vendor);
            ConvertGroupToItems(itemsNeeded, promo);
            ConvertGroupToItems(itemsNeeded, holiday);
        }

        private void ConvertGroupToItems(Dictionary<string, List<SaleItem>> itemsNeeded, Tuple<string, long[]> grouping)
        {
            itemsNeeded.Add(grouping.Item1, CreateSalesItems(grouping.Item2.Distinct()));
        }

        private List<SaleItem> CreateSalesItems(IEnumerable<long> promoCodes)
        {
            var salesItems = new List<SaleItem>();
            foreach (var hash in promoCodes)
            {
                var saleItem = new SaleItem();
                saleItem.item = new Item() { itemHash = hash };
                saleItem.unlockStatuses = new List<UnlockStatus>();

                salesItems.Add(saleItem);
            }
            return salesItems;
        }
    }
}
