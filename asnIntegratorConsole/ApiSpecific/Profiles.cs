using asnIntegratorConsole.UiSpecific;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asnIntegratorConsole
{
    // Class to load, update, store and delete Profiles. 
    // Profiles are essentially Credentials, however, a profile does not store a password, while credentials do.
    class Profiles
    {
        //Array of all stored profiles
        private String profilesPath = ConfigurationManager.AppSettings["ProfilesFilePath"];
        public List<Profile> PROFILES;

        public Profiles()
        {
            PROFILES = readProfiles();
        }

        public void setProfilesPath(String path)
        {
            profilesPath = path;
            PROFILES = readProfiles();
        }
        public String getProfilesPath()
        {
            return profilesPath;
        }


        public List<Profile> readProfiles()
        {
            Console.Write("maybe baby");
            String rawData;
            List<Profile> profiles;
            try
            {
                using (StreamReader r = new StreamReader(profilesPath))
                {
                    rawData = r.ReadToEnd();
                }
                profiles = JsonConvert.DeserializeObject<List<Profile>>(rawData);
                ConsoleLogger.log("Loaded Profiles");
            }
            catch (Exception e)
            {
                ConsoleLogger.log("Couldn't Load Profiles");
                ConsoleLogger.log(e.ToString());
                profiles = new List<Profile>();
            }

            return profiles;
        }

        public void saveProfiles()
        {
            String rawData = JsonConvert.SerializeObject(PROFILES);
            try
            {
                using (StreamWriter r = new StreamWriter(profilesPath))
                {
                    r.Write(rawData);
                }

            }
            catch (Exception e)
            {
                ConsoleLogger.log("Couldn't Update Profiles");
                return;
            }
            ConsoleLogger.log("Updated Profiles");
        }

        public void saveProfile(Profile profile)
        {
            int i = 1;
            String profName = profile.ProfileName;
            if (findProfile(profName) != null)
            {
                deleteProfile(profName);
                //profName = profile.ProfileName + "_" + i;
                //i++;
            }

                profile.ProfileName = profName;
            PROFILES.Add(profile);
            ConsoleLogger.log("Added Profile");
            saveProfiles();
        }

        public void deleteProfile(String profilename)
        {
            Profile prof = findProfile(profilename);
            if(prof != null)
            {
                PROFILES.Remove(prof);
                ConsoleLogger.log("Removed Profile");
            } else
            {
                ConsoleLogger.log("Couldn't Remove Profile");
            }
            saveProfiles();
        }

        public Profile findProfile(String profileName)
        {
            return PROFILES.Find(x => x.ProfileName == profileName);
        }
    }

    class Profile
    {
        public String ProfileName       { get; set; }
        public String Username          { get; set; }
        public String SecurityToken     { get; set; }
        public String ConsumerKey       { get; set; }
        public String ConsumerSecret    { get; set; }
        public Boolean IsSandboxUser    { get; set; }

        public List<LoaderInfo> CustomFields { get; set; }

        public Profile()
        {

        }
        public Profile(string profile_name, string username, string consumer_key, string consumer_secret, string security_token, Boolean is_sandbox_user, List<LoaderInfo> customFields)
        {
            this.ProfileName = profile_name;
            this.Username = username;
            this.ConsumerKey = consumer_key;
            this.ConsumerSecret = consumer_secret;
            this.SecurityToken = security_token;
            this.IsSandboxUser = is_sandbox_user;
            this.CustomFields = customFields;
        }
    }

    class LoaderInfo
    {
        public String DataLoaderName { get; set; }
        public List<Field> CustomFields { get; set; }
    }
}
