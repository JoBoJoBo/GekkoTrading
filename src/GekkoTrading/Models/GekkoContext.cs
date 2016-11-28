using GekkoTrading.Models.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GekkoTrading.Models.Entities
{
    public partial class GekkoContext
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public List<GekkoMAData> Data { get; set; }
        public GekkoContext(DbContextOptions<GekkoContext> options, UserManager<ApplicationUser> userManager) : base(options)
        {
            _userManager = userManager;
            Data = new List<GekkoMAData>();
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

        public async Task<ResultVM> GetResult(MovingAverageVM viewModel)
        {

            //Hämta data baserat på viewmodel
            Data = PriceData.Where(x => x.Timestamp.CompareTo(viewModel.StartDate) >= 0
            && x.Timestamp.CompareTo(viewModel.EndDate.AddDays(1)) < 0)
            .Select(y => new GekkoMAData(y.Timestamp, y.PriceAverage, y.OpenPrice))
            .ToList();
            //Eventuellt begränsa pga performance

            Data = GetCandleAverage(Data, viewModel.CandeDuration);

            GetMADifference(viewModel.MovingAverage1, viewModel.MovingAverage2);

            GetIntersections();

            InitiateTransactions();

            return null;
        }

        private decimal InitiateTransactions()
        {
            var intersections = Data.Where(x => x.IsIntersection);
            decimal BankRoll = 0;

            if (intersections.Count() > 2)
            {
                int counter = 0;

                for (int i = 0; i < Data.Count; i++)
                {
                    //Köp!
                    if (Data[i].IsIntersection && i % 2 == 0)
                    {
                        if (i == 0)
                        {
                            BankRoll -= Data[i + 1].OpeningPrice;

                        }
                        else
                        {
                            BankRoll -= Data[i + 1].OpeningPrice * 2;
                        }
                    }
                    //Sälj!
                    else if (Data[i].IsIntersection && i % 2 != 0)
                    {
                        if (counter == intersections.Count() - 2)
                        {
                            BankRoll += Data[i + 1].OpeningPrice;
                        }
                        else
                        {
                            BankRoll += Data[i + 1].OpeningPrice * 2;
                        }
                    }
                    counter++;
                }
            }
            return BankRoll;
        }

        private void GetIntersections()
        {
            bool isNotFirst = false;

            for (int i = 1; i < Data.Count; i++)
            {
                if (Data[i - 1].MADifference > 0 && Data[i].MADifference < 0 && isNotFirst)
                {
                    Data[i].IsIntersection = true;
                }
                else if (Data[i - 1].MADifference < 0 && Data[i].MADifference > 0)
                {
                    Data[i].IsIntersection = true;
                    isNotFirst = true;
                }
            }


        }

        private List<GekkoMAData> GetCandleAverage(List<GekkoMAData> inData, int duration)
        {
            List<GekkoMAData> returnData = new List<GekkoMAData>();
            for (int i = 0; i < inData.Count - duration; i += duration)
            {
                decimal value = 0;
                for (int j = i; j < i + duration; j++)
                {
                    value += inData[j].CandleAveragePrice;
                }
                inData[i].CandleAveragePrice = value / duration;
                returnData.Add(inData[i]);

            }
            return returnData;
        }


        //Metod för att räkna ut CandlestickAverage

        public async void GetMADifference(int ma1, int ma2)
        {
            for (int i = ma2; i < Data.Count; i++)
            {
                var results = await Task.WhenAll(GetAverage(ma1, i), GetAverage(ma2, i));

                // Callback start
                Data[i].MA1Average = results[0];
                Data[i].MA2Average = results[1];
                Data[i].MADifference = results[0] - results[1];
                // Callback end
            }

            Data.RemoveRange(0, ma2);
        }

        private async Task<decimal> GetAverage(int ma, int i)
        {
            decimal movingAveragePrice = 0;

            for (int j = i - ma; j < i; j++)
            {
                movingAveragePrice += Data[j].CandleAveragePrice;
            }
            movingAveragePrice = movingAveragePrice / ma;

            return movingAveragePrice;
        }
    }
}








