using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    public class Data_AppUserWalletHandling_File : IData_AppUserWalletHandling
    {
        public String GetInfo()
        {
            return FolderNames.GetFolder(NiceSystemInfo.DEFAULT, MyFolders.ASP_UserWalletFolder_);
        }

        public static string GetEmailFromFileName(string fileName)
        {
            return fileName
                .Replace("Wallet_", "")
                .Replace(".txt", "")
                .Replace("_at_", "@")
                .Replace("_dot_", ".");
        }

        private static string getWalletFullPath(string email)
        {
            string filePath =
                FolderNames.GetFolder(NiceSystemInfo.DEFAULT, MyFolders.ASP_UserWalletFolder_) +
                Path.DirectorySeparatorChar + "Wallet_" +
                Data_AppUserFile.EmailSaveChars(email) + ".txt";
            return filePath;
        }

        public void UpdateAll(
            string Email,
            string Title,
            string[] DisplayLines,
            Data_AppUserFile.eUserStatus RequestedType,
            AmountAndPrice Numbers,
            AmountAndPrice Messages,
            AmountAndPrice Month,
            AmountAndPrice Setup,
            AmountAndPrice FullPayment,
            IMyLog log)
        {
            string filePath = getWalletFullPath(Email);

            using (Stream stream = OpenFile.ForRead(filePath, true, true, log))
            {
                Data_AppUserWallet r = Data_AppUserWallet.CreateBlank();

                //1) read it in
                BinaryReader br = new BinaryReader(stream);
                r.ReadFromStream(br);

                //2) update values
                r.Email = Email;
                r.Title = Title;
                r.DisplayLines = DisplayLines.MyClone();
                r.RequestedType = RequestedType;
                r.Numbers = Numbers.Clone();
                r.Messages = Messages.Clone();
                r.Month = Month.Clone();
                r.Setup = Setup.Clone();
                r.FullPayment = FullPayment.Clone();

                //3) write it back
                stream.Seek(0, SeekOrigin.Begin);
                BinaryWriter bw = new BinaryWriter(stream);
                r.WriteToStream(bw);
            }
        }

        public void StoreNew(Data_AppUserWallet data, out bool fileArleadyUsed, IMyLog log)
        {
            fileArleadyUsed = false;
            string file = getWalletFullPath(data.Email);

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

        public Data_AppUserWallet RetrieveOne(string email, IMyLog log)
        {
            string filePath = getWalletFullPath(email);

            Data_AppUserWallet o = Data_AppUserWallet.CreateBlank();
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

        public void DeleteOne(string email, IMyLog log)
        {
            string filePath = getWalletFullPath(email);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }


        public void RetrieveAll(d1_Data_AppUserWallet d, IMyLog log)
        {
            foreach (string f1 in Directory.GetFiles(FolderNames.GetFolder(NiceSystemInfo.DEFAULT, MyFolders.ASP_UserWalletFolder_)))
            {
                string email = Data_AppUserWalletHandling_File.GetEmailFromFileName(Path.GetFileName(f1));
                Data_AppUserWallet w1 = RetrieveOne(email, log);
                if (w1 != null)
                {
                    d(w1);
                }
            }
        }
    }

    public class Data_SqlHandling_File : IData_SqlHandling
    {
        public object ProcessSql(NiceSystemInfo niceSystem, string sqlQuery)
        {
            throw new SystemException("Not on Data_SqlHandling_File please");
        }
    }
}
