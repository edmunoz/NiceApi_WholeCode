using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public class BinBase64StreamHelper
    {
        private static BinaryReader b64StreamToTrueBinReader(Stream inStreamB64)
        {
            string inB64 = new StreamReader(inStreamB64).ReadToEnd();
            MemoryStream inStreamBin = new MemoryStream(Convert.FromBase64String(inB64));
            BinaryReader brBin = new BinaryReader(inStreamBin);
            return brBin;
        }

        public static void Tray2ASP_FromB64Stream(ref Data_Net_Tray2ASP tray2ASP, Stream inStreamB64)
        {
            BinaryReader brTrueBin = b64StreamToTrueBinReader(inStreamB64);
            tray2ASP.NetFrom(brTrueBin);
        }
        public static void ASP2Tray_FromB64Stream(ref Data_Net_ASP2Tray aps2Tray, Stream inStreamB64)
        {
            BinaryReader brTrueBin = b64StreamToTrueBinReader(inStreamB64);
            aps2Tray.NetFrom(brTrueBin);
        }

        public static void ASP2Tray_ToB64Stream(ref Data_Net_ASP2Tray toTray, Stream outStreamB64)
        {
            MemoryStream msOutBin = new MemoryStream();
            toTray.NetTo(new BinaryWriter(msOutBin));
            binStreamToB64(msOutBin, outStreamB64);
        }

        public static void Tray2ASP_ToB64Stream(ref Data_Net_Tray2ASP toASP, Stream outStreamB64)
        {
            MemoryStream msOutBin = new MemoryStream();
            toASP.NetTo(new BinaryWriter(msOutBin));
            binStreamToB64(msOutBin, outStreamB64);
        }

        private static void binStreamToB64(MemoryStream msOutBin, Stream outStreamB64)
        {
            msOutBin.Seek(0, SeekOrigin.Begin);
            string outB64 = Convert.ToBase64String(msOutBin.ToArray());
            StreamWriter outWriter = new StreamWriter(outStreamB64);
            outWriter.Write(outB64);
            outWriter.Flush();
        }
    }
}
