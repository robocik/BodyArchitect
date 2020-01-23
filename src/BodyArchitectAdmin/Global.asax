<%@ Application Language="C#" %>
<%@ Import Namespace="BodyArchitect.DataAccess.Converter.V4_V5" %>
<%@ Import Namespace="BodyArchitect.DataAccess.NHibernate" %>
<%@ Import Namespace="BodyArchitect.Logger" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        ExceptionHandler.Default.EmailFeaturesEnabled = false;
        //NHibernateConfigurator.ConfigureMySqlDatabases();
        NHibernateFactory.Initialize();
#if DEBUG
                Console.SetOut(new CustomDebugWriter());
#endif
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
