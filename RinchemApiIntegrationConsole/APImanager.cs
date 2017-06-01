using RinchemApiIntegrationConsole.ASN;
using RinchemApiIntegrationConsole.OBO;
using RinchemApiIntegrationConsole.UiSpecific;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace RinchemApiIntegrationConsole
{
    /// <summary>
    /// Bridgeing class between the UI and functional calls
    /// Everything aside from Credentials should intermediately run through here
    /// </summary>
    public class APImanager
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// REGISTER ANY CUSTOM DATA LOADERS HERE
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static List<DataLoader> AsnDataLoaders = new List<DataLoader>() { new AsnRinchemExcelLoader(), new AsnRinchemJsonLoader() };
        private static List<DataLoader> OboDataLoaders = new List<DataLoader>() { new OboRinchemExcelLoader(), new OboRinchemJsonLoader() };

        private DataLoader dataLoader { get; set; }                 // Interface responsible for loading and converting data
        private DataObject dataObject { get; set; }                 // Interface responsible for defining the model type

        private static Profiles profiles { get; set; }              // Contains all prior saved Profiles

        private SalesForceConnection sfConnection { get; set; }     // Manages the salesforce connection
        private Profile profile { get; set; }                       // The profile that we use to connect to salesforce
        private String httpVerb { get; set; }                       // The verb we use during the API call
        private String apiType { get; set; }                        // The api that we are interested in calling

        private Boolean areCredentialsVerified = false;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor -- needs ui so that it can update the logbox
        public APImanager()
        {
            profiles = new Profiles();
            dataLoader = AsnDataLoaders[0];
            httpVerb = "POST";
            apiType = "ASN";
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // DATA LOADERS
        public List<DataLoader> getAllDataLoaders()
        {
            List<DataLoader> dataLoaders = new List<DataLoader>();
            dataLoaders.AddRange(AsnDataLoaders);
            dataLoaders.AddRange(OboDataLoaders);
            return dataLoaders;
        }
        public List<DataLoader> getApiDataLoaders()
        {
            switch (apiType)
            {
                case "ASN":
                    return AsnDataLoaders;
                case "OBO":
                    return OboDataLoaders;
                default:
                    Console.WriteLine("Couldn't find the specified API");
                    return null;
            }
        }
        public DataLoader getCurrentDataLoader()
        {
            return dataLoader;
        }
        public void setCurrentDataLoader(String typeName)
        {
            dataLoader = getAllDataLoaders().Find(x => x.GetUniqueName() == typeName);
            if (dataLoader == null) ConsoleLogger.log("Couldn't find the specified DataLoader" + typeName);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // CREDENTIALS
        public void setAreCredentialsVerified(Boolean val)
        {
            areCredentialsVerified = val;
        }
        public Boolean getAreCredentialsVerified()
        {
            return areCredentialsVerified;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // PROFILES
        //Edit or Save
        public void saveProfile(Profile profile)
        {
            profiles.saveProfile(profile);
        }
        public void deleteProfile(String profilename)
        {
            profiles.deleteProfile(profilename);
        }

        //Get
        public List<Profile> getProfiles()
        {
            return profiles.PROFILES;
        }
        
        public Profile getProfileById(String uid)
        {
            return profiles.findProfile1(uid);
        }
        public Profile getProfileByName(String name)
        {
            return profiles.findProfileByName(name);
        }
        public Profile getCurrentProfile()
        {
            return profile;
        }

        //Set
        public void setCurrentProfile(String profileName)
        {
            profile = getProfileByName(profileName);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // API Setup Specific
        public void setVerb(String verb)
        {
            httpVerb = verb;
        }
        public void setApiType(String api)
        {
            apiType = api;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// FUNCTIONAL METHODS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        // Try to login to salesforce with the specified credentials
        public async Task<Boolean> testCredentials(String password)
        {
            Credentials credentials = new Credentials(
                                    profile.SecurityToken,
                                    profile.ConsumerKey,
                                    profile.ConsumerSecret,
                                    profile.Username,
                                    password,
                                    profile.IsSandboxUser);

            if (sfConnection != null && sfConnection.isConnectedWithCredentials(credentials)) return true;
            
            sfConnection = new SalesForceConnection(credentials);
            bool success = await sfConnection.tryToConnect();
            if (success)
            {
                ConsoleLogger.log("Connected to Salesforce successfully");
            } else
            {
                ConsoleLogger.log("Failed to connect to Salesforce");
            }
            return success;
        }

        // Try to load the raw object data with the current data loader
        public async Task<Boolean> testLoadData()
        {
            bool success = await dataLoader.LoadData();
            if (success) success = dataLoader.TestData();
            if (success)
            {
                ConsoleLogger.log("Loaded the data successfully");
            }
            else
            {
                ConsoleLogger.log("Failed to load data");
            }
            return success;
        }

        // Try to convert the raw object data to a DataObject
        public Boolean testConvertData()
        {
            dataObject = dataLoader.ConvertDataToObject();
            bool success = (dataObject != null) ? dataObject.validate() : false;

            if (success)
            {
                ConsoleLogger.log("Converted the data successfully");
            }
            else
            {
                ConsoleLogger.log("Failed to convert the data");
            }

            return success;
        }

        // Try to serialize the DataObject and open the HTTP connection
        public Boolean testApiSetup()
        {
            bool success = (sfConnection == null || dataObject == null) ? false : true;
            if(success) sfConnection.tryApiSetup(dataObject, httpVerb, apiType);

            if (success)
            {
                ConsoleLogger.log("API request message set up successfully");
            }
            else
            {
                ConsoleLogger.log("Failed to setup API request message");
            }

            return success;
        }

        // Try to send the actual API call to Salesforce
        public async Task<Boolean> testApiCall()
        {
            bool success = await sfConnection.tryApiCall();

            if (success)
            {
                ConsoleLogger.log("Successful API call!");
            }
            else
            {
                ConsoleLogger.log("Failed API call");
            }

            return success;
        }
    }
}
