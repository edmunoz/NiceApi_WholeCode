using NiceApiLibrary_low;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceApiLibrary.ASP_AppCode
{

    /// <summary>
    /// Summary description for ScreenShot
    /// </summary>
    public class DataFile_ScreenShot : DataFile_Base, IDataFile_User
    {
        public Image imgScreen;

        public DataFile_ScreenShot(NiceSystemInfo niceSystem, DataFile_Base.OpenType openType)
            : base(niceSystem, openType)
        { }

        public override void NetIniMembers()
        {
        }

        public override void NetFrom(BinaryReader br)
        {
            if (br.BaseStream.Length != 0)
            {
                imgScreen = Bitmap.FromStream(br.BaseStream);
            }
        }

        public override void NetTo(BinaryWriter bw)
        {
            if (imgScreen != null)
            {
                imgScreen.Save(bw.BaseStream, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public override string GetFullPath()
        {
            return FolderNames.GetFolder(niceSystem, MyFolders.ASP_ServerStateFolder) + "\\" + GetFileName();
        }
        public override string GetFileName()
        {
            return "ScreenShot.png";
        }

        public static void Update(NiceSystemInfo niceSystem, string b64In)
        {
            byte[] baImg = Convert.FromBase64String(b64In);
            MemoryStream msImg = new MemoryStream(baImg);
            using (DataFile_ScreenShot ss = new DataFile_ScreenShot(niceSystem, OpenType.ForUpdate_CreateIfNotThere))
            {
                ss.imgScreen = Bitmap.FromStream(msImg);
            }
        }
    }
}

