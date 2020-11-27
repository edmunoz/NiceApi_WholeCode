using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Drawing;
using NiceApiLibrary_low;


namespace NiceTray
{
    public class HttpServer : IDisposable
    {
        private Thread intThread = null;
        public StringBuilder Result = new StringBuilder();

        private TcpListener Listener;

        public void GoAndReturn()
        {
            intThread = new Thread(new ThreadStart(GoAndStay));
            intThread.Start();
        }

        public void Dispose()
        {
            if (intThread != null)
            {
                intThread.Abort();
            }
        }

        public void GoAndStay()
        {
            try
            {
                ListenerLoop();
            }
            catch (SystemException se)
            {
                Result.AppendLine(se.Message);
            }
        }

        private void ListenerLoop()
        {
            Int16 port = Int16.Parse(ConfigurationManager.AppSettings["HTTP.ListenerPort"]);
            if (port == 0)
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
            else
            {
                Listener = new TcpListener(port);
                Listener.Start();
                while (true)
                {
                    try
                    {
                        HandleClientCreateThread(Listener.AcceptTcpClient());
                    }
                    catch (SystemException se)
                    {
                        Result.AppendLine(se.Message);

                    }
                }
            }
        }

        private static void HandleClientCreateThread(TcpClient client)
        {
            Thread th = new Thread(new ParameterizedThreadStart(HandleClientTheThread));
            th.Start(client);
        }

        private static void HandleClientTheThread(object obj)
        {
            try
            {
                TcpClient client = (TcpClient)obj;
                HandleClient(client);
            }
            catch
            {

            }
        }

        private static void HandleClient(TcpClient client)
        {
            Stream stream = client.GetStream();
            HandleClient(stream, stream);
            stream.Flush();
            stream.Close();
        }

        public static void HandleClient(Stream inStream, Stream outStream)
        {
            // 1) Read
            string get = null;
            StreamReader sr = new StreamReader(inStream);
            StreamWriter sw = new StreamWriter(outStream);
            while (true)
            {
                string line = sr.ReadLine();
                if (line == null)
                {
                    break;
                }
                else if (line.StartsWith("GET "))
                {
                    get = line.Substring(5).Replace(" HTTP/1.1", "");
                }
                else if (line.Length == 0)
                {
                    break;
                }
            }

            // Response
            string ContentType;
            MemoryStream Content;
            new HTML().GetData(get, out Content, out ContentType);
            writeHeaderAndData(sw, ContentType, Content);
            sw.Write("  ");
            sw.Flush();

            sr.Close();
            sw.Close();
        }

        private static void writeHeaderAndData(StreamWriter sw, string ContentType, MemoryStream msData)
        {
            sw.WriteLine("HTTP/1.1 200 Ok");
            sw.WriteLine("Content-Type: " + ContentType); /*text/html*/
            sw.WriteLine("Server: SimpleServer_HTTP");
            sw.WriteLine("Content-Length: " + msData.Length);
            sw.WriteLine("");
            sw.Flush();
            msData.WriteTo(sw.BaseStream);
        }
    }

    class HTML
    {
        private HttpServerMemory Mem = new HttpServerMemory();
        private MemoryStream MS = new MemoryStream();
        private string ContentType = "text/html";
        private void MSAppendLine(string line)
        {
            byte[] baLine = Encoding.ASCII.GetBytes(line + Environment.NewLine);
            MS.Write(baLine, 0, baLine.Length);
        }

        //public string GetTest()
        //{
        //    MS = new MemoryStream();
        //    addHeader("Test");
        //    addBodyTest();
        //    addFooter();
        //    return SB.ToString();
        //}

        public void GetData(string request, out MemoryStream msOut, out string ContentType)
        {
            MS = new MemoryStream();
            KnownFiles.eKnownFiles which;
            if (KnownFiles.IsKnown(request, out which))
            {
                addHeader(KnownFiles.Get(which));
                addBodyLinks(KnownFiles.GetAll());
                addBodyFile(which);
                addFooter();
            }
            else if (request == "Screen")
            {
                Bitmap bAll = ImageCapture.GetAll();
                bAll.Save(MS, System.Drawing.Imaging.ImageFormat.Png);
                this.ContentType = "image/png";
            }
            else if (request == "FullInfo")
            {
                this.ContentType = "text/plain";
                TrayStatus full = Mem.GetValues();
                if (DisplayTextController._globInstance != null)
                {
                    full.AddRange(DisplayTextController._globInstance.GetStatus());
                }
                MSAppendLine(full.ToString());
            }
            else if (request == "FullInfoIM")
            {
                this.ContentType = "text/plain";
                StringBuilder sb = new StringBuilder();
                TrayStatus full = Mem.GetValues();
                if (DisplayTextController._globInstance != null)
                {
                    full.AddRange(DisplayTextController._globInstance.GetStatus());
                }
                full.TheList.ForEach(i => i.ToIntermapperInfo(sb));
                MSAppendLine(sb.ToString());
            }
            else if (request == "Memory")
            {
                addHeader(request);
                addBodyLinks(KnownFiles.GetAll());
                MSAppendLine("<pre>");
                MSAppendLine(Mem.GetValues().ToString());
                MSAppendLine("</pre>");
                addFooter();
            }
            else if (request == "DelPic")
            {
                addHeader(request);
                addBodyLinks(KnownFiles.GetAll());
                foreach (BmpFileHandler.PathAndDate f1 in new BmpFileHandler(".").GetSortedList())
                {
                    if (f1.Path.Contains("zapi"))
                    {
                        File.Delete(f1.Path);
                        MSAppendLine(f1.Path + "<br />");
                    }
                }
                addFooter();
            }
            else if (request == "ListPic")
            {
                addHeader(request);
                addBodyLinks(KnownFiles.GetAll());
                MSAppendLine("<pre>");
                foreach (BmpFileHandler.PathAndDate f1 in new BmpFileHandler(".").GetSortedList())
                {
                    MSAppendLine(f1.LastWrite.ToString() + " " + f1.Path + "<br />");
                }
                MSAppendLine("</pre>");
                addFooter();
            }
            else if (request.StartsWith("ShowPic_"))
            {
                try
                {
                    int id = int.Parse(request.Substring("ShowPic_".Length));
                    addHeader(request);
                    addBodyLinks(KnownFiles.GetAll());
                    MSAppendLine("<br>");
                    MSAppendLine("<br>");
                    foreach (BmpFileHandler.PathAndDate f1 in new BmpFileHandler(".").GetSortedList())
                    {
                        MSAppendLine(f1.Path);
                        MSAppendLine("<br>");
                        MSAppendLine(string.Format("<img src=\"Pic_{0}\" width=\"600\" height=\"500\" > ", f1.Path));
                        MSAppendLine("<hr>");
                    }
                    addFooter();
                }
                catch (Exception _)
                {

                }
            }
            else if (request.StartsWith("Pic_"))
            {
                string path = null;
                try
                {
                    path = request.Substring("Pic_".Length).Replace("%20", " ");
                    Bitmap bAll = new System.Drawing.Bitmap(path);
                    bAll.Save(MS, System.Drawing.Imaging.ImageFormat.Png);
                    this.ContentType = "image/png";
                }
                catch (Exception _)
                {
                }
            }
            else if (request == "DelLog")
            {
                addHeader(request);
                addBodyLinks(KnownFiles.GetAll());
                KnownFiles.ioDelete(KnownFiles.eKnownFiles.Log);
                addFooter();
            }
            else if (request == "Step")
            {
                addHeader(request);
                addBodyLinks(KnownFiles.GetAll());
                MSAppendLine("<pre>");
                MSAppendLine("_6WhatsAppProcess_Real performs steps to send a whatsapp message:<br>");
                MSAppendLine("Step 1: findSearchWindow and enter tel<br>");
                MSAppendLine("Step 2: findAccount and Click it<br>");
                MSAppendLine("Step 3: verify account string top rigth position<br>");
                MSAppendLine("Step 4: find type-msg-here, and enter text<br>");
                MSAppendLine("</pre>");
                addFooter();
            }
            else 
            {
                addHeader("No selection");
                addBodyLinks(KnownFiles.GetAll());
                addFooter();
            }

            msOut = this.MS;
            ContentType = this.ContentType;
        }

        private void addHeader(string title)
        {
            MSAppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            MSAppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
            MSAppendLine("<html><head><title>" + title + "</title></head><body>");
        }

        private void addBodyLinks(List<string> names)
        {
            foreach (string name in names)
            {
                MSAppendLine("<a href=\"" + name + "\">" + name + "</a><br />");
            }
            MSAppendLine("<a href=\"Step\">Step</a><br />");
            MSAppendLine("<a href=\"ListPic\">ListPic</a><br />");
            MSAppendLine("<a href=\"DelPic\">DelPic</a><br />");
            MSAppendLine("<a href=\"DelLog\">DelLog</a><br />");
            MSAppendLine("<a href=\"Memory\">Memory</a><br />");
            MSAppendLine("<a href=\"FullInfo\">FullInfo</a><br />");
            MSAppendLine("<a href=\"ShowPic_0\">ShowPic_0</a><br />");
        }

        private void addBodyFile(KnownFiles.eKnownFiles which)
        {
            try
            {
                MSAppendLine("<pre>");
                MSAppendLine(KnownFiles.ioReadAllText(which));
                MSAppendLine("</pre>");
            }
            catch (IOException)
            { }
        }

        private void addBodyTest()
        {
            MSAppendLine("<pre>TestText\nTest</pre>");
        }

        private void addFooter()
        {
            MSAppendLine("</body></html>");
        }
    }

    //public class AndroidMemory
    //{
    //    class oneSet
    //    {
    //        public long total;
    //        public long free;
    //        public long max;

    //        public override string ToString()
    //        {
    //            return string.Format("total:{0} free:{1} max:{2}", total, free, max);
    //        }

    //        public void Update(oneSet other)
    //        {
    //            this.total = other.total;
    //            this.free = other.free;
    //            this.max = other.max;
    //        }
    //    }
    //    private static oneSet Now = new oneSet();
    //    public static string CurrentStatus()
    //    {
    //        string ret = null;
    //        System.Threading.Monitor.Enter(Now);
    //        try
    //        {
    //            ret = Now.ToString();
    //        }
    //        finally
    //        {
    //            System.Threading.Monitor.Exit(Now);
    //        }
    //        return ret;
    //    }
    //    internal static HttpServerMemory.Usage CurrentUsage()
    //    {
    //        HttpServerMemory.Usage r = null;
    //        System.Threading.Monitor.Enter(Now);
    //        try
    //        {

    //            //totalMemory: 65 441 792
    //            //freeMemory:   4 150 256
    //            //maxMemory:  201 326 592

    //            //totalMemory: 65 441 792
    //            //freeMemory:   4 085 184
    //            //maxMemory:  201 326 592

    //            //totalMemory: 52 760 576
    //            //freeMemory:   2 462 152
    //            //maxMemory:  201 326 592

    //            //totalMemory: 52 772 864
    //            //freeMemory:   3 362 728
    //            //maxMemory:  201 326 592

    //            r = new HttpServerMemory.Usage("Android", 100, 100, Now.ToString(), false);
    //        }
    //        finally
    //        {
    //            System.Threading.Monitor.Exit(Now);
    //        }
    //        return r;
    //    }
    //    public static void OnAndroidMemory(string[] lines)
    //    {
    //        if (
    //            (lines.Length == 3) && 
    //            (lines[0].StartsWith("totalMemory: ")) &&
    //            (lines[1].StartsWith("freeMemory: ")) && 
    //            (lines[2].StartsWith("maxMemory: ")))
    //        {
    //            oneSet set = new oneSet();
    //            try
    //            {
    //                set.total = long.Parse(lines[0].Substring("totalMemory: ".Length)) / (1024 * 1024);
    //                set.free = long.Parse(lines[1].Substring("freeMemory: ".Length)) / (1024 * 1024);
    //                set.max = long.Parse(lines[2].Substring("maxMemory: ".Length)) / (1024 * 1024);
    //            }
    //            catch
    //            {
    //                return;
    //            }
    //            System.Threading.Monitor.Enter(Now);
    //            Now.Update(set);
    //            System.Threading.Monitor.Exit(Now);
    //        }
    //    }
    //}

    public class HttpServerMemory
    {
        //internal class Usage
        //{
        //    public string Id;
        //    public long Max;
        //    public long Used;
        //    public string AdditionalInfo;

        //    public Usage(string Id, long Max, long Used, string AdditionalInfo, bool invert)
        //    {
        //        this.Id = Id;
        //        this.Max = Max;
        //        this.Used = invert ? Max - Used : Used;
        //        this.AdditionalInfo = AdditionalInfo;
        //    }

        //    public override string ToString()
        //    {
        //        return string.Format("{0}: {1,4}% used. {2} / {3}. {4}",
        //            Id.PadRight(9),
        //            100 * Used / Max,
        //            Used,
        //            Max,
        //            AdditionalInfo);
        //    }
        //}

        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private UInt64 ramTotal;
        public HttpServerMemory()
        {
            InitialiseCPUCounter();
            InitializeRAMCounter();
            InitializeRAMTotal();
        }

        private void InitializeRAMTotal()
        {
            try
            {
                ManagementScope Scope;
                Scope = new ManagementScope(String.Format("\\\\{0}\\root\\CIMV2", "."), null);

                Scope.Connect();
                ObjectQuery Query = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
                ManagementObjectSearcher Searcher = new ManagementObjectSearcher(Scope, Query);
                foreach (ManagementObject WmiObject in Searcher.Get())
                {
                    ramTotal += (UInt64)WmiObject["Capacity"];
                }
                ramTotal /= 1024 * 1024;
            }
            catch (Exception _)
            {
            }
        }

        public TrayStatus GetValues()
        {
            try
            {
                TrayStatus ret = new TrayStatus();
                try
                {
                    ret.Add(new DetailedData_Usage(
                        eDisplayItem.MemCPU,
                        100, (long)C9_CPUSlowdown._sLastCpuUsage, "test", false));
                }
                catch (SystemException se)
                {
                    ret.__Add(DateTime.Now, eDisplayItem.__Error, "MemCPU: " + se.Message);
                }

                try
                {
                    ret.Add(new DetailedData_Usage(
                        eDisplayItem.MemRAM,
                        (long)ramTotal, (long)ramCounter.NextValue(), "(Mb)", false));
                }
                catch (SystemException se)
                {
                    ret.__Add(DateTime.Now, eDisplayItem.__Error, "MemRAM: " + se.Message);
                }

                try
                { 
                    ret.Add(GetTotalFreeDiskSpace());
                }
                catch (SystemException se)
                {
                    ret.__Add(DateTime.Now, eDisplayItem.__Error, "MemCDrive: " + se.Message);
                }

                try
                { 
                    ret.Add(new DetailedData_BMPs(
                        new BmpFileHandler(".").GetSortedList().Length));
                }
                catch (SystemException se)
                {
                    ret.__Add(DateTime.Now, eDisplayItem.__Error, "MemBMPs: " + se.Message);
                }

                try
                {
                    long? size = NiceTray._2InfoDisplay_FromApp.TheApp?.FileSize();
                    if (!size.HasValue)
                    {
                        size = -1;
                    }
                    ret.Add(new DetailedData_Logfile(size.Value));
                }
                catch (SystemException se)
                {
                    ret.__Add(DateTime.Now, eDisplayItem.__Error, "MemBMPs: " + se.Message);
                }

                return ret;
            }
            catch
            {
                return TrayStatus.Error("memory ERROR");
            }
        }

        private DetailedData_Usage GetTotalFreeDiskSpace()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    return new DetailedData_Usage(eDisplayItem.MemCDrive, (long)drive.TotalSize, (long)drive.AvailableFreeSpace, "", true);
                }
            }
            return null;
        }

        private void InitialiseCPUCounter()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        private void InitializeRAMCounter()
        {
            ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);
        }
    }

    class BmpFileHandler
    {
        public class PathAndDate
        {
            public string Path;
            public DateTime LastWrite;

            public PathAndDate(string path)
            {
                Path = path;
                LastWrite = File.GetLastWriteTime(path);
            }
        }

        private List<PathAndDate> TheList;

        public BmpFileHandler(string path)
        {
            TheList = new List<PathAndDate>();
            foreach (string f1 in Directory.GetFiles(path, "*.bmp"))
            {
                TheList.Add(new PathAndDate(f1));
            }
            TheList.Sort(DateSorter);
        }

        public int DateSorter(PathAndDate a, PathAndDate b)
        {
            if (a.LastWrite < b.LastWrite)
            {
                return -1;
            }
            else if (a.LastWrite == b.LastWrite)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }


        public PathAndDate[] GetSortedList()
        {
            return TheList.ToArray();
        }

        public string GetSortedListString()
        {
            string r = "";
            foreach (PathAndDate f1 in GetSortedList())
            {
                r += "F: " + f1.LastWrite.ToString() + " " + f1.Path + " " + "\r\n";
            }
            return r;
        }
    }
}
