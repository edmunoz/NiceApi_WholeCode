using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NiceTray
{
    public delegate void d_OnAndroidDispose(string[] sList);

    public interface I8_UpdateCommunicator
    {
        _8AndroidCommunicator Android();
        _8ServerCommunicator Server();
    }

    public interface _8AndroidCommunicator : IDisposable
    {
        void SetOnAndroidDispose(d_OnAndroidDispose cb);
        void SetCommand(string telWithPlus, I2_InfoDisplay i2);
        List<string> GetCommand(I2_InfoDisplay i2);
        string GetEndPointInfo();
    }

    public interface _8ServerCommunicator : IDisposable
    {
        List<string> GetCommand(I2_InfoDisplay i2);
    }

    public interface I9_CPUSlowdown
    {
        void Slowdown(Ix iAll);
    }
}
