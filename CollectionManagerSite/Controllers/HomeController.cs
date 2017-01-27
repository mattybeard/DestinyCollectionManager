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
using Item = BungieWebClient.Model.Character.Item;
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

        private void AddAdditionalEmblems(Dictionary<string, List<SaleItem>> itemsNeeded)
        {
            var community = new Tuple<string, long[]>("Community", new long[] {1592588002});
            var promotional = new Tuple<string, long[]>("Promotional", new long[] { 1443409301, 1443409300, 3253299356, 3253299357, 2686650663, 1409854025, 1409854024,
                                                                                            1426631718, 1426631716, 1426631717, 1443409298, 1426631713, 1426631719, 1426631722, 1426631723,
                                                                                            1443409303, 1443409302, 3580794342 });
            var unclassified = new Tuple<string, long[]>("Unclassified", new long[] {  1325965972, 1325965971, 1325965977, 1325965973, 70035233, 1443409299,
                                                                                            1342743555, 1443409308, 1409854029, 1592588001, 1342743553,  1592588000, 1426631714,
                                                                                            1592588012, 1592588002, 1592588004, 1592588005, 1592588006, 1443409297, 1443409296,
                                                                                             1409854023, 1592588003,  1409854030, 1426631715, 1325965969, 3983828201 });

            var raid = new Tuple<string, long[]>("Raid", new long[] { 3269301481, 2372257459, 2372257458, 2372257457, 2372257456, 2372257463, 185564349, 185564348, 185564351, 185564350, 185564345 });
            var srl = new Tuple<string, long[]>("SRL", new long[] { 1777175508 });
            var holiday = new Tuple<string, long[]>("Holiday", new long[] { 3347001814 });
            var riseOfIron = new Tuple<string, long[]>("Rise of Iron", new long[] { 3983828200, 3659569693 });


            ConvertGroupToItems(itemsNeeded, community);
            ConvertGroupToItems(itemsNeeded, promotional);
            ConvertGroupToItems(itemsNeeded, unclassified);
            ConvertGroupToItems(itemsNeeded, raid);
            ConvertGroupToItems(itemsNeeded, srl);
            ConvertGroupToItems(itemsNeeded, holiday);
            ConvertGroupToItems(itemsNeeded, riseOfIron);

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
                saleItem.item = new Item() {itemHash = hash};
                saleItem.unlockStatuses = new List<UnlockStatus>();

                salesItems.Add(saleItem);
            }
            return salesItems;
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
            if (type == "Emblem")
            {
                // Here we should add the custom ones in, so we can suppress if needed.
                AddAdditionalEmblems(itemsNeeded);
            }

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

                    if (itemsNeeded.ContainsKey("Unclassified"))
                    {
                        var unlocked = category.saleItems.Where(i => !i.unlockStatuses.Any() || i.unlockStatuses.All(s => s.isSet)).Select(i => i.item.itemHash);
                        itemsNeeded["Unclassified"].RemoveAll(i => unlocked.Contains(i.item.itemHash));
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