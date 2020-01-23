using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Settings;
using Facebook;

namespace BodyArchitect.Client.UI.Social
{
    /// <summary>
    /// Interaction logic for FacebookLoginWindow.xaml
    /// </summary>
    public partial class FacebookLoginWindow : Window
    {
        //luupxtg_zuckerberg_1335118220@tfbnw.net
        //kwazar
        public FacebookLoginWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FacebookLoginWindow_Loaded);
        }

        void FacebookLoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var loginUrl = GenerateLoginUrl(SocialNetworkShare.FacebookApiKey, "publish_stream,manage_pages");
            webBrowser.Navigate(loginUrl);
        }

        private Uri GenerateLoginUrl(string appId, string extendedPermissions)
        {
            dynamic parameters = new ExpandoObject();
            parameters.client_id = appId;
            parameters.redirect_uri = "https://www.facebook.com/connect/login_success.html";

            // The requested response: an access token (token), an authorization code (code), or both (code token).
            parameters.response_type = "token";

            // list of additional display modes can be found at http://developers.facebook.com/docs/reference/dialogs/#display
            parameters.display = "popup";

            // add the 'scope' parameter only if we have extendedPermissions.
            if (!string.IsNullOrWhiteSpace(extendedPermissions))
                parameters.scope = extendedPermissions;

            
            // generate the login url
            var fb = new Facebook.FacebookClient();
            return fb.GetLoginUrl(parameters);
        }

        private void webBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // whenever the browser navigates to a new url, try parsing the url.
            // the url may be the result of OAuth 2.0 authentication.
            var fb = new Facebook.FacebookClient();
            FacebookOAuthResult oauthResult;
            if (fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                // The url is the result of OAuth 2.0 authentication
                if (oauthResult.IsSuccess)
                {
                    Settings1.Default.FacebookToken = oauthResult.AccessToken;
                    fb.AccessToken = Settings1.Default.FacebookToken;

                    //now retrieve all connected accounts and if there are more than one then display combobox allows user to choose which account he want to use
                    JsonObject fbAccounts = (JsonObject)fb.Get("/me/accounts");

                    ListItem<string> item = new ListItem<string>(Strings.FacebookLoginWindow_DefaultAccount, oauthResult.AccessToken);
                    cmbFacebookAccounts.Items.Add(item);

                    foreach (dynamic account in (IEnumerable)fbAccounts["data"])
                    {
                        item = new ListItem<string>(account["name"], account["access_token"]);
                        cmbFacebookAccounts.Items.Add(item);
                    }
                    if (cmbFacebookAccounts.Items.Count == 1)
                    {
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        cmbFacebookAccounts.SelectedIndex = 0;
                        webBrowser.Visibility=Visibility.Collapsed;
                        pnlChooseAccount.Visibility = Visibility.Visible;
                    }
                    
                }
                else
                {
                    //var errorDescription = oauthResult.ErrorDescription;
                    //var errorReason = oauthResult.ErrorReason;
                    DialogResult = false;
                    Close();
                }
            }
            else
            {
                // The url is NOT the result of OAuth 2.0 authentication.
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Settings1.Default.FacebookToken = ((ListItem<string>) cmbFacebookAccounts.SelectedItem).Value;
            DialogResult = true;
            Close();
        }
    }
}
