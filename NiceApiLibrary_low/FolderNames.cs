using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public enum MyFolders
    {
        // NOTE "ASP_" and Tray_" must only be user for folders which will be created

        ASP_QueuedMessages_ = 1,
        ASP_ProcessedMessages_ = 2,
        ASP_DisposedMessages_ = 3,
        ASP_UserAccountFolder_ = 4,
        ASP_UserWalletFolder_ = 5,
        ASP_ServerStateFolder = 6,
        _end
    }

    public enum MyUrls
    {
        trayASPURL_Local = 10,
        trayASPURL_Debug = 11,
        trayASPURL_Live = 12,
        trayASPURL_LiveNoSSL = 13,
        _end
    }

    public enum MyValues
    {
        Tray_LoopDelaySec,
        Tray_TcpTimeoutSec,
    }

    public class FolderNames
    {
        public static int GetValue(MyValues v)
        {
            switch (v)
            {
                case MyValues.Tray_LoopDelaySec:
                    return 5;
                case MyValues.Tray_TcpTimeoutSec:
                    return 5 * 60;
            }
            return 0;
        }

        public static bool IsDevMachine()
        {
            bool ret = "IsDevMachine".IsAppSettingsTrue();
            return ret;
        }

        public static string GetMachineRoot()
        {
            return IsDevMachine()
            ? "C:\\HostingSpaces\\mgillman\\"
            : "e:\\HostingSpaces\\mgillman\\niceapi.net\\data\\";
        }

        public static string GetFolder(NiceSystemInfo systemInfo, MyFolders f)
        {
            string strSystem = systemInfo.Default ? "" : "_SubSystem_" + systemInfo.Name + "\\";
            string strRoot = GetMachineRoot() + "_NiceSolution\\ASP_Zap\\" + strSystem;
            switch (f)
            {
                case MyFolders.ASP_QueuedMessages_:
                    return strRoot + "Queue";
                case MyFolders.ASP_ProcessedMessages_:
                    return strRoot + "Processed";
                case MyFolders.ASP_DisposedMessages_:
                    return strRoot + "Disposed";
                case MyFolders.ASP_UserAccountFolder_:
                    return strRoot + "Users";
                case MyFolders.ASP_UserWalletFolder_:
                    return strRoot + "Wallets";
                case MyFolders.ASP_ServerStateFolder:
                    return strRoot + "ServerState";
            }
            return null;
        }

        public static string GetUrl(MyUrls url)
        {
            switch (url)
            {
                case MyUrls.trayASPURL_Local:
                    return "http://localhost/LocalNiceApi/TrayApp";
                case MyUrls.trayASPURL_Debug:
                    return "http://localhost:60257/TrayApp";
                case MyUrls.trayASPURL_Live:
                    return "https://NiceApi.net/TrayApp";
                case MyUrls.trayASPURL_LiveNoSSL:
                    return "http://NiceApi.net/TrayApp";
            }
            return null;
        }

        public static void CreateFoldersForAsp(NiceSystemInfo systemInfo)
        {
            createFolders(systemInfo, "ASP_");
        }

        private static void createFolders(NiceSystemInfo systemInfo, string preText)
        {
            int i = 0;
            while (true)
            {
                MyFolders f = (MyFolders)i;
                if (f == MyFolders._end)
                {
                    break;
                }
                string s = f.ToString();
                if (s.StartsWith(preText))
                {
                    Create1Folder(GetFolder(systemInfo, f));
                }
                i++;
            }
        }

        private static void Create1Folder(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                throw new SystemException($"Create1Folder({path}) failed: {ex.Message}", ex);
            }
        }

    }
}
