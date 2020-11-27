using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TextRecognitionLibrary
{
    public class PrepareImage
    {
        public static IPicAccess UseImage(string fullPath)
        {
            Bitmap b = (Bitmap)Bitmap.FromFile(fullPath);
            MyPic p = new MyPic(b);
            return p;
        }
    }
}
