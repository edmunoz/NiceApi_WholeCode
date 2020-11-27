using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using NiceASP;
using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;

public partial class API : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from an external user

        string XAPIId = Request.Headers["X-APIId"];
        string XAPIMobile = Request.Headers["X-APIMobile"];
        string Message = "";
        IMyLog log = MyLog.GetLogger("API");

        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            Message = sr.ReadToEnd();
        }

        Response.ContentType = "text/plain";
        Response.Write(new APIActualSending(XAPIId.GetSystemInfoFromAPIId()).SendWhatsApp(
            XAPIId,
            XAPIMobile,
            Message,
            log));
    }
}
