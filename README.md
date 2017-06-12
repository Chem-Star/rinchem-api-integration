----------

<p align="center">
  <img src="http://www.rinchem.com/images/logo.gif" alt="Slate: API Documentation Generator" width="226">
</p>

<h1 align="center">Rinchem Easy Link API Integration </h1>



## Synopsis

Rinchem is committed to making the API integration process straightforward, reliable, and transparent. Over time, we will build a robust platform that will allow you to enter and edit documents, review your history, and generate reports. APIs will be available for Advance Shipping Notices (ASN), Confirmation of Inbound Receipts, Outbound Order Releases, Outbound Order Ship Confirmations, and Inventory Reconciliation. Supplementary documents will be available for each document/API type.

Our technical support staff will be available to guide you through every step of the process. If you have only stumbled upon this page and don't have a Rinchem contact, please send an email to 'aseals@rinchem.com'.

If you are looking to get started with the C# source code, please check out the readme in the nested **RinchemApiIntegrationConsole** folder above. If you would like more specific information about the API calls, see the **API Calls** Section directly below this. If you would like to download the pre-built API Integration Console App, check out the **Installation** section further down this page.

## API Calls

### Authentication
Prior to making any API calls, you will need to retrieve an AccessToken and an InstanceUrl from Salesforce. In order to retrieve these, an HTTP 'POST' request should be made to your desired **Authentication Url** (see below). Then, in the request body you need to provide key value pairs, corresponding to your consumer key, consumer secret, username, password and security token. The **key -> value** mapping is shown below.

#### Authentication URLs:
Sandbox (Test)
```
https://test.salesforce.com/services/oauth2/token
```
Production (Live)
```
https://login.salesforce.com/services/oauth2/token
```
#### Authentication (Key -> Value) Mapping
Please note that **key -> value** pairs are not pure json and sending this as a json body may not work. If you are unsure of these values, please see the **My Credentials** section at the bottom of this page.
<table>
<tr><th>Key</th><th>Value</th></tr>
<tr><td>"grant_type"	</td><td>"password"				</td></tr>
<tr><td>"client_id"		</td><td>your_consumer_key		</td></tr>
<tr><td>"client_secret"	</td><td>your_consumer_secret	</td></tr>
<tr><td>"username"		</td><td>your_username			</td></tr>
<tr><td>"password"		</td><td><b>your_password + your_security_token</b></td></tr>
</table>

After sending the authentication request, salesforce will return a json response. 

If the credentials are incorrect, the response will be similar to this: 
```json
{
  "error": "invalid_grant",
  "error_description": "authentication failure"
}
```

If the credentials are correct and the call is made successfully, the response will be something like:
```json
{
  "access_token": "00Dn000000093KQ!ARIAQGlBxv3YTjEomig8JcfIXwWjO8eDp_xQDo8trckJK33b.o85iU8bktoPMLfe6gby_o.7bkXoUESjn3qVswvmlzBJD4ek",
  "instance_url": "https://rinchem--CSPortalQA.cs30.my.salesforce.com",
  "id": "https://test.salesforce.com/id/00Dn000000093KQEAY/005n0000002M68dAAC",
  "token_type": "Bearer",
  "issued_at": "1494884052218",
  "signature": "ivZIqu5EdUGmmAkL964t4YEJ164X08IC97ok7yjKmok="
}
```
From this response, take special note of the access_token and the instance_url as these will be needed to make the actual API Call. 
The access_token and instance_url will remain valid until your salesforce session times out. Salesforce timeouts default to 2 hours, though this can be changed in your org settings.

### Your Data
Depending on which API you are trying to use, your data will be formatted differently. For more information about each data format, see the corresponding **README_X.md** file above. Each call requires a JSON body, however, the fields within the body vary. 

A C# class has been implemented for each API object type, showing the object equivelant of the json. Using a json library, such as Newtonsoft.Json, these objects may be serialized to create the JSON body. The classes also provide some field validation prior to making the call. Rinchem has provided a few examples for how to read in raw data and format the objects. Please see the specific API documentations or the **README** within the nested **RinchemApiIntegrationConsole** folder.

If you have issues converting your data to an appropriate object, please let your Rinchem contact know.

### The Rinchem API Call
Once you have retrieved the access_token and instance_url and you have created your json body, you are ready to call the Rinchem API. The format of the API call is shown below.
```
{
    "Method": <your_verb>, 
    "RequestUri": your_instance_url + <your_api_suffix>, 
    "Version": 1.1, 
    "Headers":
        {
            "Authorization": "Bearer " + <your_access_token>,
            "Accept": "application/json",
            "Content-Type": "application/json; charset=utf-8",
            "Content-Length": 972
        },

    "Content": <your_json_body>
}
```
POST and PATCH should be the primary verbs used, though, please visit the specific API READMEs to see which method verbs (POST, PATCH, GET, PUT, DELETE) are accepted for each API. The other items have been covered priorly.

#### API Suffixes
Depending on your company's needs, you may be assigned a custom suffix. However, if you are using the generic APIs then their suffixes are as follows:
<table>
<tr><td>Advance Shipping Notices (ASN)	  </td><td>"/services/apexrest/v1/ASN__c"   </td></tr>
<tr><td>Outbound Order Release 			  </td><td>"/services/apexrest/v1/OBO_c"    </td></tr>
<tr><td>Confirmation of Inbound Receipt   </td><td>"/services/apexrest/v1/XXX__c"   </td></tr>
<tr><td>Outbound Order Ship Confirmation  </td><td>"/services/apexrest/v1/ZZZ__c"   </td></tr>
<tr><td>Inventory Reconciliation 		  </td><td>"/services/apexrest/v1/QQQ__c"   </td></tr>
</table>

After calling the API, a json response is returned with a status code and a phrase.

If a field item isn't valid or if their is a duplicate item an error will be returned along the likes of this:
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

If the authentication tokens are still valid and the json content body is valid, a response will be returned along the likes of this:
```
{
    "status":"Success",
    "message":"Your request has been imported successfully", 
    "asn_order_num" : unique_asn_id
    your_object_type : your_object_sent
}
```
This includes the status, a message, the content that was originally sent to the api, and the **asn_order_num**. The asn_order_num may be used to reference the proper ASN during PATCH (UPDATE, and DELETE) calls to the system.

**Congrats! You have successfully made an API call to the Rinchem Salesforce server.**


## Pre-built Console Installation
Rinchem has built a small console to aid in your integration pursuit, it can be used as-is to test your credentials in a remote call, as well as  to send test API Calls. If desired, using C#, this can be built upon to function as a full integration solution. If this is your intent, please see the README in the nested RinchemApiIntegrationConsole folder.

<img src="https://cloud.githubusercontent.com/assets/25616817/26076136/f38b2b0a-3974-11e7-9102-b213d3e2f156.PNG" alt="">

If you are just getting started and simply want to download the pre-built console, you may do so by downloading the RinchemAsnIntegrator.zip file (click on the file, then on the subsequent page there is a button to 'Download'). Once downloaded:
1. Extract the zip file to your desired location. 
2. Within the folder, the executable is named **'RinchemApiIntegrationConsole.exe'**
	- Running this will open the empty console. 
	- Please note that there is also a file called 'RinchemApiIntegrationConsole.exe.config', if your file explorer hides extensions you may open this by mistake.
3. Also, within the folder is a nested folder named **'Development'** which contains an example **'profiles.json'** file and an example **'CustomRestPayload.json'** file. 
	- On the console, click the **Set 'profiles.json' Location** button at the top of the User Interface. Navigate to the aforementioned 'Development/profiles.json' file and click 'open'.
4. Fill in your personal credentials and click **save profile** (in the future you'll only have to enter your password).
	- If you are unsure of your credentials, see the **My Credentials** section below.
5. Click **Test Credentials** to see if Salesforce will accept your credentials and return a valid authentication token.
6. In the data loader section, select the RinchemApiIntegrationConsole.RinchemJsonLoader.
7. In the FileLocation field, input the path to the aforementioned 'Development/profiles.json' file.
8. Click **Test Retrieval** to see if the raw data is pulled in properly.
9. Click **Test Reformat** to see if the raw data is converted to an ASN object properly.
10. Click **Test API Call** to send the API request and see if a successful response is received.

If any errors are encountered during the process they will show up in the **Log Output** box. If the API Call is successful the json body that was sent to the API will be returned.

### My Credentials

An **Account Name** and **Password** will be assigned to your company for ASN and other integration methods. In addition to account name and password,  a **Consumer Key**, and **Consumer Secret** will also be provided by your Integration Specialist. 

You will have to manually retrieve your **Security Token** from salesforce. To do so:
1. In a browser, login to **salesforce.com**
2. In the top right corner click on your username and then select **My Settings**
3. On the left bar, click **Personal**
4. Click **Reset My Security Token**
5. In the main page, click the button **Reset My Security Token**
6. Check your email, you should have received an email from Salesforce with your security token.

Your security token will be valid until one of your other credentials is changed, or your salesforce environment is changed.
