using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;

namespace BodyArchitect.Service.V2.Payments
{
    public class Przelewy24Handler : PaymentsHandlerBase
    {

        public Przelewy24Handler(PaymentsHolder paymentsManager):base(paymentsManager)
        {
        }

        public string KluczCRC
        {
            get { return "3d8a14d790e89125"; }
        }

        public string Url
        {
            get
            {
                if (UseSandbox)
                {
                    return "https://sandbox.przelewy24.pl/index.php";
                }
                else
                {
                    return "https://secure.przelewy24.pl/index.php";
                }
            }
        }

        public string ConfirmationUrl
        {
            get
            {
                if(UseSandbox)
                {
                    return "https://sandbox.przelewy24.pl/transakcja.php";
                }
                else
                {
                    return "https://secure.przelewy24.pl/transakcja.php";
                }
            }
        }

        public string NotifyUrl
        {
            get
            {
                if (UseSandbox)
                {
                    return "http://test.bodyarchitectonline.com/V2/Przelewy24OrderProcessing.aspx";
                }
                else
                {
                    return "http://service.bodyarchitectonline.com/V2/Przelewy24OrderProcessing.aspx";
                }
                
            }
        }

        public string MyId
        {
            get { return "17035"; }
        }

        public void ProcessOrderRequest(ISession session, NameValueCollection form, HttpContext request)
        {
            string requestForm = getFormString(form);
            Log.Write("Request={0}", TraceEventType.Information, "PayPal", requestForm);

            string order_id = form["p24_order_id"];
            var arr = form["p24_session_id"].Split('|');
            string profile_id = arr[1];
            string pointsPlan = arr[2];
            //string kwota = form["p24_kwota "];

            var strResponse = SendPayPalRequest(request);
            Log.Write("Received respose: {0}", TraceEventType.Information, "PayPal",strResponse);
            strResponse = strResponse.Replace("RESULT\r\n","");
            if(strResponse=="TRUE")
            {
                Log.Write("Request is ok", TraceEventType.Information, "PayPal");
                //everything is ok
                writeBAPointsToDb(order_id, profile_id, BAPointsType.Przelewy24, session, pointsPlan);
            }
            else
            {
                Log.WriteError("Invalid paypal callback request. Form={0}", requestForm);
                Log.Write("INVALID", TraceEventType.Error, "PayPal");
                throw new InvalidDataException("Invalid przelewy24 callback request");
            }
            //if(strResponse=="")
            //{

            //}
        }

        protected virtual string SendPayPalRequest(HttpContext request)
        {
            string sessionId = request.Request.Form["p24_session_id"];
            string orderId = request.Request.Form["p24_order_id"];
            string kwota = request.Request.Form["p24_kwota"];

            //Post back to either sandbox or live
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(ConfirmationUrl);
            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            string value = sessionId + "|" + orderId + "|" + kwota + "|" + KluczCRC;
            string crc = FormsAuthentication.HashPasswordForStoringInConfigFile(value, "MD5").ToLower();

            string responseString = "p24_session_id=" +sessionId + "&p24_order_id=" + orderId+ "&p24_id_sprzedawcy=" + MyId + "&p24_kwota=" + kwota+ "&p24_crc="+crc;
            req.ContentLength = responseString.Length;
            Log.Write("Sent validation notify", TraceEventType.Information, "PayPal");
            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://url:port#"));
            //req.Proxy = proxy;

            //Send the request to PayPal and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            streamOut.Write(responseString);
            streamOut.Close();
            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            streamIn.Close();
            return strResponse;
        }
    }
}
