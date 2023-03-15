using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSystem.Data.Model
{
    public class Terminal
    {
        public int ID { get; set; }
        public string Location { get; set; }
        public ICollection<TerminalStock> Stock { get; set; }
        public ICollection<TerminalStats> Stats { get; set; }
        public ICollection<TerminalCash> Cash { get; set; }
    }
}
