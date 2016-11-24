using GekkoTrading.Models.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GekkoTrading.Models.Entities
{
    public partial class GekkoContext
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GekkoContext(DbContextOptions<GekkoContext> options, UserManager<ApplicationUser> userManager) : base(options)
        {
            _userManager = userManager;
        }

        public async Task<HomeVM> GetUserHomeVM(string username)
        {
            var viewModel = new HomeVM();

            var tmpUser = await _userManager.FindByNameAsync(username);

            viewModel.Username = tmpUser.UserName;

            viewModel.UserHistory = Cases
                .Where(c => c.UserId == tmpUser.Id)
                .ToList();

            return viewModel;
        }
    }
}
