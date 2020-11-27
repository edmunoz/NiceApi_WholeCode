using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Configuration;

using NiceApiLibrary_low;


namespace NiceTray
{
    public class C9_CPUSlowdown : I9_CPUSlowdown
    {
        public static float _sLastCpuUsage;
        float maxCou;
        int sleepTime;
        System.Diagnostics.PerformanceCounter cpuCounter;
        public C9_CPUSlowdown()
        {
            if ("_9_CPUSlowdown".IsConfiguredAndTRUE())
            {
                maxCou = "_9_CPUSlowdown_MaxCpu".GetConfigFloat();
                sleepTime = "_9_CPUSlowdown_Sleep".GetConfigInt();
                cpuCounter = new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total");
            }
        }
        internal float getCurrentCpuUsage()
        {
            var ret = cpuCounter.NextValue();
            _sLastCpuUsage = ret;
            return ret;
        }
        public void Slowdown(Ix iAll)
        {
            if (cpuCounter != null)
            {
                var cur = getCurrentCpuUsage();
                while (cur > maxCou)
                {
                    iAll.iDsp.FileLog_Debug($"Slowdown: {cur} > {maxCou}, Sleep({sleepTime})");

                    iAll.TimeControl?.AddMilliSeconds(sleepTime);
                    System.Threading.Thread.Sleep(sleepTime);
                    // next 
                    cur = getCurrentCpuUsage();
                }
            }
        }
    }
}
