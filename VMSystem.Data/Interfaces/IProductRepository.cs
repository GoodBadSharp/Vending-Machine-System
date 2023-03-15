using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSystem.Data.Model;
using VMSystem.Data.Repositories;

namespace VMSystem.Data.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        event Action<IEnumerable<string>, IEnumerable<string>> UpdateColumnsHandler;

        void Delete(Product product);

        IEnumerable<FullProduct> GetExtendedData(); //returns product with numeric properties defining actual price and cost
                                                    //as opposed to standard class storing collection of price changes
    }
}
