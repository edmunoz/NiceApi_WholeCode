using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
    
namespace NiceApiLibrary_low
{
    public delegate void d1_Data_AppUserFile(Data_AppUserFile d);
    public delegate void d_On_User_Action(Data_AppUserFile user, Object args);
    public delegate void d_On_User_PostAction(Object args);

    public interface IData_AppUserFileHandling
    {
        String GetInfo();

        void Update_General(string email, d_On_User_Action action, Object args, d_On_User_PostAction postAction, IMyLog log);

        void StoreNew(Data_AppUserFile data, out bool fileArleadyUsed, IMyLog log);
        bool HasAccount(string email, IMyLog log);

        TelListController GetCurrentTelList(IMyLog log);
        Data_AppUserFile RetrieveOne(string email, IMyLog log);
        void RetrieveAll(Data_AppUserFile.SortType sort, d1_Data_AppUserFile d, IMyLog log);

    }

    public partial class Data_AppUserFile
    {
        #region statics
        public static string GetAccountStatusExplanationHtml()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0 ; i < (int)eUserStatus._end ; i++)
            {
                sb.AppendLine(i.ToString() + "=" + GetSimpleStatusText((eUserStatus)i) + "<br>");
            }
            return sb.ToString();
        }

        public static eUserStatus GetStatusFromText(string text)
        {
            for (int i1 = 0; i1 < (int)eUserStatus._end; i1++)
            {
                eUserStatus u = (eUserStatus)i1;
                string sNice = GetSimpleStatusText(u);

                if (text == GetSimpleStatusText((eUserStatus)i1))
                {
                    return (eUserStatus)i1;
                }
            }
            int i2 = Int32.Parse(text);
            if (i2 < (int)eUserStatus._end)
            {
                return (eUserStatus)i2;
            }
            throw new ArgumentException();
            //return eUserStatus._end;
        }

        #region static checks

        public static string API_ToId(string email, Guid guid)
        {
            List<byte> byteList = new List<byte>();
            byteList.AddRange(guid.ToByteArray());
            byteList.AddRange(Encoding.ASCII.GetBytes(EmailSaveChars(email)));
            string r = Convert.ToBase64String(byteList.ToArray());
            return r;
        }

        public static string API_IdToEmail(string b64Id)
        {
            try
            {
                byte[] bId = Convert.FromBase64String(b64Id);
                string email = Encoding.ASCII.GetString(bId, 16, bId.Length - 16);
                return email;
            }
            catch (SystemException)
            {
            }
            return null;
        }

        public static string EmailSaveChars(string email)
        {
            return email.Replace("@", "_at_").Replace(".", "_dot_");
        }

        public static string EmailToRealEmail(string emailWithSaveChars)
        {
            return emailWithSaveChars.Replace("_at_", "@").Replace("_dot_", ".");
        }

        public static string Check_UserName(string userName) 
        {
            if (userName.Length < 3)
            {
                return "UserName is too short.";
            }
            if (!userName.My_OnlyLetterDigitAndSpace())
            {
                return "UserName contains illegal characters. Use letters, digits and space only.";
            }
            return null;
        }

        public static string Check_MobileNumber(string mobileNumber, int id)
        {
            if ((mobileNumber == null) || (mobileNumber.Length == 0))// string.IsNullOrWhiteSpace(mobileNumber))
            {
                // ok
                return null;
            }
            else if (!mobileNumber.My_MobileNumber())
            {
                return "MobileNumber " + (id).ToString() + " is invalid.";
            }
            return null;
        }

        public static string Check_Email(string email)
        {
            if (!email.My_Email())
            {
                return "Email is invalid";
            }
            return null;
        }

        public static string Check_Password(string pwd)
        {
            if (pwd.Length < 6)
            {
                return "Password needs to be at least 6 characters long.";
            }
            return null;
        }

#endregion

        public static string GetNiceStatusText(eUserStatus s)
        {
            switch (s)
            {
                case eUserStatus.free_account: return "Free Account";
                case eUserStatus.commercial_payassent: return "PayAsSent Account";
                case eUserStatus.commercial_monthly: return "Monthly Account";
                case eUserStatus.commercial_monthlyDifPrice: return "Monthly Flex Account";
                case eUserStatus.commercial_systemDuplication: return "SystemDuplication Account";
                default:
                    return GetSimpleStatusText(s);
            }
        }

        public static string GetSimpleStatusText(eUserStatus s)
        {
            return s.ToString().Replace("_", " ");
        }

        #endregion

        private Data_AppUserFile(string UserName, bool NoSpam, string MoblieAllNumbers, string Email, string Password, string CreationIp)
        {
            InitWithDefaults();
            this.UserName = UserName.Trim();
            this.NoSpam = NoSpam;
            this.MobileNumbers_AllUnConfirmed__ = new MobileNoHandler(MoblieAllNumbers);
            this.Email = Email.Trim();
            this.Password = Password.Trim();

            this.CreationDate = DateTime.UtcNow.Ticks;
            this.CreationIp = CreationIp;
            this.ApiGuId = API_ToId(Email, Guid.NewGuid());
        }
            
        private Data_AppUserFile()
        {
            InitWithDefaults();
        }

        public static Data_AppUserFile Create(string UserName, bool NoSpam, string MoblieAllNumbers, string Email, string Password, string CreationIp)
        {
            return new Data_AppUserFile(UserName, NoSpam, MoblieAllNumbers, Email, Password, CreationIp);
        }

        public static Data_AppUserFile CreateBlank()
        {
            return new Data_AppUserFile();
        }

        public override string ToString()
        {
            throw new NotImplementedException("to check");//todo
            //Data_AppUserFile_CheckerBase checker = this.GetCheckerBase();
            //if (checker == null)
            //{
            //    return "???";
            //}
            //return checker.Info(this);
        }
    }

    public class MobileNoHandler
    {
        private List<string> List;
        private bool ThrowOnBadData;

        public string getVal
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s1 in List)
                {
                    sb.Append(s1);
                }
                return sb.ToString();
            }
        }

        private string[] checkData(string strDirty)
        {
            List<string> ret = new List<string>();
            string[] dirtArray = strDirty
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "")
                .Split(new char[] {'+'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string dirtOne in dirtArray)
            {
                string cleanOne = "";
                foreach (char c1 in dirtOne)
                {
                    if (Char.IsDigit(c1))
                    {
                        // good
                        cleanOne += c1;
                    }
                    else
                    {
                        // bad character
                        if (this.ThrowOnBadData)
                        {
                            throw new ArgumentException("Invalid character used for telephone number.");
                        }
                    }
                }
                ret.Add("+" + cleanOne);
            }
            return ret.ToArray();
        }

        private void doActualAddingAvoidDoubles(string[] cleanArray)
        {
            foreach (string clean1 in cleanArray)
            {
                if (!List.Contains(clean1))
                {
                    List.Add(clean1);
                }
            }
        }

        public MobileNoHandler(string str, bool ThrowOnBadData)
        {
            this.ThrowOnBadData = ThrowOnBadData;
            this.List = new List<string>();
            doActualAddingAvoidDoubles(checkData(str));
        }
        public MobileNoHandler(string str)
        {
            this.ThrowOnBadData = false;
            this.List = new List<string>();
            doActualAddingAvoidDoubles(checkData(str));
        }

        public void Add(string newTel)
        {
            doActualAddingAvoidDoubles(checkData(newTel));
        }

        public void AddIfNew(string newTel)
        {
            doActualAddingAvoidDoubles(checkData(newTel));
        }

        public void Remove(string oldTel)
        {
            if (List.Contains(oldTel))
            {
                List.Remove(oldTel);
            }
        }

        public bool Contains(string Tel)
        {
            return List.Contains(Tel);
        }

        public string RemoveAndReturnFirst()
        {
            string ret = MobileNumberX(0);
            if (ret.Length > 0)
            {
                Remove(ret);
            }
            return ret;
        }

        public string MobileNumberX(int id)
        {
            if (MobileNumbersCount > id)
            {
                // good
                return List[id];
            }
            else
            {
                // out of index
                return string.Empty;
            }
        }

        public string MobileNumberX_AsZapi(int id)
        {
            return "zapi_" + MobileNumberX(id).Replace("+", "");
        }

        public int MobileNumbersCount
        {
            get
            {
                return List.Count;
            }
        }

        public string[] MobileNumberArray
        {
            get
            {
                return List.ToArray();
            }
        }

        public string MainTelNo
        {
            get
            {
                if (List.Count > 0)
                {
                    return List[0];
                }
                return "N/A";
            }
        }

        public string CounteyCode
        {
            get
            {
                if (List.Count > 0)
                {
                    return List[0].Substring(1, 2);;
                }
                return "N/A";
            }
        }
    }

    public class MobileNoHandlerWithUserName
    {
        public string UserName;
        public MobileNoHandler Handler;

        public MobileNoHandlerWithUserName(string user)
        {
            UserName = user;
            Handler = new MobileNoHandler("");
        }
    }

    public class MobileNoWithState
    {
        public string Number;
        public bool IsConfirmed;

        public MobileNoWithState(string no, bool confirmed)
        {
            Number = no;
            IsConfirmed = confirmed;
        }
    }

    public class MobileHandleConfUnconfList
    {
        Dictionary<string, MobileNoWithState> m_Dic;

        public MobileHandleConfUnconfList()
        {
            m_Dic = new Dictionary<string, MobileNoWithState>();
        }

        public void SortAndReturn(out MobileNoHandler confirmed, out MobileNoHandler unconfirmed, out string status)
        {
            confirmed = new MobileNoHandler("");
            unconfirmed = new MobileNoHandler("");
            status = "";

            int confirmedCounter = 0;
            int unconfirmedCounter = 0;
            string[] keysToSort = new string[m_Dic.Keys.Count];
            m_Dic.Keys.CopyTo(keysToSort, 0);
            List<string> keysList = new List<string>(keysToSort);
            keysList.Sort();
            foreach (string k1 in keysList)
            {
                var kv = m_Dic[k1];
                if (kv.IsConfirmed)
                {
                    confirmed.Add(kv.Number);
                    confirmedCounter++;
                }
                else
                {
                    unconfirmed.Add(kv.Number);
                    unconfirmedCounter++;
                }
            }
            status = string.Format("{0} confirmed and {1} unconfirmed numbers.", confirmedCounter, unconfirmedCounter);
        }

        public void Add(string tels, bool isConfirmed)
        {
            MobileNoHandler h = new MobileNoHandler(tels);
            string nextNo = "";
            while ((nextNo = h.RemoveAndReturnFirst()).Length > 0)
            {
                string err = Data_AppUserFile.Check_MobileNumber(nextNo, 0);
                if (err == null)
                {
                    if (!m_Dic.ContainsKey(nextNo))
                    {
                        m_Dic.Add(nextNo, new MobileNoWithState(nextNo, isConfirmed));
                    }
                    else
                    {
                        // already there
                        m_Dic[nextNo].IsConfirmed = isConfirmed;
                    }
                }
            }
        }

        public bool Conrtains(string tel)
        {
            if (!m_Dic.ContainsKey(tel))
            {
                return false;
            }
            return true;
        }

        public void Remove(string tels)
        {
            MobileNoHandler h = new MobileNoHandler(tels);
            string nextNo = "";
            while ((nextNo = h.RemoveAndReturnFirst()).Length > 0)
            {
                if (m_Dic.ContainsKey(nextNo))
                {
                    m_Dic.Remove(nextNo);
                }
            }
        }
    }
}
