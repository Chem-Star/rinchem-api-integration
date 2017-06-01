using RinchemApiIntegrationConsole.UiSpecific;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;       //Microsoft Excel 14 object in references-> COM tab

namespace RinchemApiIntegrationConsole.OBO
{
    class OboRinchemExcelLoader : DataLoader
    {
        Field dbLocation = new Field() { Name = "FileLocation", Value= "" };
        Field poNum = new Field() { Name = "PO Number", Value = "" };

        List<List<String>> rawData;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **DEFINE UNIQUE NAME**              CUSTOMER MUST IMPLEMENT                                      ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public String GetUniqueName()
        {
            return "OBO - Rinchem Excel Loader";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **DEFINE CUSTOM FIELDS**            CUSTOMER MUST IMPLEMENT                                      ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<Field> GetCustomFields()
        {
            Button location = new Button();
            location.Content = dbLocation.Value;
            location.Click += location_click;
            dbLocation.element = location;

            //Tell the user interface what custom fields to display
            List<Field> fields = new List<Field>() { dbLocation, poNum };
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
                dbLocation.Value = filename;
                ((Button)sender).Content = filename;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **LOAD DATA**                       CUSTOMER MUST IMPLEMENT                                      ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<bool> LoadData()
        {
           return await Task.Run(() =>
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

                   //Finds the first and last row containing the specified PoNum
                   int[] rowRange = FindPoNumRange(xlRange, poNum.Value);

                   int rowFirst = rowRange[0];     //First row containing the po num
                   int rowLast = rowRange[1];      //Last row containing the po num
                   if (rowLast + rowFirst <= 0) return false;

                   //Iterate over each row, pulling it in as an Array of strings
                   // then add each row to the rawData Array
                   for (int i = rowFirst; i <= rowLast; i++)
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
           });
        }

        // Read the specified row i from the excel sheet into a 1 dimensional string array
        // then return the array
        private List<String> readRow(Excel.Range xlRange, int i)
        {

            List<String> columns = new List<String>();
            int colCount = xlRange.Columns.Count;

            // Read each cell in the row and add it to the columns array
            for (int j = 1; j <= colCount; j++)
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
            if (rawData.ElementAt(0).ElementAt(0) != "Order_Date")    //Ensure that the first cell we brought in has value "Date"
            {
                ConsoleLogger.log("FirstColumn should be 'Order_Date' found: '" + rawData.ElementAt(0).ElementAt(0) + "'");
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///         **CONVERT THE DATA TO ASN FORMAT**        CUSTOMER MUST IMPLEMENT                                ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public DataObject ConvertDataToObject()
        {
            OboObject oboObject = new OboObject();          //Initialize our ASN object
            oboObject.rqst = new Request();                 //Initialize the request object

            oboObject.rqst.obo = populateObo();             //Reformat data for the ASN info object
            oboObject.rqst.lineItems = populateLineItems(); //Reformat data for each of our line items

            return oboObject;
        }

        public OBO populateObo()
        {
            OBO obo                           = new OBO();
            obo.Name                          = "";
            obo.Action__c                     = "";
            obo.Message_Id__c                 = "";
            obo.Order_Date__c                         = getDateItem( 1, "Order_Date"                  );
            obo.Rinchem_Supplier_Id__c                = getStringItem( 1, "Rinchem_Supplier_Id"          );
            obo.Purchase_Order_Number__c              = getStringItem( 1, "Purchase_Order_Number"        );
            obo.Ship_To_Customer__c                   = getStringItem( 1, "Ship_To_Customer_Code"        );
            obo.Ship_To_Name__c                       = getStringItem( 1, "Ship_To_Name"                 );
            obo.Freight_Payment_Terms_Type__c         = getStringItem( 1, "Freight_Payment_Terms_Type"   );
            obo.Bill_To_Customer_Code__c              = getStringItem( 1, "Bill_To_Customer_Code"        );
            obo.Bill_To_Name__c                       = getStringItem( 1, "Bill_To_Name"                 );
            obo.Desired_Delivery_Date__c              = getDateItem( 1, "Desired_Delivery_Date"        );
            obo.Carrier_Service__c                    = getStringItem( 1, "Carrier_Service"              );
            obo.Carrier_Name__c                       = getStringItem( 1, "Carrier_Name"                 );
            obo.From_Warehouse_Code__c                = getStringItem( 1, "From_Warehouse_Code"          );
            obo.Order_Type__c                         = getStringItem( 1, "Order_Type"                   );
            obo.Product_Owner_Id__c                   = getStringItem( 1, "Product_Owner_Id"             );
            return obo;
        }

        private List<LineItems> populateLineItems()
        {
            List<LineItems> lineItems = new List<LineItems>();

            for(int i=1; i<rawData.Count; i++)
            {
                LineItems lineItem = new LineItems();

                lineItem.Name                       = getStringItem( i, "Line_Item_#"                      );
                lineItem.Hold_Code__c               = getStringItem( i, "Hold_Code"                          );
                lineItem.Inventory_Detail__c        = getStringItem( i, "Inventory_Detail"                   );
                lineItem.Lot_Number__c              = getStringItem( i, "Lot_Number"                         );
                lineItem.Rinchem_Part_Number__c     = getStringItem( i, "Rinchem_Part_Number"                );
                lineItem.Quantity__c                = getStringItem( i, "Quantity"                           );
                lineItem.Unit_of_Measure__c         = getStringItem( i, "Unit_of_Measure"                    );

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


        //Searches the excel sheet for all rows that contain the given tPoNum
        // returns the first row using the id in int[0] and the last row in int[1]
        private int[] FindPoNumRange(Excel.Range xlRange, String tPoNum)
        {
            //Simple trackers
            Excel.Range currentFind = null;
            Excel.Range firstFind = null;
            Excel.Range lastFind = null;

            //Find which column our PO_Nums are in
            List<String> rowData = rawData.ElementAt(0);
            if (rowData == null) return null;
            int colNum = rawData.ElementAt(0).FindIndex(col => col == "Purchase_Order_Number");
            char colChar = (char) ('A' + colNum);

            //Figure out how many rows are in our excel sheet
            int rows = xlRange.Rows.Count;

            // Limit our search range to the PO_Num column
            Excel.Range PoNumsRange = xlRange.get_Range(""+colChar+""+1, ""+colChar+""+rows);
            // Find the first item in the range that contains the PO_Num
            // You should specify all these parameters every time you call this method,
            // since they can be overridden in the Excel user interface. 
            currentFind = PoNumsRange.Find(tPoNum, Type.Missing,
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
                // Look for the next item in the range that has the PO_Num
                // IF the next item found is the first item then we are done
                else if (currentFind.get_Address(Excel.XlReferenceStyle.xlA1)
                      == firstFind.get_Address(Excel.XlReferenceStyle.xlA1))
                {
                    break;
                }

                // Keep track of this find and then continue the search with the nextFind
                lastFind = currentFind;
                currentFind = PoNumsRange.FindNext(currentFind);
            }

            if(firstFind == null || lastFind == null)
            {
                ConsoleLogger.log("No Matching POs Found");
                return new int[] { 0, 0 };
            }
            int start = firstFind.Row;
            int end = lastFind.Row;
            ConsoleLogger.log("Range: " + start + "-" + end);

            return new int[] { start, end };
        }
    }
}
