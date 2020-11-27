using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NiceApiLibrary_low;

namespace NiceTray
{
    public interface I3_GetData
    {
        void Reset_toASP();
        void ExchangeDataWithServer(I2_InfoDisplay d, I6_WhatsAppProcess p);
        List<ASPTrayBase> GetServerFiles(Ix ix);
        void Debug_GetProcessIdOfFile(ASPTrayBase file, out int processId, out bool useProcessId);

        void AddResultFile(ASPTrayBase file);

        bool DoWeHaveDataToSend();
    }
}
