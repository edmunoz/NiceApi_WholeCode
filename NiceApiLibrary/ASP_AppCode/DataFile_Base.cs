using NiceApiLibrary_low;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceApiLibrary.ASP_AppCode
{

    /// <summary>
    /// Summary description for DataFile_Base
    /// </summary>
    public abstract class DataFile_Base : IDisposable, IDataFile_User
    {
        protected Stream _stream;
        protected bool _forUpdate;
        protected NiceSystemInfo niceSystem;

        public enum OpenType
        {
            ForUpdate_CreateIfNotThere,
            ReadOnly_CreateIfNotThere,

        }

        protected DataFile_Base(NiceSystemInfo niceSystem, OpenType openType)
        {
            this.niceSystem = niceSystem;
            switch (openType)
            {
                case OpenType.ForUpdate_CreateIfNotThere:
                    _forUpdate = true;
                    _stream = AnyServerFile.GetForUpdate_CreateIfNeeded(GetFullPath(), this);
                    break;

                case OpenType.ReadOnly_CreateIfNotThere:
                    _forUpdate = false;
                    _stream = AnyServerFile.GetForReadOnly(GetFullPath());
                    if (_stream == null)
                    {
                        _forUpdate = true;
                        _stream = AnyServerFile.GetForUpdate_CreateIfNeeded(GetFullPath(), this);
                    }
                    else
                    {
                        NetFrom(new BinaryReader(_stream));
                    }
                    break;
            }
        }

        abstract public string GetFullPath();
        abstract public string GetFileName();
        abstract public void NetFrom(BinaryReader br);
        abstract public void NetTo(BinaryWriter bw);
        abstract public void NetIniMembers();

        public void Dispose()
        {
            if (_forUpdate)
            {
                _stream.Seek(0, SeekOrigin.Begin);
                BinaryWriter bw = new BinaryWriter(_stream);
                NetTo(bw);
            }
            _stream.Close();
        }

    }

    public interface IDataFile_User : IData_Base, IDisposable
    {
        string GetFullPath();
    }

    public static class AnyServerFile
    {
        private static IMyLog s_log = MyLog.GetLogger("AnyServerFile");
        public static Stream GetForReadOnly(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                return OpenFile.ForRead(fullPath, true, false, s_log);
            }
            return null;
        }
        public static Stream GetForUpdate_CreateIfNeeded(string fullPath, IData_Base oOut)
        {
            Stream r = null;
            if (!File.Exists(fullPath))
            {
                // not there yet
                oOut.NetIniMembers();
                r = OpenFile.ForWrite(fullPath, s_log);
                using (BinaryWriter bw = new BinaryWriter(r))
                {
                    oOut.NetTo(bw);
                }
            }

            r = OpenFile.ForRead(fullPath, true, true, s_log);
            oOut.NetFrom(new BinaryReader(r));
            return r;
        }
    }
}

