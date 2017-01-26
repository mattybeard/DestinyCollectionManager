using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
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
        private static AdvisorsEndpoint EvaCache { get; set; }
        private static AdvisorsEndpoint AmandaCache { get; set; }
        private static AdvisorsEndpoint PetraCache { get; set; }
        private static AdvisorsEndpoint VanguardQuartermasterCache { get; set; }
        private static AdvisorsEndpoint CrucibleQuartermasterCache { get; set; }
        private static DateTime cacheExpiry { get; set; }
        private bool CacheExpired
        {
            get
            {
                if (cacheExpiry == null || EvaCache == null || AmandaCache == null || PetraCache == null || VanguardQuartermasterCache == null || CrucibleQuartermasterCache == null)
                    return true;

                if (cacheExpiry < DateTime.Now)
                    return true;

                if (DateTime.Now.Hour == 10 && DateTime.Now.Minute < 30)
                    return true;

                return false;
            }
        }

        private BungieClient webClient { get; set; }

        public ActionResult Index(int console = 0)
        {
            var authorised = System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"] != null && System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"] != null;

            if (authorised)
            {                
                webClient = new BungieClient(System.Web.HttpContext.Current.Request.Cookies["BungieAccessToken"].Value, System.Web.HttpContext.Current.Request.Cookies["BungieRefreshToken"].Value);
                webClient.RefreshAccessToken();
                webClient.GetUserDetails();

                if (webClient.MembershipType == -1)
                    return RedirectToAction("Index", "Authentication");

                var consoleChoice = console == 0 ? webClient.MembershipType : console;
                var characterIds = consoleChoice == 1 ? webClient.XboxCharacterIds : webClient.PsCharacterIds;
                ViewBag.Console = consoleChoice;
                ViewBag.DualConsole = webClient.DualAccount;
                
                var shaders = GetVendorItems(characterIds, "Shader", 2420628997, consoleChoice);
                var emblems = GetVendorItems(characterIds, "Emblem", 3301500998, consoleChoice);
                var ships = GetVendorItems(characterIds, "Ship", 2244880194, consoleChoice);
                var sparrows = GetVendorItems(characterIds, "Sparrow", 44395194, consoleChoice);

                var results = new Dictionary<string, Dictionary<string, List<MissingItemModel>>>();
                results.Add("Shaders", new Dictionary<string, List<MissingItemModel>>() { { "Needed", shaders } });
                results.Add("Emblems", new Dictionary<string, List<MissingItemModel>>() { { "Needed", emblems } });
                results.Add("Ships", new Dictionary<string, List<MissingItemModel>>() { { "Needed", ships } });
                results.Add("Sparrows", new Dictionary<string, List<MissingItemModel>>() { { "Needed", sparrows } });

                if (CacheExpired)
                {
                    EvaCache = GetVendorMetadata(134701236);
                    AmandaCache = GetVendorMetadata(459708109);
                    PetraCache = GetVendorMetadata(1410745145);
                    VanguardQuartermasterCache = GetVendorMetadata(2668878854);
                    CrucibleQuartermasterCache = GetVendorMetadata(3658200622);

                    cacheExpiry = DateTime.Now.AddMinutes(30);
                }


                results["Shaders"].Add("ForSale", GetCurrentlyForSale(EvaCache, shaders, "Shaders", "Shaders"));
                results["Shaders"]["ForSale"].AddRange(GetCurrentlyForSale(PetraCache, shaders, "Queen's Wrath: Rank 2", "Shaders"));
                results["Shaders"]["ForSale"].AddRange(GetCurrentlyForSale(PetraCache, shaders, "Queen's Wrath: Rank 3", "Shaders"));

                results["Emblems"].Add("ForSale", GetCurrentlyForSale(EvaCache, emblems, "Emblems", "Emblems"));
                results["Emblems"]["ForSale"].AddRange(GetCurrentlyForSale(PetraCache, emblems, "Queen's Wrath: Rank 1", "Emblems"));

                results["Ships"].Add("ForSale", GetCurrentlyForSale(AmandaCache, ships, "Ship Blueprints", "Ships"));

                results["Sparrows"].Add("ForSale", GetCurrentlyForSale(AmandaCache, sparrows, "Vehicles", "Sparrows"));
                results["Sparrows"]["ForSale"].AddRange(GetCurrentlyForSale(VanguardQuartermasterCache, sparrows, "Vehicles", "Sparrows"));
                results["Sparrows"]["ForSale"].AddRange(GetCurrentlyForSale(CrucibleQuartermasterCache, sparrows, "Vehicles", "Sparrows"));

                // Do we have any currently being sold?!   
                //currentlyListed.AddRange(GetCurrentlyForSale(petraItems, emblems, "Queen's Wrath: Rank 1", "Petra Emblems"));
                //currentlyListed.AddRange(GetCurrentlyForSale(petraItems, shaders, "Queen's Wrath: Rank 2", "Petra Shaders"));
                //currentlyListed.AddRange(GetCurrentlyForSale(petraItems, shaders, "Queen's Wrath: Rank 3", "Petra Shaders"));

                //if (shaders == null || emblems == null)
                //return RedirectToAction("Index", "Authentication");

                return View(results);
            }

            return RedirectToAction("Index", "Authentication");
        }

        private DateTime GetExpiryTime()
        {
            throw new NotImplementedException();
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
                Faction = i.Faction,
                Name = i.Name,
                Icon = i.Icon,
                SecondaryIcon = i.SecondaryIcon ?? "",
                UnlockHashes = i.UnlockHashes,
                Hash = i.Hash,          
            }).ToList();
        }

        private AdvisorsEndpoint GetVendorMetadata(long vendorId)
        {
            var vendorDetails = webClient.RunGetAsync<AdvisorsEndpoint>($"Platform/Destiny/Vendors/{vendorId}/Metadata/");

            return vendorDetails;
        }

        //private string[] RetriveCharacterDetails()
        //{
        //    if(webClient.MembershipType == 0 && webClient.AccountName == null)
        //        SendErrorAlert(new Exception($"Error retrieving details - no account or authcode. {webClient.AccountName}{webClient.AuthCode}"));

        //    var membershipDetails = webClient.RunGetAsync<MembershipResponse>($"Platform/Destiny/SearchDestinyPlayer/{webClient.MembershipType}/{webClient.AccountName}/");
        //    var _membershipId = membershipDetails?.Response?.FirstOrDefault();
        //    if (_membershipId == null)
        //    {
        //        // this should be use the refresh token but for now lets re-authenticate
        //        return null;
        //    }
            
        //    webClient.MembershipType = _membershipId.membershipType;

        //    var characterDetails = webClient.RunGetAsync<CharacterEndpoint>($"Platform/Destiny/{webClient.MembershipType}/Account/{_membershipId.membershipId}/Summary/");
        //    var _characterIds = characterDetails.Response.data.characters.Select(c => c.characterBase.characterId).ToArray();

        //    return _characterIds;
        //}

        private void SendErrorAlert(Exception exception)
        {
            MailMessage msg = new MailMessage();

            msg.From = new MailAddress("mattybeard@gmail.com");
            msg.To.Add("mattybeard@gmail.com");
            msg.Subject = "GO Exception";
            msg.Body = exception.ToString();
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential("mattybeard@gmail.com", "Baxter2242");
            client.Timeout = 20000;
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                msg.Dispose();
            }
        }

        private List<MissingItemModel> GetVendorItems(string[] characterIds, string type, long vendorId, int membershipType)
        {
            var itemsNeeded = new Dictionary<string, List<SaleItem>>();
            foreach (var character in characterIds)
            {
                var itemsCollection = webClient.RunGetAsync<VendorPlatformResponse>($"Platform/Destiny/{membershipType}/MyAccount/Character/{character}/Vendor/{vendorId}/");
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
                            Faction = group.Key,
                            Hash = inventoryItem.Response.data.inventoryItem.itemHash,
                            Name = inventoryItem.Response.data.inventoryItem.itemName,
                            Icon = inventoryItem.Response.data.inventoryItem.icon,
                            SecondaryIcon = inventoryItem.Response.data.inventoryItem.secondaryIcon ?? "",
                            UnlockHashes = item.unlockStatuses.Select(i => i.unlockFlagHash)
                        };

                        results.Add(result);
                    }
                }
            }

            return results;
        }
    }
}