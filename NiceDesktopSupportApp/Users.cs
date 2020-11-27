using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NiceDesktopSupportApp
{

    internal interface IUser
    {
        string getId();
        string getMobile();
        string getName();
    }
    internal class User : IUser
    {
        private string Id;
        private string Mobile;
        private string Name;
        public string getId() { return Id; }
        public string getMobile() { return Mobile; }
        public string getName() { return Name; }

        public override string ToString()
        {
            return $"{Name} {Mobile}";
        }

        private User(string nam, string id, string mob) { this.Id = id; this.Mobile = mob; this.Name = nam; }

        private static List<User> s_UserList = buildList();

        private static List<User> buildList()
        {
            string conf = ConfigurationManager.AppSettings["AppUser"]
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "");
            List<User> userList = new List<User>();
            try
            {
                foreach (string c1 in conf.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] part = c1.Split(new char[] { ',' });
                    if (part.Length != 3)
                    {
                        throw new ConfigurationException(c1);
                    }
                    userList.Add(new User(part[0], part[1], part[2]));
                }
            }
            catch { }
            return userList;
        }

        public static IUser selectUser()
        {
            for (int i = 0; i < s_UserList.Count; i++)
            {
                Console.WriteLine(String.Format("{0} {1}", i, s_UserList[i].getName()));
            }
            return (s_UserList[Int32.Parse(Console.ReadLine())]);
        }
    }
}
