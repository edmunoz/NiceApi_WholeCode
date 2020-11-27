using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace NiceASP
{

    /// <summary>
    /// Summary description for XmlHelper
    /// </summary>
    public class XmlHelper
    {
        private XmlDocument xDoc;
        public XmlHelper()
        {
            xDoc = new XmlDocument();
        }

        public override string ToString()
        {
            string s = xDoc.InnerXml;
            return s;
        }

        public XmlElement AddRootElemet(string name, string attr, string attrVal)
        {
            XmlElement xe = xDoc.CreateElement(name);

            if ((!String.IsNullOrEmpty(attr)) && (!String.IsNullOrEmpty(attrVal)))
            {
                XmlAttribute xa = xDoc.CreateAttribute(attr);
                xa.Value = attrVal;

                xe.Attributes.Append(xa);

                xDoc.AppendChild(xe);
            }

            return xe;
        }

    }

    public static class CommonHelper
    {
        public static void DoLogout(SessionData sd)
        {
            sd.LoggonOnUserIsAdmin = false;
            sd.LoggedOnUserName = null;
            sd.LoggedOnUserEmail = null;
            sd.LoggedOnUsersNiceSystemInfo = null;
        }
    }
}
