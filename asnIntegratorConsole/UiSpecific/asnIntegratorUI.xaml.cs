﻿using asnIntegratorConsole.UiSpecific;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace asnIntegratorConsole
{
    /// <summary>
    /// Interaction logic for asnIntegratorConsole.xaml
    /// </summary>
    public partial class asnIntegratorUI : Window
    {
        APImanager apiManager;

        public asnIntegratorUI()
        {
            InitializeComponent();
            ConsoleLogger.initialize(this);
            apiManager = new APImanager();
        }


        //Appends a new message to the log box content
        public void updateLogBox(String logContent)
        {
            TextBlock LogOutput = this.FindName("logOutput") as TextBlock;
            LogOutput.Text = logContent;
            ScrollViewer LogScroller = this.FindName("LogScroller") as ScrollViewer;
            LogScroller.ScrollToEnd();
        }
        //Returns all login related buttons to the base color
        public void resetLoginDependentButtons()
        {
            Color color = Color.FromRgb(81, 207, 239);

            Button TestCredentialsButton = this.FindName("TestCredentialsButton") as Button;
            TestCredentialsButton.Background = new SolidColorBrush(color);

            Button TestApiSetup = this.FindName("TestAPISetupButton") as Button;
            TestApiSetup.Background = new SolidColorBrush(color);
        }
        //Returns all data related buttons to the base color
        public void resetDataDependentButtons()
        {
            Color color = Color.FromRgb(81, 207, 239);

            Button TestDataLoadButton = this.FindName("TestDataRetrievalButton") as Button;
            TestDataLoadButton.Background = new SolidColorBrush(color);

            Button TestDataConvertButton = this.FindName("TestDataReformatButton") as Button;
            TestDataConvertButton.Background = new SolidColorBrush(color);
        }

        //Updates the credentials in the apiManager so the SalesForceConnection can use them
        private void updateCredentials()
        {
            //pull the credentials data from the UI form
            String tUserName = UserName.Text;
            String tPassword = Password.Password;
            String tSecurityToken = SecurityToken.Text;
            String tConsumerKey = ConsumerKey.Text;
            String tConsumerSecret = ConsumerSecret.Text;

            apiManager.setCredentials( new Credentials(tSecurityToken, tConsumerKey, tConsumerSecret, tUserName, tPassword, true) );
        }


        ////////////////////////////////////////////////////////////
        //// Button Actions
        ////////////////////////////////////////////////////////////

        //Action to take when the Test Credentials button is clicked
        private async void test_credentails_click(object sender, RoutedEventArgs e)
        {
            updateCredentials();

            Button TestCredentialsButton = this.FindName("TestCredentialsButton") as Button;
            Color color = Color.FromRgb(255, 255, 192);
            TestCredentialsButton.Background = new SolidColorBrush(color);

            // Test the Credentials
            Boolean success = await apiManager.testCredentials();

            color = success ? Color.FromRgb(0, 255, 0) : Color.FromRgb(255, 0, 0);
            TestCredentialsButton.Background = new SolidColorBrush(color);
            Console.Write("Test Credentials: "+success);
        }


        //Action to take when the Test Data Retreival button is clicked
        private async void test_retrieval_click(object sender, RoutedEventArgs e)
        {
            Button TestLoadData = this.FindName("TestDataRetrievalButton") as Button;
            Color color = Color.FromRgb(255, 255, 192);
            TestLoadData.Background = new SolidColorBrush(color);

            bool success = await apiManager.testLoadData();

            color = success ? Color.FromRgb(0, 255, 0) : Color.FromRgb(255, 0, 0);
            TestLoadData.Background = new SolidColorBrush(color);
            Console.Write("Test Load Data: " + success);
        }

        //Action to take when the Test Data Reformat button is clicked
        private void test_reformat_click(object sender, RoutedEventArgs e)
        {
            Button TestConvertData = this.FindName("TestDataReformatButton") as Button;
            Color color = Color.FromRgb(255, 255, 192);
            TestConvertData.Background = new SolidColorBrush(color);

            bool success = apiManager.testConvertData();

            color = success ? Color.FromRgb(0, 255, 0) : Color.FromRgb(255, 0, 0);
            TestConvertData.Background = new SolidColorBrush(color);
            Console.Write("Test Data Reformat: " + success);
        }

        //Action to take when the Test API setup button is clicked
        private async void test_api_setup_click(object sender, RoutedEventArgs e)
        {
            Button TestApiSetup = this.FindName("TestAPISetupButton") as Button;
            Color color = Color.FromRgb(255, 255, 192);
            TestApiSetup.Background = new SolidColorBrush(color);

            bool success = apiManager.testApiSetup();
            if (success) success = await apiManager.testApiCall();

            color = success ? Color.FromRgb(0, 255, 0) : Color.FromRgb(255, 0, 0);
            TestApiSetup.Background = new SolidColorBrush(color);
            Console.Write("Test API Setup: " + success);
        }

        //Action to take when the Test All button is clicked
        private async void test_all_click(object sender, RoutedEventArgs e)
        {
            updateCredentials();

            Color color = Color.FromRgb(192, 192, 192);
            ((Button)this.FindName("TestCredentialsButton")).Background = new SolidColorBrush(color);
            ((Button)this.FindName("TestDataRetrievalButton")).Background = new SolidColorBrush(color);
            ((Button)this.FindName("TestDataReformatButton")).Background = new SolidColorBrush(color);
            ((Button)this.FindName("TestAPISetupButton")).Background = new SolidColorBrush(color);

            Button TestAll = this.FindName("TestAllButton") as Button;
            color = Color.FromRgb(255, 255, 192);
            TestAll.Background = new SolidColorBrush(color);

            updateCredentials();
            bool success = await apiManager.testAll();

            color = success ? Color.FromRgb(0, 255, 0) : Color.FromRgb(255, 0, 0);
            TestAll.Background = new SolidColorBrush(color);
            Console.Write("Test All: " + success);

        }


        //Action to take when Save Profile is clicked
        private void save_profile_click(object sender, RoutedEventArgs e)
        {
            updateCredentials();

            TextBox ProfileName = this.FindName("ProfileName") as TextBox;
            TextBox UserName = this.FindName("UserName") as TextBox;
            TextBox ConsumerKey = this.FindName("ConsumerKey") as TextBox;
            TextBox ConsumerSecret = this.FindName("ConsumerSecret") as TextBox;
            TextBox SecurityToken = this.FindName("SecurityToken") as TextBox;
            PasswordBox UserPassword = this.FindName("Password") as PasswordBox;
            CheckBox IsSandbox = this.FindName("IsSandbox") as CheckBox;

            Profile profile = new Profile(ProfileName.Text,
                                                UserName.Text,
                                                ConsumerKey.Text,
                                                ConsumerSecret.Text,
                                                SecurityToken.Text,
                                                IsSandbox.IsChecked ?? true);
            apiManager.saveProfile( profile );

            ComboBox dd = this.FindName("LoadProfileDD") as ComboBox;
            on_init_load_profile(dd, null);
            dd.SelectedIndex = apiManager.getProfiles().FindIndex( v => v.ProfileName == profile.ProfileName);
            UpdateLayout();
        }

        //Action to take when Delete Profile is clicked
        private void delete_profile_click(object sender, RoutedEventArgs e)
        {
            updateCredentials();
            TextBox ProfileName = this.FindName("ProfileName") as TextBox;
            apiManager.deleteProfile(ProfileName.Text);

            ComboBox dd = this.FindName("LoadProfileDD") as ComboBox;
            on_init_load_profile(dd, null);
            UpdateLayout();
        }

        //Action to take whe Set Json Filepath is clicked
        private void set_profiles_file_click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                ConsoleLogger.log("New profiles.json path" + filename);
                apiManager.setProfilesPath(filename);
                TextBlock profilesPath = this.FindName("ProfilesLocationText") as TextBlock;
                profilesPath.Text = filename;

                ComboBox dd = this.FindName("LoadProfileDD") as ComboBox;
                on_init_load_profile(dd, null);
                UpdateLayout();
            }

        }

        //Action to take when the LoadProfile dropdown is loaded initially
        private void on_init_load_profile(object sender, RoutedEventArgs e)
        {
            // ... A List.
            List<string> data = new List<string>();

            apiManager.getProfiles().ForEach(x =>
                {
                    data.Add(x.ProfileName);
                }
            );

            TextBlock profilesPath = this.FindName("ProfilesLocationText") as TextBlock;
            profilesPath.Text = apiManager.getProfilesPath();

            ComboBox comboBox = sender as ComboBox;
            comboBox.ItemsSource = data;

            comboBox.SelectedIndex = 0;
        }
        //Action to take when a new Profile is selected
        private void on_new_profile_selected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            string profileName = comboBox.SelectedItem as string;
            Profile profile = apiManager.getProfile(profileName);

            TextBox ProfileNameBox = this.FindName("ProfileName") as TextBox;
            TextBox UserNameBox = this.FindName("UserName") as TextBox;
            TextBox ConsumerKeyBox = this.FindName("ConsumerKey") as TextBox;
            TextBox ConsumerSecretBox = this.FindName("ConsumerSecret") as TextBox;
            TextBox SecurityTokenBox = this.FindName("SecurityToken") as TextBox;
            PasswordBox UserPasswordBox = this.FindName("Password") as PasswordBox;
            CheckBox IsSandbox = this.FindName("IsSandbox") as CheckBox;
            if (profile != null)
            {
                ProfileNameBox.Text = profile.ProfileName;
                UserNameBox.Text = profile.Username;
                ConsumerKeyBox.Text = profile.ConsumerKey;
                ConsumerSecretBox.Text = profile.ConsumerSecret;
                SecurityTokenBox.Text = profile.SecurityToken;
                IsSandbox.IsChecked = profile.IsSandboxUser;
            } else
            {
                ProfileNameBox.Text = "";
                UserNameBox.Text = "";
                ConsumerKeyBox.Text = "";
                ConsumerSecretBox.Text = "";
                SecurityTokenBox.Text = "";
                IsSandbox.IsChecked = true;
            }
            UserPasswordBox.Password = "";

            resetLoginDependentButtons();
        }


        //Action to take when the LoadProfile dropdown is loaded initially
        private void on_init_load_data_loader(object sender, RoutedEventArgs e)
        {
            // ... A List.
            List<string> data = new List<string>();

            apiManager.getDataLoaders().ForEach(x =>
                {
                    data.Add(x.GetType().ToString());
                }
            );


            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = data;
            comboBox.SelectedIndex = 0;
        }
        //Action to take when a new Data Loader is selected
        private void on_new_data_loader_selected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox DataLoaderDD = sender as ComboBox;
            apiManager.setDataLoader((String) DataLoaderDD.SelectedValue);
            if (apiManager.getDataLoader() == null) return;
            List<Field> data = apiManager.getDataLoader().GetCustomFields();
            
            ListBox listBox = this.FindName("DataLoaderFields") as ListBox;
            listBox.ItemsSource = data;

            resetDataDependentButtons();
        }
        private void handle_custom_field_change(object sender, TextChangedEventArgs e)
        {
            TextBox item = sender as TextBox;

            IEnumerator<Field> fields = apiManager.getDataLoader().GetCustomFields().GetEnumerator();

            fields.MoveNext();
            Field field = fields.Current;
            while (field != null && field.Name != (String)item.Tag)
            {
                fields.MoveNext();
                field = fields.Current;
            }
            if(field == null)
            {
                ConsoleLogger.log("Couldn't find the field " + (String) item.Tag);
                return;
            }

            resetDataDependentButtons();

            field.Value = item.Text;
        }

        //Reset the login related buttons whenever a login field is changed
        //Salesforce connection is no longer valid with the new credentials
        private void login_related_field_changed(object sender, RoutedEventArgs e)
        {
            resetLoginDependentButtons();
        }

    }
}
