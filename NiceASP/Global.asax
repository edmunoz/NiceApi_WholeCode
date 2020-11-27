<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="NiceApiLibrary.ASP_AppCode" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        log4net.Config.XmlConfigurator.Configure();
        MyLog.GetLogger("Global").Info("ASP start");
        NiceApiLibrary.DSSwitch.NiceApiLibrary_StartUp(MyLog.GetLogger("Global"), false);
        CountryListLoader.Load();
        NiceASP.RouteConfig.RegisterRoutes(RouteTable.Routes);
        NiceASP.BundleConfig.RegisterBundles(BundleTable.Bundles);

        using (DataFile_Loopback ld = new DataFile_Loopback(DataFile_Base.OpenType.ForUpdate_CreateIfNotThere))
        {
            ld.ASP_Rebuild_Time = DateTime.UtcNow; 
        }
    }

    void Application_End(object sender, EventArgs e)
    {
        MyLog.GetLogger("Global").Info("ASP ending...");
        NiceApiLibrary.DSSwitch.NiceApiLibrary_End();
        MyLog.GetLogger("Global").Info("ASP end done");
    }

</script>
