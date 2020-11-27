
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Util;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace NiceBackgroundApp
{
    [Service]
    public class MyService : Service
    {
        private MyTcpListnerThread myTcpListnerThread;
        public IBinder Binder { get; private set; }

        public override void OnCreate()
        {
            // This method is optional to implement
            base.OnCreate();
            Log.Debug("SS", "OnCreate");
            myTcpListnerThread = new MyTcpListnerThread();
            myTcpListnerThread.StartThread();
            Log.Debug("SS", "OnCreate.Started");
        }

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug("SS", "OnBind");
            this.Binder = new Binder();
            return this.Binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            // This method is optional to implement
            Log.Debug("SS", "OnUnbind");
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            // This method is optional to implement
            Log.Debug("SS", "OnDestroy");
            Binder = null;
            base.OnDestroy();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Log.Debug("SS", "OnStartCommand");

            return StartCommandResult.Sticky;
        }

        //public override void OnStart(Intent intent, int startId)
        //{
        //    Log.Debug("SS", "OnStart");
        //    base.OnStart(intent, startId);
        //}
    }

    public class MyTcpListnerThread
    {
        /// <summary>
        /// get this to work on the emulator by
        /// 1) telnet localhost 5554
        /// 2) auth ...
        /// 3) redir add tcp:5050:9988
        /// 
        /// 4) now use telnet localhost 5050 to talk to this code here
        /// 
        /// </summary>
        public const int SLEPP_LONG = 1000;
        public const int SLEEP_SHORT = 100;
        private Thread thread;
        public void StartThread()
        {
            thread = new Thread(new ThreadStart(bg_ListenerMain));
            thread.Start();
        }

        private void bg_ListenerMain()
        {
            Log.Debug("MyTcpListnerThread", "bg_Main");
            while (true)
            {
                try
                {
                    Thread.Sleep(SLEEP_SHORT);
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (IPAddress ip1 in ipHostInfo.AddressList)
                    {
                        Log.Debug("MyTcpListnerThread a", ip1.ToString());
                    }
                    IPAddress ipAddress = ipHostInfo.AddressList[0];
                    Log.Debug("MyTcpListnerThread using", ipAddress.ToString());

                    TcpListener server = new TcpListener(ipAddress, 6000);
                    server.Start();
                    while (true)
                    {
                        TcpClient client = server.AcceptTcpClient();
                        Log.Debug("MyTcpListnerThread", "Incomming");
                        Thread thIncomming = new Thread(new ParameterizedThreadStart(bg_IncommingMain));
                        thIncomming.Start(client);
                    }
                }
                catch (System.Exception)
                {
                    Thread.Sleep(SLEPP_LONG);
                }
            }
        }

        private void bg_IncommingMain(object con)
        {
            try
            {
                Log.Debug("bg_IncommingMain", "start");
                TcpClient client = (TcpClient)con;
                MyServiceContent content = new MyServiceContent(client.GetStream());
                content.Handle();
            }
            catch (System.Exception)
            {
            }
        }
    }
}
