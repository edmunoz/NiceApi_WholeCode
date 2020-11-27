using log4net;
using NiceApiLibrary_low;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceApiLibrary
{
    public static class DirectTel_Processing
    {
        public delegate void dOnAction(NiceSystemInfo subSystem);

        public static DirectTel_OutJson ProcessDirectTel(DirectTel_InJson inJson, IMyLog trayLog, LogForEmailSend log4Email, dOnAction onGet, dOnAction onAck)
        {
            trayLog.Debug("DirectTel: Loading " + inJson.SubSystem);
            NiceSystemInfo subSystem = inJson.SubSystem.GetSystemInfoFromTrayType();
            if (subSystem == null)
            {
                throw new Exception("subSystem == null");
            }

            switch (inJson.Inst)
            {
                case "Get":
                    {
                        // Update loopback file (with the data from the incoming object)
                        if (onGet != null)
                        {
                            onGet(subSystem);
                        }

                        // Prepare the object to be sent
                        ASPTrayBase trayBase = MessageProcessing_TrayTo.GetFilesToSendToTray_ConsiderPriority(subSystem, 1, trayLog).FirstOrDefault();
                        if (trayBase == null)
                        {
                            // nothing, so just wait
                            return new DirectTel_OutJson()
                            {
                                WaitSec = 10,
                            };
                        }
                        if (trayBase is Data_Net__00NormalMessage)
                        {
                            Data_Net__00NormalMessage _00 = trayBase as Data_Net__00NormalMessage;
                            return new DirectTel_OutJson()
                            {
                                Id = "00" +_00.GetFileName(),
                                Zapi = _00.DestMobile.Replace("+", ""),
                                Text = _00.Msg,
                                IsAddTelOnly = false,
                                WaitSec = 1,
                            };
                        }
                        if (trayBase is Data_Net__04CheckTelNumbers)
                        {
                            Data_Net__04CheckTelNumbers _04 = trayBase as Data_Net__04CheckTelNumbers;
                            MobileNoHandler h = new MobileNoHandler(_04.TelList);
                            string firstTel = h.MobileNumberX_AsZapi(0);
                            return new DirectTel_OutJson()
                            {
                                Id = "04" + _04.GetFileName(),
                                Zapi = firstTel,
                                Text = null,
                                IsAddTelOnly = true,
                                WaitSec = 1,
                            };
                        }
                    }
                    break;
                case "Ack":
                    // Update loopback file (with the data from the incoming object)
                    if (onAck != null)
                    {
                        onAck(subSystem);
                    }

                    MessageProcessing_TrayFrom proc = new MessageProcessing_TrayFrom(subSystem, null, trayLog, log4Email);
                    Data_Net_Tray2ASP helper = new Data_Net_Tray2ASP();
                    if (inJson.Id.StartsWith("00"))
                    {
                        inJson.Id = inJson.Id.Substring(2);
                        Data_Net__01NormalMessageResult _01 = new Data_Net__01NormalMessageResult(inJson.Id, true);
                        helper.ObjectList.Add(_01);
                        proc.Process_TrayFrom(helper, true);
                        return new DirectTel_OutJson();
                    }
                    else if (inJson.Id.StartsWith("04"))
                    {
                        inJson.Id = inJson.Id.Substring(2);
                        Data_Net__05CheckTelNumbersResult _05 = new Data_Net__05CheckTelNumbersResult(inJson.Id, null, null, null);
                        helper.ObjectList.Add(_05);
                        proc.Process_TrayFrom(helper, true);
                        return new DirectTel_OutJson();

                    }
                    else
                    {

                    }


                    break;
                default:
                    throw new Exception("Unknown inst");

            }

            return null;

        }
    }

    public class DirectTel_InJson
    {
        public string Inst { get; set; }
        public string SubSystem { get; set; }

        public string Id { get; set; }
    }
    public class DirectTel_OutJson
    {
        public string Id { get; set; }
        public string Zapi { get; set; }

        public string Text { get; set; }
        public bool IsAddTelOnly { get; set; }
        public int WaitSec { get; set; }
    }

}
