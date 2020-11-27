using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;
using System.Diagnostics;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    public interface ISqlAny
    {
        string TableName { get; }
    }

    public class SqlCmdBuilder
    {
        private Dictionary<string, string> Data = new Dictionary<string, string>();
        private string TableName;
        public SqlCmdBuilder(string tableName)
        {
            TableName = tableName;
        }
        public void Add(string key, string val)
        {
            Data.Add(key, val);
        }
        public void AddInc(string key)
        {
            Add(key, key + " + 1");
        }
        public void AddInc(string key, int count)
        {
            Add(key, key + " + " + count.ToString());
        }
        public void AddDec(string key, int count)
        {
            Add(key, key + " - " + count.ToString());
        }
        public void AddDec(string key)
        {
            Add(key, key + " - 1");
        }
        public string GetSql_Update(string whereCondition)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("UPDATE {0} SET ", TableName));

            int count = 0;
            foreach (KeyValuePair<string, string> kv1 in Data)
            {
                if (count == (Data.Count - 1))
                {
                    // last
                    sb.AppendLine(String.Format("{0} = {1} ", kv1.Key, kv1.Value));
                }
                else
                {
                    sb.AppendLine(String.Format("{0} = {1} ,", kv1.Key, kv1.Value));
                }

                // move on
                count++;
            }
            sb.AppendLine(whereCondition);
            return sb.ToString();
        }

        public string GetSql_InsertSelect()
        {
            StringBuilder sbHeade = new StringBuilder();
            StringBuilder sbKey = new StringBuilder();
            StringBuilder sbVal = new StringBuilder();
            StringBuilder sbFoot = new StringBuilder();

            //INSERT INTO [dbo].[MessageFileTable2]
            sbHeade.AppendLine("INSERT INTO " + TableName);
            sbKey.AppendLine("(");
            sbVal.AppendLine("SELECT ");
            int count = 0;
            foreach (KeyValuePair<string, string> kv1 in Data)
            {
                if (count == (Data.Count - 1))
                {
                    // last
                    sbKey.AppendLine(kv1.Key);
                    sbVal.AppendLine(kv1.Value);
                }
                else
                {
                    sbKey.AppendLine(kv1.Key + ",");
                    sbVal.AppendLine(kv1.Value + ",");
                }

                // move on
                count++;

            }
            sbKey.AppendLine(")");
            sbVal.AppendLine("");

            string r = sbHeade.ToString() + sbKey.ToString() + sbVal.ToString() + sbFoot.ToString();
            return r;
        }

        public string GetSql_InsertValues()
        {
            StringBuilder sbHeade = new StringBuilder();
            StringBuilder sbKey = new StringBuilder();
            StringBuilder sbVal = new StringBuilder();
            StringBuilder sbFoot = new StringBuilder();

            //INSERT INTO [dbo].[MessageFileTable2]
            sbHeade.AppendLine("INSERT INTO " + TableName);
            sbKey.AppendLine("(");
            sbVal.AppendLine("VALUES (");
            int count = 0;
            foreach (KeyValuePair<string, string> kv1 in Data)
            {
                if (count == (Data.Count - 1))
                {
                    // last
                    sbKey.AppendLine(kv1.Key);
                    sbVal.AppendLine(kv1.Value);
                }
                else
                {
                    sbKey.AppendLine(kv1.Key + ",");
                    sbVal.AppendLine(kv1.Value + ",");
                }

                // move on
                count++;

            }
            sbKey.AppendLine(")");
            sbVal.AppendLine(");");

            string r = sbHeade.ToString() + sbKey.ToString() + sbVal.ToString() + sbFoot.ToString();
            return r;
        }
    }

    public static class SQLDBConfig
    {
        //private static string s_DatabaseConnectionOld = "Data Source=198.38.83.200;                                    Integrated Security=False;                           User ID=mgillman_DBTester13;Password=Tester13;  Connect Timeout=15;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //private static string s_DBTest = "Data Source=198.38.83.200;Initial Catalog=mgillman_TestDBNice;User ID=mgillman_Tester;Password=Tester13;Integrated Security=FALSE;Persist security info=True;";
        // not working private static string s_DBTest = "Data Source=198.38.83.200;Integrated Security=True; Encrypt=True;Initial Catalog=mgillman_TestDBNice;User ID=mgillman_Tester;Password=Tester13;";
        private static string s_LogDB = "Data Source=198.38.83.200;Integrated Security=FALSE;Persist security info=True;Initial Catalog=mgillman_TestDBNice;User ID=mgillman_Tester;Password=Tester13;";
        //private static string s_DBLive = "Data Source=198.38.83.200;Initial Catalog=mgillman_NiceApi.net_Live;User ID=mgillman_Live;Password=Johanna_2014;Integrated Security=FALSE;Persist security info=True;";

        public enum DBToUse
        {
            LogDB,
            DevDB,
            LiveDB
        }
        public static string GetDBString(DBToUse db, bool returnAllFields)
        {
            string r = null;
            switch (db)
            {
                case DBToUse.LogDB:
                    r = s_LogDB; break;
                case DBToUse.DevDB:
                    r = s_LogDB; break;
                default:
                    throw new ArgumentOutOfRangeException("DBToUse");
            }

            if (!returnAllFields)
            {
                int index = r.IndexOf("Password=", StringComparison.InvariantCultureIgnoreCase);
                if (index > 0)
                {
                    r = r.Substring(0, index);
                }
            }
            return r;
        }



    }
 

    public class SqlDisposable : IDisposable
    {
        private static Object s_Lock = new Object();
        //private static string s_DatabaseConnectionOld = "Data Source=198.38.83.200;                                    Integrated Security=False;                           User ID=mgillman_DBTester13;Password=Tester13;  Connect Timeout=15;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //private static string s_DatabaseConnection    = "Data Source=198.38.83.200;Initial Catalog=mgillman_TestDBNice;Integrated Security=FALSE;Persist security info=True;User ID=mgillman_Tester;Password=Tester13";

        private static SqlConnection s_Con = null;

        private SqlConnection Conn;
        public SqlDataReader Reader;
        private SqlCommand Cmd;
        public Stopwatch TimeConnect;
        public Stopwatch TimeExecute;
        private SQLDBConfig.DBToUse Db;

        public SqlDisposable(SQLDBConfig.DBToUse db, string sqlCommand)
        {
            System.Threading.Monitor.Enter(s_Lock);
            this.Db = db;
            try
            {
                ReConnect();
                TimeExecute = Stopwatch.StartNew();
                Cmd = new SqlCommand(sqlCommand, Conn);
                Reader = Cmd.ExecuteReader();
                TimeExecute.Stop();
            }
            catch (SystemException se)
            {
                throw se;
            }
        }

        public void Dispose()
        {
            if (Reader != null)
            {
                Reader.Close();
            }
            System.Threading.Monitor.Exit(s_Lock);
        }

        private void ReConnect()
        {
            TimeConnect = Stopwatch.StartNew();
            if (s_Con == null)
            {
                s_Con = new SqlConnection(SQLDBConfig.GetDBString(Db, true));
                s_Con.Open();
            }
            if (s_Con.State != System.Data.ConnectionState.Open)
            {
                s_Con = new SqlConnection(SQLDBConfig.GetDBString(Db, true));
                s_Con.Open();
            }
            if (s_Con.ConnectionString != SQLDBConfig.GetDBString(Db, true))
            {
                s_Con = new SqlConnection(SQLDBConfig.GetDBString(Db, true));
                s_Con.Open();
            }
            Conn = s_Con;
            TimeConnect.Stop();
        }

    }

    public static class SqlExtensions
    {
        public static string Quote(this string sIn)
        {
            return "'" + sIn + "'";
        }
        public static string Bracket(this string sIn)
        {
            return "[" + sIn + "]";
        }
        public static string SqlWhereEmail(this string email)
        {
            return String.Format("WHERE [Email] = {0}", email.Quote());
        }
        private static Int64 SqlDatePivot = 63082281600;
        public static Int64 SqlDateRead(this System.Data.SqlClient.SqlDataReader r, string key)
        {
            object o = r[key];
            if (o.GetType() == typeof(System.Int32))
            {
                Int64 i = (Int64)(int)o;
                i += SqlDatePivot;
                i *= TimeSpan.TicksPerSecond;

                // test
                //SqlDate(i);
                return i;
            }
            if (o.GetType() == typeof(System.DBNull))
            {
                return 0;
            }
            string type = o.GetType().ToString();
            return 0;
        }
        public static int SqlDateUtcNow()
        {
            return SqlDate(DateTime.UtcNow.Ticks);
        }
        public static int SqlDate(this Int64 i)
        {
            if (i == 0)
            {
                return 0;
            }

            DateTime dt = new DateTime(i, DateTimeKind.Utc);
            Int64 div = dt.Ticks / TimeSpan.TicksPerSecond;
            Int64 save = div - SqlDatePivot;

            // test
            //DateTime dtBack = new DateTime((save + SqlDatePivot) * TimeSpan.TicksPerSecond, DateTimeKind.Utc);

            //DateTime pif = new DateTime(2000, 1, 1, 0, 0, 0);
            //Int64 divPif = pif.Ticks / TimeSpan.TicksPerSecond;
            return (int)save;
        }
        //public static string SqlDate(this Int64 i)
        //{
        //    DateTime dt = new DateTime(i, DateTimeKind.Utc);
        //    string r = dt.ToString("yyyyMMdd HH:mm:ss");
        //    return r;
        //}
        public static string ToIntString(this Data_AppUserFile.eUserStatus s)
        {
            int i = (int)s;
            string str = i.ToString();
            return str;
        }
    }
}
