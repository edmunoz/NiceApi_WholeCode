using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using NiceApiLibrary_low;


namespace NiceApiLibrary
{
    public delegate void dProcessOne(ASPTrayBase msg);
    
    public class MessageProcessing
    {
        public static void ProcessQueuedItems(NiceSystemInfo niceSystem, dProcessOne processAction, IMyLog log)
        {
            DSSwitch.msgFile00().ForEach(niceSystem, Data_Net__00NormalMessage.eLocation.Queued, log,
                delegate(Data_Net__00NormalMessage msg)
                {
                    processAction(msg);
                });
            DSSwitch.msgFile02().ForEach(niceSystem, log,
                delegate(Data_Net__02ScreenshotRequest msg)
                {
                    processAction(msg);
                });
            DSSwitch.msgFile04().ForEach(niceSystem, log,
                delegate(Data_Net__04CheckTelNumbers msg)
                {
                    processAction(msg);
                });
        }
    }
}
