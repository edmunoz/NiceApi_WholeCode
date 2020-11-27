using System;
using System.Collections.Generic;
using System.Text;

namespace NiceDesktopSupportApp
{
    public delegate void MyAction(NiceApiLibrary_low.IMyLog log, QuestionOption it);

    public class QuestionOption
    {
        public string OptionText;
        public MyAction Action;

        public QuestionOption(string optionText, MyAction action)
        {
            this.OptionText = optionText;
            this.Action = action;
        }
    }

    class Question
    {
        private List<QuestionOption> Options = new List<QuestionOption>();

        public bool HasQuestions
        {
            get
            {
                return (Options.Count > 0);
            }
        }
        public void Add(QuestionOption o)
        {
            Options.Add(o);
        }

        public void AskAndAct(string question, NiceApiLibrary_low.IMyLog log)
        {
            int r = -1;
            Console.WriteLine(question);
            int loop = 1;
            foreach (QuestionOption o1 in Options)
            {
                Console.WriteLine(String.Format("{0:00} {1}", loop++, o1.OptionText));
            }
            r = Int32.Parse(Console.ReadLine()) - 1;
            Options[r].Action(log, Options[r]);
        }

        public QuestionOption AskAndReturnOption(string question)
        {
            int r = -1;
            Console.WriteLine(question);
            int loop = 0;
            foreach (QuestionOption o1 in Options)
            {
                Console.WriteLine(String.Format("{0:00} {1}", loop++, o1.OptionText));
            }
            r = Int32.Parse(Console.ReadLine());
            return Options[r];
        }

        public static bool Ask(string prompt, string expectedAnswer)
        {
            Console.WriteLine(prompt);
            Console.WriteLine("(" + expectedAnswer + ")");
            string answer = Console.ReadLine();
            return answer == expectedAnswer;
        }

        public static string Ask(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }
    }
}
