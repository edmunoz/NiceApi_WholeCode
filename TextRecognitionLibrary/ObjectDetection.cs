using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TextRecognitionLibrary
{
    public delegate bool dOnDetectObject(ObjectPoints o, IPicAccess originalPic);
    public class ObjectPoints
    {
        public PixelInfo Type;
        public List<Point> Points;
        public int XLeft = int.MaxValue;
        public int XRight = int.MinValue;
        public int YTop = int.MaxValue;
        public int YBottom = int.MinValue;
        public Rectangle Rect;

        public ObjectPoints(PixelInfo Type)
        {
            this.Type = Type;
            this.Points = new List<Point>();
        }
        public void Add(int x, int y)
        {
            Points.Add(new Point(x, y));
        }
        public void DoneAdding()
        {
            // rect
            foreach (Point p1 in Points)
            {
                XLeft = Math.Min(XLeft, p1.X);
                XRight = Math.Max(XRight, p1.X);
                YTop = Math.Min(YTop, p1.Y);
                YBottom = Math.Max(YBottom, p1.Y);
            }
            Rect = new Rectangle(XLeft, YTop, XRight - XLeft + 1, YBottom - YTop + 1);
        }

        public string CalcHashString()
        {
            StringBuilder sbHash = new StringBuilder();
            for (int y = YTop ; y < YBottom ; y++)
            {
                for (int x = XLeft ; x < XRight ; x++)
                {
                    sbHash.Append(
                        (Points.Contains(new Point(x, y))
                        ? "1" : "0"));
                }
            }
            return sbHash.ToString();
        }
    }

    public class ObjectDetection
    {
        public List<ObjectPoints> WhiteObjectList;
        public List<ObjectPoints> BlackObjectList;
        public IPicAccess Pic;
        public bool TryCrossOnWhite;
        public bool TryCrossOnBlack;

        public ObjectPoints BiggerstWhite
        {
            get
            {
                ObjectPoints r = null;
                foreach (ObjectPoints ps1 in WhiteObjectList)
                {
                    if (r == null)
                    {
                        r = ps1;
                    }
                    else
                    {
                        if (ps1.Points.Count > r.Points.Count)
                        {
                            r = ps1;
                        }
                    }
                }
                return r;
            }

        }

        public ObjectDetection(IPicAccess pic, bool tryCrossOnWhite, bool tryCrossOnBlack)
        {
            WhiteObjectList = new List<ObjectPoints>();
            BlackObjectList = new List<ObjectPoints>();
            this.Pic = pic;
            this.TryCrossOnWhite = tryCrossOnWhite;
            this.TryCrossOnBlack = tryCrossOnBlack;
        }

        public static IPicAccess ObjectToPic(ObjectPoints o, IPicAccess originalPic)
        {
            MyPic pic = new MyPic(originalPic.getDimenion(), MyPic.Invert(o.Type));
            foreach (Point p1 in o.Points)
            {
                pic.setPixel(p1.X, p1.Y, o.Type);
            }
            return pic;
        }

        public static IPicAccess ObjectToPic_RegionOfInterest(ObjectPoints o, IPicAccess originalPic)
        {
            MyPic pic = new MyPic(o.Rect.Size, MyPic.Invert(o.Type));
            foreach (Point p1 in o.Points)
            {
                pic.setPixel(p1.X - o.XLeft, p1.Y - o.YTop, o.Type); //mg was  PixelInfo.Black);
            }
            return pic;
        }

        private static int onDebugCount = 0;
        private void onDebug(bool debug, int minSize, ObjectPoints points)
        {
            if (debug)
            {
                if ((points.Rect.Height >= minSize) && (points.Rect.Width >= minSize))
                {
                    MyPic debPic = new MyPic(points);
                    int area = points.Rect.Width * points.Rect.Height;
                    string hash = points.CalcHashString();
                    string fileName = String.Format("debPic_{0}_{1}_{2}_w{3}_h{4}_{5}", 
                        points.XLeft, points.YTop, area, points.Rect.Width, points.Rect.Height, onDebugCount++);
                    debPic.ToFile(fileName);
                    System.IO.File.WriteAllText(fileName + "hash.txt", hash);
                    debPic = null;
                }
            }
        }

        public void DoDetection(dOnDetectObject cb, bool debug, int minSize)
        {
            bool stayInLoop = true;
            #region theLoop
            SlowDown slowdown = new SlowDown();
            for (int y = 0; ((stayInLoop) && (y < Pic.getDimenion().Height)); y++)
            {
                for (int x = 0; ((stayInLoop) && (x < Pic.getDimenion().Width)); x++)
                {
                    slowdown.Next();

                    PixelInfo i = Pic.getPixel(x, y);
                    switch (i)
                    {
                        case PixelInfo.White:
                            ObjectPoints whites = new ObjectPoints(i);
                            onPoint_AddInvalidateExpand(whites, x, y);
                            whites.DoneAdding();
                            WhiteObjectList.Add(whites);
                            onDebug(debug, minSize, whites);
                            if (!cb(whites, Pic))
                            {
                                stayInLoop = false;
                            }
                            break;

                        case PixelInfo.Black:
                            ObjectPoints black = new ObjectPoints(i);
                            onPoint_AddInvalidateExpand(black, x, y);
                            black.DoneAdding();
                            BlackObjectList.Add(black);
                            onDebug(debug, minSize, black);
                            if (!cb(black, Pic))
                            {
                                stayInLoop = false;
                            }
                            break;

                        case PixelInfo.Processed:
                            break;

                        default:
                            break;
                    }
                }
            }
            #endregion
        }

        private void onPoint_AddInvalidateExpand(ObjectPoints curHole, int x, int y)
        {
            curHole.Add(x, y);
            invalidate(x, y);

            List<Point> PointsToTry = new List<Point>();
            addPotentialPoints(PointsToTry, x, y, curHole.Type);
            while (PointsToTry.Count > 0)
            {
                Point pTry = PointsToTry[0];
                PointsToTry.RemoveAt(0);
                if (curHole.Type == Pic.getPixel(pTry.X, pTry.Y))
                {
                    curHole.Add(pTry.X, pTry.Y);
                    invalidate(pTry.X, pTry.Y);
                    addPotentialPoints(PointsToTry, pTry.X, pTry.Y, curHole.Type);
                }
            }
        }
        private void addPotentialPoints(List<Point> PointsToTry, int x, int y, PixelInfo type)
        {
            bool tryCross = type == PixelInfo.White ? TryCrossOnWhite : TryCrossOnBlack;
            if (tryCross)
            {
                addPotentialPoints_8(PointsToTry, x, y);
            }
            else
            {
                addPotentialPoints_4(PointsToTry, x, y);
            }
        }

        private void addPotentialPoints_4(List<Point> PointsToTry, int x, int y)
        {
            for (int l = 1; l < 5; l++)
            {
                switch (l)
                {
                    case 1:
                        if ((x < (Pic.getDimenion().Width) - 1)) PointsToTry.Add(new Point(x + 1, y)); break;
                    case 2:
                        if ((y < (Pic.getDimenion().Height) - 1)) PointsToTry.Add(new Point(x, y + 1)); break;
                    case 3:
                        if ((x > 0)) PointsToTry.Add(new Point(x - 1, y)); break;
                    case 4:
                        if ((y > 0)) PointsToTry.Add(new Point(x, y - 1)); break;
                }
            }
        }

        private void addPotentialPoints_8(List<Point> PointsToTry, int x, int y)
        {
            for (int l = 1; l < 9; l++)
            {
                switch (l)
                {
                    case 1:
                        if ((x < (Pic.getDimenion().Width) - 1)) PointsToTry.Add(new Point(x + 1, y)); break;
                    case 2:
                        if ((y < (Pic.getDimenion().Height) - 1)) PointsToTry.Add(new Point(x, y + 1)); break;
                    case 3:
                        if ((x > 0)) PointsToTry.Add(new Point(x - 1, y)); break;
                    case 4:
                        if ((y > 0)) PointsToTry.Add(new Point(x, y - 1)); break;

                    case 5:
                        if (((x < (Pic.getDimenion().Width) - 1)) && ((y < (Pic.getDimenion().Height) - 1)))
                            PointsToTry.Add(new Point(x + 1, y + 1)); break;
                    case 6:
                        if (((y < (Pic.getDimenion().Height) - 1)) && ((x > 0)))
                            PointsToTry.Add(new Point(x - 1, y + 1)); break;
                    case 7:
                        if (((x > 0)) && ((y > 0)))
                            PointsToTry.Add(new Point(x - 1, y - 1)); break;
                    case 8:
                        if (((x < (Pic.getDimenion().Width) - 1)) && ((y > 0)))
                            PointsToTry.Add(new Point(x + 1, y - 1)); break;
                }
            }
        }

        private void invalidate(int x, int y)
        {
            Pic.setPixel(x, y, PixelInfo.Processed);
        }
    }
}
