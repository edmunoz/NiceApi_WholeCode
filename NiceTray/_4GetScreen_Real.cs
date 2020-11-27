using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using TextRecognitionLibrary;

namespace NiceTray
{
    public class _4GetScreen_Real : I4_GetScreen
    {
        public MyPicWithOriginalStream GetScreenShot(int screenId)
        {
            Bitmap b = ImageCapture.GetAll();
            MyPicWithOriginalStream ret = new MyPicWithOriginalStream(b);
            return ret;
        }
        public void Debug_SetId(int processId, string tel) { }
        public void Debug_ResetRetryCounter() { }
    }
}
