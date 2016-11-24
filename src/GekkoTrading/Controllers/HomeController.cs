﻿using System;
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

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Home));
            }
            else
            return View();
        }

        public IActionResult Home()
        {
            string username = User.Identity.Name;
            var viewModel = GekkoContext.GetUserHomeVM(username);

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
