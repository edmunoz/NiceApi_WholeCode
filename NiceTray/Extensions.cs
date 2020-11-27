using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Configuration;

namespace NiceTray
{
    static class Extensions
    {
        public static string GetLocalIPAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch { }
            return "NoIp ";
        }
        public static string Test(this string str)
        {
            return str;
        }
        public static bool IsMayBeGood(this eI6Error e)
        {
            return e == eI6Error.FailedButNoLettingHostKnow_TelNotActive;
        }
        public static bool IsGood(this eI6Error e)
        {
            return e == eI6Error.Success;
        }
        public static bool IsBad(this eI6Error e)
        {
            if (e.IsMayBeGood())
            {
                return false;
            }
            if (e.IsGood())
            {
                return false;
            }
            return true;
        }


        //public static string ToReadableString(this TimeSpan span)
        //{
        //    span = span.Duration();
        //    string day = span.Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty;
        //    string hour = span.Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty;
        //    string min = span.Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty;
        //    string sec = 
        //        string.Format("{0:0}", span.Seconds) +
        //        string.Format(".{0:000} second{1}", span.Milliseconds, span.Seconds == 1 ? String.Empty : "s");

        //    string formatted = string.Format("{0}{1}{2}{3}",
        //        day, hour, min, sec);

        //    if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

        //    if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

        //    return formatted;
        //}

        //public static string AvoidUnprintableChars(this string sIn)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (char c in sIn)
        //    {
        //        if (c == Encoding.ASCII.GetString(new byte[] { 16 })[0])// DEL was seen in the log,
        //        { 
        //            // avoid
        //        }
        //        else if (Char.IsLetterOrDigit(c))
        //        {
        //            sb.Append(c);
        //        }
        //        else if (Char.IsPunctuation(c))
        //        {
        //            sb.Append(c);
        //        }
        //        else if (c == ' ')
        //        {
        //            sb.Append(c);
        //        }
        //        else if (c == '\r')
        //        {
        //            sb.Append("<\\r>");
        //        }
        //        else if (c == '\n')
        //        {
        //            sb.Append("<\\n>");
        //        }
        //        else if (c == '\t')
        //        {
        //            sb.Append("<\\t****>");
        //        }
        //        else if (c == '\v')
        //        {
        //            sb.Append("<\\v****>");
        //        }
        //        else if (Char.IsWhiteSpace(c))
        //        {
        //            sb.Append(c);
        //        }
        //        else if (Char.IsSymbol(c))
        //        {
        //            sb.Append("<Symbol>");
        //        }
        //        else if (Char.IsSurrogate(c))
        //        {

        //        }
        //        else
        //        {

        //        }
        //    }
        //    return sb.ToString();
        //}

        public static string BmpToB64_Png(this Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            string b64 = Convert.ToBase64String(ms.ToArray());
            return b64;
        }

        public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }
    }
}
