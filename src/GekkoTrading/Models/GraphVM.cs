using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoTrading.Models
{
    public class GraphVM
    {
        public List<GraphData> GraphPoints { get; set; }
        public int Instrument { get; set; }
        public GraphVM(int instrumentID)
        {
            GraphPoints = new List<GraphData>();
            Instrument = instrumentID;
        }
        public string PrintToCsv()
        {
            StringBuilder csv = new StringBuilder($"Timestamp, Profit\n");
            foreach (var item in GraphPoints)
            {
                csv.Append($"{item.TimeStamp},{(item.CurrentProfit - 1).ToDot()}\n");
            }
            return csv.ToString();
        }
    }
}
