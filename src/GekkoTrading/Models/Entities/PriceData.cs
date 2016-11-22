using System;
using System.Collections.Generic;

namespace GekkoTrading.Models.Entities
{
    public partial class PriceData
    {
        public long Id { get; set; }
        public int InstrumentId { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal PriceLow { get; set; }
        public decimal PriceHigh { get; set; }
        public decimal PriceAverage { get; set; }
        public string TimeUnit { get; set; }

        public virtual Instrument Instrument { get; set; }
    }
}
