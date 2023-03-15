﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace VMSystem.Data.Model
{
    public class TerminalStock
    {
        [Key, Column(Order = 0), ForeignKey("Terminal"), JsonProperty("Terminal_id")]
        public int TerminalID { get; set; }

        [Key, Column(Order = 1), ForeignKey("Product"), JsonProperty("Product_id")]
        public string ProductID { get; set; }

        public Terminal Terminal { get; set; }

        public Product Product { get; set; }

        [JsonProperty("Product_quantity")]
        public int ProductQuantity { get; set; }
    }
}
