using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Configuration;
using System.Diagnostics;

namespace TextRecognitionLibrary
{
    class TextCourierNew11
    {
        private static bool s_Initialised = false;
        private static Dictionary<string,int> Dict;

        private static void doInitialisation_CourierNew11()
        {
            if (!s_Initialised)
            {
                s_Initialised = true;
                Dict = new Dictionary<string, int>();

                Debug.WriteLine("Config: " + ConfigurationManager.AppSettings.Count.ToString() + " elementes");

                foreach (string key in ConfigurationManager.AppSettings)
                {
                    if (key.StartsWith("__"))
                    {
                        string val = ConfigurationManager.AppSettings[key];
                        Dict.Add(key.Substring(2), Int32.Parse(val));
                    }
                }
            }
        }

        public static char RecogniseChar(MyPic pic)
        {
            pic.ToFileIfConfigured("TextRecognitionLibrary.TextCourierNew11.SaveInput");
            Size pSize = pic.getDimenion();
            if (pic.getDimenion().Height != 11)
            {
                throw new ArgumentException("TextCourierNew11");
            }
            doInitialisation_CourierNew11();
            string unqIs2 = pic.ToUniqueString();

            if (Dict.ContainsKey(unqIs2))
            {
                char r = (char)Dict[unqIs2];
                if (r == 99)
                {
                    return '?';
                }
                r += '0';
                return r;
            }
            else
            {
                // not there, so dump the file so it can be added to the configration
                if (unqIs2.Length < 150)
                {
                    pic.ToFile(unqIs2);
                }
            }


            MyPic picBlack = pic.Threshold_AlmostBlack();


            string unqIs = picBlack.ToUniqueString();
            return '?';
        }
    }
}
