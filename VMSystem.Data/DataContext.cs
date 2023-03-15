using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSystem.Data.Model;

namespace VMSystem.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Credit> Credits { get; set; }
        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<TerminalCash> TerminalCashes { get; set; }
        public DbSet<TerminalStats> TerminalStats { get; set; }
        public DbSet<TerminalStock> TerminalStocks { get; set; }

        public DataContext() : base("localsql") { }
    }
}
