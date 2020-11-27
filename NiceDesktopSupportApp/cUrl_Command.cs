using NiceApiLibrary_low;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceDesktopSupportApp
{
    class cUrl_Command
    {
        public static void Go(IMyLog log, QuestionOption it)
        {
            List<ftpFile> list = getList();
            cUrlCmd cfg = getcUrlCmd();
            StringBuilder sb = new StringBuilder();
            sb.Append(cfg.pre);
            foreach (ftpFile e in list)
            {
                if (e.sub != null)
                {
                    string line = $"-T \"{cfg.srcPath}{e.sub}\\{e.src}\" \"{cfg.dstPath}{e.sub}/\" ";
                    sb.Append(line);
                }
                else
                {
                    string line = $"-T \"{cfg.srcPath}{e.src}\" \"{cfg.dstPath}\" ";
                    sb.Append(line);
                }
            }
            sb.Append(cfg.pos);
            System.IO.File.WriteAllText(cfg.batName, sb.ToString());
        }

        internal static cUrlCmd getcUrlCmd()
        {
            return new cUrlCmd()
            {
                pre = "curl --ftp-ssl-control -1 --insecure --user <user:pwd> ",
                pos = "\r\n\r\npause\r\n",
                srcPath = throw new NotImplementedException("not suitable for publication!"),
                dstPath = throw new NotImplementedException("not suitable for publication!"),
                batName = "SyncLocWWWroot_to_FTP.bat"
            };
        }

        internal static List<ftpFile> getList()
        {
            List<ftpFile> last = new List<ftpFile>() { new ftpFile() { src = "Global.asax" } };
            List<ftpFile> list = new List<ftpFile>() 
            {
                new ftpFile() { src = "Admin.aspx" },
                new ftpFile() { src = "Admin.aspx.cs" },
                new ftpFile() { src = "AdminImages.aspx" },
                new ftpFile() { src = "AdminImages.aspx.cs" },
                new ftpFile() { src = "API.aspx" },
                new ftpFile() { src = "API.aspx.cs" },
                new ftpFile() { src = "APICredit.aspx" },
                new ftpFile() { src = "APICredit.aspx.cs" },
                new ftpFile() { src = "APIForm.aspx" },
                new ftpFile() { src = "APIForm.aspx.cs" },
                new ftpFile() { src = "APIPrivAddTel.aspx" },
                new ftpFile() { src = "APIPrivAddTel.aspx.cs" },
                new ftpFile() { src = "APITelNumbers.aspx" },
                new ftpFile() { src = "APITelNumbers.aspx.cs" },
                new ftpFile() { src = "Counters.aspx" },
                new ftpFile() { src = "Counters.aspx.cs" },
                new ftpFile() { src = "DataAll.aspx" },
                new ftpFile() { src = "DataAll.aspx.cs" },
                new ftpFile() { src = "Default.aspx" },
                new ftpFile() { src = "Default.aspx.cs" },
                new ftpFile() { src = "Details.aspx" },
                new ftpFile() { src = "Details.aspx.cs" },
                new ftpFile() { src = "HowItWorks.aspx" },
                new ftpFile() { src = "HowItWorks.aspx.cs" },
                new ftpFile() { src = "HowToUse.aspx" },
                new ftpFile() { src = "HowToUse.aspx.cs" },
                new ftpFile() { src = "ItemX.aspx" },
                new ftpFile() { src = "ItemX.aspx.cs" },
                new ftpFile() { src = "Login.aspx" },
                new ftpFile() { src = "Login.aspx.cs" },
                new ftpFile() { src = "Logout.aspx" },
                new ftpFile() { src = "Logout.aspx.cs" },
                new ftpFile() { src = "Loopback.aspx" },
                new ftpFile() { src = "Loopback.aspx.cs" },
                new ftpFile() { src = "Maintenance.aspx" },
                new ftpFile() { src = "Maintenance.aspx.cs" },
                new ftpFile() { src = "Price.aspx" },
                new ftpFile() { src = "Price.aspx.cs" },
                new ftpFile() { src = "Register.aspx" },
                new ftpFile() { src = "Register.aspx.cs" },
                new ftpFile() { src = "Site.master" },
                new ftpFile() { src = "Site.master.cs" },
                new ftpFile() { src = "Sql.aspx" },
                new ftpFile() { src = "Sql.aspx.cs" },
                new ftpFile() { src = "Test.aspx" },
                new ftpFile() { src = "Test.aspx.cs" },
                new ftpFile() { src = "TrayApp.aspx" },
                new ftpFile() { src = "TrayApp.aspx.cs" },
                new ftpFile() { src = "Upgrade.aspx" },
                new ftpFile() { src = "Upgrade.aspx.cs" },
                new ftpFile() { src = "NiceApiLibrary.dll", sub = "bin" },
                new ftpFile() { src = "NiceApiLibrary_low.dll", sub = "bin" },
                new ftpFile() { src = "APIactualSending.cs", sub = "App_Code" },
                new ftpFile() { src = "BitcoinPrice.cs", sub = "App_Code" },
                new ftpFile() { src = "BundleConfig.cs", sub = "App_Code" },
                new ftpFile() { src = "ConstantStrings.cs", sub = "App_Code" },
                new ftpFile() { src = "CountryListLoader.cs", sub = "App_Code" },
                new ftpFile() { src = "DataFile_Base.cs", sub = "App_Code" },
                new ftpFile() { src = "DataFile_Loopback.cs", sub = "App_Code" },
                new ftpFile() { src = "DataFile_ScreenShot.cs", sub = "App_Code" },
                new ftpFile() { src = "KeywordLoader.cs", sub = "App_Code" },
                new ftpFile() { src = "MyLog.cs", sub = "App_Code" },
                new ftpFile() { src = "RouteConfig.cs", sub = "App_Code" },
                new ftpFile() { src = "SessionData.cs", sub = "App_Code" },
                new ftpFile() { src = "Startup.Auth.cs", sub = "App_Code" },
                new ftpFile() { src = "Startup.cs", sub = "App_Code" },
                new ftpFile() { src = "XmlHelper.cs", sub = "App_Code" },
            };

            list.AddRange(last);
            return list;
        }
    }

    internal class ftpFile
    {
        public string src { get; set; }
        public string sub { get; set; }
    }

    internal class cUrlCmd
    {
        public string pre { get; set; }
        public string pos { get; set; }
        public string srcPath { get; set; }
        public string dstPath { get; set; }
        public string batName { get; set; }
    }
}
