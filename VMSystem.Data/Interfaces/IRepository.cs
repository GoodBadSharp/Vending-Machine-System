using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace VMSystem.Data.Interfaces
{
    public interface IRepository<T>
    {
        void Add(T item);

        void Edit(T item);

        IEnumerable<T> GetData();
    }
}
