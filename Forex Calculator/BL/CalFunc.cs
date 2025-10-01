using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Forex_Assistant.BL
{
    public class CalFunc
    {


        public double Curr_Cal(string _Curr1,string _Curr2)
        {
            try
            {
                var url = "https://www.google.com";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                string result = "";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                result = result.Trim();

                return Convert.ToDouble(result);
            }
            catch
            {
                return 0;
            }
        }


    
        public class Result_CurrList
        {
            public int id { get; set; }
            public string text { get; set; }
            public string desc { get; set; }
        }

        public class Root_CurrList
        {
            public List<Result_CurrList> results { get; set; }
        }


        public Root_CurrList Curr_List()
        {
            try
            {
                var url = "https://www.google.com";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                string result = "";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                ///////////////////

                url = "https://www.google.com";

                string result2 = "";

                httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result2 = streamReader.ReadToEnd();
                }


                //////////////////////

                url = "https://www.google.com";

                string result3 = "";

                httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result3 = streamReader.ReadToEnd();
                }


                //////////////////////////

                Root_CurrList myDeserializedClass = JsonConvert.DeserializeObject<Root_CurrList>(result);
                Root_CurrList myDeserializedClass2 = JsonConvert.DeserializeObject<Root_CurrList>(result2);
                Root_CurrList myDeserializedClass3 = JsonConvert.DeserializeObject<Root_CurrList>(result3);

                myDeserializedClass.results.AddRange(myDeserializedClass2.results);
                myDeserializedClass.results.AddRange(myDeserializedClass3.results);



                return myDeserializedClass;
            }
            catch
            {
                return null;
            }
        }




        private const double StandardLotSize = 100000;




        public  string PosSize_Cal(
        string pair,
        double accountSize,
        double riskRatio,
        double stopLossPips,
        double tradeSizeInLots,
        string accountCurrency
        )
        {
            try
            {
                double onePip = pair.Contains("JPY") ? 0.01 : 0.0001;

                string quoteCurrency = pair.Split('/')[1];

                double exchangeRateQuoteToAccount = 1.0;
                if (quoteCurrency != accountCurrency)
                {
                    exchangeRateQuoteToAccount = Curr_Cal(quoteCurrency, accountCurrency);
                }


                double pipValuePerLot = (onePip * 100000 * tradeSizeInLots) * exchangeRateQuoteToAccount;


                double riskAmount = accountSize * (riskRatio / 100);


                double positionLots = riskAmount / (stopLossPips * pipValuePerLot);

                return riskAmount.ToString("#.##") + "#" + Convert.ToDecimal(positionLots).ToString("#.##");
            }
            catch
            {
                return null;
            }
        }



        public class ProfitResult
        {
            public double Pips { get; set; }
            public double Profit { get; set; }
            public string AccountCurrency { get; set; }
        }

        public ProfitResult ProfitCal(
string pair,
double tradeSizeInLots,
double openPrice,
double closePrice,
string direction, 
string accountCurrency 
)
        {
            try
            {

                string[] currencies = pair.Split('/');
                string baseCurrency = currencies[0];
                string quoteCurrency = currencies[1];


                double pipSize = pair.Contains("JPY") ? 0.01 : 0.0001;


                double pips;
                if (direction.ToLower() == "buy")
                    pips = (closePrice - openPrice) / pipSize;
                else
                    pips = (openPrice - closePrice) / pipSize;


                double pipValuePerLot = pipSize * StandardLotSize;


                double exchangeRateQuoteToAccount = 1.0;
                if (quoteCurrency != accountCurrency)
                {
                    exchangeRateQuoteToAccount = Curr_Cal(quoteCurrency, accountCurrency);
                    pipValuePerLot /= exchangeRateQuoteToAccount;
                }


                double pipValue = pipValuePerLot * tradeSizeInLots;


                double profit = pips * pipValue;

                return new ProfitResult
                {
                    Pips = pips,
                    Profit = profit,
                    AccountCurrency = accountCurrency
                };
            }
            catch
            {
                return null;
            }
        }




        public  double MarginCal(
string pair,
double tradeSizeInLots,
double leverage, 
string accountCurrency 
)
        {
            try
            {
                string[] currencies = pair.Split('/');
                string baseCurrency = currencies[0];
                string quoteCurrency = currencies[1];

                double _price = Curr_Cal(baseCurrency, quoteCurrency);

                double notionalValue = tradeSizeInLots * StandardLotSize * _price;


                double marginInQuote = notionalValue / leverage;

                double marginInAccountCurrency = marginInQuote;


                if (quoteCurrency != accountCurrency)
                {
                    double rate = Curr_Cal(quoteCurrency, accountCurrency);
                    marginInAccountCurrency = marginInQuote * rate;
                }

                return marginInAccountCurrency;
            }
            catch
            {
                return 0;
            }
        }



    }
}
