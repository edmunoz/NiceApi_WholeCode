using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NiceTray
{
    public interface I5_MouseAndKeyboard
    {
        void MouseToPoint(Point p);
        void MouseClickAndWait(int delayPre, int delayPos, Ix ix);
        Point CurrentMousePos();

        void KeyBoardKeyAndWait(char c, int delayPos, I2_InfoDisplay i2);
        
    }
}
