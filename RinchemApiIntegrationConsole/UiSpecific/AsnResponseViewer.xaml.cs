using Newtonsoft.Json;
using RinchemApiIntegrationConsole;
using RinchemApiIntegrationConsole.ASN;
using RinchemApiIntegrationConsole.UiSpecific;
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
        private Response asnWrapper;
        private AsnResponseObject asnResponseObject;
        private int currentAsn = 0;
        private int currentLineItem = 0;
        public AsnResponseViewer(AsnResponseObject response)
        {
            if (response.asns == null) response.asns = new List<Response>();

            if (response.asn != null)
            {
                Response wrapper = new Response();
                wrapper.asn = response.asn;
                wrapper.lineItems = response.lineItems;
                wrapper.statuses = response.statuses;

                response.asns.Add(wrapper);
            }
            asnResponseObject = response;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (asnResponseObject.asns.Count > 0) viewAsn(0);
        }

        private void viewAsn(int i)
        {

            List<KeyValuePair<String, String>> fields = new List<KeyValuePair<string, string>>();
            if (asnResponseObject.asns.Count <= i)
            {
                currentAsn = 0;
                AsnDetailLeft.ItemsSource = fields; return;
            }

            asnWrapper = asnResponseObject.asns.ElementAt(i);
            currentAsn = i;

            AsnNumbers.Children.RemoveRange(1, AsnNumbers.Children.Count - 1);
            for (int j = 0; j < asnResponseObject.asns.Count; j++)
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
            if (asnWrapper.statuses != null && asnWrapper.statuses.Count > 0)
            {
                fields.Add(new KeyValuePair<string, string>("STATUS", asnWrapper.statuses.ElementAt(0).Name));
            }
            fields.Add(new KeyValuePair<string, string>("ASN", asnWrapper.asn.Name));
            fields.Add(new KeyValuePair<string, string>("ASN Recipien tId", asnWrapper.asn.ASN_Recipient_Id__c));
            fields.Add(new KeyValuePair<string, string>("ASN Recipient Name", asnWrapper.asn.ASN_Recipient_Name__c));
            fields.Add(new KeyValuePair<string, string>("Date ASN Sent", asnWrapper.asn.Date_ASN_Sent__c));
            fields.Add(new KeyValuePair<string, string>("SF Created Date", asnWrapper.asn.CreatedDate));
            fields.Add(new KeyValuePair<string, string>("Estimated Arrival Date", asnWrapper.asn.Estimated_Arrival_Date__c));
            fields.Add(new KeyValuePair<string, string>("Estimated Ship Date", asnWrapper.asn.Estimated_Ship_Date__c));
            fields.Add(new KeyValuePair<string, string>("Order Type", asnWrapper.asn.Order_Type__c));
            AsnDetailLeft.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("ShipmentId", asnWrapper.asn.Shipment_Id__c));
            fields.Add(new KeyValuePair<string, string>("OrderNumber", asnWrapper.asn.Order_Number__c));
            fields.Add(new KeyValuePair<string, string>("PurchaseOrderNumber", asnWrapper.asn.Purchase_Order_Number__c));
            fields.Add(new KeyValuePair<string, string>("ProductOwnerId", asnWrapper.asn.Product_Owner_Id__c));
            fields.Add(new KeyValuePair<string, string>("SupplierName", asnWrapper.asn.Supplier_Name__c));
            fields.Add(new KeyValuePair<string, string>("RinchemSupplierId", asnWrapper.asn.Rinchem_Supplier_Id__c));
            fields.Add(new KeyValuePair<string, string>("CarrierId", asnWrapper.asn.Carrier_Id__c));
            fields.Add(new KeyValuePair<string, string>("CarrierName", asnWrapper.asn.Carrier_Name__c));
            fields.Add(new KeyValuePair<string, string>("BOLNumber", asnWrapper.asn.BOL_Number__c));
            AsnDetailRight.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("Destination Warehouse Code", asnWrapper.asn.Destination_Warehouse_Code__c));
            fields.Add(new KeyValuePair<string, string>("Destination Name", asnWrapper.asn.Destination_Name__c));
            fields.Add(new KeyValuePair<string, string>("Destination Address", asnWrapper.asn.Destination_Address__c));
            fields.Add(new KeyValuePair<string, string>("Destination City", asnWrapper.asn.Destination_City__c));
            fields.Add(new KeyValuePair<string, string>("Destination State", asnWrapper.asn.Destination_State__c));
            fields.Add(new KeyValuePair<string, string>("Destination Postal Code", asnWrapper.asn.Destination_Postal_Code__c));
            fields.Add(new KeyValuePair<string, string>("Destination Country", asnWrapper.asn.Destination_Country__c));
            DateDetailLeft.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("Ship From Supplier", asnWrapper.asn.Ship_From_Supplier__c));
            fields.Add(new KeyValuePair<string, string>("Origin Id", asnWrapper.asn.Origin_Id__c));
            fields.Add(new KeyValuePair<string, string>("Origin StreetAddress", asnWrapper.asn.Origin_Street_Address__c));
            fields.Add(new KeyValuePair<string, string>("Origin City", asnWrapper.asn.Origin_City__c));
            fields.Add(new KeyValuePair<string, string>("Origin State", asnWrapper.asn.Origin_State__c));
            fields.Add(new KeyValuePair<string, string>("Origin Postal Code", asnWrapper.asn.Origin_Postal_Code__c));
            fields.Add(new KeyValuePair<string, string>("Origin Country", asnWrapper.asn.Origin_Country__c));
            DateDetailRight.ItemsSource = fields;

            viewLineItem(0);
        }

        private void viewLineItem(int i)
        {
            List<KeyValuePair<String, String>> fields = new List<KeyValuePair<string, string>>();
            LineItemNumbers.Children.RemoveRange(1, LineItemNumbers.Children.Count - 1);

            if (asnWrapper.lineItems.Count <= i)
            {
                currentLineItem = 0;
                LineItemDataLeft.ItemsSource = fields;
                LineItemDataRight.ItemsSource = fields;
                return;
            }


            currentLineItem = i;
            for (int j = 0; j < asnWrapper.lineItems.Count; j++)
            {
                Button li = new Button();
                li.BorderThickness = new Thickness(0);
                li.Background = Brushes.Transparent;
                if (j == currentLineItem) li.FontWeight = FontWeights.Bold;
                li.Tag = (j); li.Content = (j + 1);
                li.Click += handle_view_lineitem;
                LineItemNumbers.Children.Add(li);
            }

            fields.Add(new KeyValuePair<string, string>("ASN #", asnWrapper.asn.Name));
            fields.Add(new KeyValuePair<string, string>("Line Item #", asnWrapper.lineItems.ElementAt(currentLineItem).Name));
            fields.Add(new KeyValuePair<string, string>("Product Description", asnWrapper.lineItems.ElementAt(currentLineItem).Product_Description__c));
            fields.Add(new KeyValuePair<string, string>("Product Lot Number", asnWrapper.lineItems.ElementAt(currentLineItem).Product_Lot_Number__c));
            fields.Add(new KeyValuePair<string, string>("Product Expiration Date", asnWrapper.lineItems.ElementAt(currentLineItem).Product_Expiration_Date__c));
            LineItemDataLeft.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("Quantity", asnWrapper.lineItems.ElementAt(currentLineItem).Quantity__c));
            fields.Add(new KeyValuePair<string, string>("Unit of Measure", asnWrapper.lineItems.ElementAt(currentLineItem).Unit_of_Measure__c));
            fields.Add(new KeyValuePair<string, string>("Vendor Part Number", asnWrapper.lineItems.ElementAt(currentLineItem).Vendor_Part_Number__c));
            fields.Add(new KeyValuePair<string, string>("Hold Code", asnWrapper.lineItems.ElementAt(currentLineItem).Hold_Code__c));
            fields.Add(new KeyValuePair<string, string>("Serial Number", asnWrapper.lineItems.ElementAt(currentLineItem).Serial_Number__c));
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
