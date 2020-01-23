/******************************************************************************* 
 *  Copyright 2008-2010 Amazon Technologies, Inc.
 *  Licensed under the Apache License, Version 2.0 (the "License"); 
 *  
 *  You may not use this file except in compliance with the License. 
 *  You may obtain a copy of the License at: http://aws.amazon.com/apache2.0
 *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
 *  CONDITIONS OF ANY KIND, either express or implied. See the License for the 
 *  specific language governing permissions and limitations under the License.
 * ***************************************************************************** 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Security.Permissions;
namespace BodyArchitect.Service.V2.Payments
{
    public class SignatureUtilsForOutbound
    {
        private static readonly string CERTIFICATE_URL_KEYNAME = "certificateUrl";
        private static readonly string SIGNATURE_KEYNAME = "signature";
        private static readonly string SIGNATURE_METHOD_KEYNAME = "signatureMethod";
        private static readonly string SIGNATURE_VERSION_KEYNAME = "signatureVersion";

        private static readonly string FPS_PROD_ENDPOINT = "https://fps.amazonaws.com/";
        private static readonly string FPS_SANDBOX_ENDPOINT = "https://fps.sandbox.amazonaws.com/";

        private static readonly string ACTION_PARAM = "?Action=VerifySignature";
        private static readonly string END_POINT_PARAM = "&UrlEndPoint=";
        private static readonly string HTTP_PARAMS_PARAM = "&HttpParameters=";
        // current FPS API version, needs updates if new API versions are released
        private static readonly string VERSION_PARAM_VALUE = "&Version=2008-09-17";

        public static readonly string SUCCESS_RESPONSE =
            "<VerificationStatus>Success</VerificationStatus>";

        public static readonly string USER_AGENT_STRING = "amazon-fps-2008-09-17-cs-library-2010-09-13";

        public SignatureUtilsForOutbound()
        {
        }

        /**
         * Validates the request by checking the integrity of its parameters.
         * @param parameters - all the http parameters sent in IPNs or return urls. 
         * @param urlEndPoint should be the url which recieved this request. 
         * @param httpMethod should be either POST (IPNs) or GET (returnUrl redirections)
         */
        public bool ValidateRequest(IDictionary<string, string> parameters,
               string urlEndPoint, string httpMethod)
        {
            //1. input validation.
            if (parameters == null || parameters.Keys.Count == 0)
            {
                throw new Exception("must provide http parameters.");
            }

            string signature = parameters[SIGNATURE_KEYNAME];
            if (String.IsNullOrEmpty(signature))
            {
                throw new Exception("'signature' is missing from the parameters.");
            }

            string signatureVersion = parameters[SIGNATURE_VERSION_KEYNAME];
            if (String.IsNullOrEmpty(signatureVersion))
            {
                throw new Exception("'signatureVersion' is missing from the parameters.");
            }

            string signatureMethod = parameters[SIGNATURE_METHOD_KEYNAME];
            if (String.IsNullOrEmpty(signatureMethod))
            {
                throw new Exception("'signatureMethod' is missing from the parameters.");
            }

            string certificateUrl = parameters[CERTIFICATE_URL_KEYNAME];
            if (certificateUrl == null)
            {
                throw new Exception("'certificateUrl' is missing from the parameters.");
            }

            if (String.IsNullOrEmpty(urlEndPoint))
            {
                throw new Exception("must provide url end point.");
            }

            StringBuilder requestUrlBuilder = new StringBuilder();

            // 2. determine production or sandbox endpoint
            if (certificateUrl.StartsWith(FPS_PROD_ENDPOINT))
            {
                requestUrlBuilder.Append(FPS_PROD_ENDPOINT);
            }
            else if (certificateUrl.StartsWith(FPS_SANDBOX_ENDPOINT))
            {
                requestUrlBuilder.Append(FPS_SANDBOX_ENDPOINT);
            }
            else
            {
                throw new Exception("certificate url was incorrect.");
            }

            // 3. build VerifySignature request URL
            requestUrlBuilder.Append(ACTION_PARAM);
            requestUrlBuilder.Append(END_POINT_PARAM);
            requestUrlBuilder.Append(urlEndPoint);
            requestUrlBuilder.Append(HTTP_PARAMS_PARAM);
            requestUrlBuilder.Append(buildHttpParams(parameters));
            requestUrlBuilder.Append(VERSION_PARAM_VALUE);

            // 4. make call to VerifySignature API
            string verifySignatureResponse = getUrlContents(requestUrlBuilder.ToString());
            return verifySignatureResponse.Contains(SUCCESS_RESPONSE);
        }

        private string buildHttpParams(IDictionary<string, string> httpParams)
        {
            StringBuilder paramsBuilder = new StringBuilder();
            Boolean first = true;
            foreach (string key in httpParams.Keys)
            {
                if (!first)
                {
                    paramsBuilder.Append("&");
                }
                else
                {
                    first = false;
                }
                paramsBuilder.Append(urlEncode(key));
                paramsBuilder.Append("=");
                paramsBuilder.Append(urlEncode(httpParams[key]));
            }
            return urlEncode(paramsBuilder.ToString());
        }

        private string urlEncode(string value)
        {
            return HttpUtility.UrlEncodeUnicode(value);
        }

        public string getUrlContents(string url)
        {
            Console.WriteLine(url);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.AllowAutoRedirect = false;
            request.UserAgent = USER_AGENT_STRING;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8) as StreamReader)
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                throw new Exception("recieved response error", ex);
            }
        }
    }
}
