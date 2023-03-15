using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VMSystem.Data.Model
{
    public class Product
    {
        [Key, StringLength(4)]
        public string ID { get; set; }
        public string Name { get; set; }
        public ICollection<TerminalStock> Stock { get; set; }
        public ICollection<TerminalStats> Stats { get; set; }
        public ICollection<ProductPrice> ProductPrice { get; set; }    // product's prices can change over time and are tracked in a separate table with additional Date atribute
    }
}
