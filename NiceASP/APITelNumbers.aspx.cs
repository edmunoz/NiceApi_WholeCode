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

public partial class APITelNumbers : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from the AdminTool

        try
        {
            string XAPIId = Request.Headers["X-APIId"];
            string XAPIInstruction = Request.Headers["X-APIInstruction"];
            string Message = "";
            IMyLog log = MyLog.GetLogger("API");

            using (StreamReader sr = new StreamReader(Request.InputStream))
            {
                Message = sr.ReadToEnd();
            }

            MessageProcessing_API api = new MessageProcessing_API(XAPIId);
            string result = api.Process_TelNumAPI(XAPIId.GetSystemInfoFromAPIId(), XAPIInstruction, Message, log);
            Response.ContentType = "text/plain";
            Response.Write(result);
        }
        catch (DataUnavailableException due)
        {
            Response.ContentType = "text/plain";
            Response.Write(due.Message);
        }
    }
}
