﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GekkoTrading.Models
{
    public class ResultVM
    {
        public int MovingAverage1 { get; set; }

        public int MovingAverage2 { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int CandleDuration { get; set; }
        public decimal Result { get; set; }
        public int NrOfIntersections { get; set; }

        public ResultVM(MovingAverageVM viewModel, int movingAverage1, int movingAverage2, decimal result, int nrOfIntersections)
        {
            StartDate = viewModel.StartDate;
            EndDate = viewModel.EndDate;
            CandleDuration = viewModel.CandleDuration;
            MovingAverage1 = movingAverage1;
            MovingAverage2 = movingAverage2;
            Result = result;
            NrOfIntersections = nrOfIntersections;
        }
    }
}
