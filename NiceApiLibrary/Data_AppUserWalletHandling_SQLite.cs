using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Data.SQLite;
using System.Data;
using NiceApiLibrary_low;

//namespace NiceApiLibrary
//{
//    class Data_AppUserWalletHandling_SQLite : IData_AppUserWalletHandling, IsqLiteTable
//    {
//        #region table
//        public static readonly string fEmail = "Email";
//        public static readonly string fTitle = "Title";
//        public static readonly string fDisplayLines = "DisplayLines";
//        public static readonly string fRequestedType = "RequestedType";
//        public static readonly string fCostNumbers = "CostNumbers";
//        public static readonly string fCostMessages = "CostMessages";
//        public static readonly string fCostMonth = "CostMonth";
//        public static readonly string fCostSetup = "Setup";

//        public MultiThreadAbledConnection Connection { get { return sqLiteConnectionHolder.once.db_All; } }

//        public string TableName { get { return "Wallets"; } }

//        private static void custom_Trans_Value(bool toDatabase, MyFieldInfo fieldInfo, ref object csHoldingObject, ref object dbObject)
//        {
//            if (!toDatabase)
//            {
//                // from Database
//                if (fieldInfo.FieldType == typeof(System.String[]))
//                {
//                    string[] sa = StringListParser.Parse((string)dbObject);
//                    fieldInfo.SetValue(csHoldingObject, sa);
//                }
//                else if (fieldInfo.FieldType == typeof(Data_AppUserFile.eUserStatus))
//                {
//                    int i = (int)dbObject;
//                    Data_AppUserFile.eUserStatus us = (Data_AppUserFile.eUserStatus)i;
//                    fieldInfo.SetValue(csHoldingObject, us);
//                }
//                else if (fieldInfo.FieldType == typeof(AmountAndPrice))
//                {
//                    AmountAndPrice ap = AmountAndPrice.FromDBString((string)dbObject);
//                    fieldInfo.SetValue(csHoldingObject, ap);
//                }
//                else
//                {

//                }
//            }
//            else
//            {
//                // to database
//                if (fieldInfo.FieldType == typeof(System.String[]))
//                {
//                    string[] vsa = (string[])fieldInfo.GetValue(csHoldingObject);
//                    dbObject = StringListParser.ToDataString(vsa);
//                }
//                else if (fieldInfo.FieldType == typeof(Data_AppUserFile.eUserStatus))
//                {
//                    Data_AppUserFile.eUserStatus us = (Data_AppUserFile.eUserStatus)fieldInfo.GetValue(csHoldingObject);
//                    Int32 i = (int)us;
//                    dbObject = i;
//                }
//                else if (fieldInfo.FieldType == typeof(AmountAndPrice))
//                {
//                    AmountAndPrice ap = (AmountAndPrice)fieldInfo.GetValue(csHoldingObject);
//                    dbObject = AmountAndPrice.ToDBString(ap);
//                }
//                else
//                {

//                }
//            }
//        }
        
//        public List<sqLiteTableField> Fields
//        {
//            get
//            {
//                return new List<sqLiteTableField>() {
//                    new sqLiteTableField(fEmail, "TEXT UNIQUE", "Email"),
//                    new sqLiteTableField(fTitle, "TEXT", "Title"),
//                    new sqLiteTableField(fDisplayLines, "TEXT", "DisplayLines", null, custom_Trans_Value),
//                    new sqLiteTableField(fRequestedType, "INT", "RequestedType", null, custom_Trans_Value),
//                    new sqLiteTableField(fCostNumbers, "TEXT", "Numbers", null, custom_Trans_Value),
//                    new sqLiteTableField(fCostMessages, "TEXT", "Messages", null, custom_Trans_Value),
//                    new sqLiteTableField(fCostMonth, "TEXT", "Month", null, custom_Trans_Value),
//                    new sqLiteTableField(fCostSetup, "TEXT", "Setup", null, custom_Trans_Value),
//                };
//            }
//        }

//        public string AtCreateSql { get { return null; } }

//        public object Translate_DB2Object_Create()
//        {
//            return Data_AppUserWallet.CreateBlank();
//        }
//        #endregion

//        public Data_AppUserWalletHandling_SQLite(IMyLog log)
//        {
//            sqLiteSQLHelper.createIfNotExist(this, log);
//        }

//        public String GetInfo()
//        {
//            return "SQLite " + TableName;
//        }

//        public void UpdateAll(
//            string Email,
//            string Title,
//            string[] DisplayLines,
//            Data_AppUserFile.eUserStatus RequestedType,
//            AmountAndPrice Numbers,
//            AmountAndPrice Messages,
//            AmountAndPrice Month,
//            AmountAndPrice Setup,
//            IMyLog log)
//        {
//            try
//            {
//                //1) read
//                Data_AppUserWallet r = null;
//                string where = string.Format("WHERE {0} = '{1}'", fEmail, Data_AppUserFile.EmailToRealEmail(Email));
//                sqLiteSQLHelper.buildObjects(
//                    sqLiteSQLHelper.select(this, where, log),
//                    this,
//                    null,
//                    delegate(object built)
//                    {
//                        r = (Data_AppUserWallet)built;
//                    });

//                //2) call action / update values
//                r.Email = Email;
//                r.Title = Title;
//                r.DisplayLines = DisplayLines.MyClone();
//                r.RequestedType = RequestedType;
//                r.Numbers = Numbers.Clone();
//                r.Messages = Messages.Clone();
//                r.Month = Month.Clone();
//                r.Setup = Setup.Clone();

//                //3) update
//                if (r != null)
//                {
//                    sqLiteSQLHelper.update(r, this, null, where, log);
//                }
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//        }

//        public void StoreNew(Data_AppUserWallet data, out bool fileArleadyUsed, IMyLog log)
//        {
//            fileArleadyUsed = false;
//            try
//            {
//                if (HasAccount(data.Email, log))
//                {
//                    fileArleadyUsed = true;
//                    return;
//                }
//                sqLiteSQLHelper.insert(data, this, null, log);
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//        }

//        public bool HasAccount(string email, IMyLog log)
//        {
//            try
//            {
//                return sqLiteSQLHelper.select(this, string.Format("WHERE {0} = '{1}'", fEmail, Data_AppUserFile.EmailToRealEmail(email)), log).HasRows;
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//            return false;
//        }

//        public Data_AppUserWallet RetrieveOne(string email, IMyLog log)
//        {
//            try
//            {
//                Data_AppUserWallet ret = null;
//                string where = string.Format("WHERE {0} = '{1}'", fEmail, Data_AppUserFile.EmailToRealEmail(email));
//                sqLiteSQLHelper.buildObjects(
//                    sqLiteSQLHelper.select(this, where, log),
//                    this,
//                    null,
//                    delegate(object built)
//                    {
//                        ret = (Data_AppUserWallet)built;
//                    });
//                return ret;
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//            return null;
//        }

//        public void RetrieveAll(d1_Data_AppUserWallet d, IMyLog log)
//        {
//            try
//            {
//                sqLiteSQLHelper.buildObjects(
//                    sqLiteSQLHelper.select(this, string.Format("ORDER BY {0} ASC", fEmail), log),
//                    this,
//                    null,
//                    delegate(object built)
//                    {
//                        Data_AppUserWallet bd = (Data_AppUserWallet)built;
//                        d(bd);
//                    });
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//        }

//        public void DeleteOne(string email, IMyLog log)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    class Data_SqlHandling_SQLite : IData_SqlHandling
//    {
//        IMyLog log;

//        public Data_SqlHandling_SQLite(IMyLog log)
//        {
//            this.log = log;
//        }

//        public object ProcessSql(string sqlQuery)
//        {
//            throw new NotImplementedException("SQLiteDataReader2Table missing");
//            //MultiThreadAbledConnection database = sqLiteConnectionHolder.once.db_All;
//            //using (var l = database.GetLocked())
//            //{
//            //    using (SQLiteCommandWithLog cmd = new SQLiteCommandWithLog(sqlQuery, l.con, log))
//            //    {
//            //        SQLiteDataReader exec = cmd.ExecuteReader();
//            //        DataTable ret = exec.SQLiteDataReader2Table();
//            //        return ret;
//            //    }
//            //}
//        }
//    }
//}

