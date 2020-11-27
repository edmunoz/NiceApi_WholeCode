using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public partial class Data_AppUserFile
    {
        public Int32 _FileVersion = -1;
        public string UserName;
        public bool NoSpam;
        public MobileNoHandler MobileNumbers_AllConfirmed__;
        public MobileNoHandler MobileNumbers_AllUnConfirmed__;
        public string Email;
        public string Password;
        public Int64 CreationDate;
        public string CreationIp;
        public string ApiGuId;
        public string Comment;
        public bool DeleteOnFailed;
        public bool AddNumber_AllowedWithAPI;
        public bool AddNumber_ActivateOnSyncRequest;
        public eUserStatus AccountStatus;

        public freeAccount FreeAccount;
        public monthlyAccount MonthlyAccount;
        public payAsSentAccount PayAsSentAccount;
        public monthlyDifPriceAccount MonthlyDifPriceAccount;
        public systemDuplicationAccount SystemDuplicationAccount;

        public enum SortType
        {
            Date,
            State,
            Email, // default
            Usage,
            DontSort,
        }

        public enum eUserStatus
        {
            email_sent_for_verification,
            verified_welcome_No_sent,
            verified_welcome_queued,
            blocked,
            free_account,
            commercial_monthly,
            commercial_payassent,
            commercial_systemDuplication,
            verified_checkingTelNumbers,
            commercial_monthlyDifPrice,
            _end
        }
        private enum eUserStatus_V0
        {
            email_sent_for_verification,
            verified,
            blocked,
            active,
            _end
        }

        private void InitWithDefaults()
        {
            this.UserName = string.Empty;
            this.NoSpam = false; ;
            this.MobileNumbers_AllConfirmed__ = new MobileNoHandler("");
            this.MobileNumbers_AllUnConfirmed__ = new MobileNoHandler("");
            this.Email = string.Empty;
            this.Password = string.Empty;

            this.CreationDate = DateTime.MinValue.Ticks;
            this.CreationIp = string.Empty;
            this.ApiGuId = string.Empty;
            //this.NoOfQueuedMessages = 0;
            //this.NoOfSentMessages = 0;
            //this.NoOfBytesSent = 0;
            //this.LastMsgQueued = 0;
            //this.MinDelayInSeconds = 60;
            this.AccountStatus = eUserStatus.email_sent_for_verification;
            //this.RemainingMessages = 200;
            this.Comment = string.Empty;
            //this.SendFooter = true;
            //this.WelcomeCounter = 0;
            this.DeleteOnFailed = false;
            this.AddNumber_AllowedWithAPI = false;
            this.AddNumber_ActivateOnSyncRequest = false;

            this.FreeAccount = new freeAccount();
            this.MonthlyAccount = new monthlyAccount();
            this.PayAsSentAccount = new payAsSentAccount();
            this.MonthlyDifPriceAccount = new monthlyDifPriceAccount();
            this.SystemDuplicationAccount = new systemDuplicationAccount();
        }

        public void WriteToStream(BinaryWriter bw)
        {
            // v0
            // v1 enum changed 
            // v2 MoblieAllNumbers in one string
            // v3 Comment and sendFooter added
            // v4 WelcomeCounter added
            // v5 MobileNumbers_AllUnConfirmed added, support for commercial 
            // v6 Counters split for differenc accont types
            // v7 Added MonthlyDifPriceAccount
            // v8 Added SystemDuplicationAccount
            bw.Write((Int32)8);
            bw.Write(UserName);
            bw.Write(NoSpam);
            bw.Write(MobileNumbers_AllConfirmed__.getVal);
            bw.Write(MobileNumbers_AllUnConfirmed__.getVal);
            bw.Write(Email);
            bw.Write(Password);
            bw.Write(CreationDate);
            bw.Write(CreationIp);
            bw.Write(ApiGuId);
            bw.Write((Int32)AccountStatus);
            bw.Write(Comment);
            bw.Write(DeleteOnFailed);
            bw.Write(AddNumber_AllowedWithAPI);
            bw.Write(AddNumber_ActivateOnSyncRequest);
            FreeAccount.WriteToStream(bw);
            MonthlyAccount.WriteToStream(bw);
            PayAsSentAccount.WriteToStream(bw);
            MonthlyDifPriceAccount.WriteToStream(bw);
            SystemDuplicationAccount.WriteToStream(bw);
        }



        public void ReadFromStream(BinaryReader br)
        {
            // Set defaults for fields that might not be read
            InitWithDefaults();

            Int32 v = br.ReadInt32();
            _FileVersion = v;
            switch (v)
            {
                case 0: ReadFromStream_V0(br); break;
                case 1: ReadFromStream_V1(br); break;
                case 2: ReadFromStream_V2(br); break;
                case 3: ReadFromStream_V3(br); break;
                case 4: ReadFromStream_V4(br); break;
                case 5: ReadFromStream_V5(br); break;
                case 6: ReadFromStream_V6(br); break;
                case 7: ReadFromStream_V7(br); break;
                case 8: ReadFromStream_V8(br); break;

                default: throw new NotSupportedException("File invalid format");
            }
        }

        private void ReadFromStream_V8(BinaryReader br)
        {
            ReadFromStream_V7(br);
            SystemDuplicationAccount.ReadFromStream(br);
        }

        private void ReadFromStream_V7(BinaryReader br)
        {
            ReadFromStream_V6(br);
            MonthlyDifPriceAccount.ReadFromStream(br);
        }

        private void ReadFromStream_V6(BinaryReader br)
        {
            UserName = br.ReadString();
            NoSpam = br.ReadBoolean();
            MobileNumbers_AllConfirmed__ = new MobileNoHandler(br.ReadString());
            MobileNumbers_AllUnConfirmed__ = new MobileNoHandler(br.ReadString());
            Email = br.ReadString();
            Password = br.ReadString();
            CreationDate = br.ReadInt64();
            CreationIp = br.ReadString();
            ApiGuId = br.ReadString();
            AccountStatus = (eUserStatus)br.ReadInt32();
            Comment = br.ReadString();
            DeleteOnFailed = br.ReadBoolean();
            AddNumber_AllowedWithAPI = br.ReadBoolean();
            AddNumber_ActivateOnSyncRequest = br.ReadBoolean();
            FreeAccount.ReadFromStream(br);
            MonthlyAccount.ReadFromStream(br);
            PayAsSentAccount.ReadFromStream(br);
        }

        private void ReadFromStream_V5(BinaryReader br)
        {
            UserName = br.ReadString();
            NoSpam = br.ReadBoolean();
            MobileNumbers_AllConfirmed__ = new MobileNoHandler(br.ReadString());
            MobileNumbers_AllUnConfirmed__ = new MobileNoHandler(br.ReadString());
            Email = br.ReadString();
            Password = br.ReadString();
            CreationDate = br.ReadInt64();
            CreationIp = br.ReadString();
            ApiGuId = br.ReadString();
            FreeAccount.free_MsgQueued = br.ReadInt64();
            FreeAccount.free_MsgSent /* NoOfSentMessages */ = br.ReadInt64();
            /* NoOfBytesSent = */
            br.ReadInt64();
            FreeAccount.free_MsgLeft /* RemainingMessages */ = br.ReadInt64();
            FreeAccount.free_LastMsgQueued = br.ReadInt64();
            FreeAccount.free_MinDelayInSeconds = br.ReadInt32();
            AccountStatus = (eUserStatus)br.ReadInt32();
            Comment = br.ReadString();
            FreeAccount.free_SendFooter = br.ReadBoolean();
            FreeAccount.free_WelcomeCounter = br.ReadInt16();
            DeleteOnFailed = br.ReadBoolean();
            AddNumber_AllowedWithAPI = br.ReadBoolean();
            AddNumber_ActivateOnSyncRequest = br.ReadBoolean();
        }

        private void ReadFromStream_V4(BinaryReader br)
        {
            UserName = br.ReadString();
            NoSpam = br.ReadBoolean();
            MobileNumbers_AllConfirmed__ = new MobileNoHandler(br.ReadString());
            MobileNumbers_AllUnConfirmed__ = new MobileNoHandler("");
            Email = br.ReadString();
            Password = br.ReadString();
            CreationDate = br.ReadInt64();
            CreationIp = br.ReadString();
            ApiGuId = br.ReadString();
            FreeAccount.free_MsgQueued = br.ReadInt64();
            FreeAccount.free_MsgSent /* NoOfSentMessages */ = br.ReadInt64();
            /* NoOfBytesSent = */
            br.ReadInt64();
            FreeAccount.free_MsgLeft /* RemainingMessages */ = br.ReadInt64();
            FreeAccount.free_LastMsgQueued = br.ReadInt64();
            FreeAccount.free_MinDelayInSeconds = br.ReadInt32();
            AccountStatus = (eUserStatus)br.ReadInt32();
            Comment = br.ReadString();
            FreeAccount.free_SendFooter = br.ReadBoolean();
            FreeAccount.free_WelcomeCounter = br.ReadInt16();
        }

        private void ReadFromStream_V3(BinaryReader br)
        {
            UserName = br.ReadString();
            NoSpam = br.ReadBoolean();
            MobileNumbers_AllConfirmed__ = new MobileNoHandler(br.ReadString());
            MobileNumbers_AllUnConfirmed__ = new MobileNoHandler("");
            Email = br.ReadString();
            Password = br.ReadString();
            CreationDate = br.ReadInt64();
            CreationIp = br.ReadString();
            ApiGuId = br.ReadString();
            FreeAccount.free_MsgQueued = br.ReadInt64();
            FreeAccount.free_MsgSent /* NoOfSentMessages */ = br.ReadInt64();
            /* NoOfBytesSent = */
            br.ReadInt64();
            FreeAccount.free_MsgLeft /* RemainingMessages */ = br.ReadInt64();
            FreeAccount.free_LastMsgQueued = br.ReadInt64();
            FreeAccount.free_MinDelayInSeconds = br.ReadInt32();
            AccountStatus = (eUserStatus)br.ReadInt32();
            Comment = br.ReadString();
            FreeAccount.free_SendFooter = br.ReadBoolean();
        }

        private void ReadFromStream_V2(BinaryReader br)
        {
            UserName = br.ReadString();
            NoSpam = br.ReadBoolean();
            MobileNumbers_AllConfirmed__ = new MobileNoHandler(br.ReadString());
            MobileNumbers_AllUnConfirmed__ = new MobileNoHandler("");
            Email = br.ReadString();
            Password = br.ReadString();
            CreationDate = br.ReadInt64();
            CreationIp = br.ReadString();
            ApiGuId = br.ReadString();
            FreeAccount.free_MsgQueued = br.ReadInt64();
            FreeAccount.free_MsgSent /* NoOfSentMessages */ = br.ReadInt64();
            /* NoOfBytesSent = */
            br.ReadInt64();
            FreeAccount.free_MsgLeft /* RemainingMessages */ = br.ReadInt64();
            FreeAccount.free_LastMsgQueued = br.ReadInt64();
            FreeAccount.free_MinDelayInSeconds = br.ReadInt32();
            AccountStatus = (eUserStatus)br.ReadInt32();
        }

        private void ReadFromStream_V1(BinaryReader br)
        {
            UserName = br.ReadString();
            NoSpam = br.ReadBoolean();
            int mobCount = br.ReadInt32();
            MobileNumbers_AllConfirmed__ = new MobileNoHandler("");
            MobileNumbers_AllUnConfirmed__ = new MobileNoHandler("");
            for (int i = 0; i < mobCount; i++)
            {
                MobileNumbers_AllConfirmed__.Add(br.ReadString());
            }
            Email = br.ReadString();
            Password = br.ReadString();
            CreationDate = br.ReadInt64();
            CreationIp = br.ReadString();
            ApiGuId = br.ReadString();
            FreeAccount.free_MsgQueued = br.ReadInt64();
            FreeAccount.free_MsgSent /* NoOfSentMessages */ = br.ReadInt64();
            /* NoOfBytesSent = */
            br.ReadInt64();
            FreeAccount.free_MsgLeft /* RemainingMessages */ = br.ReadInt64();
            FreeAccount.free_LastMsgQueued = br.ReadInt64();
            FreeAccount.free_MinDelayInSeconds = br.ReadInt32();
            AccountStatus = (eUserStatus)br.ReadInt32();
        }

        private void ReadFromStream_V0(BinaryReader br)
        {
            UserName = br.ReadString();
            NoSpam = br.ReadBoolean();
            int mobCount = br.ReadInt32();
            MobileNumbers_AllConfirmed__ = new MobileNoHandler("");
            MobileNumbers_AllUnConfirmed__ = new MobileNoHandler("");
            for (int i = 0; i < mobCount; i++)
            {
                MobileNumbers_AllConfirmed__.Add(br.ReadString());
            }
            Email = br.ReadString();
            Password = br.ReadString();
            CreationDate = br.ReadInt64();
            CreationIp = br.ReadString();
            ApiGuId = br.ReadString();
            FreeAccount.free_MsgQueued = br.ReadInt64();
            FreeAccount.free_MsgSent /* NoOfSentMessages */ = br.ReadInt64();
            /* NoOfBytesSent = */
            br.ReadInt64();
            FreeAccount.free_MsgLeft /* RemainingMessages */ = br.ReadInt64();
            FreeAccount.free_LastMsgQueued = br.ReadInt64();
            FreeAccount.free_MinDelayInSeconds = br.ReadInt32();
            switch ((eUserStatus_V0)br.ReadInt32())
            {
                case eUserStatus_V0.email_sent_for_verification: AccountStatus = eUserStatus.email_sent_for_verification; break;
                case eUserStatus_V0.verified: AccountStatus = eUserStatus.verified_welcome_No_sent; break;
                case eUserStatus_V0.blocked: AccountStatus = eUserStatus.blocked; break;
                case eUserStatus_V0.active: AccountStatus = eUserStatus.free_account; break;
            }
        }

        public class freeAccount
        {
            public Int64 free_LastMsgQueued;
            public Int64 free_MsgSent;
            public Int64 free_MsgLeft;
            public Int32 free_MinDelayInSeconds;
            public bool free_SendFooter;
            public Int16 free_WelcomeCounter;
            public Int64 free_MsgQueued;

            public freeAccount()
            {
                InitWithDefaults();
            }

            public void InitWithDefaults()
            {
                free_LastMsgQueued = 0; ;
                free_MsgSent = 0;
                free_MsgLeft = 200;
                free_MinDelayInSeconds = 60;
                free_SendFooter = true;
                free_WelcomeCounter = 0;
                free_MsgQueued = 0;
            }
            public void WriteToStream(BinaryWriter bw)
            {
                // freeAccount v0
                bw.Write((Int32)0);
                bw.Write(free_LastMsgQueued);
                bw.Write(free_MsgSent);
                bw.Write(free_MsgLeft);
                bw.Write(free_MinDelayInSeconds);
                bw.Write(free_SendFooter);
                bw.Write(free_WelcomeCounter);
                bw.Write(free_MsgQueued);
            }
            private void ReadFromStream_V0(BinaryReader br)
            {
                free_LastMsgQueued = br.ReadInt64();
                free_MsgSent = br.ReadInt64();
                free_MsgLeft = br.ReadInt64();
                free_MinDelayInSeconds = br.ReadInt32();
                free_SendFooter = br.ReadBoolean();
                free_WelcomeCounter = br.ReadInt16();
                free_MsgQueued = br.ReadInt64();
            }
            public void ReadFromStream(BinaryReader br)
            {
                // Set defaults for fields that might not be read
                InitWithDefaults();

                Int32 v = br.ReadInt32();
                switch (v)
                {
                    case 0: ReadFromStream_V0(br); break;
                    default: throw new NotSupportedException("File invalid format");
                }
            }
        }

        public class niceMoney
        {
            public Int64 ValueInUsCent;

            public niceMoney(Int64 ValueInUsCent)
            {
                this.ValueInUsCent = ValueInUsCent;
            }

            public static niceMoney Parse(string val)
            {
                if (val == null)
                {
                    throw new ArgumentNullException();
                }
                string[] sp = val.Replace("Usd", "").Split(new char[] { '.' });
                if (sp.Length == 1)
                {
                    // only dolar
                    return new niceMoney(100 * Int64.Parse(sp[0]));
                }
                else if (sp.Length == 2)
                {
                    // dolar and cents
                    return new niceMoney(Int64.Parse(sp[1]) + (100 * Int64.Parse(sp[0])));
                }
                else
                {
                    throw new FormatException();
                }
            }

            public override string ToString()
            {
                Int64 d = ValueInUsCent / 100;
                Int64 c = ValueInUsCent % 100;
                string s = String.Format("{0}.{1:00} Usd", d, c);
                return s;
            }

            public niceMoney MultiplyBy(int m)
            {
                return new niceMoney(ValueInUsCent * m);
            }

            public void WriteToStream(BinaryWriter bw)
            {
                bw.Write(ValueInUsCent);
            }
            public void ReadFromStream(BinaryReader br)
            {
                ValueInUsCent = br.ReadInt64();
            }

        }

        public class monthlyAccount
        {
            public Int64 monthly_LastMsgQueued;
            public Int64 monthly_MsgSent;
            public Int64 monthly_PaidUntil;
            public Int32 monthly_MinDelayInSeconds;
            public niceMoney monthly_CostPerNumber;
            public niceMoney monthly_CurrentCredit;

            public monthlyAccount()
            {
                InitWithDefaults();
            }

            public void InitWithDefaults()
            {
                monthly_LastMsgQueued = 0;
                monthly_MsgSent = 0;
                monthly_PaidUntil = 0;
                monthly_MinDelayInSeconds = 10;
                monthly_CostPerNumber = new niceMoney(50);
                monthly_CurrentCredit = new niceMoney(0);
            }
            public void WriteToStream(BinaryWriter bw)
            {
                // monthlyAccount v0
                bw.Write((Int32)0);
                bw.Write(monthly_LastMsgQueued);
                bw.Write(monthly_MsgSent);
                bw.Write(monthly_PaidUntil);
                bw.Write(monthly_MinDelayInSeconds);
                monthly_CostPerNumber.WriteToStream(bw);
                monthly_CurrentCredit.WriteToStream(bw);
            }
            private void ReadFromStream_V0(BinaryReader br)
            {
                monthly_LastMsgQueued = br.ReadInt64();
                monthly_MsgSent = br.ReadInt64();
                monthly_PaidUntil = br.ReadInt64();
                monthly_MinDelayInSeconds = br.ReadInt32();
                monthly_CostPerNumber.ReadFromStream(br);
                monthly_CurrentCredit.ReadFromStream(br);
            }
            public void ReadFromStream(BinaryReader br)
            {
                // Set defaults for fields that might not be read
                InitWithDefaults();

                Int32 v = br.ReadInt32();
                switch (v)
                {
                    case 0: ReadFromStream_V0(br); break;
                    default: throw new NotSupportedException("File invalid format");
                }
            }
        }

        public class monthlyDifPriceAccount
        {
            public Int64 monthlyDifPrice_LastMsgQueued;
            public Int64 monthlyDifPrice_TotalMsgSent;
            public Int64 monthlyDifPrice_ThisMonthMsgSent;
            public Int64 monthlDifPricey_PeriodeStart;
            public Int32 monthlyDifPrice_PeriodeDurationInDays;
            public Int32 monthlyDifPrice_MinDelayInSeconds;
            public niceMoney monthlyDifPrice_CostPerNumber;
            public niceMoney monthlyDifPrice_CurrentCredit;
            public string monthlyDifPrice_LevelDefinitions;
            public int monthlyDifPrice_Level;
            public bool monthlyDifPrice_AutoInceremntLevel;
            public bool monthlyDifPrice_AutoRenewMonthPayment;

            public monthlyDifPriceAccount()
            {
                InitWithDefaults();
            }

            public void InitWithDefaults()
            {
                monthlyDifPrice_LastMsgQueued = 0;
                monthlyDifPrice_TotalMsgSent = 0;
                monthlyDifPrice_ThisMonthMsgSent = 0;
                monthlDifPricey_PeriodeStart = 0;
                monthlyDifPrice_PeriodeDurationInDays = 30;
                monthlyDifPrice_MinDelayInSeconds = 0;
                monthlyDifPrice_CostPerNumber = new niceMoney(50);
                monthlyDifPrice_CurrentCredit = new niceMoney(0);
                monthlyDifPrice_LevelDefinitions = "200/9.99|500/14.99|1000/19.99|2000/24.99|5000/29.99|10000/34.99";
                monthlyDifPrice_Level = 0;
                monthlyDifPrice_AutoInceremntLevel = true;
                monthlyDifPrice_AutoRenewMonthPayment = true;
            }

            public void WriteToStream(BinaryWriter bw)
            {
                // monthlyAccount v0
                bw.Write((Int32)0);

                bw.Write(monthlyDifPrice_LastMsgQueued);
                bw.Write(monthlyDifPrice_TotalMsgSent);
                bw.Write(monthlyDifPrice_ThisMonthMsgSent);
                bw.Write(monthlDifPricey_PeriodeStart);
                bw.Write(monthlyDifPrice_PeriodeDurationInDays);
                bw.Write(monthlyDifPrice_MinDelayInSeconds);
                monthlyDifPrice_CostPerNumber.WriteToStream(bw);
                monthlyDifPrice_CurrentCredit.WriteToStream(bw);
                bw.Write(monthlyDifPrice_LevelDefinitions);
                bw.Write(monthlyDifPrice_Level);
                bw.Write(monthlyDifPrice_AutoInceremntLevel);
                bw.Write(monthlyDifPrice_AutoRenewMonthPayment);
            }
            private void ReadFromStream_V0(BinaryReader br)
            {
                monthlyDifPrice_LastMsgQueued = br.ReadInt64();
                monthlyDifPrice_TotalMsgSent = br.ReadInt64();
                monthlyDifPrice_ThisMonthMsgSent = br.ReadInt64();
                monthlDifPricey_PeriodeStart = br.ReadInt64();
                monthlyDifPrice_PeriodeDurationInDays = br.ReadInt32();
                monthlyDifPrice_MinDelayInSeconds = br.ReadInt32();
                monthlyDifPrice_CostPerNumber.ReadFromStream(br);
                monthlyDifPrice_CurrentCredit.ReadFromStream(br);
                monthlyDifPrice_LevelDefinitions = br.ReadString();
                monthlyDifPrice_Level = br.ReadInt32();
                monthlyDifPrice_AutoInceremntLevel = br.ReadBoolean();
                monthlyDifPrice_AutoRenewMonthPayment = br.ReadBoolean();
            }
            public void ReadFromStream(BinaryReader br)
            {
                // Set defaults for fields that might not be read
                InitWithDefaults();

                Int32 v = br.ReadInt32();
                switch (v)
                {
                    case 0: ReadFromStream_V0(br); break;
                    default: throw new NotSupportedException("File invalid format");
                }
            }
        }

        public class payAsSentAccount
        {
            public Int64 payAsSent_LastMsgQueued;
            public Int64 payAsSent_MsgSent;
            public Int32 payAsSent_MinDelayInSeconds;
            public niceMoney payAsSent_CostPerNumber;
            public niceMoney payAsSent_CostPerMessage;
            public niceMoney payAsSent_CurrentCredit;

            public payAsSentAccount()
            {
                InitWithDefaults();
            }

            public void InitWithDefaults()
            {
                payAsSent_LastMsgQueued = 0;
                payAsSent_MsgSent = 0;
                payAsSent_MinDelayInSeconds = 0;
                payAsSent_CostPerNumber = new niceMoney(50);
                payAsSent_CostPerMessage = new niceMoney(12);
                payAsSent_CurrentCredit = new niceMoney(0);
            }
            public void WriteToStream(BinaryWriter bw)
            {
                // payAsSentAccount v0
                bw.Write((Int32)0);
                bw.Write(payAsSent_LastMsgQueued);
                bw.Write(payAsSent_MsgSent);
                bw.Write(payAsSent_MinDelayInSeconds);
                payAsSent_CostPerNumber.WriteToStream(bw);
                payAsSent_CostPerMessage.WriteToStream(bw);
                payAsSent_CurrentCredit.WriteToStream(bw);
            }
            private void ReadFromStream_V0(BinaryReader br)
            {
                payAsSent_LastMsgQueued = br.ReadInt64();
                payAsSent_MsgSent = br.ReadInt64();
                payAsSent_MinDelayInSeconds = br.ReadInt32();
                payAsSent_CostPerNumber.ReadFromStream(br);
                payAsSent_CostPerMessage.ReadFromStream(br);
                payAsSent_CurrentCredit.ReadFromStream(br);
            }
            public void ReadFromStream(BinaryReader br)
            {
                // Set defaults for fields that might not be read
                InitWithDefaults();

                Int32 v = br.ReadInt32();
                switch (v)
                {
                    case 0: ReadFromStream_V0(br); break;
                    default: throw new NotSupportedException("File invalid format");
                }
            }
        }

        public class systemDuplicationAccount
        {
            public Int64 systemDuplication_LastMsgQueued;
            public Int64 systemDuplication_MsgSent;
            public Int64 systemDuplication_PaidUntil;

            public systemDuplicationAccount()
            {
                InitWithDefaults();
            }

            public void InitWithDefaults()
            {
                systemDuplication_LastMsgQueued = 0;
                systemDuplication_MsgSent = 0;
                systemDuplication_PaidUntil = 0;
            }
            public void WriteToStream(BinaryWriter bw)
            {
                // systemDuplicationAccount v0
                bw.Write((Int32)0);
                bw.Write(systemDuplication_LastMsgQueued);
                bw.Write(systemDuplication_MsgSent);
                bw.Write(systemDuplication_PaidUntil);
            }
            private void ReadFromStream_V0(BinaryReader br)
            {
                systemDuplication_LastMsgQueued = br.ReadInt64();
                systemDuplication_MsgSent = br.ReadInt64();
                systemDuplication_PaidUntil = br.ReadInt64();
            }
            public void ReadFromStream(BinaryReader br)
            {
                // Set defaults for fields that might not be read
                InitWithDefaults();

                Int32 v = br.ReadInt32();
                switch (v)
                {
                    case 0: ReadFromStream_V0(br); break;
                    default: throw new NotSupportedException("File invalid format");
                }
            }
        }

    }
}
