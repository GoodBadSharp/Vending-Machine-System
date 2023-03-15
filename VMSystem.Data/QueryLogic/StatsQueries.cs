using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using VMSystem.Data.Model;
using System.Data.Entity;

namespace VMSystem.Data.QueryLogic
{
    public class StatsQueries
    {
        public Action<IEnumerable<string>, IEnumerable<string>> UpdateColumnsHandler;
        public Action<IEnumerable<TerminalReport>> TerminalReportHandler;
        public Action<IEnumerable<ProductReport>> ProductReportHandler;
        public Func<int> GetSpecifiedYearCallback;
        public Func<int> GetSpecifiedMonthCallback;

        protected DataContext _context;

        public StatsQueries(DataContext context)
        {
            _context = context;
        }

        public void ConductQuery(int reportID)
        {
            // see QueryLogic.ReportContainer for hardcoded reports' descriptions and IDs
            switch (reportID)
            {
                case 1:
                    UpdateColumnsHandler?.Invoke(new List<string> { "ID", "Terminal's Location", "Products out of stock" }, new List<string> { "TerminalID", "Location", "ReportDetails" });
                    OutOfStock();
                    break;
                case 2:
                    UpdateColumnsHandler?.Invoke(new List<string> { "ID", "Product Name", "Sold" }, new List<string> { "ProductID", "Name", "ReportDetails" });
                    LeastSold();
                    break;
                case 3:
                    UpdateColumnsHandler?.Invoke(new List<string> { "ID", "Terminal's Location", "Prodit Made" }, new List<string> { "TerminalID", "Location", "ReportDetails" });
                    TerminalsByProfit();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        void OutOfStock()
        {
            var reportArray = _context.TerminalStocks
                .Where(ts => ts.ProductQuantity == 0)
                .GroupBy(t => t.TerminalID)
                .Select(tg => new
                {
                    TerminalID = tg.Key,
                    Location = _context.Terminals.FirstOrDefault(t => t.ID == tg.Key).Location,
                    ReportDetails = tg.Select(g => g.ProductID),
                    TotalStock = _context.TerminalStocks.Where(t => t.TerminalID == tg.Key).Sum(g => g.ProductQuantity)
                })
                .OrderBy(aObj => aObj.TotalStock)
                //prevent Aggregate() below from being called in projection 
                .AsEnumerable() 
                .Select(tgNew => new TerminalReport
                 {
                     TerminalID = tgNew.TerminalID,
                     Location = tgNew.Location,
                     ReportDetails = tgNew.ReportDetails.Aggregate((s1, s2) => s1 + ", " + s2)
                 });

            TerminalReportHandler?.Invoke(reportArray);
        }


        void LeastSold()
        {
            int specifiedMonth;
            int specifiedYear;

            try
            {
                specifiedMonth = GetSpecifiedMonthCallback.Invoke();
                specifiedYear = GetSpecifiedYearCallback.Invoke();
            }
            catch { throw new NullReferenceException(); }
            
            var reportArray = _context.Products
                .Select(p => new
                {
                    ProductID = p.ID,
                    Name = p.Name,
                    ReportDetails = p.Stats
                        .Where(s => s.PurchaseDate.Month == specifiedMonth && s.PurchaseDate.Year == specifiedYear)
                        .Sum(s => s.QuantitySold)
                })
                .OrderBy(anObj => anObj.ReportDetails).ThenBy(anObj => anObj.ProductID)
                .Take(5)
                .Select(r => new ProductReport
                {
                    ProductID = r.ProductID,
                    Name = r.Name,
                    ReportDetails = r.ReportDetails.ToString() == String.Empty ? "0" : r.ReportDetails.ToString()
                });

            ProductReportHandler?.Invoke(reportArray);
        }


        void TerminalsByProfit()
        {
            int specifiedMonth;
            int specifiedYear;

            try
            {
                specifiedMonth = GetSpecifiedMonthCallback.Invoke();
                specifiedYear = GetSpecifiedYearCallback.Invoke();
            }
            catch { throw new NullReferenceException(); }

            var reportArray = _context.Terminals
                .Select(t => new
                {
                    TerminalID = t.ID,
                    Location = t.Location,
                    TotalProfit = t.Stats
                        .Where(ts => ts.PurchaseDate.Month == specifiedMonth && ts.PurchaseDate.Year == specifiedYear)
                        .Select(ts => new
                        {
                            ProductID = ts.ProductID,
                            Date = ts.PurchaseDate,
                            ProfitPerProductOnTheDay = ts.QuantitySold * _context.ProductPrices
                                .Where(pp => pp.ProductID == ts.ProductID && pp.DateIntroduced <= ts.PurchaseDate)
                                .Select(pp => new
                                {
                                    TimeDistance = DbFunctions.DiffDays(pp.DateIntroduced, ts.PurchaseDate) ?? 0,   //using conditional null operator for the sake of TimeDistance being non-nullable int
                                    ProfitPerProductPurchase = pp.SellingPrice - pp.PurchasePrice
                                })
                                .OrderBy(a => a.TimeDistance).Take(1).Sum(a => a.ProfitPerProductPurchase)      //<-- this had to replace more elegant looking query chain (IMO) because apparently Aggregate() is extremply picky with what it's fed (e.g. anonymous types):
                        })                                                                                      //.Aggregate((aObj1, aObj2) => aObj1.TimeDistance < aObj2.TimeDistance ? aObj1 : aObj2).ProfitPerProductPurchase
                        .Sum(a => a.ProfitPerProductOnTheDay)
                })
                .OrderByDescending(a => a.TotalProfit)
                .Select(a => new TerminalReport
                {
                    TerminalID = a.TerminalID,
                    Location = a.Location,
                    ReportDetails = a.TotalProfit.ToString() == String.Empty ? "0" : a.TotalProfit.ToString()
                });

            TerminalReportHandler?.Invoke(reportArray);
        }


    }
}