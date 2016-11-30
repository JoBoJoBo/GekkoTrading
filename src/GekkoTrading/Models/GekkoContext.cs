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

        public async Task<List<ResultVM>> GetResultAsync(MovingAverageVM viewModel)
        {
            var results = new List<ResultVM>();

            await SetupDataAsync(viewModel.StartDate, viewModel.EndDate, viewModel.CandleDuration);

            for (int ma1 = 1; ma1 <= viewModel.MovingAverage1; ma1++)
            {
                for (int ma2 = ma1 + 1; ma2 <= viewModel.MovingAverage2; ma2++)
                {
                    GetMADifference(ma1, ma2);

                    GetIntersections(ma2);

                    var result = InitiateTransactions();

                    var resultData = new ResultVM(viewModel, ma1, ma2, result, Data.Where(x => x.IsIntersection).Count());
                    results.Add(resultData);
                }
            }
            return results;
        }

        private async Task SetupDataAsync(DateTime startDate, DateTime endDate, int candleDuration)
        {

            //Hämta data baserat på viewmodel
            Data = await PriceData.Where(x => x.Timestamp.CompareTo(startDate) >= 0
            && x.Timestamp.CompareTo(endDate.AddDays(1)) < 0)
            .Select(y => new GekkoMAData(y.Timestamp, y.PriceAverage, y.OpenPrice))
            .ToListAsync();
            //Eventuellt begränsa pga performance

            if (candleDuration != 1)
            {
                Data = GetCandleAverage(Data, candleDuration);
            }
        }

        public async Task<GraphVM> GetGraphDataAsync(ResultVM resultVM)
        {
            await SetupDataAsync(resultVM.StartDate, resultVM.EndDate, resultVM.CandleDuration);

            GetMADifference(resultVM.MovingAverage1, resultVM.MovingAverage2);

            GetIntersections(resultVM.MovingAverage2);

            var results = GetAllTransactionData();

            return results;
        }

        private GraphVM GetAllTransactionData()
        {
            var returnData = new GraphVM();
            var intersections = Data.Where(x => x.IsIntersection).Count();
            decimal CurrentPercentage = 1;
            decimal previousTransactionPrice = 0;
            decimal initialPrice = 0;

            if (intersections > 2)
            {
                int counter = 0;

                for (int i = 0; i < Data.Count - 1; i++)
                {
                    if (Data[i].IsIntersection)
                    {
                        if (counter == 0)
                        {
                            initialPrice = Data[i + 1].OpeningPrice;
                        }
                        //Köp!
                        if (counter % 2 == 0)
                        {
                            previousTransactionPrice = Data[i + 1].OpeningPrice;
                            // Handla med negativt innehav?
                            //CurrentPercentage /= (Data[i + 1].OpeningPrice / lastOpeningPrice);
                        }
                        //Sälj!
                        else if (counter % 2 != 0)
                        {
                            CurrentPercentage *= (Data[i + 1].OpeningPrice / previousTransactionPrice);
                        }
                        returnData.GraphPoints.Add(new GraphData(Data[i].TimeStamp, CurrentPercentage, Data[i + 1].OpeningPrice));
                        counter++;
                    }
                }
            }
            return returnData;
        }

        private decimal InitiateTransactions()
        {
            var intersections = Data.Where(x => x.IsIntersection).Count();
            decimal CurrentPercentage = 1;
            decimal previousTransactionPrice = 0;
            decimal initialPrice = 0;

            if (intersections > 2)
            {
                int counter = 0;

                for (int i = 0; i < Data.Count - 1; i++)
                {
                    if (Data[i].IsIntersection)
                    {
                        if (counter == 0)
                        {
                            initialPrice = Data[i + 1].OpeningPrice;
                        }
                        //Köp!
                        if (counter % 2 == 0)
                        {
                            previousTransactionPrice = Data[i + 1].OpeningPrice;
                            // Handla med negativt innehav?
                            //CurrentPercentage /= (Data[i + 1].OpeningPrice / lastOpeningPrice);
                        }
                        //Sälj!
                        else if (counter % 2 != 0)
                        {
                            CurrentPercentage *= (Data[i + 1].OpeningPrice / previousTransactionPrice);
                        }
                        counter++;
                    }
                }
            }
            return CurrentPercentage;
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

        public void GetMADifference(int ma1, int ma2)
        {
            for (int i = ma2; i < Data.Count; i++)
            {
                //var results = await Task.WhenAll(GetAverage(ma1, i), GetAverage(ma2, i));

                // Callback start
                Data[i].MA1Average = GetAverage(ma1, i);
                Data[i].MA2Average = GetAverage(ma2, i);
                Data[i].MADifference = Data[i].MA1Average - Data[i].MA2Average;
                // Callback end
            }

            Data.RemoveRange(0, ma2);
        }

        private decimal GetAverage(int ma, int i)
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








