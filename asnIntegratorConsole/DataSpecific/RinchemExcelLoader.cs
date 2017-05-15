using asnIntegratorConsole.UiSpecific;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;       //Microsoft Excel 14 object in references-> COM tab

namespace asnIntegratorConsole
{
    class RinchemExcelLoader : DataLoader
    {
        Field dbLocation = new Field() { Name = "FileLocation", Value= "C:/Users/jdenning/Desktop/ASN_MasterFile.xlsx" };
        Field shipID = new Field() { Name = "Ship_ID", Value = "154606" };

        List<List<String>> rawData;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **DEFINE CUSTOM FIELDS**            CUSTOMER MUST IMPLEMENT                                      ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<Field> GetCustomFields()
        {
            //Tell the user interface what custom fields to display
            List<Field> fields = new List<Field>() { dbLocation, shipID };
            return fields;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **LOAD DATA**                       CUSTOMER MUST IMPLEMENT                                      ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<bool> LoadData()
        {
            rawData = new List<List<String>>();
            try
            {
                //Create COM Objects. Create a COM object for everything that is referenced
                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(dbLocation.Value);
                //Excel isn't 0 based
                Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                Excel.Range xlRange = xlWorksheet.UsedRange;

                //Get the column headers, set them as the first entry in our rawData array
                List<String> headers = readRow(xlRange, 1);
                rawData.Add(headers);

                //Finds the first and last row containing the specified shipping id
                int[] rowRange = FindShipIdRange(xlRange, shipID.Value);

                int rowFirst = rowRange[0];     //First row containing the shipping id
                int rowLast = rowRange[1];      //Last row containing the shipping id

                //Iterate over each row, pulling it in as an Array of strings
                // then add each row to the rawData Array
                for (int i=rowFirst; i<=rowLast; i++)
                {
                    if (xlRange.Cells[i, 1] != null && xlRange.Cells[i, 1].Value2 != null)
                    {
                        List<String> rowData = readRow(xlRange, i);
                        rawData.Add(rowData);
                    }
                    else
                    {
                        break;
                    }
                }
                //cleanup
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //rule of thumb for releasing com objects:
                //  never use two dots, all COM objects must be referenced and released individually
                //  ex: [somthing].[something].[something] is bad

                //release com objects to fully kill excel process from running in the background
                Marshal.ReleaseComObject(xlRange);
                Marshal.ReleaseComObject(xlWorksheet);

                //close and release
                xlWorkbook.Close();
                Marshal.ReleaseComObject(xlWorkbook);

                //quit and release
                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);
            }
            catch (Exception e)
            {
                ConsoleLogger.log(e.ToString());
                return false;
            }
            return true;
        }

        // Read the specified row i from the excel sheet into a 1 dimensional string array
        // then return the array
        private List<String> readRow(Excel.Range xlRange, int i)
        {

            List<String> columns = new List<String>();
            int colCount = xlRange.Columns.Count;

            // Read each cell in the row and add it to the columns array
            for (int j = 1; j < colCount; j++)
            { 
                if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                {
                    columns.Add( xlRange.Cells[i, j].Text.ToString() );
                }
                else
                {
                    // The json format can't handle nulls so add a blank string instead.
                    columns.Add("");
                }
            }
            return columns;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **TEST THAT DATA LOADED**           CUSTOMER SHOULD IMPLEMENT                                    ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool TestData()
        {
            if (rawData == null) return false;          //Ensure that our rawData was initialized
            if (rawData.Count < 1) return false;        //Ensure that our rawData isn't empty
            if (rawData.ElementAt(0).Count < 1) return false;   //Ensure that our first row isn't empty
            if (rawData.ElementAt(0).ElementAt(0) != "Date")    //Ensure that the first cell we brought in has value "Date"
            {
                ConsoleLogger.log("FirstColumn should be 'Date' found: '" + rawData.ElementAt(0).ElementAt(0) + "'");
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **CONVERT THE DATA TO ASN FORMAT**        CUSTOMER MUST IMPLEMENT                                ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public AsnObject ConvertDataToAsnObject()
        {
            AsnObject asnObject = new AsnObject();          //Initialize our ASN object
            asnObject.rqst = new Request();                 //Initialize the request object

            asnObject.rqst.asn = populateAsn();             //Reformat data for the ASN info object
            asnObject.rqst.lineItems = populateLineItems(); //Reformat data for each of our line items

            return asnObject;
        }

        public ASN populateAsn()
        {
            ASN asn                             = new ASN();
            asn.Name                          = "";
            asn.Action__c                     = "";
            asn.Message_Id__c                 = "";
            asn.Date_ASN_Sent__c                = getDateItem( 1, "Date"                       );
            asn.Supplier_Name__c                = getStringItem( 1, "Sup_Name"                   );
            asn.Rinchem_Supplier_Id__c        = "MAL";
            asn.ASN_Recipient_Name__c           = getStringItem( 1, "ASN_Recpt_ #"               );
            asn.ASN_Recipient_Id__c             = getStringItem( 1, "ASN_Recpt_ID"               );
            asn.Template_Version__c             = getStringItem( 1, "Templt_Ver"                 );
            asn.Estimated_Ship_Date__c          = getDateItem( 1, "Est_Ship_Date"              );
            asn.Estimated_Arrival_Date__c       = getDateItem( 1, "Est_Arriv_Date"             );
            asn.Shipment_Id__c                  = getStringItem( 1, "Ship_ID"                    );
            asn.BOL_Number__c                   = getStringItem( 1, "BOL_#"                      );
            asn.Ship_From_Supplier__c           = getStringItem( 1, "Ship_Frm_Sup"               );
            asn.Order_Type__c                   = getStringItem( 1, "Ord_Ty_(PO)"                );
            asn.Order_Number__c               = getStringItem(1, "BOL_#");
            asn.Origin_Id__c                    = getStringItem( 1, "Origin_ID"                  );
            asn.Origin_Street_Address__c        = getStringItem( 1, "Origin_Address"             );
            asn.Origin_City__c                  = getStringItem( 1, "Origin_City"                );
            asn.Origin_State__c                 = getStringItem( 1, "Origin_State"               );
            asn.Origin_Postal_Code__c           = getStringItem( 1, "Origin_Zip_Code"            );
            asn.Origin_Country__c               = getStringItem( 1, "Origin_Cntry"               );
            asn.Destination_Name__c             = getStringItem( 1, "Dest_Nm"                    );
            asn.Destination_Warehouse_Code__c   = getStringItem( 1, "Dest_WHSE_Cd"               );
            asn.Destination_Address__c          = getStringItem( 1, "Dest_Address"               );
            asn.Destination_City__c             = getStringItem( 1, "Dest_City"                  );
            asn.Destination_State__c            = getStringItem( 1, "Dest_State"                 );
            asn.Destination_Postal_Code__c      = getStringItem( 1, "Dest_Zip"                   );
            asn.Destination_Country__c          = getStringItem( 1, "Dest_Country"               );
            asn.Carrier_Id__c                   = getStringItem( 1, "Carrier_ID"                 );
            asn.Carrier_Name__c                 = getStringItem( 1, "Carrier_Name"               );
            asn.Purchase_Order_Number__c        = getStringItem( 1, "PO_#"                       );
            asn.Product_Owner_Id__c             = getStringItem(1, "Prod_Ownr_ID"                );
            return asn;
        }

        private List<LineItems> populateLineItems()
        {
            List<LineItems> lineItems = new List<LineItems>();

            for(int i=1; i<rawData.Count; i++)
            {
                LineItems lineItem = new LineItems();

                lineItem.Name                               = getStringItem( i, "Line_Item_#"               );
                lineItem.Vendor_Part_Number__c              = getStringItem( i, "Vendor_PN"                 );
                lineItem.Product_Description__c             = getStringItem( i, "Prod_Descrip_"             );
                lineItem.Product_Lot_Number__c              = getStringItem( i, "Lot_Num"                   );
                lineItem.Product_Expiration_Date__c         = getDateItem( i, "Exprire_Date"              );
                lineItem.Quantity__c                        = getStringItem( i, "Quantity"                  );
                lineItem.Unit_of_Measure__c                 = getStringItem( i, "UOM"                       );
                lineItem.Hold_Code__c                       = getStringItem( i, "Hold_Code"                 );
                lineItem.Serial_Number__c                   = getStringItem( i, "SN"                        );

                lineItems.Add(lineItem);
            }

            return lineItems;
        }

        //Gets the item from rawData in the given row, in the column corresponding to the given columnName
        //returns the result as a string
        private String getStringItem(int row, String columnName)
        { 
            List<String> rowData = rawData.ElementAt(row);

            if (rowData == null) return "";
            int column = rawData.ElementAt(0).FindIndex(col => col == columnName);
            if (column == -1) return "";
            String item = rowData.ElementAt(column);

            return item;
        }
        //Gets a date string from rawData in the given row, in the column corresponding to the given columnName
        //reformats the date to the format required by the Salesforce API, returns the result as a string
        private String getDateItem(int row, String columnName)
        {
            String item = getStringItem(row, columnName);

            String[] rawDate = item.Split('/');
            if (rawDate.Length != 3)
            {
                ConsoleLogger.log("Error parsing date - " + rawDate);
                return "";
            }
            String properDate = "20" + rawDate[2] + "-" + rawDate[0] + "-" + rawDate[1];

            return properDate;
        }


        //Searches the excel sheet for all rows that contain the given tShipID
        // returns the first row using the id in int[0] and the last row in int[1]
        private int[] FindShipIdRange(Excel.Range xlRange, String tShipID)
        {
            //Simple trackers
            Excel.Range currentFind = null;
            Excel.Range firstFind = null;
            Excel.Range lastFind = null;

            //Find which column our Ship_IDs are in
            List<String> rowData = rawData.ElementAt(0);
            if (rowData == null) return null;
            int colNum = rawData.ElementAt(0).FindIndex(col => col == "Ship_ID");
            char colChar = (char) ('A' + colNum);

            //Figure out how many rows are in our excel sheet
            int rows = xlRange.Rows.Count;

            // Limit our search range to the Ship_ID column
            Excel.Range ShipIDsRange = xlRange.get_Range(""+colChar+""+1, ""+colChar+""+rows);
            // Find the first item in the range that contains the Ship_ID
            // You should specify all these parameters every time you call this method,
            // since they can be overridden in the Excel user interface. 
            currentFind = ShipIDsRange.Find(tShipID, Type.Missing,
                Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
                Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false,
                Type.Missing, Type.Missing);

            while (currentFind != null)
            {
                // Keep track of the first range you find. 
                if (firstFind == null)
                {
                    firstFind = currentFind;
                }
                // Look for the next item in the range that has the ShipID
                // IF the next item found is the first item then we are done
                else if (currentFind.get_Address(Excel.XlReferenceStyle.xlA1)
                      == firstFind.get_Address(Excel.XlReferenceStyle.xlA1))
                {
                    break;
                }

                // Keep track of this find and then continue the search with the nextFind
                lastFind = currentFind;
                currentFind = ShipIDsRange.FindNext(currentFind);
            }

            int start = firstFind.Row;
            int end = lastFind.Row;
            ConsoleLogger.log("Range: " + start + "-" + end);

            return new int[] { start, end };
        }
    }
}
