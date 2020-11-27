using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Configuration;

using NiceApiLibrary_low;

namespace NiceTray
{    
    class _6WhatsAppProcess_Pre_TelControll : _6WhatsAppProcess_Pre_Base
    {
        private ManualResetEvent StopRequest;
        private ManualResetEvent BackgroundRunning;
        private TelListController TheInfoList;
        private DateTime LastUpdated;
        private string AndroidEndPoint;
        private TimeSpan TimeBetweenUpdates;
        private Thread InternalThread;
        private Ix IxFaces;

        private DateTime NextUpdateScheduledFor
        {
            get
            {
                return LastUpdated + TimeBetweenUpdates;
            }
        }

        private TimeSpan TimeToNextUpdate
        {
            get
            {
                TimeSpan dif = NextUpdateScheduledFor - DateTime.UtcNow;
                if (dif.Ticks < 0)
                {
                    return new TimeSpan(0);
                }
                return dif;
            }
        }

        public _6WhatsAppProcess_Pre_TelControll(ManualResetEvent stopRequest)
        {
            StopRequest = stopRequest;
            BackgroundRunning = new ManualResetEvent(false);
            TheInfoList = new TelListController(
                "_6WhatsAppProcess_Pre_TelControll.SecondsInRecentAddedState".GetConfigInt());
            LastUpdated = DateTime.MinValue;
            AndroidEndPoint = "";
            TimeBetweenUpdates = new TimeSpan(6, 0, 0); // 6 hours
            InternalThread = new Thread(new ThreadStart(bg_updateThread));

            InternalThread.Start();
        }

        public override eI6Error Process(string destMobile, string msg, Ix iAll)
        {
            using (var x = new LogPreText("TelC", iAll))
            {
                // pre, initialise IFace
                if (IxFaces == null)
                {
                    using (var _lock = TheInfoList.GetLock())
                    {
                        IxFaces = iAll;
                    }
                }

                // pre, wait for the backgrount task to be up
                if (BackgroundRunning.WaitOne(1, false) == false)
                {
                    iAll.iDsp.AddLine("Waiting for background ...");
                    BackgroundRunning.WaitOne();
                    iAll.iDsp.AddLine("Waiting for background. Now up&running");
                }

                // 1) Check if tel is on android
                string telWithPlus = destMobile.Replace("zapi_", "");
                TelListController.OneTelEntry theTelEntry = null;

                using (var _lock = TheInfoList.GetLock())
                {
                    theTelEntry = _lock.Locked.GetEntry(telWithPlus);
                }
                if ((theTelEntry == null) || (!theTelEntry.IsOnAndroid)) // First round
                {
                    iAll.iDsp.AddLine("Tel not on Android ...");
                    iAll.iDsp.FileLog_Info("Tel not on Android");
                    // It seems this tel no is not on android,
                    // s1 ) refresh
                    List<string> retOnAndroid = null;
                    try
                    { 
                        using (var com = IxFaces.iUpdater.Android())
                        {
                            retOnAndroid = com.GetCommand(iAll.iDsp);
                        }
                        using (var _lock = TheInfoList.GetLock())
                        {
                            _lock.Locked.Merge(null, retOnAndroid, false);
                            _lock.Locked.Verify();
                            // update e1
                            theTelEntry = _lock.Locked.GetEntry(telWithPlus);
                        }
                        iAll.iDsp.AddLine2(new DetailedData_AndroidCommunication(true));
                    }
                    catch (SocketException se)
                    {

                    }
                    TelControllerReportToFile(TheInfoList.LastVerifyResult.GetFullReport());
                }

                if ((theTelEntry == null) || (!theTelEntry.IsOnAndroid)) // secound round
                {
                    iAll.iDsp.AddLine("Adding # to Android");
                    iAll.iDsp.FileLog_Info("Adding # to Android");
                    try
                    {
                        using (var com = IxFaces.iUpdater.Android())
                        {
                            com.SetCommand(telWithPlus, IxFaces.iDsp);
                        }
                        using (var _lock = TheInfoList.GetLock())
                        {
                            _lock.Locked.AddOneAndroid(telWithPlus);
                            _lock.Locked.AddOneServer(telWithPlus);
                            _lock.Locked.Verify();
                            // update e1
                            theTelEntry = _lock.Locked.GetEntry(telWithPlus);
                        }
                        iAll.iDsp.AddLine2(new DetailedData_AndroidCommunication(true));
                    }
                    catch (SocketException)
                    {

                    }
                    TelControllerReportToFile(TheInfoList.LastVerifyResult.GetFullReport());
                }

                if (theTelEntry == null)
                {
                    // could not add the number to android, so the android is down
                    iAll.iDsp.FileLog_Debug("*************** BAD ERROR: ANDROID IS DOWN ************* ");
                    iAll.iDsp.AddLine2(new DetailedData_AndroidCommunication(false));
                    return eI6Error.FailedButNoLettingHostKnow_TelNotActive;
                }

                eI6Error childResult = Child.Process(destMobile, msg, iAll);
                if (childResult == eI6Error.Step2Failed)
                {
                    // change the error code on recent added tel numbers, so we retry rather than fail
                    string why = null;
                    if ((theTelEntry == null) || (theTelEntry.RecentAddedToAndroid(out why)))
                    {
                        iAll.iDsp.FileLog_Debug("Deescalateing Step2Failed to TelNotActive");
                        childResult = eI6Error.FailedButNoLettingHostKnow_TelNotActive;
                    }
                    iAll.iDsp.FileLog_Debug("Why: " + why);
                }

                return childResult;
            }
        }

        public void Debug_AmendProcessId(int newProcessId)
        {
            Child.Debug_AmendProcessId(newProcessId);
        }

        public string Debug_AmendUrl(string suggestedUrl, string additionalInfo)
        {
            return suggestedUrl;
        }

        #region background

        private void bg_updateThread()
        {
            while (StopRequest.WaitOne(5000, false) == false)
            {
                // wait until we are due
                bool interfaceAvailable = false;

                try
                {
                    if (TimeToNextUpdate.Ticks == 0)
                    {
                        // due now
                        using (var _lock = TheInfoList.GetLock())
                        {
                            interfaceAvailable = IxFaces != null;
                        }

                        if (!interfaceAvailable)
                        {
                            // no interface
                            StopRequest.WaitOne(1000, false);
                        }
                        else
                        {
                            // interface ok
                            IxFaces.iDsp.AddLine2(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eStart));
                            OneUpdateResult upRes = bg_oneUpdate();
                            BackgroundRunning.Set();
                            IxFaces.iDsp.AddLine2(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eDone, true, NextUpdateScheduledFor));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (IxFaces != null)
                    {
                        IxFaces.iDsp.FileLog_Error("Background thread Exception: " + ex.Message);
                        IxFaces.iDsp.FileLog_Debug(ex.StackTrace);
                        IxFaces.iDsp.AddLine2(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eDone, false, NextUpdateScheduledFor));
                        IxFaces.iDsp.Delay(5000);
                    }
                }
            }
        }

        class OneUpdateResult
        {
            public int TelNoServer;
            public int TelNoAndroid;
            public bool AddedTelToAndroid;

        }
        private OneUpdateResult bg_oneUpdate()
        {
            OneUpdateResult ret = new OneUpdateResult();
            // 1) gather data
            IxFaces.iDsp.FileLog_Debug("bg_oneUpdate...");
            using (_8AndroidCommunicator com = IxFaces.iUpdater.Android())
            {
                AndroidEndPoint = com.GetEndPointInfo();
            }
            IxFaces.iDsp.AddLine2(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportEndPoint, AndroidEndPoint));

            List<string> fromServer = bg_nn_serverUpdate();
            ret.TelNoServer = fromServer.Count;
            IxFaces.iDsp.AddLine2(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportTelNumbers, 0, ret.TelNoServer));

            List<string> fromAndroid = bg_nn_androidUpdate();
            ret.TelNoAndroid = fromAndroid.Count;
            IxFaces.iDsp.AddLine2(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportTelNumbers, ret.TelNoAndroid, 0));

            TelListController.VerifyResult verRes = null;

            using (var _lock = TheInfoList.GetLock())
            {
                // 2) clear existing data
                _lock.Locked.Clear();

                // 3) recalc
                _lock.Locked.Merge(fromServer, fromAndroid, true);
                verRes = _lock.Locked.Verify();
            }
            IxFaces.iDsp.AddLine2(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportTelNumberMerge, verRes));
            TelControllerReportToFile(verRes.GetFullReport());

            if (verRes.OnlyOnServer.Count > 0)
            {
                using (_8AndroidCommunicator com = IxFaces.iUpdater.Android())
                {
                    foreach (string needUpdate in verRes.OnlyOnServer)
                    {
                        com.SetCommand(needUpdate, IxFaces.iDsp);
                        using (var _lock = TheInfoList.GetLock())
                        {
                            _lock.Locked.AddOneAndroid(needUpdate);
                        }
                        ret.AddedTelToAndroid = true;
                    }
                }
            }
            LastUpdated = DateTime.UtcNow;
            if (ret.AddedTelToAndroid)
            {
                LastUpdated = DateTime.MinValue;
            }
            return ret;
        }

        private void TelControllerReportToFile(TelListController.VerifyResult.Report report)
        {
            List<string> verRes = report.Summary;
            List<string> bigReport = report.Full;

            foreach (string line in verRes)
            {
                IxFaces.iDsp.FileLog_Info(line);
            }
            IxFaces.iDsp.FileLog_TelStatus(bigReport);
        }

        private List<string> bg_nn_serverUpdate()
        {
            List<string> ret = new List<string>();
            using (var com = IxFaces.iUpdater.Server())
            {
                ret = com.GetCommand(IxFaces.iDsp);
            }
            return ret;
        }

        private List<string> bg_nn_androidUpdate()
        {
            List<string> retOnAndroid = new List<string>();
            using (var com = IxFaces.iUpdater.Android())
            {
                //com.SetOnAndroidDispose(AndroidMemory.OnAndroidMemory);
                retOnAndroid = com.GetCommand(IxFaces.iDsp);
            }
            return retOnAndroid;
        }

        #endregion
    }
}
