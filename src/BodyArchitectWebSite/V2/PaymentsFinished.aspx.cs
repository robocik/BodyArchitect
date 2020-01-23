using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class V2_PayementsFinished : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Request.QueryString["Transferuj"]==null)
        {
            lblPayPalDescription.Visible = true;
            lblTransferujDescription.Visible = false;
        }
        else
        {
            lblPayPalDescription.Visible =false;
            lblTransferujDescription.Visible = true;
        }
    }
}