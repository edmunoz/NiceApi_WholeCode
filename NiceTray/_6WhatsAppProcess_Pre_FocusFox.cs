using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

using NiceApiLibrary_low;


namespace NiceTray
{
    class _6WhatsAppProcess_Pre_FocusFox : _6WhatsAppProcess_Pre_Base
    {
        private bool LogActiveWindowTitle;
        private bool LogAllWindows;
        public _6WhatsAppProcess_Pre_FocusFox()
        {
            LogActiveWindowTitle = "_6WhatsAppProcess_Pre_FocusFox.LogActiveWindowTitle".IsAppSettingsTrue();
            LogAllWindows = "_6WhatsAppProcess_Pre_FocusFox.LogAllWindows".IsAppSettingsTrue();
        }
        public override eI6Error Process(string destMobile, string msg, Ix iAll)
        {
            using (var x = new LogPreText("FocusFox", iAll))
            {
                try
                {
                    IDictionary<IntPtr, string> wins = win32DLL_native._GetOpenWindows();
                    string active = win32DLL_native._GetActiveWindowTitle();

                    if (LogActiveWindowTitle)
                    {
                        iAll.iDsp.FileLog_Info($"Active: {active}");
                    }
                    if (LogAllWindows)
                    {
                        iAll.iDsp.FileLog_Info($"Wins.Count: {wins.Count}");
                        foreach (KeyValuePair<IntPtr, string> win in wins)
                        {
                            iAll.iDsp.FileLog_Info($"Wins: {win.Value}");
                        }
                    }
                    if (!active.EndsWith("- Mozilla Firefox"))
                    {
                        iAll.iDsp.FileLog_Info($"Bad - Needs refocue");
                        KeyValuePair<IntPtr, string> fox = wins.FirstOrDefault(w => w.Value.EndsWith("- Mozilla Firefox"));
                        iAll.iDsp.FileLog_Info($"Bad - Refocus {fox.Value}");
                        win32DLL_native.user32_SetForegroundWindow(fox.Key);
                    }
                    else
                    {
                        iAll.iDsp.FileLog_Info("Ok");
                    }

                }
                catch { }

                return Child.Process(destMobile, msg, iAll);
            }
        }
    }
}
