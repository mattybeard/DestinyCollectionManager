using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BungieWebClient;

namespace CollectionManagerSite.Controllers
{
    public class AuthenticationController : Controller
    {        
        public BungieClient WebClient { get; set; }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StartAuth()
        {
            return Redirect("https://www.bungie.net/en/Application/Authorize/11093");
        }

        public ActionResult ConfirmAuth(string code)
        {
            if(code == null)
                throw new AccessViolationException();

            WebClient = new BungieClient
            {
                AuthCode = code
            };
            var response = WebClient.ObtainAccessToken();
            var accessCookie = new HttpCookie("BungieAccessToken")
            {
                Value = response.Response.AccessToken.Value,
                Expires = DateTime.Now.AddDays(7)
            };

            var refreshToken = new HttpCookie("BungieRefreshToken")
            {
                Value = response.Response.RefreshToken.Value,
                Expires = DateTime.Now.AddDays(7)
            };

            Response.Cookies.Add(accessCookie);
            Response.Cookies.Add(refreshToken);

            return RedirectToAction("Index", "Home");
        }
    }
}