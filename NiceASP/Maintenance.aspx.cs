using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NiceApiLibrary.ASP_AppCode;


public partial class Maintenance : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.MaintenanceTitle.Text = "Maintenance in progress";
        this.MaintenanceText.Text = "Please try again in 5 minutes.";
        this.MaintenanceDebug.Text = NiceApiLibrary.DSSwitch.GetMaintenanceLog();
    }
}