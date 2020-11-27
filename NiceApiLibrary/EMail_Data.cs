using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    public static class EMail_Data
    {
        public static string GetHeaderHtml()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            MemoryStream msHeaderImg = new MemoryStream();
            assembly.GetManifestResourceStream("NiceApiLibrary.Embedded.WholeHeader.png").CopyTo(msHeaderImg);
            string s64HeaderImg = Convert.ToBase64String(msHeaderImg.ToArray());

            string sHeaderHtml = new StreamReader(assembly.GetManifestResourceStream("NiceApiLibrary.Embedded.Header.html")).ReadToEnd();
            sHeaderHtml = sHeaderHtml.Replace("{Base64ImgHeader}", s64HeaderImg);
            return sHeaderHtml;
        }

        public static string GetFooterHtml()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string sFooterHtml = new StreamReader(assembly.GetManifestResourceStream("NiceApiLibrary.Embedded.Footer.html")).ReadToEnd();
            return sFooterHtml;
        }

        public static string GetRegistrationEmailBody(string activationLink)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();

            string sBody = new StreamReader(assembly.GetManifestResourceStream("NiceApiLibrary.Embedded.RegistrationEmialText.txt")).ReadToEnd();
            sBody = sBody.Replace("{ActivationLink}", activationLink);
            return sBody;
        }
    }
}
