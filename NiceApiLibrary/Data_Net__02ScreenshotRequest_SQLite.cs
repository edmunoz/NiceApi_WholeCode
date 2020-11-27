using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using NiceApiLibrary_low;

//namespace NiceApiLibrary
//{
//    class Data_Net__02ScreenshotRequest_SQLite : IData_Net__02ScreenshotRequest_store, IsqLiteTable
//    {
//        #region table
//        public static readonly string fMsgTicks = "MsgTicks";
//        public static readonly string fFileName = "FileName";

//        public MultiThreadAbledConnection Connection { get { return sqLiteConnectionHolder.once.db_All; } }
//        public string TableName { get { return "ScreenshotRequests"; } }

//        private void custom_Trans_getProperty(string property, object srcTarget, object auxData, out MyFieldInfo prop, out object csHoldingObject)
//        {
//            prop = new MyFieldInfo(srcTarget.GetType().GetField(property));
//            csHoldingObject = srcTarget;
//            if (property == "__FileName")
//            {
//                Data_Net__02ScreenshotRequest x = (Data_Net__02ScreenshotRequest)srcTarget;
//                prop = new MyFieldInfo(property, x.GetFileName());
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
//                    new sqLiteTableField(fMsgTicks, "TEXT", "MsgTicks", null, custom_Trans_Value),
//                    new sqLiteTableField(fFileName, "TEXT", "__FileName", custom_Trans_getProperty, null),
//                };
//            }
//        }
//        public string AtCreateSql { get { return null; } }

//        public object Translate_DB2Object_Create()
//        {
//            return new Data_Net__02ScreenshotRequest();
//        }
//        #endregion

//        public Data_Net__02ScreenshotRequest_SQLite(IMyLog log)
//        {
//            sqLiteSQLHelper.createIfNotExist(this, log);
//        }

//        public String GetInfo()
//        {
//            return "SQLite " + TableName;
//        }

//        public void Store(Data_Net__02ScreenshotRequest msg, IMyLog log)
//        {
//            try
//            {
//                sqLiteSQLHelper.insert(msg, this, null, log);
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//        }

//        public void ForEach(IMyLog log, dProcess_Data_Net__02ScreenshotRequest cb)
//        {
//            try
//            {
//                sqLiteSQLHelper.buildObjects(
//                    sqLiteSQLHelper.select(this, "", log),
//                    this,
//                    null,
//                    delegate(object built)
//                    {
//                        Data_Net__02ScreenshotRequest bd = (Data_Net__02ScreenshotRequest)built;
//                        cb(bd);
//                    });
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//        }

//        public Data_Net__02ScreenshotRequest ReadOne(string fileName, IMyLog log)
//        {
//            Data_Net__02ScreenshotRequest ret = null;
//            try
//            {
//                string where = string.Format("WHERE {0} = '{1}' ", fFileName, fileName);
//                sqLiteSQLHelper.buildObjects(
//                    sqLiteSQLHelper.select(this, where, log),
//                    this,
//                    null,
//                    delegate(object built)
//                    {
//                        Data_Net__02ScreenshotRequest bd = (Data_Net__02ScreenshotRequest)built;
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

//        public void Delete(string fileName, IMyLog log)
//        {
//            try
//            {
//                string where = string.Format("WHERE {0} = '{1}' ", fFileName, fileName);
//                sqLiteSQLHelper.deleteFrom(this, where, log);
//            }
//            catch (SystemException se)
//            {
//                log.Error("*** SystemException ***");
//                log.Error(se.Message);
//            }
//        }

//    }
//}

