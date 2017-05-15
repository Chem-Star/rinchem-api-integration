----------

<p align="center">
  <img src="http://www.rinchem.com/images/logo.gif" alt="Slate: API Documentation Generator" width="226">
  <br>
  <a href="https://github.com/Chem-Star/asn-integration"><img src="https://travis-ci.org/lord/slate.svg?branch=master" alt="Build Status"></a>
</p>

<h1 align="center">ASN-Integration documentation for integration to API</h1>

## Synopsis

We are committed to making the ASN integration straightforward, reliable, and transparent. Over time, we will build a robust platform that will allow you to enter and edit ASNs, review your history, and generate reports. 
Our technical support staff will guide you through every step of the process.

## Motivation

The ASN provides order and shipment information in one electronic transaction sent from the shipper to the receiver. While the ASN is similar to a Bill of Lading (BOL) and even carries much of the same information, it has a different function. The BOL is meant to accompany a shipment along its path. An ASN is intended to provide information in advance of the shipment arriving at its destination.
The value of the ASN comes from receiving it prior to the actual shipment. Rinchem provides a standardized data integration method to ensure easy, efficient, and accurate data exchange. 

----------
## Installation
If you are looking to get started with the source code, please check out the readme in the nested **asnIntegratorConsole** folder above.

If you are just getting started and simply want to download the pre-built console, you may do so by downloading the RinchemAsnIntegrator.zip file (click on the file, then on the subsequent page there is a button to 'Download'). Once downloaded, extract the zip file to your desired location. Within the folder, the executable is named **'asnIntegratorConsole.exe'**; if you launch this the empty console will open. Also, within the folder is a nested folder named **'Development'** within this there is an example **'profiles.json'** file and an example **'CustomRestPayload.json'** file. 
If you would like the console to be populated initially, you may move the 'Development' folder to 'C:/', otherwise you may point the UI to a different location by clicking the **Set 'profiles.json' Location"** button at the top of the User Interface.


## Interface
The interface is broken down into 5 sections so that the customer path to integration is minimized and itemizable. The sections consist of Credentials, Load Data, Convert Data, API Setup and API Call.

### Credentials
Connecting to Salesforce environment is the first step to sending ASN data.
Connecting to the environment requires some configuration items that your Rinchem Integration Specialist will provide.  For both Sandbox (Test) and Production (Live) Environments:

An **account name** and **password** will be assigned to your company for ASN and other integration methods. 
In addition to account name and password,  a **Consumer Key**, **Consumer Secret**, and **Security Token** will also be provided by your Integration Specialist. 

These should be entered into the corresponding fields in the credentials section of the GUI. (If you don't see the fields, please click the 'Edit Profile' button) After, you have entered your credential information, it is suggested that you 'Save' it. This prevents you from needing to re-enter data the next time that you use the console. For security reasons, the password is not saved, and will have to be entered each time.


### Data Retrieval
The two data sections are where the customer will have to implement their own solution. Data retrieval consists of connecting to the current raw data source that needs to be sent through the API. It should also verify that the expected data format was pulled in. 
When the desired data loader is selected from the drop down, any specified 'custom fields' will be displayed below it. Currently, the only built in solution is the RinchemJsonLoader, to use this, you will need to provide the location of a .json file that is already in the proper ASN format.

### Data Conversion
Before the API call can be sent, the data must be in a very specific format. During this call the user must convert their raw data object, to the proper AsnObject format. The system then verifies that all required fields are inputed and subsequently serializes the AsnObject into a json string that can then be transmitted in the API call.

#### AsnObject
The AsnObject is a simple class following the ASN json format.
```
{
	"rqst": {
		"asn": {
			"Name": "",
			"Action__c": "xxx",
			"Message_Id__c": "xxx",
			"Date_ASN_Sent__c": "xxxx-xx-xx",
			"Supplier_Name__c": "xxx",
			"Rinchem_Supplier_Id__c": "xxx",
			"ASN_Recipient_Name__c": "xxxx",
			"ASN_Recipient_Id__c": "xxxxx",
			"Template_Version__c": "xx.xx",
			"Estimated_Ship_Date__c": "xxxx-xx-xx",
			"Estimated_Arrival_Date__c": "xxxx-xx-xx",
			"Shipment_Id__c": "xxx xxxxxx",
			"BOL_Number__c": "",
			"Ship_From_Supplier__c": "xxx",
			"Order_Type__c": "xx",
			"Order_Number__c": "",
			"Origin_Id__c": "xxx",
			"Origin_Street_Address__c": "xxx Rinchem Dr.",
			"Origin_City__c": "City",
			"Origin_State__c": "State",
			"Origin_Postal_Code__c": "123445",
			"Origin_Country__c": "USA",
			"Destination_Name__c": "",
			"Destination_Warehouse_Code__c": "xx",
			"Destination_Address__c": "1023 address ave nw",
			"Destination_City__c": "city",
			"Destination_State__c": "state",
			"Destination_Postal_Code__c": "123445",
			"Destination_Country__c": "USA",
			"Carrier_Id__c": "ddd",
			"Carrier_Name__c": "xxx",
			"Purchase_Order_Number__c": "xxx",
			"Product_Owner_Id__c": "xxx"
		},
		"lineItems": [{
			"Name": "1",
			"Vendor_Part_Number__c": "dfdasf_part",
			"Product_Description__c": "This is an example line item",
			"Product_Lot_Number__c": "213123",
			"Product_Expiration_Date__c": "2312-23-01",
			"Quantity__c": "343290",
			"Unit_of_Measure__c": "DRUM",
			"Hold_Code__c": "",
			"Serial_Number__c": ""
		},
		{
			"Name": "2",
			"Vendor_Part_Number__c": "dfdasf_part",
			"Product_Description__c": "This is an example line item",
			"Product_Lot_Number__c": "213123",
			"Product_Expiration_Date__c": "2312-23-01",
			"Quantity__c": "343290",
			"Unit_of_Measure__c": "DRUM",
			"Hold_Code__c": "",
			"Serial_Number__c": ""
		}]
	}
}
```
#### Required Fields:
If any of the following required fields are missing, the reformat will fail and the guilty fields will be listed in the **Log Output**.
```
{
	"rqst": {
		"asn": {
			"Message_Id__c": "xxx",
			"Date_ASN_Sent__c": "xxxx-xx-xx",
			"Supplier_Name__c": "xxx",
			"Rinchem_Supplier_Id__c": "xxx",
			"ASN_Recipient_Name__c": "xxxx",
			"ASN_Recipient_Id__c": "xxxxx",
			"Template_Version__c": "xx.xx",
			"Estimated_Ship_Date__c": "xxxx-xx-xx",
			"Estimated_Arrival_Date__c": "xxxx-xx-xx",
			"Shipment_Id__c": "xxx xxxxxx",
			"Order_Type__c": "xx",
			"Origin_Id__c": "xxx",
			"Destination_Warehouse_Code__c": "xx",
			"Carrier_Id__c": "ddd",
			"Carrier_Name__c": "xxx",
			"Purchase_Order_Number__c": "xxx",
			"Product_Owner_Id__c": "xxx"
		},
		"lineItems": [{
			"Name": "1",
			"Vendor_Part_Number__c": "dfdasf_part",
			"Product_Description__c": "This is an example line item",
			"Quantity__c": "343290",
			"Unit_of_Measure__c": "DRUM"
		}]
	}
}
```


### API Call
The API Call serializes all of the pre-entered and converted data into one json strong string and then sends it over an HTTPclient to the Salesorce server, where the API receives and handles the package.