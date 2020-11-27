using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace NiceTray
{
    public class _8UpdateCommunicator_Dummy : I8_UpdateCommunicator
    {
        public _8AndroidCommunicator Android() { return new _8AndroidCommunicator_Dummy(); }
        public _8ServerCommunicator Server() { return new _8ServerCommunicator_Dummy(); } 
    }

    public class _8AndroidCommunicator_Dummy : _8AndroidCommunicator
    {
        private static bool first_8AndroidCommunicator_Dummy = true;
        d_OnAndroidDispose OnDone = null;
        List<string> Temp = new List<string>() { //mg add android tel here
            "+55123456789",
            "+55111111111",
            "+60146110151"
        };
        public _8AndroidCommunicator_Dummy()
        {
            if (!first_8AndroidCommunicator_Dummy)
            {
                if ("_8UpdateCommunicator.Dummy.OnlyWorkOnce".IsConfiguredAndTRUE())
                {
                    throw new SocketException(10060);
                }
            }
            // end
            first_8AndroidCommunicator_Dummy = false;
        }
        public void SetOnAndroidDispose(d_OnAndroidDispose cb)
        {
            OnDone = cb;
        }
        public void SetCommand(string telWithPlus, I2_InfoDisplay i2)
        {
            if (!Temp.Contains(telWithPlus))
            {
                Temp.Add(telWithPlus);
            }
        }
        public List<string> GetCommand(I2_InfoDisplay i2)
        {
            return Temp;
        }

        public string GetEndPointInfo()
        {
            return "Dummy";
        }

        public void Dispose()
        {
            if (OnDone != null)
            {
                OnDone(new string[] {
                    "totalMemory: 139292672",
                    "freeMemory: 11436632",
                    "maxMemory: 201326592"});
            }
        }
    }

    public class _8ServerCommunicator_Dummy : _8ServerCommunicator
    {
        List<string> Temp = new List<string>() { 
            "+55123456789",
            "+55111111111" 
        };
        public List<string> GetCommand(I2_InfoDisplay i2)
        {
            return Temp;
        }

        public void Dispose()
        {

        }
    }
}
