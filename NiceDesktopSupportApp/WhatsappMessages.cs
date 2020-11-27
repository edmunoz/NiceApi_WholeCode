using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

using NiceApiLibrary;
using NiceApiLibrary_low;

namespace NiceDesktopSupportApp
{
    class WhatsappMessages
    {
        public static void Error500(IMyLog log, QuestionOption it)
        {
            string fileData = File.ReadAllText(@"C:\NiceApi\NiceLog_500Error1.txt");
            string[] fileDataSplit = fileData.Split(new string[] { "\r\n" }, 3, StringSplitOptions.None);

            string message = fileDataSplit[2];
            string apiMobile = fileDataSplit[1];
            string apiId = fileDataSplit[0];

            int msgLength = message.Length;
            //message = "".PadLeft(msgLength, 'A');


            SendNormalPost(message, apiMobile, apiId);
        }

        public static void Go(IMyLog log, QuestionOption it)
        {
            SendNormal(User.selectUser());
        }

        public static void GoSys01(IMyLog log, QuestionOption it) { SendNormal(User.userSys01()); }
        public static void GoSys02(IMyLog log, QuestionOption it) { SendNormal(User.userSys02()); }
        public static void GoSys03(IMyLog log, QuestionOption it) { SendNormal(User.userSys03()); }
        public static void GoSys03group(IMyLog log, QuestionOption it) { SendNormal(User.userSys03group()); }
        public static void GoSys04(IMyLog log, QuestionOption it) { SendNormal(User.userSys04()); }
        public static void GoSysTest(IMyLog log, QuestionOption it) { SendNormal(User.userSysTest()); }

        public static void CheckCredit(IMyLog log, QuestionOption it)
        {
            SendNormal(User.selectUser(), true);
        }

        static void SendNormal(IUser user, bool doCreditRequest = false)
        {
            try
            {
                string message = doCreditRequest ? "" : getMessage(user);

                string url = GetHost(doCreditRequest ? "APICredit" : "API");// "https://whatsappapi.net/API";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = doCreditRequest ? "GET" : "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-APIId", user.getId());
                if (!doCreditRequest)
                {
                    request.Headers.Add("X-APIMobile", user.getMobile());
                }
                LowLevelHttpDumper.Dump(request, message);
                Console.WriteLine(user.getId());
                Console.WriteLine(user.getMobile());
                Console.WriteLine(message);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                    {
                        streamOut.Write(message);
                    }
                }
                using (StreamReader streamIn = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    Console.WriteLine(streamIn.ReadToEnd());
                }
            }
            catch (SystemException se)
            {
                Console.WriteLine(se.Message);
            }
            Console.ReadLine();
        }

        static void SendNormalPost(string message, string aPIMobile, string aPIId)
        {
            try
            {
                string url = GetHost("API");// "https://whatsappapi.net/API";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded; charset=ISO-8859-1";
                request.Headers.Add("X-APIId", aPIId);
                request.Headers.Add("X-APIMobile", aPIMobile);
                LowLevelHttpDumper.Dump(request, message);
                Console.WriteLine(aPIId);
                Console.WriteLine(aPIMobile);
                Console.WriteLine(message);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                    {
                        streamOut.Write(message);
                    }
                }
                using (StreamReader streamIn = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    Console.WriteLine(streamIn.ReadToEnd());
                }
            }
            catch (SystemException se)
            {
                Console.WriteLine(se.Message);
            }
            Console.ReadLine();
        }

        static string getMessage(IUser user)
        {
            Console.Clear();
            Console.WriteLine("getMessage:");
            string ret = null;
            //if (user.getId() == "onmewUn3KkapJXDbR6yrhmdpbGxtYW5uX21hcnRpbl9hdF95YWhvb19kb3RfY29t")
            {
                // MG special
                Question q = new Question();
                q.Add(new QuestionOption("No", delegate (IMyLog log, QuestionOption it) { }));
                q.Add(new QuestionOption("Yes, Bad loos focuse 01.06.2020", delegate (IMyLog log, QuestionOption it)
                {
                    ret = "";
                    foreach (int i in new List<int>() { 0xf0, 0x9f, 0x8d, 0x8e })
                    {
                        ret += (char)i;
                    }
                    var x = ret;
                }));
                q.Add(new QuestionOption("Yes, Test \\n\\r", delegate (IMyLog log, QuestionOption it)
                {
                    ret = "NR-Test\n\rSecoundLine";
                }));
                q.Add(new QuestionOption("Yes, Test \\n", delegate (IMyLog log, QuestionOption it)
                {
                    ret = "N-Test\nSecoundLine";
                }));
                q.Add(new QuestionOption("Yes, Test \\r", delegate (IMyLog log, QuestionOption it)
                {
                    ret = "R-Test\n\rSecoundLine";
                }));
                q.Add(new QuestionOption("Yes, Test \\0", delegate (IMyLog log, QuestionOption it)
                {
                    ret = "0-Test\0SecoundLine";
                }));
                q.Add(new QuestionOption("Yes, The one failed with fernandoMedicaloffice0", delegate (IMyLog log, QuestionOption it)
                {
                    ret = @"

                    Dear FERNANDO BRITO,



                    We would like to remind you of your marked appointment, as follows:

 

Doctor: Doctor 1

Date: 06 / 28 / 2019 Friday

    Time: 09:30 am

    Agreement: < PARTICULAR >



    If you can not attend, we request that you notify us at least 24 hours in advance.



    Regards,

 

Doctor 1

Jovina, 344 - living room 81 - Vila Mascote

04363 - 080 - South Africa

11 3453 - 5867 / 11 99948 - 9202




Do not answer this message, if you want to keep in touch use the above phones

";
                }));

                q.AskAndAct("Do you want to send a special message", null);
            }

            if (ret == null)
            {
                Console.WriteLine(user.getName());
                Console.WriteLine("What to send (blank allowed)");
                ret = Console.ReadLine();
                if (ret.Length == 0)
                {
                    ret = "__NoSend and any text";
                }
            }
            return ret;
        }

        public static string GetHost(string ending)
        {
            Question q = new Question();
//            q.Add(new QuestionOption("https://whatsappapi.net/" + ending, null));
            q.Add(new QuestionOption("https://niceapi.net/" + ending, null));
            q.Add(new QuestionOption("http://localhost:60257/" + ending, null));
            q.Add(new QuestionOption("http://127.0.0.1/LocalNiceApi/" + ending, null));
            q.Add(new QuestionOption("http://localhost/Nice_WithSql/" + ending, null));
            QuestionOption o = q.AskAndReturnOption("Select host");
            return o.OptionText;
        }
    }
}
