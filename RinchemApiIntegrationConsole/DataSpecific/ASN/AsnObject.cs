using Newtonsoft.Json;
using RinchemApiIntegrationConsole.UiSpecific;
using RinchemApiIntegrator.UiSpecific;
using System;
using System.Collections.Generic;
using System.Windows;

namespace RinchemApiIntegrationConsole.ASN
{
    class AsnObject : DataObject
    {
        public AsnRequestObject rqstObject = new AsnRequestObject();     //This is serialized and sent to the API
        public AsnResponseObject respObject = new AsnResponseObject();    //This is populated by deserializing the API json response

        public void initializeRequest()
        {
            rqstObject = new AsnRequestObject();
            rqstObject.initialize();
        }
        public void initializeResponse()
        {
            respObject = new AsnResponseObject();
            respObject.initialize();
        }

        public String serializeRequest()
        {
            return JsonConvert.SerializeObject(rqstObject);
        }
        public void deserializeResponse(String response)
        {
            respObject.initialize();
            respObject = JsonConvert.DeserializeObject<AsnResponseObject>(response);
        }

        public Window getResponseView()
        {
            return new AsnResponseViewer(respObject);
        }


        public Boolean validate()
        {
            Request rqst = rqstObject.rqst;
            bool validated = true;
            if (rqst.asn.Date_ASN_Sent__c               == null) { ConsoleLogger.log( "Missing required field \"Date_ASN_Sent__c\"               ");     validated = false; };
            if (rqst.asn.Supplier_Name__c               == "") { ConsoleLogger.log( "Missing required field \"Supplier_Name__c\"               ");     validated = false; };
            if (rqst.asn.Rinchem_Supplier_Id__c         == "") { ConsoleLogger.log( "Missing required field \"Rinchem_Supplier_Id__c\"         ");     validated = false; };
            if (rqst.asn.ASN_Recipient_Name__c          == "") { ConsoleLogger.log( "Missing required field \"ASN_Recipient_Name__c\"          ");     validated = false; };
            if (rqst.asn.ASN_Recipient_Id__c            == "") { ConsoleLogger.log( "Missing required field \"ASN_Recipient_Id__c\"            ");     validated = false; };
            if (rqst.asn.Template_Version__c            == "") { ConsoleLogger.log( "Missing required field \"Template_Version__c\"            ");     validated = false; };
            if (rqst.asn.Estimated_Ship_Date__c         == "") { ConsoleLogger.log( "Missing required field \"Estimated_Ship_Date__c\"         ");     validated = false; };
            if (rqst.asn.Estimated_Arrival_Date__c      == "") { ConsoleLogger.log( "Missing required field \"Estimated_Arrival_Date__c\"      ");     validated = false; };
            if (rqst.asn.Order_Type__c                  == "") { ConsoleLogger.log( "Missing required field \"Order_Type__c\"                  ");     validated = false; };
            if (rqst.asn.Destination_Warehouse_Code__c  == "") { ConsoleLogger.log( "Missing required field \"Destination_Warehouse_Code__c\"  ");     validated = false; };
            if (rqst.asn.Carrier_Id__c                  == "") { ConsoleLogger.log( "Missing required field \"Carrier_Id__c\"                  ");     validated = false; };
            if (rqst.asn.Carrier_Name__c                == "") { ConsoleLogger.log( "Missing required field \"Carrier_Name__c\"                ");     validated = false; };
            if (rqst.asn.Purchase_Order_Number__c       == "") { ConsoleLogger.log( "Missing required field \"Purchase_Order_Number__c\"       ");     validated = false; };
            if (rqst.asn.Product_Owner_Id__c            == "") { ConsoleLogger.log( "Missing required field \"Product_Owner_Id__c\"            ");     validated = false; };
            
            
            rqst.lineItems.ForEach(item =>
               {
                   if (item.Vendor_Part_Number__c     == "") { ConsoleLogger.log("A line item is missing the required field \"Vendor_Part_Number__c\"    ");     validated = false; };
                   if (item.Product_Description__c    == "") { ConsoleLogger.log("A line item is missing the required field \"Product_Description__c\"   ");     validated = false; };
                   if (item.Quantity__c               == "") { ConsoleLogger.log("A line item is missing the required field \"Quantity__c\"              ");     validated = false; };
                   if (item.Unit_of_Measure__c        == "") { ConsoleLogger.log("A line item is missing the required field  \"Unit_of_Measure__c\"      ");     validated = false; };
               }
            );

            return validated;
        }

        public String getCustomApiSuffix()
        {
            return "/services/apexrest/v1/ASN__c";
        }

        public String getObjectName()
        {
            return rqstObject.rqst.asn.Name;
        }
        public void setObjectName(String name)
        {
            rqstObject.rqst.asn.Name = name;
        }
        public void setAction(String action)
        {
            rqstObject.rqst.asn.Action__c = action;
        }
    }

    public class AsnRequestObject
    {
        public Request rqst;

        public void initialize()
        {
            rqst = new Request();
            rqst.asn = new ASN();
            rqst.lineItems = new List<LineItems>();
        }
    }

    public class AsnResponseObject
    {
        public ASN asn;
        public List<LineItems> lineItems;
        public List<Status> statuses;
        public List<Response> asns;

        public void initialize()
        {
            asn = new ASN();
            lineItems = new List<LineItems>();
            statuses = new List<Status>();
        }
    }

    public class Request
    {
        public ASN asn;
        public List<LineItems> lineItems;
    }
    public class Response
    {
        public ASN asn;
        public List<LineItems> lineItems;
        public List<Status> statuses;
    }

    public class ASN
    {
        public String Name;
		public String Action__c;
        public String Message_Id__c;
        public String Date_ASN_Sent__c;
        public String Supplier_Name__c;
        public String Rinchem_Supplier_Id__c;
        public String ASN_Recipient_Name__c;
        public String ASN_Recipient_Id__c;
        public String Template_Version__c;
        public String Estimated_Ship_Date__c;
        public String Estimated_Arrival_Date__c;
        public String Shipment_Id__c;
        public String BOL_Number__c;
        public String Ship_From_Supplier__c;
        public String Order_Type__c;
        public String Order_Number__c;
        public String Origin_Id__c;
        public String Origin_Street_Address__c;
        public String Origin_City__c;
        public String Origin_State__c;
        public String Origin_Postal_Code__c;
        public String Origin_Country__c;
        public String Destination_Name__c;
        public String Destination_Warehouse_Code__c;
        public String Destination_Address__c;
        public String Destination_City__c;
        public String Destination_State__c;
        public String Destination_Postal_Code__c;
        public String Destination_Country__c;
        public String Carrier_Id__c;
        public String Carrier_Name__c;
        public String Purchase_Order_Number__c;
        public String Product_Owner_Id__c;
        public String CreatedDate;
    }

    public class LineItems
    {
        public String Name;
        public String Vendor_Part_Number__c;
        public String Product_Description__c;
        public String Product_Lot_Number__c;
        public String Product_Expiration_Date__c;
        public String Quantity__c;
        public String Unit_of_Measure__c;
        public String Hold_Code__c;
        public String Serial_Number__c;
    }

    public class Status
    {
        public String Name;
    }

}
