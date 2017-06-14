using RinchemApiIntegrationConsole.UiSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinchemApiIntegrationConsole.OBO
{
    class OboObject : DataObject
    {
        public Request rqst = new Request();

        public void initialize()
        {
            rqst = new Request();
            rqst.obo = new OBO();
            rqst.lineItems = new List<LineItems>();
        }

        public Boolean validate()
        {
            bool validated = true;
            if (rqst.obo.Message_Id__c                  == null) { ConsoleLogger.log( "Missing required field \"Message_Id__c\"                  ");     validated = false; };
            if (rqst.obo.Rinchem_Supplier_Id__c         == "") { ConsoleLogger.log( "Missing required field \"Rinchem_Supplier_Id__c\"         ");     validated = false; };
            if (rqst.obo.Order_Type__c                  == "") { ConsoleLogger.log( "Missing required field \"Order_Type__c\"                  ");     validated = false; };
            if (rqst.obo.Carrier_Name__c                == "") { ConsoleLogger.log( "Missing required field \"Carrier_Name__c\"                ");     validated = false; };
            if (rqst.obo.Purchase_Order_Number__c       == "") { ConsoleLogger.log( "Missing required field \"Purchase_Order_Number__c\"       ");     validated = false; };
            if (rqst.obo.Product_Owner_Id__c            == "") { ConsoleLogger.log( "Missing required field \"Product_Owner_Id__c\"            ");     validated = false; };


            rqst.lineItems.ForEach(item =>
               {
                   if (item.Name                      == "") { ConsoleLogger.log("A line item is missing the required field \"Name\"                     ");     validated = false; };
                   if (item.Quantity__c               == "") { ConsoleLogger.log("A line item is missing the required field \"Quantity__c\"              ");     validated = false; };
                   if (item.Unit_of_Measure__c        == "") { ConsoleLogger.log("A line item is missing the required field  \"Unit_of_Measure__c\"      ");     validated = false; };
               }
            );

            return validated;
        }

        public void setObjectName(String name)
        {
            rqst.obo.Name = name;
        }
        public void setAction(String action)
        {
            rqst.obo.Action__c = action;
        }
    }

    class Request
    {
        public OBO obo = new OBO();
        public List<LineItems> lineItems;
    }

    public class OBO
    {
        public String Name;
		public String Action__c;
        public String Message_Id__c;
        public String Order_Date__c;
        public String Rinchem_Supplier_Id__c;
        public String Purchase_Order_Number__c;
        public String Ship_To_Customer__c;
        public String Ship_To_Name__c;
        public String Freight_Payment_Terms_Type__c;
        public String Bill_To_Customer_Code__c;
        public String Bill_To_Name__c;
        public String Desired_Delivery_Date__c;
        public String Carrier_Service__c;
        public String Carrier_Name__c;
        public String From_Warehouse_Code__c;
        public String Order_Type__c;
        public String Product_Owner_Id__c;
    }

    public class LineItems
    {
        public String Name;
        public String Hold_Code__c;
        public String Inventory_Detail__c;
        public String Lot_Number__c;
        public String Rinchem_Part_Number__c;
        public String Quantity__c;
        public String Unit_of_Measure__c;
    }
}
