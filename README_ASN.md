----------

<p align="center">
  <img src="http://www.rinchem.com/images/logo.gif" alt="Slate: API Documentation Generator" width="226">
</p>

<h1 align="center">Rinchem API Integration - ASN </h1>

## Motivation

The Advance Shipping Notice, **ASN**, provides inbound order and shipment information in one electronic transaction sent from the shipper to Rinchem. An ASN is intended to provide information in advance of the shipment arriving at its destination. The value of the ASN comes from receiving it prior to the actual shipment. Rinchem provides a standardized data integration method to ensure easy, efficient, and accurate data exchange. 

## Calling The ASN API
If you have not done so already, please review the documentation in the main **README.md** as that walks you through the process to get the necessary **instance_url** and **authentication_token**, as well as how to format the call once you have the correct body payload. 

#### ASN Suffix:
Append the following suffix to your Salesforce **instance_url**.
```
/services/apexrest/v1/ASN__c
```

#### ASN Accepted HTTP Verbs:
<Table>
<tr><th>Verb</th><th>Actions</th><th>Purpose</th></tr>
<tr><td>POST</td><td>N/A</td><td>Used for adding a new ASN</td></tr>
<tr><td>PATCH</td><td>UPDATE, CANCEL</td><td>Used to Update or Cancel an ASN</td></tr>
</Table>


### Body Payload - POST and PATCH: 
For a new ASN request the API expects a body payload with the JSON format displayed below. The demonstrated payload shows all fields that the API can handle, though some of these are not required. Please see the following sections for minimum permissible payloads for each VERB type.

In order to make the payloads more accessible, a C# object representation has been created which mimics this format. By building up the C# representation, the JSON body can then be created with a simple serialize call with a JSON library such as 'Newtonsoft.JSON'.

```
{
	"rqst": {
		"asn": {
			"Name": "",
			"Action__c": "xxx",
			"Message_Id__c": "xxx",
			"Date_ASN_Sent__c": "YYYY-MM-DD",
			"Supplier_Name__c": "xxx",
			"Rinchem_Supplier_Id__c": "xxx",
			"ASN_Recipient_Name__c": "xxxx",
			"ASN_Recipient_Id__c": "xxxxx",
			"Template_Version__c": "xx.xx",
			"Estimated_Ship_Date__c": "YYYY-MM-DD",
			"Estimated_Arrival_Date__c": "YYYY-MM-DD",
			"Shipment_Id__c": "xxx xxxxxx",
			"BOL_Number__c": "",
			"Ship_From_Supplier__c": "xxx",
			"Order_Type__c": "xx",
			"Order_Number__c": "",
			"Origin_Id__c": "xxx",
			"Origin_Street_Address__c": "xxx Rinchem Dr.",
			"Origin_City__c": "City",
			"Origin_State__c": "State",
			"Origin_Postal_Code__c": "xxxxx",
			"Origin_Country__c": "USA",
			"Destination_Name__c": "",
			"Destination_Warehouse_Code__c": "xx",
			"Destination_Address__c": "xxxx Address Ave NW",
			"Destination_City__c": "city",
			"Destination_State__c": "state",
			"Destination_Postal_Code__c": "xxxxx",
			"Destination_Country__c": "USA",
			"Carrier_Id__c": "ddd",
			"Carrier_Name__c": "xxx",
			"Purchase_Order_Number__c": "xxx",
			"Product_Owner_Id__c": "xxx"
		},
		"lineItems": [{
			"Name": "1",
			"Vendor_Part_Number__c": "xxxxxx_part",
			"Product_Description__c": "This is an example line item",
			"Product_Lot_Number__c": "xxxxxx",
			"Product_Expiration_Date__c": "YYYY-MM-DD",
			"Quantity__c": "XX",
			"Unit_of_Measure__c": "DRUM",
			"Hold_Code__c": "",
			"Serial_Number__c": ""
		},
		{
			"Name": "2",
			"Vendor_Part_Number__c": "xxxxxx_part",
			"Product_Description__c": "This is an example line item",
			"Product_Lot_Number__c": "xxxxxx",
			"Product_Expiration_Date__c": "YYYY-MM-DD",
			"Quantity__c": "xx",
			"Unit_of_Measure__c": "DRUM",
			"Hold_Code__c": "",
			"Serial_Number__c": ""
		}]
	}
}
```
#### Required Fields - POST:
POST requests will not be accepted into the WMS if any of these fields have improper values. The **Accepted Values** section below outlines permissible values for some of the more specialized fields such as *Units of Measure* and *Hold Codes*.

```
{
	"rqst": {
		"asn": {
			"Date_ASN_Sent__c": "2017-04-20",
			"Supplier_Name__c": "Example Supplier, INC",
			"Rinchem_Supplier_Id__c": "EXA",
			"ASN_Recipient_Name__c": "Rinchem",
			"ASN_Recipient_Id__c": "16",
			"Template_Version__c": "1.0",
			"Estimated_Ship_Date__c": "2017-03-24",
			"Estimated_Arrival_Date__c": "2017-03-24",
			"Order_Type__c": "PO",
			"Destination_Warehouse_Code__c": "11",
			"Carrier_Id__c": "RINCHEM",
			"Carrier_Name__c": "Rinchem",
			"Purchase_Order_Number__c": "123456",
			"Product_Owner_Id__c": "XXXXXX"
		},
		"lineItems": [{
			"Vendor_Part_Number__c": "1234-01",
			"Product_Description__c": "EXAMPLE OXIDE, 65%",
			"Quantity__c": "4",
			"Unit_of_Measure__c": "BOTTLE",
		}]
	}
}
```

#### Required Fields - PATCH
In order to use the PATCH verb, the payload should provide the unique order id (returned after a successful POST call) in the *'Name'* field, and the desired action in the *'Action__c'* field. Any other fields that are provided will be updated with their new values.

```
{
	"rqst": {
		"asn": {
			"Name": "xxxxx",
			"Action__c": "xxxxxx",
		}
	}
}
```

### Request Format - GET: 
To retrieve ASN data a get call must be submitted. Similar to the POST and PATCH calls, you will once again need to follow the authentication procedure to acquire your **instance_url** and **authentication_token**; the **ASN API Suffix**. However, no payload is sent with the request, rather, the desired parameters are appended to the end of the request URI. The first 10 ASNs matching the request will then be returned in a payload with format as described below. 

Rinchem has provided a few methods to accomplish queries. *Common Queries* require a query type and a value. *Custom Queries* give the developer the ability to build their own queries using the SOQL format. 

#### Common Queries
 All *Common Queries* follow the same basic format.
 ```
<instance_url>/<API_suffix>?<query_type>=<query_value>
```
For example,
```
https://rinchem--CSportalQA.cs66.my.salesforce.com/services/apexrest/v1/ASN__c?Name=ASN0000000457
```
Accepted *Common Query* types and examples are listed below.
<Table>
<tr><th>Query Type</th><th>Related Fields</th><th>Example Query Request</th></tr>
<tr><td>Name</td><td>Name</td><td><b>?</b>Name=ASN-00305</td></tr>
<tr><td>ShipId</td><td>Shipment_Id__c</td><td><b>?</b>ShipId=0080121374</td></tr>
</Table>

#### Custom Queries
*Custom Queries* have a similar format, however, < query_type> will always be 'query' and < query_value> will be custom query conditions written with SOQL format. A few examples are shown below.
<Table>
<tr><th>Description</th><th>Example Query Request</th></tr>
<tr><td>Expires On Date</td><td><b>?</b>Expiration_Date__c='2017-06-28'</td></tr>
<tr><td>Created After Date</td><td><b>?</b>CreatedDate>2017-06-27T00:00:00Z</td></tr>
<tr><td>Created Between Dates</td><td><b>?</b>CreatedDate&lt;2017-06-27T00:00:00Z <b>and</b> CreatedDate&gt;2017-06-18T00:00:00Z </td></tr>
</Table>

## Accepted Values
#### Accepted Carrier IDs
For a full list of Carrier IDs please see the attached excel sheet **AcceptedValues.xlsx**, under the *Carrier_Id* tab.
<Table>
<tr><th>Warehouse</th><th>Carrier Service</th><th>Carrier Service Name</th></tr>
<tr> <td>11</td> <td>CGL</td> <td>CGL</td> </tr>
<tr> <td>11</td> <td>RINCHEM</td> <td>Rinchem</td> </tr>
</Table>

#### Accepted Warehouse IDs
For a full list of Warehouse IDs please see the attached excel sheet **AcceptedValues.xlsx**, under the *Warehouse_Code* tab.

<Table>
<tr><th>Code</th><th>Name</th><th>Address</th><th>City</th><th>State</th><th>Zip</th><th>Country</th></tr>
<tr>
<td>14</td> <td>Chandler</td> <td>6843 W. Frye Rd.</td> <td>Chandler</td> <td>AZ</td> <td>85226</td> <td>USA</td> 
</tr>

</Table>

#### Accepted Hold Codes
Hold Codes are also available in the AcceptedValues excel sheet, under the *Hold_Code* tab.
<Table>
<tr><th>Hold Code</th><th>Description</th><th>Affect Damaged</th></tr>
<tr> <td>DMG</td>  <td>DAMAGED                              </td> <td>Yes</td> </tr>
<tr> <td>OH</td>   <td>ON HOLD                              </td> <td>No</td> </tr>
<tr> <td>RCAL</td> <td>RECALLED PRODUCT                     </td> <td>Yes</td> </tr>
<tr> <td>REJ</td>  <td>REJECTED PRODUCT                     </td> <td>Yes</td> </tr>
<tr> <td>RET</td>  <td>RETURNABLE CONTAINER                 </td> <td>No</td> </tr>
<tr> <td>RSL</td>  <td>RESELL                               </td> <td>Yes</td> </tr>
<tr> <td>SPCL</td> <td>SPECIAL AUTHORIZATION REQUIRED       </td> <td>Yes</td> </tr>
<tr> <td>VH</td>   <td>VENDOR HOLD                          </td> <td>No</td> </tr>
<tr> <td>WST</td>  <td>WASTE                                </td> <td>Yes</td> </tr>

</Table>

#### Accepted Units of Measure
Units of Measure are also available in the AcceptedValues excel sheet, under the *Unit_Of_Measure* tab.
<Table>
<tr>
<td>TOTE</td>
<td>PAIL</td>
<td>NOW PAK</td>
<td>CANISTER</td>
</tr>
<tr>
<td>QUARTZ</td>
<td>EACH</td>
<td>CYLINDER</td>
<td>PACK</td>
</tr>
<tr>
<td>AMPULE</td>
<td>DIPTUBE</td>
<td>BAG</td>
<td>TUBES</td>
</tr>
<tr><td>BOX</td>
<td>JERRICAN</td>
<td>2LPO</td>
<td>6 PACK CYL</td>
</tr>
<tr>
<td>BULK SACK</td>
<td>POUND</td>
<td>ROLL</td>
<td>CARTON</td>
</tr>
<tr>
<td>PALLET</td>
<td>OTHER</td>
<td>CAGE</td>
<td>PALLET</td>
</tr>
</Table>




# Errors
Information about specific errors may be added at a later time. If you do encounter an undocumented error, please don't hesitate to ask your Rinchem contact, or 'aseals@rinchem.com'.