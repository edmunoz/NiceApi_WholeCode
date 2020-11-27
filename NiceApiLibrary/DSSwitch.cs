using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Threading;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    public class DSSwitch
    {
        public enum eDataSource
        {
            File,
            SQLite,
        }

        private static bool underMaintenance = true;
        private static void checkStarted()
        {
            if (underMaintenance)
            {
                throw new DataUnavailableException("Maintenance. Please try again in 5 minutes.");
            }
        }

        public static void OnDataUnavailableException(Page page)
        {
            page.Response.Redirect("~/Maintenance.aspx");
        }

        private static MyMemoryLog maintenanceLog = null;
        public static string GetMaintenanceLog()
        {
            if (maintenanceLog != null)
            {
                string r = maintenanceLog.GetData();
                return r;
            }
            return "???";
        }

        private static IData_Full _data_Full = null;

        public static IData_Full full()
        {
            return _data_Full;
        }

        /* appUser */
        public static IData_AppUserFileHandling appUser()
        {
            checkStarted();
            return _data_Full.AppUserFile();
        }

        /* appWallet */
        public static IData_AppUserWalletHandling appWallet()
        {
            checkStarted();
            return _data_Full.AppWalletFile();
        }

        /* sql */
        public static IData_SqlHandling sql()
        {
            checkStarted();
            return _data_Full.Sql(); ;
        }

        /* msgFile00 */
        public static IData_Net__00NormalMessage_store msgFile00()
        {
            checkStarted();
            return _data_Full._00NormalMessage();
        }

        /* msgFile02 */
        public static IData_Net__02ScreenshotRequest_store msgFile02()
        {
            checkStarted();
            return _data_Full._02ScreenshotRequest();
        }

        /* msgFile04 */
        public static IData_Net__04CheckTelNumbers_store msgFile04()
        {
            checkStarted();
            return _data_Full._04CheckTelNumbers();
        }

        private static bool startupCalled = false;
        public static void NiceApiLibrary_StartUp(IMyLog log, bool waitUntilStartupIsDone)
        {
            if (!startupCalled)
            {
                startupCalled = true;
                DSSwitch.StartUp(log, waitUntilStartupIsDone);
            }
        }

        public static void NiceApiLibrary_End()
        {
            DSSwitch.End();
        }


        internal static void StartUp(IMyLog _log, bool waitUntilStartupIsDone)
        {
            maintenanceLog = new MyMemoryLog();

            IMyLog log = new MyManyLog(new List<IMyLog>() { maintenanceLog, _log });
            log.Info("DSSwitch.StartUp");
            System.Reflection.Assembly.GetAssembly(typeof(IMyLog)).GetAssemblyVersion().ToList().ForEach(_ => log.Debug(_));

            ManualResetEvent bgDone = new ManualResetEvent(false);

            Thread bg = new Thread(
                delegate ()
                {
                    try
                    {
                        log.Debug("This is the startup background thread.");
                        log.Debug(OperatingSystemInfo.Get().ToString());
                        log.Debug(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        log.Debug(log.GetLoggerInfo());
                        log.Debug("Reading instruction file");
                        string instruction = "IniAndRunFile";
                        try
                        {
                            //instruction =  
                            //    File.ReadAllText(
                            //        FolderNames.GetFolder(MyFolders.ASP_ServerStateFolder) + "\\StartupInstruction.txt")
                            //        .Trim()
                            //        .Replace("\r", "")
                            //        .Replace("\n", "")
                            //        .Replace("\t", "")
                            //        .Replace(" ", "");
                        }
                        catch (SystemException)
                        {
                            log.Error("Unable to read instruction file, using default values.");
                        }
                        log.Debug("=> " + instruction);
                        foreach (string i1 in instruction.Split(new char[] { ',' }))
                        {
                            log.Debug("-> " + i1);
                            switch (i1)
                            {
                                case "Wait":
                                    try
                                    {
                                        log.Debug("Waiting...");
                                        Thread.Sleep(5000);
                                        log.Debug("Waiting done");
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(ex.ToString());
                                    }
                                    break;

                                //case "SQLiteLogFilename":
                                //    try
                                //    {
                                //        log.Debug(FolderNames.GetFolder(MyFolders.sqlite_All));
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        log.Error(ex.ToString());
                                //    }
                                //    break;

                                //case "SQLiteTestStartup":
                                //    try
                                //    {
                                //        log.Debug(TestStartUp());
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        log.Error(ex.ToString());
                                //    }
                                //    break;

                                //case "SQLiteExists":
                                //    try
                                //    {
                                //        if (File.Exists(FolderNames.GetFolder(MyFolders.sqlite_All)))
                                //        {
                                //            log.Info("SQLite is there.");
                                //        }
                                //        else
                                //        {
                                //            log.Info("SQLite is NOT there.");
                                //        }
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        log.Error(ex.ToString());
                                //    }
                                //    break;

                                //case "CopyToSQLiteIfNotExist":
                                //    try
                                //    {
                                //        if (File.Exists(FolderNames.GetFolder(MyFolders.sqlite_All)))
                                //        {
                                //            log.Info("SQLite is there. So nothing to copy");
                                //        }
                                //        else
                                //        {
                                //            if (sqLiteConnectionHolder.StartUp(log))
                                //            {
                                //                File2DbConverter.Convert(true, log);
                                //            }
                                //        }
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        log.Error(ex.ToString());
                                //    }
                                //    break;

                                //case "IniAndRunSQLite":
                                //    try
                                //    {
                                //        if (sqLiteConnectionHolder.StartUp(log))
                                //        {
                                //            appUser_SQLite = new Data_AppUserFileHandling_SQLite(log);
                                //            appWallet_SQLite = new Data_AppUserWalletHandling_SQLite(log);
                                //            sql_SQLite = new Data_SqlHandling_SQLite(log);
                                //            msg00_SQLite = new Data_Net__00NormalMessage_SQLite(log);
                                //            msg02_SQLite = new Data_Net__02ScreenshotRequest_SQLite(log);
                                //            msg04_SQLite = new Data_Net__04CheckTelNumbers_SQLite(log);
                                //            ChangeDataSource(eDataSource.SQLite);
                                //            underMaintenance = false;
                                //            log.Info("Startup.Running");
                                //        }
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        log.Error(ex.ToString());
                                //    }
                                //    break;

                                case "IniAndRunFile":
                                    try
                                    {
                                        _data_Full = new DataFull_File();

                                        var ini = _data_Full.GetSystems(true);


                                        //appUser_File = new Data_AppUserFileHandling_File();
                                        //appWallet_File = new Data_AppUserWalletHandling_File();
                                        //sql_File = new Data_SqlHandling_File();
                                        //msg00_File = new Data_Net__00NormalMessage_File();
                                        //msg02_File = new Data_Net__02ScreenshotRequest_File();
                                        //msg04_File = new Data_Net__04CheckTelNumbers_File();
                                        //ChangeDataSource(eDataSource.File);
                                        underMaintenance = false;
                                        log.Info("Startup.Running");
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(ex.ToString());
                                    }
                                    break;

                                default:
                                    log.Error("instruction not recognised");
                                    break;
                            }
                        }
                    }
                    catch (SystemException se)
                    {
                        log.Error(se.Message);
                    }
                    bgDone.Set();
                });
            bg.Start();

            if (waitUntilStartupIsDone)
            {
                bgDone.WaitOne();
            }
            log.Info("DSSwitch.StartUp done");
        }
        internal static void End()
        {
            //sqLiteConnectionHolder.End();
        }

        public class File2DbConverter
        {
            public enum eConvertItem
            {
                QueuedMessage00,
                ProcessedMessage00,
                DisposedMessage00,
                AppUserData,
                AppWalletData,
                QueuedScreenShot02,
                QueuedTelCheck04,
                _end
            }
            internal class ProgressLog : IDisposable
            {
                private IMyLog Log;
                private int Count;
                public ProgressLog(IMyLog log)
                {
                    Log = log;
                }
                public void Next()
                {
                    Count++;
                    if (Count % 100 == 0)
                    {
                        Log.Info(Count.ToString());
                    }
                }
                public void Dispose()
                {
                    Log.Info(Count.ToString());
                }
            }
            //public static void Convert(eConvertItem itemToConvert, bool fromFileToDB, IMyLog log)
            //{
            //    if (fromFileToDB)
            //    {
            //        using (ProgressLog progress = new ProgressLog(log))
            //        {
            //            var txn = sqLiteConnectionHolder.once.db_All.BeginTransaction();
            //            switch (itemToConvert)
            //            {
            //                case eConvertItem.AppUserData:
            //                    {
            //                        IData_AppUserFileHandling src = new Data_AppUserFileHandling_File();
            //                        IData_AppUserFileHandling dest = new Data_AppUserFileHandling_SQLite(log);
            //                        src.RetrieveAll(Data_AppUserFile.SortType.DontSort,
            //                            delegate(Data_AppUserFile d)
            //                            {
            //                                progress.Next();
            //                                bool dummyAlreadyExists;
            //                                dest.StoreNew(d, out dummyAlreadyExists, log);
            //                            }, log);
            //                    }
            //                    break;
            //                case eConvertItem.AppWalletData:
            //                    {
            //                        IData_AppUserWalletHandling src = new Data_AppUserWalletHandling_File();
            //                        IData_AppUserWalletHandling dest = new Data_AppUserWalletHandling_SQLite(log);
            //                        src.RetrieveAll(
            //                            delegate(Data_AppUserWallet w)
            //                            {
            //                                progress.Next();
            //                                bool dummyAlreadyExists;
            //                                dest.StoreNew(w, out dummyAlreadyExists, log);
            //                            }, log);
            //                    }
            //                    break;
            //                case eConvertItem.QueuedMessage00:
            //                    {
            //                        IData_Net__00NormalMessage_store src = new Data_Net__00NormalMessage_File();
            //                        IData_Net__00NormalMessage_store dest = new Data_Net__00NormalMessage_SQLite(log);
            //                        src.ForEach(Data_Net__00NormalMessage.eLocation.Queued, log,
            //                            delegate(Data_Net__00NormalMessage msg)
            //                            {
            //                                progress.Next();
            //                                dest.Store(msg, Data_Net__00NormalMessage.eLocation.Queued, log);
            //                            });
            //                    }
            //                    break;

            //                case eConvertItem.ProcessedMessage00:
            //                    {
            //                        IData_Net__00NormalMessage_store src = new Data_Net__00NormalMessage_File();
            //                        IData_Net__00NormalMessage_store dest = new Data_Net__00NormalMessage_SQLite(log);
            //                        src.ForEach(Data_Net__00NormalMessage.eLocation.Processed, log,
            //                            delegate(Data_Net__00NormalMessage msg)
            //                            {
            //                                progress.Next();
            //                                dest.Store(msg, Data_Net__00NormalMessage.eLocation.Processed, log);
            //                            });
            //                    }
            //                    break;

            //                case eConvertItem.DisposedMessage00:
            //                    {
            //                        IData_Net__00NormalMessage_store src = new Data_Net__00NormalMessage_File();
            //                        IData_Net__00NormalMessage_store dest = new Data_Net__00NormalMessage_SQLite(log);
            //                        src.ForEach(Data_Net__00NormalMessage.eLocation.Disposed, log,
            //                            delegate(Data_Net__00NormalMessage msg)
            //                            {
            //                                progress.Next();
            //                                dest.Store(msg, Data_Net__00NormalMessage.eLocation.Disposed, log);
            //                            });
            //                    }
            //                    break;

            //                case eConvertItem.QueuedScreenShot02:
            //                    {
            //                        IData_Net__02ScreenshotRequest_store src = new Data_Net__02ScreenshotRequest_File();
            //                        IData_Net__02ScreenshotRequest_store dest = new Data_Net__02ScreenshotRequest_SQLite(log);
            //                        src.ForEach(log,
            //                            delegate(Data_Net__02ScreenshotRequest msg)
            //                            {
            //                                progress.Next();
            //                                dest.Store(msg, log);
            //                            });
            //                    }
            //                    break;

            //                case eConvertItem.QueuedTelCheck04:
            //                    {
            //                        IData_Net__04CheckTelNumbers_store src = new Data_Net__04CheckTelNumbers_File();
            //                        IData_Net__04CheckTelNumbers_store dest = new Data_Net__04CheckTelNumbers_SQLite(log);
            //                        src.ForEach(log,
            //                            delegate(Data_Net__04CheckTelNumbers msg)
            //                            {
            //                                progress.Next();
            //                                dest.Store(msg, log);
            //                            });
            //                    }
            //                    break;

            //                default:
            //                    throw new SystemException("TODO, what other item is there to convert? " + itemToConvert.ToString());
            //            }
            //            txn.Commit();
            //        }
            //    }
            //    else
            //    {
            //        throw new NotImplementedException("DB to file not done yet");
            //    }
            //}

            //public static void Convert(bool fromFileToDB, IMyLog log)
            //{
            //    Stopwatch sw = new Stopwatch();
            //    for (int i = 0; i < (int)eConvertItem._end; i++)
            //    {
            //        eConvertItem item = (eConvertItem)i;
            //        sw.Reset();
            //        sw.Start();
            //        log.Info("About to convert " + item.ToString());
            //        Convert(item, fromFileToDB, log);
            //        log.Info(item.ToString().PadRight(20) + " converted in " + sw.Elapsed.ToString());
            //    }
            //}

            //public static void ToObjectLines(eConvertItem item, TextWriter tw, IMyLog log)
            //{
            //    switch(item)
            //    {
            //        case eConvertItem.QueuedMessage00:
            //            {
            //                tw.WriteLine("".PadLeft(100, '\''));
            //                tw.WriteLine(DSSwitch.msgFile00().GetInfo() + " Queued");
            //                IsqLiteTable t = (Data_Net__00NormalMessage_SQLite)msg00_SQLite;
            //                DSSwitch.msgFile00().ForEach(Data_Net__00NormalMessage.eLocation.Queued, log, 
            //                    delegate(Data_Net__00NormalMessage msg) 
            //                    {
            //                        ObjectToLinesClass.ObjectToLines(
            //                            msg, 
            //                            t, 
            //                            Data_Net__00NormalMessage.eLocation.Queued, 
            //                            tw);
            //                    });
            //            }
            //            break;

            //        case eConvertItem.ProcessedMessage00:
            //            {
            //                tw.WriteLine("".PadLeft(100, '\''));
            //                tw.WriteLine(DSSwitch.msgFile00().GetInfo() + " Processed");
            //                IsqLiteTable t = (Data_Net__00NormalMessage_SQLite)msg00_SQLite;
            //                DSSwitch.msgFile00().ForEach(Data_Net__00NormalMessage.eLocation.Processed, log,
            //                    delegate(Data_Net__00NormalMessage msg)
            //                    {
            //                        ObjectToLinesClass.ObjectToLines(
            //                            msg,
            //                            t,
            //                            Data_Net__00NormalMessage.eLocation.Processed,
            //                            tw);
            //                    });
            //            }
            //            break;

            //        case eConvertItem.DisposedMessage00:
            //            {
            //                tw.WriteLine("".PadLeft(100, '\''));
            //                tw.WriteLine(DSSwitch.msgFile00().GetInfo() + " Disposed"); 
            //                IsqLiteTable t = (Data_Net__00NormalMessage_SQLite)msg00_SQLite;
            //                DSSwitch.msgFile00().ForEach(Data_Net__00NormalMessage.eLocation.Disposed, log,
            //                    delegate(Data_Net__00NormalMessage msg)
            //                    {
            //                        ObjectToLinesClass.ObjectToLines(
            //                            msg,
            //                            t,
            //                            Data_Net__00NormalMessage.eLocation.Disposed,
            //                            tw);
            //                    });
            //            }
            //            break;
            //        case eConvertItem.AppUserData:
            //            {
            //                tw.WriteLine("".PadLeft(100, '\''));
            //                tw.WriteLine(DSSwitch.appUser().GetInfo());
            //                IsqLiteTable t = (Data_AppUserFileHandling_SQLite)appUser_SQLite;
            //                DSSwitch.appUser().RetrieveAll(Data_AppUserFile.SortType.Date, 
            //                    delegate(Data_AppUserFile u)
            //                    {
            //                        ObjectToLinesClass.ObjectToLines(
            //                            u,
            //                            t,
            //                            null,
            //                            tw);
            //                    }, log);
            //            }
            //            break;

            //        case eConvertItem.AppWalletData:
            //            {
            //                tw.WriteLine("".PadLeft(100, '\''));
            //                tw.WriteLine(DSSwitch.appWallet().GetInfo());
            //                IsqLiteTable t = (Data_AppUserWalletHandling_SQLite)appWallet_SQLite;
            //                DSSwitch.appWallet().RetrieveAll(
            //                    delegate(Data_AppUserWallet w)
            //                    {
            //                        ObjectToLinesClass.ObjectToLines(
            //                            w,
            //                            t,
            //                            null,
            //                            tw);
            //                    }, log);
            //            }
            //            break;

            //        case eConvertItem.QueuedScreenShot02:
            //            {
            //                tw.WriteLine("".PadLeft(100, '\''));
            //                tw.WriteLine(DSSwitch.msgFile02().GetInfo());
            //                IsqLiteTable t = (Data_Net__02ScreenshotRequest_SQLite)msg02_SQLite;
            //                DSSwitch.msgFile02().ForEach(log, 
            //                    delegate(Data_Net__02ScreenshotRequest m)
            //                    {
            //                        ObjectToLinesClass.ObjectToLines(
            //                            m,
            //                            t,
            //                            null,
            //                            tw);
            //                    });
            //            }
            //            break;

            //        case eConvertItem.QueuedTelCheck04:
            //            {
            //                tw.WriteLine("".PadLeft(100, '\''));
            //                tw.WriteLine(DSSwitch.msgFile04().GetInfo());
            //                IsqLiteTable t = (Data_Net__04CheckTelNumbers_SQLite)msg04_SQLite;
            //                DSSwitch.msgFile04().ForEach(log, 
            //                    delegate(Data_Net__04CheckTelNumbers m)
            //                    {
            //                        ObjectToLinesClass.ObjectToLines(
            //                            m,
            //                            t,
            //                            null,
            //                            tw);
            //                    });
            //            }
            //            break;

            //        default:
            //            {

            //            }
            //            //mg throw new SystemException("TODO, what other item is there ToObjectLines? " + item.ToString());
            //            break;

            //    }

            //}

            //public static void ToObjectLines(TextWriter tw, IMyLog log)
            //{
            //    for (int i = 0; i < (int)eConvertItem._end; i++)
            //    {
            //        eConvertItem item = (eConvertItem)i;
            //        ToObjectLines(item, tw, log);
            //    }
            //}
        }
    }

    public class DataUnavailableException : SystemException
    {
        public DataUnavailableException(string message) : base(message) { }
    }

    public class DataFull_File : IData_Full
    {
        private IData_AppUserFileHandling _appUser;
        private IData_AppUserWalletHandling _appWallet;
        private IData_SqlHandling _sql;
        private IData_Net__00NormalMessage_store _00;
        private IData_Net__02ScreenshotRequest_store _02;
        private IData_Net__04CheckTelNumbers_store _04;

        public List<NiceSystemInfo> GetSystems(bool createResources)
        {
            string fileSystems = "FileSystems".AppSettingsGet().Replace("\r", "").Replace("\n", "").Replace("\t", "");
            string[] systems = fileSystems.Split('|');
            List<NiceSystemInfo> ret = new List<NiceSystemInfo>();
            foreach (var system in systems)
            {
                string[] systemSplit = system.Split(new char[] { ',' });
                if (systemSplit.Length != 2)
                {
                    throw new System.Configuration.ConfigurationErrorsException(system);
                }

                NiceSystemInfo a = new NiceSystemInfo();
                a.Name = systemSplit[0];
                a.Default = a.Name == "Default";
                a.APIId = systemSplit[1];
                ret.Add(a);
                if (createResources)
                {
                    NiceApiLibrary_low.FolderNames.CreateFoldersForAsp(a);
                }
            }
            if (ret.FirstOrDefault(_ => _.Default) == null)
            {
                throw new System.Configuration.ConfigurationErrorsException("GetSystems. No Default found.");
            }

            _appUser = new Data_AppUserFileHandling_File();
            _appWallet = new Data_AppUserWalletHandling_File();
            _00 = new Data_Net__00NormalMessage_File();
            _02 = new Data_Net__02ScreenshotRequest_File();
            _04 = new Data_Net__04CheckTelNumbers_File();

            return ret;
        }

        public NotificationInfo GetNotificationInfo()
        {
            // get this info from the app settings
            return new NotificationInfo()
            {
                Name = "NotificationInfo.Name".AppSettingsGet(),
                APIId = "NotificationInfo.APIId".AppSettingsGet(),
                RxTel = "NotificationInfo.RxTel".AppSettingsGet(),
            };
        }

        public void AddSystem(NiceSystemInfo systemInfo)
        {
            throw new NotImplementedException();//todo AddSystem
        }

        public IData_AppUserFileHandling AppUserFile()
        {
            return _appUser;
        }

        public IData_AppUserWalletHandling AppWalletFile()
        {
            return _appWallet;
        }

        public IData_SqlHandling Sql()
        {
            throw new NotImplementedException();//todo Sql
        }

        public IData_Net__00NormalMessage_store _00NormalMessage()
        {
            return _00;
        }

        public IData_Net__02ScreenshotRequest_store _02ScreenshotRequest()
        {
            return _02;
        }

        public IData_Net__04CheckTelNumbers_store _04CheckTelNumbers()
        {
            return _04;
        }

        public NiceSystemType GetNiceSystemType()
        {
            return NiceSystemType.File;
        }

    }
}
