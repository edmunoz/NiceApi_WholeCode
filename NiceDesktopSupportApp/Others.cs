using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Reflection;

using NiceApiLibrary;
using NiceApiLibrary_low;

namespace NiceDesktopSupportApp
{
    class Others
    {
        public static void GetNewAPIId(IMyLog log, QuestionOption it)
        {
            string email = Question.Ask("Enter email");
            string apiid = Data_AppUserFile.API_ToId(email, Guid.NewGuid());
            Console.WriteLine("New APIId produced for " + email);
            Console.WriteLine(apiid);

        }

        public static void JustALogMessage(IMyLog log, QuestionOption it)
        {
            log.Info("JustALogMessage");
        }

        public static void LibVersion(IMyLog log, QuestionOption it)
        {
            Assembly a = Assembly.GetAssembly(typeof(IMyLog));
            Console.Write(a.FullName.ToString() + "\r\n");
        }
    }
}
