using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using Facebook;
using BodyArchitect.Client.Resources.Localization;

namespace BodyArchitect.Client.UI.Social
{
    //rokoko121@wp.pl
    public class SocialNetworkShare
    {
        public const string FacebookApiKey="423455364350858";
        private const string FacebookSecret = "b712139697a576b8f082455ac9d86b32";
        /// <summary>
        /// Renews the token.. (offline deprecation)
        /// </summary>
        /// <param name="existingToken">The token to renew</param>
        /// <returns>A new token (or the same as existing)</returns>
        public static string RenewToken(string existingToken)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Get("oauth/access_token",
                                    new
                                    {
                                        client_id = FacebookApiKey,
                                        client_secret = FacebookSecret,
                                        grant_type = "fb_exchange_token",
                                        fb_exchange_token = existingToken
                                    });

            return result.access_token;
        }

        public void PostOnWall(ShareSocialContent content)
        {
            bool tryAgain=true;
            while (tryAgain)
            {
                try
                {
                    var fbApp = new FacebookClient(Settings1.Default.FacebookToken);
                    var args = new Dictionary<string, object>();
                    args["name"] = content.Name;
                    args["link"] = string.IsNullOrEmpty(content.Url) ? "http://bodyarchitectonline.com" : content.Url;
                    args["caption"] = content.Caption;
                    args["description"] = content.Description;
                    //args["picture"] = "http://www.bodyarchitectonline.com/images/stories/blue-09_thumb.png";
                    if (!string.IsNullOrEmpty(content.Message))
                    {
                        args["message"] = content.Message;
                    }
                    args["actions"] = "";
                    fbApp.Post("/571048516255177/feed", args);
                    tryAgain = false;
                }
                catch (FacebookOAuthException ex)
                {
                    tryAgain = false;
                    try
                    {
                        Settings1.Default.FacebookToken = RenewToken(Settings1.Default.FacebookToken);
                        tryAgain = true;
                    }
                    catch (Exception)
                    {
                        ExceptionHandler.Default.Process(ex);
                        FacebookLoginWindow dlg = new FacebookLoginWindow();
                        if (dlg.ShowDialog() == true)
                        {
                            tryAgain = true;
                        }
                    }
                    
                    
                }
                catch(Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, Strings.Exception_SocialNetworkShare_CannotPublishToFacebook, ErrorWindow.EMailReport);
                }
            }
            
            
        }

        public void PostNote(ShareSocialContent content)
        {
            bool tryAgain = true;
            while (tryAgain)
            {
                try
                {
                    var fbApp = new FacebookClient(Settings1.Default.FacebookToken);

                    var args = new Dictionary<string, object>();
                    //args["name"] = content.Name;
                    //args["link"] = string.IsNullOrEmpty(content.Url) ? "http://bodyarchitectonline.com" : content.Url;
                    args["subject"] = content.Name;
                    args["message"] = content.Description;
                    args["icon"] = "http://www.bodyarchitectonline.com/images/bodyarchitect1.jpg";
                    if (!string.IsNullOrEmpty(content.Message))
                    {
                        args["message"] = content.Message;
                    }

                    args["message"] += "<br/><br/><a href='http://bodyarchitectonline.com'><img  src='http://bodyarchitectonline.com/images/bodyarchitect1.jpg'></img></a><br/><a href='http://bodyarchitectonline.com'>BodyArchitect</a> - " + Strings.SocialNetworkShare_Footer;

                    
                    //args["actions"] = "";
                    fbApp.Post("/me/notes", args);
                    tryAgain = false;
                }
                catch (FacebookOAuthException ex)
                {
                    tryAgain = false;
                    try
                    {
                        Settings1.Default.FacebookToken = RenewToken(Settings1.Default.FacebookToken);
                        tryAgain = true;
                    }
                    catch (Exception)
                    {
                        ExceptionHandler.Default.Process(ex);
                        FacebookLoginWindow dlg = new FacebookLoginWindow();
                        if (dlg.ShowDialog() == true)
                        {
                            tryAgain = true;
                        }
                    }


                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, Strings.Exception_SocialNetworkShare_CannotPublishToFacebook, ErrorWindow.EMailReport);
                }
            }


        }

        public bool PostEntryObject(EntryObjectDTO entry)
        {
            var provider=PluginsManager.Instance.GetEntryObjectProvider(entry.GetType());
            if(provider!=null)
            {
                var content=provider.ShareToSocial(entry);
                if(content!=null)
                {
                    PostNote(content);
                    return true;
                }
            }
            return false;
        }
    }
}
