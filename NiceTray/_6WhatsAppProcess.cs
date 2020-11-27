using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NiceTray
{
    public enum eI6Error
    {
        _notSet,
        _notSetBufOkSoFar,
        Success,
        FailedButNoLettingHostKnow_TelNotActive,
        FirefoxrestartFailed,

        Step1Failed,
        Step2Failed,
        Step3Failed,
        Step4Failed,

        MouseMoved,
        ScreenCaptureFailed,

        _end
    }

    public interface I6_WhatsAppProcess
    {
        void SetUp(I6_WhatsAppProcess child);
        eI6Error Process(string destMobile, string msg, Ix iAll);
        void Debug_AmendProcessId(int newProcessId);
        string Debug_AmendUrl(string suggestedUrl, string additionalInfo);

    }
}
