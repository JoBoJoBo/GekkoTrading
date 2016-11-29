using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GekkoTrading.Models
{
    public class GraphData
    {
        public DateTime TimeStamp { get; set; }
        public decimal CurrentProfit { get; set; }
        public decimal TransactionPrice { get; set; }
        public GraphData(DateTime date, decimal currentProfit, decimal transactionPrice)
        {
            TimeStamp = date;
            CurrentProfit = currentProfit;
            TransactionPrice = transactionPrice;
        }
    }
}
