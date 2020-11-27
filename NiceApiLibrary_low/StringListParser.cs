using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public delegate void dOnOneString(string str);
    public static class StringListParser
    {
        public static string ToDataString(string[] strArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s1 in strArray)
            {
                string len = s1.Length.ToString();
                string lenlen = len.Length.ToString();
                sb.Append(lenlen);
                sb.Append(len);
                sb.Append(s1);
            }
            return sb.ToString();
        }

        public static string[] Parse(string dataInOneString)
        {
            List<string> list = new List<string>();
            Parse(dataInOneString, 
                delegate(string s) 
                {
                    list.Add(s);
                });
            return list.ToArray();
        }

        public static void Parse(string dataInOneString, dOnOneString cb)
        {
            try
            {
                StringReader Sr = new StringReader(dataInOneString);
                while (true)
                {
                    // 1) read lenlen
                    int lenlen = Sr.Read();
                    if (lenlen == -1)
                    {
                        throw new IOException();
                    }
                    lenlen -= '0';
                    // 2) read len
                    int len = 0;
                    for (int i = 0; i < lenlen; i++)
                    {
                        len *= 10;
                        len += Sr.Read() - '0';
                    }
                    // 3) read string
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < len; i++)
                    {
                        sb.Append((char)Sr.Read());
                    }
                    cb(sb.ToString());
                }
            }
            catch (IOException)
            {

            }
        }
    }
}
