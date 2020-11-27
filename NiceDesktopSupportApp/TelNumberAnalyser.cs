using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Diagnostics;

using NiceApiLibrary;
using NiceApiLibrary_low;


namespace NiceDesktopSupportApp
{
    class TelNumberAnalyser
    {
        public static void Analyse(IMyLog log, QuestionOption it)
        {
            try
            {
                Console.WriteLine("Getting # from server...");
                var data = ContactHost();
                var dir = ParseTextList(data);
                dumpDuplicates(dir);

            }
            catch (SystemException e)
            {
                Console.WriteLine("SystemException");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

        }

        private static void dumpDuplicates(Dictionary<string, List<string>> dir)
        {
            int noOfDuplicates = 0;
            foreach (var oneEntry in dir)
            {
                //Console.WriteLine(oneEntry.Key + " " + oneEntry.Value.Count.ToString());
                if (oneEntry.Value.Count > 1)
                {
                    Console.WriteLine(oneEntry.Key + " " + oneEntry.Value.Count.ToString());
                    foreach (var oneTel in oneEntry.Value)
                    {
                        Console.WriteLine(oneTel);
                    }
                    noOfDuplicates++;
                }
                //foreach (var oneTel in oneEntry.Value)
                //{
                //    Console.WriteLine(oneTel);
                //}
            }

            Console.WriteLine("No of duplicates: " + noOfDuplicates.ToString());
        }

        private static Dictionary<string, List<string>> ParseTextList(MemoryStream data)
        {
            data.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(data);
            var dir = new Dictionary<string, List<string>>();
            string currentUser = "";
            int lineCounter = 0;
            while (true)
            {
                var line = reader.ReadLine();
                lineCounter++;
                Debug.WriteLine(line);
                if (lineCounter > 2)
                {
                    if (line.StartsWith("User: "))
                    {
                        // user
                        currentUser = line.Substring(6);
                    }
                    else if (line.StartsWith("+"))
                    {
                        // tel#
                        if (dir.ContainsKey(line))
                        {

                        }
                        else
                        {
                            // new tel#
                            dir.Add(line, new List<string>());
                            dir[line].Add(currentUser);
                        }
                    }
                    else if (line.StartsWith("Sumary"))
                    {
                        Console.WriteLine(line);
                        break;
                    }
                    else
                    {

                    }
                }
            }
            return dir;
        }

        private static MemoryStream ContactHost()
        {
            using (var client = new HttpClient())
            {
                var serverData = client.GetAsync("https://niceapi.net/ItemX?id=GetAllTelNumbers").Result;
                if (serverData.StatusCode == HttpStatusCode.OK)
                {
                    var ms = new MemoryStream();
                    serverData.Content.ReadAsStreamAsync().Result.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;
                }
            }
            return null;
        }
    }
}
