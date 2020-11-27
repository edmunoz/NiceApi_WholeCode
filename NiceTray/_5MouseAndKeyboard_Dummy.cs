using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NiceTray
{
    public class _5MouseAndKeyboard_Dummy : I5_MouseAndKeyboard
    {
        Point curPos = new Point();
        public void MouseToPoint(Point p)
        {
            curPos = p;
        }
        public Point CurrentMousePos()
        {
            return curPos;
        }
        public void MouseClickAndWait(int delayPre, int delayPos, Ix ix)
        {
            ix.iDsp.Delay(delayPre);
            ix.iDsp.FileLog_Debug(String.Format("MouseClick @ {0}", curPos));
            ix.iDsp.Delay(delayPos);
        }
        public void KeyBoardKeyAndWait(char c, int delayPos, I2_InfoDisplay i2)
        {
            i2.Delay(delayPos);
        }

    }
}
