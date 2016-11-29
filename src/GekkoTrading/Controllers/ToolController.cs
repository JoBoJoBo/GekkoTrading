using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GekkoTrading.Models;
using GekkoTrading.Models.Entities;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GekkoTrading.Controllers
{
    public class ToolController : Controller
    {
        GekkoContext context;
        public ToolController(GekkoContext context)
        {
            this.context = context;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult MA()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MA(MovingAverageVM viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            else if(viewModel.MovingAverage1 >= viewModel.MovingAverage2)
            {
                ViewBag.MA2Error = "Moving average 2 must be greater than moving average 1";
                return View(viewModel);
            }
            else if(viewModel.StartDate.CompareTo(viewModel.EndDate) > 0)
            {
                ViewBag.DateError = "Does this look like a valid date-interval to you?!";
                return View(viewModel);
            }

            else return RedirectToAction(nameof(ToolController.Result), viewModel);

        }

        public async Task<IActionResult> Result(MovingAverageVM viewModel)
        {
            var result = await context.GetResult(viewModel);
            var bestResult = result.OrderByDescending(x => x.Result).ToList();
            var graphData = await context.GetGraphData(bestResult[0]);
            viewModel.Results = bestResult;
            viewModel.Graph = graphData;
            return View("MA", viewModel);
        }
    }
}
