using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GekkoTrading.Models.Entities;

namespace GekkoTrading.Controllers
{
    public class HomeController : Controller
    {
        GekkoContext context;
        public HomeController(GekkoContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Home));
            }
            else
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Home()
        {
            string username = User.Identity.Name;
            var viewModel = await context.GetUserHomeVM(username);

            return View(viewModel);
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Authorize]
        public IActionResult TheTool()
        {
            ViewData["Message"] = "The tool.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }
    }
}
