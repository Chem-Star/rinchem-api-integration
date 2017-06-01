using RinchemApiIntegrationConsole.UiSpecific;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinchemApiIntegrationConsole
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
            PROFILES = readProfilesFromConfig();
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


        public List<Profile> readProfilesFromConfig()
        {
            List<Profile> profiles;
            try
            {
                String profilesString = ConfigurationManager.AppSettings["Profiles"];
                profiles = JsonConvert.DeserializeObject<List<Profile>>(profilesString);
            }
            catch (Exception e)
            {
                ConsoleLogger.log("Couldn't Load Profiles");
                ConsoleLogger.log(e.ToString());
                profiles = new List<Profile>();
            }

            return profiles;
        }


        public List<Profile> readProfiles()
        {
            String rawData;
            List<Profile> profiles;
            try
            {
                using (StreamReader r = new StreamReader(profilesPath))
                {
                    rawData = r.ReadToEnd();
                }
                profiles = JsonConvert.DeserializeObject<List<Profile>>(rawData);
            }
            catch (Exception e)
            {
                ConsoleLogger.log("Couldn't Load Profiles");
                ConsoleLogger.log(e.ToString());
                profiles = new List<Profile>();
            }

            return profiles;
        }

        public void saveProfilesInConfig()
        {
            String rawData = JsonConvert.SerializeObject(PROFILES);
            try
            {
                Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                oConfig.AppSettings.Settings["Profiles"].Value = rawData;
                oConfig.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception e)
            {
                ConsoleLogger.log("Couldn't Update Profiles");
                ConsoleLogger.log(e.ToString());
                return;
            }
            ConsoleLogger.log("Updated Profiles In Config");
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
            ConsoleLogger.log("Updated Profiles In File");
        }

        public void saveProfile(Profile profile)
        {
            String profId = profile._id;

            if (findProfileByName(profile.ProfileName) != null && 
                findProfileByName(profile.ProfileName)._id != profId)
            {
                String newName = profile.ProfileName;
                int i = 0;
                while (findProfileByName(newName) != null)
                {
                    i++;
                    newName = profile.ProfileName + "_" + i;
                }
                profile.ProfileName = newName;
            }

            if (findProfile1(profId) == null)
            {
                PROFILES.Add(profile);
            }

            saveProfilesInConfig();
        }

        public void deleteProfile(String uid)
        {
            Profile prof = findProfile1(uid);
            if(prof != null)
            {
                PROFILES.Remove(prof);
                ConsoleLogger.log("Removed Profile");
            } else
            {
                ConsoleLogger.log("Couldn't Remove Profile");
            }
            saveProfilesInConfig();
        }

        public Profile findProfile1(String uid)
        {
            return PROFILES.Find(x => x._id == uid);
        }
        public Profile findProfileByName(String name)
        {
            return PROFILES.Find(x => x.ProfileName == name);
        }
    }

    public class Profile
    {
        public String _id               { get; private set; }
        public String ProfileName       { get; set; }
        public String Username          { get; set; }
        public String SecurityToken     { get; set; }
        public String ConsumerKey       { get; set; }
        public String ConsumerSecret    { get; set; }
        public Boolean IsSandboxUser    { get; set; }

        public Profile()
        {
            _id = Guid.NewGuid().ToString();
        }
        public Profile(string uid, string profile_name, string username, string consumer_key, string consumer_secret, string security_token, Boolean is_sandbox_user)
        {
            this._id = (uid != null) ? uid : Guid.NewGuid().ToString();
            this.ProfileName = profile_name;
            this.Username = username;
            this.ConsumerKey = consumer_key;
            this.ConsumerSecret = consumer_secret;
            this.SecurityToken = security_token;
            this.IsSandboxUser = is_sandbox_user;
        }

        internal void updateFields(string profile_name, string username, string consumer_key, string consumer_secret, string security_token, Boolean is_sandbox_user)
        {
            this.ProfileName = profile_name;
            this.Username = username;
            this.ConsumerKey = consumer_key;
            this.ConsumerSecret = consumer_secret;
            this.SecurityToken = security_token;
            this.IsSandboxUser = is_sandbox_user;
        }
    }

    public class LoaderInfo
    {
        public String DataLoaderName { get; set; }
        public List<Field> CustomFields { get; set; }
    }
}
