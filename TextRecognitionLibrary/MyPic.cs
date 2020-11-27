using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Configuration;

//mg small change for test Git

namespace TextRecognitionLibrary
{
    public class MyPic : IPicAccess
    {
        public enum MyPicPixelInfoEnum
        {
            e0, // white
            e1,
            e2,
            e3_mid,
            e4_mid,
            e5,
            e6,
            e7,
            _e_end,
            Processed,
        }
        private Size Size;
        private MyPicPixelInfoEnum[] PixelInfoArray;
        public bool Threshold_IsBlackest()
        {
            return pTreshold == MyPicPixelInfoEnum.e7;
        }
        public void Threshold_AlmostWhite()
        {
            pTreshold = MyPicPixelInfoEnum.e0;
        }
        public void Threshold_MoreWhite()
        {
            if (pTreshold > MyPicPixelInfoEnum.e0)
            {
                pTreshold--;
            }
        }
        public void Threshold_MoreBlack()
        {
            pTreshold++;
            if (pTreshold >= MyPicPixelInfoEnum._e_end)
            {
                pTreshold = MyPicPixelInfoEnum._e_end - 1;
            }
        }
        public MyPic Threshold_AlmostBlack()
        {
            pTreshold = MyPicPixelInfoEnum._e_end - 1;
            return this;
        }

        private MyPicPixelInfoEnum pTreshold;

        public static PixelInfo Invert(PixelInfo color)
        {
            switch (color)
            {
                case PixelInfo.White: return PixelInfo.Black;
                case PixelInfo.Black: return PixelInfo.White;
            }
            throw new ArgumentException("B/W");
        }

        public MyPic(Size Size)
        {
            this.Size = Size;
            this.pTreshold = MyPicPixelInfoEnum.e3_mid;
            PixelInfoArray = new MyPicPixelInfoEnum[(Size.Width * Size.Height) + 1];
        }
        public MyPic(Size Size, PixelInfo backgroundColor)
        {
            this.Size = Size;
            this.pTreshold = MyPicPixelInfoEnum.e3_mid;
            PixelInfoArray = new MyPicPixelInfoEnum[(Size.Width * Size.Height) + 1];
            SlowDown slowDown = new SlowDown();
            for (int y = 0 ; y < Size.Height ; y++)
            {
                for (int x = 0; x < Size.Width; x++ )
                {
                    slowDown.Next();
                    setPixel(x, y, backgroundColor);
                }
            }
        }
        public MyPic(Bitmap bIn, bool forBlackWhite)
        {
            this.Size = bIn.Size;
            this.pTreshold = MyPicPixelInfoEnum.e3_mid;
            PixelInfoArray = new MyPicPixelInfoEnum[(Size.Width * Size.Height) + 1];

            for (int y = 0; y < Size.Height; y++)
            {
                for (int x = 0; x < Size.Width; x++)
                {

                    Color c = bIn.GetPixel(x, y);
                    int rgbTot = (int)c.R + (int)c.G + (int)c.B;
                    float rgbF = (float)rgbTot / 255 / 3;
                    float b = c.GetBrightness();
                    MyPicPixelInfoEnum e;
                    switch (c.A)
                    {
                        case 255:
                            // no transparenty
                            e = BrightnessToEnum(rgbF);
                            PixelInfoArray[toIndex(x, y)] = e;
                            break;
                        case 0:
                            // full transparenty
                            PixelInfoArray[toIndex(x, y)] = MyPicPixelInfoEnum.e7;
                            break;
                        default:
                            throw new ArgumentException("Alpha");
                    }
                }
            }
        }

        public MyPic(Bitmap bIn)
        {
            this.Size = bIn.Size;
            this.pTreshold = MyPicPixelInfoEnum.e3_mid;
            this.PixelInfoArray = new MyPicPixelInfoEnum[(Size.Width * Size.Height) + 1];

            for (int y = 0; y < Size.Height; y++)
            {
                for (int x = 0; x < Size.Width; x++)
                {
                    float b = bIn.GetPixel(x, y).GetBrightness();
                    int max = (int)MyPic.MyPicPixelInfoEnum._e_end;
                    int curI = 0;
                    float curF = 0.0f;
                    float step = 1 / (float)max;
                    while (true)
                    {
                        if (b <= curF)
                        {
                            // set and end
                            PixelInfoArray[toIndex(x, y)] = (MyPicPixelInfoEnum)curI;
                            break;
                        }
                        curI++;
                        curF += step;
                    }
                }
            }
        }
        public MyPic(ObjectPoints objectPoints)
        {
            this.Size = objectPoints.Rect.Size;
            this.pTreshold = MyPicPixelInfoEnum.e3_mid;
            this.PixelInfoArray = new MyPicPixelInfoEnum[(Size.Width * Size.Height) + 1];

            foreach (Point p1 in objectPoints.Points)
            {
                Point p00 = new Point(p1.X - objectPoints.XLeft, p1.Y - objectPoints.YTop);
                int arrayPos = p00.X + (p00.Y * this.Size.Width);
                this.PixelInfoArray[arrayPos] = MyPicPixelInfoEnum.e7;
            }
        }
        private MyPic.MyPicPixelInfoEnum BrightnessToEnum(float brightness)
        {
            int max = (int)MyPic.MyPicPixelInfoEnum._e_end;
            int curI = 0;
            float curF = 0.0f;
            float step = 1 / (float)max;
            while (true)
            {
                if (brightness <= curF)
                {
                    return (MyPicPixelInfoEnum)curI;
                }
                curI++;
                curF += step;
            }
        }

        public IPicAccess getClone()
        {
            return getSubPic(0, 0, Size.Width, Size.Height);
        }
        public IPicAccess getSubPic(int x, int y, int w, int h)
        {
            MyPic r = new MyPic(new Size(w, h));
            r.pTreshold = pTreshold;
            for (int yl = 0; yl < r.getDimenion().Height; yl++)
            {
                for (int xl = 0; xl < r.getDimenion().Width; xl++)
                {
                    r.setPixel2(xl, yl, getPixel2(xl + x, yl + y));
                }
            }
            return r;
        }

        public void ToFile_allTreshlds(string fileNoExtension)
        {
            MyPicPixelInfoEnum backup = pTreshold;
            pTreshold = MyPicPixelInfoEnum.e0;
            while (pTreshold < MyPicPixelInfoEnum._e_end)
            {
                ToFile(fileNoExtension + pTreshold.ToString());
                pTreshold++;
            }
            pTreshold = backup;
        }

        public string ToUniqueString()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < Size.Height; y++)
            {
                for (int x = 0; x < Size.Width; x++)
                {
                    switch (getPixel(x, y))
                    {
                        case PixelInfo.White:
                            sb.Append("0");break;
                        case PixelInfo.Black:
                            sb.Append("1");break;
                        case PixelInfo.Processed:
                            sb.Append("?");break;
                    }
                }
            }
            return sb.ToString();
        }

        public void ToFile(string fileNoExtension)
        {
            Bitmap bmp = new Bitmap(Size.Width, Size.Height);
            Color cwhite = Color.FromKnownColor(KnownColor.White);
            Color cblack = Color.FromKnownColor(KnownColor.Black);
            Color cused = Color.FromKnownColor(KnownColor.Yellow);

            for (int y = 0; y < Size.Height; y++)
            {
                for (int x = 0; x < Size.Width; x++)
                {
                    switch (getPixel(x, y))
                    {
                        case PixelInfo.White:
                            bmp.SetPixel(x, y, cwhite); break;
                        case PixelInfo.Black:
                            bmp.SetPixel(x, y, cblack); break;
                        case PixelInfo.Processed:
                            bmp.SetPixel(x, y, cused); break;
                    }
                }
            }
            bmp.Save(fileNoExtension + ".bmp");
            bmp.Dispose();
            bmp = null;
        }

        private int toIndex(int x, int y)
        {
            if ((x < 0) || (x >= Size.Width))
            {
                throw new ArgumentException("x");
            }
            if ((y < 0) || (y >= Size.Height))
            {
                throw new ArgumentException("y");
            }
            return (y * Size.Width) + x;
        }

        public Size getDimenion()
        {
            return Size;
        }
        public PixelInfo getPixel(int x, int y)
        {
            MyPicPixelInfoEnum v = PixelInfoArray[toIndex(x, y)];
            if (v == MyPicPixelInfoEnum.Processed) return PixelInfo.Processed;
            PixelInfo r = (v <= pTreshold) ? PixelInfo.Black : PixelInfo.White;
            return r;
        }
        public MyPicPixelInfoEnum getPixel2(int x, int y)
        {
            MyPicPixelInfoEnum v = PixelInfoArray[toIndex(x, y)];
            return v;
        }
        public void setPixel2(int x, int y, MyPicPixelInfoEnum val)
        {
            PixelInfoArray[toIndex(x, y)] = val;
        }
        public void setPixel(int x, int y, PixelInfo val)
        {
            MyPicPixelInfoEnum eOldVal = PixelInfoArray[toIndex(x, y)];
            switch (val)
            {
                case PixelInfo.Black: PixelInfoArray[toIndex(x, y)] = MyPicPixelInfoEnum.e0; break;
                case PixelInfo.White: PixelInfoArray[toIndex(x, y)] = MyPicPixelInfoEnum._e_end - 1; break;
                case PixelInfo.Processed: PixelInfoArray[toIndex(x, y)] = MyPicPixelInfoEnum.Processed; break;
            }
        }

        public void ToFileIfConfigured(string ConfigId)
        {
            ToFileIfConfigured(ConfigId, ConfigId, false);
        }

        public void ToFileIfConfigured(string ConfigId, string FileName, bool blackClone)
        {
            try
            {
                string cnfItem = ConfigurationManager.AppSettings[ConfigId];
                if ((cnfItem != null) && (Boolean.Parse(cnfItem)))
                {
                    if (blackClone)
                    {
                        ((MyPic)getClone()).Threshold_AlmostBlack().ToFile(FileName);

                    }
                    else
                    {
                        ToFile(FileName);
                    }
                }
            }
            catch (Exception) {}
        }
    }

    public class SlowDown
    {
        private long count = 0;
        public void Next()
        {
            if (count++ > 500)
            {
                count = 0;
                System.Threading.Thread.Sleep(1);
            }
        }
    }
}
