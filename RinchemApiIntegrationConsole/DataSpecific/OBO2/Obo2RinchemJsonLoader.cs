﻿using RinchemApiIntegrationConsole.UiSpecific;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RinchemApiIntegrationConsole.OBO2
{
    // Simple implementation of the data loader
    class Obo2RinchemJsonLoader : DataLoader
    {
        private String filepath { get; set; }
        private String rawData { get; set; }

        private Field fileLocation = new Field() { Name = "FileLocation" , Value = "" };

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **DEFINE UNIQUE NAME**              CUSTOMER MUST IMPLEMENT                                      ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public String GetUniqueName()
        {
            return "OBO2 - Rinchem JSON Loader";
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **DEFINE CUSTOM FIELDS**            CUSTOMER MUST IMPLEMENT                                      ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<Field> GetCustomFields()
        {
            Button location = new Button();
            location.Content = fileLocation.Value;
            location.Click += location_click;
            fileLocation.element = location;

            List<Field> fields = new List<Field>();
            fields.Add(fileLocation);

            return fields;
        }

        private void location_click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                fileLocation.Value = filename;
                ((Button)sender).Content = filename;
            }
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
                Obo2Object oboObject = new Obo2Object();
                oboObject.initializeRequest();

                oboObject.rqstObject = JsonConvert.DeserializeObject<OboRequestObject>(rawData);
                return oboObject;
            }
            catch (Exception e)
            {
                ConsoleLogger.log(e.ToString());
                return null;
            }         
        }

    }
}
