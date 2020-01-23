using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Payments;

public partial class V2_Przelewy24OrderProcessing : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var session = NHibernateFactory.OpenSession();
        var payPal = new Przelewy24Handler(InternalBodyArchitectService.PaymentsManager);
        try
        {
            payPal.ProcessOrderRequest(session, Request.Form, Context);
            Response.Redirect("PaymentsFinished.aspx?Transferuj=1");
        }
        catch (Exception ex)
        {
            ExceptionHandler.Default.Process(ex);
        }
    }
}