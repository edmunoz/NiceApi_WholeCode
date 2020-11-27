using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Configuration;

using NiceApiLibrary_low;


namespace NiceTray
{
    delegate void d_onParsedString(string value);

    public class _8UpdateCommunicator_Real : I8_UpdateCommunicator
    {
        public enum Cmds
        {
            get,
            set,
            memory,
            exit,
        }

        public _8AndroidCommunicator Android()
        {
            return new _8AndroidCommunicator_Real(
                "_8UpdateCommunicator.Real.AndroidEndpoint".AppSettingsGet(),
                "_8UpdateCommunicator.Real.ClearLogFirst".IsAppSettingsTrue());
        }

        public _8ServerCommunicator Server()
        {
            return new _8ServerCommunicator_Real();
        }
    }

    public class _8ServerCommunicator_Real : _8ServerCommunicator
    {
        public void Dispose()
        {

        }

        public List<string> GetCommand(I2_InfoDisplay i2)
        {
            i2.FileLog_Debug("_8ServerCommunicator_Real.GetCommand ...");
            List<string> retOnServer = new List<string>();
            string url = "http://niceapi.net/ItemX?id=GetTelNumbers&sub=" + "_3GetData.Server.TrayType".GetConfig();

            i2.FileLog_Debug("_8ServerCommunicator_Real.GetCommand " + url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    string lastLine = "???";
                    try
                    {
                        while (true)
                        {
                            string serverLine = sr.ReadLine();
                            if (serverLine == null)
                            {
                                throw new IOException();
                            }
                            lastLine = serverLine;
                            if (serverLine.StartsWith("+"))
                            {
                                retOnServer.Add(serverLine);
                            }
                        }
                    }
                    catch (IOException)
                    {

                    }
                    i2.FileLog_Info("ServerUpdate done: " + lastLine);
                    if (retOnServer.Count > 0)
                    {
                        i2.FileLog_Info("ServerUpdate 1st Entry: " + retOnServer[0]);
                    }
                }
            }
            return retOnServer;
        }
    }

    public class _8AndroidCommunicator_Real : _8AndroidCommunicator
    {
        private TcpClient Client;
        public StreamReader Read;
        public StreamWriter Write;
        private d_OnAndroidDispose OnDone;
        private string Endpoint;
        private bool InitHasBeenCalled;
        private bool FileLogDeletedFirst;
        private bool ClearLogFirst;
        private bool ClearLogFirst_Done;

        public _8AndroidCommunicator_Real(string Endpoint, bool ClearLogFirst)
        {
            this.Endpoint = Endpoint;
            this.ClearLogFirst = ClearLogFirst;
        }

        private void fileLog(bool isWrite, string msg)
        {
            if (ClearLogFirst && !ClearLogFirst_Done)
            {
                ClearLogFirst_Done = true;
                KnownFiles.ioDelete(KnownFiles.eKnownFiles.AndroidCom);
                KnownFiles.ioAppend(KnownFiles.eKnownFiles.AndroidCom, Endpoint);
            }
            var line = $"{DateTime.UtcNow.ToSwissTime(false)}: {(isWrite ? "pc --> a" : "pc <-- a")}: {msg}";
            KnownFiles.ioAppend(KnownFiles.eKnownFiles.AndroidCom, line);
        }

        private void InitIfNot()
        {
            if (!InitHasBeenCalled)
            {
                InitHasBeenCalled = true;
                string[] endPointArray = Endpoint.Split(new char[] { ':' });
                var endPoint = IPAddress.Parse(endPointArray[0]);
                IPEndPoint ipEnd = new IPEndPoint(endPoint, int.Parse(endPointArray[1]));

                Client = new TcpClient();
                Client.Connect(ipEnd);
                Read = new StreamReader(Client.GetStream());
                Write = new StreamWriter(Client.GetStream());
            }
        }

        public string GetEndPointInfo()
        {
            return Endpoint;
        }
        public List<string> GetCommand(I2_InfoDisplay i2)
        {
            InitIfNot();
            i2.AddLine2(new DetailedData_TypeAndAndroidEP() { Endpoint  = Endpoint });
            i2.FileLog_Debug("_8AndroidCommunicator_Real.GetCommand ...");
            List<string> retOnAndroid = new List<string>();
            string getResponse = Exchange(new AndroidCmdToken(_8UpdateCommunicator_Real.Cmds.get));
            string firstLine = "";
            using (AndroidStringParser p = new AndroidStringParser(getResponse))
            {
                p.Parse(delegate(string val)
                {
                    if (firstLine.Length == 0)
                    {
                        firstLine = val;
                    }
                    if (val.StartsWith("zapi_"))
                    {
                        retOnAndroid.Add("+" + val.Substring("zapi_".Length));
                    }
                });
            }
            i2.FileLog_Info("Android GET done: " + firstLine);
            if (retOnAndroid.Count > 0)
            {
                i2.FileLog_Info("Android GET 1st Entry: " + retOnAndroid[0]);
            }
            return retOnAndroid;
        }
        public void SetOnAndroidDispose(d_OnAndroidDispose cb)
        {
            OnDone = cb;
        }
        public void SetCommand(string telWithPlus, I2_InfoDisplay i2)
        {
            InitIfNot();
            i2.FileLog_Debug("Android Add: " + telWithPlus + " ...");
            string androidAnswer = Exchange(new AndroidCmdToken(_8UpdateCommunicator_Real.Cmds.set, telWithPlus.Replace("+", "")));
            i2.FileLog_Debug("Android Add: " + androidAnswer);
        }

        public class AndroidCmdToken
        {
            public string cmd;
            public AndroidCmdToken(_8UpdateCommunicator_Real.Cmds cmd, string aux = "")
            {
                this.cmd = cmd.ToString() + aux;
            }
        }

        public string Exchange(AndroidCmdToken t)
        {
            InitIfNot();
            fileLog(true, t.cmd);
            Write.WriteLine(t.cmd);
            Write.Flush();
            var ret = Read.ReadLine();
            fileLog(false, ret);
            return ret;
        }

        public string[] ExchangeN(AndroidCmdToken t, int numberOfLines)
        {
            InitIfNot();
            List<string> retList = new List<string>();
            fileLog(true, t.cmd);
            Write.WriteLine(t.cmd);
            Write.Flush();
            for (int i = 0 ; i < numberOfLines; i++)
            {
                var line = Read.ReadLine();
                fileLog(false, line);
                retList.Add(line);
            }
            return retList.ToArray();
        }

        public void Dispose()
        {
            if (FileLogDeletedFirst)
            {
                if (ClearLogFirst)
                {
                    fileLog(false, "**Dispose**");
                }
            }

            if ((Client != null) && (Client.Connected))
            {
                if (OnDone != null)
                {
                    OnDone(ExchangeN(new AndroidCmdToken(_8UpdateCommunicator_Real.Cmds.memory), 3));
                }
                string test = Exchange(new AndroidCmdToken(_8UpdateCommunicator_Real.Cmds.exit));
            }
        }
    }

    class AndroidStringParser : IDisposable
    {
        StringReader Sr;
        public AndroidStringParser(string strIn)
        {
            Sr = new StringReader(strIn);
        }

        public void Parse(d_onParsedString cb)
        {
            try
            {
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

        public void Dispose()
        {

        }
    }

}
