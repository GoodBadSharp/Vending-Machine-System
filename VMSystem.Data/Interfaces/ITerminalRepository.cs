using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSystem.Data.Model;

namespace VMSystem.Data.Interfaces
{
    public interface ITerminalRepository : IRepository<Terminal>
    {
        event Action<IEnumerable<string>, IEnumerable<string>> UpdateColumnsHandler;

        event Func<int> StockOptionCallback;
    }
}
