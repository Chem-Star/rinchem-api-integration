----------

<p align="center">
  <img src="http://www.rinchem.com/images/logo.gif" alt="Slate: API Documentation Generator" width="226">
</p>

<h1 align="center">Rinchem API Integration - Outbound Orders</h1>

## Motivation

The Outbound Order, OBO, provides details related to orders being shipped from Rinchem either to a facility or to another Rinchem warehouse. By integrating with the API, this information becomes more consistent, more secure, and more easily accessible where and when you need it.

## Calling The OBO API
If you have not done so already, please review the documentation in the main README.md as that walks you through the process to get the necessary **instance_url** and **authentication_token**, as well as how to format the call once you have the correct body payload. 

#### OBO Suffix:
Append this to your Salesforce **instance_url**.
```
/services/apexrest/v1/OBO__c
```

#### OBO Accepted HTTP Verbs:
<Table>
<tr><th>Verb</th><th>Actions</th></tr>
<tr><td>POST</td><td>Used for adding a new OBO</td></tr>
<tr><td>PATCH</td><td>Used to Update or Cancel an OBO</td></tr>
</Table>


### Body Payload: 
For a new OBO request the API expects a body payload with the following json format. In order to make this more accessible, a C# object representation has been created that mimicks this format. By building up the C# representation, the json body can then be created with a simple serialize call with a json library such as 'Newtonsoft.JSON'.

```
{
	"rqst": {
		"obo": {
			"Name": "",
			"Action__c": "",
			"Message_Id__c": "",
			"Order_Date__c": "2017-04-20",
			"Rinchem_Supplier_Id__c": "MAL",
			"Purchase_Order_Number__c": "153329",
			"Ship_To_Customer__c": "INTEL",
			"Ship_To_Name__c": "Intel",
			"Freight_Payment_Terms_Type__c": "",
			"Bill_To_Customer_Code__c": "",
			"Bill_To_Name__c": "",
			"Desired_Delivery_Date__c": "2017-04-21",
			"Carrier_Service__c": "FRGHTWORKS",
			"From_Warehouse_Code__c": "Chandler",
			"Order_Type__c": "PO",
			"Product_Owner_Id__c": "MAL"
		},
		"lineItems": [{
			"Name": "1",
			"Outbound_Order__c": "",
			"Hold_Code__c": "",
			"Inventory_Detail__c": "",
			"Lot_Number__c": "",
			"Rinchem_Part_Number__c": "",
			"Quantity__c": "",
			"Unit_of_Measure__c": ""
		},		
		{
			"Name": "2",
			"Outbound_Order__c": "",
			"Hold_Code__c": "",
			"Inventory_Detail__c": "",
			"Lot_Number__c": "",
			"Rinchem_Part_Number__c": "",
			"Quantity__c": "",
			"Unit_of_Measure__c": ""
		}]
	}
}
```
#### Required Fields:
Requests will not be accepted into the system if any of these fields have improper values. The following sections outline accepted values for some of the more specialized fields such as unit of measure and hold codes.
```
{
	"rqst": {
		"obo": {
			"Name": "",
			"Action__c": "",
			"Message_Id__c": "",
			"Order_Date__c": "YYYY-MM-DD",
			"Rinchem_Supplier_Id__c": "xxx",
			"Purchase_Order_Number__c": "xxxxxx",
			"Ship_To_Customer__c": "",
			"Ship_To_Name__c": "",
			"Freight_Payment_Terms_Type__c": "",
			"Bill_To_Customer_Code__c": "",
			"Bill_To_Name__c": "",
			"Desired_Delivery_Date__c": "YYYY-MM-DD",
			"Carrier_Service__c": "",
			"From_Warehouse_Code__c": "",
			"Order_Type__c": "PO",
			"Product_Owner_Id__c": "XXX"
		},
		"lineItems": [{
			"Name": "1",
			"Outbound_Order__c": "",
			"Hold_Code__c": "",
			"Inventory_Detail__c": "",
			"Lot_Number__c": "",
			"Rinchem_Part_Number__c": "",
			"Quantity__c": "",
			"Unit_of_Measure__c": ""
		}]
	}
}
```


### Accepted Carrier IDs
For a full list of Carrier IDs please see the attached excel sheet **AcceptedValues.xlsx**, under the *Carrier_Id* tab.
<Table>
<tr><th>Warehouse</th><th>Carrier Service</th><th>Carrier Service Name</th></tr>
<tr> <td>11</td> <td>RINCHEM</td> <td>Rinchem</td> </tr>
<tr> <td>11</td> <td>CGL</td> <td>CGL</td> </tr>
<tr> <td>11</td> <td>FRGHTWORKS</td> <td>FREIGHTWORKS</td> </tr>
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
Also available in the AcceptedValues excel sheet, under the *Hold_Code* tab.
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
Also available in the AcceptedValues excel sheet, under the *Unit_Of_Measure* tab.
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
