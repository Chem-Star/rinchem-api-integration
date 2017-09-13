using RinchemApiIntegrationConsole.OBO2;
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

namespace RinchemApiIntegrator.UiSpecific
{
    /// <summary>
    /// Interaction logic for OboResponseViewer.xaml
    /// </summary>
    public partial class Obo2ResponseViewer : Window
    {
        private OboWrapper oboWrapper;
        private OboResponseObject oboResponseObject;
        private int currentAsn = 0;
        private int currentLineItem = 0;
        public Obo2ResponseViewer(OboResponseObject response)
        {
            if (response.obos == null) response.obos = new List<OboWrapper>();

            oboResponseObject = response;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Window Loaded");
            if (oboResponseObject.obos.Count > 0) viewAsn(0);
        }

        private void viewAsn(int i)
        {
            Console.WriteLine(i);
            List<KeyValuePair<String, String>> fields = new List<KeyValuePair<string, string>>();
            if (oboResponseObject.obos.Count <= i)
            {
                currentAsn = 0;
                AsnDetailLeft.ItemsSource = fields; return;
            }

            oboWrapper = oboResponseObject.obos.ElementAt(i);
            currentAsn = i;

            AsnNumbers.Children.RemoveRange(1, AsnNumbers.Children.Count - 1);
            for (int j = 0; j < oboResponseObject.obos.Count; j++)
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
            if (oboWrapper.statuses != null && oboWrapper.statuses.Count > 0)
            {
                fields.Add(new KeyValuePair<string, string>("STATUS", oboWrapper.statuses.ElementAt(0).Name));
            }
            fields.Add(new KeyValuePair<string, string>("OBO", oboWrapper.obo.Name));
            fields.Add(new KeyValuePair<string, string>("Order Date", oboWrapper.obo.Order_Date__c));
            fields.Add(new KeyValuePair<string, string>("Rinchem Supplier Id", oboWrapper.obo.Rinchem_Supplier_Id__c));
            fields.Add(new KeyValuePair<string, string>("Product Owner Id", oboWrapper.obo.Product_Owner_Id__c));
            fields.Add(new KeyValuePair<string, string>("Purchase Order Number", oboWrapper.obo.Purchase_Order_Number__c));
            fields.Add(new KeyValuePair<string, string>("Order Type", oboWrapper.obo.Order_Type__c));
            fields.Add(new KeyValuePair<string, string>("Desired Delivery Date", oboWrapper.obo.Desired_Delivery_Date__c));
            AsnDetailLeft.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("Action", oboWrapper.obo.Action__c));
            fields.Add(new KeyValuePair<string, string>("Bill To Customer Code", oboWrapper.obo.Bill_To_Customer_Code__c));
            fields.Add(new KeyValuePair<string, string>("Bill To Name", oboWrapper.obo.Bill_To_Name__c));
            fields.Add(new KeyValuePair<string, string>("Carrier Service", oboWrapper.obo.Carrier_Service__c));
            fields.Add(new KeyValuePair<string, string>("Freight Payment Terms Type", oboWrapper.obo.Freight_Payment_Terms_Type__c));
            AsnDetailRight.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("From Warehouse Code", oboWrapper.obo.From_Warehouse_Code__c));
            DateDetailLeft.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("Ship To Customer", oboWrapper.obo.Ship_To_Customer__c));
            fields.Add(new KeyValuePair<string, string>("Ship To Name", oboWrapper.obo.Ship_To_Name__c));
            fields.Add(new KeyValuePair<string, string>("Ship To Address", oboWrapper.obo.Ship_To_Address__c));
            fields.Add(new KeyValuePair<string, string>("Ship To City", oboWrapper.obo.Ship_To_City__c));
            fields.Add(new KeyValuePair<string, string>("Ship To State", oboWrapper.obo.Ship_To_State__c));
            fields.Add(new KeyValuePair<string, string>("Ship To Postal Code", oboWrapper.obo.Ship_To_Postal_Code__c));
            fields.Add(new KeyValuePair<string, string>("Ship To Country", oboWrapper.obo.Ship_To_Country__c));
            DateDetailRight.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("Importer_of_Record__c", oboWrapper.obo.Importer_of_Record__c));
            fields.Add(new KeyValuePair<string, string>("Special_Shipment_Instructions__c", oboWrapper.obo.Special_Shipment_Instructions__c));
            fields.Add(new KeyValuePair<string, string>("International_Shipment__c", oboWrapper.obo.International_Shipment__c));
            fields.Add(new KeyValuePair<string, string>("Carrier_Account_Number__c", oboWrapper.obo.Carrier_Account_Number__c));
            fields.Add(new KeyValuePair<string, string>("Approver_Company__c", oboWrapper.obo.Approver_Company__c));
            fields.Add(new KeyValuePair<string, string>("Approver_First_Name__c", oboWrapper.obo.Approver_First_Name__c));
            fields.Add(new KeyValuePair<string, string>("Approver_Last_Name__c", oboWrapper.obo.Approver_Last_Name__c));
            fields.Add(new KeyValuePair<string, string>("Approver_Email__c", oboWrapper.obo.Approver_Email__c));
            fields.Add(new KeyValuePair<string, string>("Approver_Phone__c", oboWrapper.obo.Approver_Phone__c));
            CaseDetailLeft.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("Requestor_Phone__c", oboWrapper.obo.Requestor_Phone__c));
            fields.Add(new KeyValuePair<string, string>("Requestor_Email__c", oboWrapper.obo.Requestor_Email__c));
            fields.Add(new KeyValuePair<string, string>("Third_Party_Billing_Name__c", oboWrapper.obo.Third_Party_Billing_Name__c));
            fields.Add(new KeyValuePair<string, string>("Third_Party_Billing_Address__c", oboWrapper.obo.Third_Party_Billing_Address__c));
            fields.Add(new KeyValuePair<string, string>("Third_Party_Billing_City__c", oboWrapper.obo.Third_Party_Billing_City__c));
            fields.Add(new KeyValuePair<string, string>("Third_Party_Billing_State__c", oboWrapper.obo.Third_Party_Billing_State__c));
            fields.Add(new KeyValuePair<string, string>("Third_Party_Billing_Postal_Code__c", oboWrapper.obo.Third_Party_Billing_Postal_Code__c));
            fields.Add(new KeyValuePair<string, string>("Third_Party_Billing_PO_Number__c", oboWrapper.obo.Third_Party_Billing_PO_Number__c));
            CaseDetailRight.ItemsSource = fields;



            viewLineItem(0);
        }

        private void viewLineItem(int i)
        {
            Console.WriteLine("View Line Item");

            List<KeyValuePair<String, String>> fields = new List<KeyValuePair<string, string>>();
            LineItemNumbers.Children.RemoveRange(1, LineItemNumbers.Children.Count - 1);

            if (oboWrapper.lineItems.Count <= i)
            {
                currentLineItem = 0;
                LineItemDataLeft.ItemsSource = fields;
                LineItemDataRight.ItemsSource = fields;
                return;
            }


            currentLineItem = i;
            for (int j = 0; j < oboWrapper.lineItems.Count; j++)
            {
                Button li = new Button();
                li.BorderThickness = new Thickness(0);
                li.Background = Brushes.Transparent;
                if (j == currentLineItem) li.FontWeight = FontWeights.Bold;
                li.Tag = (j); li.Content = (j + 1);
                li.Click += handle_view_lineitem;
                LineItemNumbers.Children.Add(li);
            }

            LineItemWrapper lineItemWrapper = oboWrapper.lineItems.ElementAt(currentLineItem);
            LineItem lineItem = lineItemWrapper.lineItem;

            fields.Add(new KeyValuePair<string, string>("Line Item Name", lineItem.Name));
            fields.Add(new KeyValuePair<string, string>("Hold Code", lineItem.Hold_Code__c));
            fields.Add(new KeyValuePair<string, string>("Lot Number", lineItem.Lot_Number__c));
            fields.Add(new KeyValuePair<string, string>("Unit Of Measure", lineItem.Unit_of_Measure__c));
            fields.Add(new KeyValuePair<string, string>("Quantity", lineItem.Quantity__c));
            LineItemDataLeft.ItemsSource = fields;

            fields = new List<KeyValuePair<string, string>>();
            fields.Add(new KeyValuePair<string, string>("OBO Name", oboWrapper.obo.Name));
            fields.Add(new KeyValuePair<string, string>("Purchase Order Number", lineItem.Purchase_Order_Number__c));
            fields.Add(new KeyValuePair<string, string>("Owner Part Number", lineItem.Owner_Part_Number__c));
            fields.Add(new KeyValuePair<string, string>("Supplier Part Number", lineItem.Supplier_Part_Number__c));
            fields.Add(new KeyValuePair<string, string>("Rinchem Part Number", lineItem.Rinchem_Part_Number__c));
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