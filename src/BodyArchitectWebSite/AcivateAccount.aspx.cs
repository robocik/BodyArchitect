using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service;
using NHibernate;

public partial class AcivateAccount : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string id = Request.QueryString["Id"];
        if(!string.IsNullOrWhiteSpace(id))
        {
            try
            {
                ISession session = NHibernateFactory.OpenSession();
                using (var transactionScope = session.BeginSaveTransaction())
                {
                    var profile = session.QueryOver<Profile>().Where(x => x.ActivationId == id).SingleOrDefault();
                    if (profile != null)
                    {
                        profile.ActivationId = null;
                        session.Update(profile);
                    }
                    else
                    {
                        throw new ArgumentException("There is no profile with this activationId");
                    }
                    transactionScope.Commit();
                }
                session.Close();
            }
            catch
            {
                lblMessage.Text = "Cannot activate this id";
            }
            
        }
    }
}