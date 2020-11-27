using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NiceApiLibrary_low.Data_AppUserFile;

namespace NiceApiLibrary
{
    public class PriceLevelHandler
    {
        public class Entry
        {
            public int Level;
            public int MaxMessages;
            public niceMoney Cost;

            public override string ToString()
            {
                return $"L:{Level} M:{MaxMessages} C:{Cost.ValueInUsCent}";
            }
        }

        public List<Entry> Config;
        public bool Ok;
        public PriceLevelHandler(string config)
        {
            // "200/9.99|500/14.99|1000/19.99|2000/24.99|5000/29.99|10000/34.99"
            try
            {
                Config = new List<Entry>();
                string[] spCfg = config.Split('|');
                int level = 0;
                foreach (string lev in spCfg)
                {
                    level++;
                    string[] spLev = lev.Split('/');
                    if (spLev.Length != 2)
                    {
                        throw new ArgumentException("Bad format");
                    }
                    int msgCount = int.Parse(spLev[0]);
                    string[] spCent = spLev[1].Split('.');
                    if (spCent.Length != 2)
                    {
                        throw new ArgumentException("Bad format");
                    }
                    int cost = 100 * int.Parse(spCent[0]);
                    cost += int.Parse(spCent[1]);

                    Entry add = new Entry()
                    {
                        Level = level,
                        MaxMessages = msgCount,
                        Cost = new niceMoney(cost)
                };
                Config.Add(add);
            }
                Ok = Config.Count >= 1;
            }
            catch (Exception) { }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"ok:{Ok}");
            foreach (Entry e in Config)
            {
                sb.Append($"- {e}");
            }
            return sb.ToString();
        }
    }
}
