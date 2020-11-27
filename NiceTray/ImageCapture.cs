using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace NiceTray
{
    public class ImageCapture
    {
        private static List<bool> GetHash(Bitmap bmpSource, Size smallSize, out Bitmap bmpMin)
        {
            List<bool> lResult = new List<bool>();
            //create new image with 16x16 pixel
            bmpMin = new Bitmap(bmpSource, smallSize);
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }

        public static bool IsSame(byte[] baIs, byte[] baShould, string fileName)
        {
            MemoryStream msIs = new MemoryStream(baIs);
            MemoryStream msShould = new MemoryStream(baShould);
            Bitmap bmpIs = (Bitmap)Bitmap.FromStream(msIs);
            Bitmap bmpShould = (Bitmap)Bitmap.FromStream(msShould);
            Bitmap bmpIsmin;
            Bitmap bmpShouldmin;

            Size smallSize = new Size(200, 20);
            List<bool> iHashIs = GetHash(bmpIs, smallSize, out bmpIsmin);
            List<bool> iHashShould = GetHash(bmpShould, smallSize, out bmpShouldmin);

            //bmpa.Save("C:\\temp\\ZapZapTray_bmpa.png", System.Drawing.Imaging.ImageFormat.Png);
            //bmpb.Save("C:\\temp\\ZapZapTray_bmpb.png", System.Drawing.Imaging.ImageFormat.Png);
            bmpIsmin.Save("C:\\temp\\" + fileName + "_Is.png", System.Drawing.Imaging.ImageFormat.Png);
            bmpShouldmin.Save("C:\\temp\\" + fileName + "_Should.png", System.Drawing.Imaging.ImageFormat.Png);

            //int equalElements = iHasha.Zip(iHashb, (i, j) => i == j).Count(eq => eq);
            int equalElements = HashZip(iHashIs, iHashShould);
            int min = (int)(iHashIs.Count * 0.95);
            if (equalElements >= min)
            {
                File.WriteAllText("C:\\temp\\" + fileName + "_R.txt", "true");
                return true;
            }
            File.WriteAllText("C:\\temp\\" + fileName + "_R.txt", "false");
            return false;
        }
        
        private static int HashZip(List<bool> l1, List<bool> l2)
        {
            int r = 0;
            for (int i = 0 ; i < l1.Count ; i++)
            {
                if (l1[i] == l2[i])
                {
                    r++;
                }
            }
            return r;
        }

        public static string GetScreenInfo()
        {
            StringBuilder sb = new StringBuilder();
            int iScreen = 0;
            foreach (System.Windows.Forms.Screen s1 in System.Windows.Forms.Screen.AllScreens)
            {
                if (iScreen++ > 0)
                {
                    sb.Append(" | ");
                }
                sb.AppendFormat("{0}: X:{1} Y:{2} W:{3} H:{4}", iScreen,
                    s1.Bounds.X, s1.Bounds.Y, s1.Bounds.Width, s1.Bounds.Height);
            }
            return sb.ToString();
        }

        public static Bitmap GetAll()
        {
            Bitmap r;
            // 1) Get the screenshot
            if ((Environment.MachineName == "VAIO") && (Environment.UserName == "<YourName>") && (System.Windows.Forms.Screen.AllScreens.Length == 2))
            {
                // special case of my PC
                Rectangle s0 = System.Windows.Forms.Screen.AllScreens[0].Bounds;
                Rectangle s1 = System.Windows.Forms.Screen.AllScreens[1].Bounds;
                Rectangle rme = new Rectangle(
                    Math.Min(s0.Left, s1.Left),
                    Math.Min(s0.Top, s1.Top),
                    Math.Max(s0.Right, s1.Right),
                    Math.Max(s0.Bottom, s1.Bottom) + 500);

                r = Get(rme.X, rme.Y, rme.Width, rme.Height);
            }
            else
            {
                // any other PC
                Rectangle rec = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                r = Get(rec.X, rec.Y, rec.Width, rec.Height);
            }

            // 2) Add the click points
            //ClickConfig cnf = ClickConfig.GetConfig();
            //Data_TextAtPos tap = cnf.GetNextAction(null);
            //using (Graphics g = Graphics.FromImage(r))
            //{
            //    while (tap != null)
            //    {
            //        g.DrawRectangle(Pens.Black, tap.X - 20, tap.Y - 20, 40, 40);
            //        g.DrawString(tap.Id.ToString(), SystemFonts.MessageBoxFont, Brushes.Black, tap.X, tap.Y);
            //        // move on 
            //        tap = cnf.GetNextAction(null);
            //    }

            //    g.DrawRectangle(Pens.Black, cnf.GetImageRect());
            //}
            return r;
        }
        public static Bitmap Get(Rectangle rec)
        {
            return Get(rec.X, rec.Y, rec.Width, rec.Height);
        }
        public static Bitmap Get(int x, int y, int w, int h)
        {
            Bitmap screenshot = new Bitmap(
                w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            using (Graphics screenGraph = Graphics.FromImage(screenshot))
            {
                //
                Point upperLeftSource = new Point(x, y);
                Point upperLeftDestination = new Point(0, 0);
                Size blockRegionSize = new Size(w, h);
                CopyPixelOperation copyPixelOperation = CopyPixelOperation.SourceCopy;


                screenGraph.CopyFromScreen(
                    upperLeftSource,
                    upperLeftDestination,
                    blockRegionSize,
                    copyPixelOperation);
            }

            return screenshot;
        }
    }
}
