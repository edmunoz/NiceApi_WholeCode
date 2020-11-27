using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using TextRecognitionLibrary;
using NiceApiLibrary_low;

namespace NiceTray
{
    delegate eI6Error dCheck(int screenId, Ix ix, _6WhatsAppProcess_RealData data, ref bool increaseTimeoutOnce);

    public class _6WhatsAppProcess_Real : I6_WhatsAppProcess
    {
        private static int s_WhatsAppProcessCounter = 0;

        public void Debug_AmendProcessId(int newProcessId)
        {
            s_WhatsAppProcessCounter = newProcessId;
        }

        public string Debug_AmendUrl(string suggestedUrl, string additionalInfo)
        {
            return suggestedUrl;
        }

        public void SetUp(I6_WhatsAppProcess c) { }
        public eI6Error Process(string destMobile, string msg, Ix iAll)
        {
            using (var x = new LogPreText("Real", iAll))
            {
                return processPreLogDone(destMobile, msg, iAll);
            }
        }

        private eI6Error processPreLogDone(string destMobile, string msg, Ix iAll)
        {
            if (iAll.TypeOfProcess == Ix.eTypeOfProcess.EmptyWakeUpTrigger)
            {
                // nothing else to do
                return eI6Error.Success;
            }

            DateTime dtStart = DateTime.Now;
            eI6Error ret = eI6Error._notSet;
            _6WhatsAppProcess_RealData u = new _6WhatsAppProcess_RealData(destMobile, msg, s_WhatsAppProcessCounter++);
            u.MouseToPoint_UpdateExpectedPos(new Point(200, 200), -1, -1, iAll.iMouse);
            iAll.iDsp.FileLog_Info("Id: " + u.id.ToString() + " " + destMobile);
            iAll.iDsp.AddLine("Id: " + u.id.ToString());
            iAll.iDsp.AddLine2(new DetailedData_ProcessingId(u.id));
            iAll.iScreen.Debug_SetId(u.id, u.Msg_DestMobile);

            SpecificResult_SearchGlas _1GlasPos = null;
            Point _2TelLinePos;
            Point _4RightOfSmile;

            // Step 1
            if (ret == eI6Error._notSet)
            {
                iAll.iDsp.FileLog_Debug("Step1 findSearchWindow and enter tel");
                iAll.iDsp.AddLine("Step1 findSearchWindow and enter tel");
                ret = GetScreenAndCheckLoop(new MyTimeControl(5000), 5000, iAll, 1, u, eI6Error.Step1Failed, delegate(int screenId, Ix ix, _6WhatsAppProcess_RealData data, ref bool increaseTimeoutOnce)
                {
                    ix.iDsp.FileLog_Debug("Performing Step1...");
                    // delegate
                    MyPic pic = ScreenGet(screenId, data);

                    // 1) we search in the top left corner
                    Size wholeScreenSize = pic.getDimenion();
                    Rectangle sa = new Rectangle(0, 0, wholeScreenSize.Width / 2, wholeScreenSize.Height / 2);
                    MyPic sub = (MyPic)pic.getSubPic(sa.X, sa.Y, sa.Width, sa.Height);
                    ScreenStoreSub(sub, screenId, data);
//                    ((MyPic)sub.getClone()).Threshold_AlmostBlack().ToFile("1sub");

                    // 2) Call the Dll to search for the SearchGlas
                    _1GlasPos = TextRecognitionLibrary.SpecificDetection.DetectSearchGlas(sub);

                    // 3) Check the Dll´s result
                    if (!_1GlasPos.XPos.IsEmpty)
                    {
                        // the x is there, so text was already entered
                        ix.iDsp.FileLog_Debug("The x is there " + _1GlasPos.XPos.ToString());
                        ix.iDsp.AddLine("X detected, increasing timeout");
                        data.MouseToPoint_UpdateExpectedPos(_1GlasPos.XPos, screenId, 1, ix.iMouse);
                        ix.iMouse.MouseClickAndWait(100, 300, ix);
                        increaseTimeoutOnce = true;
                    }
                    else if (_1GlasPos.SearchGlasPos.IsEmpty)
                    {
                        // no Searchglas found
                        ix.iDsp.FileLog_Debug("SearchGlasPos.IsEmpty");
                    }
                    else if (_1GlasPos.SearchGlasInFactIsABack)
                    {
                        // the <- arror is there, so text was already entered, 
                        ix.iDsp.FileLog_Debug("<- detected at " 
                            + _1GlasPos.SearchGlasPos.ToString()
                            + " / "
                            + _1GlasPos.XPos.ToString());
                        data.MouseToPoint_UpdateExpectedPos(_1GlasPos.SearchGlasPos, screenId, 1, ix.iMouse);
                        ix.iMouse.MouseClickAndWait(100, 300, ix);
                        increaseTimeoutOnce = true;
                    }
                    else
                    {
                        // the SearchGlas is there, so click it
                        ix.iDsp.FileLog_Debug("SearchGlas @ " + _1GlasPos.SearchGlasPos.ToString());
                        data.MouseToPoint_UpdateExpectedPos(_1GlasPos.SearchGlasPos, screenId, 2, ix.iMouse);
                        ix.iDsp.Delay(300);
                        Point pWhereText = _1GlasPos.SearchGlasPos;
                        pWhereText.Offset(100, 0);
                        ix.iDsp.FileLog_Debug("pWhereText.click(" + pWhereText.ToString() + ")");
                        data.MouseToPoint_UpdateExpectedPos(pWhereText, screenId, 3, ix.iMouse);
                        ix.iMouse.MouseClickAndWait(100, 100, ix);

                        // Now enter the tel no
                        return EnterTextAndCheckForMouseMove(data.Msg_DestMobile.Replace("+", ""), false, ix, data);
                    }
                    ix.iDsp.FileLog_Debug("Early out: Reason above.");
                    ix.iDsp.Delay(200);
                    return eI6Error._notSet;
                });
            }
            if (ret == eI6Error._notSetBufOkSoFar)
            {
                ret = eI6Error._notSet;
            }

            // Step 2
            if (ret == eI6Error._notSet)
            {
                iAll.iDsp.FileLog_Debug("Step2 findAccount and Click it");
                iAll.iDsp.AddLine("Step2 findAccount and Click it");
                ret = GetScreenAndCheckLoop(new MyTimeControl(5000), 5000, iAll, 2, u, eI6Error.Step2Failed, delegate(int screenId, Ix ix, _6WhatsAppProcess_RealData data, ref bool increaseTimeoutOnce)
                {
                    ix.iDsp.FileLog_Debug("Performing Step2...");
                    // delegate
                    MyPic pic = ScreenGet(screenId, data);

                    // 1) we search in a limitted area
                    Rectangle sa = new Rectangle(_1GlasPos.SearchGlasPos.X + 40, _1GlasPos.SearchGlasPos.Y + 10, 200, 250);
                    MyPic sub = (MyPic)pic.getSubPic(sa.X, sa.Y, sa.Width, sa.Height);
                    ScreenStoreSub(sub, screenId, data);
//                    ((MyPic)sub.getClone()).Threshold_AlmostBlack().ToFile("4AlmostBlack");

                    // 2) Call the Dll to search for the text
                    SpecificResult_Text rText = SpecificDetection.DetectText(sub);
                    if (rText.TextPos.IsEmpty)
                    {
                        ix.iDsp.FileLog_Debug("Early out: TextPos.IsEmpty");
                        ix.iDsp.Delay(200);
                        return eI6Error._notSet;
                    }
                    ix.iDsp.FileLog_Info("Step2 tel text found: " + rText.Text);
                    bool first12AreTheSame;
                    if (!AreFirst12CharSame(data.Msg_DestMobile, rText.Text, out first12AreTheSame))
                    { 
                        ix.iDsp.FileLog_Debug("Early out: Text is wrong");
                        ix.iDsp.Delay(200);
                        return eI6Error._notSet;
                    }
                    if (first12AreTheSame)
                    {
                        ix.iDsp.FileLog_Debug("Text is not the same but we accept.");
                        ix.iDsp.FileLog_Debug(data.Msg_DestMobile);
                        ix.iDsp.FileLog_Debug(rText.Text);
                    }
                    _2TelLinePos = rText.TextPos;
                    _2TelLinePos.Offset(sa.X, sa.Y);
                    _2TelLinePos.Offset(0, 7);

                    ix.iDsp.FileLog_Debug("Step2 TelLine detected at " + _2TelLinePos.ToString());

                    // 3) and click on top of the tel string
                    data.MouseToPoint_UpdateExpectedPos(_2TelLinePos, screenId, 4, ix.iMouse);
                    ix.iMouse.MouseClickAndWait(100, 100, ix);

                    return eI6Error._notSetBufOkSoFar;
                });
            }
            if (ret == eI6Error._notSetBufOkSoFar)
            {
                ret = eI6Error._notSet;
            }

            // Step 3
            if (ret == eI6Error._notSet)
            {
                iAll.iDsp.FileLog_Debug("Step3 verify account string top rigth position");
                iAll.iDsp.AddLine("Step3 verify account string top rigth position");
                ret = GetScreenAndCheckLoop(new MyTimeControl(5000), 5000, iAll, 3, u, eI6Error.Step3Failed, delegate(int screenId, Ix ix, _6WhatsAppProcess_RealData data, ref bool increaseTimeoutOnce)
                {
                    ix.iDsp.FileLog_Debug("Performing Step3...");

                    // delegate
                    MyPic pic = ScreenGet(screenId, data);

                    // 1) we search in a limitted area
                    //Rectangle sa = new Rectangle(400, 100, 280, 80);
                    Rectangle sa = new Rectangle(475, 100, 200, 80);
                    MyPic sub = (MyPic)pic.getSubPic(sa.X, sa.Y, sa.Width, sa.Height);
                    ScreenStoreSub(sub, screenId, data);

                    // 2) Call the Dll to search for the text
                    SpecificResult_Text rText = SpecificDetection.DetectText(sub);
                    if (rText.TextPos.IsEmpty)
                    {
                        ix.iDsp.FileLog_Debug("Early out: TextPos.IsEmpty");
                        ix.iDsp.Delay(200);
                        return eI6Error._notSet;
                    }
                    ix.iDsp.FileLog_Info("Step3 tel text fount: " + rText.Text);
                    if (!rText.Text.Contains(data.Msg_DestMobile.Replace("zapi_", "").Replace("+", "")))
                    {
                        ix.iDsp.FileLog_Debug("Early out: Text is wrong");
                        ix.iDsp.Delay(200);
                        return eI6Error._notSet;
                    }
                    return eI6Error._notSetBufOkSoFar;
                });
            }
            if (ret == eI6Error._notSetBufOkSoFar)
            {
                ret = eI6Error._notSet;
            }

            // Step 4
            if ((ret == eI6Error._notSet) && (iAll.TypeOfProcess == Ix.eTypeOfProcess.TelNumberChecking))
            {
                // we are done for tel checking
                ret = eI6Error.Success;
            }
            if (ret == eI6Error._notSet)
            {
                iAll.iDsp.FileLog_Debug("Step4 Find type-msg-here");
                iAll.iDsp.AddLine("Step4 Find type-msg-here");
                ret = GetScreenAndCheckLoop(new MyTimeControl(5000), 5000, iAll, 4, u, eI6Error.Step4Failed, delegate(int screenId, Ix ix, _6WhatsAppProcess_RealData data, ref bool increaseTimeoutOnce)
                {
                    ix.iDsp.FileLog_Debug("Performing Step4...");

                    // delegate
                    MyPic pic = ScreenGet(screenId, data);

                    // 1) we serach in a specific area
                    Size wholeScreenSize = pic.getDimenion();
                    // was {X = 0 Y = 568 Width = 1024 Height = 200}
                    Rectangle sa = new Rectangle(
                        300,
                        wholeScreenSize.Height - 100,
                        wholeScreenSize.Width - 300,
                        100);
                    MyPic sub = (MyPic)pic.getSubPic(sa.X, sa.Y, sa.Width, sa.Height);
                    ScreenStoreSub(sub, screenId, data);
//                    ((MyPic)sub.getClone()).Threshold_AlmostBlack().ToFile("8subAlmostBlac");

                    // 2) Call the Dll to search the simle and mic
                    specificResult_SmileAndMic s = TextRecognitionLibrary.SpecificDetection.SpecificResult_SmileAndMic(sub);
                    if (s.SmilePos.IsEmpty)
                    {
                        // no smile found
                        ix.iDsp.FileLog_Debug("Early out: No smile");
                        return eI6Error._notSet;
                    }
                    else if (s.MicPos.IsEmpty)
                    {
                        // we have a smile not no mic ==> rubbishtext is there. So delete it
                        _4RightOfSmile = s.SmilePos;
                        _4RightOfSmile.Offset(sa.X, sa.Y);
                        _4RightOfSmile.Offset(300, 5);

                        data.MouseToPoint_UpdateExpectedPos(_4RightOfSmile, screenId, 5, ix.iMouse);
                        ix.iMouse.MouseClickAndWait(50, 50, ix);
                        EnterTextAndCheckForMouseMove("\b\b\b\b\b\b\b\b\b", false, ix, data);
                        ix.iDsp.FileLog_Debug("Early out: Rubbish text in chat box");
                        return eI6Error._notSet;

                    }
                    // we have both, smile and mic
                    _4RightOfSmile = s.SmilePos;
                    _4RightOfSmile.Offset((s.MicPos.X - s.SmilePos.X) / 2, 0);
                    _4RightOfSmile.Offset(sa.X, sa.Y);

                    // 3) Set the mouse to the text box
                    data.MouseToPoint_UpdateExpectedPos(_4RightOfSmile, screenId, 6, ix.iMouse);

                    // 4) Click it
                    ix.iMouse.MouseClickAndWait(50, 50, ix);

                    // 5) And enter the text
                    if (data.Msg_Msg.StartsWith("__NoSend"))
                    {
                        ix.iDsp.AddLine("__NoSend");
                        ix.iDsp.FileLog_Info("__NoSend");
                    }
                    else
                    {
                        EnterTextAndCheckForMouseMove(data.Msg_Msg, true, ix, data);
                    }
                    return eI6Error.Success;
                });
            }

            // END, clean up
            TimeSpan duration = (DateTime.Now - dtStart);
            string strDurtion = duration.ToReadableString();

            if (ret == eI6Error.Success)
            {
                // Success
                iAll.iDsp.Clear();
                iAll.iDsp.AddLine("Done in " + strDurtion);
                iAll.iDsp.AddLine2(new DetailedData_LastGoodProcess(u.id, duration));
                u.DumpImagesIfConfigured("NiceTray._6WhatsAppProcess_Real.DumpScreensOnSuccess", iAll);
            }
            else
            {
                // Problem
                iAll.iDsp.AddLine2(new DetailedData_LastBadProcess(u.id, duration));
                u.DumpImagesIfConfigured("NiceTray._6WhatsAppProcess_Real.DumpScreensOnProblem", iAll);
            }
            iAll.iDsp.FileLog_Info(
                "_6WhatsAppProcess_Real.Process took " 
                + strDurtion 
                + " "
                + (ret == eI6Error.Success ? "Success" : ret.ToString())
                );

            return ret;
        }

        private static bool AreFirst12CharSame(string telFromServer, string telFromScreen, out bool first12AreTheSame)
        {
            telFromServer = telFromServer.Replace("zapi_", "").Replace("+", "");
            first12AreTheSame = false;

            if (telFromScreen.Contains(telFromServer))
            {
                return true;
            }
            else
            {
                if ((telFromServer.Length > 12) && (telFromScreen.Length > 12))
                {
                    first12AreTheSame = true;
                    for (int i = 0; i < 12; i++)
                    {
                        if (telFromServer[i] != telFromScreen[i])
                        {
                            first12AreTheSame = false;
                        }
                    }
                    return first12AreTheSame;
                }
            }
            return false;
        }

        private eI6Error GetScreenAndCheckLoop(
            MyTimeControl timeControl,
            int timeoutMs_, 
            Ix ix, 
            int screenId, 
            _6WhatsAppProcess_RealData data, 
            eI6Error onFailErrorCode,
            dCheck check)
        {
            ix.TimeControl = timeControl;
            bool timeoutOnceIncreased = false;
            eI6Error err = eI6Error._notSet;
            ix.iScreen.Debug_ResetRetryCounter();
            while (err == eI6Error._notSet)
            {
                if (err == eI6Error._notSet)
                {
                    err = CheckMouseMoved(ix, data);
                }
                if (err == eI6Error._notSet)
                {
                    err = GetAndSaveScreen(ix, screenId, data);
                }
                if (err == eI6Error._notSet)
                {
                    bool increaseTimeoutOnce = false;
                    err = check(screenId, ix, data, ref increaseTimeoutOnce);
                    if ((!timeoutOnceIncreased) && (increaseTimeoutOnce))
                    {
                        timeoutOnceIncreased = true;
                        timeControl.AddSeconds(10);
                    }
                }
                if (err == eI6Error._notSet)
                {
                    err = CheckMouseMoved(ix, data);
                }
                if (err == eI6Error._notSet)
                {
                    if (timeControl.TimedOut)
                    {
                        return onFailErrorCode;
                    }
                }
            }
            return err;
        }

        public static eI6Error EnterTextAndCheckForMouseMove(string text, bool isMessageInput, Ix ix, _6WhatsAppProcess_RealData d)
        {
            int i = 0;
            ix.iDsp?.FileLog_Debug("Enter text at pos " + d.ExpectedMousePos.ToString());
            ix.iDsp?.FileLog_Debug("Text is: " + text.MsgForLogFileHideSpecial());
            if (isMessageInput)
            {
                // replace final \r\n and add a \0
                while (text.EndsWith("\n") || text.EndsWith("\r"))
                {
                    text = text.Substring(0, text.Length - 1);
                }
                text += "\0";

                // Filter start
                // Filter start
            }

            foreach (char c in text)
            {
                if ((d != null) && (ix.iMouse.CurrentMousePos() != d.ExpectedMousePos))
                {
                    ix?.iDsp.FileLog_Error("Mouse moved on text entry.");
                    ix?.iDsp.AddLine("Mouse moved on text entry.");
                    return eI6Error.MouseMoved;
                }
                ix.iMouse.KeyBoardKeyAndWait(c, 5, ix.iDsp);
            }
            return eI6Error._notSetBufOkSoFar;
        }


        private eI6Error CheckMouseMoved(Ix ix, _6WhatsAppProcess_RealData data)
        {
            try
            {
                if (ix.iMouse.CurrentMousePos() == data.ExpectedMousePos)
                {
                    // ok
                    return eI6Error._notSet;
                }
            }
            catch (SystemException)
            {
            }
            ix.iDsp.AddLine("Mouse moved");
            ix.iDsp.FileLog_Info("Mouse moved");
            return eI6Error.MouseMoved;
        }

        private MyPic ScreenGet(int screenId, _6WhatsAppProcess_RealData data)
        {
            return data.ImageList[new IdAndStep(screenId, data.id, false, -1).ToString()].thePic.myPic;
        }

        private void ScreenStoreSub(MyPic myPic, int screenId, _6WhatsAppProcess_RealData data)
        {
            data.ImageStore_ScreenStore(new MyPicWithOriginalStream(myPic), new IdAndStep(screenId, data.id, true, -1));
        }

        private void ScreenStore(MyPicWithOriginalStream screen, int screenId, _6WhatsAppProcess_RealData data)
        {
            data.ImageStore_ScreenStore(screen, new IdAndStep(screenId, data.id, false, -1));
        }

        private eI6Error GetAndSaveScreen(Ix ix, int screenId, _6WhatsAppProcess_RealData data)
        {
            try
            {
                ix.iDsp.FileLog_Debug("GetAndSaveScreen " + screenId.ToString());
                ScreenStore(ix.iScreen.GetScreenShot(screenId), screenId, data);
            }
            catch (SystemException ex)
            {
                ix.iDsp.FileLog_Error("GetScreenShot failed");
                ix.iDsp.AddLine("GetScreenShot failed");
                return eI6Error.ScreenCaptureFailed;
            }
            return eI6Error._notSet;
        }
    }

    public class MyTimeControl
    {
        DateTime Start;
        DateTime Max;
        public bool TimedOut { get { return DateTime.Now > Max ; } }
        public MyTimeControl(int msTimeout)
        {
            Start = DateTime.Now;
            Max = Start.AddMilliseconds(msTimeout);
        }
        public void AddSeconds(int i)
        {
            Max = Max.AddSeconds(i);
        }
        public void AddMilliSeconds(int i)
        {
            Max = Max.AddMilliseconds(i);
        }

    }
    public class Ix
    {
        public enum eTypeOfProcess
        {
            Normal,
            TelNumberChecking,
            EmptyWakeUpTrigger,
        }

        public I2_InfoDisplay iDsp;
        public I4_GetScreen iScreen;
        public I5_MouseAndKeyboard iMouse;
        public I6_WhatsAppProcess iWhatsApp;
        public I8_UpdateCommunicator iUpdater;
        public I9_CPUSlowdown iSlowdown;
        public eTypeOfProcess TypeOfProcess;
        public MyTimeControl TimeControl;

        public Ix() { }
        public Ix(
            I2_InfoDisplay iDsp, 
            I4_GetScreen iScreen, 
            I5_MouseAndKeyboard iMouse, 
            I6_WhatsAppProcess iWhatsApp, 
            I8_UpdateCommunicator iUpdater,
            I9_CPUSlowdown iSlowdown
            )
        {
            this.iDsp = iDsp;
            this.iScreen = iScreen;
            this.iMouse = iMouse;
            this.iWhatsApp = iWhatsApp;
            this.iUpdater = iUpdater;
            this.iSlowdown = iSlowdown;
        }
    }
}
