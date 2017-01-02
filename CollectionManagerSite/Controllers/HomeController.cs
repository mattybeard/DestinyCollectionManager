using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BungieWebClient;
using BungieWebClient.Model.Advisors;
using BungieWebClient.Model.Character;
using BungieWebClient.Model.InventoryItem;
using BungieWebClient.Model.Membership;
using BungieWebClient.Model.Vendor;
using CollectionManagerSite.Models;
using SaleItem = BungieWebClient.Model.Vendor.SaleItem;

namespace CollectionManagerSite.Controllers
{
    public class HomeController : Controller
    {
        private BungieClient webClient { get; set; }

        public ActionResult Index()
        {
            var authorised = System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"] != null && System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"] != null;

            if (authorised)
            {                
                if (webClient == null)
                    webClient = new BungieClient(System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"].Value, System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"].Value);

                webClient.RefreshAccessToken();

                var characterIds = RetriveCharacterDetails();
                if (characterIds == null)
                    return RedirectToAction("Index", "Authentication");

                var evaItems = GetVendorMetadata(134701236);
                var petraItems = GetVendorMetadata(1410745145);

                var shaders = GetVendorItems(characterIds, "Shader", 2420628997);
                var emblems = GetVendorItems(characterIds, "Emblem", 3301500998);

                // Do we have any currently being sold?!
                var currentlyListed = GetCurrentlyForSale(evaItems, shaders, "Shaders", "Eva Shaders");
                currentlyListed.AddRange(GetCurrentlyForSale(evaItems, emblems, "Emblems", "Eva Emblems"));
                currentlyListed.AddRange(GetCurrentlyForSale(petraItems, emblems, "Queen's Wrath: Rank 1", "Petra Emblems"));
                currentlyListed.AddRange(GetCurrentlyForSale(petraItems, shaders, "Queen's Wrath: Rank 2", "Petra Shaders"));
                currentlyListed.AddRange(GetCurrentlyForSale(petraItems, shaders, "Queen's Wrath: Rank 3", "Petra Shaders"));

                if (shaders == null || emblems == null)
                    return RedirectToAction("Index", "Authentication");

                return View(currentlyListed.Union(emblems).Union(shaders).ToList());
            }

            return RedirectToAction("Index", "Authentication");
        }

        private List<MissingItemModel> GetCurrentlyForSale(AdvisorsEndpoint allEvaItems, List<MissingItemModel> missingItems, string itemCategory, string type)
        {
            var currentEvaSaleItems = allEvaItems.Response.data.vendor.saleItemCategories.FirstOrDefault(sic => sic.categoryTitle == itemCategory);
            if (currentEvaSaleItems == null)
                return new List<MissingItemModel>();

            var currentEvaItems = currentEvaSaleItems.saleItems.Select(si => si.item.itemHash).ToList();
            var items = missingItems.Where(ms => currentEvaItems.Contains(ms.Hash));

            return items.Select(i => new MissingItemModel()
            {
                Type = $"{type} currently for sale!",
                Section = i.Section,
                Name = i.Name,
                Icon = i.Icon,
                UnlockHashes = i.UnlockHashes,
                Hash = i.Hash
            }).ToList();
        }

        private AdvisorsEndpoint GetVendorMetadata(long vendorId)
        {
            var vendorDetails = webClient.RunGetAsync<AdvisorsEndpoint>($"Platform/Destiny/Vendors/{vendorId}/Metadata/");

            return vendorDetails;
        }

        private string[] RetriveCharacterDetails()
        {
            var membershipDetails = webClient.RunGetAsync<MembershipResponse>($"Platform/Destiny/SearchDestinyPlayer/{webClient.MembershipType}/{webClient.AccountName}/");
            var _membershipId = membershipDetails?.Response?.FirstOrDefault();
            if (_membershipId == null)
            {
                // this should be use the refresh token but for now lets re-authenticate
                return null;
            }
            
            webClient.MembershipType = _membershipId.membershipType;

            var characterDetails = webClient.RunGetAsync<CharacterEndpoint>($"Platform/Destiny/{webClient.MembershipType}/Account/{_membershipId.membershipId}/Summary/");
            var _characterIds = characterDetails.Response.data.characters.Select(c => c.characterBase.characterId).ToArray();

            return _characterIds;
        }
       
        private List<MissingItemModel> GetVendorItems(string[] characterIds, string type, long vendorId)
        {
            var itemsNeeded = new Dictionary<string, List<SaleItem>>();
            foreach (var character in characterIds)
            {
                var itemsCollection = webClient.RunGetAsync<VendorPlatformResponse>($"Platform/Destiny/{webClient.MembershipType}/MyAccount/Character/{character}/Vendor/{vendorId}/");
                if(itemsCollection.ErrorCode > 1)
                    throw new InvalidOperationException($"Problem getting your {type}s.");

                foreach (var category in itemsCollection.Response.data.saleItemCategories)
                {
                    if (!itemsNeeded.ContainsKey(category.categoryTitle))
                    {
                        itemsNeeded.Add(category.categoryTitle, new List<SaleItem>());

                        foreach (var item in category.saleItems)
                        {
                            if (item.unlockStatuses.Any(i => !i.isSet))
                                itemsNeeded[category.categoryTitle].Add(item);
                        }
                    }
                    else
                    {
                        var unlocked = category.saleItems.Where(i => !i.unlockStatuses.Any() || i.unlockStatuses.All(s => s.isSet)).Select(i => i.item.itemHash);
                        itemsNeeded[category.categoryTitle].RemoveAll(i => unlocked.Contains(i.item.itemHash));
                    }
                }
            }

            var results = new List<MissingItemModel>();
            foreach (var group in itemsNeeded)
            {
                if (group.Value.Any())
                {
                    foreach (var item in group.Value)
                    {
                        var inventoryItem = webClient.RunGetAsync<InventoryItemPlatformResponse>($"/Platform/Destiny/Manifest/InventoryItem/{item.item.itemHash}/");
                        var result = new MissingItemModel()
                        {
                            Type = type,
                            Section = group.Key,
                            Hash = inventoryItem.Response.data.inventoryItem.itemHash,
                            Name = inventoryItem.Response.data.inventoryItem.itemName,
                            Icon = inventoryItem.Response.data.inventoryItem.icon,
                            UnlockHashes = item.unlockStatuses.Select(i => i.unlockFlagHash)
                        };

                        //var unlockHash = item.unlockStatuses.First().unlockFlagHash;
                        //inventoryItem = webClient.RunGetAsync<InventoryItemPlatformResponse>($"/Platform/Destiny/Manifest/UnlockFlag/{unlockHash}/");

                        results.Add(result);
                    }
                }
            }

            return results;
        }
    }
}