using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{

    public abstract class ASPTrayBase : IData_Base
    {
        public enum eASPtrayType
        {
            NormalMessage,
            NormalMessageResult,
            ScreenShotRequest,
            ScreenShotResult,
            CheckTelNumbers,
            CheckTelNumbersResult,
        }
        public enum eFilePriority
        {
            High,
            Normal,
            Low
        }
        public virtual eASPtrayType GetEnumType()
        {
            Type t1 = this.GetType();
            if (false) { }
            else if (t1 == typeof(Data_Net__00NormalMessage)) { return eASPtrayType.NormalMessage; }
            else if (t1 == typeof(Data_Net__01NormalMessageResult)) { return eASPtrayType.NormalMessageResult; }
            else if (t1 == typeof(Data_Net__02ScreenshotRequest)) { return eASPtrayType.ScreenShotRequest; }
            else if (t1 == typeof(Data_Net__03ScreenshotResult)) { return eASPtrayType.ScreenShotResult; }
            else if (t1 == typeof(Data_Net__04CheckTelNumbers)) { return eASPtrayType.CheckTelNumbers; }
            else if (t1 == typeof(Data_Net__05CheckTelNumbersResult)) { return eASPtrayType.CheckTelNumbersResult; }
            else
            {
                throw new SystemException("Only type 0 to 5 are supported");
            }
        }

        public static ASPTrayBase ReadOne(string filePath, IMyLog log)
        {
            if (!s_MsgFile_IsOld(filePath))
            {
                using (BinaryReader br = new BinaryReader(OpenFile.ForRead(filePath, true, false, log)))
                {
                    return ASPTrayBase.ReadOne(br);
                }
            }
            return null;
        }

        public static ASPTrayBase ReadOne(BinaryReader br)
        {
            ASPTrayBase r = null;
            Int32 i32 = br.ReadInt32();
            eASPtrayType e = (eASPtrayType)i32;
            switch (e)
            {
                case eASPtrayType.NormalMessage:
                    r = new Data_Net__00NormalMessage(br);
                    break;
                case eASPtrayType.NormalMessageResult:
                    r = new Data_Net__01NormalMessageResult(br);
                    break;
                case eASPtrayType.ScreenShotRequest:
                    r = new Data_Net__02ScreenshotRequest(br);
                    break;
                case eASPtrayType.ScreenShotResult:
                    r = new Data_Net__03ScreenshotResult(br);
                    break;
                case eASPtrayType.CheckTelNumbers:
                    r = new Data_Net__04CheckTelNumbers(br);
                    break;
                case eASPtrayType.CheckTelNumbersResult:
                    r = new Data_Net__05CheckTelNumbersResult(br);
                    break;
                default:
                    throw new IOException("File format not supported");
            }
            return r;
        }
        public virtual void NetFrom(BinaryReader br)
        {
            throw new NotSupportedException("Use ReadOne please.");
        }
        public virtual void NetTo(BinaryWriter bw)
        {
            bw.Write((Int32)GetEnumType());
        }
        public abstract void NetIniMembers();
        public abstract string GetFileName();
        public abstract int GetFailedCount();
        public abstract eFilePriority GetFilePriority();

        public abstract string GetNiceStatus();

        public virtual bool IsPriority()
        {
            if (GetFileName().Contains("at_NiceApi_dot_net"))
            {
                return true;
            }
            return false;
        }

        protected static string s_MsgFile_GetFileName(Int64 UtcTicks, string Email)
        {
            DateTime utc = new DateTime(UtcTicks, DateTimeKind.Utc);
            string ret = string.Format("Ms2_{0:0000}{1:00}{2:00}x{3:00}{4:00}_{5}_{6}.txt",
                utc.Year,
                utc.Month,
                utc.Day,
                utc.Hour,
                utc.Minute,
                UtcTicks,
                Data_AppUserFile.EmailSaveChars(Email));
            return ret;
        }

        public static bool s_MsgFile_IsOld(string FilePathAndName)
        {
            string fileName = Path.GetFileNameWithoutExtension(FilePathAndName);

            if (fileName.StartsWith("Ms2_"))
            {
                return false;
            }
            return true;
        }

        public class MsgFileParts
        {
            public DateTime Time;
            public string Email;
        }

        public static MsgFileParts s_MsgFile_GetPartsFromMessageFile(string FilePathAndName)
        {
            MsgFileParts r = new MsgFileParts();

            string fileName = Path.GetFileNameWithoutExtension(FilePathAndName);

            if (fileName.StartsWith("Msg_"))
            {
                if (fileName.EndsWith("_.txt"))
                {
                    fileName = fileName.Substring(0, fileName.Length - "_.txt".Length);
                }
                else if (fileName.EndsWith("_"))
                {
                    fileName = fileName.Substring(0, fileName.Length - "_".Length);
                }
                string[] sp = fileName.Split(new char[] { '_' }, 4);
                Int64 ticks = Int64.Parse(sp[1]);
                DateTime fileTime = new DateTime(ticks, DateTimeKind.Utc);
                r.Time = fileTime;
                r.Email = sp[3];
            }
            else if (fileName.StartsWith("Ms2_"))
            {
                if (fileName.EndsWith("_.txt"))
                {
                    fileName = fileName.Substring(0, fileName.Length - "_.txt".Length);
                }
                else if (fileName.EndsWith("_"))
                {
                    fileName = fileName.Substring(0, fileName.Length - "_".Length);
                }
                string[] sp = fileName.Split(new char[] { '_' }, 4);
                Int64 ticks = Int64.Parse(sp[2]);
                DateTime fileTime = new DateTime(ticks, DateTimeKind.Utc);
                r.Time = fileTime;
                r.Email = sp[3];
            }
            else
            {
                throw new IOException("Invalid file format in s_GetDateOfMessageFile: " + FilePathAndName);
            }
            return r;
        }

        public static DateTime s_MsgFile_GetDateTimeOfMessageFile(string FilePathAndName)
        {
            return s_MsgFile_GetPartsFromMessageFile(FilePathAndName).Time;
        }
    }

    public abstract class ASPTrayBaseWithUniqueFileName : ASPTrayBase
    {
        public string UserId;
        public Int64 MsgTicks;

        protected void SetMembers(string UserId, Int64 MsgTicks)
        {
            this.UserId = UserId;
            this.MsgTicks = MsgTicks;
        }
        protected ASPTrayBaseWithUniqueFileName()
        {
            NetIniMembers();
        }

        public override string GetFileName()
        {
            return ASPTrayBase.s_MsgFile_GetFileName(MsgTicks, UserId);
        }
        public override int GetFailedCount()
        {
            return 0;
        }
        public override void NetIniMembers()
        {
            this.UserId = "";
            this.MsgTicks = 0;
        }
        public override void NetTo(BinaryWriter bw)
        {
            base.NetTo(bw);
            bw.Write(UserId);
            bw.Write(MsgTicks);
        }
        public override void NetFrom(BinaryReader br)
        {
            UserId = br.ReadString();
            MsgTicks = br.ReadInt64();
        }
    }
}
