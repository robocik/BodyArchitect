<%@ Application Language="C#" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="BodyArchitect.DataAccess.NHibernate" %>
<%@ Import Namespace="BodyArchitect.Logger" %>
<%@ Import Namespace="BodyArchitect.Service.V2" %>
<%@ Import Namespace="BodyArchitect.Service.V2.Model" %>
<%@ Import Namespace="BodyArchitect.Service.V2.Payments" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="BodyArchitect.Settings" %>

<script runat="server">

	void Application_Start(object sender, EventArgs e) 
	{
		try
		{
            
			ExceptionHandler.Default.EmailFeaturesEnabled = false;
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
			NHibernateFactory.Initialize();
            //this is for NHibernate to show sql queries in output window in VS
            #if DEBUG
                Console.SetOut(new CustomDebugWriter());
             #endif
		}
		catch (System.Exception ex)
		{
			ExceptionHandler.Default.Process(ex);
			throw;
		}
		
	}

    private FileSystemWatcher ConfigWatcher;
    
    void setLocalization(string culture)
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
    }
    void Application_BeginRequest(object sender, EventArgs e)
    {
        if (Request.QueryString["Token"] != null)
        {
            
            try
            {
                Token token = new Token(new Guid(Request.QueryString["Token"]));
                var securityInfo = InternalBodyArchitectService.SecurityManager.EnsureAuthentication(token);
                try
                {
                    setLocalization(securityInfo.SessionData.Token.Language);
                }
                catch (Exception)
                {
                    Thread.CurrentThread.CurrentCulture=Thread.CurrentThread.CurrentUICulture = ServiceHelper.GetCultureFromCountry(securityInfo.SessionData.Profile.CountryId);
                }
                
            }
            catch (Exception)
            {
            }

        }
        //determine service main path (like http://service.bodyarchitectonline.com/)
        if (ApplicationSettings.ServerUrl == null)
        {
            var segments = Request.Url.Segments.Length;
            ApplicationSettings.ServerUrl = Request.Url.ToString().Substring(0,Request.Url.ToString().LastIndexOf(Request.Url.Segments[segments - 2].ToString()));
            if(ApplicationSettings.ServerUrl.EndsWith("/V2/"))
            {
                ApplicationSettings.ServerUrl=ApplicationSettings.ServerUrl.Replace("/V2/", "/");
            }
            var paymentConfig = Server.MapPath("~/V2/BAPoints.xml");
            loadPaymentsConfiguration(paymentConfig);
            //reload payment config after changes
            ConfigWatcher = new FileSystemWatcher(Server.MapPath("~/V2/"), "BAPoints.xml");
            ConfigWatcher.EnableRaisingEvents = true;
            ConfigWatcher.Changed += (a, b) =>
                                         {
                                             loadPaymentsConfiguration(paymentConfig);
                                         };
        }
    }
    
    void loadPaymentsConfiguration(string file)
    {
        PaymentsManager paymentsManager = new PaymentsManager();
        using (var reader = File.OpenRead(file))
        {
            InternalBodyArchitectService.PaymentsManager=paymentsManager.Load(reader);
        }
        
    }
	
	void Application_End(object sender, EventArgs e) 
	{
		//  Code that runs on application shutdown

	}
		
	void Application_Error(object sender, EventArgs e) 
	{ 
		// Code that runs when an unhandled error occurs

	}

	void Session_Start(object sender, EventArgs e) 
	{
		// Code that runs when a new session is started

	}

	void Session_End(object sender, EventArgs e) 
	{
		// Code that runs when a session ends. 
		// Note: The Session_End event is raised only when the sessionstate mode
		// is set to InProc in the Web.config file. If session mode is set to StateServer 
		// or SQLServer, the event is not raised.

	}
	   
</script>
