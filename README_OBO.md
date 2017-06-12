----------

<p align="center">
  <img src="http://www.rinchem.com/images/logo.gif" alt="Slate: API Documentation Generator" width="226">
</p>

<h1 align="center">Rinchem API Integration - Outbound Orders</h1>

## Motivation

The Outbound Order, **OBO**, provides details related to orders being shipped from Rinchem either to a facility or to another Rinchem warehouse. By integrating with the API, this information becomes more consistent, more secure, and more easily accessible where and when you need it.

## Calling The OBO API
If you have not done so already, please review the documentation in the main **README.md** as that walks you through the process to get the necessary **instance_url** and **authentication_token**, as well as how to format the call once you have the correct body payload. 

#### OBO Suffix:
Append the following suffix to your Salesforce **instance_url**.
```
/services/apexrest/v1/OBO__c
```

#### OBO Accepted HTTP Verbs:
<Table>
<tr><th>Verb</th><th>Actions</th><th>Purpose</th></tr>
<tr><td>POST</td><td>N/A</td><td>Used for adding a new OBO</td></tr>
<tr><td>PATCH</td><td>UPDATE, CANCEL</td><td>Used to Update or Cancel an OBO</td></tr>
</Table>


### Body Payload: 
For a new OBO request the API expects a body payload with the JSON format displayed below. The demonstrated payload shows all fields that the API can handle, though some of these are not required. Please see the following sections for minimum permissible payloads for each VERB type.

In order to make the payloads more accessible, a C# object representation has been created which mimics this format. By building up the C# representation, the JSON body can then be created with a simple serialize call with a JSON library such as 'Newtonsoft.JSON'.

```
{
	"rqst": {
		"obo": {
			"Name": "",
			"Action__c": "",
			"Message_Id__c": "",
			"Order_Date__c": "YYYY-MM-DD",
			"Rinchem_Supplier_Id__c": "XXX",
			"Purchase_Order_Number__c": "xxxxxxx",
			"Ship_To_Customer__c": "xxxxxx",
			"Ship_To_Name__c": "xxxxxx",
			"Freight_Payment_Terms_Type__c": "xx",
			"Bill_To_Customer_Code__c": "xxxxxxx",
			"Bill_To_Name__c": "xxxxxxx",
			"Desired_Delivery_Date__c": "2017-04-21",
			"Carrier_Service__c": "xxxxxxx",
			"From_Warehouse_Code__c": "xxxxxxx",
			"Order_Type__c": "XX",
			"Product_Owner_Id__c": "XXX"
		},
		"lineItems": [
    		{
    			"Name": "1",
    			"Outbound_Order__c": "OBO-XXXXX",
    			"Hold_Code__c": "XX",
    			"Inventory_Detail__c": "xxxxxxx",
    			"Lot_Number__c": "XXXX",
    			"Rinchem_Part_Number__c": "xxxxxx_part",
    			"Quantity__c": "XX",
    			"Unit_of_Measure__c": "xxxxx"
    		},		
    		{
    			"Name": "2",
    			"Outbound_Order__c": "OBO-XXXXX",
    			"Hold_Code__c": "XX",
    			"Inventory_Detail__c": "xxxxxxx",
    			"Lot_Number__c": "XXXX",
    			"Rinchem_Part_Number__c": "xxxxxx_part",
    			"Quantity__c": "XX",
    			"Unit_of_Measure__c": "xxxxx"
    		}
		]
	}
}
```
#### Required Fields - POST:
POST requests will not be accepted into the WMS if any of these fields have improper values. The **Accepted Values** section below outlines permissible values for some of the more specialized fields such as *Units of Measure* and *Hold Codes*.
```
{
	"rqst": {
		"obo": {
			"Message_Id__c": "",
			"Order_Date__c": "YYYY-MM-DD",
			"Rinchem_Supplier_Id__c": "xxx",
			"Purchase_Order_Number__c": "xxxxxx",
			"Ship_To_Customer__c": "",
			"Freight_Payment_Terms_Type__c": "",
			"Bill_To_Customer_Code__c": "",
			"Desired_Delivery_Date__c": "YYYY-MM-DD",
			"Carrier_Service__c": "",
			"From_Warehouse_Code__c": "",
			"Order_Type__c": "PO",
			"Product_Owner_Id__c": "XXX"
		},
		"lineItems": [{
			"Name": "1",
			"Rinchem_Part_Number__c": "",
			"Quantity__c": "",
			"Unit_of_Measure__c": ""
		}]
	}
}
```
#### Required Fields - PATCH
In order to use the PATCH verb, the payload must provide the unique order id (returned after a successful POST call) in the *'Name'* field, and the desired action in the *'Action__c'* field. Any other fields that are provided will be updated with their new values.
```
{
	"rqst": {
		"obo": {
			"Name": "xxxxx",
			"Action__c": "xxxxxx",
		}
	}
}
```


### Accepted Carrier IDs
For a full list of Carrier IDs please see the attached excel sheet **AcceptedValues.xlsx**, under the *Carrier_Id* tab.
<Table>
<tr><th>Warehouse</th><th>Carrier Service</th><th>Carrier Service Name</th></tr>
<tr> <td>11</td> <td>CGL</td> <td>CGL</td> </tr>
<tr> <td>11</td> <td>RINCHEM</td> <td>Rinchem</td> </tr>
</Table>

### Accepted Warehouse IDs
For a full list of Warehouse IDs please see the attached excel sheet **AcceptedValues.xlsx**, under the *Warehouse_Code* tab.

<Table>
<tr><th>Code</th><th>Name</th><th>Address</th><th>City</th><th>State</th><th>Zip</th><th>Country</th></tr>
<tr>
<td>14</td> <td>Chandler</td> <td>6843 W. Frye Rd.</td> <td>Chandler</td> <td>AZ</td> <td>85226</td> <td>USA</td> 
</tr>

</Table>

### Accepted Hold Codes
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

### Accepted Units of Measure
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
