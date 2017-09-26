using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using BungieDatabaseClient;
using BungieDatabaseClient.D2;
using BungieWebClient;
using BungieWebClient.Model.Advisors;
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

                var goData = new CompleteResults()
                {
                    //Info = user,
                    Emblems = db.InventoryEmblems.Where(ie => !string.IsNullOrEmpty(ie.icon)).ToList(),
                    Sparrows = db.InventorySparrows.Where(ie => !string.IsNullOrEmpty(ie.icon)).ToList(),
                    Ships = db.InventoryShips.Where(ie => !string.IsNullOrEmpty(ie.icon)).ToList()
                };

                var consoleChoice = console == 0 ? webClient.MembershipType : console;
                goData.Console = (consoleChoice == 1) ? "Xbox" : "Playstation";
                goData.GamingUsername = (consoleChoice == 1) ? webClient.XboxAccountName : webClient.PsAccountName;

                ViewBag.Console = consoleChoice;
                ViewBag.DualConsole = webClient.DualAccount;

                //var vendorDetailsC = webClient.RunGetAsync<string>($"Platform/Destiny2/1/Profile/4611686018431904749/?components=Kiosks");
                //var vendorDetailsP = webClient.RunGetAsync<string>($"Platform/Destiny2/1/Profile/4611686018431904749/Character/2305843009261356818/?components=500");

                return View(goData);
            }

            return RedirectToAction("Index", "Authentication");

        }
    }
}