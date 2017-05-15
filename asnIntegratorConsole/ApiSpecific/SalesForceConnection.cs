﻿using System;
using System.Net;
//requires DeveloperForce.Force and DeveloperForce.Chatter nuget package
using Salesforce.Common;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using asnIntegratorConsole.UiSpecific;
using System.Configuration;
using System.Collections.Generic;

namespace asnIntegratorConsole
{
    internal class SalesForceConnection
    {
        private Credentials credentials;
        private string CustomAPI    = ConfigurationManager.AppSettings["CustomAPI"];
        private string httpVerb     = ConfigurationManager.AppSettings["httpVerb"];

        private AuthenticationClient auth;
        private HttpRequestMessage request;

        public SalesForceConnection(Credentials credentials)
        {
            this.credentials = credentials;

            // Sets the security protocol for all ServicePoint objects managed by the ServicePointManager
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public async Task<Boolean> tryToConnect()
        {
            // Uses DeveloperForce.Force nuget package -- a force.com toolkit for .net 
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
            //  the try/catch block in Main()
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

            return true;
        }


        public Boolean tryApiSetup( AsnObject asn)
        {
            //  auth.* tokens are populated subsequent to a succesful UserNamePasswrdAsync() call
            //  Used to access SalesForce on the now open connection
            string oauthToken = auth.AccessToken;
            string serviceUrl = auth.InstanceUrl;

            //  Convert our asnObject into a json object string
            string requestMessage = JsonConvert.SerializeObject(asn);

            //  Convert our new JSON String into an HttpContent format that can actually be transmitted
            //  Third parameter is stored as a 'MediaType'
            HttpContent content = new StringContent(requestMessage, Encoding.UTF8, "application/json");
            //  Create a new identifier based on our API path and our SalesForce instance URL
            string uri = serviceUrl + CustomAPI;


            //  Create request message associated with POST verb
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
                Console.WriteLine(result);

                try
                {
                    SalesForceResponse resultObject = JsonConvert.DeserializeObject<SalesForceResponse>(result);
                    ConsoleLogger.log(resultObject.status);
                    ConsoleLogger.log(resultObject.message);
                    ConsoleLogger.log(JsonConvert.SerializeObject(resultObject.asn));
                    resultObject.lineItems.ForEach(x => ConsoleLogger.log(JsonConvert.SerializeObject(x)));
                    if (resultObject.status == "Error") return false;
                    else return true;
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
        public String status;
        public String message;
        public List<Object> lineItems;
        public Object asn;
    }

}