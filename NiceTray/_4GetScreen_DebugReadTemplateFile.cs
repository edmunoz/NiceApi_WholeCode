using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using TextRecognitionLibrary;

namespace NiceTray
{
    public class _4GetScreen_DebugReadTemplateFile : I4_GetScreen
    {
        string rootPath;
        string processId;
        string tel;
        int retryCounter;

        public _4GetScreen_DebugReadTemplateFile(string rootPath)
        {
            this.rootPath = rootPath;
        }
        public void Debug_ResetRetryCounter()
        {
            retryCounter = 0;
        }
        public void Debug_SetId(int processId, string tel)
        {
            if (tel.StartsWith("zapi_+"))
            {
                tel = tel.Substring(6);
            }

            this.processId = processId.ToString();
            this.tel = tel;
        }

        public MyPicWithOriginalStream GetScreenShot(int screenId)
        {
            string serachPattern = String.Format("{0}_St{1}_zapi_{2}*",
                processId, screenId, tel);
            string storedDataPath = true ? rootPath : String.Format("{0}\\{1}", rootPath, processId);
            string[] files = Directory.GetFiles(storedDataPath, serachPattern);
            if (files.Length > 1)
            {
                // use retryCounter to find the right file
                retryCounter++;
                serachPattern = String.Format("{0}_St{1}_zapi_{2}_{3}.bmp",
                    processId, screenId, tel, retryCounter);
                files = Directory.GetFiles(storedDataPath, serachPattern);
            }
            return new MyPicWithOriginalStream(Bitmap.FromFile(files[0]));
        }
    }
}
