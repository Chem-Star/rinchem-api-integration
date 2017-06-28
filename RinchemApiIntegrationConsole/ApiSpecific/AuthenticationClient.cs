using Newtonsoft.Json;
using RinchemApiIntegrationConsole.UiSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RinchemApiIntegrator.ApiSpecific
{

    /// <summary>
    /// CODE COMPILED FROM FORCE.COM GITHUB PAGE
    /// https://github.com/developerforce/Force.com-Toolkit-for-NET/blob/master/src/CommonLibrariesForNET/AuthenticationClient.cs
    /// https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/src/CommonLibrariesForNET/Models/Json
    /// </summary>
    public class AuthenticationClient
    {
        public string InstanceUrl { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Id { get; set; }
        public string ApiVersion { get; set; }

        private readonly HttpClient _httpClient;
        private const string UserAgent = "forcedotcom-toolkit-dotnet";


        public AuthenticationClient() : this(new HttpClient())
        {
        }
        public AuthenticationClient(HttpClient httpClient)
        {
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            _httpClient = httpClient;
            ApiVersion = "v36.0";
        }

        public async Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password, string tokenRequestEndpointUrl)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId");
            if (string.IsNullOrEmpty(clientSecret)) throw new ArgumentNullException("clientSecret");
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password");
            if (string.IsNullOrEmpty(tokenRequestEndpointUrl)) throw new ArgumentNullException("tokenRequestEndpointUrl");
            if (!Uri.IsWellFormedUriString(tokenRequestEndpointUrl, UriKind.Absolute)) throw new FormatException("tokenRequestEndpointUrl");

            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
                });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(tokenRequestEndpointUrl),
                Content = content
            };

            request.Headers.UserAgent.ParseAdd(string.Concat(UserAgent, "/", ApiVersion));

            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            ConsoleLogger.log(response);

            if (responseMessage.IsSuccessStatusCode)
            {
                var authToken = JsonConvert.DeserializeObject<AuthToken>(response);

                AccessToken = authToken.AccessToken;
                InstanceUrl = authToken.InstanceUrl;
                Id = authToken.Id;
            }
            else
            {
                //var errorResponse = JsonConvert.DeserializeObject<AuthErrorResponse>(response);
                throw new Exception(response);
            }
        }

        public class AuthToken
        {
            [JsonProperty(PropertyName = "id")]
            public string Id;

            [JsonProperty(PropertyName = "issued_at")]
            public string IssuedAt;

            [JsonProperty(PropertyName = "instance_url")]
            public string InstanceUrl;

            [JsonProperty(PropertyName = "signature")]
            public string Signature;

            [JsonProperty(PropertyName = "access_token")]
            public string AccessToken;

            [JsonProperty(PropertyName = "refresh_token")]
            public string RefreshToken;
        }

        public class AuthErrorResponse
        {
            [JsonProperty(PropertyName = "error_description")]
            public string ErrorDescription;

            [JsonProperty(PropertyName = "error")]
            public string Error;
        }
    }
}
