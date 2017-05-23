using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinchemApiIntegrationConsole
{
    // Simple interface that customers must implement to use the ASN Integrator
    interface DataLoader
    {
        /// <summary>
        /// Implementer must implement this.
        /// </summary>
        /// <returns>A unique string to represent the data loader in the User Interface</returns>
        String GetUniqueName();

        /// <summary>
        /// Implementer should implement this if they need to add cutom fields and pull data from the GUI
        /// </summary>
        /// <returns>A collection of UI elements</returns>
        List<Field> GetCustomFields();

        /// <summary>
        /// Implementer needs to pull in their own data in whatever format is most convenient.
        /// </summary>
        /// <returns>True if the data was pulled in properly. False if there was an error in loading.</returns>
        Task<Boolean> LoadData();
        /// <summary>
        /// Implementer is highly suggested to Test the data that they pulled in to ensure that it is complete.
        /// and correct. If robustness isn't a concern, this can be skipped and simply return true in the 
        /// method override.
        /// </summary>
        /// <returns>True if the raw data is properly verified. False if there are issues with the raw data model</returns>
        Boolean TestData();
        
        /// <summary>
        /// Implementers must convert their raw data model into a (class) AsnObject. See DataSpecific -> AsnObject
        /// This object will then be verified and validated. After verification it will be serialized and this
        /// is what is sent through the API to Salesforce.
        /// </summary>
        /// <returns>An AsnObject model of the data</returns>
        DataObject ConvertDataToObject();
    }

    public class Field
    {
        public string Name { get; set; }  //Name for the field label
        public string Value { get; set; } //The return value entered into the field
    }
}
