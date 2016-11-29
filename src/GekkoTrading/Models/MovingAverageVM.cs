﻿using GekkoTrading.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GekkoTrading.Models
{
    public class MovingAverageVM
    {
        //[Required]
        public List<Instrument> Instruments { get; set; }

        [Range(1, 250)]
        [Required]
        public int MovingAverage1 { get; set; }

        [Range(1, 250)]
        [Required]
        public int MovingAverage2 { get; set; }

        [Range(typeof(DateTime), "2016-05-23", "2016-11-22", ErrorMessage = "Date is out of range")]
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        [Range(typeof(DateTime), "2016-05-23", "2016-11-22", ErrorMessage = "Date is out of range")]
        public DateTime EndDate { get; set; }
        
        [Required]
        public int CandleDuration { get; set; }
        public List<ResultVM> Results { get; set; }
        public string Username { get; set; }
        public GraphVM Graph { get; set; }

        public MovingAverageVM()
        {
            Results = new List<ResultVM>();
        }
    }
}
