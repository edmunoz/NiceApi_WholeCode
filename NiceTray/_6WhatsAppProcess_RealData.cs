using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Configuration;

using TextRecognitionLibrary;

namespace NiceTray
{
    public class _6WhatsAppProcess_RealData
    {
        public string Msg_DestMobile;
        public string Msg_Msg;
        public Point ExpectedMousePos;
        public Dictionary<string, StoredPicInfo> ImageList;
        public int id;
        public StoredPicInfo _lastStoredImage;

        public void DumpImagesIfConfigured(string ConfigId, Ix ix)
        {
            try
            {
                if (ConfigId.IsConfiguredAndTRUE())
                {
                    DumpImages(ix);
                }
            }
            catch (Exception) { }
        }

        public void DumpImages(Ix ix)
        {
            foreach (var i1 in ImageList)
            {
                //IdAndStep s = new IdAndStep(i1.Key);
                //string fileName = String.Format("{0}_St{1}{2}_zapi_{3}.bmp", 
                //    s.ProcessId, 
                //    s.ScreenId, 
                //    s.IsSub ? "Sub" : "",
                //    Msg_DestMobile.Replace("zapi_", "").Replace("+", ""));

                ix.iDsp.FileLog_Debug("Dumping file " + i1.Value.fileName);
                if (i1.Value.isSub)
                {
                    i1.Value.thePic.myPic.getClone().ToFile(i1.Value.fileName);
                }
                else
                {
                    MemoryStream ms = i1.Value.thePic.origStream;
                    using (FileStream fs = new FileStream(i1.Value.fileName + ".bmp", FileMode.Create))
                    {
                        ms.WriteTo(fs);
                    }
                    i1.Value.thePic.origStream.Seek(0, SeekOrigin.Begin);
                }
            }
        }

        public _6WhatsAppProcess_RealData(string destMobile, string msg, int id)
        {
            this.Msg_DestMobile = destMobile;
            this.Msg_Msg = msg;
            this.id = id;
            this.ImageList = new Dictionary<string, StoredPicInfo>();
        }

        public void MouseToPoint_UpdateExpectedPos(Point p, int screenId, int clickId, I5_MouseAndKeyboard iMouse)
        {
            if (_lastStoredImage != null)
            {
                MyPic picNew = (MyPic)_lastStoredImage.thePic.myPic.getClone();
                Size s = picNew.getDimenion();
                int dim = 15;
                for (int ix = -dim; ix < dim; ix += 3)
                {
                    for (int iy = -dim; iy < dim; iy += 3)
                    {
                        int X = p.X + ix;
                        int Y = p.Y + iy;
                        if ((X > 0) && (X < s.Width) && (Y > 0) && (Y < s.Height))
                        {
                            int ixy = ix + iy;
                            PixelInfo picInf;
                            if ((ixy % 2) == 0)
                            {
                                picInf = PixelInfo.White;
                            }
                            else
                            {
                                picInf = PixelInfo.Black;
                            }
                            picNew.setPixel(X, Y, picInf);

                        }
                    }
                }
                ImageStore_ScreenStore(new MyPicWithOriginalStream(picNew), new IdAndStep(screenId, id, true, clickId));

            }
            ExpectedMousePos = p;
            iMouse.MouseToPoint(p);
            
        }

        public void ImageStore_ScreenStore(MyPicWithOriginalStream screen, IdAndStep id)
        {
            StoredPicInfo imageToAdd = new StoredPicInfo(screen, id.CreateFileName(Msg_DestMobile), id.IsSub);

            if (ImageList.ContainsKey(id.ToString()))
            {
                ImageList.Remove(id.ToString());
                ImageList.Add(id.ToString(), imageToAdd);
            }
            else
            {
                // not there yet
                ImageList.Add(id.ToString(), imageToAdd);
            }
            if (!id.IsSub)
            {
                _lastStoredImage = imageToAdd;
            }
            screen.myPic.ToFileIfConfigured("NiceTray._6WhatsAppProcess_Real.DumpScreensOnStore", imageToAdd.fileName, true);
        }
    }

    public static class _6Extensions
    {
        public static bool IsConfiguredAndTRUE(this string configId)
        {
            bool ret = false;
            string cnfItem = ConfigurationManager.AppSettings[configId];
            if ((cnfItem != null) && (Boolean.Parse(cnfItem)))
            {
                ret = true;
            }
            return ret;
        }

        public static int GetConfigInt(this string configId)
        {
            return int.Parse(configId.GetConfig());
        }

        public static float GetConfigFloat(this string configId)
        {
            return float.Parse(configId.GetConfig());
        }
        public static string GetConfig(this string configId)
        {
            string cnfItem = ConfigurationManager.AppSettings[configId];
            return cnfItem;
        }
    }

    public class IdAndStep
    {
        public int ScreenId;
        public int ProcessId;
        public bool IsSub;
        public bool IsPixelChanged;
        public int ClickId;

        public IdAndStep(int ScreenId, int ProcessId, bool IsSub, int ClickId)
        {
            this.ScreenId = ScreenId;
            this.ProcessId = ProcessId;
            this.IsSub = IsSub;
            this.IsPixelChanged = (ClickId != -1);
            this.ClickId = ClickId;
        }
        public IdAndStep(string res)
        {
            string[] sp = res.Split(new char[] {'.'});
            if (sp.Length == 2)
            {
                this.ScreenId = int.Parse(sp[0]);
                this.ProcessId = int.Parse(sp[1]);
                this.IsSub = false;
            }
            else if ((sp.Length == 3) && (sp[2] == "Sub"))
            {
                this.ScreenId = int.Parse(sp[0]);
                this.ProcessId = int.Parse(sp[1]);
                this.IsSub = true;
            }
            else if ((sp.Length == 4) && (sp[2] == "Sub"))
            {
                this.ScreenId = int.Parse(sp[0]);
                this.ProcessId = int.Parse(sp[1]);
                this.IsSub = true;
                this.IsPixelChanged = true;
                this.ClickId = int.Parse(sp[3]);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public override string ToString()
        {
            if (!IsSub)
            {
                // normal
                return String.Format("{0}.{1}", ScreenId, ProcessId);
            }
            else if (!IsPixelChanged)
            {
                // only Sub
                return String.Format("{0}.{1}.Sub", ScreenId, ProcessId);
            }
            else
            {
                // Sub and Pix
                return String.Format("{0}.{1}.Sub.{2}", ScreenId, ProcessId, ClickId);
            }
        }

        public string CreateFileName(string telNo)
        {
            string ret = "";
            telNo = telNo.Replace("zapi_", "").Replace("+", "");

            if (!IsSub)
            {
                // normal
                //1313_St1_zapi_5585981130283.bmp
                ret = String.Format("{0}_St{1}_zapi_{2}",
                    this.ProcessId, this.ScreenId, telNo);
            }
            else if (!IsPixelChanged)
            {
                // only Sub
                //1313_St1Sub_zapi_5585981130283.bmp
                ret = String.Format("{0}_St{1}Sub_zapi_{2}",
                    this.ProcessId, this.ScreenId, telNo);
            }
            else
            {
                // Sub and Pix
                //1313_St1_zapi_5585981130283_Pix.bmp
                ret = String.Format("{0}_St{1}Sub_zapi_{2}_Click{3}",
                    this.ProcessId, this.ScreenId, telNo, ClickId);
            }
            return ret;
        }
    }

    public class StoredPicInfo
    {
        public MyPicWithOriginalStream thePic;
        public string fileName;
        public bool isSub;

        public StoredPicInfo(MyPicWithOriginalStream thePic, string fileName, bool isSub)
        {
            this.thePic = thePic;
            this.fileName = fileName;
            this.isSub = isSub;
        }
    }

    public class MyPicWithOriginalStream
    {
        public MyPic myPic;
        public MemoryStream origStream;
        public MyPicWithOriginalStream(Image orig)
        {
            origStream = new MemoryStream();
            orig.Save(origStream, System.Drawing.Imaging.ImageFormat.Bmp);
            origStream.Seek(0, SeekOrigin.Begin);
            myPic = new MyPic((Bitmap)orig);
            orig.Dispose();
        }
        public MyPicWithOriginalStream(MyPic pic)
        {
            myPic = (MyPic)pic.getClone();
        }
    }
}
