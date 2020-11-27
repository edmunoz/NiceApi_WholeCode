using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
    
namespace NiceApiLibrary_low
{
    public delegate void d1_Data_AppUserWallet(Data_AppUserWallet d);

    public interface IData_AppUserWalletHandling
    {
        String GetInfo();
        void UpdateAll(
            string Email,
            string Title,
            string[] DisplayLines,
            Data_AppUserFile.eUserStatus RequestedType,
            AmountAndPrice Numbers,
            AmountAndPrice Messages,
            AmountAndPrice Month,
            AmountAndPrice Setup,
            AmountAndPrice FullPayment,
            IMyLog log);

        void StoreNew(Data_AppUserWallet data, out bool fileArleadyUsed, IMyLog log);
        Data_AppUserWallet RetrieveOne(string email, IMyLog log);
        void RetrieveAll(d1_Data_AppUserWallet d, IMyLog log);
        void DeleteOne(string email, IMyLog log);
    }

    public interface IData_SqlHandling
    {
        object ProcessSql(NiceSystemInfo niceSystem, string sqlQuery);
    }

    public class Data_AppUserWallet
    {
        public string Email;
        public string Title;
        public string[] DisplayLines;
        public Data_AppUserFile.eUserStatus RequestedType;
        public AmountAndPrice Numbers;
        public AmountAndPrice Messages;
        public AmountAndPrice Month;
        public AmountAndPrice Setup;
        public AmountAndPrice FullPayment;

        private Data_AppUserWallet(
            string Email,
            string Title,
            string[] DisplayLines,
            Data_AppUserFile.eUserStatus RequestedType,
            AmountAndPrice Numbers,
            AmountAndPrice Messages,
            AmountAndPrice Month,
            AmountAndPrice Setup,
             AmountAndPrice FullPayment)
        {
            InitWithDefaults();
            this.Email = Email.Trim();
            this.Title = Title;
            this.DisplayLines = DisplayLines.MyClone();
            this.RequestedType = RequestedType;
            this.Numbers = Numbers.Clone();
            this.Messages = Messages.Clone();
            this.Month = Month.Clone();
            this.Setup = Setup.Clone();
            this.FullPayment = FullPayment.Clone();
        }

        public Data_AppUserWallet(
            Data_AppUserFile.eUserStatus RequestedType,
            AmountAndPrice Numbers,
            AmountAndPrice Messages,
            AmountAndPrice Month,
            AmountAndPrice Setup,
            AmountAndPrice FullPayment
            )
        {
            InitWithDefaults();
            this.RequestedType = RequestedType;
            this.Numbers = Numbers.Clone();
            this.Messages = Messages.Clone();
            this.Month = Month.Clone();
            this.Setup = Setup.Clone();
            this.FullPayment = FullPayment.Clone();
        }

        private Data_AppUserWallet()
        {
            InitWithDefaults();
        }

        private void InitWithDefaults()
        {
            this.Email = string.Empty;
            this.Title = string.Empty;
            this.DisplayLines = new string[0];
            this.RequestedType = Data_AppUserFile.eUserStatus._end;
            this.Numbers = new AmountAndPrice(0, 0M);
            this.Messages = new AmountAndPrice(0, 0M);
            this.Month = new AmountAndPrice(0, 0M);
            this.Setup = new AmountAndPrice(0, 0M);
            this.FullPayment = new AmountAndPrice(0, 0M);
        }

        public static Data_AppUserWallet Create(
            string Email,
            string Title,
            string[] DisplayLines,
            Data_AppUserFile.eUserStatus RequestedType,
            AmountAndPrice Numbers,
            AmountAndPrice Messages,
            AmountAndPrice Month,
            AmountAndPrice Setup,
            AmountAndPrice FullPayment)
        {
            return new Data_AppUserWallet(Email, Title, DisplayLines, RequestedType, Numbers, Messages, Month, Setup, FullPayment);
        }

        public static Data_AppUserWallet CreateBlank()
        {
            return new Data_AppUserWallet();
        }

        public void WriteToStream(BinaryWriter bw)
        {
            // v0 initial version
            // v1 with Fullpayment
            // v ...
            bw.Write((Int32)1);
            bw.Write(this.Email);
            bw.Write(this.Title);
            DisplayLines.WriteToStream(bw);
            bw.Write((Int32)RequestedType);
            Numbers.WriteToStream(bw);
            Messages.WriteToStream(bw);
            Month.WriteToStream(bw);
            Setup.WriteToStream(bw);
            FullPayment.WriteToStream(bw);
        }

        public void ReadFromStream(BinaryReader br)
        {
            Int32 v = br.ReadInt32();
            switch (v)
            {
                case 0: ReadFromStream_V0(br); break;
                case 1: ReadFromStream_V1(br); break;

                default: throw new NotSupportedException("File invalid format");
            }
        }
        private void ReadFromStream_V1(BinaryReader br)
        {
            ReadFromStream_V0(br);
            this.FullPayment = new AmountAndPrice(br);
        }
        private void ReadFromStream_V0(BinaryReader br)
        {
            this.Email = br.ReadString();
            this.Title = br.ReadString();
            this.DisplayLines = DisplayLines.ReadFromStream(br);
            this.RequestedType = (Data_AppUserFile.eUserStatus)br.ReadInt32();
            this.Numbers = new AmountAndPrice(br);
            this.Messages = new AmountAndPrice(br);
            this.Month = new AmountAndPrice(br);
            this.Setup = new AmountAndPrice(br);
        }

    }

    public class PriceAndText
    {
        public Decimal FinalPrice = 0M;
        public string Explained = string.Empty;
    }

    public class AmountAndPrice
    {
        public int Amount;
        public Decimal Price;

        public static string ToDBString(AmountAndPrice val)
        {
            return string.Format("{0}|{1:0.00}", val.Amount, val.Price);
        }
        public static AmountAndPrice FromDBString(string val)
        {
            try
            {
                string[] a = val.Split(new char[] { '|' });
                AmountAndPrice r = new AmountAndPrice(int.Parse(a[0]), decimal.Parse(a[1]));
                return r;
            }
            catch { }
            return null;
        }

        public string ToString(string Title)
        {
            return string.Format("{0}: Amount:{1} Price:{2:0.00}", Title, Amount, Price);
        }

        public AmountAndPrice(int Amount, Decimal Price)
        {
            this.Amount = Amount;
            this.Price = Price;
        }
        public AmountAndPrice(BinaryReader br)
        {
            ReadFromStream(br);
        }

        public AmountAndPrice Clone()
        {
            return new AmountAndPrice(Amount, Price);
        }

        public void WriteToStream(BinaryWriter bw)
        {
            bw.Write((Int32)Amount);
            bw.Write(Price);
        }

        public void ReadFromStream(BinaryReader br)
        {
            Amount = br.ReadInt32();
            Price = br.ReadDecimal();
        }
    }
}
