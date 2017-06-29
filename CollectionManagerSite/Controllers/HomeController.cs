using System.Collections.Generic;
using System.Web.Mvc;
using BungieWebClient;
using CollectionManagerSite.Models;

namespace CollectionManagerSite.Controllers
{
    public class HomeController : Controller
    {
        private BungieClient webClient { get; set; }

        public ActionResult Index(int console = 0)
        {
            var authorised = System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"] != null && System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"] != null;

            if (authorised)
            {
                webClient = new BungieClient(System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"].Value, System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"].Value);
                var response = webClient.RefreshAccessToken();
                if(response.ErrorCode != 1)
                    return RedirectToAction("Index", "Authentication");
                //return View("Maintenance");

                var gotDetails = webClient.GetUserDetails();
                if (webClient.MembershipType == -1 || !gotDetails)
                    return RedirectToAction("Index", "Authentication");

                var consoleChoice = console == 0 ? webClient.MembershipType : console;
                ViewBag.Console = consoleChoice;
                ViewBag.DualConsole = webClient.DualAccount;

                var results = new CompleteTypeResults();
                return View(results);
            }

            return RedirectToAction("Index", "Authentication");
        }
    }
}