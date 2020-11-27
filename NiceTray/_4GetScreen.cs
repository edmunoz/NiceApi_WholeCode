using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TextRecognitionLibrary;

namespace NiceTray
{
    public interface I4_GetScreen
    {
        MyPicWithOriginalStream GetScreenShot(int screenId);
        void Debug_SetId(int processId, string tel);
        void Debug_ResetRetryCounter();
    }
}
