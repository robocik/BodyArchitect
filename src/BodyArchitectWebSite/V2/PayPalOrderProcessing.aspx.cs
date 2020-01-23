using System;
using System.Diagnostics;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Payments;

public partial class V2_PayPalOrderProcessing : System.Web.UI.Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        BodyArchitect.Logger.Log.Write("PayPalOrderProcessing started", TraceEventType.Information, "PayPal");
        var session = NHibernateFactory.OpenSession();
        PayPalHandler payPal = new PayPalHandler(InternalBodyArchitectService.PaymentsManager);
        try
        {
            payPal.ProcessOrderRequest(session, Request.Form, Request);
        }
        catch (Exception ex)
        {
            ExceptionHandler.Default.Process(ex);
        }
        
    }
}