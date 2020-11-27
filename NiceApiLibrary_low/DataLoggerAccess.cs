// DataLoggerAccess.cs
//
// The C# file for the DataLoggerAccess project, which gives general access to the DataLogger application.
/*

******************************************************************
This file has been tested only with the following project type:
- Visual Studio 2005, C# Console Application

For any C++ projects use DataLoggerAccess.h instead.
******************************************************************

To send a string use
    CSC.DataLoggerAccess.Send("--{0:d}-- Hallo World", 13);  // same syntax as string.Format
or for binary data
    CSC.DataLoggerAccess.Send(new byte[] { 0x00, 0xFF, 0x13 });

*/
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
        static string _sHost = string.Empty;
        static int _iRetry;
        static Socket _socket;
        static DateTime _dtNextRetry = DateTime.Now;

        static bool ReadConfigFile()
        {
            _iPort = 3000;
            _sHost = "127.0.0.1";
            _iRetry = 10;
            return true;
        }

        static bool ReConnectNow()
        {
            bool bConnected = false;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(_sHost), _iPort);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.NoDelay = true;
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
				        if(!ReadConfigFile())
				        {
					        // could not read config file, stop here for ever
                            _State = eState.eError_NoRetry;
					        bExit = true;
				        }
				        else
				        {
					        // read file was ok, move to next state
                            _State = eState.eReConnectNow;
				        }
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