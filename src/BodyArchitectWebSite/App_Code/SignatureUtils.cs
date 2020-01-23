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

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class SignatureUtils
    {
        public static readonly String SIGNATURE_KEYNAME = "signature";
        // Constants used when constructing the string to sign for v2
        public static readonly String AppName = "ASP";
        public static readonly String NewLine = "\n";
        public static readonly String EmptyUriPath = "/";
        public static String equals = "=";
        public static readonly String And = "&";
        public static readonly String UTF_8_Encoding = "UTF-8";

        /**
	 * Computes RFC 2104-compliant HMAC signature for request parameters This
	 * involves 2 steps - Calculate string-to-sign and then compute signature
	 * 
	 * Step 1: Calculate string-to-sign
	 *  In Signature Version 2, string to sign is based on following:
	 * 
	 * 1. The HTTP Request Method (POST or GET) followed by an ASCII newline
	 * (%0A) 2. The HTTP Host header in the form of lowercase host, followed by
	 * an ASCII newline. 3. The URL encoded HTTP absolute path component of the
	 * URI (up to but not including the query string parameters); if this is
	 * empty use a forward '/'. This parameter is followed by an ASCII newline.
	 * 4. The concatenation of all query string components (names and values) as
	 * UTF-8 characters which are URL encoded as per RFC 3986 (hex characters
	 * MUST be uppercase), sorted using lexicographic byte ordering. Parameter
	 * names are separated from their values by the '=' character (ASCII
	 * character 61), even if the value is empty. Pairs of parameter and values
	 * are separated by the '&' character (ASCII code 38).
	 * 
	 * Step 2: Compute RFC 2104-compliant HMAC signature
	 */



        public static String signParameters(IDictionary<String, String> parameters, String key, String HttpMethod, String Host, String RequestURI,String algorithm) 
        {
            String stringToSign = null;
            stringToSign = calculateStringToSignV2(parameters, HttpMethod, Host, RequestURI);
            return sign(stringToSign, key, algorithm);
        }


        /**
    	 * Calculate String to Sign for SignatureVersion 2
	     * @param parameters
    	 * @param httpMethod - POST or GET
	     * @param hostHeader - Service end point
    	 * @param requestURI - Path
	     * @return
    	 */
        private static String calculateStringToSignV2(IDictionary<String, String> parameters, String httpMethod, String hostHeader, String requestURI)// throws SignatureException
        {
            StringBuilder stringToSign = new StringBuilder();
            if (httpMethod == null) throw new Exception("HttpMethod cannot be null");
            stringToSign.Append(httpMethod);
            stringToSign.Append(NewLine);

            // The host header - must eventually convert to lower case
            // Host header should not be null, but in Http 1.0, it can be, in that
            // case just append empty string ""
            if (hostHeader == null)
                stringToSign.Append("");
            else
                stringToSign.Append(hostHeader.ToLower());
            stringToSign.Append(NewLine);

            if (requestURI == null || requestURI.Length == 0)
                stringToSign.Append(EmptyUriPath);
            else
                stringToSign.Append(UrlEncode(requestURI, true));
            stringToSign.Append(NewLine);

            IDictionary<String, String> sortedParamMap = new SortedDictionary<String, String>(parameters, StringComparer.Ordinal);
            foreach (String key in sortedParamMap.Keys)
            {
                if (String.Compare(key, SIGNATURE_KEYNAME, true) == 0) continue;
                stringToSign.Append(UrlEncode(key, false));
                stringToSign.Append(equals);
                stringToSign.Append(UrlEncode(sortedParamMap[key], false));
                stringToSign.Append(And);
            }

            String result = stringToSign.ToString();
            return result.Remove(result.Length - 1);
        }

        public static String UrlEncode(String data, bool path)
        {
            StringBuilder encoded = new StringBuilder();
            String unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~" + (path ? "/" : "");

            foreach (char symbol in System.Text.Encoding.UTF8.GetBytes(data))
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    encoded.Append(symbol);
                }
                else
                {
                    encoded.Append("%" + String.Format("{0:X2}", (int)symbol));
                }
            }

            return encoded.ToString();

        }

        /**
         * Computes RFC 2104-compliant HMAC signature.
         */
        public static String sign(String data, String key, String signatureMethod)// throws SignatureException
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                HMAC Hmac = HMAC.Create(signatureMethod);
                Hmac.Key = encoding.GetBytes(key);
                Hmac.Initialize();
                CryptoStream cs = new CryptoStream(Stream.Null, Hmac, CryptoStreamMode.Write);
                cs.Write(encoding.GetBytes(data), 0, encoding.GetBytes(data).Length);
                cs.Close();
                byte[] rawResult = Hmac.Hash;
                String sig = Convert.ToBase64String(rawResult, 0, rawResult.Length);
                return sig;
            }
            catch (Exception e)
            {
                throw new AmazonFPSException("Failed to generate signature: " + e.Message);
            }
        }

    }
}
