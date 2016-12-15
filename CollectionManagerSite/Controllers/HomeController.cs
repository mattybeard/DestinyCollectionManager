using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BungieWebClient;
using BungieWebClient.Model.Character;
using BungieWebClient.Model.InventoryItem;
using BungieWebClient.Model.Membership;
using BungieWebClient.Model.Vendor;

namespace CollectionManagerSite.Controllers
{
    public class HomeController : Controller
    {
        private BungieClient webClient { get; set; }
        // GET: Home
        public ActionResult Index()
        {
            var authorised = System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"] != null && System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"] != null;

            if (authorised)
            {
                if (webClient == null)
                    webClient = new BungieClient(System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"].Value, System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"].Value);

                var emblems = GetEmblems();

                return View(emblems);
            }

            return RedirectToAction("Index", "Authentication");
        }

        private string[] RetriveCharacterDetails()
        {
            var membershipDetails = webClient.RunGetAsync<MembershipResponse>($"Platform/Destiny/SearchDestinyPlayer/1/mbeard/");
            var _membershipId = membershipDetails?.Response?.First().membershipId;
            var _membershipType = (int)membershipDetails?.Response?.First().membershipType;

            var characterDetails = webClient.RunGetAsync<CharacterEndpoint>($"Platform/Destiny/{_membershipType}/Account/{_membershipId}/Summary/");
            var _characterIds = characterDetails.Response.data.characters.Select(c => c.characterBase.characterId).ToArray();

            return _characterIds;
        }

        private List<InventoryItemPlatformResponse> GetEmblems()
        {
            var emblemsNeeded = new Dictionary<string, List<long>>();
            var _characterIds = RetriveCharacterDetails();

            foreach (var character in _characterIds)
            {
                var emblemsCollection = webClient.RunGetAsync<VendorPlatformResponse>($"Platform/Destiny/1/MyAccount/Character/{character}/Vendor/2420628997/");
                foreach (var category in emblemsCollection.Response.data.saleItemCategories)
                {
                    if (!emblemsNeeded.ContainsKey(category.categoryTitle))
                    {
                        emblemsNeeded.Add(category.categoryTitle, new List<long>());

                        foreach (var item in category.saleItems)
                        {
                            if (item.unlockStatuses.Any(i => !i.isSet))
                                emblemsNeeded[category.categoryTitle].Add(item.item.itemHash);
                        }
                    }
                    else
                    {
                        var unlocked = category.saleItems.Where(i => !i.unlockStatuses.Any() || i.unlockStatuses.All(s => s.isSet)).Select(i => i.item.itemHash);
                        emblemsNeeded[category.categoryTitle].RemoveAll(i => unlocked.Contains(i));
                    }
                }
            }

            var results = new List<InventoryItemPlatformResponse>();
            foreach (var group in emblemsNeeded)
            {
                if (group.Value.Any())
                {
                    foreach (var emblem in group.Value)
                    {
                        results.Add(webClient.RunGetAsync<InventoryItemPlatformResponse>($"/Platform/Destiny/Manifest/InventoryItem/{emblem}/"));
                    }
                }
            }

            return results;
        }
    }
}