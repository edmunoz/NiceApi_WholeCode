using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NiceApiLibrary_low
{
    public delegate void dProcess_Data_Net__00NormalMessage(Data_Net__00NormalMessage one);
    public delegate void dProcess_Data_Net__02ScreenshotRequest(Data_Net__02ScreenshotRequest one);
    public delegate void dProcess_Data_Net__04CheckTelNumbers(Data_Net__04CheckTelNumbers one);

    public interface IData_Net__00NormalMessage_store
    {
        String GetInfo(NiceSystemInfo niceSystem);
        void Store(NiceSystemInfo niceSystem, Data_Net__00NormalMessage msg, Data_Net__00NormalMessage.eLocation location, IMyLog log);
        void ForEach(NiceSystemInfo niceSystem, DateTime newerThan, string containsUser, Data_Net__00NormalMessage.eLocation location, IMyLog log, dProcess_Data_Net__00NormalMessage cb);
        void ForEach(NiceSystemInfo niceSystem, Data_Net__00NormalMessage.eLocation location, IMyLog log, dProcess_Data_Net__00NormalMessage cb);
        Data_Net__00NormalMessage ReadOne(NiceSystemInfo niceSystem, string fileName, Data_Net__00NormalMessage.eLocation location, IMyLog log);
        void Delete(NiceSystemInfo niceSystem, string fileName, Data_Net__00NormalMessage.eLocation location, IMyLog log);
        int GetNoOfQueuedItems(NiceSystemInfo niceSystem, IMyLog log);
    }

    public interface IData_Net__02ScreenshotRequest_store
    {
        String GetInfo(NiceSystemInfo niceSystem);
        void Store(NiceSystemInfo niceSystem, Data_Net__02ScreenshotRequest msg, IMyLog log);
        void ForEach(NiceSystemInfo niceSystem, IMyLog log, dProcess_Data_Net__02ScreenshotRequest cb);
        Data_Net__02ScreenshotRequest ReadOne(NiceSystemInfo niceSystem, string fileName, IMyLog log);
        void Delete(NiceSystemInfo niceSystem, string fileName, IMyLog log);
    }

    public interface IData_Net__04CheckTelNumbers_store
    {
        String GetInfo(NiceSystemInfo niceSystem);
        void Store(NiceSystemInfo niceSystem, Data_Net__04CheckTelNumbers msg, IMyLog log);
        void ForEach(NiceSystemInfo niceSystem, IMyLog log, dProcess_Data_Net__04CheckTelNumbers cb);
        Data_Net__04CheckTelNumbers ReadOne(NiceSystemInfo niceSystem, string fileName, IMyLog log);
        void Delete(NiceSystemInfo niceSystem, string fileName, IMyLog log);
    }

    public class NiceSystemInfo
    {
        public string Name { get; set; }
        public bool Default { get; set; }
        public string APIId { get; set; }

        public static readonly string DEFAULT_STR = "Default";
        public static readonly NiceSystemInfo DEFAULT = new NiceSystemInfo() { Name = DEFAULT_STR, Default = true, APIId = string.Empty };
    }

    public class NotificationInfo
    {
        public string Name { get; set; }
        public string APIId { get; set; }
        public string RxTel { get; set; }
    }

    public enum NiceSystemType
    {
        File, 
        SQLite,
    }

    public interface IData_Full
    {
        NiceSystemType GetNiceSystemType();
        List<NiceSystemInfo> GetSystems(bool createResources);
        NotificationInfo GetNotificationInfo();
        void AddSystem(NiceSystemInfo systemInfo);
        IData_AppUserFileHandling AppUserFile();
        IData_AppUserWalletHandling AppWalletFile();
        IData_SqlHandling Sql();
        IData_Net__00NormalMessage_store _00NormalMessage();
        IData_Net__02ScreenshotRequest_store _02ScreenshotRequest();
        IData_Net__04CheckTelNumbers_store _04CheckTelNumbers();
    }
}
