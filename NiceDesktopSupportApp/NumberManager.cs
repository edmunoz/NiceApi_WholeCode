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

    class WhatApp
    {
        private String APPIID;

        public WhatApp(String appid)
        {
            this.APPIID = appid;
        }

        public bool Send(String mobileNo, String message)
        {
            try
            {
                string url = "https://NiceApi.net/API";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-APIId", APPIID);
                request.Headers.Add("X-APIMobile", mobileNo);
                using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                {
                    streamOut.Write(message);
                }
                using (StreamReader streamIn = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    return streamIn.ReadToEnd().Equals("queued");
                }
            }
            catch
            {
            }
            return false;
        }

        public void AddNumberToCommercialAccount(String numberToAdd)
        {
            try
            {
                string url = "https://niceapi.net/APITelNumbers";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-APIId", APPIID);
                request.Headers.Add("X-APIInstruction", "Add");
                using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                {
                    streamOut.Write(numberToAdd);
                }
                using (StreamReader streamIn = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    streamIn.ReadToEnd();
                }
            }
            catch
            {
            }
        }

        static void AddNumber_Example()
        {
            try
            {
                string numberToAdd = "+55123456789";

                string url = "https://niceapi.net/APITelNumbers";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-APIId", "<Your unique X-APIId>");
                request.Headers.Add("X-APIInstruction", "Add");
                if (!string.IsNullOrWhiteSpace(numberToAdd))
                {
                    using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                    {
                        streamOut.Write(numberToAdd);
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

    }

    class NumberManager
    {
        public static void Manager(IMyLog log, QuestionOption it)
        {
            SendNormal(User.selectUser());
        }

        public static void SendTelSync(IMyLog log, QuestionOption it)
        {
            if (Question.Ask("This is dangerous, Do you really want to do that? (Y)", "Y"))
            {
                try
                {
                    string url = WhatsappMessages.GetHost("ItemX");// "https://whatsappapi.net/API";
                    url += "?id=GetAllTelNumbers";
                    if (Question.Ask("Add at least a noSync=1 (y)", "y"))
                    {
                        url += "&noSync=1";
                    }
                    Console.WriteLine("Your url is:");
                    Console.WriteLine(url);
                    if (Question.Ask("Use it? (Y)", "Y"))
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        //                    request.Method = "POST";
                        //                    request.ContentType = "application/x-www-form-urlencoded";
                        //                    request.Headers.Add("id", "GetAllTelNumbers");
                        //using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                        //{
                        //    streamOut.Write("");
                        //}
                        using (StreamReader streamIn = new StreamReader(request.GetResponse().GetResponseStream()))
                        {
                            Console.WriteLine(streamIn.ReadToEnd());
                        }
                    }
                }
                catch (SystemException se)
                {
                    Console.WriteLine(se.Message);
                }
                Question.Ask("Done");
            }
        }

        static void AddNumber_Example()
        {
            try
            {
                string numberToAdd = "+55123456789";

                string url = "https://niceapi.net/APITelNumbers";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-APIId", "<Your unique X-APIId>");
                request.Headers.Add("X-APIInstruction", "Add");
                if (!string.IsNullOrWhiteSpace(numberToAdd))
                {
                    using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                    {
                        streamOut.Write(numberToAdd);
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


        static void SendNormal(IUser user)
        {
            try
            {
                string instruction = getInstruction(user);
                string message = "";
                string method = "GET";
                switch (instruction)
                {
                    case "Add":
                    case "Remove":
                        message = getTelNumbers(user);
                        method = "POST";
                        break;
                }

                string url = WhatsappMessages.GetHost("APITelNumbers");// "https://whatsappapi.net/API";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-APIId", user.getId());
                request.Headers.Add("X-APIInstruction", instruction);
                LowLevelHttpDumper.Dump(request, message);
                Console.WriteLine(user.getId());
                Console.WriteLine(instruction);
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

        static void AddNumber()
        {
            try
            {
                string instruction = "Add";
                string message = "<TelNumbers>";
                string method = "POST";
                string url = WhatsappMessages.GetHost("APITelNumbers");// "https://whatsappapi.net/API";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-APIId", "<ApiId>");
                request.Headers.Add("X-APIInstruction", instruction);
                LowLevelHttpDumper.Dump(request, message);
                Console.WriteLine(instruction);
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


        static string getTelNumbers(IUser user)
        {
            string yourMessage = null;
            Console.WriteLine(user.getName());
            Console.WriteLine("Enter the Tel# string");
            yourMessage = Console.ReadLine();
            return yourMessage;
        }
        static string getInstruction(IUser user)
        {
            string ret = "";
            Console.WriteLine(user.getName());
            Question q = new Question();
            q.Add(new QuestionOption("Add", null));
            q.Add(new QuestionOption("Remove", null));
            q.Add(new QuestionOption("Show", null));
            q.Add(new QuestionOption("ShowConfirmed", null));
            q.Add(new QuestionOption("ShowUnconfirmed", null));
            ret = q.AskAndReturnOption("Which Instruction").OptionText;
            return ret;
        }

        //////////////////////////////////////////////////////////////////////
        public static void SendTelCheck(IMyLog log, QuestionOption it)
        {
            try
            {
                string telList = Question.Ask("TelList");

                string url = WhatsappMessages.GetHost("APIPrivAddTel");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-TelList", telList);
                request.Headers.Add("X-CheckTel", "1");
                LowLevelHttpDumper.Dump(request);
                using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                {
                    streamOut.Write(" ");
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
        public static void AddToFree(IMyLog log, QuestionOption it)
        {
            try
            {
                string email = Question.Ask("Email");
                string telList = Question.Ask("TelList");

                string url = WhatsappMessages.GetHost("APIPrivAddTel");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-EmailB64", Convert.ToBase64String(Encoding.ASCII.GetBytes(email)));
                request.Headers.Add("X-TelList", telList);
                LowLevelHttpDumper.Dump(request);
                using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                {
                    streamOut.Write(" ");
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
    }

    static class LowLevelHttpDumper
    {
        public static bool Enabled;

        public static void Dump(HttpWebRequest requ, string msg = null)
        {
            //if (Enabled)
            {
                Console.WriteLine($"--{requ.Method} {requ.RequestUri}");
                foreach (object h in requ.Headers)
                {
                    if (h.ToString().StartsWith("X-"))
                    {
                        Console.WriteLine($"-- {h}: {requ.Headers[h.ToString()]}");
                    }
                }
                if (msg != null)
                {
                    Console.WriteLine($"-- Msg: {msg}");
                }
            }

        }
    }
}
