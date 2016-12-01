using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GekkoTrading.Models
{
    public class GekkoMAData
    {
        public GekkoMAData(DateTime TimeStamp, decimal CandleAveragePrice, decimal OpeningPrice, int instrumentID)
        {
            this.TimeStamp = TimeStamp;
            this.CandleAveragePrice = CandleAveragePrice;
            this.OpeningPrice = OpeningPrice;
            this.InstrumentID = instrumentID;
        }

        public DateTime TimeStamp { get; set; }
        public decimal CandleAveragePrice { get; set; }
        public decimal OpeningPrice { get; set; }
        public decimal MA1Average { get; set; }
        public decimal MA2Average { get; set; }
        public decimal MADifference { get; set; }
        public bool IsIntersection { get; set; }

        public int InstrumentID { get; set; }
    }
}
