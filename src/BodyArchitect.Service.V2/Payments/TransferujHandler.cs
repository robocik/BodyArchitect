using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;

namespace BodyArchitect.Service.V2.Payments
{
    public class TransferujHandler : PaymentsHandlerBase
    {
        public TransferujHandler(PaymentsHolder paymentsManager):base(paymentsManager)
        {
        }

        public string Url
        {
            get { return "https://secure.transferuj.pl"; }
        }

        public string TransferujId
        {
            get { return "6025"; }
        }

        public string TransferujKodPotwierdzajcy
        {
            get { return "BA_Code_Potwierdzajacy"; }
        }

        protected void ValidateRequest(Dictionary<string, string> data)
        {
            Log.Write("UserHostAddress={0}", TraceEventType.Information, "PayPal", data["UserHostAddress"]);
            if (data["UserHostAddress"] != "195.149.229.109")
            {
                Log.Write("Request comes from incorrect ip address. IP={0}", TraceEventType.Error, "PayPal", data["UserHostAddress"]);
                throw new ValidationException("Invalid transferuj.pl transaction status. Not TRUE");
            }
        }

        protected virtual Dictionary<string,string>  CreateAdditionalData(HttpContext context)
        {
            Dictionary<string,string> data = new Dictionary<string, string>();
            data.Add("UserHostAddress",context.Request.UserHostAddress);
            return data;
        }

        protected virtual void SendConfirmation(HttpContext request)
        {
            //HttpContext.Current.Response.Clear();
            
            HttpContext.Current.Response.Write("TRUE");
           // HttpContext.Current.Response.End();
        }

        public void ProcessOrderRequest(ISession session, NameValueCollection form, HttpContext request)
        {
            string requestForm = getFormString(form);
            var data=CreateAdditionalData(request);
            Log.Write("Request={0}", TraceEventType.Information, "PayPal", requestForm);
            
            
            
            string idSprzedawcy = form["id"];
            string transactionStatus = form["tr_status"];
            string transactionId = form["tr_id"];
            string kwota = form["tr_amount"];
            string kwota_zaplacona = form["tr_paid"];
            string isError = form["tr_error"];
            string custom = form["tr_crc"];//profile id|BA_Points_{points number}
            string retrevedMd5 = form["md5sum"];

            var arr=custom.Split('|');
            if(arr.Length!=2)
            {
                throw new ValidationException("Invalid crc");
            }
            string profileId=arr[0];
            string pointsPlan = arr[1];
            string calculatedMd5 = FormsAuthentication.HashPasswordForStoringInConfigFile(idSprzedawcy + transactionId + kwota + custom + TransferujKodPotwierdzajcy, "MD5");
            Log.Write("calculatedMd5={0}", TraceEventType.Information, "PayPal", calculatedMd5);
            Log.Write("retrevedMd5={0}", TraceEventType.Information, "PayPal", retrevedMd5);
            Log.Write("idSprzedawcy={0}", TraceEventType.Information, "PayPal", idSprzedawcy);
            Log.Write("kwota={0}", TraceEventType.Information, "PayPal", kwota);
            Log.Write("profileId={0}", TraceEventType.Information, "PayPal", profileId);
            Log.Write("TransferujKodPotwierdzajcy={0}", TraceEventType.Information, "PayPal", TransferujKodPotwierdzajcy);

            ValidateRequest(data);

            if (transactionStatus != "TRUE")
            {
                Log.Write("Invalid transferuj.pl transaction status. Not TRUE. Form={0}", TraceEventType.Error,"PayPal", transactionStatus);
                throw new ValidationException("Invalid transferuj.pl transaction status. Not TRUE");
            }
            if (isError!= "none" )
            {
                Log.Write("Transferuj.pl returns error. Error={0}", TraceEventType.Error,"PayPal", isError);
                throw new ValidationException("Transferuj.pl returns error.");
            }

            if (StringComparer.CurrentCultureIgnoreCase.Compare(retrevedMd5,calculatedMd5)!=0)
            {
                Log.Write("MD5 checking failed", TraceEventType.Error,"PayPal");
                throw new ValidationException("MD5 checking failed");
            }
            Log.Write("Request is ok", TraceEventType.Information, "PayPal");
            SendConfirmation(request);
            //everything is ok
            writeBAPointsToDb(transactionId, profileId, BAPointsType.Transferuj, session, pointsPlan);
        }


        
    }
}
