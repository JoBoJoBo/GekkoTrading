using GekkoTrading.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GekkoTrading.Models
{
    public class MovingAverageVM
    {
        public List<Instrument> Instruments { get; set; }

        public int MovingAverage1 { get; set; }
        public int MovingAverage2 { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CandeDuration { get; set; }

        public double Result { get; set; }
        public string Username { get; set; }


    }
}
