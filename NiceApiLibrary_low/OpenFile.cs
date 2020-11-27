using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace NiceApiLibrary_low
{
    public static class OpenFile
    {
        public static Stream ForWrite(string path, IMyLog log)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Stream r = null;
            while (r == null)
            {
                try
                {
                    r = File.Create(path);
                }
                catch (IOException)
                {
                    if (watch.ElapsedMilliseconds > 5000)
                    {
                        log.Error(String.Format("ForWrite failed on {0}", path));
                        break;
                    }
                    Thread.Sleep(50);
                }
            }
            return r;
        }

        public static Stream ForRead(string path, bool doRetry, bool readAndWrite, IMyLog log)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Stream r = null;
            while (r == null)
            {
                try
                {
                    if (readAndWrite)
                    {
                        r = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
                    }
                    else
                    {
                        r = File.OpenRead(path);
                    }
                }
                catch (FileNotFoundException)
                {
                    if (doRetry)
                    {
                        if (watch.ElapsedMilliseconds > 5000)
                        {
                            log.Error(String.Format("ForRead FileNotFoundException looped for 5sec {0}", path));
                            break;
                        }
                        Thread.Sleep(500);
                    }
                    else
                    {
                        if (!path.Contains("admin_at_niceapi_dot_net"))
                        {
                            log.Error(String.Format("ForRead FileNotFoundException no retry {0}", path));
                        }
                        break;
                    }
                }
                catch (IOException ioe)
                {
                    if (doRetry)
                    {
                        if (watch.ElapsedMilliseconds > 5000)
                        {
                            log.Error(String.Format("ForRead {1} looped for 5sec {0}", path, ioe.ToString()));
                            break;
                        }
                        Thread.Sleep(500);
                    }
                    else
                    {
                        log.Error(String.Format("ForRead {1} no retry {0}", path, ioe.ToString()));
                        break;
                    }
                }
            }
            return r;
        }
    }
}
