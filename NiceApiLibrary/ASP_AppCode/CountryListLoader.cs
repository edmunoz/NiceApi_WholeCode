using NiceApiLibrary_low;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NiceApiLibrary.ASP_AppCode
{
    static public class CountryListLoader
    {
        public static Dictionary<int, string> List3Digit = new Dictionary<int, string>();
        public static Dictionary<int, string> List2Digit = new Dictionary<int, string>();

        public static string Lookup(string telNo)
        {
            try
            {
                if (telNo.StartsWith("+"))
                {
                    Int32 i3 = Int32.Parse(telNo.Substring(1, 3));
                    if (List3Digit.ContainsKey(i3))
                    {
                        return telNo.Substring(1, 3) + List3Digit[i3];
                    }

                    Int32 i2 = Int32.Parse(telNo.Substring(1, 2));
                    if (List2Digit.ContainsKey(i2))
                    {
                        return telNo.Substring(1, 2) + List2Digit[i2];
                    }
                }
            }
            catch
            {
            }
            return telNo;
        }

        public static void Load()
        {
            try
            {
                List3Digit.Clear();
                List2Digit.Clear();
                XmlDocument xD = new XmlDocument();
                xD.Load(FolderNames.GetFolder(NiceSystemInfo.DEFAULT, MyFolders.ASP_ServerStateFolder) + "\\CountryList.xml");
                XmlNode listNode = xD["CountryList"];
                foreach (XmlNode e1 in listNode)
                {
                    if (e1.Name.StartsWith("CC"))
                    {
                        if (e1.Name.Length == 5)
                        {
                            // 3 digit code
                            List3Digit.Add(Int32.Parse(e1.Name.Substring(2)), e1.InnerText);
                        }
                        if (e1.Name.Length == 4)
                        {
                            // 2 digit code
                            List2Digit.Add(Int32.Parse(e1.Name.Substring(2)), e1.InnerText);
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
