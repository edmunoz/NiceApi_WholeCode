using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace NiceASP
{
    public class BitcoinPrice
    {
        public static BitcoinPriceInfos PriceFromGecko()
        {
            return JSON2Info(getJSONFromServer());
        }

        private static BitcoinPriceInfos JSON2Info(String j)
        {
            BitcoinPriceInfos ret = new BitcoinPriceInfos();

            XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(
                Encoding.UTF8.GetBytes(j), new System.Xml.XmlDictionaryReaderQuotas());
            XElement root = XElement.Load(jsonReader);
            foreach (XElement x1 in root.Nodes())
            {
                string strSymbol = x1.Element("symbol").Value;
                BitcoinPriceInfo update = null;
                if (strSymbol == "btc")
                {
                    ret.btc = update = new BitcoinPriceInfo();
                }
                else if (strSymbol == "bch")
                {
                    ret.bch = update = new BitcoinPriceInfo();
                }

                if (update != null)
                {
                    update.LastUpdate = x1.Element("last_updated").Value;
                    update.PriceInUSD = Decimal.Parse(x1.Element("market_data").Element("current_price").Element("usd").Value);
                    update.Symbol = strSymbol;
                }
            }

            root = null;
            return ret;
        }


        private static string getJSONFromServer()
        {
            try
            {
                string url = "https://api.coingecko.com/api/v3/coins";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                string strAllData = "";
                using (StreamReader streamIn = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    strAllData = streamIn.ReadToEnd();
                    return strAllData;
                }
            }
            catch (Exception se)
            {
            }
            return null;
        }

        public class BitcoinPriceInfo
        {
            public string Symbol;
            public string LastUpdate;
            public Decimal PriceInUSD;
        }
        public class BitcoinPriceInfos
        {
            public BitcoinPriceInfo btc;
            public BitcoinPriceInfo bch;
        }
    }
}
