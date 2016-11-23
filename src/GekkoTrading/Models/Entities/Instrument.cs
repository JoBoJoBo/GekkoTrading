using System;
using System.Collections.Generic;

namespace GekkoTrading.Models.Entities
{
    public partial class Instrument
    {
        public Instrument()
        {
            Cases = new HashSet<Cases>();
            PriceData = new HashSet<PriceData>();
        }

        public int Id { get; set; }
        public string InstrumentName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Cases> Cases { get; set; }
        public virtual ICollection<PriceData> PriceData { get; set; }
    }
}
