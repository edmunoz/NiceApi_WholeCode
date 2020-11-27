using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml;

using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;


namespace NiceASP
{
    public class KeywordLoader
    {
        public enum Which
        {
            Default,
            HowItWorks,
            HowToUse,
            Login,
            Register,
            Price
        }
        public static void Load(Page page, Which which)
        {
            try
            {
                XmlDocument xD = new XmlDocument();
                xD.Load(FolderNames.GetFolder(NiceSystemInfo.DEFAULT, MyFolders.ASP_ServerStateFolder) + "\\KeyWords\\KeyWords.xml");
                XmlNode allkeyNode = xD["KeyWords"];
                XmlNode keyNode = null;
                switch (which)
                {
                    case Which.Default: keyNode = allkeyNode["Default"]; break;
                    case Which.HowItWorks: keyNode = allkeyNode["HowItWorks"]; break;
                    case Which.HowToUse: keyNode = allkeyNode["HowToUse"]; break;
                    case Which.Login: keyNode = allkeyNode["Login"]; break;
                    case Which.Register: keyNode = allkeyNode["Register"]; break;
                    case Which.Price: keyNode = allkeyNode["Price"]; break;
                }

                page.MetaKeywords = keyNode["KeyWords"].InnerText;
                page.MetaDescription = keyNode["Description"].InnerText;
                page.Title = keyNode["Title"].InnerText;

            }
            catch
            {

            }
        }
    }
}