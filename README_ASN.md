----------

<p align="center">
  <img src="http://www.rinchem.com/images/logo.gif" alt="Slate: API Documentation Generator" width="226">
</p>

<h1 align="center">Rinchem API Integration - ASN </h1>

## Motivation

The Advance Shipping Notice, ASN, provides order and shipment information in one electronic transaction sent from the shipper to the receiver. While the ASN is similar to a Bill of Lading (BOL) and even carries much of the same information, it has a different function. The BOL is meant to accompany a shipment along its path. An ASN is intended to provide information in advance of the shipment arriving at its destination.
The value of the ASN comes from receiving it prior to the actual shipment. Rinchem provides a standardized data integration method to ensure easy, efficient, and accurate data exchange. 

###API URLs:

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

### Body Payload: 
The API expects a body payload with a specific json format (see below). In order to make this more accessible, a C# object representation has been created that mimicks this format. By building up the C# representation, the json body can then be created with a simple serialize call with a json library such as 'Newtonsoft.JSON'.

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
PATCH
```

A request may then be sent to our API with method and content. The RequestUri is composed with the InstanceUrl (returned by Salesforce during credential authorization) and then appends the desired API url. In the Headers section, 'Authorization' is composed of the text, 'Bearer ' and then appends the AccessToken (returned by Salesforce during credential authorization). 'Content' is composed of the JSON body mentioned above.
```
{
	Method: POST, 
    RequestUri: 'https://rinchem--asnqa.cs60.my.salesforce.com/services/data/v37.0/sobjects/ASN__c', 
    Version: 1.1, 
    Headers:
		{
  			Authorization: Bearer fdasfdsafsd50Hz!fdasfdsafdsaf.fdasfdsafdsafdsafdsafsdaf
  			Accept: application/json
  			Content-Type: application/json; charset=utf-8
  			Content-Length: 972
		},

    Content: <"the ASN json body goes here">
}
```

A response is then returned with a status code and a phrase.

###Types of Responses:

 ***Success:***
```
{"status":"Success","message":"Your request has been imported successfully"}
```
It will also return the ASN body so that you may ensure the data was not corrupted in transit.


***Error:***
```
{StatusCode: 400, ReasonPhrase: 'Bad Request', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  Strict-Transport-Security: max-age=31536000; includeSubDomains
  Sforce-Limit-Info: api-usage=674/5000000
  Transfer-Encoding: chunked
  Date: Wed, 08 Feb 2017 19:02:28 GMT
  Set-Cookie: BrowserId=sXXZUL6JRu6h-PKrICyowg;Path=/;Domain=.salesforce.com;Expires=Sun, 09-Apr-2017 19:02:28 GMT
  Content-Type: application/json; charset=UTF-8
  Expires: Thu, 01 Jan 1970 00:00:00 GMT
}}

"[{\"message\":\"duplicate value found: Message_Id__c duplicates value on record with id: a1q3C000000AqUp\",\"errorCode\":\"DUPLICATE_VALUE\",\"fields\":[]}]"
```

More information about specific errors may be added at a later time. If you do encounter an undocumented error, please don't hesitate to ask your Rinchem contact, or 'jdenning@rinchem.com'.