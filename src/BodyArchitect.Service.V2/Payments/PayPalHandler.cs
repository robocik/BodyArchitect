using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;

namespace BodyArchitect.Service.V2.Payments
{
    public class PayPalHandler : PaymentsHandlerBase
    {
        public PayPalHandler(PaymentsHolder paymentsManager):base(paymentsManager)
        {
        }

        public string PayPalUrl
        {
            get
            {
                if (UseSandbox)
                {
                    return "https://www.sandbox.paypal.com/cgi-bin/webscr";
                }
                else
                {
                    return "https://www.paypal.com/cgi-bin/webscr";
                }
            }
        }

        public string BAPoints_30_Button
        {
            get
            {
                if (UseSandbox)
                {
                    return "VQCK4MLHXHC7A";
                }
                else
                {
                    return "KJCEAWPSJKH7A";
                }
            }
        }

        public string BAPoints_120_Button
        {
            get
            {
                if (UseSandbox)
                {
                    return "ANNN5SKPLVEUW";
                }
                else
                {
                    return "93U94ZV2KKNMW";
                }
            }
        }

        public virtual string ReceiverEmail
        {
            get
            {
                if(UseSandbox)
                {
                    return "receiver_1342816399_biz@poczta.onet.pl";
                }
                return "receiver@gmail.com";
            }
        }

        protected virtual string SendPayPalRequest(HttpRequest request)
        {
            //Post back to either sandbox or live
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(PayPalUrl);
            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            //byte[] param = request.BinaryRead(HttpContext.Current.Request.ContentLength);
            byte[] param = request.BinaryRead(request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);
            strRequest += "&cmd=_notify-validate";
            req.ContentLength = strRequest.Length;
            Log.Write("Sent validation notify", TraceEventType.Information, "PayPal");
            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://url:port#"));
            //req.Proxy = proxy;

            //Send the request to PayPal and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            streamOut.Write(strRequest);
            streamOut.Close();
            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            streamIn.Close();
            return strResponse;
        }

        public void ProcessOrderRequest(ISession session,NameValueCollection requestData, HttpRequest request)
        {
            string requestForm = getFormString(requestData);
            Log.Write("Request={0}", TraceEventType.Information, "PayPal", requestForm);

            var strResponse = SendPayPalRequest(request);
            Log.Write("Received respose", TraceEventType.Information, "PayPal");

            if (strResponse == "VERIFIED")
            {
                //check the payment_status is Completed
                //check that txn_id has not been previously processed
                //check that receiver_email is your Primary PayPal email
                //check that payment_amount/payment_currency are correct
                //process payment

                Log.Write("PayPal callback request: VERIFIED", TraceEventType.Information, "PayPal");

                if (requestData["receiver_email"] != ReceiverEmail )
                {
                    Log.Write("Invalid paypal callback request. Invalid email.", TraceEventType.Error, "PayPal");
                    throw new ValidationException("Invalid paypal callback request. Invalid email");
                }

                if ( requestData["txn_type"] != "web_accept")
                {
                    Log.Write("Invalid paypal callback request. Not web_accept.", TraceEventType.Error, "PayPal");
                    throw new ValidationException("Invalid paypal callback request. Not web_accept");
                }

                //the amount of payment, the status of the payment, amd a possible reason of delay
                //The fact that Getting txn_type=web_accept or txn_type=subscr_payment are got odes not mean that
                //seller will receive the payment.
                //That's why we check payment_status=completed. The single exception is when the seller's account in
                //not American and pending_reason=intl
                if (requestData["payment_status"] != "Completed" && requestData["pending_reason"] != "intl")
                {
                    Log.Write("Wrong payment status", TraceEventType.Error, "PayPal");
                    return;
                }

                Log.Write("receiver_email is correct", TraceEventType.Information, "PayPal");

                string transactionId = requestData["txn_id"];
                string profileId = requestData["custom"];
                string pointsPlan = requestData["item_number"];
                
                writeBAPointsToDb(transactionId, profileId, BAPointsType.PayPal, session,pointsPlan);
            }
            else if (strResponse == "INVALID")
            {
                Log.WriteError("Invalid paypal callback request. Form={0}", requestForm);
                Log.Write("INVALID", TraceEventType.Error, "PayPal");
                throw new InvalidDataException("Invalid paypal callback request");
            }
            else
            {
                //log response/ipn data for manual investigation
                Log.Write("LAST ELSE", TraceEventType.Error, "PayPal");
            }
        }

    }
}
