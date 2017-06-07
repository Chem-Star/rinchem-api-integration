using asnIntegratorConsole.UiSpecific;
using RinchemApiIntegrationConsole.UiSpecific;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RinchemApiIntegrationConsole
{
    /// <summary>
    /// Interaction logic for asnIntegratorConsole.xaml
    /// </summary>
    public partial class DebugConsole : Window
    {
        APImanager apiManager;
        LogOutputDialog logOutputDialog;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     Consturctor and Lifecycle
        public DebugConsole()
        {
            InitializeComponent();
            ConsoleLogger.initialize(this);
            apiManager = new APImanager();
            logOutputDialog = new LogOutputDialog();
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     UI EVENT HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
            // Instantiate window
            DebugConsole debugConsole = new DebugConsole();
            debugConsole.Owner = this;
            debugConsole.ShowDialog();
        }


        //Edit Profile
        private void handle_edit_profile(object sender, RoutedEventArgs e)
        {
            Profile profile = apiManager.getProfileByName((String)ProfileSelector.SelectedItem);

            // Instantiate window
            EditProfileDialog editProfileDialog = new EditProfileDialog(apiManager, profile);
            editProfileDialog.Owner = this;
            editProfileDialog.ShowDialog();

            updateProfileSelectorData();
            int index = apiManager.getProfiles().FindIndex(x => x._id == profile._id);

            if (index < 0) ProfileSelector.SelectedIndex = ProfileSelector.Items.Count - 1;
        }
        //New Profile
        private void handle_new_profile(object sender, RoutedEventArgs e)
        {
            int itemCount0 = ProfileSelector.Items.Count;

            Profile profile = new Profile();

            // Instantiate window
            EditProfileDialog editProfileDialog = new EditProfileDialog(apiManager, profile);
            editProfileDialog.Owner = this;
            editProfileDialog.ShowDialog();

            updateProfileSelectorData();

            int itemCount = ProfileSelector.Items.Count;
            if (itemCount != itemCount0) ProfileSelector.SelectedIndex = itemCount - 1;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///    API BUTTON METHODS
        //Connect to Salesforce
        private async void handle_connect(object sender, RoutedEventArgs e)
        {
            setCredentialsSatus(STATUS.TRYING);
            Boolean success = await apiManager.testCredentials(getPassword());
            setCredentialsSatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
        }
        //Load Data
        private async void handle_load_data(object sender, RoutedEventArgs e)
        {
            setLoadDataStatus(STATUS.TRYING);
            Boolean success = await apiManager.testLoadData();
            setLoadDataStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
        }
        //Convert Data
        private void handle_convert_data(object sender, RoutedEventArgs e)
        {
            setConvertDataStatus(STATUS.TRYING);
            Boolean success = apiManager.testConvertData();
            setConvertDataStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
        }
        //Send DataL
        private async void handle_send_data(object sender, RoutedEventArgs e)
        {
            setCallApiStatus(STATUS.TRYING);
            Boolean success = apiManager.testApiSetup();
            if (success) success = await apiManager.testApiCall();
            setCallApiStatus(success ? STATUS.SUCCESS : STATUS.FAILURE);
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
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     Handle Radio Button Changes
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void handle_api_type_changed(object sender, RoutedEventArgs e)
        {
            RadioButton apiType = sender as RadioButton;
            apiManager.setApiType((String)apiType.Tag);
            updateDataLoaderSelectorData();
            DataLoaderSelector.SelectedIndex = 0;
        }
        private void handle_api_action_changed(object sender, RoutedEventArgs e)
        {
            RadioButton apiAction = sender as RadioButton;
            apiManager.setApiVerb((String)apiAction.Tag);
        }






        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     Non Handler Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void updateLogBox(String logContent)
        {
            try
            {
                LogOutput.Text = logContent;
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

        public enum STATUS
        {
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

        public void setCredentialsSatus(STATUS status)
        {
            setButtonStatus(ConnectToSalesforceButton, status);
            setTextStatus(ProgressCredentials, status);

            UpdateLayout();
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
            setButtonStatus(SendDataButton, status);
            setTextStatus(ProgressCallApi, status);
        }
        public void setAllStatuses(STATUS status)
        {
            setCredentialsSatus(status);
            setLoadDataStatus(status);
            setConvertDataStatus(status);
            setCallApiStatus(status);
        }
    }
}
