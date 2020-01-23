using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Service.V2;
using BodyArchitect.Logger;

public partial class V2_AmazonOrderProcessing : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var session = NHibernateFactory.OpenSession();
        AmazonHandler amazon = new AmazonHandler(InternalBodyArchitectService.PaymentsManager);
        try
        {
            amazon.ProcessOrderRequest(session, Request.Form, Request);
        }
        catch (Exception ex)
        {
            ExceptionHandler.Default.Process(ex);
        }
    }
}