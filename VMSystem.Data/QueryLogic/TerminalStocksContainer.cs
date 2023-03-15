using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSystem.Data.QueryLogic
{
    public class TerminalStocksContainer
    {
        private IEnumerable<ComboBoxItemTemplate> _stocks;

        public IEnumerable<ComboBoxItemTemplate> Stocks { get { return _stocks; } }

        public TerminalStocksContainer()
        {
            _stocks = new List<ComboBoxItemTemplate>
            {
                new ComboBoxItemTemplate { Value = 1, Description = "Fish feast"},
                new ComboBoxItemTemplate { Value = 2, Description = "GoodOl Sandwiches"},
            };
        }
    }
}
