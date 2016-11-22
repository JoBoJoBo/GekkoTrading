using System;
using System.Collections.Generic;

namespace GekkoTrading.Models.Entities
{
    public partial class Cases
    {
        public int Id { get; set; }
        public DateTime CaseDate { get; set; }
        public int InstrumentId { get; set; }
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BestMaSize1 { get; set; }
        public int BestMaSize2 { get; set; }
        public int CandleDuration { get; set; }
        public double Result { get; set; }

        public virtual Instrument Instrument { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
