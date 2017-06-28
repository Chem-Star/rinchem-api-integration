using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using RinchemApiIntegrationConsole.UiSpecific;
using System.Configuration;
using System.Collections.Generic;
using RinchemApiIntegrator.ApiSpecific;

namespace RinchemApiIntegrationConsole
{
    internal class SalesForceConnection
    {
        private APImanager apiManager;
        private Credentials credentials;
        private string CustomAPI    = ConfigurationManager.AppSettings["CustomAPI"];

        private AuthenticationClient auth;
        private HttpRequestMessage request;

        private Boolean isConnected = false;

        public SalesForceConnection(APImanager apiManager, Credentials credentials)
        {
            this.apiManager = apiManager;
            this.credentials = credentials;

            // Sets the security protocol for all ServicePoint objects managed by the ServicePointManager
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public Boolean isConnectedWithCredentials(Credentials newCredentials)
        {
            if (!isConnected) return false;

            if (credentials.Username != newCredentials.Username)                return false;
            if (credentials.Password != newCredentials.Password)                return false;
            if (credentials.SecurityToken != newCredentials.SecurityToken)      return false;
            if (credentials.ConsumerKey != newCredentials.ConsumerKey)          return false;
            if (credentials.ConsumerSecret != newCredentials.ConsumerSecret)    return false;
            if (credentials.IsSandboxUser != newCredentials.IsSandboxUser)      return false;

            return true;
        }

        public async Task<Boolean> tryToConnect()
        {
            // Uses DeveloperForce.Force nuget package -- a force.com toolkit for .net 
            // UPDATE: Migrated required functions to local AuthenticationClient.cs
            // Used to login to SalesForce
            auth = new AuthenticationClient();

            // Authenticate with Salesforce
            ConsoleLogger.log("Authenticating with Salesforce");

            // Check if we are using a sandbox or dev (value is saved as a string not a boolean so we
            //  tell it to ignore text case in the second parameter) Then update the urlContent link
            //  accordingly.
            var urlContent = credentials.IsSandboxUser
                ? "https://test.salesforce.com/services/oauth2/token"
                : "https://login.salesforce.com/services/oauth2/token";

            //  Try to connect to the salesforce service. If there is an error, it will be caught in 
            try
            {
                await auth.UsernamePasswordAsync(credentials.ConsumerKey,
                                            credentials.ConsumerSecret,
                                            credentials.Username,
                                            credentials.Password+credentials.SecurityToken,
                                            urlContent);

            }
            catch (Exception e)
            {
                ConsoleLogger.log(e.ToString());
                return false;
            }

            isConnected = true;
            return true;
        }


        public Boolean tryApiSetup( DataObject obj , String httpVerb, String apiType)
        {
            string CustomAPI = "";
            switch (apiType)
            {
                case "ASN":
                    CustomAPI = "/services/apexrest/v1/ASN__c";
                    break;
                case "OBO":
                    CustomAPI = "/services/apexrest/v1/OBO__c";
                    break;
                default:
                    ConsoleLogger.log("Couldn't find the specified API type");
                    return false;
            }

            //  auth.* tokens are populated subsequent to a succesful UserNamePasswrdAsync() call
            //  Used to access SalesForce on the now open connection
            string oauthToken = auth.AccessToken;
            string serviceUrl = auth.InstanceUrl;

            HttpContent content = null;
            if (httpVerb != "GET") 
            {
                //  Convert our asnObject into a json object string
                string requestMessage = JsonConvert.SerializeObject(obj);
                //  Convert our new JSON String into an HttpContent format that can actually be transmitted
                //  Third parameter is stored as a 'MediaType'
                content = new StringContent(requestMessage, Encoding.UTF8, "application/json");
            } else
            {  //GET requests append to the url and leave an empty body
                String action = apiManager.getCurrentApiAction();
                if (action == "GETBYNAME") CustomAPI += "?name=" + obj.getObjectName();
                else if (action == "GETBYQUERY") CustomAPI += "?query=" + apiManager.getQueryString();
            }

            //  Create a new identifier based on our API path and our SalesForce instance URL
            string uri = serviceUrl + CustomAPI;
            //  Create request message associated with the desired verb
            HttpMethod httpMethod = new HttpMethod(httpVerb);
            request = new HttpRequestMessage(httpMethod, uri);

            //  Add token to header
            request.Headers.Add("Authorization", "Bearer " + oauthToken);
            //  Allow xml to be returned to the caller with the same 'MediaType' that was specified in our HttpContent
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //  Set the requests content that we will send
            request.Content = content;


            return true;
        }

        public async Task<Boolean> tryApiCall()
        {
            //  Create a new HttpClient to handle our open connection requests and responses
            HttpClient createClient = new HttpClient();

            //  Try the actual API call. Should return the content string right back. If there is an error we print
            //  the exception.
            try
            {
                //  Try the API call, and hopefully receive a response
                HttpResponseMessage response = await createClient.SendAsync(request);
                
                //  Convert the Content of the response to a String
                string result = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(result);
                //ConsoleLogger.log(result);

                try
                {
                    SalesForceResponse resultObject = JsonConvert.DeserializeObject<SalesForceResponse>(result);
                    apiManager.setResponse(resultObject);

                    ConsoleLogger.log("|| Status: "+resultObject.status);
                    ConsoleLogger.log("|| Message: "+resultObject.message);
                    if (resultObject.asn_order_num != null)
                    {
                        ConsoleLogger.log("|| ASN Order Num: " + resultObject.asn_order_num);
                        //ConsoleLogger.log(JsonConvert.SerializeObject(resultObject.asn));
                    }
                    if (resultObject.obo_order_num != null)
                    {
                        ConsoleLogger.log("|| OBO Order Num: " + resultObject.obo_order_num);
                        //ConsoleLogger.log(JsonConvert.SerializeObject(resultObject.obo));
                    }
                    //resultObject.lineItems.ForEach(x => ConsoleLogger.log(JsonConvert.SerializeObject(x)));
                    if (resultObject.status == "Error")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch
                {
                    ConsoleLogger.log(result);
                    return false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                var innerException = e.InnerException;
                while (innerException != null)
                {
                    Console.WriteLine(innerException.Message);
                    Console.WriteLine(innerException.StackTrace);

                    innerException = innerException.InnerException;
                }

                Console.ReadLine(); // Prevent the app from closing so the error can be seen
                return false;
            }
        }
    }

    public class SalesForceResponse
    {
        public String status;               //Returned by any API call
        public String message;              //Returned by any API call
        public String asn_order_num;        //Returned by a successful ASN, POST or PATCH call
        public String obo_order_num;        //Returned by a successful OBO, POST or PATCH call
        public List<Object> lineItems;      //Returned by a successful ASN or OBO, POST or PATCH call
        public Object asn;                  //Returned by a successful ASN, POST or PATCH call
        public Object obo;                  //Returned by a successful ASN, POST or PATCH call

        public List<ASN.Request> asns;      //Returned by a successful ASN, GET call

    }

}