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
            var results = new List<MovingAverageVM>();

            //Hämta data baserat på viewmodel
            Data = PriceData.Where(x => x.Timestamp.CompareTo(viewModel.StartDate) >= 0
            && x.Timestamp.CompareTo(viewModel.EndDate.AddDays(1)) < 0)
            .Select(y => new GekkoMAData(y.Timestamp, y.PriceAverage, y.OpenPrice))
            .ToList();
            //Eventuellt begränsa pga performance

            Data = GetCandleAverage(Data, viewModel.CandeDuration);

            for (int ma1 = 1; ma1 < viewModel.MovingAverage1; ma1++)
            {
                for (int ma2 = ma1+1 ; ma2 < viewModel.MovingAverage2; ma2++)
                {
                    GetMADifference(ma1, ma2);

                    GetIntersections(ma2);

                    var result = InitiateTransactions();

                    results.Add( new MovingAverageVM(viewModel, ma1, ma2, result));
                }
            }
            return new ResultVM(results);
        }

        private decimal InitiateTransactions()
        {
            var intersections = Data.Where(x => x.IsIntersection).ToList();
            decimal BankRoll = 100;
            decimal lastOpeningPrice = 0;

            if (intersections.Count() > 2)
            {
                int counter = 0;

                for (int i = 0; i < Data.Count; i++)
                {
                    if (Data[i].IsIntersection)
                    {
                        //Köp!
                        if (counter % 2 == 0 && counter != 0)
                        {
                            BankRoll /= (Data[i + 1].OpeningPrice / lastOpeningPrice);
                        }
                        //Sälj!
                        else if (counter % 2 != 0)
                        {
                            BankRoll *= (Data[i + 1].OpeningPrice / lastOpeningPrice);
                        }
                        lastOpeningPrice = Data[i + 1].OpeningPrice;
                        counter++;
                    }
                }
            }
            return BankRoll;
        }

        private void GetIntersections(int value)
        {
            bool isNotFirst = false;

            for (int i = value; i < Data.Count; i++)
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








