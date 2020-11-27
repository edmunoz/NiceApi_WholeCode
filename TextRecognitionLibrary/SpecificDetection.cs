using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using System.Configuration;

namespace TextRecognitionLibrary
{
    public class SpecificResult_Text
    {
        public string Text;
        public Point TextPos;

        public override string ToString()
        {
            return String.Format("{0} @{1}", Text, TextPos.ToString());
        }
    }
    public class SpecificResult_SearchGlas
    {
        public Point SearchGlasPos;
        public bool SearchGlasInFactIsABack;
        public Point XPos;
    }
    public class specificResult_SmileAndMic
    {
        public Point SmilePos;
        public Point MicPos;
    }
    public class SpecificResult_InfoForBrowserRestart
    {
        public Point ChatBox;
        public Point CloseX;
        public Point RestartFox;

        public override string ToString()
        {
            return String.Format("ChatBox: {0} CloseX:{1} RestartFox: {2}",
                ChatBox.ToString(),
                CloseX.ToString(),
                RestartFox.ToString());
        }
    }

    public class SpecificDetection
    {
        class HashWithSize
        {
            public string Hash;
            public Size Size;
            public HashWithSize(string hash, int width, int height)
            {
                this.Hash = hash;
                this.Size = new Size(width, height);
            }
        }
        public static specificResult_SmileAndMic SpecificResult_SmileAndMic(MyPic mPic, bool debug = false, int minSize = 0)
        {
            List<HashWithSize> SmileList = new List<HashWithSize>(){
                new HashWithSize("0000011111111000000011111111111000001111111111111100111111111111111001111111111111111111110011110011111111100111100111111111001111001111111111111111111111111111111111111111100011111100011111011111111110111111001111111011101110000000000111011111000000111110011111111111111000011111111111100", 18, 18),
                new HashWithSize("0000001111110000000001111111111000001111111111111000011111111111111001111111111111111011110001110001111111000011000011111111000111000111111110011110011111111111111111111111100000000000011111000000000000111110000000000001101110000000000111001110000000011100011111000011111000001111111111000", 18, 18),
                new HashWithSize("0000011111110000000001111111111000001111111111111000011111111111111001111111111111111111110011110011111111100011100011111111000111000111111111011111011111111111111111111111100000000000011111000000000000111111000000000001101110000000000111011110000000011110011111100111111000011111111111100", 18, 18),
                new HashWithSize("0000000111110000000000001111111111000000011111111111110000011111111111111100001111111111111111001111100111100111110111100001110001111111110000110000111111111000111100011111111110011111011111111111111111111111111100000000000000111110000000000000011011100000000000011101110000000000001110011100000000001110001111100000011111000011111111111111000000011111111110000", 20, 20),
                new HashWithSize("0000000111110000000000001111111111000000001111111111110000001111111111111100001111111111111111001111100111100011110111100001110001111011110000110000111111111000011100011111111110011110011111111111111111111111111100000000000000111110000000000000011011100000000000011101110000000000001110011100000000001110000111100000011110000001111111111110000000011111111110000", 20, 20)
            };
            List<HashWithSize> MicList = new List<HashWithSize>(){
                new HashWithSize("01111111111111111111111111111111111111111111111111111111111111111", 6, 14),
                new HashWithSize("0111110111111111111111111111111111111111111111111111111111111111111111111111111111110111111", 8, 14),
                new HashWithSize("01000000000000011110000000000001011000000000001101100000000000110111000000000111001111000000111000011111111111000000011111111000000000011100000000000001110000000000000111000000", 17, 12),
                new HashWithSize("111000000000000111100000000000111110000000000011011100000000001101111000000001110011110000011110000111111111110000001111111110000000000111000000000000011100000000000001110000000000000111000000", 17, 13),
                new HashWithSize("110000000000000111100000000000111110000000000011011100000000001101110000000001110011110000011110000111111111110000001111111110000000000111000000000000011100000000000001110000000000000111000000", 17, 13)
            };

            specificResult_SmileAndMic r = new specificResult_SmileAndMic();

            MyPic mClone = (MyPic)mPic.getClone();
            mClone.Threshold_AlmostBlack();
            mClone.ToFile("Smile");

            ObjectDetection od = new ObjectDetection(mClone, true, true);
            int i = 0;
            bool test_DumpAllObjects = false; // mg needs to be false

            od.DoDetection(new dOnDetectObject(delegate(ObjectPoints points, IPicAccess originalPic)
            {
                #region test
                if (test_DumpAllObjects)
                {
                    if (points.Rect.Height < 100)
                    {
                        i++;
                        string path = "smile_object" + i.ToString();

                        System.IO.File.WriteAllText(path + ".txt", 
                            points.CalcHashString() + "\r\n" + points.Rect.ToString());
                        ObjectDetection
                            .ObjectToPic_RegionOfInterest(points, originalPic)
                            .ToFile(path);

                    }
                }
                // test s
                //if (points.Rect.Height > 9)
                //{
                //    if (i == 46)
                //    {
                //        string mic = points.CalcHashString();

                //    }
                //    if (i == 47)
                //    {
                //        string smil = points.CalcHashString();

                //    }
                //    ObjectDetection.ObjectToPic_RegionOfInterest(points, originalPic).ToFile("smile_" + i++.ToString());
                //}
                // test e
                #endregion

                int start = 1;

                foreach (var smile1 in SmileList)
                {
                    if ((smile1.Size == points.Rect.Size) && (smile1.Hash == points.CalcHashString()))
                    {
                        r.SmilePos = points.Rect.Location;
                        r.SmilePos.Offset(points.Rect.Width / 2, points.Rect.Height / 2);
                    }
                }
                foreach (var mic1 in MicList)
                {
                    if ((mic1.Size == points.Rect.Size) && (mic1.Hash == points.CalcHashString()))
                    {
                        r.MicPos = points.Rect.Location;
                        r.MicPos.Offset(points.Rect.Width / 2, points.Rect.Height / 2);
                    }
                }

                //if (
                //    ((points.Rect.Size == refSmileSize1) && (refSmileHash1 == points.CalcHashString())) ||
                //    ((points.Rect.Size == refSmileSize2) && (refSmileHash2 == points.CalcHashString())) ||
                //    ((points.Rect.Size == refSmileSize3) && (refSmileHash3 == points.CalcHashString())) ||
                //    ((points.Rect.Size == refSmileSize4) && (refSmileHash4 == points.CalcHashString()))
                //    )
                //{
                //    r.SmilePos = points.Rect.Location;
                //    r.SmilePos.Offset(points.Rect.Width / 2, points.Rect.Height / 2);
                //}
                //if (
                //    ((points.Rect.Size == refMicSize1) && (points.CalcHashString() == refMicHash1)) ||
                //    ((points.Rect.Size == refMicSize2) && (points.CalcHashString() == refMicHash2)) ||
                //    ((points.Rect.Size == refMicSize3) && (points.CalcHashString() == refMicHash3))
                //    )
                //{
                //    r.MicPos = points.Rect.Location;
                //    r.MicPos.Offset(points.Rect.Width / 2, points.Rect.Height / 2);
                //}

                if ((r.MicPos.IsEmpty) || (r.SmilePos.IsEmpty))
                {
                    return true;
                }
                return false;
            }), debug, minSize);
            return r;
        }

        private static Point detectOneObject(ObjectDetection detEngine, Rectangle rOffest, string hashString, Size objectSize)
        {
            Point pOut = new Point();
            detEngine.DoDetection(new dOnDetectObject(delegate(ObjectPoints points, IPicAccess originalPic)
            {
                if (
                    ((points.Rect.Size == objectSize) && (hashString == points.CalcHashString()))
                    )
                {
                    // found it
                    pOut = points.Rect.Location;
                    pOut.Offset(points.Rect.Width / 2, points.Rect.Height / 2);
                    pOut.Offset(rOffest.Left, rOffest.Top);
                    return false;
                }
                return true;
            }), false, 0);
            return pOut;
        }

        public static void InfoForBrowserRestart_FindFireFoxStart(MyPic mPic, Rectangle rWindow, ref SpecificResult_InfoForBrowserRestart info)
        {
            MyPic mClone = (MyPic)mPic.getClone();
            ObjectDetection od = new ObjectDetection(mClone, true, true);

            info.RestartFox = detectOneObject(od, rWindow,
                "0011111110001100111111000000100110111000000111111000001111100000011100000000111000000001011111000010011000000100011000111",
                new Size(12, 12));
        }

        public static void InfoForBrowserRestart_FindCloseX(MyPic mPic, Rectangle rWindow, ref SpecificResult_InfoForBrowserRestart info)
        {
            MyPic mClone = (MyPic)mPic.getClone();
            mClone.Threshold_AlmostBlack();
            ObjectDetection od = new ObjectDetection(mClone, true, true);

            info.CloseX = detectOneObject(od, rWindow, 
                "0100000001111000001101110001110011101110000111110000001110000001111100001110111001110001111110000011",
                new Size(11, 11));
        }

        public static void InfoForBrowserRestart_FindChatBox(MyPic mPic, ref SpecificResult_InfoForBrowserRestart info)
        {
            MyPic mClone = (MyPic)mPic.getClone();
            mClone.Threshold_AlmostBlack();
            ObjectDetection od = new ObjectDetection(mClone, true, true);

            info.ChatBox = detectOneObject(od, new Rectangle(),
                "111111111111111111111111111111111111111111111111111111111111111111111111111110000000001111111111111111111111111111111111111111111111111111111111111110000001111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111100000000000000111000000000000000",
                new Size(19, 17));
        }

        public static void DumpDetectedObjects(MyPic mPic, bool callAlmostBlack, bool debug = false, int minSize = 0)
        {
            MyPic mClone = (MyPic)mPic.getClone();
            if (callAlmostBlack)
            {
                mClone.Threshold_AlmostBlack();
            }

            ObjectDetection od = new ObjectDetection(mClone, true, true);
            int i = 0;
            od.DoDetection(new dOnDetectObject(delegate(ObjectPoints points, IPicAccess originalPic)
            {
                if (points.Rect.Height > 9)
                {
                    if (i == 4)
                    {
                        string s4 = points.CalcHashString();
                        System.Diagnostics.Trace.WriteLine(s4);
                    }
                    ObjectDetection.ObjectToPic_RegionOfInterest(points, originalPic).ToFile("DumpDetectedObjects_" + i++.ToString());
                }
                return true;
            }), debug, minSize);
        }

        internal class ObjData
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public string Hash { get; set; }
        }

        internal static bool Contains(List<ObjData> list, ObjectPoints points)
        {
            foreach (ObjData objData in list)
            {
                if ((objData.Width == points.Rect.Width) && (objData.Height == points.Rect.Height))
                {
                    if (points.CalcHashString() == objData.Hash)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static SpecificResult_SearchGlas DetectSearchGlas(MyPic mPic, bool debug = false, int minSize = 0)
        {
            List<ObjData> SearchGlases = new List<ObjData>();
            SearchGlases.Add(new ObjData() { Width = 12, Height = 13, Hash = "000111000000110001000010000001000100000001001000000010000000000100100000001001000000110001000011000001111101000000000001000000000001" });
            SearchGlases.Add(new ObjData() { Width = 14, Height = 14, Hash = "0001111000000001111111000001100000110001100000011000110000000100011000000010001100000011000010000001100001110011100000011111101100000000000111000000000001110000000000011" });
            SearchGlases.Add(new ObjData() { Width = 14, Height = 14, Hash = "0001111100000001111111000001100001110001100000011000110000000100011000000010001100000011000111000001100001110011110000011111111100000000000111000000000001110000000000011" });
            SearchGlases.Add(new ObjData() { Width = 14, Height = 14, Hash = "0001111100000011111111000001100001110001100000011000110000001110011000000011001100000011000111000001100001111011110000011111111100000001000111000000000001110000000000011" });

            List<ObjData> Backs = new List<ObjData>();
            Backs.Add(new ObjData() { Width = 16, Height = 16, Hash = "000000011000000000000111000000000001111000000000011110000000000111100000000001111000000000011110000000000111111111111111111111111111111011110000000000001111000000000000111100000000000011110000000000001111000000000000111000000" });

            List<ObjData> Xes = new List<ObjData>();
            Xes.Add(new ObjData() { Width = 9, Height = 10, Hash = "100000001100000101100011001101100001110000011100001101100110001111000001" });
            Xes.Add(new ObjData() { Width = 10, Height = 10, Hash = "110000001111000011011100111001111110000111100000111100001111110011100111111000011" });
            Xes.Add(new ObjData() { Width = 10, Height = 10, Hash = "110000000011000001001100011000110110000011100000011100000110110001100011011000001" });


            SpecificResult_SearchGlas r = new SpecificResult_SearchGlas();
            r.SearchGlasPos = new Point();
            r.XPos = new Point();

            MyPic mClone = (MyPic)mPic.getClone();
            mClone.Threshold_AlmostBlack();
            mClone.ToFile("DetectSearchGlas");

            ObjectDetection od = new ObjectDetection(mClone, true, true);
            od.DoDetection(new dOnDetectObject(delegate(ObjectPoints points, IPicAccess originalPic)
            {
                // test s
                //if (points.Rect.Height > 9)
                //{
                //    if (i == 37)
                //    {
                //        string s37 = points.CalcHashString();
                //    }
                //    ObjectDetection.ObjectToPic_RegionOfInterest(points, originalPic).ToFile("SearchGlas_" + i++.ToString());
                //}

                //if (points.XLeft != 0) //mg to find new objects
                //{
                //    if ((points.Rect.Height > 3) && (points.Rect.Width > 3))
                //    {
                //        MyPic debug = new MyPic(points);
                //        int area = points.Rect.Width * points.Rect.Height;
                //        string hash = points.CalcHashString();
                //        string fileName = String.Format("DetectSearchGlasSub_{0}_{1}_{2}_w{3}_h{4}", points.XLeft, points.YTop, area, points.Rect.Width, points.Rect.Height);
                //        debug.ToFile(fileName);
                //        System.IO.File.WriteAllText(fileName + "hash.txt", hash);
                //        debug = null;
                //    }
                //}

                // test e

                if (Contains(SearchGlases, points))
                {
                    // found it
                    r.SearchGlasPos = points.Rect.Location;
                    r.SearchGlasPos.Offset(points.Rect.Width / 2, points.Rect.Height / 2);
                }

                if (Contains(Backs, points))
                {
                    r.SearchGlasPos = points.Rect.Location;
                    r.SearchGlasPos.Offset(points.Rect.Width / 2, points.Rect.Height / 2);
                    r.SearchGlasInFactIsABack = true;
                }

                if (Contains(Xes, points))
                {
                    r.XPos = points.Rect.Location;
                    r.XPos.Offset(points.Rect.Width / 2, points.Rect.Height / 2);
                }

                if ((!r.SearchGlasPos.IsEmpty) || (!r.XPos.IsEmpty))
                {
                    return false;
                }

                return true;
            }), debug, minSize);

            // cals common

            return r;
        }

        internal static void DebLog(bool debug, string text)
        {
            if (debug)
            {
                System.IO.File.AppendAllText("debLog.txt", text + Environment.NewLine);
            }
        }
        public static SpecificResult_Text DetectText(MyPic mPic, bool debug = false, int minSize = 0)
        {
            SpecificResult_Text r = new SpecificResult_Text();
            StringBuilder sb = new StringBuilder();
            try
            {
                DebLog(debug, "New");
                MyPic mClone = (MyPic)mPic.getClone();
                mClone.Threshold_AlmostBlack();
                bool first = true;
                mClone.ToFileIfConfigured("TextRecognitionLibrary.DetectText.SaveInput");

                ObjectDetection od = new ObjectDetection(mClone, true, true);
                int id = 0;
                od.DoDetection(new dOnDetectObject(delegate (ObjectPoints points, IPicAccess originalPic)
                {
                    id++;
                    string cnfDumpAll = ConfigurationManager.AppSettings["TextRecognitionLibrary.DetectText.DumpAll"];
                    if ((cnfDumpAll != null) && (Boolean.Parse(cnfDumpAll)))
                    {
                        MyPic picDumpAll = (MyPic)ObjectDetection.ObjectToPic_RegionOfInterest(points, originalPic);
                        picDumpAll.ToFile(id.ToString());

                    }

                    DebLog(debug, $"id{id} {points.Rect.Height}x{points.Rect.Width} soFar{sb}");

                    if (points.Rect.Height == 10)
                    {
                        MyPic pic10 = (MyPic)ObjectDetection.ObjectToPic_RegionOfInterest(points, originalPic);
                        char c = TextCourierNew10.RecogniseChar(pic10);
                        sb.Append(c);
                        if (first)
                        {
                            first = false;
                            r.TextPos = points.Points[0];
                        }
                    }
                    else if (points.Rect.Height == 11)
                    {
                        MyPic pic11 = (MyPic)ObjectDetection.ObjectToPic_RegionOfInterest(points, originalPic);
                        char c = TextCourierNew11.RecogniseChar(pic11);
                        sb.Append(c);
                        if (first)
                        {
                            first = false;
                            r.TextPos = points.Points[0];
                        }
                    }
                    else if (points.Rect.Height == 12)
                    {
                        ObjectDetection.ObjectToPic_RegionOfInterest(points, originalPic).ToFile("a12");
                        if ((points.Rect.Width == 4) || (points.Rect.Width == 5) || (points.Rect.Width == 7))
                        {
                            IPicAccess pchar = ObjectDetection.ObjectToPic_RegionOfInterest(points, originalPic);
                            IPicAccess pbu = pchar.getClone();
                            char c = TextRecognition.RecogniseChar(pchar);
                            sb.Append(c);
                            if (first)
                            {
                                first = false;
                                r.TextPos = points.Points[0];
                            }
                        }
                        else if (points.Rect.Width == 8)
                        {
                            IPicAccess pchar =
                                ObjectDetection.ObjectToPic_RegionOfInterest(points, originalPic)
                                .getSubPic(1, 0, 7, 12);
                            IPicAccess pbu = pchar.getClone();
                            char c = TextRecognition.RecogniseChar(pchar);
                            sb.Append(c);
                            if (first)
                            {
                                first = false;
                                r.TextPos = points.Points[0];
                            }
                        }
                        else
                        {
                        // could this be 2 or more chars together?
                        ObjectDetection.ObjectToPic_RegionOfInterest(points, originalPic).ToFile("subAll");
                            int x = 0;
                            while (x < points.Rect.Width)
                            {
                                IPicAccess sub = mPic.getSubPic(points.Rect.X + x, points.Rect.Y, 7, 12);
                                sub.ToFile("subtogether");
                                char c = TextRecognition.RecogniseChar(sub.getClone());
                                if (c == '?')
                                {
                                // not recognised
                                x = points.Rect.Width;
                                }
                                else
                                {
                                    x += 9;
                                    sb.Append(c);
                                    if (first)
                                    {
                                        first = false;
                                        r.TextPos = points.Points[0];
                                    }
                                }
                            }
                        }
                    }
                    return true;
                }), debug, minSize);
                r.Text = sb.ToString();
                return r;
            }
            catch (Exception ex)
            {
                DebLog(debug, "Exception");
                DebLog(debug, ex.Message);
                if (sb.ToString().Length > 0)
                {
                    r.Text = sb.ToString();
                    return r;
                }
                var err = new SpecificResult_Text();
                err.Text = "Exception: " + ex.Message;
                err.TextPos = new Point(0, 0);
                return err;
            }
        }
    }
}
