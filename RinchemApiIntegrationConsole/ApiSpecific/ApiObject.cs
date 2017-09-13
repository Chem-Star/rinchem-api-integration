using asnIntegratorConsole.UiSpecific;
using RinchemApiIntegrationConsole;
using RinchemApiIntegrationConsole.ASN;
using RinchemApiIntegrationConsole.OBO;
using RinchemApiIntegrationConsole.OBO2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RinchemApiIntegrator.ApiSpecific
{
    public class ApiObject
    {
        private APImanager apiManager;
        private List<KeyValuePair<String, List<ApiActionObject>>> apiVerbActionMap;

        private String apiType { get; set; }                       // The api that we are interested in calling
        private int apiVerb { get; set; }                          // The verb we use during the API call
        private int apiAction { get; set; }                        // The action that we would like the API to perform


        private List<DataLoader> dataLoaders { get; set; }
        private DataObject dataObject { get; set; }                 // Interface responsible for defining the model type

        public ApiObject(
            APImanager apiManager,
            String apiType,
            List<KeyValuePair<String, List<ApiActionObject>>> apiVerbActionMap, //List of verbs mapped with a list of corresponding actions
            List<DataLoader> dataLoaders,
            DataObject dataObject
            )
        {
            this.apiManager = apiManager;
            this.apiType = apiType;
            this.apiVerbActionMap = apiVerbActionMap;
            this.apiVerb = 0;
            this.apiAction = 0;
            this.dataLoaders = dataLoaders;
            this.dataObject = dataObject;
        }

        public String getApiType()
        {
            return apiType;
        }

        public List<DataLoader> getDataLoaders() { return dataLoaders; }
        public DataObject getDataObject() { return dataObject; }

        public List<String> getApiVerbs()
        {
            List<String> verbs = new List<String>();
            apiVerbActionMap.ForEach(x => verbs.Add(x.Key));
            return verbs;
        }
        public List<String> getApiActions()
        {
            List<String> actions = new List<String>();
            apiVerbActionMap[apiVerb].Value.ForEach(x => actions.Add(x.getName()));
            return actions;
        }

        public String getApiVerb() { return apiVerbActionMap[apiVerb].Key; }
        public void setApiVerb(String verb)
        {
            apiVerb = apiVerbActionMap.FindIndex(x => x.Key == verb);
            if (apiVerb == -1) Console.WriteLine("Specified api verb wasn't found.");
            apiAction = 0;
        }
        public String getApiAction()
        {
            return apiVerbActionMap[apiVerb].Value[apiAction].getName();
        }
        public ApiActionObject getApiActionObject()
        {
            return apiVerbActionMap[apiVerb].Value[apiAction];
        }
        public void setApiAction(String action)
        {
            apiAction = apiVerbActionMap[apiVerb].Value.FindIndex(x => x.getName() == action);
            if (apiAction == -1) apiAction = 0;
        }

        public void deserializeResponse(String response)
        {
            dataObject.deserializeResponse(response);
        }
        public void viewResponse(ApiUserConsole console)
        {
            System.Windows.Window dataResponseViewer = dataObject.getResponseView();
            dataResponseViewer.Owner = console;
            dataResponseViewer.Show();
        }
    }

    public class ApiObjects
    {
        private static ApiObject getAsnApiObject(APImanager apiManager)
        {
            List<DataLoader> AsnDataLoaders = new List<DataLoader>() { new AsnRinchemExcelLoader(), new AsnRinchemJsonLoader() };
            AsnObject asnObject = new AsnObject(); asnObject.initializeRequest();
            List<KeyValuePair<String, List<ApiActionObject>>> asnVerbActionMap =
            new List<KeyValuePair<string, List<ApiActionObject>>>()
            {
                new KeyValuePair<string, List<ApiActionObject>> ( "POST",  new List<ApiActionObject> {
                    new PostNew()
                } ),
                new KeyValuePair<string, List<ApiActionObject>> ( "PATCH", new List<ApiActionObject> {
                    new PatchUpdate(apiManager, asnObject),
                    new PatchCancel(apiManager, asnObject)
                } ),
                new KeyValuePair<string, List<ApiActionObject>> ( "GET",   new List<ApiActionObject> {
                    new GetByName(apiManager, asnObject),
                    new GetByQuery(apiManager, asnObject)
                } ),
            };

            return new ApiObject(
                apiManager,
                "ASN",
                asnVerbActionMap,
                AsnDataLoaders,
                asnObject
            );
        }

        private static ApiObject getOboApiObject(APImanager apiManager) {
            List<DataLoader> OboDataLoaders = new List<DataLoader>() { new OboRinchemExcelLoader(), new OboRinchemJsonLoader() };
            OboObject oboObject = new OboObject(); oboObject.initializeRequest();
            List<KeyValuePair<String, List<ApiActionObject>>> oboVerbActionMap =
            new List<KeyValuePair<string, List<ApiActionObject>>>()
            {
                new KeyValuePair<string, List<ApiActionObject>> ( "POST",  new List<ApiActionObject> {
                    new PostNew()
                } ),
                new KeyValuePair<string, List<ApiActionObject>> ( "PATCH", new List<ApiActionObject> {
                    new PatchUpdate(apiManager, oboObject),
                    new PatchCancel(apiManager, oboObject)
                } ),
                new KeyValuePair<string, List<ApiActionObject>> ( "GET",   new List<ApiActionObject> {
                    new GetByName(apiManager, oboObject),
                    new GetByQuery(apiManager, oboObject)
                } ),
            };

            return new ApiObject(
                apiManager,
                "OBO (old)",
                oboVerbActionMap,
                OboDataLoaders,
                oboObject
            );
        }

        private static ApiObject getOboApi2Object(APImanager apiManager) {
            List<DataLoader> Obo2DataLoaders = new List<DataLoader>() { new Obo2RinchemJsonLoader() };
            Obo2Object obo2Object = new Obo2Object(); obo2Object.initializeRequest();
            List<KeyValuePair<String, List<ApiActionObject>>> obo2VerbActionMap =
            new List<KeyValuePair<string, List<ApiActionObject>>>()
            {
                new KeyValuePair<string, List<ApiActionObject>> ( "POST",  new List<ApiActionObject> {
                    new PostNew()
                } ),
                new KeyValuePair<string, List<ApiActionObject>> ( "PATCH", new List<ApiActionObject> {
                    new PatchUpdate(apiManager, obo2Object),
                    new PatchCancel(apiManager, obo2Object)
                } ),
                new KeyValuePair<string, List<ApiActionObject>> ( "GET",   new List<ApiActionObject> {
                    new GetByName(apiManager, obo2Object),
                    new GetByQuery(apiManager, obo2Object)
                } ),
            };

            return new ApiObject(
                apiManager,
                "OBO",
                obo2VerbActionMap,
                Obo2DataLoaders,
                obo2Object
            );
        }

        public static List<ApiObject> getApiObjects(APImanager apiManager)
        {
            return new List<ApiObject>() {
                getAsnApiObject(apiManager),
                //getOboApiObject(apiManager),
                getOboApi2Object(apiManager) };
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// ACTION OBJECTS
    ////////////////////////////////////////////////////////////////////////////////////////////////
    public abstract class ApiActionObject
    {
        private Boolean usesDataLoader;
        private String name;

        public ApiActionObject(String name, Boolean usesDataLoader)
        {
            this.name = name;
            this.usesDataLoader = usesDataLoader;
        }

        public String getName() { return name; }
        public Boolean getUsesDataLoader() { return usesDataLoader; }
        public abstract List<System.Windows.FrameworkElement> getDataInformationElements();
    }

    //POST NEW
    public class PostNew : ApiActionObject
    {
        public PostNew() : base("NEW", true)
        {
        }

        override
        public List<System.Windows.FrameworkElement> getDataInformationElements()
        {
            return new List<System.Windows.FrameworkElement>();
        }
    }

    //PATCH UPDATE
    public class PatchUpdate : ApiActionObject
    {
        APImanager apiManager;
        DataObject dataObject;
        public PatchUpdate(APImanager apiManager, DataObject dataObject) : base("UPDATE", true)
        {
            this.apiManager = apiManager;
            this.dataObject = dataObject;
        }

        override
        public List<System.Windows.FrameworkElement> getDataInformationElements()
        {
            List<System.Windows.FrameworkElement> elements = new List<System.Windows.FrameworkElement>();
            ApiActionUiElements apiActionUiElements = new ApiActionUiElements(apiManager, dataObject);
            elements.Add(apiActionUiElements.getApiNameFieldElement());
            return elements;
        }
    }

    //PATCH CANCEL
    public class PatchCancel : ApiActionObject
    {
        APImanager apiManager;
        DataObject dataObject;
        public PatchCancel(APImanager apiManager, DataObject dataObject) : base("CANCEL", false)
        {
            this.apiManager = apiManager;
            this.dataObject = dataObject;
        }

        override
        public List<System.Windows.FrameworkElement> getDataInformationElements()
        {
            List<System.Windows.FrameworkElement> elements = new List<System.Windows.FrameworkElement>();
            ApiActionUiElements apiActionUiElements = new ApiActionUiElements(apiManager, dataObject);
            elements.Add(apiActionUiElements.getApiNameFieldElement());
            return elements;
        }
    }

    //GET BY NAME
    public class GetByName : ApiActionObject
    {
        APImanager apiManager;
        DataObject dataObject;
        public GetByName(APImanager apiManager, DataObject dataObject) : base("by Name", false)
        {
            this.apiManager = apiManager;
            this.dataObject = dataObject;
        }

        override
        public List<System.Windows.FrameworkElement> getDataInformationElements()
        {
            List<System.Windows.FrameworkElement> elements = new List<System.Windows.FrameworkElement>();
            ApiActionUiElements apiActionUiElements = new ApiActionUiElements(apiManager, dataObject);
            elements.Add( apiActionUiElements.getApiNameFieldElement() );
            return elements;
        }
    }
    //GET BY QUERY
    public class GetByQuery : ApiActionObject
    {
        APImanager apiManager;
        DataObject dataObject;
        public GetByQuery(APImanager apiManager, DataObject dataObject) : base("by Query", false)
        {
            this.apiManager = apiManager;
            this.dataObject = dataObject;
        }

        override
        public List<System.Windows.FrameworkElement> getDataInformationElements()
        {
            List<System.Windows.FrameworkElement> elements = new List<System.Windows.FrameworkElement>();
            ApiActionUiElements apiActionUiElements = new ApiActionUiElements(apiManager, dataObject);
            elements.Add(apiActionUiElements.getApiQueryFieldElement());
            return elements;
        }
    }







    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // UI ELEMENTS
    public class ApiActionUiElements
    {
        APImanager apiManager;
        DataObject dataObject;
        public ApiActionUiElements(APImanager apiManager, DataObject dataObject)
        {
            this.apiManager = apiManager;
            this.dataObject = dataObject;
        }

        public Grid getApiNameFieldElement()
        {
            int numRows = 1;
            Grid grid = new Grid();
            ColumnDefinition col0 = new ColumnDefinition(); col0.Width = new System.Windows.GridLength(120);
            ColumnDefinition col1 = new ColumnDefinition(); col1.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
            grid.ColumnDefinitions.Add(col0);
            grid.ColumnDefinitions.Add(col1);

            for (int i = 0; i < numRows; i++) grid.RowDefinitions.Add(new RowDefinition());

            TextBlock name = new TextBlock();
            name.Text = "Name";
            name.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            name.Padding = new System.Windows.Thickness(5);
            Grid.SetRow(name, 0); Grid.SetColumn(name, 0);
            grid.Children.Add(name);

            TextBox nameValue = new TextBox();
            nameValue.Text = "";
            nameValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            nameValue.Margin = new System.Windows.Thickness(5);
            nameValue.TextChanged += handle_object_name_changed;
            Grid.SetRow(nameValue, 0); Grid.SetColumn(nameValue, 1);
            grid.Children.Add(nameValue);

            return grid;
        }
        private void handle_object_name_changed(object sender, System.Windows.RoutedEventArgs e)
        {
            apiManager.setObjectName((sender as TextBox).Text);
        }

        public Grid getApiQueryFieldElement()
        {
            int numRows = 1;
            Grid grid = new Grid();
            ColumnDefinition col0 = new ColumnDefinition(); col0.Width = new System.Windows.GridLength(120);
            ColumnDefinition col1 = new ColumnDefinition(); col1.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
            grid.ColumnDefinitions.Add(col0);
            grid.ColumnDefinitions.Add(col1);

            for (int i = 0; i < numRows; i++) grid.RowDefinitions.Add(new RowDefinition());

            TextBlock queryLabel = new TextBlock();
            queryLabel.Text = "Query";
            queryLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            queryLabel.Padding = new System.Windows.Thickness(5);
            Grid.SetRow(queryLabel, 0); Grid.SetColumn(queryLabel, 0);
            grid.Children.Add(queryLabel);

            TextBox queryValue = new TextBox();
            queryValue.Text = "";
            queryValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            queryValue.Margin = new System.Windows.Thickness(5);
            queryValue.TextChanged += handle_query_string_changed;
            Grid.SetRow(queryValue, 0); Grid.SetColumn(queryValue, 1);
            grid.Children.Add(queryValue);

            return grid;
        }
        private void handle_query_string_changed(object sender, System.Windows.RoutedEventArgs e)
        {
            apiManager.setQueryString((sender as TextBox).Text);
        }


    }
}
