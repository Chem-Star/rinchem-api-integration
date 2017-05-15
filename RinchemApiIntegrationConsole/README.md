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
Clone via 
```
$ git clone https://github.com/jdenning-rinchem/asn-integrator-console.git
```
or using the GitHub clone wizard and/or the GitHub Desktop application.
<p align="left">
  <img src="https://image.ibb.co/deCOWF/clonezip.png" alt="Slate: API Documentation Generator">
  <br>
</p>

## Data Loader
The DataLoader is a simple interface class that suppliers must implement in order to use the integrator console. It contains three methods that must be overridden: LoadData, TestData, and ConverDataToAsnObject. Descriptions of the methods may be found below. If the (UserImplementedDataLoader) DataLoader requires any initial startup data, it is recommended that the user pass this in at the APImanager constructor.

#### LoadData()
This function is where the supplier should pull in their raw data. It is implemented as an async task so that the user interface doesn't freeze up. If the data is pulled in successfully, the implementation should return true. If something goes wrong while loading the data, the implementation should return false.

#### TestData()
Although not entirely necessary, it is highly suggested that the supplier implement their own data checking in this method in order to ensure that their raw data has been imported correctly, and the assumed data model is actually in place. If the data is properly validated, the implementation should return true, if something has gone wrong the implementation should return false.

#### ConvertDataToAsnObject()
In order to minimize complexity for the implementers, an AsnObject class has been created. It contains all of the fields that the asn api will accept (More information about the AsnObject can be found below). Implementers should create a new instance of AsnObject and then build up the desired fields. After the fields have been updated, the AsnObject should be returned. After being returned, the APImanager then verifies that all necessary fields have proper values. After verification, the AsnObject is then serialized to a json string and then is ready to be sent to the API call.


## Connecting with the API

Sending ASN data via the API requires some configuration items that your Rinchem Integration Specialist will provide.  For both Sandbox (Test) and Production (Live) Environments:

An **account name** and **password** will be assigned to your company for ASN and other integration methods. 
In addition to account name and password,  a **Consumer Key**, **Consumer Secret**, and **Security Token** will also be provided by your Integration Specialist. 

These should be entered into the credentials section of the GUI.
Profiles are currently loaded and saved to C:/Development/profiles.json
This can be changed in the App.config file.

Saved profiles are not encrypted. Passwords are not stored.


### APImanager
Everything that the GUI executes first runs through the APImanager. It keeps an instance of the current Profile, the SalesForceConnection, the DataLoader, and the asnObject.

### Authentication URLs:

**Sandbox (Test)**
```
https://test.salesforce.com/services/oauth2/token
```

**Production (Live)**
```
https://login.salesforce.com/services/oauth2/token
```

### API URLs:

**Sandbox (Test)**
```
https://salesforceinstance.rinchem.com/services/apexrest/v1/ASN__c
```
**Production (Live)**

```
https://salesforceinstance.rinchem.com/services/apexrest/v1/ASN__c
```

Depending on authentication, a unique URI is created for sending the request/
```
https://rinchem--ASNQA.cs60.my.salesforce.com/services/data/v37.0/sobjects/ASN__c
```

To create an ASN request, a message is populated in collaboration with data from your existing system which will be parsed into ours. Data will be passed in JSON format

### AsnObject
The AsnObject is a class that mimicks the following json format.
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
### Required Fields:
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


### ASN integration supports  these HTTP methods:
```
POST 
```


a request is then sent to our API with method and content
```
{Method: POST, RequestUri: 'https://rinchem--asnqa.cs60.my.salesforce.com/services/data/v37.0/sobjects/ASN__c', Version: 1.1, Content: <"this will be body of asn">, Headers:
{
  Authorization: Bearer fdasfdsafsd50Hz!fdasfdsafdsaf.fdasfdsafdsafdsafdsafsdaf
  Accept: application/json
  Content-Type: application/json; charset=utf-8
  Content-Length: 972
}}
```

A response is given with a status code and a phrase.
