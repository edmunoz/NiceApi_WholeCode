using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using NiceApiLibrary_low;

namespace NiceTray
{
    class _6WhatsAppProcess_Pre_FoxStarter : _6WhatsAppProcess_Pre_Base
    {
        public override eI6Error Process(string destMobile, string msg, Ix iAll)
        {
            using (var x = new LogPreText("FoxS", iAll))
            {
                eI6Error stateThisTime = Process_NoLog(destMobile, msg, iAll);
                return stateThisTime;
            }
        }

        private eI6Error Process_NoLog(string destMobile, string msg, Ix iAll)
        {
            eI6Error ret = eI6Error._end;
            eI6Error eFirstInCheck = Child.Process(destMobile, msg, iAll);
            iAll.iDsp.FileLog_Debug(eFirstInCheck.ToString());
            if (eFirstInCheck.IsGood())
            {
                // Success
                ret = eFirstInCheck;
            }
            else if (eFirstInCheck.IsMayBeGood())
            {
                // MayBe
                ret = eFirstInCheck;
            }
            else
            {
                // Bad
                iAll.iDsp.FileLog_Debug("1stCheck is Bad");
                if (iAll.TypeOfProcess == Ix.eTypeOfProcess.Normal)
                {
                    // normal prosessing
                    ret = eFirstInCheck;
                }
                else
                {
                    // tel number checking
                    iAll.iDsp.FileLog_Info("1stCheck bad on TelNoChecking, checking KnowNumber1...");
                    eI6Error eKnown1 = Child.Process(
                        "_6WhatsAppProcess_PreFoxStarter.KnowNumber1".GetConfig(), null, iAll);
                    if (eKnown1.IsBad())
                    {
                        // not even the know works, we are having a problem with Fox
                        iAll.iDsp.FileLog_Debug("Not even the known1 works.");
                        ret = eI6Error.FailedButNoLettingHostKnow_TelNotActive;
                    }
                    else if (eKnown1.IsMayBeGood())
                    {
                        // this should never happen, it is a known number
                        iAll.iDsp.FileLog_Error("How can a known1 return MayBe?");
                        ret = eI6Error.FailedButNoLettingHostKnow_TelNotActive;
                    }
                    else
                    {
                        // the known1 one worked
                        iAll.iDsp.FileLog_Debug("Well, known1 worked. Checking KnowNumber2...");
                        eI6Error eKnown2 = Child.Process(
                        "_6WhatsAppProcess_PreFoxStarter.KnowNumber2".GetConfig(), null, iAll);
                        if (eKnown2.IsBad())
                        {
                            // not even the know works, we are having a problem with Fox
                            iAll.iDsp.FileLog_Debug("Not even the known2 works.");
                            ret = eI6Error.FailedButNoLettingHostKnow_TelNotActive;
                        }
                        else if (eKnown2.IsMayBeGood())
                        {
                            // this should never happen, it is a known number
                            iAll.iDsp.FileLog_Error("How can a known2 return MayBe?");
                            ret = eI6Error.FailedButNoLettingHostKnow_TelNotActive;
                        }
                        else
                        {
                            // the known2 one worked
                            iAll.iDsp.FileLog_Debug("Well, known2 worked too. Checking dest number again...");
                            eI6Error eSecondInCheck = Child.Process(destMobile, msg, iAll);
                            if (eSecondInCheck.IsGood())
                            {
                                // ok, now it worked
                                iAll.iDsp.FileLog_Debug("Now the inNumber worked too.");
                                ret = eSecondInCheck;
                            }
                            else if (eSecondInCheck.IsMayBeGood())
                            {
                                // this cant really happen
                                iAll.iDsp.FileLog_Debug("inNumber returned mayBe the second time.");
                                ret = eSecondInCheck;
                            }
                            else
                            {
                                // it all works but this number, so fail forever
                                iAll.iDsp.FileLog_Info("We fail this number forever.");
                                ret = eSecondInCheck;
                            }
                        }
                    }
                }
            }
            return ret;
        }
    }
}
