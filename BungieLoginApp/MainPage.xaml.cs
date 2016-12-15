using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BungieLoginApp.DAL;
using BungieLoginApp.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BungieLoginApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int Success = 1;
        private string _membershipId;
        private int _membershipType;
        private string[] _characterIds;


        public MainPage()
        {
            InitializeComponent();
            Status.Text = "Not Logged In";
            BungieClient.Instance.PropertyChanged += BungieClient_PropertyChanged;
        }

        private async void BungieClient_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BungieClient.AuthCode))
            {
                var result = await BungieClient.Instance.ObtainAccessToken();
                Status.Text = result?.ErrorCode == Success ? "Logged In" : "Not Logged In";

                RetriveCharacterDetails();
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(BungieClient.AuthenticationCodeRequest));
        }

        private async void Print_Click(object sender, RoutedEventArgs e)
        {
            var result = await BungieClient.Instance.RunGetAsync<GamertagResponse>("Platform/User/GetBungieNetUser/");
            if (result?.ErrorCode == Success)
            {
                PsnText.Text = result.Response?.PsnId ?? "None";
                XboxText.Text = result.Response?.GamerTag ?? "None";
            }
            else
                PsnText.Text = "error";
        }

        private async void RetriveCharacterDetails()
        {
            var membershipDetails = await BungieClient.Instance.RunGetAsync<MembershipResponse>($"Platform/Destiny/SearchDestinyPlayer/1/mbeard/");
            _membershipId = membershipDetails?.Response?.First().membershipId;
            _membershipType = (int) membershipDetails?.Response?.First().membershipType;

            var characterDetails = await BungieClient.Instance.RunGetAsync<CharacterEndpoint>($"Platform/Destiny/{_membershipType}/Account/{_membershipId}/Summary/");
            _characterIds = characterDetails.Response.data.characters.Select(c => c.characterBase.characterId).ToArray();
        }

        private async void Emblems_Click(object sender, RoutedEventArgs e)
        {
            var emblemsNeeded = new Dictionary<string, List<long>>();

            foreach (var character in _characterIds)
            {
                var emblemsCollection = await BungieClient.Instance.RunGetAsync<VendorPlatformResponse>($"Platform/Destiny/{_membershipType}/MyAccount/Character/{character}/Vendor/2420628997/");
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

            foreach (var group in emblemsNeeded)
            {
                if (group.Value.Any())
                {
                    ResultsTest.Items.Add($"{group.Key} cateory");
                    foreach (var emblem in group.Value)
                    {
                        var itemResult = await BungieClient.Instance.RunGetAsync<InventoryItemPlatformResponse>($"/Platform/Destiny/Manifest/InventoryItem/{emblem}/");
                        ResultsTest.Items.Add($"{itemResult.Response.data.inventoryItem.itemName} still needed");
                    }
                }
            }
        }
    }
}
