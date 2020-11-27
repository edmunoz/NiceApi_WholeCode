using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.IO;
using System.Data.SQLite;
using NiceApiLibrary_low;

//namespace NiceApiLibrary
//{
//    public delegate void dBuiltObject(object built);
//    public delegate void dTrans_getProperty(string property, object srcTarget, object auxData, out MyFieldInfo prop, out object csHoldingObject);
//    public delegate void dTrans_Value(bool toDatabase, MyFieldInfo fieldInfo, ref object csHoldingObject, ref object dbObject);

//    public class sqLiteTableField
//    {
//        public string SqlName;
//        public string SqlType;
//        public string CsProperty;
//        public dTrans_getProperty Trans_getProperty;
//        public dTrans_Value Trans_Value;

//        public sqLiteTableField(string SqlName, string SqlType, string CsProperty, dTrans_getProperty Trans_getProperty, dTrans_Value Trans_Value)
//        {
//            this.SqlName = SqlName;
//            this.SqlType = SqlType;
//            this.CsProperty = CsProperty;
//            this.Trans_getProperty = Trans_getProperty == null ? default_Trans_getProperty : Trans_getProperty;
//            this.Trans_Value = Trans_Value == null ? default_Trans_Value : Trans_Value;
//        }
//        public sqLiteTableField(string SqlName, string SqlType, string CsProperty)
//        {
//            this.SqlName = SqlName;
//            this.SqlType = SqlType;
//            this.CsProperty = CsProperty;
//            this.Trans_getProperty = default_Trans_getProperty;
//            this.Trans_Value = default_Trans_Value;
//        }

//        private static void default_Trans_getProperty(string property, object srcTarget, object auxData, out MyFieldInfo prop, out object csHoldingObject)
//        {
//            prop = new MyFieldInfo(srcTarget.GetType().GetField(property));
//            csHoldingObject = srcTarget;
//        }

//        private static void default_Trans_Value(bool toDatabase, MyFieldInfo fieldInfo, ref object csHoldingObject, ref object dbObject)
//        {
//            if (toDatabase)
//            {
//                dbObject = fieldInfo.GetValue(csHoldingObject);
//            }
//            else
//            {
//                fieldInfo.SetValue(csHoldingObject, dbObject);
//            }
//        }

//    }

//    public class MultiThreadAbledConnection
//    {
//        private SQLiteConnection conn;

//        public MultiThreadAbledConnection(string connectionString)
//        {
//            conn = new SQLiteConnection(connectionString);
//            conn.Open();
//        }
//        public void Close()
//        {
//            conn.Close();
//        }

//        public SQLiteTransaction BeginTransaction()
//        {
//            return conn.BeginTransaction();
//        }

//        public UsingLocker GetLocked()
//        {
//            UsingLocker ret = new UsingLocker(conn);
//            return ret;
//        }


//        public class UsingLocker : IDisposable
//        {
//            public SQLiteConnection con;
//            public UsingLocker(SQLiteConnection connection)
//            {
//                this.con = connection;
//                System.Threading.Monitor.Enter(con);
//            }

//            public void Dispose()
//            {
//                System.Threading.Monitor.Exit(con);
//            }
//        }
//    }

//    public class sqLiteConnectionHolder
//    {
//        public bool ConstructionResult;
//        private static sqLiteConnectionHolder once_ = null;

//        public static sqLiteConnectionHolder once 
//        {
//            get
//            {
//                return once_;            
//            } 
//            set
//            {
//                once_ = value;
//            }
//        }


//        public MultiThreadAbledConnection db_All;
//        private sqLiteConnectionHolder(IMyLog log, bool doInitOnce = true)
//        {
//            if (doInitOnce)
//            {
//                once = this;
//            }
//            try
//            {
//                db_All = new MultiThreadAbledConnection(getConnectionString(MyFolders.sqlite_All));
//                if (doInitOnce)
//                {
//                    var txn = db_All.BeginTransaction();
//                    new sqLiteLocationEnum(log);
//                    new sqLiteUserStaturEnum(log);
//                    txn.Commit();
//                }
//                log.Info("sqLiteConnectionHolder started");
//                ConstructionResult = true;
//            }
//            catch (DllNotFoundException de)
//            {
//                log.Error("DllNotFoundException");
//                log.Error(de.Message);
//            }
//            catch (BadImageFormatException bi)
//            {
//                log.Error("BadImageFormatException");
//                log.Error(bi.Message);
//            }
//        }

//        private static string getConnectionString(MyFolders item)
//        {
//            return "Data Source=" + FolderNames.GetFolder(item) + ";Version=3;";
//        }

//        internal static bool TestStartUp(IMyLog log)
//        {
//            sqLiteConnectionHolder i = new sqLiteConnectionHolder(log, false);
//            return i.ConstructionResult;
//        }

//        internal static bool StartUp(IMyLog log)
//        {
//            if (once == null)
//            {
//                new sqLiteConnectionHolder(log);
//            }
//            return once.ConstructionResult;
//        }

//        internal static void End()
//        {
//            once.db_All.Close();
//        }
//    }

//    public class sqLiteUserStaturEnum : IsqLiteTable
//    {
//        public static readonly string fStatusId = "StatusId";
//        public static readonly string fStatus = "Status";
//        public MultiThreadAbledConnection Connection { get { return sqLiteConnectionHolder.once.db_All; } }
//        public string TableName { get { return "UserStaturEnum"; } }
//        public List<sqLiteTableField> Fields
//        {
//            get
//            {
//                return new List<sqLiteTableField>() {
//                    new sqLiteTableField(fStatusId, "INT", "__"),
//                    new sqLiteTableField(fStatus, "TEXT", "__")
//                };
//            }
//        }
//        public string AtCreateSql
//        {
//            get
//            {
//                StringBuilder sb = new StringBuilder();
//                sb.Append("INSERT INTO " + TableName + " VALUES");
//                bool first = true;
//                for (int i = 0; i < (int)Data_AppUserFile.eUserStatus._end; i++)
//                {
//                    Data_AppUserFile.eUserStatus u = (Data_AppUserFile.eUserStatus)i;
//                    string nice = u.ToString();
//                    if (!first)
//                    {
//                        sb.Append(",");
//                    }
//                    string add = string.Format("({0}, '{1}')", i, nice);
//                    sb.Append(add);
//                    first = false;
//                }
//                return sb.ToString();
//            }
//        }

//        public sqLiteUserStaturEnum(IMyLog log)
//        {
//            sqLiteSQLHelper.createIfNotExist(this, log);
//        }

//        public object Translate_DB2Object_Create()
//        {
//            throw new SystemException("Will never be called");
//        }
//    }

//    public class sqLiteLocationEnum : IsqLiteTable
//    {
//        public static readonly string fLocationId = "LocationId";
//        public static readonly string fLocationName = "LocationName";
//        public MultiThreadAbledConnection Connection 
//        { 
//            get 
//            { 
//                if (sqLiteConnectionHolder.once == null)
//                {
//                    throw new NotSupportedException("sqLiteConnectionHolder.once == null");
//                }
//                return sqLiteConnectionHolder.once.db_All; 
//            } 
//        }
//        public string TableName { get { return "LocationEnum"; } }
//        public List<sqLiteTableField> Fields
//        {
//            get
//            {
//                return new List<sqLiteTableField>() {
//                    new sqLiteTableField(fLocationId, "INT", "__"),
//                    new sqLiteTableField(fLocationName, "TEXT", "__")
//                };
//            }
//        }
//        public string AtCreateSql { get { return "INSERT INTO " + TableName + " VALUES(1, 'Queued'),(2, 'Processed'),(3, 'Disposed')"; } }

//        public sqLiteLocationEnum(IMyLog log)
//        {
//            sqLiteSQLHelper.createIfNotExist(this, log);
//        }

//        public object Translate_DB2Object_Create()
//        {
//            throw new SystemException("Will never be called");
//        }
//    }

//    public interface IsqLiteTable
//    {
//        MultiThreadAbledConnection Connection { get; } 
//        string TableName { get; }
//        List<sqLiteTableField> Fields { get; }
//        string AtCreateSql { get; }

//        object Translate_DB2Object_Create();
//    }

//    internal class SQLiteCommandWithLog : IDisposable
//    {
//        private SQLiteCommand bas;
//        private SQLiteConnection basCon;
//        private IMyLog log;

//        public SQLiteCommandWithLog(string commandText, SQLiteConnection connection, IMyLog log)
//        {
//            this.basCon = connection;
//            this.log = log;
//            this.log.SqlStatement(commandText);
//            this.bas = new SQLiteCommand(commandText, connection);
//        }

//        public void Dispose()
//        {
//            bas.Dispose();
//        }

//        public SQLiteDataReader ExecuteReader()
//        {
//            try
//            {
//                return bas.ExecuteReader();
//            }
//            catch (SystemException se)
//            {
//                log.Error(se.Message);
//                throw se;
//            }
//        }

//        public int ExecuteNonQuery()
//        {
//            try
//            {
//                return bas.ExecuteNonQuery();
//            }
//            catch (SystemException se)
//            {
//                log.Error(se.Message);
//                throw se;
//            }
//        }
//    }

//    static class sqLiteSQLHelper
//    {
//        public static readonly string sqlTimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
//        public static readonly string sqlFormatSelectAll = "SELECT * FROM {0}";
//        public static readonly string sqlFormatSelectSome = "SELECT {0} FROM {1}";
//        public static readonly string sqlFormatSelectSomeWithWhere = "SELECT {0} FROM {1} WHERE {2}";
//        public static readonly string masterTableName = "sqlite_master";

//        public static void deleteFrom(IsqLiteTable table, string whereClause, IMyLog log)
//        {
//            string sql = string.Format("DELETE FROM {0} {1}", table.TableName, whereClause);
//            try
//            {
//                using (var l = table.Connection.GetLocked())
//                {
//                    using (SQLiteCommandWithLog cmd = new SQLiteCommandWithLog(sql, l.con, log))
//                    {
//                        int iTest = cmd.ExecuteNonQuery();
//                        if (iTest != 1)
//                        {
//                            // error
//                            log.Error("SQLite deleteFrom returned " + iTest.ToString());
//                            log.Error("SQL: " + sql);
//                        }
//                    }
//                }
//            }
//            catch (SystemException se)
//            {
//                log.Error(sql);
//                log.Error(se.Message);
//                throw se;
//            }
//        }

//        public static int count(IsqLiteTable table, string whereClause, IMyLog log)
//        {
//            string sql = string.Format("SELECT count(*) FROM {0} {1}", table.TableName, whereClause);
//            try
//            {
//                using (var l = table.Connection.GetLocked())
//                {
//                    using (SQLiteCommandWithLog cmd = new SQLiteCommandWithLog(sql, l.con, log))
//                    {
//                        SQLiteDataReader read = cmd.ExecuteReader();
//                        read.Read();
//                        int res = (int)(long)read["count(*)"];
//                        return res;
//                    }
//                }
//            }
//            catch (SystemException se)
//            {
//                log.Error(sql);
//                log.Error(se.Message);
//                throw se;
//            }
//        }

//        public static SQLiteDataReader select(IsqLiteTable table, string whereOrOrderBy, IMyLog log)
//        {
//            string sql = string.Format("SELECT * FROM {0} {1}", table.TableName, whereOrOrderBy);
//            try
//            {
//                using (var l = table.Connection.GetLocked())
//                {
//                    using (SQLiteCommandWithLog cmd = new SQLiteCommandWithLog(sql, l.con, log))
//                    {
//                        return cmd.ExecuteReader();
//                    }
//                }
//            }
//            catch (SystemException se)
//            {
//                log.Error(sql);
//                log.Error(se.Message);
//                throw se;
//            }
//        }

//        public static void update(object what, IsqLiteTable table, object auxData, string whereClause, IMyLog log)
//        {
//            // UPDATE table_name SET column1 = value1, column2 = value2 WHERE condition; 
//            StringBuilder sb = new StringBuilder("UPDATE " +  table.TableName + " SET ");
//            bool first = true;
//            foreach (sqLiteTableField f1 in table.Fields)
//            {
//                MyFieldInfo fInfo = null;
//                object csHoldingObject = null;
//                f1.Trans_getProperty(f1.CsProperty, what, auxData, out fInfo, out csHoldingObject);
//                if ((fInfo == null) || (!fInfo.IsOk) || (csHoldingObject == null))
//                {
//                    throw new SystemException("SQL insert fields");
//                }

//                object dbObject = null;
//                f1.Trans_Value(true, fInfo, ref csHoldingObject, ref dbObject);
//                if (dbObject == null)
//                {
//                    throw new SystemException("SQL insert fields");
//                }
//                string sqlPart = _String_Int_Bool_ToDbString(dbObject);
//                if (sqlPart == null)
//                {
//                    throw new SystemException("SQL insert fields");
//                }

//                // UPDATE table_name SET column1 = value1, column2 = value2 WHERE condition; 
//                if (!first)
//                {
//                    sb.Append(",");
//                }
//                sb.Append(f1.SqlName + "=" + sqlPart);
//                first = false;
//            }
//            sb.Append(" " + whereClause);
//            string sql = sb.ToString();
//            int iVar = 0;
//            try
//            {
//                using (var l = table.Connection.GetLocked())
//                {
//                    using (SQLiteCommandWithLog cmd = new SQLiteCommandWithLog(sql, l.con, log))
//                    {
//                        iVar = cmd.ExecuteNonQuery();
//                        if (iVar < 1)
//                        {
//                            throw new SQLiteException("insert didnt change any rows");
//                        }
//                    }
//                }
//            }
//            catch (SystemException se)
//            {
//                log.Error(sql);
//                log.Error(se.Message);
//                throw se;
//            }
//        }

//        public static void insert(object what, IsqLiteTable table, object auxData, IMyLog log)
//        {
//            // INSERT INTO Messages(UserId,...) VALUES('email@xxx.com','1111')
//            StringBuilder sbKey = new StringBuilder("INSERT INTO " + table.TableName + " (");
//            StringBuilder sbVal = new StringBuilder(" VALUES(");
//            bool first = true;
//            foreach (sqLiteTableField f1 in table.Fields)
//            {
//                MyFieldInfo fInfo = null;
//                object csHoldingObject = null;
//                f1.Trans_getProperty(f1.CsProperty, what, auxData, out fInfo, out csHoldingObject);
//                if ((fInfo == null) || (!fInfo.IsOk) || (csHoldingObject == null))
//                {
//                    throw new SystemException("SQL insert fields");
//                }

//                object dbObject = null;
//                f1.Trans_Value(true, fInfo, ref csHoldingObject, ref dbObject);
//                if (dbObject == null)
//                {
//                    throw new SystemException("SQL insert fields");
//                }
//                string sqlPart = _String_Int_Bool_ToDbString(dbObject);
//                if (sqlPart == null)
//                {
//                    throw new SystemException("SQL insert fields");
//                }
//                if (!first)
//                {
//                    sbKey.Append(",");
//                    sbVal.Append(",");
//                }
//                sbKey.Append(f1.SqlName);
//                sbVal.Append(sqlPart);
//                first = false;
//            }
//            sbKey.Append(")");
//            sbVal.Append(")");
//            string sql = sbKey.ToString() + sbVal.ToString();
//            int iVar = 0;
//            try
//            {
//                using (var l = table.Connection.GetLocked())
//                {
//                    using (SQLiteCommandWithLog cmd = new SQLiteCommandWithLog(sql, l.con, log))
//                    {
//                        iVar = cmd.ExecuteNonQuery();
//                        if (iVar < 1)
//                        {
//                            throw new SQLiteException("insert didnt change any rows");
//                        }
//                    }
//                }
//            }
//            catch (SystemException se)
//            {
//                log.Error(sql);
//                log.Error(se.Message);
//                throw se;
//            }
//        }

//        public static void buildObjects(SQLiteDataReader dbData, IsqLiteTable table, object auxData, dBuiltObject cb)
//        {
//            while (dbData.Read())
//            {
//                object target = table.Translate_DB2Object_Create();
//                foreach (sqLiteTableField f1 in table.Fields)
//                {
//                    var fromDB = dbData[f1.SqlName];

//                    MyFieldInfo fInfo = null;
//                    object csHoldingObject = null;
//                    f1.Trans_getProperty(f1.CsProperty, target, auxData, out fInfo, out csHoldingObject);
//                    if ((fInfo == null) || (!fInfo.IsOk) || (csHoldingObject == null))
//                    {
//                        throw new SystemException("SQL insert fields");
//                    }

//                    fromDB = _IntTo_Bool(fInfo, fromDB);
//                    f1.Trans_Value(false, fInfo, ref csHoldingObject, ref fromDB);
//                }
//                cb(target);
//            }
//        }

//        private static object _IntTo_Bool(MyFieldInfo fieldInfo, object CsObject)
//        {
//            if (fieldInfo.FieldType == typeof(System.Boolean))
//            {
//                int i = (int)CsObject;
//                Boolean b = i == 0 ? false : true;
//                return b;
//            }
//            return CsObject;
//        }

//        private static string _String_Int_Bool_ToDbString(object CsObject)
//        {
//            string sqlPart = null;
//            if (CsObject.GetType() == typeof(System.String))
//            {
//                String _value = (String)CsObject;
//                // double up any singel '
//                _value = _value.Replace("'", "''");
//                sqlPart = "'" + _value + "'";
//            }
//            else if (CsObject.GetType() == typeof(System.Int32))
//            {
//                sqlPart = CsObject.ToString();
//            }
//            else if (CsObject.GetType() == typeof(System.Int64))
//            {
//                sqlPart = CsObject.ToString();
//            }
//            else if (CsObject.GetType() == typeof(System.Boolean))
//            {
//                sqlPart = ((System.Boolean)CsObject) ? "1" : "0";
//            }
//            // e: default convert type
//            return sqlPart;
//        }

//        public static void createIfNotExist(IsqLiteTable table, IMyLog log)
//        {
//            try
//            {
//                if (!hasTable(table, log))
//                {
//                    createTable(table, log);
//                    if (table.AtCreateSql != null)
//                    {
//                        using (var l = table.Connection.GetLocked())
//                        {
//                            doSql(table.AtCreateSql, l.con, log);
//                        }
//                    }
//                }
//            }
//            catch (SystemException se)
//            {
//                log.Error(se.Message);
//            }
//        }

//        public static void doSql(string sql, SQLiteConnection con, IMyLog log)
//        {
//            using (SQLiteCommandWithLog cmd = new SQLiteCommandWithLog(sql, con, log))
//            {
//                cmd.ExecuteNonQuery();
//            }
//        }

//        public static bool hasTable(IsqLiteTable table, IMyLog log)
//        {
//            // SELECT name FROM my_db.sqlite_master WHERE type='table';
//            string sql = string.Format(sqlFormatSelectSomeWithWhere, "name", masterTableName, "type='table'");
//            using (var l = table.Connection.GetLocked())
//            {
//                using (SQLiteCommandWithLog cmd = new SQLiteCommandWithLog(sql, l.con, log))
//                {
//                    SQLiteDataReader reader = cmd.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        if (((string)reader["name"]) == table.TableName)
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }
//            return false;
//        }

//        public static void createTable(IsqLiteTable table, IMyLog log)
//        {
//            // create table highscoress (name varchar(20), score int)
//            StringBuilder sb = new StringBuilder();
//            sb.Append("CREATE TABLE " + table.TableName);
//            sb.Append(" (");
//            bool first = true;
//            foreach (sqLiteTableField f1 in table.Fields)
//            {
//                if (!first)
//                {
//                    sb.Append(", ");
//                }
//                sb.Append(" " + f1.SqlName + " " + f1.SqlType);
//                first = false;
//            }
//            sb.Append(" )");

//            using (var l = table.Connection.GetLocked())
//            {
//                using (SQLiteCommandWithLog cmd = new SQLiteCommandWithLog(sb.ToString(), l.con, log))
//                {
//                    int res = cmd.ExecuteNonQuery();
//                }
//            }
//        }
//    }

//    public class MyFieldInfo
//    {
//        private FieldInfo fi;
//        private string _Name;
//        private object _Obj;

//        public MyFieldInfo(FieldInfo fi)
//        {
//            this.fi = fi;
//        }
//        public MyFieldInfo(string name, object obj)
//        {
//            this._Name = name;
//            this._Obj = obj;
//        }

//        public bool IsOk
//        {
//            get
//            {
//                if (fi != null)
//                {
//                    return true;
//                }
//                if (_Obj != null)
//                {
//                    return true;
//                }
//                return false;
//            }
//        }

//        public Type FieldType
//        {
//            get
//            {
//                if (fi != null)
//                {
//                    return fi.FieldType;
//                }
//                else if (_Obj != null)
//                {
//                    return _Obj.GetType();
//                }
//                else
//                {
//                    return null;
//                }
//            }
//        }

//        public string Name
//        {
//            get
//            {
//                if (fi != null)
//                {
//                    return fi.Name;
//                }
//                else if (_Name != null)
//                {
//                    return _Name;
//                }
//                else
//                {
//                    return null;
//                }
//            }
//        }

//        public object GetValue(object obj)
//        {
//            if (fi != null)
//            {
//                return fi.GetValue(obj);
//            }
//            else
//            {
//                return _Obj;
//            }
//        }

//        public void SetValue(object obj, object value)
//        {
//            if (fi != null)
//            {
//                fi.SetValue(obj, value);
//            }
//        }
//    }
 
//    public static class ObjectToLinesClass
//    {
//        public static void ObjectToLines(object what, IsqLiteTable table, object auxData, TextWriter tw)
//        {
//            foreach (sqLiteTableField f1 in table.Fields)
//            {
//                MyFieldInfo fInfo = null;
//                object csHoldingObject = null;

//                tw.Write(f1.CsProperty);
//                tw.Write(": ");
//                f1.Trans_getProperty(f1.CsProperty, what, auxData, out fInfo, out csHoldingObject);
//                if ((fInfo == null) || (!fInfo.IsOk) || (csHoldingObject == null))
//                {
//                    break;
//                }

//                object dbObject = null;
//                f1.Trans_Value(true, fInfo, ref csHoldingObject, ref dbObject);
//                if (dbObject == null)
//                {
//                    break;
//                }
//                tw.WriteLine(dbObject.ToString());
//            }
//        }
//    }
//}
