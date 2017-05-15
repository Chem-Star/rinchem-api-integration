using asnIntegratorConsole.UiSpecific;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace asnIntegratorConsole
{
    /// <summary>
    /// Bridgeing class between the UI and functional calls
    /// Everything aside from Credentials should intermediately run through here
    /// </summary>
    class APImanager
    {
        private List<DataLoader> dataLoaders = new List<DataLoader>() { new RinchemJsonLoader(), new RinchemExcelLoader() };

        private Profiles profiles { get; set; }                     // Contains all prior saved Profiles
        private Credentials credentials { get; set; }               // The login details used to connect to SalesForce
        private SalesForceConnection sfConnection { get; set; }     // Manages the salesforce connection
        private DataLoader dataLoader { get; set; }                 // Interface responsible for loading and converting data ***USER MUST IMPLEMENT THEIR OWN***
        private AsnObject asnObject { get; set; }                   // Object containing all information to be sent in the ASN API call


        // Constructor -- needs ui so that it can update the logbox
        public APImanager()
        {
            profiles = new Profiles();
        }

        // Array of all potential data loaders
        public List<DataLoader> getDataLoaders()
        {
            return dataLoaders;
        }
        // Returns the DataLoader object
        public DataLoader getDataLoader()
        {
            return dataLoader;
        }
        // Get the specified data loader
        public void setDataLoader(String typeName)
        {
            dataLoader = dataLoaders.Find(x => x.GetType().ToString() == typeName);
            if (dataLoader == null) ConsoleLogger.log("Couldn't find the specified DataLoader" + typeName);
        }

        // Sets new credentials for the salesforce login
        public void setCredentials(Credentials credentials)
        {
            this.credentials = credentials;
        }
        // Saves a new profile
        public void saveProfile(Profile profile)
        {
            profiles.saveProfile(profile);
        }
        // Deletes a profile
        public void deleteProfile(String profilename)
        {
            profiles.deleteProfile(profilename);
        }
        // Returns the currently loaded profiles
        public List<Profile> getProfiles()
        {
            return profiles.PROFILES;
        }
        // Sets the path to the profiles.json file
        public void setProfilesPath(String filepath)
        {
            profiles.setProfilesPath(filepath);
        }
        // Sets the path to the profiles.json file
        public String getProfilesPath()
        {
            return profiles.getProfilesPath();
        }
        // Returns the profile with the give uid
        public Profile getProfile(String uid)
        {
            return profiles.findProfile(uid);
        }

        // Trys to login to salesforce with the currently loaded credentials
        public async Task<Boolean> testCredentials()
        {
            if (credentials == null) return false;
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

        // Trys to load the raw asn data
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

        // Trys to convert the raw asn data to an AsnObject
        public Boolean testConvertData()
        {
            asnObject = dataLoader.ConvertDataToAsnObject();
            bool success = (asnObject != null) ? asnObject.validate() : false;

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

        // Trys to serializes the AsnObject and open the HTTP connection
        public Boolean testApiSetup()
        {
            if (sfConnection == null || asnObject == null) return false;
            bool success = sfConnection.tryApiSetup(asnObject);

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

        // Trys to send the actual API call to Salesforce
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

        // Trys to run the full program
        public async Task<bool> testAll()
        {
            ConsoleLogger.log("Running All");

            bool success;
            success = await testCredentials();
            if (success) success = await testLoadData();
            if (success) success = testConvertData();
            if (success) success = testApiSetup();
            if (success) success = await testApiCall();

            return success;
        }
    }
}
