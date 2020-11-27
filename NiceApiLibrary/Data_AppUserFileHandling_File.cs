using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NiceApiLibrary_low;


namespace NiceApiLibrary
{
    public class Data_AppUserFileHandling_File : IData_AppUserFileHandling
    {
        public String GetInfo()
        {
            return FolderNames.GetFolder(NiceSystemInfo.DEFAULT, MyFolders.ASP_UserAccountFolder_);
        }

        public void Update_General(string email, d_On_User_Action action, Object args, d_On_User_PostAction postAction, IMyLog log)
        {
            string filePath =
                FolderNames.GetFolder(NiceSystemInfo.DEFAULT, MyFolders.ASP_UserAccountFolder_) +
                Path.DirectorySeparatorChar +
                Data_AppUserFile.EmailSaveChars(email) + ".txt";

            using (Stream stream = OpenFile.ForRead(filePath, true, true, log))
            {
                Data_AppUserFile r = null;

                if (stream != null)
                {
                    r = Data_AppUserFile.CreateBlank();
                    //1) read it in
                    BinaryReader br = new BinaryReader(stream);
                    r.ReadFromStream(br);
                }

                //2) update values
                if (action != null)
                {
                    action(r, args);
                }

                //3) write it back
                if (r != null)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    BinaryWriter bw = new BinaryWriter(stream);
                    r.WriteToStream(bw);
                    stream.SetLength(stream.Position);
                    stream.Close();
                }
            }
            if (postAction !=null)
            {
                postAction(args);
            }
        }

        public bool HasAccount(string email, IMyLog log)
        {
            string folder = FolderNames.GetFolder(NiceSystemInfo.DEFAULT, MyFolders.ASP_UserAccountFolder_);
            string file = folder + Path.DirectorySeparatorChar + Data_AppUserFile.EmailSaveChars(email) + ".txt";

            if (File.Exists(file))
            {
                return true;
            }
            return false;
        }

        public TelListController GetCurrentTelList(IMyLog log)
        {
            TelListController ret = new TelListController();
            using (var _lock = ret.GetLock())
            {
                RetrieveAll(Data_AppUserFile.SortType.DontSort,
                    delegate(Data_AppUserFile d)
                    {
                        TelListController.currentTelListBuilder(d, _lock);
                    }, log);
            }
            return ret;
        }

        public void StoreNew(Data_AppUserFile data, out bool fileArleadyUsed, IMyLog log)
        {
            fileArleadyUsed = false;
            string folder = FolderNames.GetFolder(NiceSystemInfo.DEFAULT, MyFolders.ASP_UserAccountFolder_);
            string file = folder + Path.DirectorySeparatorChar + Data_AppUserFile.EmailSaveChars(data.Email) + ".txt";

            if (File.Exists(file))
            {
                fileArleadyUsed = true;
                return;
            }

            try
            {
                using (BinaryWriter bw = new BinaryWriter(OpenFile.ForWrite(file, log)))
                {
                    data.WriteToStream(bw);
                }
            }
            catch (IOException)
            { }
        }

        public Data_AppUserFile RetrieveOne(string email, IMyLog log)
        {
            string folder = FolderNames.GetFolder(NiceSystemInfo.DEFAULT, MyFolders.ASP_UserAccountFolder_);
            string filePath = folder + Path.DirectorySeparatorChar + Data_AppUserFile.EmailSaveChars(email) + ".txt";

            Data_AppUserFile o = Data_AppUserFile.CreateBlank();
            Stream stream = OpenFile.ForRead(filePath, false, false, log);
            if (stream == null)
            {
                return null;
            }
            using (BinaryReader br = new BinaryReader(stream))
            {
                o.ReadFromStream(br);
            }
            return o;
        }

        private static int ComparisonUsage(Data_AppUserFile x, Data_AppUserFile y)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append($"{x.AccountStatus.ToString()} vs {y.AccountStatus.ToString()}");
            int ret = 0;
            // 1) Test account types
            if (ret == 0)
            {
                ret = 0 - CompareAccountStatus(x, y);
            }
            if (ret == 0)
            {
                int ix = x.GetCheckerBase(true).AccountImportance1();
                int iy = y.GetCheckerBase(true).AccountImportance1();
                ret = iy - ix;
            }
            if (ret == 0)
            {
                int ix = x.GetCheckerBase(true).AccountImportance2();
                int iy = y.GetCheckerBase(true).AccountImportance2();
                ret = iy - ix;
            }
            if (ret == 0)
            {
                ret = y.UsedInPercent() - x.UsedInPercent();
            }
            //System.Diagnostics.Debug.WriteLine($"{ret} on {sb.ToString()}");
            return ret;
        }

        private static int CompareAccountStatus(Data_AppUserFile x, Data_AppUserFile y)
        {
            return x.AccountStatus.Importance() - y.AccountStatus.Importance();
        }

        private static int ComparisonDate(Data_AppUserFile x, Data_AppUserFile y)
        {
            int r = DateTime.Compare(new DateTime(y.CreationDate), new DateTime(x.CreationDate));
            return r;
        }
        private static int ComparisonState(Data_AppUserFile x, Data_AppUserFile y)
        {
            int r = (int)x.AccountStatus - (int)y.AccountStatus;
            if (r == 0)
            {
                r = ComparisonDate(x, y);
            }
            return r;
        }
        private static int ComparisonEmail(Data_AppUserFile x, Data_AppUserFile y)
        {
            int r = String.Compare(x.Email, y.Email);
            return r;
        }


        private static void Sort(List<Data_AppUserFile> allUsers, Data_AppUserFile.SortType SortBy)
        {
            switch (SortBy)
            {
                case Data_AppUserFile.SortType.Date:
                    // sort by date
                    allUsers.Sort(ComparisonDate);
                    break;

                case Data_AppUserFile.SortType.State:
                    allUsers.Sort(ComparisonState);
                    break;

                case Data_AppUserFile.SortType.Usage:
                    allUsers.Sort(ComparisonUsage);
                    break;

                case Data_AppUserFile.SortType.DontSort:
                    break;

                default:
                    //Email
                    allUsers.Sort(ComparisonEmail);
                    break;
            }
        }

        public void RetrieveAll(Data_AppUserFile.SortType sort, d1_Data_AppUserFile d, IMyLog log)
        {
            List<Data_AppUserFile> allUsers = new List<Data_AppUserFile>();
            foreach (string s1 in Directory.GetFiles(FolderNames.GetFolder(NiceSystemInfo.DEFAULT, MyFolders.ASP_UserAccountFolder_)))
            {
                string file = Path.GetFileNameWithoutExtension(s1);
                Data_AppUserFile u1 = this.RetrieveOne(file, log);
                allUsers.Add(u1);
            }
            Sort(allUsers, sort);
            foreach (Data_AppUserFile u1 in allUsers)
            {
                if (d != null)
                {
                    d(u1);
                }
            }
        }
    }
}
