using Newtonsoft.Json;
using RinchemApiIntegrationConsole.UiSpecific;
using RinchemApiIntegrator.UiSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RinchemApiIntegrationConsole.OBO2
{
    class Obo2Object : DataObject
    {
        public OboRequestObject rqstObject = new OboRequestObject();     //This is serialized and sent to the API
        public OboResponseObject respObject = new OboResponseObject();    //This is populated by deserializing the API json response

        public void initializeRequest()
        {
        }
        public void initializeResponse()
        {
        }

        public String serializeRequest()
        {
            String serial = JsonConvert.SerializeObject(rqstObject);
            //Console.WriteLine(serial);
            return serial;
        }
        public void deserializeResponse(String response)
        {
            respObject = JsonConvert.DeserializeObject<OboResponseObject>(response);
        }

        public Window getResponseView()
        {
            return new Obo2ResponseViewer(respObject);
        }


        public Boolean validate()
        {
            OboWrapper rqst = rqstObject.rqst;
            OBO obo = rqst.obo;

            bool validated = true;
            if (obo.Rinchem_Supplier_Id__c == "") { ConsoleLogger.log("Missing required field \"Rinchem_Supplier_Id__c\"         "); validated = false; };
            if (obo.Order_Type__c == "") { ConsoleLogger.log("Missing required field \"Order_Type__c\"                  "); validated = false; };
            if (obo.Purchase_Order_Number__c == "") { ConsoleLogger.log("Missing required field \"Purchase_Order_Number__c\"       "); validated = false; };
            if (obo.Product_Owner_Id__c == "") { ConsoleLogger.log("Missing required field \"Product_Owner_Id__c\"            "); validated = false; };


            rqst.lineItems.ForEach(itemWrapper =>
            {
                LineItem item = itemWrapper.lineItem;

                if (item.Name == "") { ConsoleLogger.log("A line item is missing the required field \"Name\"                     "); validated = false; };
                if (item.Quantity__c == "") { ConsoleLogger.log("A line item is missing the required field \"Quantity__c\"              "); validated = false; };
                if (item.Unit_of_Measure__c == "") { ConsoleLogger.log("A line item is missing the required field  \"Unit_of_Measure__c\"      "); validated = false; };
            }
            );

            return validated;
        }

        public String getCustomApiSuffix()
        {
            return "/services/apexrest/v1/OBO__c";
        }

        public String getObjectName()
        {
            return rqstObject.rqst.obo.Name;
        }
        public void setObjectName(String name)
        {
            rqstObject.rqst.obo.Name = name;
        }
        public void setAction(String action)
        {
            rqstObject.rqst.obo.Action__c = action;
        }
        public void displayResponse()
        {

        }
    }

    public class OboRequestObject
    {
        public OboWrapper rqst = new OboWrapper();
        public OboRequestObject()
        {
        }
    }

    public class OboResponseObject
    {
        public List<OboWrapper> obos = new List<OboWrapper>();

        public String rqstStatus;
        public String rqstMessage;
    }


    public class OboWrapper
    {
        public OBO obo = new OBO();
        public List<CustomField> customFields = new List<CustomField>();
        public List<LineItemWrapper> lineItems = new List<LineItemWrapper>();
        public List<Status> statuses = new List<Status>();
    }

    public class LineItemWrapper
    {
        public LineItem lineItem;
        public List<CustomField> customFields = new List<CustomField>();
        public List<Attribute> lineItemAttributes = new List<Attribute>();
    }

    public class OBO
    {
        //outbound order fields
        public String Name;
        public String Action__c;
        public String Order_Type__c;
        public String Purchase_Order_Number__c;
        public String Rinchem_Supplier_Id__c;
        public String Product_Owner_Id__c;
        public String Bill_To_Customer_Code__c;
        public String Bill_To_Name__c;
        public String Carrier_Service__c;
        public String Freight_Payment_Terms_Type__c;
        public String Order_Date__c;
        public String Desired_Delivery_Date__c;
        public String Ship_To_Customer__c;
        public String Ship_To_Name__c;
        public String Ship_To_Address__c;
        public String Ship_To_City__c;
        public String Ship_To_State__c;
        public String Ship_To_Postal_Code__c;
        public String Ship_To_Country__c;
        public String From_Warehouse_Code__c;

        //case fields
        public String Importer_of_Record__c;
        public String Special_Shipment_Instructions__c;
        public String International_Shipment__c; //not in the system yet
        public String Carrier_Account_Number__c; //not in the system yet
        public String Approver_Company__c;
        public String Approver_First_Name__c;
        public String Approver_Last_Name__c;
        public String Approver_Email__c;
        public String Approver_Phone__c;
        public String Requestor_Phone__c;
        public String Requestor_Email__c;
        public String Third_Party_Billing_Name__c;
        public String Third_Party_Billing_Address__c;
        public String Third_Party_Billing_City__c;
        public String Third_Party_Billing_State__c;
        public String Third_Party_Billing_Postal_Code__c;
        public String Third_Party_Billing_PO_Number__c;
    }

    public class LineItem
    {
        public String Name;
        public String Purchase_Order_Number__c;
        public String Hold_Code__c;
        public String Lot_Number__c;
        public String Rinchem_Part_Number__c;
        public String Owner_Part_Number__c;
        public String Supplier_Part_Number__c;
        public String Quantity__c;
        public String Unit_of_Measure__c;

    }

    public class Status
    {
        public String Name;
    }

    public class Attribute
    {
        public String Name;
        public String Value__c;
        public String Warehouse__c;
    }

    public class CustomField
    {
        public String Name;
        public String Value__c;
    }
}
