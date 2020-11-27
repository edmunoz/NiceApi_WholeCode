using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;
using NiceASP;


public partial class Test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from the Admin User (protected)

        NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);

        if (!IsPostBack)
        {
            IMyLog logInTest = MyLog.GetLogger("Test");
            LogForEmailSend log4Email = new LogForEmailSend(MyLog.GetLogger("Email"));

            string user = null;
            Data_AppUserFile userF = null;

            string sVal = Request.QueryString["Id"];
            switch (sVal)
            {

                case "ShowRecentMessages":
                    Response.ContentType = "text/plain";
                    Response.Write("ShowRecentMessages:\r\n");
                    int Days = 5;
                    if (Request.QueryString["Days"] != null)
                    {
                        Days = Int32.Parse(Request.QueryString["Days"]);
                    }
                    DateTime pivTime = DateTime.UtcNow;
                    pivTime = pivTime.AddDays(-1 * Days);
                    DSSwitch.msgFile00().ForEach(
                        NiceASP.SessionData.SessionsSystem_Get(Session), pivTime, null, Data_Net__00NormalMessage.eLocation.Processed, logInTest, 
                        delegate(Data_Net__00NormalMessage _00) 
                        { 
                            if (!_00.NoCounterUpdate)
                            {
                                Response.Write(_00.DestMobile + "\r\n".PadLeft(40, '*'));
                                Response.Write(_00.UserId + "\r\n");
                                Response.Write(_00.MsgTicks.ToUkTime(true) + "\r\n");
                                if (_00.FailedCounter != 0)
                                {
                                    Response.Write(String.Format("FailedCounter: {0} / {1}\r\n", _00.FailedCounter, _00.DisposeAfterNFailed));
                                }
                                Response.Write(_00.Msg + "\r\n\r\n");
                            }
                        });
                    break;

                case "ShowSentMessages":
                    Response.ContentType = "text/plain";
                    Response.Write("ShowSentMessages:\r\n");
                    user = Request.QueryString["u"];
                    Response.Write(user + ":\r\n");
                    DSSwitch.msgFile00().ForEach(NiceASP.SessionData.SessionsSystem_Get(Session), DateTime.MinValue, user, Data_Net__00NormalMessage.eLocation.Processed, logInTest,
                        delegate(Data_Net__00NormalMessage _00)
                        {
                            Response.Write(_00.DestMobile + "\r\n");
                            Response.Write(_00.MsgTicks.ToUkTime(true) + "\r\n");
                            Response.Write(String.Format("FailedCounter: {0} / {1}\r\n", _00.FailedCounter, _00.DisposeAfterNFailed));
                            Response.Write("NoCounterUpdate: " + _00.NoCounterUpdate.ToString() + "\r\n");
                            Response.Write(_00.Msg + "\r\n\r\n");
                        });
                    break;

                case "ShowUsers":
                    bool filterOut_email_sent_for_verification = false;
                    bool filterOut_verified_welcome_No_sent = false;
                    bool filterOut_verified_welcome_queued = false;
                    bool filterOut_verified_checkingTelNumbers = false;
                    bool filterOut_blocked = false;
                    bool filterOut_free_account = false;
                    bool filterOut_commercial_monthly = false;
                    bool filterOut_commercial_payassent = false;
                    bool filterOut_commercial_systemDuplication = false;
                    switch (Request.QueryString["Filter"])
                    {
                        case "OnlyInteresting":
                            filterOut_email_sent_for_verification = true;
                            filterOut_blocked = true;
                            break;

                        case "OnlyCommercial":
                            filterOut_verified_checkingTelNumbers = true;
                            filterOut_verified_welcome_No_sent = true;
                            filterOut_verified_welcome_queued = true;
                            filterOut_email_sent_for_verification = true;
                            filterOut_blocked = true;
                            filterOut_free_account = true;
                            break;

                    }

                    Data_AppUserFile.SortType sortType = Data_AppUserFile.SortType.State;
                    switch (Request.QueryString["Sort"])
                    {
                        case "Date":
                            sortType = Data_AppUserFile.SortType.Date;
                            break;
                        case "State":
                            sortType = Data_AppUserFile.SortType.State;
                            break;
                        case "Email":
                            sortType = Data_AppUserFile.SortType.Email;
                            break;
                        case "Usage":
                            sortType = Data_AppUserFile.SortType.Usage;
                            break;
                    }

                    showUsers_tableHeader(DSSwitch.appUser().GetInfo());
                    int count = 0;
                    DSSwitch.appUser().RetrieveAll(sortType, 
                        delegate(Data_AppUserFile d) 
                        { 
                            if (count ++ > 1000)
                            {
                                // limit to 1000 results
                                return;
                            }

                            switch (d.AccountStatus)
                            {
                                case Data_AppUserFile.eUserStatus.email_sent_for_verification:
                                    if (!filterOut_email_sent_for_verification)
                                    {
                                        showUsers_tableEntry(d, Data_AppUserFile.EmailSaveChars(d.Email));
                                    }
                                    break;
                                case Data_AppUserFile.eUserStatus.verified_welcome_No_sent:
                                    if (!filterOut_verified_welcome_No_sent)
                                    {
                                        showUsers_tableEntry(d, Data_AppUserFile.EmailSaveChars(d.Email));
                                    }
                                    break;
                                case Data_AppUserFile.eUserStatus.verified_welcome_queued:
                                    if (!filterOut_verified_welcome_queued)
                                    {
                                        showUsers_tableEntry(d, Data_AppUserFile.EmailSaveChars(d.Email));
                                    }
                                    break;
                                case Data_AppUserFile.eUserStatus.verified_checkingTelNumbers:
                                    if (!filterOut_verified_checkingTelNumbers)
                                    {
                                        showUsers_tableEntry(d, Data_AppUserFile.EmailSaveChars(d.Email));
                                    }
                                    break;
                                case Data_AppUserFile.eUserStatus.blocked:
                                    if (!filterOut_blocked)
                                    {
                                        showUsers_tableEntry(d, Data_AppUserFile.EmailSaveChars(d.Email));
                                    }
                                    break;
                                case Data_AppUserFile.eUserStatus.free_account:
                                    if (!filterOut_free_account)
                                    {
                                        showUsers_tableEntry(d, Data_AppUserFile.EmailSaveChars(d.Email));
                                    }
                                    break;
                                case Data_AppUserFile.eUserStatus.commercial_monthly:
                                case Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice:
                                    if (!filterOut_commercial_monthly)
                                    {
                                        showUsers_tableEntry(d, Data_AppUserFile.EmailSaveChars(d.Email));
                                    }
                                    break;
                                case Data_AppUserFile.eUserStatus.commercial_payassent:
                                    if (!filterOut_commercial_payassent)
                                    {
                                        showUsers_tableEntry(d, Data_AppUserFile.EmailSaveChars(d.Email));
                                    }
                                    break;
                                case Data_AppUserFile.eUserStatus.commercial_systemDuplication:
                                    if (!filterOut_commercial_systemDuplication)
                                    {
                                        showUsers_tableEntry(d, Data_AppUserFile.EmailSaveChars(d.Email));
                                    }
                                    break;

                            }
                        }, 
                        logInTest);
                    showUsers_tableFooter();
                    break;

                //case "CopyAppUsersToDB":
                //    int count = 0;
                //    DSSwitch.appUser().RetrieveAll(
                //        Data_AppUserFile.SortType.State,
                //        delegate(Data_AppUserFile d)
                //        {
                //            bool fileArleadyUsed;
                //            DSSwitch.appUser_DB().StoreNew(d, out fileArleadyUsed, logInTest);
                //            count++;
                //        },
                //        logInTest);
                //    Response.ContentType = "text/plain";
                //    Response.Write("Done " + count.ToString());
                //    break;
                        

                case "SendWelcomeSMS":
                    Response.ContentType = "text/plain";
                    user = Request.QueryString["u"];
                    userF = DSSwitch.appUser().RetrieveOne(user, logInTest);
                    Response.ContentType = "text/plain";
                    foreach (string tel1 in userF.MobileNumberArray())
                    {
                        string additionalInfo;
                        Response.Write(tel1 + " " +
                            (new APIActualSending(NiceASP.SessionData.SessionsSystem_Get(Session)).SendWhatsApp(
                            userF.ApiGuId,
                            tel1,
                            "Welcome to NiceApi.net\r\n",
                            true,
                            logInTest,
                            out additionalInfo)));
                        Response.Write(" " + additionalInfo);
                        System.Threading.Thread.Sleep(200);
                        Response.Write(" slept\r\n");
                    }
                    break;

                case "SendJustActivated":
                    user = Request.QueryString["u"];
                    userF = DSSwitch.appUser().RetrieveOne(user, logInTest);
                    Response.ContentType = "text/plain";
                    Response.Write(sVal + (
                        EMail.SendJustActivated(userF, log4Email) ?
                            " ok" : " failed"));
                    break;

                case "ResendVerifyMail":
                    user = Request.QueryString["u"];
                    userF = DSSwitch.appUser().RetrieveOne(user, logInTest);
                    Response.ContentType = "text/plain";
                    Response.Write(sVal + (
                        EMail.SendRegisterActivation(userF, log4Email) ?
                            " ok" : " failed"));
                    break;

                case "Queue":
                    doQueue(logInTest);
                    break;

                case "Log":
                    logInTest.Info("A test msg from Test.Log");
                    Response.ContentType = "text/plain";
                    Response.Write("Log done");
                    break;

                case "null":
                    userF = null;
                    user = userF.UserName; // simulate bad code
                    break;

                default:
                    throw new ArgumentNullException("only a test");
            }
        }
    }

    private void doQueue_old(IMyLog log)
    {
        // show queued files
        responseWriteHomeLink();

        int queue = DSSwitch.msgFile00().GetNoOfQueuedItems(NiceASP.SessionData.SessionsSystem_Get(Session), log);

        Response.Write(String.Format("Length : {0}<br />", queue));
    }

    private void doQueue(IMyLog log)
    {
        responseWriteHomeLink();
        List<ASPTrayBase> test = MessageProcessing_TrayTo.GetFilesToSendToTray_ConsiderPriority(NiceASP.SessionData.SessionsSystem_Get(Session), -1, log);
        int count = 0;
        foreach (var one in test)
        {
            string line = string.Format("{0} {1} {2}",
                ++count,
                one.GetNiceStatus(),
                "<br>");
            Response.Write(line);
        }
    }

    private void responseWriteHomeLink()
    {
        if (FolderNames.IsDevMachine())
        {
            Response.Write("<a href=\"./\">Home</a><br />");
        }
        else
        {
            Response.Write("<a href=\"/\">Home</a><br />");
        }
    }

    private void showUsers_tableHeader(string infoText)
    {
        responseWriteHomeLink();
        Response.Write("<br />" + infoText + "<br />");
        Response.Write("<table><tr>");
        Response.Write("<th>Email</th>");
        Response.Write("<th>CCode</th>");
        Response.Write("<th>#</th>");
        Response.Write("<th>Date</th>");
        Response.Write("<th>%</th>");
        Response.Write("<th></th>");
        Response.Write("<th>State</th>");
        Response.Write("<th>Edit</th>");
        Response.Write("<th>space</th>");
        //Response.Write("<th>SMS_Wel</th>");
        //Response.Write("<th>Email_OnNow</th>");
        //Response.Write("<th>SMS_Bug</th>");
        //Response.Write("<th>Email_NoTel</th>");
        //Response.Write("<th>Email_RegBug</th>");
        Response.Write("</tr>");
    }
    private void showUsers_tableFooter()
    {
        Response.Write("</table>");
    }
    private void showUsers_tableEntry(Data_AppUserFile u1, string emailFile)
    {
        Response.Write("<tr>");
        Response.Write("<td>" + HttpUtility.HtmlEncode(u1.Email) + "</td>");

        Response.Write("<td>" + HttpUtility.HtmlEncode(CountryListLoader.Lookup(u1.MainTelNo())) + "</td>");
        Response.Write("<td>" + u1.MobileNumbersCount().ToString() + "</td>");
        Response.Write("<td>" + new DateTime(u1.CreationDate).ToString("dd/MM/yyyy") + "</td>");
        Response.Write("<td>" + u1.UsedInPercent().ToString() + "</td>");
        if (u1.QueuedCount() > 0)
        {
            Response.Write("<td>" + u1.QueuedCount().ToString() + "</td>");
        }
        else
        {
            Response.Write("<td></td>");
        }

        if (
            (u1.AccountStatus == Data_AppUserFile.eUserStatus.free_account) ||
            (u1.AccountStatus == Data_AppUserFile.eUserStatus.commercial_monthly) ||
            (u1.AccountStatus == Data_AppUserFile.eUserStatus.commercial_payassent) ||
            (u1.AccountStatus == Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice) ||
            (u1.AccountStatus == Data_AppUserFile.eUserStatus.commercial_systemDuplication))
        {
            // commercial or acive free
            Response.Write("<td><a href=\"Test?Id=ShowSentMessages&u=" + emailFile + "\" target=\"_blank\">" + u1.AccountStatus.ToString() + "</a></td>");
        }
        else
        {
            // not active yet
            Response.Write("<td>" + HttpUtility.HtmlEncode(u1.AccountStatus.ToString()) + "</td>");
        }
        
        if (
            (u1.AccountStatus == Data_AppUserFile.eUserStatus.commercial_monthly) ||
            (u1.AccountStatus == Data_AppUserFile.eUserStatus.commercial_payassent) ||
            (u1.AccountStatus == Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice) ||
            (u1.AccountStatus == Data_AppUserFile.eUserStatus.commercial_systemDuplication))
        {
            string edit = "edit " + u1.GetCheckerBase(true).Info(u1);
            Response.Write("<td><a href=\"DataAll?user=" + emailFile + "\" target=\"_blank\">" + edit + "</a></td>");
        }
        else
        {
            Response.Write("<td><a href=\"DataAll?user=" + emailFile + "\" target=\"_blank\">" + "edit" + "</a></td>");
        }

        Response.Write("<td></td>");
        //Response.Write("<td><a href=\"Test?Id=SendWelcomeSMS&u=" + emailFile + "\" target=\"_blank\">" + "WA_Wel" + "</a></td>");
        //Response.Write("<td><a href=\"Test?Id=SendJustActivated&u=" + emailFile + "\" target=\"_blank\">" + "E_OnNow" + "</a></td>");
        Response.Write("</tr>");
    }
}