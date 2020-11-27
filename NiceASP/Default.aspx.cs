using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;
using NiceASP;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This come from an anonymous browser
        try
        {
            if (!IsPostBack)
            {
                if (!FolderNames.IsDevMachine())
                {
                    if (Request["SCRIPT_NAME"] != "/default.aspx")
                    {
                        Response.Redirect("~/ItemX.aspx?id=Home");
                    }
                }

                Page.MetaKeywords = "API, Online, Send Whatsapp message, Free";
                Page.MetaDescription = "Let your application send you WhatsApp messages. Sign up for free today.";
                NiceASP.KeywordLoader.Load(this, KeywordLoader.Which.Default);
                ExOut.InnerHtml = APIActualSending.fakeRequest("&lt;Your unique X-APIId&gt;", "&lt;Mobile number&gt;", "What a great day");
                ExRes.InnerHtml = APIActualSending.fakeResponse("queued");
            }
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }
}