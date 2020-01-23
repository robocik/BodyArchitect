using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using HtmlAgilityPack;

namespace BodyArchitect.Client.UI.Translator
{
    public class HtmlTranslator
    {
        private HtmlAgilityPack.HtmlDocument html;
        private string sourceLanguage;
        private List<HtmlNode> coll;
        private string token;

        public HtmlTranslator(string sourceText,string sourceLanguage)
        {
            this.sourceLanguage = sourceLanguage;
            html = new HtmlAgilityPack.HtmlDocument();
            MemoryStream stream = new MemoryStream();
            StreamWriter reader = new StreamWriter(stream);
            reader.Write(sourceText);
            reader.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            html.Load(stream, true);
            coll = html.DocumentNode.SelectNodes("//text()[normalize-space(.) != '']").ToList();
            coll =coll.Where(x =>!x.ParentNode.Attributes.Contains("class") || x.ParentNode.Attributes["class"].Value != "doNotTranslate").ToList();
        }

        
        public int MaxRequests
        {
            get { return coll.Count; }
        }

        public string[] GetSupportedLanguages()
        {
            ensureAuthenticated();
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/GetLanguagesForTranslate";
            System.Net.WebRequest translationWebRequest = System.Net.WebRequest.Create(uri);
            translationWebRequest.Headers.Add("Authorization", token);
            System.Net.WebResponse response = null;
            response = translationWebRequest.GetResponse();
            System.IO.Stream stream = response.GetResponseStream();
            System.Text.Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            System.IO.StreamReader translatedStream = new System.IO.StreamReader(stream, encode);
            System.Xml.XmlDocument xTranslation = new System.Xml.XmlDocument();
            xTranslation.LoadXml(translatedStream.ReadToEnd());
            List<string> languages = new List<string>();
            foreach (XmlElement lang in xTranslation.DocumentElement.ChildNodes)
            {
                languages.Add(lang.InnerText);
            }
            return languages.ToArray();
        }

        void ensureAuthenticated()
        {
            if(token==null)
            {
                token = authorize();
            }
        }
        string authorize()
        {
            string clientID = "BodyArchitect";
            string clientSecret = "A+/n01Me7SlrW2V176LWJblB+KwDpHXoc3r1lr0qbzE=";

            String strTranslatorAccessURI = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
            String strRequestDetails = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", Uri.EscapeDataString(clientID), Uri.EscapeDataString(clientSecret));

            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(strTranslatorAccessURI);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(strRequestDetails);
            webRequest.ContentLength = bytes.Length;
            using (System.IO.Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            System.Net.WebResponse webResponse = webRequest.GetResponse();

            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(AdmAccessToken));
            //Get deserialized object from JSON stream 
            AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());

            return "Bearer " + token.access_token;
        }

        public string Translate(string targetLanguage,MethodParameters param)
        {
            ensureAuthenticated();

            if (!processDocument(token, param, sourceLanguage, targetLanguage))
            {
                return null;
            }

            MemoryStream t = new MemoryStream();
            html.Save(t);
            t.Seek(0, SeekOrigin.Begin);
            var reader1 = new StreamReader(t);
            string translatedHtml = reader1.ReadToEnd();
            return translatedHtml;
        }

        string translate(string text, string token, string sourceLanguage, string targetLanguage)
        {
            string txtToTranslate = text;
            string uri = string.Format("http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&from={1}&to={2}", txtToTranslate, sourceLanguage, targetLanguage);
            System.Net.WebRequest translationWebRequest = System.Net.WebRequest.Create(uri);
            translationWebRequest.Headers.Add("Authorization", token);
            System.Net.WebResponse response = null;
            response = translationWebRequest.GetResponse();
            System.IO.Stream stream = response.GetResponseStream();
            System.Text.Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            System.IO.StreamReader translatedStream = new System.IO.StreamReader(stream, encode);
            System.Xml.XmlDocument xTranslation = new System.Xml.XmlDocument();
            xTranslation.LoadXml(translatedStream.ReadToEnd());
            return xTranslation.InnerText;
        }

        private bool processDocument(string token, MethodParameters par, string sourceLanguage, string targetLanguage)
        {
            Dictionary<string,string > translations = new Dictionary<string, string>();
            int n = 0;
            foreach (HtmlNode node in coll)
            {
                n++;
                if (par.Cancel)
                {
                    return false;
                }
                if (node.InnerText == node.InnerHtml)
                {
                    //this dictionary is to translate every sentence only once.
                    if(!translations.ContainsKey(node.InnerText))
                    {
                        translations[node.InnerText] = translate(node.InnerText, token, sourceLanguage, targetLanguage);
                        //translations[node.InnerText] = node.InnerText;
                    }
                    node.InnerHtml = translations[node.InnerText];

                }
                par.SetProgress(n);

            }
            return true;
        }

        [DataContract]
        public class AdmAccessToken
        {
            [DataMember]
            public string access_token { get; set; }

            [DataMember]
            public string token_type { get; set; }

            [DataMember]
            public string expires_in { get; set; }

            [DataMember]
            public string scope { get; set; }
        }
    }
}
