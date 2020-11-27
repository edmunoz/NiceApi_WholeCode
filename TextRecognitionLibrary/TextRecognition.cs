using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing;

namespace TextRecognitionLibrary
{
    public enum PixelInfo
    {
        White,
        Black,
        Processed,
    }
    public interface IPicAccess
    {
        Size getDimenion();
        PixelInfo getPixel(int x, int y);
        void setPixel(int x, int y, PixelInfo val);
        void ToFile(string file);
        IPicAccess getSubPic(int x, int y, int w, int h);
        IPicAccess getClone();        
    }
    public static class TextRecognition
    {
        private static bool areWhite(IPicAccess pic, string str)
        {
            string[] sa = str.Split(new char[] { '.' });
            for (int i = 0; i < sa.Length; i+=2 )
            {
                int x = int.Parse(sa[i]);
                int y = int.Parse(sa[i+1]);
                if (pic.getPixel(x, y) != PixelInfo.White)
                {
                    return false;
                }
            }
            return true;
        }
        private static bool s_Initialised = false;
        private static void doInitialisation_TextRecognition()
        {
            if (!s_Initialised)
            {
                s_Initialised = true;
                using (Bitmap bmpRef = new Bitmap(200, 20))
                {
                    using (Graphics gRef = Graphics.FromImage(bmpRef))
                    {
                        gRef.DrawString(
                            "0123456789",
                            new Font("Courier New", 14.0f),
//                            new Font(SystemFonts.GetFontByName("Courier New"), 14.0f),
//                            SystemFonts.GetFontByName("Courier New"),
//                            new Font("Arial Bold", 12.0f),
                            new SolidBrush(Color.Black),// FromArgb(0x00, 0x96, 0x88)),
                            new Point(0, 0));
                    }
                    bmpRef.Save("TextRecognition.doInitialisation_TextRecognition.bmp");
                }
            }
        }
        public static char RecogniseChar(IPicAccess pic)
        {
            pic.ToFile("RecogniseChar");
            Size pSize = pic.getDimenion();
            Size sBig = new Size(7, 12);
            Size sSmall4 = new Size(4, 12);
            Size sSmall5 = new Size(5, 12);
            doInitialisation_TextRecognition();
            if ((pSize == sSmall4) || (pSize == sSmall5))
            {
                charQuater q2 = new charQuater(pic.getSubPic(0, 0, 4, 6));
                charQuater q3 = new charQuater(pic.getSubPic(0, 6, 4, 6));
                string q23 = q2.ToString() + q3.ToString();
                if (q23 == "?BWWB?BWWB")
                {
                    return '1';
                }
            }
            else if (pSize == sBig)
            {
                IPicAccess p1 = pic.getSubPic(3, 0, 4, 6);
                charQuater q1 = new charQuater(p1);

                IPicAccess p2 = pic.getSubPic(0, 0, 4, 6);
                charQuater q2 = new charQuater(p2);

                IPicAccess p3 = pic.getSubPic(0, 6, 4, 6);
                charQuater q3 = new charQuater(p3);

                IPicAccess p4 = pic.getSubPic(3, 6, 4, 6);
                charQuater q4 = new charQuater(p4);

                string q14 = q1.ToString() + q2.ToString() + q3.ToString() + q4.ToString();

                Dictionary<string, char> charDir = new Dictionary<string,char>();
                charDir.Add("\\WBWB/BWBW\\WBWB/BWBW", '0');
                charDir.Add("\\WBWB?BWWW?WWBB?WWBB", '2');
                charDir.Add("?WBWW?BWWW?WWBB?WWBB", '2');
                charDir.Add("\\WBWB?BWWW/BWBB?WBBB", '2');
                charDir.Add("?WBBW?BWWB?WWWB/BWBW", '3');
                charDir.Add("/BWBB?WWWB?WWWW?BWWB", '4');
                charDir.Add("?BBWW/BWBW?WWWB/BWBW", '5');
                charDir.Add("?BBBW?BWWB?WBWB/BWBW", '5');
                charDir.Add("?BBBB?BWWB?WBWB/BWBW", '5');
                charDir.Add("?WBWB/BWBW\\WBWB/BWBW", '6');
                charDir.Add("?WBBB/BWBB\\WBWB/BWBW", '6');
                charDir.Add("?WBBW/BWBB\\WBWB/BWBW", '6');
                charDir.Add("/BBBW?BBWB?BWWW?WBWW", '7');
                charDir.Add("?WBBW?BWWB\\WBWB?WWBW", '8');
                charDir.Add("?WBBW?BWWB?BWWB?WBBW", '8');
                charDir.Add("?WBBW?BWWB?WWWB?WWBW", '8');
                charDir.Add("\\WBWB/BWBW?WBWB/BWBW", '9');
                charDir.Add("\\WBWB/BWBW?BBWB/BBBW", '9');
                charDir.Add("\\WBWB/BWBW?BWWB/BBBW", '9');
                charDir.Add("\\WBWB/BWBW?WWWB/BWBW", '9');

                charDir.Add("\\WBWB/BBBW?WBBW?BWWW", 'p');

                if (charDir.ContainsKey(q14))
                {
                    return charDir[q14];
                }

                if (q14 == "?WWWW?WWWW?WWWW?WWWW")
                {
                    // no pattern, all corner while
                    if (areWhite(pic, "0.9.1.9.2.9.3.9.3.10.3.11"))
                    {
                        return '4';
                    }
                }

                try
                {
                    pic.getClone().ToFile(
                        "CharUnknown_"
                        + q14
                        .Replace("?", "_F_")
                        .Replace("/", "_SF_")
                        .Replace("\\", "_SB_"));
                }
                catch (SystemException)
                {

                }


                ObjectDetection test = new ObjectDetection(pic, false, true);
                int count = 0;
                test.DoDetection(new dOnDetectObject(delegate(ObjectPoints points, IPicAccess originalPic)
                    {
                        return true;
                    }), false, 0);
                //test.BiggerstWhite.Rect.Height > 
                test = null;
                

            }
            
            return '?';

        }
    }

    class charQuater
    {
        private Point topLeft = new Point(0, 0);
        private Point topRight = new Point(3, 0);
        private Point bottomLeft = new Point(0, 5);
        private Point bottomRight = new Point(3, 5);

        public bool topLeftToBottomRight;
        public bool topRightToBottomLeft;
        public string cornerStr;

        public override string ToString()
        {
            if ((topLeftToBottomRight) && (topRightToBottomLeft))
            {
                return "X" + cornerStr;
            }
            if (topLeftToBottomRight)
            {
                return "\\" + cornerStr;
            }
            if (topRightToBottomLeft)
            {
                return "/" + cornerStr;
            }
            return "?" + cornerStr;
        }

        private char getPixelChar(IPicAccess pic4x6, int x, int y)
        {
            switch (pic4x6.getPixel(x, y))
            {
                case PixelInfo.White: return 'W';
                case PixelInfo.Black: return 'B';
                case PixelInfo.Processed: return '_';
            }
            return '?';
        }
        public charQuater(IPicAccess pic4x6)
        {
            if ((pic4x6.getDimenion() != new Size(4, 6)))
            {
                throw new ArgumentException("not 4x6");
            }
            pic4x6.ToFile("pic4x6");

            cornerStr = "";
            cornerStr += getPixelChar(pic4x6, 3, 0);
            cornerStr += getPixelChar(pic4x6, 0, 0);
            cornerStr += getPixelChar(pic4x6, 0, 5);
            cornerStr += getPixelChar(pic4x6, 3, 5);

            ObjectDetection det = new ObjectDetection(pic4x6, false, true);
            det.DoDetection(new dOnDetectObject(delegate(ObjectPoints points, IPicAccess originalPic)
                {
                    if (points.Type == PixelInfo.Black)
                    {
                        if (points.Points.Contains(topLeft) && points.Points.Contains(bottomRight))
                        {
                            topLeftToBottomRight = true;
                        }
                        if (points.Points.Contains(topRight) && points.Points.Contains(bottomLeft))
                        {
                            topRightToBottomLeft = true;
                        }
                    }
                    return true;
                }), false, 0);
        }
    }

}
