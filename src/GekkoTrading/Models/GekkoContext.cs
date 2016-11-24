using GekkoTrading.Models.UserViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GekkoTrading.Models.Entities
{
    public partial class GekkoContext
    {
        public GekkoContext(DbContextOptions<GekkoContext> options) : base(options)
        {

        }

        public HomeVM GetUserHomeVM(string username)
        {
            var viewModel = new HomeVM();

            var tmpUser = AspNetUsers.FirstOrDefault(x => x.UserName == username);

            viewModel.Username = tmpUser.UserName;

            viewModel.UserHistory = this.Cases
                .Where(c => c.UserId == tmpUser.Id)
                .ToList();

            return viewModel;
        }
    }
}
