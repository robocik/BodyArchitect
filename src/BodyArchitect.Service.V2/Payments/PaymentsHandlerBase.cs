using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;

namespace BodyArchitect.Service.V2.Payments
{
    public abstract class PaymentsHandlerBase
    {
        private PaymentsHolder paymentsManager;

        public PaymentsHandlerBase(PaymentsHolder paymentsManager)
        {
            this.paymentsManager = paymentsManager;
            UseSandbox = ConfigurationManager.AppSettings["UsePayPalSandBox"]=="true";
        }
        public bool UseSandbox { get; set; }

        // This helper method encodes a string correctly for an HTTP POST
        private string Encode(string oldValue)
        {
            string newValue = oldValue.Replace("\"", "'");
            newValue = System.Web.HttpUtility.UrlEncode(newValue);
            newValue = newValue.Replace("%2f", "/");
            return newValue;
        }

        protected  string getFormString(NameValueCollection requestData)
        {
            StringBuilder builder = new StringBuilder();
            foreach (String postKey in requestData)
            {
                string postValue = Encode(requestData[postKey]);
                builder.AppendFormat("{0}={1}&", postKey, postValue);
            }
            return builder.ToString();
        }

        int getPointsFromItemNumber(string itemNumber)
        {
            var points = paymentsManager.GetPointsForOffert(itemNumber);
            if (points > 0)
            {
                return points;
            }
            throw new ArgumentException("Wrong item number");
        }
        protected void writeBAPointsToDb(string transactionId,string profileIdString,BAPointsType paymentType, ISession session,string pointsName)
        {
            Guid profileId = new Guid(profileIdString);

            using (var tx = session.BeginSaveTransaction())
            {
                //first check if this order has been already processed

                var count = session.QueryOver<BAPoints>().Where(x => x.Identifier == transactionId).RowCount();
                if (count > 0)
                {
                    Log.Write("TransactionId {0} is already processed.", TraceEventType.Warning, "PayPal", transactionId);
                    throw new UniqueException("This transaction is already processed");
                }
                Log.Write("TransactionId is not duplicated We can continue.", TraceEventType.Information, "PayPal", transactionId);


                var pointsToAdd = getPointsFromItemNumber(pointsName);
                Log.Write("Request is OK. Adding {0} BA points for ProfileId {1}", TraceEventType.Error, "PayPal", pointsToAdd, profileId);
                var profile = session.Get<Profile>(profileId);
                BAPoints points = new BAPoints();
                points.ImportedDate = DateTime.UtcNow;
                points.Profile = profile;
                points.Points = pointsToAdd;
                points.Type = paymentType;
                points.Identifier = transactionId;
                session.Save(points);
                profile.Licence.BAPoints += points.Points;
                session.Update(profile);
                tx.Commit();
                Log.Write("Everything OK. END", TraceEventType.Error, "PayPal", pointsToAdd);
            }
        }
    }
}
