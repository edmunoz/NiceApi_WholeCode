using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiceASP
{

    /// <summary>
    /// Summary description for SessionData
    /// </summary>
    public class SessionData
    {
        public string LoggedOnUserEmail;
        public string LoggedOnUserName;
        public bool LoggonOnUserIsAdmin;
        public NiceApiLibrary_low.NiceSystemInfo LoggedOnUsersNiceSystemInfo;

        public string QuickMessage;
        public bool QuickMessageGood;

        public SessionData()
        {
        }

        public static bool IsLoggedOn(System.Web.SessionState.HttpSessionState Session, bool checkForAdmin)
        {
            SessionData sd = ConstantStrings.GetSessionData(Session);
            if (sd.LoggedOnUserEmail != null)
            {
                if (checkForAdmin)
                {
                    return sd.LoggonOnUserIsAdmin;
                }
                return true;
            }
            return false;
        }

        public static void SessionsSystem_Set(System.Web.SessionState.HttpSessionState Session, NiceApiLibrary_low.NiceSystemInfo systemInfo)
        {
            SessionData sd = ConstantStrings.GetSessionData(Session);
            sd.LoggedOnUsersNiceSystemInfo = systemInfo;
        }

        public static NiceApiLibrary_low.NiceSystemInfo SessionsSystem_Get(System.Web.SessionState.HttpSessionState Session)
        {
            NiceApiLibrary_low.NiceSystemInfo ret = ConstantStrings.GetSessionData(Session).LoggedOnUsersNiceSystemInfo;
            if (ret == null)
            {
                ret = NiceApiLibrary_low.NiceSystemInfo.DEFAULT;
            }
            return ret;
        }

        public static void LoggedOnOrRedirectToRoot(
            System.Web.SessionState.HttpSessionState Session,
            System.Web.HttpResponse Response,
            bool checkForAdmin
            )
        {
            if (!IsLoggedOn(Session, checkForAdmin))
            {
                Response.Redirect("~/default.aspx");
            }
        }

        public static void LoggedOnOrRedirectToLogin(
            System.Web.SessionState.HttpSessionState Session,
            System.Web.HttpResponse Response,
            System.Web.HttpRequest Request)
        {
            if (!IsLoggedOn(Session, false))
            {
                string b64Info = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(Request.RawUrl));
                Response.Redirect("~/Login?" + ConstantStrings.url_Redirect + "=" + b64Info);
            }
        }
    }
}
