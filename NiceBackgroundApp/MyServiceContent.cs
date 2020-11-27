using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace NiceBackgroundApp
{
    class MyServiceContent
    {
        StreamWriter w;
        StreamReader r;
        public MyServiceContent(Stream stream)
        {
            try
            {
                log("ctor");
                w = new StreamWriter(stream);
                r = new StreamReader(stream);
            }
            catch (Exception)
            {
            }
        }

        public void Handle()
        {
            bool leave = false;
            try
            {
                log("Handle");
                while (!leave)
                {
                    try
                    {
                        String read = r.ReadLine();
                        log("Read: " + read);
                        if (read.Equals("help"))
                        {
                            sockEndAndFlush("memory");  // used by NiceAPI
                            sockEndAndFlush("exit");    // used by NiceAPI
                            sockEndAndFlush("get");     // used by NiceAPI
                            sockEndAndFlush("set");     // used by NiceAPI

                            //sockEndAndFlush("ip");
                            //sockEndAndFlush("update");
                            sockEndAndFlush("version");
                            //sockEndAndFlush("v");
                            sockEndAndFlush("get10");
                            sockEndAndFlush("get1");
                            //sockEndAndFlush("kick1");
                            //sockEndAndFlush("kick2");
                        }
                        /////////////////////////////////////////////////////////////////////////////////////////////////////
                        else if (read.Equals("version"))
                        {
                            sockEndAndFlush("07.09.2020 6000 (sticky)");
                        }
                        else if (read.Equals("exit"))
                        {
                            sockEndAndFlush("bye");
                            leave = true;
                            w.BaseStream.Close();
                            w.Close();
                        }
                        else if (read.Equals("memory"))
                        {
                            /* was with the old app :
                            totalMemory: 64729088
                            freeMemory: 8084056
                            maxMemory: 201326592

                            welches zu folgender linie führte
                            100 % used. 100 / 100.total:61 free: 9 max: 192
                            */
                            ;
                            //final Runtime runtime = Runtime.getRuntime();
                            //sockEndAndFlush("totalMemory: " + String.valueOf(runtime.totalMemory()));
                            //sockEndAndFlush("freeMemory: " + String.valueOf(runtime.freeMemory()));
                            //sockEndAndFlush("maxMemory: " + String.valueOf(runtime.maxMemory()));
                            sockEndAndFlush("totalMemory: " + GC.GetTotalMemory(false).ToString());
                            sockEndAndFlush("freeMemory: " + "0");
                            sockEndAndFlush("maxMemory: " + "0");
                        }
                        else if (read.Equals("get"))
                        {
                            MyContactsHelper.GetAllContacts(-1, w);
                            sockEndAndFlush("");
                        }
                        else if (read.Equals("get1"))
                        {
                            MyContactsHelper.GetAllContacts(1, w);
                            sockEndAndFlush("");
                        }
                        else if (read.Equals("get10"))
                        {
                            MyContactsHelper.GetAllContacts(10, w);
                            sockEndAndFlush("");
                        }
                        else if (read.StartsWith("set"))
                        {
                            string errorText = MyContactsHelper.InsertZap(read.Substring(3));
                            if (errorText == null)
                            {
                                sockEndAndFlush("set done");
                            }
                            else
                            {
                                sockEndAndFlush(errorText);
                                sockEndAndFlush("set FAILED");
                            }
                        }
                        /////////////////////////////////////////////////////////////////////////////////////////////////////
                        else
                        {
                            sockEndAndFlush("???");
                            log("Unknown");
                        }
                    }
                    catch (Exception ex)
                    {
                        log(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                log(ex.ToString());
            }
        }

        private void sockEndAndFlush(String endText)
        {
            this.w.Write(endText + "\r\n");
            this.w.Flush();
        }

        private void log(string text)
        {
            Log.Debug("MyServiceContent", text);
        }
    }
}