using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NiceApiLibrary_low
{
    public class TelListController
    {
        private LockedTelListController toLock;
        public VerifyResult LastVerifyResult;

        public Locker GetLock()
        {
            return new Locker(toLock);
        }

        public TelListController(int secondsInRecentAddedState)
        {
            toLock = new LockedTelListController(this, secondsInRecentAddedState);
        }
        public TelListController()
        {
            toLock = new LockedTelListController(this, 60);
        }

        public static void currentTelListBuilder(Data_AppUserFile d, TelListController.Locker _lock)
        {
            if (
                //(d.AccountStatus == Data_AppUserFile.eUserStatus.email_sent_for_verification) ||
                (d.AccountStatus == Data_AppUserFile.eUserStatus.verified_welcome_No_sent) ||
                (d.AccountStatus == Data_AppUserFile.eUserStatus.verified_welcome_queued) ||
                (d.AccountStatus == Data_AppUserFile.eUserStatus.verified_checkingTelNumbers) ||
                (d.AccountStatus == Data_AppUserFile.eUserStatus.blocked) ||
                (d.AccountStatus == Data_AppUserFile.eUserStatus.free_account))
            //(d.AccountStatus == Data_AppUserFile.eUserStatus.commercial_monthly) ||
            //(d.AccountStatus == Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice) ||
            //(d.AccountStatus == Data_AppUserFile.eUserStatus.commercial_payassent) ||
            //(d.AccountStatus == Data_AppUserFile.eUserStatus.commercial_systemDuplication))
            {
                foreach (string t1 in d.MobileNumbers_AllConfirmed__.MobileNumberArray)
                {
                    _lock.Locked.AddOneServer(t1);
                }
                foreach (string t1 in d.MobileNumbers_AllUnConfirmed__.MobileNumberArray)
                {
                    _lock.Locked.AddOneServer(t1);
                }
            }
        }

        public class LockedTelListController
        {
            private Dictionary<string, OneTelEntry> Dic;
            private TelListController Outer;
            public int SecondsInRecentAddedState;

            public LockedTelListController(TelListController outer, int secondsInRecentAddedState)
            {
                Dic = new Dictionary<string, OneTelEntry>();
                Outer = outer;
                SecondsInRecentAddedState = secondsInRecentAddedState;
            }

            public void Clear()
            {
                Dic.Clear();
            }

            private OneTelEntry getOrCreate(string telNo)
            {
                if (!Dic.ContainsKey(telNo))
                {
                    Dic.Add(telNo, new OneTelEntry(telNo, SecondsInRecentAddedState));
                }
                return Dic[telNo];
            }

            public VerifyResult Verify()
            {
                VerifyResult res = new VerifyResult();
                List<string> sortedKeys = Dic.Keys.ToList();
                sortedKeys.Sort();
                foreach (string k1 in sortedKeys)
                {
                    OneTelEntry e1 = Dic[k1];
                    if (e1.IsOnAndroid && e1.IsOnServer)
                    {
                        res.OnBothAndroidAndServer.Add(k1);
                    }
                    if (!e1.IsOnAndroid && e1.IsOnServer)
                    {
                        res.OnlyOnServer.Add(k1);
                    }
                    if (e1.IsOnAndroid && !e1.IsOnServer)
                    {
                        res.OnlyOnAndroid.Add(k1);
                    }

                    if (e1.ServerCount > 1)
                    {
                        res.UserMoreThanOnceOnServer.Add(k1);
                    }
                }
                Outer.LastVerifyResult = res;
                return res;
            }

            public OneTelEntry GetEntry(string telNo)
            {
                if (Dic.ContainsKey(telNo))
                {
                    return Dic[telNo];
                }
                return null;
            }

            public void AddOneAndroid(string telNoAddedToAndroid)
            {
                OneTelEntry e = getOrCreate(telNoAddedToAndroid);
                if (!e.IsOnAndroid)
                {
                    e.AndroidCount++;
                }
                e.OnAndroidAddedTime = DateTime.UtcNow;
            }
            public void AddOneServer(string telNoAddedToServer)
            {
                OneTelEntry e = getOrCreate(telNoAddedToServer);
                if (!e.IsOnServer)
                {
                    e.ServerCount++;
                }
            }

            public void Merge(List<string> serverList, List<string> androidList, bool doIncrement)
            {
                if (serverList != null)
                {
                    foreach (string oneOnServer in serverList)
                    {
                        OneTelEntry e = getOrCreate(oneOnServer);
                        if (doIncrement)
                        {
                            e.ServerCount++;
                        }
                        else
                        {
                            if (!e.IsOnServer)
                            {
                                e.ServerCount++;
                            }
                        }
                    }
                }

                if (androidList != null)
                {
                    foreach (string oneOnAndroid in androidList)
                    {
                        OneTelEntry e = getOrCreate(oneOnAndroid);
                        if (doIncrement)
                        {
                            e.AndroidCount++;
                        }
                        else
                        {
                            if (!e.IsOnAndroid)
                            {
                                e.AndroidCount++;
                            }
                        }
                    }
                }
            }

            public bool IsEmpty
            {
                get
                {
                    return Dic.Count == 0;
                }
            }

        }

        public class OneTelEntry
        {
            public string Tel;
            public int ServerCount;
            public int AndroidCount;
            public bool IsOnServer { get { return ServerCount >= 1; } }
            public bool IsOnAndroid { get { return AndroidCount >= 1; } }
            public DateTime OnAndroidAddedTime;
            public int SecondsInRecentAddedState;

            public OneTelEntry(string telNo, int secondsInRecentAddedState)
            {
                this.Tel = telNo;
                this.SecondsInRecentAddedState = secondsInRecentAddedState;
            }

            public bool RecentAddedToAndroid(out string why)
            {
                why = "";
                if (!IsOnAndroid)
                {
                    // not on android at all
                    why = "Not on android at all";
                    return false;
                }
                else if (OnAndroidAddedTime.Ticks == 0)
                {
                    // adding time not known
                    why = "Adding time not known, set to now";
                    OnAndroidAddedTime = DateTime.UtcNow;
                    return true;
                }
                else if (OnAndroidAddedTime.AddSeconds(SecondsInRecentAddedState) < DateTime.UtcNow)
                {   
                    // It has been there for a long time
                    why = "It has been there for a long time. " + OnAndroidAddedTime.ToSwissTime(true);
                    return false;
                }
                else
                {
                    // It has been added recently
                    why = "It has been added recently. " + OnAndroidAddedTime.ToSwissTime(true);
                    return true;
                }
            }
        }

        public class VerifyResult
        {
            public List<string> OnBothAndroidAndServer;
            public List<string> OnlyOnAndroid;
            public List<string> OnlyOnServer;
            public List<string> UserMoreThanOnceOnServer;

            public VerifyResult()
            {
                OnBothAndroidAndServer = new List<string>();
                OnlyOnAndroid = new List<string>();
                OnlyOnServer = new List<string>();
                UserMoreThanOnceOnServer = new List<string>();
            }

            /*public*/ List<string> GetReport()
            {
                List<string> ret = new List<string>();
                ret.Add("OnBothAndroidAndServer  .Count = " + OnBothAndroidAndServer.Count.ToString());
                ret.Add("OnlyOnAndroid           .Count = " + OnlyOnAndroid.Count.ToString());
                ret.Add("OnlyOnServer            .Count = " + OnlyOnServer.Count.ToString());
                ret.Add("UserMoreThanOnceOnServer.Count = " + UserMoreThanOnceOnServer.Count.ToString());
                ret.Add("Timestamp CH                   = " + DateTime.UtcNow.ToSwissTime(false));
                return ret;
            }

            void addSection(string pre, string name, List<string> tels, ref List<string> rep)
            {
                rep.Add("".PadLeft(50, '-'));
                foreach (string tel in tels)
                {
                    rep.Add(pre + tel);
                }
            }

            public Report GetFullReport()
            {
                Report ret = new Report();
                ret.Summary = GetReport();

                ret.Full = GetReport();
                addSection("b ", "OnBothAndroidAndServer", OnBothAndroidAndServer, ref ret.Full);
                addSection("a ", "OnlyOnAndroid", OnlyOnAndroid, ref ret.Full);
                addSection("s ", "OnlyOnServer", OnlyOnServer, ref ret.Full);
                addSection("d ", "UserMoreThanOnceOnServer", UserMoreThanOnceOnServer, ref ret.Full);

                return ret;
            }

            public class Report
            {
                public List<string> Summary;
                public List<string> Full;
            }
        }

        public class Locker : IDisposable
        {
            public LockedTelListController Locked;
            public Locker(LockedTelListController toLock)
            {
                Locked = toLock;
                Monitor.Enter(Locked);
            }
            public void Dispose()
            {
                Monitor.Exit(Locked);
            }
        }
    }
}
