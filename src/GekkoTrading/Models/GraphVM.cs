using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GekkoTrading.Models
{
    public class GraphVM
    {
        public List<GraphData> GraphPoints { get; set; }
        public GraphVM()
        {
            GraphPoints = new List<GraphData>();
        }
        public string PrintToCsv()
        {
            string csv = "Date,CP\\n";
            foreach (var item in GraphPoints)
            {
                csv += $"{item.TimeStamp},{(item.CurrentProfit*100).ToDot()}\\n";
            }
            return csv;
        }
    }
}
