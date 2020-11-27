using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.SQLite;
using NiceApiLibrary_low;

//namespace NiceApiLibrary
//{
//    class Data_Net__00NormalMessage_SQLite : IData_Net__00NormalMessage_store, IsqLiteTable
//    {
//        #region table
//        public static readonly string fDestMobile = "DestMobile";
//        public static readonly string fDisposeAfterNFailed = "DisposeAfterNFailed";
//        public static readonly string fFailedCounter = "FailedCounter";
//        public static readonly string fMessage = "Message";
//        public static readonly string fTimeCreated = "TimeCreated";
//        public static readonly string fTimeProcessed = "TimeProcessed";
//        public static readonly string fNoCounterUpdate = "NoCounterUpdate";
//        public static readonly string fUserEmail = "UserEmail";
//        public static readonly string fFileName = "FileName";
//        public static readonly string fLocationId = "LocationId";

//        public MultiThreadAbledConnection Connection { get { return sqLiteConnectionHolder.once.db_All; } }
//        public string TableName { get { return "Messages"; } }
//        private void custom_Trans_getProperty(string property, object srcTarget, object auxData, out MyFieldInfo prop, out object csHoldingObject)
//        {
//            prop = new MyFieldInfo(srcTarget.GetType().GetField(property));
//            csHoldingObject = srcTarget;
//            if (property == "__FileName")
//            {
//                Data_Net__00NormalMessage x = (Data_Net__00NormalMessage)srcTarget;
//                prop = new MyFieldInfo(property, x.GetFileName());
//            }
//            else if (property == "__Location")
//            {
//                if (auxData == null)
//                {
//                    throw new ArgumentException("auxData null");
//                }
//                Data_Net__00NormalMessage.eLocation l = (Data_Net__00NormalMessage.eLocation)auxData;
//                prop = new MyFieldInfo(property, (int)l);
//            }
//            else
//            {

//            }
//        }

//        private static void custom_Trans_Value(bool toDatabase, MyFieldInfo fieldInfo, ref object csHoldingObject, ref object dbObject)
//        {
//            if (!toDatabase)
//            {
//                // from Database
//                if (fieldInfo.FieldType == typeof(System.Int64))
//                {
//                    if (fieldInfo.Name == "MsgTicks")
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
//                else
//                {

//                }
//            }
//            else
//            {
//                // to database
//                if (fieldInfo.FieldType == typeof(System.Int64))
//                {
//                    if (fieldInfo.Name == "MsgTicks")
//                    {
//                        Int64 v64 = (Int64)fieldInfo.GetValue(csHoldingObject);
//                        dbObject = v64.SQLTimeIntToSqlString();
//                    }
//                    else
//                    {
//                        dbObject = fieldInfo.GetValue(csHoldingObject);
//                    }
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
//                    new sqLiteTableField(fUserEmail, "TEXT", "UserId"),
//                    new sqLiteTableField(fDestMobile, "TEXT", "DestMobile"),
//                    new sqLiteTableField(fTimeCreated, "TEXT", "MsgTicks", null, custom_Trans_Value),
//                    new sqLiteTableField(fMessage, "TEXT", "Msg"),
//                    new sqLiteTableField(fFailedCounter, "INT", "FailedCounter"),
//                    new sqLiteTableField(fDisposeAfterNFailed, "INT", "DisposeAfterNFailed"),
//                    new sqLiteTableField(fNoCounterUpdate, "INT", "NoCounterUpdate"),

//                    new sqLiteTableField(fFileName, "TEXT", "__FileName", custom_Trans_getProperty, null),
//                    new sqLiteTableField(fLocationId, "INT", "__Location", custom_Trans_getProperty, null)
//                };
//            }
//        }
//        public string AtCreateSql { get { return null; } }

//        public object Translate_DB2Object_Create()
//        {
//            return new Data_Net__00NormalMessage();
//        }
//        #endregion

//        public Data_Net__00NormalMessage_SQLite(IMyLog log)
//        {
//            sqLiteSQLHelper.createIfNotExist(this, log);
//        }

//        public String GetInfo()
//        {
//            return "SQLite " + TableName;
//        }

//        public void Store(Data_Net__00NormalMessage msg, Data_Net__00NormalMessage.eLocation location, IMyLog log)
//        {
//            try
//            { 
//                sqLiteSQLHelper.insert(msg, this, location, log);
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//        }

//        public void ForEach(Data_Net__00NormalMessage.eLocation location, IMyLog log, dProcess_Data_Net__00NormalMessage cb)
//        {
//            ForEach(DateTime.MinValue, null, location, log, cb);
//        }

//        public void ForEach(DateTime newerThan, string containsUser, Data_Net__00NormalMessage.eLocation location, IMyLog log, dProcess_Data_Net__00NormalMessage cb)
//        {
//            try
//            {
//                string where = string.Format("WHERE {0} = {1}", fLocationId, (int)location);
//                if (newerThan > DateTime.MinValue)
//                {
//                    where += string.Format(" AND {0} > '{1}'", fTimeCreated, newerThan.Ticks.SQLTimeIntToSqlString());
//                }
//                if (containsUser != null)
//                {
//                    where += string.Format(" AND {0} = '{1}'", fUserEmail, containsUser);
//                }

//                sqLiteSQLHelper.buildObjects(
//                    sqLiteSQLHelper.select(this, where, log),
//                    this,
//                    location,
//                    delegate(object built)
//                    {
//                        Data_Net__00NormalMessage bd = (Data_Net__00NormalMessage)built;
//                        cb(bd);
//                    });
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//        }

//        public Data_Net__00NormalMessage ReadOne(string fileName, Data_Net__00NormalMessage.eLocation location, IMyLog log)
//        {
//            Data_Net__00NormalMessage ret = null;
//            try
//            {
//                string where = whereClauseForFileNameAndLocation(fileName, location);
//                sqLiteSQLHelper.buildObjects(
//                    sqLiteSQLHelper.select(this, where, log),
//                    this,
//                    location,
//                    delegate(object built)
//                    {
//                        Data_Net__00NormalMessage bd = (Data_Net__00NormalMessage)built;
//                        ret = bd;
//                    });
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//            return ret;
//        }

//        public void Delete(string fileName, Data_Net__00NormalMessage.eLocation location, IMyLog log)
//        {
//            try
//            {
//                string where = whereClauseForFileNameAndLocation(fileName, location);
//                sqLiteSQLHelper.deleteFrom(this, where, log);
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//        }
//        public int GetNoOfQueuedItems(IMyLog log)
//        {
//            int ret = -1;
//            try
//            {
//                string where = string.Format("WHERE {0} = {1} ", fLocationId, (int)Data_Net__00NormalMessage.eLocation.Queued);
//                ret = sqLiteSQLHelper.count(this, where, log);
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//            return ret;
//        }

//        private static string whereClauseForFileNameAndLocation(string fileName, Data_Net__00NormalMessage.eLocation location)
//        {
//            ASPTrayBase.MsgFileParts info = ASPTrayBase.s_MsgFile_GetPartsFromMessageFile(fileName);
//            string where = string.Empty;
//            where += string.Format("WHERE {0} = {1} ", fLocationId, (int)location);
//            where += string.Format("AND {0} = '{1}' ", fUserEmail, info.Email);
//            where += string.Format("AND {0} = '{1}' ", fTimeCreated, info.Time.Ticks.SQLTimeIntToSqlString());
//            return where;
//        }
//    }
//}

