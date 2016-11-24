using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GekkoTrading.Models.Entities;

namespace GekkoTrading.Models.UserViewModels
{
    public class HomeVM
    {
        public string Username { get; set; }

        public List<Cases> UserHistory { get; set; }
    }
}
