using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Reflection;

using NiceASP;
using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;

public partial class ItemX : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from the Admin User (protected)
        try
        {
            string id = Request["id"];
            IMyLog logItemX = MyLog.GetLogger("ItemX");

            switch (id)
            {
                case "GetAllTelNumbers": // as used by the fone
                    NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);

                    Response.ContentType = "text/plain";
                    Response.Write("GetAllTelNumbers:\r\n");
                    Response.Write(DateTime.UtcNow.ToString() + "\r\n");
                    {
                        int telCounter = 0;
                        int userCounter = 0;
                        List<MobileNoHandlerWithUserName> commercialUsersToConfirm = new List<MobileNoHandlerWithUserName>();

                        DSSwitch.appUser().RetrieveAll(
                            Data_AppUserFile.SortType.Date,
                            delegate(Data_AppUserFile d)
                            {
                                if (    (d.AccountStatus == Data_AppUserFile.eUserStatus.verified_welcome_No_sent)
                                    ||  (d.AccountStatus == Data_AppUserFile.eUserStatus.verified_welcome_queued)
                                    ||  (d.AccountStatus == Data_AppUserFile.eUserStatus.verified_checkingTelNumbers))
                                {
                                    userCounter++;
                                    Response.Write("User: " + d.Email + "\r\n");
                                    foreach (string m1 in d.MobileNumbers_AllConfirmed__.MobileNumberArray)
                                    {
                                        telCounter++;
                                        Response.Write(m1 + "\r\n");
                                    }
                                    foreach (string m1 in d.MobileNumbers_AllUnConfirmed__.MobileNumberArray)
                                    {
                                        telCounter++;
                                        Response.Write(m1 + "\r\n");
                                    }
                                }
                                else if (
                                    (d.AccountStatus == Data_AppUserFile.eUserStatus.commercial_monthly) ||
                                    (d.AccountStatus == Data_AppUserFile.eUserStatus.commercial_payassent))
                                {
                                    if (d.AddNumber_ActivateOnSyncRequest)
                                    {
                                        userCounter++;
                                        Response.Write("User: " + d.Email + "\r\n");
                                        MobileNoHandlerWithUserName conf = new MobileNoHandlerWithUserName(d.Email);
                                        foreach (string tel1 in d.MobileNumbers_AllUnConfirmed__.MobileNumberArray)
                                        {
                                            telCounter++;
                                            Response.Write(tel1 + "\r\n");
                                            conf.Handler.Add(tel1);
                                        }
                                        commercialUsersToConfirm.Add(conf);
                                    }
                                }
                            },
                            logItemX);
                        Response.Write("Sumary: Active user: " + userCounter.ToString() + " Tel: " + telCounter.ToString() + "\r\n");

                    }
                    break;

                case "GetTelNumbersBlockedUsers":
                    Response.ContentType = "text/plain";
                    Response.Write("GetTelNumbersBlockedUsers:\r\n");
                    Response.Write(DateTime.UtcNow.ToString() + "\r\n");
                    {
                        int telCounter = 0;
                        int userCounter = 0;

                        DSSwitch.appUser().RetrieveAll(
                            Data_AppUserFile.SortType.Date,
                            delegate(Data_AppUserFile d)
                            {
                                if (d.AccountStatus == Data_AppUserFile.eUserStatus.blocked)
                                {
                                    userCounter++;
                                    Response.Write("User: " + d.Email + "\r\n");
                                    foreach (string m1 in d.MobileNumberArray())
                                    {
                                        telCounter++;
                                        Response.Write(m1 + "\r\n");
                                    }
                                }
                            },
                            logItemX);
                        Response.Write("Sumary: Active user: " + userCounter.ToString() + " Tel: " + telCounter.ToString());
                    }
                    break;

                case "GetTelNumbers":
                    // no check as this comes from the TrayApp - NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);
                    Response.ContentType = "text/plain";
                    Response.Write("GetTelNumbers:\r\n");
                    Response.Write(DateTime.UtcNow.ToString() + "\r\n");
                    {
                        int telCounter = 0;
                        int userCounter = 0;

                        DSSwitch.appUser().RetrieveAll(
                            Data_AppUserFile.SortType.Date,
                            delegate(Data_AppUserFile d)
                            {
                                if (d.IsAccountActive("Welcome"))
                                {
                                    userCounter++;
                                    Response.Write("User: " + d.Email + "\r\n");
                                    foreach (string m1 in d.MobileNumberArray())
                                    {
                                        telCounter++;
                                        Response.Write(m1 + "\r\n");
                                    }
                                }
                            },
                            logItemX);
                        Response.Write("Sumary: Active user: " + userCounter.ToString() + " Tel: " + telCounter.ToString());
                    }
                    break;

                case "Home":
                    Response.Redirect("~/");
                    break;
                case "LibVer":
                    NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);

                    Assembly a = Assembly.GetAssembly(typeof(IMyLog));
                    Response.ContentType = "text/plain";
                    a.WriteAssemblyVersion(Response.Output);
                    break;

                case "DSSwitch":
                    NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);
                    Response.ContentType = "text/plain";
                    Response.Write("appUser:   " + DSSwitch.appUser().GetInfo() + "\r\n");
                    Response.Write("appWallet: " + DSSwitch.appWallet().GetInfo() + "\r\n");
                    Response.Write("msgFile00: " + DSSwitch.msgFile00().GetInfo(NiceASP.SessionData.SessionsSystem_Get(Session)) + "\r\n");
                    Response.Write("msgFile02: " + DSSwitch.msgFile02().GetInfo(NiceASP.SessionData.SessionsSystem_Get(Session)) + "\r\n");
                    Response.Write("msgFile04: " + DSSwitch.msgFile04().GetInfo(NiceASP.SessionData.SessionsSystem_Get(Session)) + "\r\n");
                    Response.Write(DSSwitch.GetMaintenanceLog() + "\r\n");
                    break;

                case "SetSubSystem":
                    NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);
                    string val = Request["val"];
                    var sub = DSSwitch.full().GetSystems(false).FirstOrDefault(_ => _.Name == val);
                    if (sub == null)
                    {
                        Response.ContentType = "text/plain";
                        Response.Write("No such SubSystem " + val + "\r\n");
                    }
                    else
                    {
                        SessionData.SessionsSystem_Set(Session, sub);
                        Response.Redirect("~/Admin.aspx");
                    }
                    break;

                case "ShowSubSystem":
                    NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);
                    Response.ContentType = "text/plain";
                    DSSwitch.full().GetSystems(false).ForEach(_ => Response.Write(string.Format("{0}, {1}, {2}\r\n", _.Name, _.APIId, _.Default)));
                    Response.Write("\r\n");
                    var cur = SessionData.SessionsSystem_Get(Session);
                    Response.Write(string.Format("currently on\r\n{0}, {1}, {2}\r\n", cur.Name, cur.APIId, cur.Default));
                    break;

                case "Screen":
                    NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);
                    Response.ContentType = "image/png";
                    byte[] baImg;
                    using (DataFile_ScreenShot ss = new DataFile_ScreenShot(NiceASP.SessionData.SessionsSystem_Get(Session), DataFile_Base.OpenType.ReadOnly_CreateIfNotThere))
                    {
                        MemoryStream ms = new MemoryStream();
                        ss.imgScreen.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        baImg = ms.ToArray();
                    }
                    Response.OutputStream.Write(baImg, 0, baImg.Length);
                    Response.OutputStream.Flush(); 
                    break;

                case "AllValues":
                    NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);
                    Response.ContentType = "text/plain";
                    int count = 0;
                    foreach (string k1 in Request.ServerVariables)
                    {
                        Response.Write(count++.ToString() + ":" + k1 + " = " + Request.ServerVariables[k1] + "\r\n");
                    }
                    using (StreamReader sr = new StreamReader(Request.InputStream))
                    {
                        Response.Write(sr.ReadToEnd());
                        Response.Write("\r\n");
                    }
                    Response.Write("Done a");
                    break;

                case "TestX":
                    NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);

                    Response.ContentType = "text/plain";
                    string pathX = Request.ServerVariables["APPL_PHYSICAL_PATH"];

                    Response.Write("in1b: " + pathX + "\n");
                    pathX = Directory.GetParent(pathX).FullName;
                    foreach (string p1 in Directory.GetFiles(pathX))
                    {
                        Response.Write(p1 + "\n");
                    }

                    Response.Write("in2: " + pathX + "\n");
                    pathX = Directory.GetParent(pathX).FullName;
                    foreach (string p1 in Directory.GetFiles(pathX))
                    {
                        Response.Write(p1 + "\n");
                    }

                    Response.Write("in3: " + pathX + "\n");
                    foreach (string p1 in Directory.GetFiles(pathX))
                    {
                        Response.Write(p1 + "\n");
                    }

                    throw new NotImplementedException("end");

                case "Dir":
                    NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);

                    FolderNames.CreateFoldersForAsp(NiceASP.SessionData.SessionsSystem_Get(Session));
                    Response.ContentType = "text/plain";
                    Response.Write("Done Dir");
                    break;

                default:
                    NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);
                    throw new ArgumentException();
                    //break;

            }
        }
        catch (Exception se)
        {
            Response.ContentType = "text/plain";
            Response.Write("ups");
            Response.Write(se.ToString());
            Response.Write(se.Message);
        }
    }
}

