using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSystem.Data.Model
{
    public class Credit
    {
        [Key]
        public int ID { get; set; }
        public int Denomination { get; set; }

        [JsonProperty("is_coin")]
        public bool IsCoin { get; set; }
        public ICollection<TerminalCash> Cash { get; set; }
    }
}

