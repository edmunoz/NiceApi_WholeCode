using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NiceTray
{
    class _5MouseAndKeyboard_Real : I5_MouseAndKeyboard
    {
        public _5MouseAndKeyboard_Real(I2_InfoDisplay iDsp)
        {
            if (!iDsp.IsRealDisplay())
            {
                throw new ArgumentException("Real Mouse needs Real Display");
            }
        }

        public void MouseToPoint(Point p)
        {
            win32DLL_native.MoveCursorToPoint(p.X, p.Y);
        }

        public void MouseClickAndWait(int delayPre, int delayPos, Ix ix)
        {
            ix.iSlowdown.Slowdown(ix);
            ix.iDsp.Delay(delayPre);
            ix.iDsp.FileLog_Debug(String.Format("MouseClick @ {0}", CurrentMousePos()));
            win32DLL_native.DoMouseClick();
            ix.iSlowdown.Slowdown(ix);
            ix.iDsp.Delay(delayPos);
        }

        public Point CurrentMousePos()
        {
            Point p = win32DLL_native.GetCursorPosition();
            return p;
        }

        public void KeyBoardKeyAndWait(char c, int delayPos, I2_InfoDisplay i2)
        {
            //char? filtered = c.KeyboardTranslate();
            //if (filtered != null)
            //{
            //    _2InfoDisplay_FromApp.theForm.UserAction_SimpleKeyPress(filtered.Value);
            //}
            _2InfoDisplay_FromApp.theForm.UserAction_SimpleKeyPress(c);
        }
    }
}
