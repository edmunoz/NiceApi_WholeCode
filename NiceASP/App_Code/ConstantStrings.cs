using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;

using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;

namespace NiceASP
{

    /// <summary>
    /// Summary description for ConstantStrings
    /// </summary>
    public static class ConstantStrings
    {
        private static string ses_LoggedOn = "LoggedOn";
        public static string url_Redirect = "Redirect";

        /* Session Data */
        public static SessionData GetSessionData(HttpSessionState s)
        {
            SessionData se = (SessionData)s[ses_LoggedOn];
            if (se == null)
            {
                se = new SessionData();
                s[ses_LoggedOn] = se;
            }
            return se;
        }

        ///* LoggedOnNavigator */
        //public static void SetLoggedOnNavigator(HttpSessionState s, string v)
        //{
        //    s[ses_LoggedOnNavigator] = v;
        //}
        //public static string GetLoggedOnNavigator(HttpSessionState s)
        //{
        //    return (string)s[ses_LoggedOnNavigator];
        //}


        //public static void SetMyMessage(HttpSessionState s, string msg, bool good)
        //{
        //    s[ses_myMessage] = msg;
        //    s[ses_myMessageGood] = good;
        //}
        //public static void GetMyMessage(HttpSessionState s, out string msg, out bool good)
        //{
        //    good = false;
        //    msg = (string)s[ses_myMessage];
        //    if (s[ses_myMessageGood] != null)
        //    {
        //        good = (bool)s[ses_myMessageGood];
        //    }
        //    s[ses_myMessage] = null;
        //}
    }
}
