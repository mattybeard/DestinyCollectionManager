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
        public ActionResult Index(int? debug = 0)
        {
            ViewBag.Debug = false;

            if (debug == 1)
            {
                ViewBag.Debug = true;
                ViewBag.AccessToken = System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"] == null ? "" : System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"].Value;
                ViewBag.RefreshToken = System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"] == null ? "" : System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"].Value;
                ViewBag.Status = System.Web.HttpContext.Current.Request.Cookies["GOStatus"] == null ? "" : System.Web.HttpContext.Current.Request.Cookies["GOStatus"].Value;
            }
            return View();
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