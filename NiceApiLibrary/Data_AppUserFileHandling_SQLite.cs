using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.SQLite;
using NiceApiLibrary_low;

//namespace NiceApiLibrary
//{
//    class Data_AppUserFileHandling_SQLite : IData_AppUserFileHandling, IsqLiteTable
//    {
//        #region table
//        public static readonly string fUserName = "UserName";
//        public static readonly string fTelConfirmed = "TelConfirmed";
//        public static readonly string fTelUnConfirmed = "TelUnConfirmed";
//        public static readonly string fEmail = "Email";
//        public static readonly string fPassword = "Password";
//        public static readonly string fCreationDate = "CreationDate";
//        public static readonly string fCreationIp = "CreationIp";
//        public static readonly string fApiGuId = "ApiGuId";
//        public static readonly string fComment = "Comment";
//        public static readonly string fDeleteOnFailed = "DeleteOnFailed";
//        public static readonly string fAddNumber_AllowedWithAPI = "AddNumber_AllowedWithAPI";
//        public static readonly string fAccountStatus = "AccountStatus";
//        public static readonly string ffree_LastMsgQueued = "free_LastMsgQueued";
//        public static readonly string ffree_MsgSent = "free_MsgSent";
//        public static readonly string ffree_MsgLeft = "free_MsgLeft";
//        public static readonly string ffree_MinDelayInSeconds = "free_MinDelayInSeconds";
//        public static readonly string ffree_SendFooter = "free_SendFooter";
//        public static readonly string ffree_MsgQueued = "free_MsgQueued";
//        public static readonly string fmonthly_LastMsgQueued = "monthly_LastMsgQueued";
//        public static readonly string fmonthly_MsgSent = "monthly_MsgSent";
//        public static readonly string fmonthly_PaidUntil = "monthly_PaidUntil";
//        public static readonly string fmonthly_MinDelayInSeconds = "monthly_MinDelayInSeconds";
//        public static readonly string fmonthly_CostPerNumber = "monthly_CostPerNumber";
//        public static readonly string fmonthly_CurrentCredit = "monthly_CurrentCredit";
//        public static readonly string fpayAsSent_LastMsgQueued = "payAsSent_LastMsgQueued";
//        public static readonly string fpayAsSent_MsgSent = "payAsSent_MsgSent";
//        public static readonly string fpayAsSent_MinDelayInSeconds = "payAsSent_MinDelayInSeconds";
//        public static readonly string fpayAsSent_CostPerNumber = "payAsSent_CostPerNumber";
//        public static readonly string fpayAsSent_CostPerMessage = "payAsSent_CostPerMessage";
//        public static readonly string fpayAsSent_CurrentCredit = "payAsSent_CurrentCredit";

//        public MultiThreadAbledConnection Connection { get { return sqLiteConnectionHolder.once.db_All; } }
//        public string TableName { get { return "Users"; } }
//        private static void custom_trans2DB_getProp(string property, object srcTarget, object auxData, out MyFieldInfo prop, out object outTarget)
//        {
//            outTarget = null;
//            prop = null;
//            if (property.StartsWith("free_"))
//            {
//                Data_AppUserFile user = (Data_AppUserFile)srcTarget;
//                prop = new MyFieldInfo(user.FreeAccount.GetType().GetField(property));
//                outTarget = user.FreeAccount;
//            }
//            else if (property.StartsWith("monthly_"))
//            {
//                Data_AppUserFile user = (Data_AppUserFile)srcTarget;
//                prop = new MyFieldInfo(user.MonthlyAccount.GetType().GetField(property));
//                outTarget = user.MonthlyAccount;
//            }
//            else if (property.StartsWith("payAsSent_"))
//            {
//                Data_AppUserFile user = (Data_AppUserFile)srcTarget;
//                prop = new MyFieldInfo(user.PayAsSentAccount.GetType().GetField(property));
//                outTarget = user.PayAsSentAccount;
//            }
//        }

//        private static void custom_Trans_Value(bool toDatabase, MyFieldInfo fieldInfo, ref object csHoldingObject, ref object dbObject)
//        {
//            if (!toDatabase)
//            {
//                // from Database
//                if (fieldInfo.FieldType == typeof(MobileNoHandler))
//                {
//                    MobileNoHandler m = new MobileNoHandler((string)dbObject);
//                    fieldInfo.SetValue(csHoldingObject, m);
//                }
//                else if (fieldInfo.FieldType == typeof(System.Int64))
//                {
//                    if ((fieldInfo.Name == "CreationDate")
//                        || (fieldInfo.Name == "free_LastMsgQueued")
//                        || (fieldInfo.Name == "monthly_LastMsgQueued")
//                        || (fieldInfo.Name == "monthly_PaidUntil")
//                        || (fieldInfo.Name == "payAsSent_LastMsgQueued"))
//                    {
//                        Int64 v64 = ((string)dbObject).SQLTimeToInt();
//                        fieldInfo.SetValue(csHoldingObject, v64);
//                    }
//                    else
//                    {
//                        int i = (int)dbObject;
//                        fieldInfo.SetValue(csHoldingObject, i);
//                    }
//                }
//                else if (fieldInfo.FieldType == typeof(Data_AppUserFile.eUserStatus))
//                {
//                    int i = (int)dbObject;
//                    Data_AppUserFile.eUserStatus us = (Data_AppUserFile.eUserStatus)i;
//                    fieldInfo.SetValue(csHoldingObject, us);
//                }
//                else if (fieldInfo.FieldType == typeof(Data_AppUserFile.niceMoney))
//                {
//                    int i = (int)dbObject;
//                    Data_AppUserFile.niceMoney n = new Data_AppUserFile.niceMoney(i);
//                    fieldInfo.SetValue(csHoldingObject, n);
//                }
//                else
//                {

//                }
//            }
//            else
//            {
//                // to database
//                if (fieldInfo.FieldType == typeof(MobileNoHandler))
//                {
//                    dbObject = ((MobileNoHandler)fieldInfo.GetValue(csHoldingObject)).getVal;
//                }
//                else if (fieldInfo.FieldType == typeof(System.Int64))
//                {
//                    if (    (fieldInfo.Name == "CreationDate") 
//                        ||  (fieldInfo.Name == "free_LastMsgQueued")
//                        ||  (fieldInfo.Name == "monthly_LastMsgQueued")
//                        ||  (fieldInfo.Name == "monthly_PaidUntil")
//                        ||  (fieldInfo.Name == "payAsSent_LastMsgQueued"))
//                    {
//                        Int64 v64 = (Int64)fieldInfo.GetValue(csHoldingObject);
//                        dbObject = v64.SQLTimeIntToSqlString();
//                    }
//                    else
//                    {
//                        dbObject = fieldInfo.GetValue(csHoldingObject);
//                    }
//                }
//                else if (fieldInfo.FieldType == typeof(Data_AppUserFile.eUserStatus))
//                {
//                    Data_AppUserFile.eUserStatus us = (Data_AppUserFile.eUserStatus)fieldInfo.GetValue(csHoldingObject);
//                    Int32 i = (int)us;
//                    dbObject = i;
//                }
//                else if (fieldInfo.FieldType == typeof(Data_AppUserFile.niceMoney))
//                {
//                    Data_AppUserFile.niceMoney n = (Data_AppUserFile.niceMoney)fieldInfo.GetValue(csHoldingObject);
//                    dbObject = n.ValueInUsCent;
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
//                    new sqLiteTableField(fUserName, "TEXT", "UserName"),
//                    new sqLiteTableField(fTelConfirmed, "TEXT", "MobileNumbers_AllConfirmed__", null, custom_Trans_Value),
//                    new sqLiteTableField(fTelUnConfirmed, "TEXT", "MobileNumbers_AllUnConfirmed__", null, custom_Trans_Value),
//                    new sqLiteTableField(fEmail, "TEXT UNIQUE", "Email"),
//                    new sqLiteTableField(fPassword, "TEXT", "Password"),
//                    new sqLiteTableField(fCreationDate, "TEXT", "CreationDate", null, custom_Trans_Value),
//                    new sqLiteTableField(fCreationIp, "TEXT", "CreationIp"),
//                    new sqLiteTableField(fApiGuId, "TEXT", "ApiGuId"),
//                    new sqLiteTableField(fComment, "TEXT", "Comment"),
//                    new sqLiteTableField(fDeleteOnFailed, "INT", "DeleteOnFailed"),
//                    new sqLiteTableField(fAddNumber_AllowedWithAPI, "INT", "AddNumber_AllowedWithAPI"),
//    // no longer eeded                new sqLiteTableField(fAddNumber_ActivateOnSyncRequest, "INT", "AddNumber_AllowedWithAPI"),
//                    new sqLiteTableField(fAccountStatus, "INT", "AccountStatus", null, custom_Trans_Value),

//                    // free account
//                    new sqLiteTableField(ffree_LastMsgQueued, "TEXT", "free_LastMsgQueued", custom_trans2DB_getProp, custom_Trans_Value),
//                    new sqLiteTableField(ffree_MsgSent, "INT", "free_MsgSent", custom_trans2DB_getProp, null),
//                    new sqLiteTableField(ffree_MsgLeft, "INT", "free_MsgLeft", custom_trans2DB_getProp, null),
//                    new sqLiteTableField(ffree_MinDelayInSeconds, "INT", "free_MinDelayInSeconds", custom_trans2DB_getProp, null),
//                    new sqLiteTableField(ffree_SendFooter, "INT", "free_SendFooter", custom_trans2DB_getProp, null),
//    // no longer used                new sqLiteTableField(ffree_WelcomeCounter, "INT", "free_WelcomeCounter"),
//                    new sqLiteTableField(ffree_MsgQueued, "INT", "free_MsgQueued", custom_trans2DB_getProp, custom_Trans_Value),

//                    // monthly
//                    new sqLiteTableField(fmonthly_LastMsgQueued, "TEXT", "monthly_LastMsgQueued", custom_trans2DB_getProp, custom_Trans_Value),
//                    new sqLiteTableField(fmonthly_MsgSent, "INT", "monthly_MsgSent", custom_trans2DB_getProp, null),
//                    new sqLiteTableField(fmonthly_PaidUntil, "TEXT", "monthly_PaidUntil", custom_trans2DB_getProp, custom_Trans_Value),
//                    new sqLiteTableField(fmonthly_MinDelayInSeconds, "INT", "monthly_MinDelayInSeconds", custom_trans2DB_getProp, null),
//                    new sqLiteTableField(fmonthly_CostPerNumber, "INT", "monthly_CostPerNumber", custom_trans2DB_getProp, custom_Trans_Value),
//                    new sqLiteTableField(fmonthly_CurrentCredit, "INT", "monthly_CurrentCredit", custom_trans2DB_getProp, custom_Trans_Value),

//                    // pay as you send
//                    new sqLiteTableField(fpayAsSent_LastMsgQueued, "TEXT", "payAsSent_LastMsgQueued", custom_trans2DB_getProp, custom_Trans_Value),
//                    new sqLiteTableField(fpayAsSent_MsgSent, "INT", "payAsSent_MsgSent", custom_trans2DB_getProp, null),
//                    new sqLiteTableField(fpayAsSent_MinDelayInSeconds, "INT", "payAsSent_MinDelayInSeconds", custom_trans2DB_getProp, null),
//                    new sqLiteTableField(fpayAsSent_CostPerNumber, "INT", "payAsSent_CostPerNumber", custom_trans2DB_getProp, custom_Trans_Value),
//                    new sqLiteTableField(fpayAsSent_CostPerMessage, "INT", "payAsSent_CostPerMessage", custom_trans2DB_getProp, custom_Trans_Value),
//                    new sqLiteTableField(fpayAsSent_CurrentCredit, "INT", "payAsSent_CurrentCredit", custom_trans2DB_getProp, custom_Trans_Value)
//                };
//            }
//        }
//        public string AtCreateSql { get { return null; } }

//        public object Translate_DB2Object_Create()
//        {
//            Data_AppUserFile user = Data_AppUserFile.CreateBlank();
//            return user;
//        }
//        #endregion

//        public Data_AppUserFileHandling_SQLite(IMyLog log)
//        {
//            sqLiteSQLHelper.createIfNotExist(this, log);
//        }

//        public String GetInfo()
//        {
//            return "SQLite " + TableName;
//        }

//        public void Update_General(string email, d_On_User_Action action, Object args, d_On_User_PostAction postAction, IMyLog log)
//        {
//            try
//            {
//                //1) read
//                Data_AppUserFile r = null;
//                string where = string.Format("WHERE {0} = '{1}'", fEmail, Data_AppUserFile.EmailToRealEmail(email));
//                sqLiteSQLHelper.buildObjects(
//                    sqLiteSQLHelper.select(this, where, log),
//                    this,
//                    null,
//                    delegate(object built)
//                    {
//                        r = (Data_AppUserFile)built;
//                    });

//                //2) call action
//                if (action != null)
//                {
//                    action(r, args);
//                }

//                //3) update
//                if (r != null)
//                {
//                    sqLiteSQLHelper.update(r, this, null, where, log);
//                }

//                //4) call postAction
//                if (postAction != null)
//                {
//                    postAction(args);
//                }
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//        }

//        public void StoreNew(Data_AppUserFile data, out bool fileArleadyUsed, IMyLog log)
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

//        public TelListController GetCurrentTelList(IMyLog log)
//        {
//            try
//            {
//                TelListController ret = new TelListController();
//                using (var _lock = ret.GetLock())
//                {
//                    RetrieveAll(Data_AppUserFile.SortType.DontSort,
//                        delegate(Data_AppUserFile d)
//                        {
//                            TelListController.currentTelListBuilder(d, _lock);
//                        }, log);
//                }
//                return ret;
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//            return null;
//        }

//        public Data_AppUserFile RetrieveOne(string email, IMyLog log)
//        {
//            try
//            {
//                Data_AppUserFile ret = null;
//                string where = string.Format("WHERE {0} = '{1}'", fEmail, Data_AppUserFile.EmailToRealEmail(email));
//                sqLiteSQLHelper.buildObjects(
//                    sqLiteSQLHelper.select(this, where, log),
//                    this,
//                    null,
//                    delegate(object built)
//                    {
//                        ret = (Data_AppUserFile)built;
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

//        public void RetrieveAll(Data_AppUserFile.SortType sort, d1_Data_AppUserFile d, IMyLog log)
//        {
//            try
//            {
//                sqLiteSQLHelper.buildObjects(
//                    sqLiteSQLHelper.select(this, getORDERBY(sort), log),
//                    this,
//                    null,
//                    delegate(object built)
//                    {
//                        Data_AppUserFile bd = (Data_AppUserFile)built;
//                        d(bd);
//                    });
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//        }

//        private static string getORDERBY(Data_AppUserFile.SortType sort)
//        {
//            switch (sort)
//            {
//                case Data_AppUserFile.SortType.Date:
//                    return string.Format("ORDER BY {0} DESC, {1} ASC", fCreationDate, fEmail);
//                case Data_AppUserFile.SortType.State:
//                    return string.Format("ORDER BY {0} ASC, {1} ASC", fAccountStatus, fCreationDate);
//                case Data_AppUserFile.SortType.Usage:
//                    return string.Format("ORDER BY {0} ASC, {1} ASC", ffree_MsgSent, fCreationDate);
//                case Data_AppUserFile.SortType.DontSort:
//                default:
//                    return string.Format("ORDER BY {0} ASC", fEmail);
//            }
//        }
//    }
//}
