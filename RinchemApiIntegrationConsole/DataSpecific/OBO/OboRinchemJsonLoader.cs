﻿using RinchemApiIntegrationConsole.UiSpecific;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RinchemApiIntegrationConsole.OBO
{
    // Simple implementation of the data loader
    class OboRinchemJsonLoader : DataLoader
    {
        private String filepath { get; set; }
        private String rawData { get; set; }

        private Field fileLocation = new Field() { Name = "FileLocation" , Value = "c:/Development/CustomRestPayload.json" };


        public void setFilePath(String path)
        {
            filepath = path;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **DEFINE UNIQUE NAME**              CUSTOMER MUST IMPLEMENT                                      ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public String GetUniqueName()
        {
            return "OBO - Rinchem JSON Loader";
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **DEFINE CUSTOM FIELDS**            CUSTOMER MUST IMPLEMENT                                      ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<Field> GetCustomFields()
        {
            List<Field> fields = new List<Field>();

            fields.Add(fileLocation);

            return fields;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **LOAD DATA**                       CUSTOMER MUST IMPLEMENT                                      ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<Boolean> LoadData()
        {
            //  Convert our desired JSON file to a string
            // rawData = (File.ReadAllText(filepath).ToString());
            try
            {
                using (StreamReader r = new StreamReader(fileLocation.Value))
                {
                    rawData = await r.ReadToEndAsync();
                }

            }
            catch (Exception e)
            {
                ConsoleLogger.log(e.ToString());
                return false;
            }
            return true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **TEST THAT DATA LOADED**           CUSTOMER SHOULD IMPLEMENT                                    ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Boolean TestData()
        {
            if (rawData != null) return true;
            return false;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **CONVERT THE DATA TO OBO FORMAT**        CUSTOMER MUST IMPLEMENT                                ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public DataObject ConvertDataToObject()
        {
            try
            {
                OboObject obo = JsonConvert.DeserializeObject<OboObject>(rawData);
                return obo;
            }
            catch (Exception e)
            {
                ConsoleLogger.log(e.ToString());
                return null;
            }         
        }

    }
}
