using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSystem.Data.Model;
using VMSystem.Data.Interfaces;
using VMSystem.Data.QueryLogic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Data.Entity;

namespace VMSystem.Data.Repositories
{
    internal class TerminalRepository :  ITerminalRepository
    {
        public event Action<IEnumerable<string>, IEnumerable<string>> UpdateColumnsHandler;
        public event Func<int> StockOptionCallback;

        private DataContext _context;

        public TerminalRepository(DataContext context)
        {
            _context = context;
        }

        public void Edit(Terminal terminal)
        {
            _context.Entry(terminal).State = EntityState.Modified;
        }

        public void Add(Terminal terminal)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            int stockOption = StockOptionCallback.Invoke();

            terminal.Cash = GetJsonData<TerminalCash>(assembly, "VMSystem.Data.Repositories.DeafultTerminalConfig.defaultcash.json");
            //1: fish pieces, 2: sandwiches. See TerminalStocksContainer for hardcoded stock options
            switch (stockOption)     
            {
                case 1:
                    terminal.Stock = GetJsonData<TerminalStock>(assembly, "VMSystem.Data.Repositories.DeafultTerminalConfig.fishstock.json");
                    break;
                case 2:
                    terminal.Stock = GetJsonData<TerminalStock>(assembly, "VMSystem.Data.Repositories.DeafultTerminalConfig.sandwichstock.json");
                    break;
                default:
                    throw new NotImplementedException();
            }

            _context.Terminals.Add(terminal);
        }

        public IEnumerable<Terminal> GetData()
        {
            UpdateColumnsHandler?.Invoke(new List<string> { "ID", "Terminal's Location"}, new List<string> { "ID", "Location"});
            return _context.Terminals.AsNoTracking();
        }

        private List<T> GetJsonData<T>(Assembly assembly, string resourceName) where T: class
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                    return JsonConvert.DeserializeObject<List<T>>(reader.ReadToEnd());
            }
        }

        public void Delete(Terminal product)
        {
            throw new NotImplementedException();
        }
    }
}
