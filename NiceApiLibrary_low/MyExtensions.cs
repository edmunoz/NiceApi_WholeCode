using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using static NiceApiLibrary_low.Data_AppUserFile;

namespace NiceApiLibrary_low
{
    public enum AppPart
    {
        ASP,
        Tray
    }

    public class WebControlsTableResult
    {
        public System.Web.UI.WebControls.Table Table;
        public System.Web.UI.WebControls.Label Label;
    }

    public static class MyExtensions
    {
        public static string ToLineName(this eDisplayItem item)
        {
            string ret = item.ToString();
            ret = ret.Replace("_", "");
            return ret;
        }

        public static int Importance(this eUserStatus status)
        {
            switch (status)
            {
                case eUserStatus.commercial_systemDuplication:
                    return 100;
                case eUserStatus.commercial_monthlyDifPrice:
                    return 95;
                case eUserStatus.commercial_monthly:
                    return 90;
                case eUserStatus.commercial_payassent:
                    return 80;
                case eUserStatus.free_account:
                    return 70;
                case eUserStatus.verified_welcome_No_sent:
                case eUserStatus.verified_welcome_queued:
                case eUserStatus.verified_checkingTelNumbers:
                    return 60;
                case eUserStatus.blocked:
                    return 50;
                case eUserStatus.email_sent_for_verification:
                    return 40;
                default:
                    throw new NotImplementedException("Importance");
            }
        }

        public static string rigtStr(this Decimal val)
        {
            return "= $ " + String.Format("{0:0.00}", val).PadLeft(8);
        }

        public static string LimitText(this string str, int maxText)
        {
            if (str.Length > maxText)
            {
                str = string.Format("T{0}: {1}", str.Length, str.Substring(0, maxText));
            }
            return str;
        }


        public static void WriteToFile(this MyFolders folder, NiceSystemInfo systemInfo, IData_Base data, IMyLog log)
        {
            string file = FolderNames.GetFolder(systemInfo, folder) + Path.DirectorySeparatorChar + data.GetFileName();

            using (BinaryWriter bw = new BinaryWriter(OpenFile.ForWrite(file, log)))
            {
                data.NetTo(bw);
            }
        }

        public static bool IsAppSettingsTrue(this string configKey)
        {
            string val = System.Configuration.ConfigurationManager.AppSettings[configKey];
            if (val != null)
            {
                bool bVal;
                if (Boolean.TryParse(val, out bVal))
                {
                    return bVal;
                }
            }
            return false;
        }

        public static bool IsAppSettingsSame(this string configKey, string same)
        {
            string val = System.Configuration.ConfigurationManager.AppSettings[configKey];
            if (val != null)
            {
                if (same == val)
                    return true;
            }
            return false;
        }

        public static string AppSettingsGet(this string configKey)
        {
            string val = System.Configuration.ConfigurationManager.AppSettings[configKey];
            return val;
        }

        public static string Zapi_Remove(this string s)
        {
            return s.Replace("zapi_", "");
        }

        public static string Zapi_Add(this string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s1 in s.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries))
            {
                sb.Append("zapi_+" + s1);
            }
            return sb.ToString();
        }

        public static string ToSwissTime(this Int64 ticks, bool withDifToCurrentTime)
        {
            return new DateTime(ticks, DateTimeKind.Utc).ToSwissTime(withDifToCurrentTime);
        }
        public static string ToSwissTime(this DateTime dateTime, bool withDifToCurrentTime)
        {
            if (dateTime == null)
            {
                return "null";
            }
            StringBuilder ret = new StringBuilder();
            if (dateTime.Ticks == 0)
            {
                return "N/A";
            }
            DateTime dtSwiss = dateTime.AddHours(2);
            ret.AppendFormat("CH: {0}", dtSwiss.ToString("dd/MM/yyyy HH:mm:ss.fff"));

            if (withDifToCurrentTime)
            {
                TimeSpan span__ = DateTime.UtcNow - dateTime;
                ret.AppendFormat(" ({0})", span__.ToReadableString());
            }
            return ret.ToString();
        }

        public static string ToUkTime(this Int64 ticks, bool withDifToCurrentTime)
        {
            return new DateTime(ticks, DateTimeKind.Utc).ToUkTime(withDifToCurrentTime);
        }
        public static string ToUkTime(this DateTime dateTime, bool withDifToCurrentTime)
        {
            if (dateTime == null)
            {
                return "null";
            }
            StringBuilder ret = new StringBuilder();
            if (dateTime.Ticks == 0)
            {
                return "N/A";
            }
            DateTime dtUk = dateTime;
            ret.AppendFormat("UTC: {0}", dtUk.ToString("dd/MM/yyyy HH:mm:ss.fff"));

            if (withDifToCurrentTime)
            {
                TimeSpan span__ = DateTime.UtcNow - dateTime;
                ret.AppendFormat(" ({0})", span__.ToReadableString());
            }
            return ret.ToString();
        }
        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public static string ToReadableString(this TimeSpan span)
        {
            if (span == TimeSpan.MinValue)
            {
                return "?";
            }
            span = span.Duration();
            string day = span.Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty;
            string hour = span.Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty;
            string min = span.Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty;
            string sec =
                string.Format("{0:0}", span.Seconds) +
                string.Format(".{0:000} second{1}", span.Milliseconds, span.Seconds == 1 ? String.Empty : "s");

            string formatted = string.Format("{0}{1}{2}{3}",
                day, hour, min, sec);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;
        }


        public static string ToByteArrayString(this string strIn)
        {
            StringBuilder hex = new StringBuilder(strIn.Length * 2);
            foreach (char c in strIn)
            {
                hex.AppendFormat("{0:X2}", (byte)c);
            }
            return hex.ToString();
        }

        public static void CopyTo(this Stream ts, Stream os)
        {
            try
            {
                while (true)
                {
                    int b = ts.ReadByte();
                    if (b == -1)
                    {
                        return;
                    }
                    os.WriteByte((byte)b);
                }
            }
            catch (EndOfStreamException)
            {
            }
            catch (SystemException)
            {
            }
            os = null;
        }


        public static Stream Peep(this Stream sIn, out string sOut, out string sOut2, out byte[] baOut)
        {
            MemoryStream mem = new MemoryStream();
            sIn.CopyTo(mem);
            sOut = new StreamReader(mem).ReadToEnd();
            mem.Position = 0;
            baOut = new byte[mem.Length];
            sOut2 = "";
            for (int i = 0; i < baOut.Length; i++)
            {
                baOut[i] = (byte)mem.ReadByte();
                if (baOut[i] > 0x1F)
                {
                    sOut2 += (char)baOut[i];

                }
            }
            mem.Position = 0;
            return mem;
        }

        public static string My_MobileNumbers_AddNewLine(this string mobNumbers)
        {
            return mobNumbers.Replace("+", Environment.NewLine + "+");
        }
        public static string My_MobileNumbers_RemoveNewLine(this string mobNumbers)
        {
            return mobNumbers.Replace(Environment.NewLine + "+", "+");
        }


        public static bool My_Contains(this char c1, bool allowLetter, bool allowZiff, bool allowSpace, bool allowUnderline)
        {
            if ((c1 >= '0') && (c1 <= '9'))
            {
                return allowZiff;
            }
            else if ((c1 >= 'A') && (c1 <= 'Z'))
            {
                return allowLetter;
            }
            else if ((c1 >= 'a') && (c1 <= 'z'))
            {
                return allowLetter;
            }
            else if (c1 == ' ')
            {
                return allowSpace;
            }
            else if (c1 == '_')
            {
                return allowUnderline;
            }
            else if (c1 == '-')
            {
                return allowUnderline;
            }
            else
            {
                return false;
            }
        }

        public static bool My_OnlyLetterDigitAndSpace(this string str)
        {
            foreach (char c1 in str)
            {
                if (!c1.My_Contains(true, true, true, false))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool My_MobileNumber(this string str)
        {
            if (str.Length < 8)
            {
                return false;
            }
            int pos = 0;
            foreach (char c1 in str)
            {
                if (pos == 0)
                {
                    if (c1 != '+')
                    {
                        return false;
                    }
                }
                else
                {
                    if (!c1.My_Contains(false, true, false, false))
                    {
                        return false;
                    }
                }
                pos++;
            }
            return true;
        }

        public static bool My_Email(this string str)
        {

            if (str.Length < 8)
            {
                return false;
            }
            bool atFound = false;
            bool dotFound = false;
            foreach (char c1 in str)
            {
                if (c1.My_Contains(true, true, false, true))
                {
                    // ok
                }
                else if (c1 == '@')
                {
                    // ok
                    atFound = true;
                }
                else if (c1 == '.')
                {
                    // ok
                    dotFound = true;
                }
                else
                {
                    return false;
                }
            }
            return atFound && dotFound;
        }

        public static void WriteToStream(this string[] val, BinaryWriter bw)
        {
            bw.Write((Int32)val.Length);
            foreach (string val1 in val)
            {
                bw.Write(val1);
            }
        }

        public static string[] ReadFromStream(this string[] val, BinaryReader br)
        {
            string[] r = new string[br.ReadInt32()];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = br.ReadString();
            }
            return r;
        }

        public static string[] MyClone(this string[] val)
        {
            string[] r = new string[val.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = val[i];
            }
            return r;
        }

        public static int SmallRest(this DateTime dt)
        {
            DateTime dt2 = new DateTime(
                dt.Year, dt.Month, dt.Day,
                dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);
            return (int)(dt.Ticks - dt2.Ticks);
        }

        public static void WriteAssemblyVersion(this Assembly a, TextWriter o)
        {
            o.WriteLine("ImageRuntimeVersion: " + a.ImageRuntimeVersion.ToString());
            o.WriteLine("EscapedCodeBase: " + a.EscapedCodeBase.ToString());
            o.WriteLine("FullName: " + a.FullName.ToString());
            o.WriteLine("Location: " + a.Location.ToString());
        }

        public static string[] GetAssemblyVersion(this Assembly a)
        {
            List<string> ret = new List<string>();
            ret.Add("ImageRuntimeVersion: " + a.ImageRuntimeVersion.ToString());
            ret.Add("EscapedCodeBase: " + a.EscapedCodeBase.ToString());
            ret.Add("FullName: " + a.FullName.ToString());
            ret.Add("Location: " + a.Location.ToString());
            return ret.ToArray();
        }

        public static string WriteAssemblyVersion(this Assembly a)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (TextWriter tw = new StreamWriter(ms))
                {
                    a.WriteAssemblyVersion(tw);
                    tw.Flush();
                    string r = Encoding.ASCII.GetString(ms.ToArray());
                    return r;
                }
            }
        }
        

    }


    public static class OperatingSystemInfo
    {
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        public class Result
        {
            public bool is64BitProcess;
            public bool is64BitOperatingSystem;
            public string OsString = "";
            public string error = "";

            public override string ToString()
            {
                string str = string.Format("OS:{0}, Process:{1}, OperatingSystem:{2} {3}",
                    OsString,
                    is64BitProcess ? "64" : "32",
                    is64BitOperatingSystem ? "64" : "32",
                    error);
                return str;
            }
        }

        public static Result Get()
        {
            Result r = new Result();
            try
            {
                r.is64BitProcess = (IntPtr.Size == 8);
                r.is64BitOperatingSystem = r.is64BitProcess;
                r.error = "";

                r.OsString = Environment.OSVersion.ToString();

                if (!r.is64BitOperatingSystem)
                {
                    if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                        Environment.OSVersion.Version.Major >= 6)
                    {
                        try
                        {
                            using (Process p = Process.GetCurrentProcess())
                            {
                                bool retVal;
                                if (!IsWow64Process(p.Handle, out retVal))
                                {
                                    r.is64BitOperatingSystem = false;
                                }
                                r.is64BitOperatingSystem = retVal;
                            }
                        }
                        catch (Exception ex)
                        {
                            r.error += " " + ex.ToString();
                        }
                    }
                    else
                    {
                        r.is64BitOperatingSystem = false;
                    }
                }
            }
            catch (Exception ex)
            {
                r.error += " " + ex.ToString();
            }
            return r;
        }

        internal static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }
    }
}

