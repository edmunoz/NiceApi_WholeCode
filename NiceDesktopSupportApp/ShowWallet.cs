using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;

using NiceApiLibrary;
using NiceApiLibrary_low;

namespace NiceDesktopSupportApp
{
    class ShowWallet
    {
        public static void Go(IMyLog log, QuestionOption it)
        {
            DSSwitch.appWallet().RetrieveAll(
                delegate (Data_AppUserWallet w1)
                {
                    Console.WriteLine();
                    Console.WriteLine(w1.RequestedType.ToString());
                    Console.WriteLine(w1.Title);
                    Console.WriteLine(w1.Email);
                    foreach (var l1 in w1.DisplayLines)
                    {
                        Console.WriteLine(l1);
                    }
                    Console.WriteLine(w1.Setup.ToString("Setup"));
                    Console.WriteLine(w1.Messages.ToString("Messages"));
                    Console.WriteLine(w1.Month.ToString("Month"));
                    Console.WriteLine(w1.Numbers.ToString("Numbers"));

                    Console.WriteLine();
                    PriceAndText pt = w1.CalucateCost();
                    Console.WriteLine(pt.Explained);
                    Console.WriteLine(pt.FinalPrice);
                }, log);
        }
    }
}
