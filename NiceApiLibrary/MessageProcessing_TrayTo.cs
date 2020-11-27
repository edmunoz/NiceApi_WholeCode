using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    public class MessageProcessing_TrayTo
    {
        public static List<ASPTrayBase> GetFilesToSendToTray_ConsiderPriority(NiceSystemInfo niceSystem, int limit, IMyLog log)
        {
            List<ASPTrayBase> FileListNormal = new List<ASPTrayBase>();
            List<ASPTrayBase> FileListPriority = new List<ASPTrayBase>();

            MessageProcessing.ProcessQueuedItems(
                niceSystem,
                new dProcessOne(delegate(ASPTrayBase msg)
                {
                    if (msg.IsPriority())
                    {
                        FileListPriority.Add(msg);
                    }
                    else
                    {
                        FileListNormal.Add(msg);
                    }
                }),
                log);

            if (FileListPriority.Count > 0)
            {
                return FileListPriority;
            }

            // 1) Sort
            FileListNormal.Sort(ComparisonWithPriority);

            // 2) Limit
            if (limit != -1)
            {
                while (FileListNormal.Count > limit)
                {
                    FileListNormal.RemoveAt(FileListNormal.Count - 1);
                }
            }

            return FileListNormal;
        }

        private static int ComparisonWithPriority(ASPTrayBase x, ASPTrayBase y)
        {
            int ret = 0;
            if (ret == 0)
            {
                // consider file priority
                ret = (int)x.GetFilePriority() - (int)y.GetFilePriority();
            }
            if ((ret == 0) && (x.GetType() != y.GetType()))
            {
                Data_Net__00NormalMessage n = null;
                Data_Net__04CheckTelNumbers t = null;
                if ((x.GetType() == typeof(Data_Net__00NormalMessage))
                    && (y.GetType() == typeof(Data_Net__04CheckTelNumbers)))
                {
                    n = (Data_Net__00NormalMessage)x;
                    t = (Data_Net__04CheckTelNumbers)y;

                    if (t.GetFailedCount() > 5)
                    {
                        ret = 1;
                    }
                    else
                    {
                        ret = -1;
                    }
                }
                if ((y.GetType() == typeof(Data_Net__00NormalMessage))
                    && (x.GetType() == typeof(Data_Net__04CheckTelNumbers)))
                {
                    n = (Data_Net__00NormalMessage)y;
                    t = (Data_Net__04CheckTelNumbers)x;

                    if (t.GetFailedCount() > 5)
                    {
                        ret = -1;
                    }
                    else
                    {
                        ret = 1;
                    }
                }
            }
            if (ret == 0)
            {
                // consider failed counter
                ret = x.GetFailedCount() - y.GetFailedCount();
            }
            if (ret == 0)
            {
                // consider file time
                if (
                    (x.GetEnumType() == ASPTrayBase.eASPtrayType.NormalMessage) &&
                    (y.GetEnumType() == ASPTrayBase.eASPtrayType.NormalMessage))
                {
                    Data_Net__00NormalMessage xn = (Data_Net__00NormalMessage)x;
                    Data_Net__00NormalMessage yn = (Data_Net__00NormalMessage)y;
                    if (ret == 0)
                    {
                        // compare the FailedConter
                        ret = xn.FailedCounter - yn.FailedCounter;
                    }
                    if (ret == 0)
                    {
                        // older message first
                        ret = (int)(xn.MsgTicks - yn.MsgTicks);
                    }
                }
                if (
                    (x.GetEnumType() == ASPTrayBase.eASPtrayType.CheckTelNumbers) &&
                    (y.GetEnumType() == ASPTrayBase.eASPtrayType.CheckTelNumbers))
                {
                    Data_Net__04CheckTelNumbers xt = (Data_Net__04CheckTelNumbers)x;
                    Data_Net__04CheckTelNumbers yt = (Data_Net__04CheckTelNumbers)y;
                    if (ret == 0)
                    {
                        // compare the FailedConter
                        ret = xt.GetFailedCount() - yt.GetFailedCount();
                    }
                    if (ret == 0)
                    {
                        // older message first
                        ret = (int)(xt.MsgTicks - yt.MsgTicks);
                    }
                }
            }
            return ret;
        }
    }
}
