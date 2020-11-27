// DataLoggerAccess.cs
//

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace CSC
{
    public static class DataLoggerAccess
    {
        enum eState { eReadConfigFile, eReConnectNow, eConnected, eWaitUntilTimeToReConnect, eError_NoRetry };

        static eState _State = eState.eReadConfigFile;
        static int _iPort;
        static string _sHost;
        static int _iRetry;
        static Socket _socket;
        static DateTime _dtNextRetry;

        public static void SetConfig(string Host, int Port)
        {
            _sHost = Host;
            _iPort = Port;
            _iRetry = 10;
        }

        static bool ReConnectNow()
        {
            bool bConnected = false;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(_sHost), _iPort);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#if (!WindowsCE)
            _socket.NoDelay = true;
#endif
            try
            {
                _socket.Connect(ipep);
                bConnected = true;
            } catch {}
            return bConnected;
        }

        static void SetWaitForReconnect()
        {
            // we could not connect or lost connection
            if (_iRetry == 0)
            {
                // no retry configured, so stop forever
                _State = eState.eError_NoRetry;
            }
            else
            {
                // set retry time and do nothing until then
                _dtNextRetry = DateTime.Now.AddSeconds(_iRetry);
                _State = eState.eWaitUntilTimeToReConnect;
            }
            try { _socket.Shutdown(SocketShutdown.Both); } catch { }
            try { _socket.Close(); } catch { }
        }

        static bool SendToHost(byte[] bData)
        {
            int iSent = 0;
            try
            {
                iSent = _socket.Send(bData);
            }
            catch {}
            return (iSent == bData.Length);
        }

        public static void Send(byte[] bIn)
        {
	        bool bExit = false;
	        try
	        {
		        while(!bExit)
		        {
			        switch(_State)
			        {
                        case eState.eReadConfigFile:
                            SetConfig("127.0.0.1", 3000);
                            _State = eState.eReConnectNow;
				        break;
                    case eState.eReConnectNow:
				        if(!ReConnectNow())
				        {
					        // we could not connect this time
					        SetWaitForReconnect();
					        bExit = true;
				        }
				        else
				        {
					        // we could connect, move to next state
                            _State = eState.eConnected;
				        }
				        break;
			        case eState.eConnected:
				        if(!SendToHost(bIn))
				        {
					        // we lost connection
					        SetWaitForReconnect();
					        bExit = true;
				        }
				        else
				        {
					        // we could send the data, nothing else to do
					        bExit = true;
				        }
				        break;
                    case eState.eWaitUntilTimeToReConnect:
                        if (DateTime.Now.Ticks >= _dtNextRetry.Ticks)
                        {
                            // it's time for a retry
                            _State = eState.eReConnectNow;
                        }
                        else
                        {
                            // too early for a retry
                            bExit = true;
                        }
                        break;
                    case eState.eError_NoRetry:
				        // No active logging
				        bExit = true;
				        break;
			        }
		        }
	        }
	        catch
	        {
		        // why did that happen?
		        _State = eState.eError_NoRetry;
	        }
        }
        public static void Send(string format, params object[] args)
        {
            try
            {
                if (_State != eState.eError_NoRetry)
                {
                    string sText = string.Format(format, args);
                    Send(Encoding.ASCII.GetBytes(sText));
                }
            }
            catch { }
        }

    }
}