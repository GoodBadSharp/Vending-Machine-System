using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace VMSystem.Data.Model
{
    public class TerminalCash
    {
        [Key, Column(Order = 0), ForeignKey("Terminal"), JsonProperty("Terminal_id")]
        public int TerminalID { get; set; }

        [Key, Column(Order = 1), ForeignKey("Credit"), JsonProperty("Credit_id")]
        public int CreditID { get; set; }

        public Terminal Terminal { get; set; }

        public Credit Credit { get; set; }

        [JsonProperty("Credit_quantity")]
        public int CreditQuantity { get; set; }
    }
}
