using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceASP;

public partial class AdminImages : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from the Admin User
        try
        {
            string dispalyText = "";
            using (DataFile_ScreenShot ss = new DataFile_ScreenShot(SessionData.SessionsSystem_Get(Session), DataFile_Base.OpenType.ReadOnly_CreateIfNotThere))
            {
                dispalyText += String.Format("Size: {0} x {1} ", ss.imgScreen.Width, ss.imgScreen.Height);
            }
            //using (NiceASP.DataFile_Loopback ld = new NiceASP.DataFile_Loopback(NiceASP.SessionData.SessionsSystem_Get(Session), NiceASP.DataFile_Base.OpenType.ReadOnly_CreateIfNotThere))
            //{
            //    dispalyText += "Received: " + ld.processedScreenshotDoneTime.ToString();
            //}

            LiteralText.Text = "<pre>" + dispalyText + "</pre>";

        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }
}