using RinchemApiIntegrationConsole;
using RinchemApiIntegrationConsole.UiSpecific;
using RinchemApiIntegrator.UiSpecific;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace asnIntegratorConsole.UiSpecific
{
    public partial class ApiUserConsole : Window
    {
        APImanager apiManager;
        LogOutputDialog logOutputDialog;
        AsnResponseViewer asnResponseViewer;
        Boolean debug;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     Consturctor and Lifecycle
        public ApiUserConsole()
        {
            InitializeComponent();
            ConsoleLogger.initialize(this);
            apiManager = new APImanager();
            logOutputDialog = new LogOutputDialog();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            setDebugConsole(false);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     UI EVENT HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public void setDebugConsole(Boolean active)
        {
            if (active)
            {
                debug = true;

                DebugButtons.Visibility = Visibility.Visible;
                ProductionButtons.Visibility = Visibility.Collapsed;
                LogOutputContainer.Visibility = Visibility.Visible;

                this.Height += 250;
            }
            else
            {
                debug = false;

                DebugButtons.Visibility = Visibility.Collapsed;
                ProductionButtons.Visibility = Visibility.Visible;
                LogOutputContainer.Visibility = Visibility.Collapsed;

                this.Height -= 250;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     Custom Data Loader View
        public void handle_populate_data_loader_grid(object sender, RoutedEventArgs e)
        {
            CustomDataLoaderGrid.Children.RemoveRange(0, CustomDataLoaderGrid.Children.Count);

            int i = 0;

            if (apiManager == null ||
                apiManager.getCurrentDataLoader() == null ||
                apiManager.getCurrentDataLoader().GetCustomFields() == null) return;

            apiManager.getCurrentDataLoader().GetCustomFields().ForEach(customField =>
            {
                TextBlock FieldName = new TextBlock();
                FieldName.Text = customField.Name;
                FieldName.HorizontalAlignment = HorizontalAlignment.Right;
                FieldName.Padding = new Thickness(0, 5, 0, 5);
                CustomDataLoaderGrid.Children.Add(FieldName);
                Grid.SetColumn(FieldName, 0);
                Grid.SetRow(FieldName, i);

                if (customField.element != null)
                {
                    CustomDataLoaderGrid.Children.Add(customField.element);
                    customField.element.Margin = new Thickness(0, 4, 0, 4);
                    customField.element.GotFocus += handle_reset_data_dependent;
                    Grid.SetColumn(customField.element, 2);
                    Grid.SetRow(customField.element, i);
                }
                else
                {
                    TextBox inputBox = new TextBox();
                    inputBox.Text = customField.Value;
                    inputBox.Tag = customField.Name;
                    inputBox.Margin = new Thickness(0, 4, 0, 4);
                    inputBox.TextChanged += input_box_text_changed;
                    CustomDataLoaderGrid.Children.Add(inputBox);
                    Grid.SetColumn(inputBox, 2);
                    Grid.SetRow(inputBox, i);
                }
                i++;
            });
            
        }
        public void input_box_text_changed(object sender, RoutedEventArgs e)
        {
            TextBox item = sender as TextBox;
            IEnumerator<Field> fields = apiManager.getCurrentDataLoader().GetCustomFields().GetEnumerator();

            fields.MoveNext();
            Field field = fields.Current;
            while (field != null && field.Name != (String)item.Tag)
            {
                fields.MoveNext();
                field = fields.Current;
            }
            if (field == null)
            {
                ConsoleLogger.log("Couldn't find the field " + (String)item.Tag);
                return;
            }

            field.Value = item.Text;
            resetDataDependent();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     Api Action View
        public void handle_update_api_action_grid(object sender, RoutedEventArgs e)
        {
            int i = 0;

            if (apiManager == null ||
                apiManager.getCurrentApiVerb() == null ) return;

            switch (apiManager.getCurrentApiVerb())
            {
                case "POST":
                    apiManager.setApiAction("NEW"); ApiActionNew.IsChecked = true;
                    ApiActionNew.Visibility = Visibility.Visible;
                    ApiActionUpdate.Visibility = Visibility.Collapsed;
                    ApiActionCancel.Visibility = Visibility.Collapsed;
                    ApiActionGetByName.Visibility = Visibility.Collapsed;
                    ApiActionGetByQuery.Visibility = Visibility.Collapsed;
                    break;
                case "PATCH":
                    apiManager.setApiAction("UPDATE"); ApiActionUpdate.IsChecked = true;
                    ApiActionNew.Visibility = Visibility.Collapsed;
                    ApiActionUpdate.Visibility = Visibility.Visible;
                    ApiActionCancel.Visibility = Visibility.Visible;
                    ApiActionGetByName.Visibility = Visibility.Collapsed;
                    ApiActionGetByQuery.Visibility = Visibility.Collapsed;
                    break;
                case "GET":
                    apiManager.setApiAction("GETBYNAME"); ApiActionGetByName.IsChecked = true;
                    ApiActionNew.Visibility = Visibility.Collapsed;
                    ApiActionUpdate.Visibility = Visibility.Collapsed;
                    ApiActionCancel.Visibility = Visibility.Collapsed;
                    ApiActionGetByName.Visibility = Visibility.Visible;
                    ApiActionGetByQuery.Visibility = Visibility.Visible;
                    break;
            }
            handle_update_data_information_grid(null, null);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     Api Action Dependent View
        public void handle_update_data_information_grid(object sender, RoutedEventArgs e)
        {
            if (apiManager == null ||
                apiManager.getCurrentApiAction() == null) return;

            ObjectNameLabel.Text = apiManager.getCurrentApiType() + " Name";

            switch (apiManager.getCurrentApiAction())
            {
                case "GETBYNAME":
                case "CANCEL":
                    DataInformationGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                    DataInformationGrid.RowDefinitions[2].Height = new GridLength(0);
                    DataInformationGrid.RowDefinitions[3].Height = new GridLength(0);
                    DataInformationGrid.RowDefinitions[4].Height = new GridLength(0);
                    break;
                case "GETBYQUERY":
                    DataInformationGrid.RowDefinitions[1].Height = new GridLength(0);
                    DataInformationGrid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
                    DataInformationGrid.RowDefinitions[3].Height = new GridLength(0);
                    DataInformationGrid.RowDefinitions[4].Height = new GridLength(0);
                    break;
                case "UPDATE":
                    DataInformationGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                    DataInformationGrid.RowDefinitions[2].Height = new GridLength(0);
                    DataInformationGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
                    DataInformationGrid.RowDefinitions[4].Height = new GridLength(1, GridUnitType.Star);
                    break;
                case "NEW":
                    DataInformationGrid.RowDefinitions[1].Height = new GridLength(0);
                    DataInformationGrid.RowDefinitions[2].Height = new GridLength(0);
                    DataInformationGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
                    DataInformationGrid.RowDefinitions[4].Height = new GridLength(1, GridUnitType.Star);
                    break;
            }

        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     Buttons and Links
        //Open Url
        private void open_url(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        //View Log
        private void handle_view_log(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            logOutputDialog = new LogOutputDialog();
            logOutputDialog.updateOutput(ConsoleLogger.getContent());
            logOutputDialog.Owner = this;
            logOutputDialog.Show();
        }

        //Debug Console
        private void handle_view_debug_console(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            setDebugConsole(!debug);
        }


        //Edit Profile
        private void handle_edit_profile(object sender, RoutedEventArgs e)
        {
            Profile profile = apiManager.getProfileByName((String) ProfileSelector.SelectedItem);

            EditProfileDialog editProfileDialog = new EditProfileDialog(apiManager, profile);
            editProfileDialog.Owner = this;
            Boolean result = editProfileDialog.ShowDialog() ?? false;
            updateProfileSelectorData();

            int index = apiManager.getProfiles().FindIndex(x => x._id == profile._id);
            if (index < 0) index = ProfileSelector.Items.Count - 1;
            ProfileSelector.SelectedIndex = index;
            resetCredentialsDependent();
        }
        //New Profile
        private void handle_new_profile(object sender, RoutedEventArgs e)
        {
            int itemCount0 = ProfileSelector.Items.Count;

            Profile profile = new Profile();

            EditProfileDialog editProfileDialog = new EditProfileDialog(apiManager, profile);
            editProfileDialog.Owner = this;
            editProfileDialog.ShowDialog();

            updateProfileSelectorData();

            int itemCount = ProfileSelector.Items.Count;
            if(itemCount != itemCount0) ProfileSelector.SelectedIndex = itemCount - 1;
        }
        //Connect to Salesforce
        private async void handle_connect(object sender, RoutedEventArgs e)
        {
            setCredentialsStatus(STATUS.TRYING);
            Boolean success = await apiManager.testCredentials(getPassword());
            setCredentialsStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
        }

        //reset credentials dependent buttons and text
        private void handle_reset_credentials_dependent(object sender, RoutedEventArgs e)
        {
            resetCredentialsDependent();
        }
        //reset data dependent buttons and text
        private void handle_reset_data_dependent(object sender, RoutedEventArgs e)
        {
            resetDataDependent();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///    API BUTTON METHODS
        private void handle_use_validation_click(object sender, RoutedEventArgs e)
        {
            apiManager.setUseValidation((sender as CheckBox).IsChecked ?? true);
        }

        //Load Data
        private async void handle_load_data_debug(object sender, RoutedEventArgs e)
        {
            setLoadDataStatus(STATUS.TRYING);
            Boolean success = await apiManager.testLoadData();
            setLoadDataStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
        }
        //Convert Data
        private void handle_convert_data_debug(object sender, RoutedEventArgs e)
        {
            setConvertDataStatus(STATUS.TRYING);
            Boolean success = apiManager.testConvertData();
            setConvertDataStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
        }
        //Send Data
        private async void handle_send_data_debug(object sender, RoutedEventArgs e)
        {
            setCallApiStatus(STATUS.TRYING);
            Boolean success = apiManager.testApiSetup();
            if (success) success = await apiManager.testApiCall();
            setCallApiStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
        }

        private async void handle_send_data(object sender, RoutedEventArgs e)
        {
            setAllStatuses(STATUS.WAITING);
            setSendDataStatus(STATUS.TRYING);

            setCredentialsStatus(STATUS.TRYING);
            Boolean success = await apiManager.testCredentials(getPassword());
            setCredentialsStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);

            //Cancel and Get don't load any data
            if (apiManager.getCurrentApiAction() != "CANCEL" &&
                apiManager.getCurrentApiVerb() != "GET")
            {
                if (success)
                {
                    setLoadDataStatus(STATUS.TRYING);
                    success = await apiManager.testLoadData();
                    setLoadDataStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
                }

                if (success)
                {
                    setConvertDataStatus(STATUS.TRYING);
                    success = apiManager.testConvertData();
                    setConvertDataStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
                
                }
            }

            if (success)
            {
                setCallApiStatus(STATUS.TRYING);
                success = apiManager.testApiSetup();
                if(success) success = await apiManager.testApiCall();
                setCallApiStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
            }

            setSendDataStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
        }

        //Send Data
        private void handle_view_response(object sender, RoutedEventArgs e)
        {
            asnResponseViewer = new AsnResponseViewer(apiManager.getResponse());
            asnResponseViewer.Owner = this;
            asnResponseViewer.Show();
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     Drop Downs

        //Profile Selector
        private void handle_profile_selector_loaded(object sender, RoutedEventArgs e)
        {
            updateProfileSelectorData();

            ComboBox profileSelector = sender as ComboBox;
            profileSelector.SelectedIndex = 0;
        }
        private void handle_profile_selector_changed(object sender, RoutedEventArgs e)
        {
            ComboBox ProfileSelctor = sender as ComboBox;

            apiManager.setCurrentProfile((String)ProfileSelctor.SelectedValue);
            resetCredentialsDependent();
        }

        //Data Loader Selector
        private void handle_data_loader_selector_loaded(object sender, RoutedEventArgs e)
        {
            updateDataLoaderSelectorData();

            ComboBox comboBox = sender as ComboBox;
            comboBox.SelectedIndex = 0;
        }
        private void handle_data_loader_selector_changed(object sender, RoutedEventArgs e)
        {
            ComboBox DataLoaderSelector = sender as ComboBox;

            apiManager.setCurrentDataLoader((String)DataLoaderSelector.SelectedValue);
            handle_populate_data_loader_grid(null, null);
            resetDataDependent();
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     Handle Radio Button Changes
        private void handle_api_type_changed(object sender, RoutedEventArgs e)
        {
            RadioButton apiType = sender as RadioButton;
            apiManager.setApiType((String) apiType.Tag);
            updateDataLoaderSelectorData();
            DataLoaderSelector.SelectedIndex = 0;
            handle_update_data_information_grid(null, null);

            resetDataDependent();
        }
        private void handle_api_verb_changed(object sender, RoutedEventArgs e)
        {
            RadioButton apiVerb = sender as RadioButton;
            apiManager.setApiVerb((String)apiVerb.Tag);
            handle_update_api_action_grid(null, null);

            resetDataDependent();
        }
        private void handle_api_action_changed(object sender, RoutedEventArgs e)
        {
            RadioButton apiAction = sender as RadioButton;
            apiManager.setApiAction((String)apiAction.Tag);
            handle_update_data_information_grid(null, null);

            resetDataDependent();
        }
        private void handle_object_name_changed(object sender, RoutedEventArgs e)
        {
            apiManager.setObjectName((sender as TextBox).Text);
            resetDataDependent();
        }
        private void handle_query_string_changed(object sender, RoutedEventArgs e)
        {
            apiManager.setQueryString((sender as TextBox).Text);
            resetDataDependent();
        }





        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     Non Handler Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void updateLogBox(String message)
        {
            if (logOutputDialog == null) return;
            logOutputDialog.updateOutput(message);

            try
            {
                LogOutput.Text = message;
                LogScroller.ScrollToEnd();
            }
            catch
            {
            }
        }

        public String getPassword()
        {
            return PasswordBox.Password;
        }

        private void updateProfileSelectorData()
        {
            List<string> data = new List<string>();

            apiManager.getProfiles().ForEach(x =>
                {
                    data.Add(x.ProfileName);
                }
            );

            ProfileSelector.ItemsSource = data;
        }

        private void updateDataLoaderSelectorData()
        {
            List<string> data = new List<string>();

            apiManager.getApiDataLoaders().ForEach(loader =>
            {
                data.Add(loader.GetUniqueName());
            }
            );

            DataLoaderSelector.ItemsSource = data;
        }

        public enum STATUS{
            WAITING, TRYING, SUCCESS, FAILURE
        }
        private void setTextStatus(TextBlock textblock, STATUS status)
        {
            Brush color = Brushes.White;
            switch (status)
            {
                case STATUS.WAITING:
                    color = Brushes.Black;
                    break;
                case STATUS.TRYING:
                    color = Brushes.PaleGoldenrod;
                    break;
                case STATUS.SUCCESS:
                    color = Brushes.Green;
                    break;
                case STATUS.FAILURE:
                    color = Brushes.Crimson;
                    break;
            }
            textblock.Foreground = color;
        }
        private void setButtonStatus(Button button, STATUS status)
        {
            Brush color = Brushes.White;
            switch (status)
            {
                case STATUS.WAITING:
                    color = Brushes.Aquamarine;
                    break;
                case STATUS.TRYING:
                    color = Brushes.PaleGoldenrod;
                    break;
                case STATUS.SUCCESS:
                    color = Brushes.LimeGreen;
                    break;
                case STATUS.FAILURE:
                    color = Brushes.Crimson;
                    break;
            }
            button.Background = color;
        }

        public void resetCredentialsDependent()
        {
            setCredentialsStatus(STATUS.WAITING);
            setSendDataStatus(STATUS.WAITING);
        }
        public void resetDataDependent()
        {
            setLoadDataStatus(STATUS.WAITING);
            setConvertDataStatus(STATUS.WAITING);
            setCallApiStatus(STATUS.WAITING);
        }

        public void setCredentialsStatus(STATUS status)
        {
            setButtonStatus(ConnectToSalesforceButton, status);
            setTextStatus(ProgressCredentials, status);
        }
        public void setLoadDataStatus(STATUS status)
        {
            setButtonStatus(LoadDataButton, status);
            setTextStatus(ProgressLoadData, status);
        }
        public void setConvertDataStatus(STATUS status)
        {
            setButtonStatus(ConvertDataButton, status);
            setTextStatus(ProgressConvertData, status);
        }
        public void setCallApiStatus(STATUS status)
        {
            setButtonStatus(CallApiButton, status);
            setTextStatus(ProgressCallApi, status);
            setSendDataStatus(status);
        }
        public void setSendDataStatus(STATUS status)
        {
            setButtonStatus(SendData, status);
            if (status == STATUS.SUCCESS)
            {
                ViewResponseDebug.Visibility = Visibility.Visible;
                ViewResponse.Visibility = Visibility.Visible;
            }
            else
            {
                ViewResponseDebug.Visibility = Visibility.Collapsed;
                ViewResponse.Visibility = Visibility.Collapsed;
            }

        }
        public void setAllStatuses(STATUS status)
        {
            setCredentialsStatus(status);
            setLoadDataStatus(status);
            setConvertDataStatus(status);
            setCallApiStatus(status);
            setSendDataStatus(status);
        }
    }
}
