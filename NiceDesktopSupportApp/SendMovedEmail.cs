using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net.Mail;

namespace NiceDesktopSupportApp
{
//    class SendMovedEmail
//    {
//        public static bool Go(ZapZapLibrary.IMyLog log)
//        {
//            bool bAll = true;

//            foreach (string s1 in Directory.GetFiles(ZapZapLibrary.FolderNames.GetFolder(ZapZapLibrary.MyFolders.ASP_UserAccountFolder)))
//            {
//                ZapZapLibrary.Data_AppUserFile u1 = ZapZapLibrary.Data_AppUserFile.ReadFromFile(Path.GetFileNameWithoutExtension(s1), log);

//                bool b1 = SendOne(u1);
//                if (!b1)
//                {
//                    bAll = false;
//                }
//                Console.WriteLine(u1.Email);
//            }
//            return bAll;
//        }

//        private static bool SendOne(ZapZapLibrary.Data_AppUserFile user)
//        {
//            try
//            {
//                string to = user.Email;
//                string subject = "We became too popular and had to move";

//                string htmlBodyNoHeader = get_htmlBodyNoHeader(user);
//                string error;
//                return ZapZapLibrary.EMail.Send(ZapZapLibrary.EMailCredentials.GetSupport(), to, subject, htmlBodyNoHeader, out error);
//            }
//            catch (SystemException)
//            {
//            }
//            return false;
//        }

//        private static string get_htmlBodyNoHeader(ZapZapLibrary.Data_AppUserFile user)
//        {
//            string str = get_htmlBodyNoHeader_noReplace()
//                .Replace("{NastyEmailRef}", "\"https://whatsappapi.net/NastyEmail.html\"")
//                .Replace("{Https__NiceApi_net}", "\"https://NiceApi.net/\"")
//                .Replace("{Email}", user.Email)
//                .Replace("{Password}", user.Password)
//                .Replace("{Whatsappapi_net}", "\"whatsappapi.net\"")
//                .Replace("{Niceapi_net}", "\"niceapi.net\"")
//                ;
//            if (-1 != str.IndexOf('{'))
//            {
//                Debug.WriteLine(str);
//                throw new NotImplementedException();
//            }
//            return str;
//        }

//        private static string get_htmlBodyNoHeader_noReplace()
//        {
//            return @"
//<p>We at WhatsAppAPI.net became too popular and had to move.</p>
//<br/>
//<p>WhatsApp Inc. believes that our domain name (whatsappapi.net ) is too close <br />
//to their registred trademark (WhatsApp) and are concerned that this could <br />
//cause <a href={NastyEmailRef}>confusion, mistake and deception</a> .</p>
//<br />
//<p>To avoid legal actions from WhatsApp Inc against us, we decided to host our popular service on another domain.</p>
//<br />
//With our move, we have also moved your registration to our new site.
//Your login details are:
//<table>
//  <tr>
//    <td>New location:</td>
//    <td><a href={Https__NiceApi_net}>NiceApi.net</a></td>
//  </tr>
//  <tr>
//    <td>User email:</td>
//    <td>{Email}</td>
//  </tr>
//  <tr>
//    <td>Password:</td>
//    <td>{Password}</td>
//  </tr>
//</table>
//<br />
//<p>
//<h2>What does this mean for you?</h2>
//It means that you need to change your application to send your HTTP POST request to the new host.<br />
//Typically you would only need to <u>replace {Whatsappapi_net} with {Niceapi_net}</u>.</p>
//<br />
//<p>
//<h2>Your action is needed!</h2>
//You need to change your application to send your HTTP POST request to the new host.<br />
//Typically you would only need to <u>replace {Whatsappapi_net} with {Niceapi_net}</u>.</p>
//<br />
//<p>We apologize for the inconvenience this may caused you.</p>
//<br />
//<p>Your NiceApi.net team.</p>
//";
//        }
//    }
}
