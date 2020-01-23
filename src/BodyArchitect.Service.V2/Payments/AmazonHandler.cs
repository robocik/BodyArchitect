using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.V2.Model;
using System.Configuration;
using System.Web;
using System.Net;
using NHibernate;
using System.Collections.Specialized;
using System.Diagnostics;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Shared;
using System.IO;

namespace BodyArchitect.Service.V2.Payments
{
    public class AmazonHandler : PaymentsHandlerBase
    {
        public AmazonHandler(PaymentsHolder paymentsManager):base(paymentsManager)
        {
        }

        public string AmazonUrl
        {
            get
            {
                if (UseSandbox)
                {
                    return "https://authorize.payments-sandbox.amazon.com/pba/paypipeline";
                }
                else
                {
                    return "https://authorize.payments.amazon.com/pba/paypipeline";
                }
            }
        }

        public virtual string ReceiverEmail
        {
            get { return ConfigurationManager.AppSettings["AmazonEmail"]; } //Do we need this??
        }

        public string AmazonNotifyUrl 
        { 
            get
            {
                return "http://test.bodyarchitectonline.com/V2/AmazonOrderProcessing.aspx";
            } 
        }

        protected virtual bool SendAmazonRequest(HttpRequest request)
        {
            NameValueCollection requestData = request.Form;
            var result = false;
            try
            {
                //Check signature
                SignatureUtilsForOutbound utils = new SignatureUtilsForOutbound();
                IDictionary<String, String> pars = new Dictionary<String, String>();
                for (int i = 0; i < requestData.Count; i++)
                    pars.Add(requestData.GetKey(i), requestData.Get(i));

                result = utils.ValidateRequest(pars, AmazonNotifyUrl, "POST");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = false;
            }
            return result;
        }

        public void ProcessOrderRequest(ISession session, NameValueCollection requestData, HttpRequest request)
        {
            string requestForm = getFormString(requestData);
            Log.Write("Request={0}", TraceEventType.Information, "PayPal", requestForm);
            Log.Write("userId={0}", TraceEventType.Information, "PayPal", requestData.Get("referenceId"));

            var result = SendAmazonRequest(request);
            Log.Write("Amazon callback request: ", TraceEventType.Information, "PayPal", result);
            if (result == true)
            {
                var strResponse = "";
                strResponse = requestData.Get("status");

                Log.Write("Received response: {0}", TraceEventType.Information, "PayPal", strResponse);

                if (strResponse == "PS")
                {
                    /* TODO: Add other return codes if necessary */
                    string transactionId = requestData["transactionId"];
                    string profileId = requestData["referenceId"];

                    Log.Write("Payment accepted", TraceEventType.Information, "PayPal");
                    writeBAPointsToDb(transactionId, profileId, BAPointsType.PayPal, session, "BAPoints_30");

                }
                else if (strResponse == "PI")   //payment initiated, it will take 5sec - 48h to complete
                {
                    Log.Write("Payment initiated", TraceEventType.Information, "PayPal");
                    //IPN requests should do the job if payment status change
                }
                else
                {
                    //log response/ipn data for manual investigation
                    Log.Write("LAST ELSE", TraceEventType.Error, "PayPal");
                }
            }
            else
            {
                Log.WriteError("Invalid amazon callback request. Form={0}", requestForm);
                Log.Write("INVALID", TraceEventType.Error, "PayPal");
                throw new InvalidDataException("Invalid amazon callback request");
            }
        }

    }
}
