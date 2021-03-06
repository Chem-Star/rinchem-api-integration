﻿using asnIntegratorConsole.UiSpecific;
using RinchemApiIntegrationConsole.ASN;
using RinchemApiIntegrationConsole.OBO;
using RinchemApiIntegrationConsole.OBO2;
using RinchemApiIntegrationConsole.UiSpecific;
using RinchemApiIntegrator.ApiSpecific;
using RinchemApiIntegrator.UiSpecific;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;

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
        //private static List<DataLoader> AsnDataLoaders = new List<DataLoader>() { new AsnRinchemExcelLoader(), new AsnRinchemJsonLoader() };
        //private static List<DataLoader> OboDataLoaders = new List<DataLoader>() { new OboRinchemExcelLoader(), new OboRinchemJsonLoader() };
        //private static List<DataLoader> Obo2DataLoaders = new List<DataLoader>() { new Obo2RinchemJsonLoader() };

        private DataLoader dataLoader { get; set; }                 // Interface responsible for loading and converting data
        private DataObject dataObject { get; set; }                 // Interface responsible for defining the model type

        //private static List<DataObject> dataObjects = new List<DataObject>() { new AsnObject(), new OboObject(), new Obo2Object() };

        private List<ApiObject> apiObjects;
        private ApiObject apiObject;

        private static Profiles profiles { get; set; }              // Contains all prior saved Profiles

        private SalesForceConnection sfConnection { get; set; }     // Manages the salesforce connection
        private Profile profile { get; set; }                       // The profile that we use to connect to salesforce
        //private String apiVerb { get; set; }                        // The verb we use during the API call
        //private String apiType { get; set; }                        // The api that we are interested in calling
        //private String apiAction { get; set; }                      // The action that we would like the API to perform
        private String objectName { get; set; }                     // The name of the DataObject that we would like the API to handle

        private String queryString { get; set; }                    // Used to get desired ASNs

        private Boolean areCredentialsVerified = false;
        private Boolean useValidation = true;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor -- needs ui so that it can update the logbox
        public APImanager()
        {
            apiObjects = ApiObjects.getApiObjects(this);
            profiles = new Profiles();
            apiObject = apiObjects[0];
            //dataLoader = AsnDataLoaders[0];
            //apiVerb = "POST";
            //apiType = "ASN";
            //apiAction = "NEW";
            objectName = "";
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // DATA LOADERS
        //public List<DataLoader> getAllDataLoaders()
        //{
        //    List<DataLoader> dataLoaders = new List<DataLoader>();
        //    dataLoaders.AddRange(AsnDataLoaders);
        //    dataLoaders.AddRange(OboDataLoaders);
        //    dataLoaders.AddRange(Obo2DataLoaders);
        //    return dataLoaders;
        //}
        public List<DataLoader> getApiDataLoaders()
        {
            return apiObject.getDataLoaders();
            //switch (apiType)
            //{
            //    case "ASN":
            //        return AsnDataLoaders;
            //    case "OBO":
            //        return OboDataLoaders;
            //    case "OBO2":
            //        return Obo2DataLoaders;
            //    default:
            //        Console.WriteLine("Couldn't find the specified API");
            //        return null;
            //}
        }
        public DataLoader getCurrentDataLoader()
        {
            //return apiObject.getCurrentDataLoader();
            return dataLoader;
        }
        public void setCurrentDataLoader(String typeName)
        {
            if (typeName == null || typeName == "")
            {
                return;
            }
            
            dataLoader = apiObject.getDataLoaders().Find(x => x.GetUniqueName() == typeName);
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
        public List<ApiObject> getApiObjects() { return apiObjects; }
        public ApiObject getApiObject() { return apiObject; }
        public ApiActionObject getApiActionObject() { return apiObject.getApiActionObject(); }

        public void setApiVerb(String verb)
        {
            apiObject.setApiVerb(verb);
            //apiVerb = verb;
        }
        public void setApiType(String api)
        {
            apiObject = apiObjects.Find(x => x.getApiType() == api);
            dataObject = apiObject.getDataObject();
            //apiType = api;
            //switch (apiType)
            //{
            //    case "ASN":
            //        dataObject = new AsnObject();
            //        break;
            //    case "OBO":
            //        dataObject = new OboObject();
            //        break;
            //    case "OBO2":
            //        dataObject = new Obo2Object();
            //        break;
            //    default:
            //        ConsoleLogger.log("API Type Not Found");
            //        return;
            //}
            //dataObject.initializeRequest();
        }
        public void setApiAction(String action)
        {
            apiObject.setApiAction(action);
            //apiAction = action;
        }
        public void setObjectName(String name)
        {
            objectName = name;
            dataObject.setObjectName(name);
        }
        public void setQueryString(String query)
        {
            queryString = query;
        }
        public String getQueryString()
        {
            return queryString;
        }

        public String getCurrentApiType()
        {
            return apiObject.getApiType();
            //return apiType;
        }
        public String getCurrentApiVerb()
        {
            return apiObject.getApiVerb();
            //return apiVerb;
        }
        public String getCurrentApiAction()
        {
            return apiObject.getApiAction();
            //return apiAction;
        }
        public String getCurrentObjectName()
        {
            return objectName;
        }

        public void setUseValidation(bool value)
        {
            useValidation = value;
        }

        public void setResponse(String response)
        {
            apiObject.deserializeResponse(response);
            //dataObject.deserializeResponse(response);
        }
        public void viewResponse(ApiUserConsole console)
        {
            apiObject.viewResponse(console);

            //Window dataResponseViewer = dataObject.getResponseView();
            //dataResponseViewer.Owner = console;
            //dataResponseViewer.Show();
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
            
            sfConnection = new SalesForceConnection(this, credentials);
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
            bool success = (dataObject != null);
            if (success && useValidation) success = dataObject.validate();

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
            bool success = (sfConnection == null) ? false : true;

            if(dataObject == null)
            {
                switch (getCurrentApiType())
                {
                    case "ASN":
                        dataObject = new AsnObject();
                        break;
                    case "OBO":
                        dataObject = new OboObject();
                        break;
                    case "OBO2":
                        dataObject = new Obo2Object();
                        break;
                    default:
                        Console.WriteLine("The specified api type wasn't found");
                        break;
                }
            }

            String apiAction = apiObject.getApiAction();
            String apiVerb = apiObject.getApiVerb();
            String apiType = apiObject.getApiType();

            dataObject.setAction(apiAction);
            dataObject.setObjectName( (apiAction != "NEW") ? objectName : "");

            if(success) sfConnection.tryApiSetup(dataObject, apiVerb, apiType);

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
