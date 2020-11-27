using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

using NiceASP;
using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;

public partial class APIPrivAddTel : System.Web.UI.Page
{
    // This comes from the AdminTool

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            MobileNoHandler XTelList = new MobileNoHandler(Request.Headers["X-TelList"]
                .Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", ""));
            if (!string.IsNullOrEmpty(Request.Headers["X-EmailB64"]))
            {
                doAddTel(
                    XTelList, 
                    Encoding.ASCII.GetString(Convert.FromBase64String(Request.Headers["X-EmailB64"])));
            }
            else if (!string.IsNullOrEmpty(Request.Headers["X-CheckTel"]))
            {
                doCheckTel(XTelList);
            }
            else
            {
                throw new ArgumentException("Wrong request");
            }
        }
        catch (Exception se)
        {
            Response.ContentType = "text/plain";
            Response.Write(se.Message);
        }
    }
    
    private void doAddTel(MobileNoHandler XTelList, string XEmail)
    {
        string Message = "";
        IMyLog log = MyLog.GetLogger("API");

        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            Message = sr.ReadToEnd();
        }
        Response.ContentType = "text/plain";

        // see if the numbers are already used
        TelListController currentList = DSSwitch.appUser().GetCurrentTelList(log);
        Response.Write("currentList loaded. Checking...\r\n");
        bool okToAdd = true;
        using (TelListController.Locker currentListLock = currentList.GetLock())
        {
            foreach (string tel1 in XTelList.MobileNumberArray)
            {
                if (currentListLock.Locked.GetEntry(tel1) == null)
                {
                    Response.Write(String.Format("Tel {0} OK\r\n", tel1));
                }
                else
                {
                    Response.Write(String.Format("*** Tel {0} ALREADY REGISTRED\r\n", tel1));
                    okToAdd = false;
                }
            }
        }

        if (okToAdd)
        {
            MessageProcessing_API api = new MessageProcessing_API(XEmail, true);
            string result = api.Process_MGUseAddTelToFreeAccounts(NiceSystemInfo.DEFAULT, XTelList, log);
            Response.Write(result);
        }
    }

    private void doCheckTel(MobileNoHandler XTelList)
    {
        string Message = "";
        IMyLog log = MyLog.GetLogger("API");

        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            Message = sr.ReadToEnd();
        }
        Response.ContentType = "text/plain";
        MessageProcessing_API api = new MessageProcessing_API(null);
        string result = api.Process_CreateCheckTelFileForTestAccount(NiceSystemInfo.DEFAULT, XTelList, log);
        Response.Write(result);
    }
}