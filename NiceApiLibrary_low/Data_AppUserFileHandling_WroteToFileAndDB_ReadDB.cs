using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NiceApiLibrary_low
{
    public class Data_AppUserFileHandling_SQL : IData_AppUserFileHandling
    {
        public readonly String TABLE_NAME = "[dbo].[AppUserFile]";
        private readonly SQLDBConfig.DBToUse Db = SQLDBConfig.DBToUse.TestDB;

        public String GetInfo()
        {
            return SQLDBConfig.GetDBString(Db, false);
        }
        public void UpdateAll(
            string email,
            string userName,
            string password,
            string creationIp,
            string telNumbers,
            Data_AppUserFile.eUserStatus status,
            Int32 minDelayInSeconds,
            Int64 noOfQueuedMessages,
            Int64 noOfSentMessages,
            Int64 noOfBytesSent,
            Int64 remainingMsg,
            IMyLog log)
        {
            SqlCmdBuilder cmd = new SqlCmdBuilder(TABLE_NAME);
            cmd.Add("[UserName]", userName.Quote());
            cmd.Add("[Password]", password.Quote());
            cmd.Add("[CreationIp]", creationIp.Quote());
            cmd.Add("[MobileNumbers]", telNumbers.Quote());
            cmd.Add("[AccountStatus]", status.ToIntString());
            cmd.Add("[MinDelayInSeconds]", minDelayInSeconds.ToString());
            cmd.Add("[NoOfQueuedMessages]", noOfQueuedMessages.ToString());
            cmd.Add("[NoOfSentMessages]", noOfSentMessages.ToString());
            cmd.Add("[NoOfBytesSent]", noOfBytesSent.ToString());
            cmd.Add("[RemainingMessages]", remainingMsg.ToString());

            string strCmd = cmd.GetSql_Update(string.Format("WHERE [Email] like '{0}'", email));
            using (SqlDisposable s = new SqlDisposable(Db, strCmd))
            {
                if (s.Reader.RecordsAffected != 1)
                {
                    log.Error("UpdateAll: *** unexpected RecordsAffected != 1");
                }
            }
        }

        public void UpdateStatus(string email, Data_AppUserFile.eUserStatus newStatus, IMyLog log)
        {
            SqlCmdBuilder cmd = new SqlCmdBuilder(TABLE_NAME);
            cmd.Add("[AccountStatus]", newStatus.ToIntString());

            string strCmd = cmd.GetSql_Update(string.Format("WHERE [Email] like '{0}'", email));
            using (SqlDisposable s = new SqlDisposable(Db, strCmd))
            {
                if (s.Reader.RecordsAffected != 1)
                {
                    log.Error("UpdateAll: *** unexpected RecordsAffected != 1");
                }
            }
        }

        public void UpdateCounters_Processed(string email, int noOfBytes, IMyLog log)
        {
            SqlCmdBuilder cmd = new SqlCmdBuilder(TABLE_NAME);
            cmd.Add("[LastMsgProcessedUtc]", SqlExtensions.SqlDateUtcNow().ToString());
            cmd.AddDec("[NoOfQueuedMessages]");
            cmd.AddInc("[NoOfBytesSent]", noOfBytes);
            cmd.AddInc("[NoOfSentMessages]");

            cmd.Add("[AccountStatus]", String.Format("CASE WHEN {0}={1} THEN {2} ELSE {0} END",
                "[AccountStatus]",
                Data_AppUserFile.eUserStatus.verified_welcome_queued.ToIntString(),
                Data_AppUserFile.eUserStatus.active.ToIntString()));

            string strCmd = cmd.GetSql_Update(string.Format("WHERE [Email] like '{0}'", email));

            using (SqlDisposable s = new SqlDisposable(Db, strCmd))
            {
                if (s.Reader.RecordsAffected != 1)
                {
                    log.Error("StoreNew: *** unexpected RecordsAffected != 1");
                }
            }
        }

        public void UpdateCounters_Queued(string email, long timeNow, int howMany, IMyLog log)
        {
            SqlCmdBuilder cmd = new SqlCmdBuilder(TABLE_NAME);
            cmd.Add("[LastMsgQueuedUtc]", SqlExtensions.SqlDateUtcNow().ToString());
            cmd.AddInc("[NoOfQueuedMessages]", howMany);
            cmd.AddDec("[RemainingMessages]", howMany);
            cmd.Add("[AccountStatus]", String.Format("CASE WHEN {0}={1} THEN {2} ELSE {0} END", 
                "[AccountStatus]",
                Data_AppUserFile.eUserStatus.verified_welcome_No_sent.ToIntString(),
                Data_AppUserFile.eUserStatus.verified_welcome_queued.ToIntString()));

            string strCmd = cmd.GetSql_Update(string.Format("WHERE [Email] like '{0}'", email));

            using (SqlDisposable s = new SqlDisposable(Db, strCmd))
            {
                if (s.Reader.RecordsAffected != 1)
                {
                    log.Error("StoreNew: *** unexpected RecordsAffected != 1");
                }
            }
        }

        public bool HasAccount(string email, IMyLog log)
        {
            String cmd = String.Format("SELECT * FROM {0} WHERE {1} like '{2}'",
                TABLE_NAME,
                "[Email]",
                email);
            using (SqlDisposable s = new SqlDisposable(Db, cmd))
            {
                return s.Reader.HasRows;
            }
        }

        public void StoreNew(Data_AppUserFile data, out bool fileArleadyUsed, IMyLog log)
        {
            fileArleadyUsed = false;

            if (HasAccount(data.Email, log))
            {
                fileArleadyUsed = true;
                return;
            }

            SqlCmdBuilder cmd = new SqlCmdBuilder(TABLE_NAME);
            cmd.Add("[UserName]", data.UserName.Quote());
            cmd.Add("[MobileNumbers]", data.MoblieAllNumbers.Quote());
            cmd.Add("[Email]", data.Email.Quote());
            cmd.Add("[Password]", data.Password.Quote());
            cmd.Add("[CreationDateUtc]", data.CreationDate.SqlDate().ToString());
            cmd.Add("[CreationIp]", data.CreationIp.Quote());
            cmd.Add("[ApiGuId]", data.ApiGuId.Quote());
            cmd.Add("[NoOfQueuedMessages]", data.NoOfQueuedMessages.ToString());
            cmd.Add("[NoOfSentMessages]", data.NoOfSentMessages.ToString());
            cmd.Add("[NoOfBytesSent]", data.NoOfBytesSent.ToString());
            cmd.Add("[RemainingMessages]", data.RemainingMessages.ToString());
            cmd.Add("[MinDelayInSeconds]", data.MinDelayInSeconds.ToString());
            cmd.Add("[AccountStatus]", data.AccountStatus.ToIntString());
            string strCmd = cmd.GetSql_InsertSelect();
            //System.Diagnostics.Debug.WriteLine(strCmd);//mg remove

            using (SqlDisposable s = new SqlDisposable(Db, strCmd))
            {
                if (s.Reader.RecordsAffected != 1)
                {
                    log.Error("StoreNew: *** unexpected RecordsAffected != 1");
                }
            }
        }

        private Data_AppUserFile readOneRecord(System.Data.SqlClient.SqlDataReader r)
        {
            Data_AppUserFile ret = Data_AppUserFile.CreateBlank();
            ret.UserName = (string)r["UserName"];
            ret.NoSpam = true;
            ret.MoblieAllNumbers = (string)r["MobileNumbers"];
            ret.Email = (string)r["Email"];
            ret.Password = (string)r["Password"];
            ret.CreationDate = r.SqlDateRead("CreationDateUtc");
            ret.CreationIp = (string)r["CreationIp"];
            ret.ApiGuId = (string)r["ApiGuId"];
            ret.NoOfQueuedMessages = (int)r["NoOfQueuedMessages"];
            ret.NoOfSentMessages = (int)r["NoOfSentMessages"];
            ret.NoOfBytesSent = (int)r["NoOfBytesSent"];
            ret.RemainingMessages = (int)r["RemainingMessages"];
            ret.LastMsgQueued = r.SqlDateRead("LastMsgQueuedUtc");
            //ret.LastMsgProcessed = r.SqlDateRead(["LastMsgProcessedUtc"]);
            ret.MinDelayInSeconds = (int)r["MinDelayInSeconds"];
            ret.AccountStatus = (Data_AppUserFile.eUserStatus)r["AccountStatus"];
            return ret;
        }

        public Data_AppUserFile RetrieveOne(string email, IMyLog log)
        {
            Data_AppUserFile ret = null;
            var strCmd = String.Format("SELECT * FROM {0} WHERE [Email] like '{1}'", TABLE_NAME, email);
            using (SqlDisposable s = new SqlDisposable(Db, strCmd))
            {
                if (s.Reader.Read())
                {
                    ret = readOneRecord(s.Reader);
                }
            }
            return ret;
        }

        public void RetrieveAll(Data_AppUserFile.SortType sort, d1_Data_AppUserFile d, IMyLog log)
        {
            string strCmdFmt = "SELECT * FROM {0} ORDER BY {1}";
            string strCmd = "";
            switch (sort)
            {
                case Data_AppUserFile.SortType.Date:
                    strCmd = string.Format(strCmdFmt, TABLE_NAME, "[CreationDateUtc]"); break;
                case Data_AppUserFile.SortType.State:
                    strCmd = string.Format(strCmdFmt, TABLE_NAME, "[AccountStatus]"); break;
                case Data_AppUserFile.SortType.Email:
                    strCmd = string.Format(strCmdFmt, TABLE_NAME, "[Email]"); break;
                default: throw new NotSupportedException("Unknown sort - " + sort.ToString());
            }

            using (SqlDisposable s = new SqlDisposable(Db, strCmd))
            {
                while (s.Reader.Read())
                {
                    Data_AppUserFile r1 = readOneRecord(s.Reader);
                    d(r1);
                }
            }
        }
    }
}
