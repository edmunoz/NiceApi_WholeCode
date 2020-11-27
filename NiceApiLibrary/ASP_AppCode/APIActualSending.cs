using NiceApiLibrary_low;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceApiLibrary.ASP_AppCode
{

    /// <summary>
    /// Summary description for APIActualSending
    /// </summary>
    public class APIActualSending
    {
        private string m_RequId;
        private string[] m_RequTelList;
        private string m_Message;
        private Data_AppUserFile m_User;
        private DateTime m_TimeNow;
        private NiceSystemInfo niceSystem;

        public APIActualSending(NiceSystemInfo niceSystem)
        {
            this.niceSystem = niceSystem;
        }

        public static string fakeRequest(string XAPIId, string XAPIMobile, string Message)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("POST /API HTTP/1.1<br>");
            sb.AppendLine("Host: NiceApi.net<br>");
            sb.AppendLine("X-APIId: " + XAPIId + "<br>");
            sb.AppendLine("X-APIMobile: " + XAPIMobile + "<br>");
            sb.AppendLine("Content-Type: application/x-www-form-urlencoded<br>");
            sb.AppendLine("Content-Length: " + Message.Length.ToString() + "<br>");
            sb.AppendLine("<br>");
            sb.AppendLine(Message);
            return sb.ToString();
        }

        public static string fakeResponse(string response)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("HTTP/1.1 200 OK<br>");
            sb.AppendLine("Cache-Control: private<br>");
            sb.AppendLine("Content-Type: text/plain; charset=utf-8<br>");
            sb.AppendLine("Content-Length: " + response.Length.ToString() + "<br>");
            sb.AppendLine("<br>");
            sb.AppendLine(response);
            return sb.ToString();
        }

        public string SendWhatsApp(string XAPIId, string XAPIMobile, string Message, IMyLog log)
        {
            string fileName;
            return SendWhatsApp(XAPIId, XAPIMobile, Message, false, log, out fileName);
        }

        public string SendWhatsApp(string XAPIId, string XAPIMobile, string Message, bool isWelcomeMessage, IMyLog log, out string fileName)
        {
            fileName = "";
            try
            {
                m_TimeNow = DateTime.UtcNow;
                // 1) extract post dat
                // 2) load user file and check guid
                // 3) check time delay
                // 4) verify mobile number
                // 5) createMessageFile
                // 6) update user counters
                extractPostData(XAPIId, XAPIMobile, Message);
                bool sendFooter = true;
                loadUserFileAndCheckGUID(delegate ()
                {
                    m_User.CommitOrThrow_Send(m_RequTelList, isWelcomeMessage, m_Message.Length, ref m_User.AccountStatus, out sendFooter);
                }, log);

                bool noCounterUpdate = false;
                if (Message.StartsWith("__NoSend"))
                {
                    noCounterUpdate = true;
                }
                else if (Message == "Welcome\r\n")
                {
                    noCounterUpdate = true;
                }

                int telNoId = 0;
                foreach (string tel1 in m_RequTelList)
                {
                    fileName +=
                    createMessageFile(
                        telNoId,
                        m_TimeNow,
                        log,
                        noCounterUpdate,
                        sendFooter,
                        this.m_User.DeleteOnFailed ?
                        1 : -1) + ", ";
                    // move on
                    telNoId++;
                }

                if (telNoId == 1)
                {
                    return "queued";
                }
                else
                {
                    return "queued x" + telNoId.ToString();
                }
            }
            catch (DataUnavailableException due)
            {
                return due.Message;
            }
            catch (ArgumentException ae)
            {
                return ae.Message;
            }
        }

        private void extractPostData(string XAPIId, string XAPIMobile, string Message)
        {
            m_RequId = XAPIId;
            if (string.IsNullOrEmpty(m_RequId))
            {
                throw new ArgumentException("X-APIId missing");
            }

            if (string.IsNullOrEmpty(XAPIMobile))
            {
                throw new ArgumentException("X-APIMobile missing");
            }

            m_RequTelList = XAPIMobile.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < m_RequTelList.Length; i++)
            {
                m_RequTelList[i] = "+" + m_RequTelList[i];
            }

            m_Message = Message.Replace(":SE", ":  SE").Replace(": SE", ":  SE");
            if (string.IsNullOrEmpty(m_Message))
            {
                throw new ArgumentException("No POST data");
            }
        }

        private void loadUserFileAndCheckGUID(Action action, IMyLog log)
        {
            string email = Data_AppUserFile.API_IdToEmail(m_RequId);
            if (email == null)
            {
                throw new ArgumentException("X-APIId unknown");
            }
            DSSwitch.appUser().Update_General(email, delegate (Data_AppUserFile user, Object args)
            {
                m_User = user;
                if (m_User == null)
                {
                    throw new ArgumentException("X-APIId unknown");
                }
                if (!m_User.IsAccountActive(m_Message))
                {
                    throw new ArgumentException("Account not active (1). " + m_User.AccountStatusExplained());
                }
                if (m_User.ApiGuId != m_RequId)
                {
                    throw new ArgumentException("X-APIId unknown");
                }
                action();
            }, null, null, log);
        }

        private string createMessageFile(int telId, DateTime utcNow, IMyLog log, bool noCounterUpdate, bool sendFooter, Int32 MaxFailBeforeDispose)
        {
            Data_Net__00NormalMessage msg = new Data_Net__00NormalMessage(
                Data_AppUserFile.EmailSaveChars(m_User.Email),
                "zapi_" + m_RequTelList[telId],
                utcNow.Ticks + (Int64)telId,
                sendFooter ?
                    m_Message + "\r\nSent via NiceApi.net\r\n" :
                    m_Message + "\r\n",
                    0,
                    MaxFailBeforeDispose,
                    noCounterUpdate);
            DSSwitch.msgFile00().Store(niceSystem, msg, Data_Net__00NormalMessage.eLocation.Queued, log);
            return msg.GetFileName();
        }
    }
}

