using Newtonsoft.Json;
using RinchemApiIntegrationConsole;
using RinchemApiIntegrationConsole.ASN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace RinchemApiIntegrator.UiSpecific
{
    /// <summary>
    /// Interaction logic for AsnResponseViewer.xaml
    /// </summary>
    public partial class AsnResponseViewer : Window
    {
        private AsnObject asnObject;
        private List<AsnObject> asnObjects = new List<AsnObject>();
        private int currentAsn = 0;
        private int currentLineItem = 0;
        public AsnResponseViewer(SalesForceResponse response)
        {
            String serial;
            if (response.asn != null)
            {
                AsnObject asnObject = new AsnObject(); asnObject.initialize();

                serial = JsonConvert.SerializeObject(response.asn);
                asnObject.rqst.asn = JsonConvert.DeserializeObject<ASN>(serial);

                serial = JsonConvert.SerializeObject(response.lineItems);
                asnObject.rqst.lineItems = JsonConvert.DeserializeObject<List<LineItems>>(serial);

                asnObjects.Add(asnObject);
            }
            else if(response.asns != null)
            {
                serial = JsonConvert.SerializeObject(response.asns);
                List<Request> rqsts = JsonConvert.DeserializeObject< List<Request>>(serial);

                foreach(Request rqst in rqsts)
                {
                    AsnObject asnObject = new AsnObject(); asnObject.initialize();
                    asnObject.rqst = rqst;

                    asnObjects.Add(asnObject);
                }
            }


            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (asnObjects.Count > 0) viewAsn(0);
        }

        private void viewAsn(int i)
        {

            List<KeyValuePair<String, String>> fields = new List<KeyValuePair<string, string>>();
            if (asnObjects.Count <= i)
            {
                currentAsn = 0;
                AsnDetailLeft.ItemsSource = fields; return;
            }

            asnObject = asnObjects.ElementAt(i);
            currentAsn = i;

            AsnNumbers.Children.RemoveRange(1, AsnNumbers.Children.Count - 1);
            for (int j = 0; j < asnObjects.Count; j++)
            {
                Button asn = new Button();
                asn.BorderThickness = new Thickness(0);
                asn.Background = Brushes.Transparent;
                if (j == currentAsn) asn.FontWeight = FontWeights.Bold;
                asn.Tag = (j); asn.Content = (j + 1);
                asn.Click += handle_view_asn;
                AsnNumbers.Children.Add(asn);
            }

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("ASN", asnObject.rqst.asn.Name));
            fields.Add(new KeyValuePair<string, string>("ASN Recipien tId", asnObject.rqst.asn.ASN_Recipient_Id__c));
            fields.Add(new KeyValuePair<string, string>("ASN Recipient Name", asnObject.rqst.asn.ASN_Recipient_Name__c));
            fields.Add(new KeyValuePair<string, string>("Date ASN Sent", asnObject.rqst.asn.Date_ASN_Sent__c));
            fields.Add(new KeyValuePair<string, string>("SF Created Date", asnObject.rqst.asn.CreatedDate));
            fields.Add(new KeyValuePair<string, string>("Estimated Arrival Date", asnObject.rqst.asn.Estimated_Arrival_Date__c));
            fields.Add(new KeyValuePair<string, string>("Estimated Ship Date", asnObject.rqst.asn.Estimated_Ship_Date__c));
            fields.Add(new KeyValuePair<string, string>("Order Type", asnObject.rqst.asn.Order_Type__c));
            AsnDetailLeft.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("ShipmentId", asnObject.rqst.asn.Shipment_Id__c));
            fields.Add(new KeyValuePair<string, string>("OrderNumber", asnObject.rqst.asn.Order_Number__c));
            fields.Add(new KeyValuePair<string, string>("PurchaseOrderNumber", asnObject.rqst.asn.Purchase_Order_Number__c));
            fields.Add(new KeyValuePair<string, string>("ProductOwnerId", asnObject.rqst.asn.Product_Owner_Id__c));
            fields.Add(new KeyValuePair<string, string>("SupplierName", asnObject.rqst.asn.Supplier_Name__c));
            fields.Add(new KeyValuePair<string, string>("RinchemSupplierId", asnObject.rqst.asn.Rinchem_Supplier_Id__c));
            fields.Add(new KeyValuePair<string, string>("CarrierId", asnObject.rqst.asn.Carrier_Id__c));
            fields.Add(new KeyValuePair<string, string>("CarrierName", asnObject.rqst.asn.Carrier_Name__c));
            fields.Add(new KeyValuePair<string, string>("BOLNumber", asnObject.rqst.asn.BOL_Number__c));
            AsnDetailRight.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("Destination Warehouse Code", asnObject.rqst.asn.Destination_Warehouse_Code__c));
            fields.Add(new KeyValuePair<string, string>("Destination Name", asnObject.rqst.asn.Destination_Name__c));
            fields.Add(new KeyValuePair<string, string>("Destination Address", asnObject.rqst.asn.Destination_Address__c));
            fields.Add(new KeyValuePair<string, string>("Destination City", asnObject.rqst.asn.Destination_City__c));
            fields.Add(new KeyValuePair<string, string>("Destination State", asnObject.rqst.asn.Destination_State__c));
            fields.Add(new KeyValuePair<string, string>("Destination Postal Code", asnObject.rqst.asn.Destination_Postal_Code__c));
            fields.Add(new KeyValuePair<string, string>("Destination Country", asnObject.rqst.asn.Destination_Country__c));
            DateDetailLeft.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("Ship From Supplier", asnObject.rqst.asn.Ship_From_Supplier__c));
            fields.Add(new KeyValuePair<string, string>("Origin Id", asnObject.rqst.asn.Origin_Id__c));
            fields.Add(new KeyValuePair<string, string>("Origin StreetAddress", asnObject.rqst.asn.Origin_Street_Address__c));
            fields.Add(new KeyValuePair<string, string>("Origin City", asnObject.rqst.asn.Origin_City__c));
            fields.Add(new KeyValuePair<string, string>("Origin State", asnObject.rqst.asn.Origin_State__c));
            fields.Add(new KeyValuePair<string, string>("Origin Postal Code", asnObject.rqst.asn.Origin_Postal_Code__c));
            fields.Add(new KeyValuePair<string, string>("Origin Country", asnObject.rqst.asn.Origin_Country__c));
            DateDetailRight.ItemsSource = fields;

            viewLineItem(0);
        }

        private void viewLineItem(int i)
        {
            List<KeyValuePair<String, String>> fields = new List<KeyValuePair<string, string>>();
            LineItemNumbers.Children.RemoveRange(1, LineItemNumbers.Children.Count - 1);

            if (asnObject.rqst.lineItems.Count <= i)
            {
                currentLineItem = 0;
                LineItemDataLeft.ItemsSource = fields;
                LineItemDataRight.ItemsSource = fields;
                return;
            }


            currentLineItem = i;
            for (int j = 0; j < asnObject.rqst.lineItems.Count; j++)
            {
                Button li = new Button();
                li.BorderThickness = new Thickness(0);
                li.Background = Brushes.Transparent;
                if (j == currentLineItem) li.FontWeight = FontWeights.Bold;
                li.Tag = (j); li.Content = (j + 1);
                li.Click += handle_view_lineitem;
                LineItemNumbers.Children.Add(li);
            }

            fields.Add(new KeyValuePair<string, string>("ASN #", asnObject.rqst.asn.Name));
            fields.Add(new KeyValuePair<string, string>("Line Item #", asnObject.rqst.lineItems.ElementAt(currentLineItem).Name));
            fields.Add(new KeyValuePair<string, string>("Product Description", asnObject.rqst.lineItems.ElementAt(currentLineItem).Product_Description__c));
            fields.Add(new KeyValuePair<string, string>("Product Lot Number", asnObject.rqst.lineItems.ElementAt(currentLineItem).Product_Lot_Number__c));
            fields.Add(new KeyValuePair<string, string>("Product Expiration Date", asnObject.rqst.lineItems.ElementAt(currentLineItem).Product_Expiration_Date__c));
            LineItemDataLeft.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("Quantity", asnObject.rqst.lineItems.ElementAt(currentLineItem).Quantity__c));
            fields.Add(new KeyValuePair<string, string>("Unit of Measure", asnObject.rqst.lineItems.ElementAt(currentLineItem).Unit_of_Measure__c));
            fields.Add(new KeyValuePair<string, string>("Vendor Part Number", asnObject.rqst.lineItems.ElementAt(currentLineItem).Vendor_Part_Number__c));
            fields.Add(new KeyValuePair<string, string>("Hold Code", asnObject.rqst.lineItems.ElementAt(currentLineItem).Hold_Code__c));
            fields.Add(new KeyValuePair<string, string>("Serial Number", asnObject.rqst.lineItems.ElementAt(currentLineItem).Serial_Number__c));
            LineItemDataRight.ItemsSource = fields;
        }

        private void handle_view_lineitem(object sender, RoutedEventArgs e)
        {
            Button li = sender as Button;
            viewLineItem((int)li.Tag);
        }
        private void handle_view_asn(object sender, RoutedEventArgs e)
        {
            Button asn = sender as Button;
            viewAsn((int)asn.Tag);
        }
    }
}
