using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RinchemApiIntegrationConsole.UiSpecific;

namespace RinchemApiIntegrationConsole
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///  Simple structural class to hold credential data that is necessary to login to Salesforce.               ///
    ///  Also, provides methods to save and load (non password) credentials to an external file.                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Credentials
    {
        // In Salesforce -> My Settings (under username) -> Personal -> Reset My Security Token -> Check Email
        public string SecurityToken     { get; }
        // In Salesforce -> Setup (under username) -> Build -> Create -> Apps -> Connected Apps -> ASN
        public string ConsumerKey       { get; }
        public string ConsumerSecret    { get; }
        // The Username and password used in Salesforce
        public string Username          { get; }
        public string Password          { get; }
        // Are you in sandbox or production --Might want to switch this to a link String instead
        public Boolean IsSandboxUser    { get; }


        // Creates a new Credentials object with the subsequent properties
        public Credentials(String SecurityToken, 
                            String ConsumerKey,
                            String ConsumerSecret,
                            String Username,
                            String Password,
                            Boolean IsSandboxUser)
        {
            this.SecurityToken  = SecurityToken;
            this.ConsumerKey    = ConsumerKey;
            this.ConsumerSecret = ConsumerSecret;
            this.Username       = Username;
            this.Password       = Password;
            this.IsSandboxUser  = IsSandboxUser;

        }
    }
}
