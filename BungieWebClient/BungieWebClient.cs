using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BungieWebClient.Model.Authentication;
using BungieWebClient.Model.Character;
using BungieWebClient.Model.Gamertag;
using BungieWebClient.Model.Membership;

namespace BungieWebClient
{
    public class BungieClient
    {
        public const string BungieBaseUri = "https://www.bungie.net/";
        public const string AccessTokenRequest = "Platform/App/GetAccessTokensFromCode/";
        public const string RefreshTokenRequest = "Platform/App/GetAccessTokensFromRefreshToken/";
        public const string AuthenticationCodeRequest = "https://www.bungie.net/en/Application/Authorize/6871";
        private const string ApiKey = "2aa2b040a1904c97b94550eaaabd54ab";

        // live code
        // public const string AuthenticationCodeRequest = "https://www.bungie.net/en/Application/Authorize/11093";
        // private const string ApiKey = "9681c0a6c9f44315bef80e15a4e3b469";
        
        // Public beta
        // public const string AuthenticationCodeRequest = "https://www.bungie.net/en/Application/Authorize/13331";
        // private const string ApiKey = "21efdd9e265349f281a0cf72fd829583";
        private const int Success = 1;
        private string _authCode;
        private string _accessToken;
        private string _refreshToken;
        public string XboxAccountName;
        public string[] XboxCharacterIds;

        public string PsAccountName;
        public string[] PsCharacterIds;
        
        public int MembershipType;
        public string MembershipId;
        public bool DualAccount { get; set; }

        public BungieClient(string accessToken, string refreshToken) : this()
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
        }
        public BungieClient()
        {
            PrepareBungieRequests();
        }

        public string AuthCode
        {
            get { return _authCode; }
            set
            {
                if (value == _authCode) return;
                _authCode = value;
            }
        }

        public HttpClient Client { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PrepareBungieRequests()
        {
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            Client = new HttpClient(handler);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            Client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            Client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);
        }

        /// <summary>
        ///     Makes an HTTP GET request to the specified endpoint
        /// </summary>
        /// <typeparam name="T">Type of object expected</typeparam>
        /// <param name="endpoint">Endpoint which is being accessed</param>
        public T RunGetAsync<T>(string endpoint)
        {
            return RunRequestAsync<T>(endpoint, "GET");
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">Type of object expected</typeparam>
        /// <param name="requestObject">Object to be serialized and sent in the POST request</param>
        /// <param name="endpoint">Endpoint which is being accessed</param>
        public T RunPostAsync<T>(object requestObject, string endpoint)
        {
            return RunRequestAsync<T>(endpoint, "POST", requestObject);
        }

        /// <summary>
        ///     Makes an HTTP Web Request to the specified Bungie.net endpoint.
        /// </summary>
        /// <returns>
        ///     Returns deserialized object of specified type T.
        /// </returns>
        private T RunRequestAsync<T>(string endpoint, string requestType, object postObject = null)
        {
            try
            {
                HttpResponseMessage response = null;
                if (!string.IsNullOrEmpty(_accessToken) && endpoint != "Platform/App/GetAccessTokensFromRefreshToken/")
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                }

                if (requestType == "POST")
                {
                    var json = postObject.ToJson();
                    var requestStringContent = new StringContent(json);
                    response = Client.PostAsync(new Uri(BungieBaseUri + endpoint), requestStringContent).Result;
                    if (response.StatusCode == HttpStatusCode.TemporaryRedirect)
                    {
                        var url = response.Headers.Location;
                        var retryContent = new StringContent(json);
                        response = Client.PostAsync(url, retryContent).Result;
                    }
                }

                else if (requestType == "GET")
                    response = Client.GetAsync(new Uri(BungieBaseUri + endpoint)).Result;

                if (response != null && response.IsSuccessStatusCode)
                {
                    var message = response.Content.ReadAsStringAsync().Result;
                    return message.FromJson<T>();
                }
            }
            catch (Exception ex)
            {
                SendErrorAlert(ex);
                //log
            }
            return default(T);
        }

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

        public AuthenticationResponse ObtainAccessToken()
        {
            var response = RunPostAsync<AuthenticationResponse>(new AccessTokenRequest { Code = AuthCode }, AccessTokenRequest);
            if (response?.ErrorCode == Success)
            {
                _accessToken = response.Response.AccessToken.Value;
                _refreshToken = response.Response.RefreshToken.Value;
                GetUserDetails();
            }
            return response;
        }

        public ResponseBase RefreshAccessToken()
        {
            var response = RunPostAsync<AuthenticationResponse>(new RefreshTokenRequest { refreshToken = _refreshToken }, RefreshTokenRequest);
            if (response?.ErrorCode == Success)
            {
                _accessToken = response.Response.AccessToken.Value;
                _refreshToken = response.Response.RefreshToken.Value;
            }

            //GetUserDetails();

            return response;
        }

        public bool GetUserDetails()
        {
            var response = RunGetAsync<GamertagResponse>("Platform/User/GetBungieNetUser/");
            if (response?.ErrorCode == Success)
            {
                XboxAccountName = response.Response?.GamerTag ?? "";
                PsAccountName = response.Response?.PsnId;

                if (!string.IsNullOrEmpty(XboxAccountName))
                    XboxCharacterIds = RetrieveCharacterDetails(1, XboxAccountName);

                if (!string.IsNullOrEmpty(PsAccountName))
                    PsCharacterIds = RetrieveCharacterDetails(2, PsAccountName);

                if (XboxCharacterIds != null && XboxCharacterIds.Any())
                    MembershipType = 1;
                else if (PsCharacterIds != null && PsCharacterIds.Any())
                    MembershipType = 2;
                else
                    MembershipType = -1;

                if ((XboxCharacterIds != null && XboxCharacterIds.Any()) && (PsCharacterIds != null && PsCharacterIds.Any()))
                    DualAccount = true;

                return true;
            }
            else
            {
                SendErrorAlert(new Exception($"GetUserDetailsFailed - Error Code:{response?.ErrorCode}      Message:{response?.Message}     Access Token:{_accessToken}     Refresh Token:{_refreshToken}"));
                return false;
            }
        }
        
        private string[] RetrieveCharacterDetails(int membershipType, string membershipName)
        {
            var membershipDetails = RunGetAsync<MembershipResponse>($"Platform/Destiny/SearchDestinyPlayer/{membershipType}/{membershipName}/");
            var membershipId = membershipDetails?.Response?.FirstOrDefault();
            if (membershipId == null)
            {
                // this should be use the refresh token but for now lets re-authenticate
                return null;
            }

            MembershipId = membershipId.membershipId;
            MembershipType = membershipId.membershipType;

            var characterDetails = RunGetAsync<CharacterEndpoint>($"Platform/Destiny/{MembershipType}/Account/{membershipId.membershipId}/Summary/");
            var characterIds = characterDetails?.Response?.data?.characters?.Select(c => c.characterBase.characterId).ToArray();
            return characterIds;
        }
    }
}
