using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GekkoTrading.Models
{
    public class ResultVM
    {
        public List<MovingAverageVM> Results { get; set; }
        public ResultVM()
        {
        }
        public ResultVM(List<MovingAverageVM> results)
        {
            Results = results;
        }
    }
}
