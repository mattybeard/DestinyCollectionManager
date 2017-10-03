using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using BungieDatabaseClient;
using BungieDatabaseClient.D2;
using BungieWebClient;
using BungieWebClient.Model.Advisors;
using BungieWebClient.Model.Plumbing.D2;
using BungieWebClient.Model.Vendor.D2;
using CollectionManagerSite.Models;

namespace CollectionManagerSite.Controllers
{
    public class HomeController : Controller
    {
        private static DestinyDaily2Entities db { get; set; }
        private BungieClient webClient { get; set; }

        public ActionResult Index(int console = 0)
        {
            var authorised = System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"] != null && System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"] != null;

            if (authorised)
            {
                if (db == null)
                    db = new DestinyDaily2Entities();

                webClient = new BungieClient(System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"].Value, System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"].Value);

                // Check we can still authorise
                var response = webClient.RefreshAccessToken();
                if (response.ErrorCode != 1 && response.ErrorCode != 2110 && response.ErrorCode != 2106)
                    return RedirectToAction("Index", "Authentication");

                // Get the details as it's nice to see
                var gotDetails = webClient.GetUserDetails();
                if (webClient.MembershipType == -1 || !gotDetails)
                    return RedirectToAction("Index", "Authentication");

                var consoleChoice = console == 0 ? webClient.MembershipType : console;
                ViewBag.Console = consoleChoice;
                ViewBag.DualConsole = webClient.DualAccount;
                
                // Load all possible emblems, maybe rework this to use Plumbing
                var allEmblems = db.InventoryEmblems.Where(ie => !string.IsNullOrEmpty(ie.icon)).ToList();
                var gottenEmblems = new List<InventoryEmblem>();

                // Get kiosk definition which includes ALL emblems
                var vendorsDefinition = webClient.GetPlumbing<Dictionary<long, VendorDefinition>>("DestinyVendorDefinition");
                VendorDefinition kiosk;
                vendorsDefinition.TryGetValue(622587395, out kiosk);

                // If we have a kiosk, lets use this
                if (kiosk != null)
                {
                    var indexes = CalculateGottenEmblemIndexes(webClient.MembershipIds, consoleChoice);
                    if (indexes == null)
                        return RedirectToAction("Index", "Authentication");

                    foreach (var index in indexes)
                    {
                        var item = kiosk.itemList.Single(i => i.vendorItemIndex == index);
                        var emblem = allEmblems.SingleOrDefault(ae => ae.id == item.itemHash);

                        if (emblem != null)
                        {
                            allEmblems.Remove(emblem);
                            gottenEmblems.Add(emblem);
                        }
                    }
                }

                var results = new CompleteTypeResults()
                {
                    GotEmblems = gottenEmblems,
                    NeededEmblems = allEmblems.Where(i => i.obtainable.HasValue && i.obtainable.Value).ToList(),
                    UnobtainableEmblems = allEmblems.Where(i => i.obtainable.HasValue && !i.obtainable.Value).ToList(),
                };

                return View(results);
            }

            return RedirectToAction("Index", "Authentication");

        }

        private List<int> CalculateGottenEmblemIndexes(string[] membershipIds, int consoleChoice)
        {
            var kioskCollection = webClient.RunGetAsync<GetProfileKiosks>($"Platform/Destiny2/{consoleChoice}/Profile/{membershipIds[consoleChoice]}/?components=Kiosks");

            if (kioskCollection.ErrorCode > 1)
                return null;

            return kioskCollection.Response.profileKiosks.data.kioskItems[622587395].Where(ki => ki.canAcquire).Select(ki => ki.index).ToList();
        }
    }
}