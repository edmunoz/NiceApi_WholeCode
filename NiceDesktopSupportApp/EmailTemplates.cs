using NiceApiLibrary_low;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceDesktopSupportApp
{
    class EmailTemplates
    {
        public static void Go(IMyLog log, QuestionOption it)
        {
            List<One> d = Fill();
            string startWith = "";
            int depth = 1;
            while (true)
            {
                Question q = new Question();
                List<One> subList = d.Where(x => x.depth == depth && x.Id.StartsWith(startWith)).ToList();
                foreach (var c in subList)
                {
                        q.Add(new QuestionOption(c.Title, null));
                }
                QuestionOption u = q.AskAndReturnOption("Select");

                One selected = d.FirstOrDefault(x => x.Title == u.OptionText);
                if (selected.Text != null)
                {
                    // end
                    Console.Clear();
                    System.Windows.Forms.Clipboard.SetText(selected.Text);
                    Console.WriteLine(selected.Text);
                    Console.WriteLine();
                    Console.WriteLine("now in Clipboard");
                    Console.ReadKey();
                    return;
                }
                depth++;
                startWith = selected.Id + "_";


            }
        }

        internal static List<One> Fill()
        {
            throw new NotImplementedException("not suitable for publication!");
        }
    }

    internal class One
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }

        public int depth
        {
            get
            {
                return Id.Split('_').Length;
            }
        }

    }
}
