using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forex_Assistant.BL
{
    public class AccountBL
    {
        public static string _MainPath = Application.StartupPath + "\\Accounts\\";
        public class Account
        {

         

            public string AccountNumber { get; set; }  
            public decimal Balance { get; set; }        
            public string Description { get; set; }     

      
            public List<Trade> Trades { get; set; } = new List<Trade>();
        }

     
        public class Trade
        {       
            public string PositionNumber { get; set; }

            public string TradePair { get; set; }
            public string TradeType { get; set; }

            public decimal TradeVolume { get; set; }
            public decimal EntryPrice { get; set; }  
            public decimal StopLoss { get; set; }      
            public decimal TakeProfit { get; set; }    
            public decimal PNL { get; set; }          
            public string Notes { get; set; }          
            public DateTime CreatedAt { get; set; } = DateTime.Now; 
        }

        public static void SaveAccounts(Account accounts,string _filePath)
        {
            try
            {
                _filePath = _MainPath + _filePath + ".json";

                var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch
            { }
        }



        public static Account LoadAccounts(string _filePath)
        {
            try
            {

                if (!File.Exists(_filePath))
                    return new Account();

                var json = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<Account>(json) ?? new Account();
            }
            catch
            {
                return null;
            }
        }

        public static int DeleteAccount(string accountId)
        {
            try
            {
                int _ret = 0;
                string filePath = _MainPath + accountId + ".json";

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _ret = 1;
                }
                else
                {
                    _ret = -1;
                }

                return _ret;
            }
            catch
            {
                return 0;
            }
        }




        public static decimal CalculateDailyDrawdownFromTrades(
            Account account,
            DateTime? asOf = null,
            decimal? initialBalanceOverride = null)
        {
            try
            {
                if (account == null) throw new ArgumentNullException(nameof(account));

                var now = (asOf ?? DateTime.Now);
                var today = now.Date;

                var trades = (account.Trades ?? new List<Trade>())
                             .OrderBy(t => t.CreatedAt).ToList();


                if (!trades.Any())
                    return 0m;


                var totalPnlUpToNow = trades.Where(t => t.CreatedAt <= now).Sum(t => t.PNL);


                decimal initialBalance;
                if (initialBalanceOverride.HasValue)
                {
                    initialBalance = initialBalanceOverride.Value;
                }
                else
                {

                    var currentBalanceReported = account.Balance;
                    initialBalance = currentBalanceReported - totalPnlUpToNow;
                }


                var firstTradeToday = trades.FirstOrDefault(t => t.CreatedAt.Date == today);


                if (firstTradeToday == null)
                    return 0m;


                var pnlBeforeFirstTradeToday = trades
                    .Where(t => t.CreatedAt < firstTradeToday.CreatedAt)
                    .Sum(t => t.PNL);

                var startOfDayBalance = initialBalance + pnlBeforeFirstTradeToday;


                var currentBalance = initialBalance + totalPnlUpToNow;

                if (startOfDayBalance == 0m)
                    return 0m;


                var dd = (startOfDayBalance - currentBalance) / startOfDayBalance * 100m;

                return dd;
            }
            catch
            {
                return 0;
            }
        }


    }
}
