using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GekkoTrading.Models
{
    public static class Extentions
    {
        public static string ToDot(this decimal d)
        {
            return d.ToString().Replace(',', '.');
        }
    }
}
