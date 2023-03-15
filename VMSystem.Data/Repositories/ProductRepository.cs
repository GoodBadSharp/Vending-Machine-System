using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSystem.Data.Interfaces;
using VMSystem.Data.Model;

namespace VMSystem.Data.Repositories
{
    internal class ProductRepository : IProductRepository
    {
        public event Action<IEnumerable<string>, IEnumerable<string>> UpdateColumnsHandler;

        private DataContext _context;

        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        public void Add(Product product)
        {
            if (_context.Products.FirstOrDefault(p => p.ID == product.ID) == null)
                _context.Products.Add(product);
            else
                throw new InvalidOperationException("ID is not unique");
        }

        public void Edit(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;

            foreach (var priceLine in product.ProductPrice)
            {
                if (_context.ProductPrices.FirstOrDefault(pp => pp.ProductID == priceLine.ProductID && pp.DateIntroduced == priceLine.DateIntroduced) == null)
                {
                    _context.Entry(priceLine).State = EntityState.Added;
                }
                else
                {
                    _context.Entry(priceLine).State = EntityState.Modified;
                }
            }
        }

        public void Delete(Product product)
        {
            if (_context.TerminalStocks.FirstOrDefault(ts => ts.ProductID == product.ID) == null)
                _context.Entry(product).State = EntityState.Deleted;
            else
                throw new FieldAccessException(product.ID);
        }

        public IEnumerable<Product> GetData()
        {
            UpdateColumnsHandler?.Invoke(new List<string> { "ID", "Product's Name" }, new List<string> { "ID", "Name" });
            return _context.Products.AsNoTracking();
        }

        public IEnumerable<FullProduct> GetExtendedData()
        {
            try
            {
                UpdateColumnsHandler?.Invoke(new List<string> { "ID", "Product's Name", "Price", "Cost" }, new List<string> { "ID", "Name", "Price", "Cost" });

                var productItems = _context.Products
                    .Select(p => new
                    {
                        ProductID = p.ID,
                        ProductName = p.Name,
                        MostRecentPrices = p.ProductPrice.OrderByDescending(pp => pp.DateIntroduced).FirstOrDefault(pp => pp.DateIntroduced <= DateTime.Today) //picking the most recent related ProductPrice entry
                })
                    .Select(a => new FullProduct
                    {
                        ID = a.ProductID,
                        Name = a.ProductName,
                        Price = a.MostRecentPrices.SellingPrice,
                        Cost = a.MostRecentPrices.PurchasePrice
                    }).AsNoTracking();
                return productItems;
            }
            catch { throw new InvalidOperationException("Failed to load products. Data may be invomplete or corrupt."); }
        }
    }
}
