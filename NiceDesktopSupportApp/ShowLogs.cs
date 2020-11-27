using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

using NiceApiLibrary;
using NiceApiLibrary_low;

namespace NiceDesktopSupportApp
{
    class ShowLogs
    {
        private IMyLog log;
        private Question ques;
        private bool stayInLoop;
        private string limitQueryForDate = string.Empty;

        private ShowLogs(IMyLog log)
        {
            ques = new Question();
            ques.Add(new QuestionOption("ERROR", lAny));
            ques.Add(new QuestionOption("Test", lAny));
            ques.Add(new QuestionOption("SupportApp", lAny));
            ques.Add(new QuestionOption("Global", lAny));
            ques.Add(new QuestionOption("Login", lAny));
            ques.Add(new QuestionOption("TrayApp", lAny));
            ques.Add(new QuestionOption("API", lAny));
            ques.Add(new QuestionOption("APIForm", lAny));
            ques.Add(new QuestionOption("Register", lAny));
            ques.Add(new QuestionOption("Email", lAny));
            ques.Add(new QuestionOption("Upgrade", lAny));
            ques.Add(new QuestionOption("LimitForDate", lLimit));

            setLimitQueryForDate(3);
            stayInLoop = true;
            this.log = log;
        }

        private void setLimitQueryForDate(Int32 days)
        {
            limitQueryForDate = 
                " AND [Date] >= '" +
                DateTime.Now.Date.AddDays(-1 * days).ToString("yyyy-MM-dd") +
                "' ";
        }

        private void lLimit(IMyLog log, QuestionOption it)
        {
            Console.Clear();
            Console.WriteLine("Current val: " + limitQueryForDate);
            if (Question.Ask("Edit?", "Yes"))
            {
                try
                {
                    Int32 newDays = Int32.Parse(Question.Ask("Enter days"));
                    setLimitQueryForDate(newDays);
                }
                catch
                {

                }
            }
        }

        private void lAny(IMyLog log, QuestionOption it)
        {
            Console.Clear();
            string select = "SELECT [Date], [Level], [Logger], [Message] FROM [dbo].[Log] ";

            string where = String.Format("WHERE [Logger] like '{0}'", it.OptionText);
            if (!String.IsNullOrEmpty(limitQueryForDate))
            {
                where += limitQueryForDate;
            }

            string order = "ORDER BY [Date] ";

            string cmd = select + where + order;
//            String.Format(@"
//SELECT [Date], [Level], [Logger], [Message] FROM [dbo].[Log] 
//WHERE [Logger] like '{0}'
//ORDER BY [Date]
//", it.OptionText);

            using (SqlDisposable s = new SqlDisposable(SQLDBConfig.DBToUse.LogDB, cmd))
            {
                while (s.Reader.Read())
                {
                    DateTime Date = (DateTime)s.Reader["Date"];
                    Date = Date.AddHours(4);//mg to get date in brasil
                    String Level = (String)s.Reader["Level"];
                    String Logger = (String)s.Reader["Logger"];
                    String Message = (String)s.Reader["Message"];
                    Console.WriteLine(String.Format("{0:dd/MM HH:mm} {1} {2} {3}",
                        Date, Level, Logger, Message));
                }
            }
        }

        private void Loop()
        {
            while (stayInLoop)
            {
                ques.AskAndAct("Select", log);
            }
        }

        public static void Go(IMyLog log, QuestionOption it)
        {
            new ShowLogs(log).Loop();
        }
    }
}
