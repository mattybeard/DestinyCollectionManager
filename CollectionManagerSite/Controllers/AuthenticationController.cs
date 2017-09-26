using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BungieDatabaseClient.D2;
using BungieWebClient;
using CollectionManagerSite.Models.Authentication;

namespace CollectionManagerSite.Controllers
{
    public class AuthenticationController : Controller
    {        
        public BungieClient WebClient { get; set; }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                var db = new DestinyDaily2Entities();
                var userInfo = db.UserInfos.FirstOrDefault(u => u.username == user.UserName);
                if (userInfo == null)
                {
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
                else
                {
                    if (user.IsValid(userInfo, user.Password))
                    {
                        FormsAuthentication.SetAuthCookie(user.UserName, user.RememberMe);

                        if (string.IsNullOrEmpty(userInfo.BungieAccessToken) || string.IsNullOrEmpty(userInfo.BungieRefreshToken))
                            return RedirectToAction("Index", "Authentication");

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Login data is incorrect!");
                    }
                }
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            ViewBag.Debug = false;
            ViewBag.BungieAuthURL = BungieClient.AuthenticationCodeRequest;

            return View();
        }

        public ActionResult ConfirmAuth(string code)
        {
            if (code == null)
                throw new AccessViolationException();

            var cookieName = FormsAuthentication.FormsCookieName;
            var authCookie = HttpContext.Request.Cookies[cookieName];
            if (authCookie == null)
                return RedirectToAction("Login", "Authentication");

            var ticket = FormsAuthentication.Decrypt(authCookie.Value);
            var db = new DestinyDaily2Entities();
            var user = db.UserInfos.FirstOrDefault(u => u.username == ticket.Name);

            if (user == null)
                return RedirectToAction("Login", "Authentication");

            WebClient = new BungieClient
            {
                AuthCode = code
            };
            var response = WebClient.ObtainAccessToken();
            user.BungieAccessToken = response.Response.AccessToken.Value;
            user.BungieRefreshToken = response.Response.RefreshToken.Value;
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}