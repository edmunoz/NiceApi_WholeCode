using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    // not used anymore
    class MessageProcessing_APIMsgQueue
    {
        private string m_RequId;
        private string[] m_RequTelList;
        private string m_Message;
        private Data_AppUserFile m_User;
        private DateTime m_TimeNow;


        //public string Queue(string XAPIId, string XAPIMobile, string Message, bool isWelcomeMessage, IMyLog log, out string fileName)
        //{
        //    fileName = "";
        //    try
        //    {
        //        m_TimeNow = DateTime.UtcNow;
        //        // 1) extract post dat
        //        // 2) load user file and check guid
        //        // 3) check time delay
        //        // 4) verify mobile number
        //        // 5) createMessageFile
        //        // 6) update user counters
        //        extractPostData(XAPIId, XAPIMobile, Message);
        //        bool sendFooter = true;
        //        loadUserFileAndCheckGUID(delegate ()
        //        {
        //            m_User.CommitOrThrow_Send(m_RequTelList, isWelcomeMessage, m_Message.Length, ref m_User.AccountStatus, out sendFooter);
        //        }, log);

        //        bool noCounterUpdate = false;
        //        if (Message.StartsWith("__NoSend"))
        //        {
        //            noCounterUpdate = true;
        //        }
        //        else if (Message == "Welcome\r\n")
        //        {
        //            noCounterUpdate = true;
        //        }

        //        int telNoId = 0;
        //        foreach (string tel1 in m_RequTelList)
        //        {
        //            fileName +=
        //            createMessageFile(
        //                telNoId,
        //                m_TimeNow,
        //                log,
        //                noCounterUpdate,
        //                sendFooter,
        //                this.m_User.DeleteOnFailed ?
        //                1 : -1) + ", ";
        //            // move on
        //            telNoId++;
        //        }

        //        if (telNoId == 1)
        //        {
        //            return "queued";
        //        }
        //        else
        //        {
        //            return "queued x" + telNoId.ToString();
        //        }
        //    }
        //    catch (DataUnavailableException due)
        //    {
        //        return due.Message;
        //    }
        //    catch (ArgumentException ae)
        //    {
        //        return ae.Message;
        //    }
        //}

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


    }
}
