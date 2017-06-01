using RinchemApiIntegrationConsole;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace asnIntegratorConsole.UiSpecific
{
    /// <summary>
    /// Interaction logic for EditProfileDialog.xaml
    /// </summary>
    partial class EditProfileDialog : Window
    {
        APImanager apiManager;
        Profile profile;
        public EditProfileDialog(APImanager apiManager, Profile profile)
        {
            InitializeComponent();
            this.apiManager = apiManager;
            this.profile = profile;
        }


        private void handle_profile_name_loaded(object sender, RoutedEventArgs e)
        {
            TextBox ProfileName = sender as TextBox;
            ProfileName.Text = profile.ProfileName;
        }
        private void handle_username_loaded(object sender, RoutedEventArgs e)
        {
            TextBox UserName = sender as TextBox;
            UserName.Text = profile.Username;
        }
        private void handle_security_token_loaded(object sender, RoutedEventArgs e)
        {
            TextBox SecurityToken = sender as TextBox;
            SecurityToken.Text = profile.SecurityToken;
        }
        private void handle_consumer_key_loaded(object sender, RoutedEventArgs e)
        {
            TextBox ConsumerKey = sender as TextBox;
            ConsumerKey.Text = profile.ConsumerKey;
        }
        private void handle_consumer_secret_loaded(object sender, RoutedEventArgs e)
        {
            TextBox ConsumerSecret = sender as TextBox;
            ConsumerSecret.Text = profile.ConsumerSecret;
        }
        private void handle_is_sandbox_user_loaded(object sender, RoutedEventArgs e)
        {
            CheckBox IsSandboxUser = sender as CheckBox;
            IsSandboxUser.IsChecked = profile.IsSandboxUser;
        }






        private void handle_delete_profile_click(object sender, RoutedEventArgs e)
        {
            TextBox ProfileName = this.FindName("ProfileName") as TextBox;
            Profile profile = apiManager.getProfileByName(ProfileName.Text);
            apiManager.deleteProfile(profile._id);
        }

        private void handle_save_profile_click(object sender, RoutedEventArgs e)
        {
            TextBox ProfileName = this.FindName("ProfileName") as TextBox;
            TextBox UserName = this.FindName("UserName") as TextBox;
            TextBox ConsumerKey = this.FindName("ConsumerKey") as TextBox;
            TextBox ConsumerSecret = this.FindName("ConsumerSecret") as TextBox;
            TextBox SecurityToken = this.FindName("SecurityToken") as TextBox;
            PasswordBox UserPassword = this.FindName("Password") as PasswordBox;
            CheckBox IsSandbox = this.FindName("IsSandbox") as CheckBox;

            List<LoaderInfo> loadersInfo = new List<LoaderInfo>();

            profile.updateFields(ProfileName.Text,
                                    UserName.Text,
                                    ConsumerKey.Text,
                                    ConsumerSecret.Text,
                                    SecurityToken.Text,
                                    IsSandbox.IsChecked ?? true);
            apiManager.saveProfile(profile);
        }
    }
}
