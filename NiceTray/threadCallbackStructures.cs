using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using NiceApiLibrary_low;

namespace NiceTray
{
    public class Data_DisplayText //todo can this be removed?
    {
        public bool ClearAll;
        public string Text;
        public ManualResetEvent StopEvent;

        public Data_DisplayText(bool clearAll, string text, ManualResetEvent stopEvent)
        {
            this.ClearAll = clearAll;
            this.Text = text;
            this.StopEvent = stopEvent;
        }
    }

}
