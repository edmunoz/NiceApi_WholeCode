using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiceApiLibrary_low;

namespace NiceTray
{
    public enum eI2LogLevel
    {
        Debug_0,
        Info_1,
        Error_2
    }

    public interface I2_InfoDisplay //this needs to be thread save
    {
        void Start();
        void ReturnWhenReady();
        void LoopStart();
        void FileLog_Debug(string str);
        void FileLog_Info(string str);
        void FileLog_Error(string str);
        void FileLog_TelStatus(List<string> val);
        void AddLine(string line);
        void AddLine2(IAddLine iFace);
        string FileLog_GetPreText();
        void FileLog_SetPreText(string val);
        void Clear();

        void Delay(int ms);
        bool IsRealDisplay();
    }

    public class PreTextHandler
    {
        private string Text = "";
        public virtual string FileLog_GetPreText()
        {
            return Text;
        }
        public virtual void FileLog_SetPreText(string val)
        {
            Text = val;
        }
    }

    public class LogPreText : IDisposable
    {
        private string PreText;
        private Ix Ix;
        private string ConstOld;
        private string ConstNew;
        public LogPreText(string preText, Ix ix)
        {
            PreText = preText + ": ";
            Ix = ix;

            ConstOld = ix.iDsp.FileLog_GetPreText();
            ConstNew = /*ConstOld + */PreText;
            ix.iDsp.FileLog_SetPreText(ConstNew);
        }

        public void Dispose()
        {
            Ix.iDsp.FileLog_SetPreText(ConstOld);
        }
    }
}
