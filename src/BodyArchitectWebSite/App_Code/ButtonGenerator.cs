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
    using System.Linq;
    using System.Text;
    public class ButtonGenerator
    {
        public static readonly String SIGNATURE_KEYNAME = "signature";
        public static readonly String SIGNATURE_METHOD_KEYNAME = "signatureMethod";
        public static readonly String SIGNATURE_VERSION_KEYNAME = "signatureVersion";
        public static readonly String SIGNATURE_VERSION_2 = "2";
        public static readonly String HMAC_SHA1_ALGORITHM = "HmacSHA1";
        public static readonly String HMAC_SHA256_ALGORITHM = "HmacSHA256";
        public static readonly String COBRANDING_STYLE = "logo";
        public static readonly String AppName = "ASP";
        public static readonly String HttpPostMethod = "POST";
        public static readonly String SANDBOX_END_POINT = "https://authorize.payments-sandbox.amazon.com/pba/paypipeline";
        public static readonly String SANDBOX_IMAGE_LOCATION = "https://authorize.payments-sandbox.amazon.com/pba/images/payNowButton.png";
        public static readonly String PROD_END_POINT = "https://authorize.payments.amazon.com/pba/paypipeline";
        public static readonly String PROD_IMAGE_LOCATION = "https://authorize.payments.amazon.com/pba/images/payNowButton.png";

            /**
            * Function creates a Map of key-value pairs for all valid values passed to the function 
            * @param accessKey - Put your Access Key here  
            * @param amount - Enter the amount you want to collect for the item
            * @param description - description - Enter a description of the item
            * @param referenceId - Optionally enter an ID that uniquely identifies this transaction for your records
            * @param abandonUrl - Optionally, enter the URL where senders should be redirected if they cancel their transaction
            * @param returnUrl - Optionally enter the URL where buyers should be redirected after they complete the transaction
            * @param immediateReturn - Optionally, enter "1" if you want to skip the final status page in Amazon Payments, 
            * @param processImmediate - Optionally, enter "1" if you want to settle the transaction immediately else "0". Default value is "1"
            * @param ipnUrl - Optionally, type the URL of your host page to which Amazon Payments should send the IPN transaction information.
            * @param collectShippingAddress - Optionally, enter "1" if you want Amazon Payments to return the buyer's shipping address as part of the transaction information.
            * @param signatureMethod -Valid values are  HmacSHA256 and HmacSHA1
            * @return - A map of key of key-value pair for all non null parameters
            * @throws SignatureException
            */
             public static IDictionary<String, String> getSimplePayStandardParams(String accessKey,String amount, String description, String referenceId, String immediateReturn,
                    String returnUrl, String abandonUrl, String processImmediate, String ipnUrl, String collectShippingAddress,
                    String signatureMethod)
            {
                String cobrandingStyle = COBRANDING_STYLE;
          
                IDictionary<String, String> formHiddenInputs = new SortedDictionary<String, String>(StringComparer.Ordinal);

                if (accessKey != null) formHiddenInputs.Add("accessKey", accessKey);
                else throw new System.ArgumentException("AccessKey is required");
                if (amount != null) formHiddenInputs.Add("amount", amount);
                else throw new System.ArgumentException("Amount is required");
                if (description!= null)formHiddenInputs.Add("description", description);
                 else throw new System.ArgumentException("Description is required");
                if (signatureMethod != null) formHiddenInputs.Add(SIGNATURE_METHOD_KEYNAME, signatureMethod);
                 else throw new System.ArgumentException("Signature method is required");
                if (referenceId != null) formHiddenInputs.Add("referenceId", referenceId);
                if (immediateReturn != null) formHiddenInputs.Add("immediateReturn", immediateReturn);
                if (returnUrl != null) formHiddenInputs.Add("returnUrl", returnUrl);
                if (abandonUrl != null) formHiddenInputs.Add("abandonUrl", abandonUrl);
                if (processImmediate != null) formHiddenInputs.Add("processImmediate", processImmediate);
                if (ipnUrl != null) formHiddenInputs.Add("ipnUrl", ipnUrl);
                if (cobrandingStyle != null) formHiddenInputs.Add("cobrandingStyle", cobrandingStyle);
                if (collectShippingAddress != null) formHiddenInputs.Add("collectShippingAddress", collectShippingAddress);
                formHiddenInputs.Add(SIGNATURE_VERSION_KEYNAME, SIGNATURE_VERSION_2);
                return formHiddenInputs;
            }
            /**
             * Creates a form from the provided key-value pairs 
             * @param formHiddenInputs - A map of key of key-value pair for all non null parameters
             * @param serviceEndPoint - The Endpoint to be used based on environment selected
             * @param imageLocation - The imagelocation based on environment
             * @return - An html form created using the key-value pairs
             */

           public static String getSimplePayStandardForm(IDictionary<String, String> formHiddenInputs,String ServiceEndPoint,String imageLocation)
            {
                StringBuilder form = new StringBuilder("<form action=\"" + ServiceEndPoint + "\" method=\"" + HttpPostMethod + "\">\n");
                form.Append("<input type=\"image\" src=\""+ imageLocation + "\" border=\"0\">\n");
                foreach (KeyValuePair<String, String> pair in formHiddenInputs)
                {
                    form.Append("<input type=\"hidden\" name=\"" + pair.Key + "\" value=\"" + pair.Value + "\" >\n");
                }
                form.Append("</form>\n");
                return form.ToString();
            }
           /**
          * Function Generates the html form  
          * @param accessKey - Put your Access Key here  
          * @param secretKey - Put your secret Key here
          * @param amount - Enter the amount you want to collect for the item
          * @param description - description - Enter a description of the item
          * @param referenceId - Optionally enter an ID that uniquely identifies this transaction for your records
          * @param abandonUrl - Optionally, enter the URL where senders should be redirected if they cancel their transaction
          * @param returnUrl - Optionally enter the URL where buyers should be redirected after they complete the transaction
          * @param immediateReturn - Optionally, enter "1" if you want to skip the final status page in Amazon Payments, 
          * @param processImmediate - Optionally, enter "1" if you want to settle the transaction immediately else "0". Default value is "1"
          * @param ipnUrl - Optionally, type the URL of your host page to which Amazon Payments should send the IPN transaction information.
          * @param collectShippingAddress - Optionally, enter "1" if you want Amazon Payments to return the buyer's shipping address as part of the transaction information.
          * @param signatureMethod -Valid values are  HmacSHA256 and HmacSHA1
          * @param environment - Sets the environment where your form will point to can be "sandbox" or "prod"
          * @throws SignatureException
          */

           public static String GenerateForm(String accessKey, String secretKey, String amount, String description, String referenceId, String immediateReturn,
                                      String returnUrl, String abandonUrl, String processImmediate, String ipnUrl, String collectShippingAddress, String signatureMethod, String environment)
            {
               
                    String endPoint, imageLocation;
                    if (environment.Equals("prod"))
                    {

                        endPoint = PROD_END_POINT;
                        imageLocation = PROD_IMAGE_LOCATION;
                    }
                     else
                    {
                        endPoint = SANDBOX_END_POINT;
                        imageLocation = SANDBOX_IMAGE_LOCATION;

                    }

                    Uri serviceEndPoint = new Uri(endPoint);
                    IDictionary<String, String> parameters = getSimplePayStandardParams(accessKey, amount, description, referenceId, immediateReturn,
                                                 returnUrl, abandonUrl, processImmediate, ipnUrl, collectShippingAddress, signatureMethod);
                    String signature = SignatureUtils.signParameters(parameters, secretKey, HttpPostMethod, serviceEndPoint.Host, serviceEndPoint.AbsolutePath,signatureMethod);
                    parameters.Add(SIGNATURE_KEYNAME, signature);
                    String simplePayStandardForm = getSimplePayStandardForm(parameters,endPoint,imageLocation);
                    //Console.WriteLine(simplePayStandardForm);
                    return simplePayStandardForm;
                   
                     
            }
        
    }
}
