using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSystem.Data.Interfaces;
using VMSystem.Data.QueryLogic;
using VMSystem.Data.Repositories;

namespace VMSystem.Data
{
    public class UnitOfWork : IDisposable
    {
        DataContext _context = new DataContext();
        StatsQueries _statsQueries;

        public ITerminalRepository Terminals { get; }
        public IProductRepository Products { get; }
        public StatsQueries StatsQueries { get { return _statsQueries; } }

        public UnitOfWork()
        {
            _statsQueries = new StatsQueries(_context);
            Terminals = new TerminalRepository(_context);
            Products = new ProductRepository(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
