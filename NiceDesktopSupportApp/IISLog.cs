using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using System.Linq;

using NiceApiLibrary;
using NiceApiLibrary_low;

namespace NiceDesktopSupportApp
{
    public class IISLogEntry
    {
        public DateTime time;
        public String ssitename;
        public String sip;
        public String csmethod;
        public String csuristem;
        public String csuriquery;
        public int sport;
        public String csusername;
        public String cip;
        public String csUserAgent;
        public String csReferer;
        public String cshost;
        public int scstatus;
        public int scsubstatus;
        public int scwin32status;
        public int scbytes;
        public int csbytes;
        public int timetaken;

        public IISLogEntry(string line)
        {
            string[] sp = line.Split(new char[] { ' ' });
            time = DateTime.Parse(sp[0] + " " + sp[1]);
            ssitename = sp[2];
            sip = sp[3];
            csmethod = sp[4];
            csuristem = sp[5];
            csuriquery = sp[6];
            sport = Int32.Parse(sp[7]);
            csusername = sp[8];
            cip = sp[9];
            csUserAgent = sp[10];
            csReferer = sp[11];
            cshost = sp[12];
            scstatus = Int32.Parse(sp[13]);
            scsubstatus = Int32.Parse(sp[14]);
            scwin32status = Int32.Parse(sp[15]);
            scbytes = Int32.Parse(sp[16]);
            csbytes = Int32.Parse(sp[17]);
            timetaken = Int32.Parse(sp[18]);
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", time, csuristem, csReferer);
        }

        public string ReferenceShort
        {
            get
            {
                Uri myUri = new Uri(csReferer);   
                string host = myUri.Host;
                string[] parts = host.Split(new char[] { '.' });
                while ((parts.Length > 2) && (host.Length > 13))
                {
                    host = host.Substring(host.IndexOf('.') + 1);
                    parts = host.Split(new char[] { '.' });
                }
                return host;
            }
        }

        public bool WorthShowing
        {
            get
            {
                if (this.csReferer.Length > 1)
                {

                    if (csReferer.StartsWith("https://NiceApi.net"))
                    {
                        return false;
                    }
                    if (csReferer.StartsWith("http://niceapi.net"))
                    {
                        return false;
                    }
                    if (csReferer.StartsWith("https://niceapi.net"))
                    {
                        return false;
                    }
                    if (csReferer.StartsWith("http://www.niceapi.net"))
                    {
                        return false;
                    }
                    if (csReferer.StartsWith("https://www.niceapi.net"))
                    {
                        return false;
                    }
                    if (csReferer.StartsWith("http://localhost:"))
                    {
                        return false;
                    }
                    return true;
                }
                else if (
                    (csuristem == "/") ||
                    (csuristem == "/favicon.ico") ||
                    (csuristem == "/.well-known/pki-validation/gsdv.txt") ||
                    (csuristem == "/robots.txt")


                    )
                {
                    return false;
                }
                return false;
            }
        }
    }

    public class IISLog
    {
        public static void All(IMyLog log, QuestionOption it)
        {
            throw new NotImplementedException("not suitable for publication!");
        }

        public static void DaySummary(IMyLog log, QuestionOption it)
        {
            throw new NotImplementedException("not suitable for publication!");
        }
    }
}
