using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Reflection;
using System.IO;

using NiceApiLibrary_low;


namespace NiceApiLibrary
{
    public class EMail
    {
        public static bool Send(EMailCredentials from, string to, string subject, string htmlBodyNoHeader, out string errorText, LogForEmailSend log, string logId)
        {
            return Send(from, to, null, subject, htmlBodyNoHeader, out errorText, log, logId);
        }
        public static bool Send(EMailCredentials from, string to, string bcc, string subject, string htmlBodyNoHeader, out string errorText, LogForEmailSend log, string logId)
        {
            EMailCredentials cred = from;
            errorText = "";

            if ((to == null) && (bcc != null))
            {
                // no to set but a bcc, so swap
                to = bcc;
                bcc = null;
            }

            MailMessage mail = new MailMessage();
            mail.To.Add(to);
            if (bcc != null)
            {
                mail.Bcc.Add(bcc);
            }
            mail.From = cred.From;
            mail.Subject = subject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = EMail_Data.GetHeaderHtml() + htmlBodyNoHeader + EMail_Data.GetFooterHtml();
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            if ("SendEmailToDataport".IsAppSettingsTrue())
            {
                using (MemoryStream ms = mail.RawMessage())
                {
                    CSC.DataLoggerAccess.Send(ms.ToArray());
                }
                return true;
            }
            else
            {
                SmtpClient client = new SmtpClient();
                client.Credentials = cred.NetCredential;
                client.Port = cred.Port;
                client.Host = cred.Host;
                client.EnableSsl = cred.EnableSsl;
                try
                {
                    client.Send(mail);
                    log.Log.Info(logId + " to " + to);
                    return true;
                }
                catch (Exception ex)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(ex.ToString());
                    sb.AppendLine(ex.Message);
                    Exception ex2 = ex;
                    while (ex2 != null)
                    {
                        sb.AppendLine(ex2.ToString());
                        ex2 = ex2.InnerException;
                    }
                    errorText = sb.ToString();
                    log.Log.Error(logId + " to " + to + " " + errorText);
                }
            }
            return false;
        }

        private static string doFieldReplacement(string htmlBodyNoHeader, Data_AppUserFile user)
        {
            string strActivationDate = (new DateTime(user.CreationDate)).ToString("dd MMMM yyyy");
            htmlBodyNoHeader = htmlBodyNoHeader
                .Replace("{SentCount}", user.SentCount().ToString())
                .Replace("{User}", System.Web.HttpUtility.HtmlEncode(user.UserName))
                .Replace("{RegistrationDate}", System.Web.HttpUtility.HtmlEncode(strActivationDate))
                .Replace("{Name}", System.Web.HttpUtility.HtmlEncode(user.UserName))
                .Replace("{Email}", System.Web.HttpUtility.HtmlEncode(user.Email))
                .Replace("{Password}", System.Web.HttpUtility.HtmlEncode(user.Password))
                .Replace("{APIId}", System.Web.HttpUtility.HtmlEncode(user.ApiGuId))
                .Replace("{Mobile1}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(0)))
                .Replace("{Mobile2}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(1)))
                .Replace("{Mobile3}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(2)))
                .Replace("{Mobile4}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(3)))
                .Replace("{Mobile5}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(4)))
                .Replace("{NiceAPI_Number}", System.Web.HttpUtility.HtmlEncode(System.Configuration.ConfigurationManager.AppSettings["NiceAPI_Number"]));
            return htmlBodyNoHeader;
        }

        public static bool SendGeneralEmail(
            string toEmail, 
            bool sendCopyToAdmin,
            string subject, 
            string htmlBodyNoHeader,
            LogForEmailSend log)
        {
            try
            {
                string error;

                return Send(
                    EMailCredentials.GetSupport(),
                    toEmail, 
                    sendCopyToAdmin ? throw new NotImplementedException("not suitable for publication!") : (string)null,
                    subject, 
                    htmlBodyNoHeader, 
                    out error, 
                    log, 
                    subject);
            }
            catch (SystemException ex)
            {
                log.Log.Error("SystemException: " + ex.Message);
            }
            return false;
        }

        public static bool SendUpgradeRequestOnHighPercent(Data_AppUserFile user, LogForEmailSend log)
        {
            try
            {
                string to = user.Email;
                string subject = "Your account usage";

                Assembly assembly = Assembly.GetExecutingAssembly();
                string htmlBodyNoHeader = new StreamReader(assembly.GetManifestResourceStream("NiceApiLibrary.Embedded.UpgradeEmail.txt")).ReadToEnd();
                htmlBodyNoHeader = doFieldReplacement(htmlBodyNoHeader, user);

                string error;

                return Send(EMailCredentials.GetSupport(), to, subject, htmlBodyNoHeader, out error, log, subject);
            }
            catch (SystemException)
            {
            }
            return false;
        }

        public static bool SendUpgrade032018(Data_AppUserFile user, LogForEmailSend log)
        {
            try
            {
                string to = user.Email;
                string subject = "Hardware upgrade";

                Assembly assembly = Assembly.GetExecutingAssembly();
                string htmlBodyNoHeader = new StreamReader(assembly.GetManifestResourceStream("NiceApiLibrary.Embedded.ServerMove032018EmialText.txt")).ReadToEnd();
                htmlBodyNoHeader = doFieldReplacement(htmlBodyNoHeader, user);

                string error;

                return Send(EMailCredentials.GetSupport(), to, subject, htmlBodyNoHeader, out error, log, "Upgrade032018");
            }
            catch (SystemException)
            {
            }
            return false;
        }

        public static bool SendLittleActivity(Data_AppUserFile user, LogForEmailSend log)
        {
            try
            {
                string to = user.Email;
                string subject = "Using NiceApi.net";

                Assembly assembly = Assembly.GetExecutingAssembly();
                string htmlBodyNoHeader = new StreamReader(assembly.GetManifestResourceStream("NiceApiLibrary.Embedded.LittleActivityEmialText.txt")).ReadToEnd();
                string strActivationDate = (new DateTime(user.CreationDate)).ToString("dd MMMM yyyy");
                htmlBodyNoHeader = htmlBodyNoHeader
                    .Replace("{User}", System.Web.HttpUtility.HtmlEncode(user.UserName))
                    .Replace("{RegistrationDate}", System.Web.HttpUtility.HtmlEncode(strActivationDate))
                    .Replace("{Name}", System.Web.HttpUtility.HtmlEncode(user.UserName))
                    .Replace("{Email}", System.Web.HttpUtility.HtmlEncode(user.Email))
                    .Replace("{Password}", System.Web.HttpUtility.HtmlEncode(user.Password))
                    .Replace("{APIId}", System.Web.HttpUtility.HtmlEncode(user.ApiGuId))
                    .Replace("{Mobile1}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(0)))
                    .Replace("{Mobile2}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(1)))
                    .Replace("{Mobile3}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(2)))
                    .Replace("{Mobile4}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(3)))
                    .Replace("{Mobile5}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(4)));

                string error;

                return Send(EMailCredentials.GetSupport(), to, subject, htmlBodyNoHeader, out error, log, "LittleActivity");
            }
            catch (SystemException)
            {
            }
            return false;
        }

        public static bool SendNoTelRegistered(Data_AppUserFile user, LogForEmailSend log)
        {
            try
            {
                string to = user.Email;
                string subject = "Your register with NiceApi.net";

                Assembly assembly = Assembly.GetExecutingAssembly();
                string htmlBodyNoHeader = new StreamReader(assembly.GetManifestResourceStream("NiceApiLibrary.Embedded.Reg_NoTel_EmailText.txt")).ReadToEnd();
                htmlBodyNoHeader = doFieldReplacement(htmlBodyNoHeader, user);

                string error;

                return Send(EMailCredentials.GetSupport(), to, subject, htmlBodyNoHeader, out error, log, "NoTelRegistered");
            }
            catch (SystemException)
            {
            }
            return false;
        }

        public static bool SendWrongTelRegistered(Data_AppUserFile user, LogForEmailSend log)
        {
            try
            {
                string to = user.Email;
                string subject = "Your registration with with NiceApi.net";

                Assembly assembly = Assembly.GetExecutingAssembly();
                string htmlBodyNoHeader = new StreamReader(assembly.GetManifestResourceStream("NiceApiLibrary.Embedded.Reg_TelWrong_EmailText.txt")).ReadToEnd();
                htmlBodyNoHeader = doFieldReplacement(htmlBodyNoHeader, user);

                string error;

                return Send(EMailCredentials.GetSupport(), to, subject, htmlBodyNoHeader, out error, log, "WrongTelRegistered");
            }
            catch (SystemException)
            {
            }
            return false;
        }

        public static bool SendJustActivated(Data_AppUserFile user, LogForEmailSend log)
        {
            try
            {
                string to = user.Email;
                string subject = "Register with NiceApi.net";

                Assembly assembly = Assembly.GetExecutingAssembly();
                string htmlBodyNoHeader = new StreamReader(assembly.GetManifestResourceStream("NiceApiLibrary.Embedded.JustActivated.txt")).ReadToEnd();
                htmlBodyNoHeader = htmlBodyNoHeader
                    .Replace("{Name}", System.Web.HttpUtility.HtmlEncode(user.UserName))
                    .Replace("{Email}", System.Web.HttpUtility.HtmlEncode(user.Email))
                    .Replace("{Password}", System.Web.HttpUtility.HtmlEncode(user.Password))
                    .Replace("{APIId}", System.Web.HttpUtility.HtmlEncode(user.ApiGuId))
                    .Replace("{Mobile1}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(0)))
                    .Replace("{Mobile2}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(1)))
                    .Replace("{Mobile3}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(2)))
                    .Replace("{Mobile4}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(3)))
                    .Replace("{Mobile5}", System.Web.HttpUtility.HtmlEncode(user.MobileNumberX(4)));

                string error;

                return Send(EMailCredentials.GetSupport(), to, subject, htmlBodyNoHeader, out error, log, "JustActivated");
            }
            catch (SystemException)
            {
            }
            return false;
        }

        public static bool SendRegisterActivation(Data_AppUserFile user, LogForEmailSend log)
        {
            try
            {
                string to = user.Email;
                string subject = "Register with NiceApi.net";
                string htmlBodyNoHeader = EMail_Data.GetRegistrationEmailBody("https://NiceApi.net/Register?ApiGuId=" + Base64_URLEncoding(user.ApiGuId));
                string error;

                bool ret = Send(EMailCredentials.GetSupport(), to, subject, htmlBodyNoHeader, out error, log, "Verify Please");
                return ret;
            }
            catch (SystemException)
            {
            }
            return false;
        }


        public static bool SendAdminNotification(string htmlBodyNoHeader, LogForEmailSend log)
        {
            try
            {
                string to = "edmunozg@gmail.com";
                string subject = "AdminNotification NiceApi.net";
                string error;
                return Send(EMailCredentials.GetSupport(), to, subject, htmlBodyNoHeader, out error, log, "AdminNotification");
            }
            catch (SystemException)
            {
            }
            return false;
        }

        public static string SendTestMail(LogForEmailSend log)
        {
            string to = "edmunozg@gmail.com";
            string subject = "Test Mail";
            string error;

            if (Send(EMailCredentials.GetSupport(), to, subject, "htmlBodyNoHeader", out error, log, "TestMail"))
            {
                return "OK";
            }
            return error;
        }

        static readonly char[] paddingEqual = { '=' };
        public static string Base64_URLEncoding(string sb64)
        {
            string r = sb64.TrimEnd(paddingEqual).Replace('+', '-').Replace('/', '_');
            return r;
        }
        public static string Base64_URLDecoding(string url)
        {
            string r = url.Replace('_', '/').Replace('-', '+');
            switch (url.Length % 4)
            {
                case 2: r += "=="; break;
                case 3: r += "="; break;
            }
            return r;
        }

    }

    public class EMailCredentials
    {
        public System.Net.NetworkCredential NetCredential;
        public int Port;
        public string Host;
        public bool EnableSsl;
        public MailAddress From;

        public static EMailCredentials GetSupport()
        {
            throw new NotImplementedException("not suitable for publication!");
        }
    }
}
