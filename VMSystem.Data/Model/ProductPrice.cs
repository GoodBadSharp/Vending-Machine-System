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
    public class ProductPrice
    {
        [Key, Column(Order = 0), JsonProperty("Date")]
        public DateTime DateIntroduced { get; set; }

        [Key, Column(Order = 1), ForeignKey("Product"), JsonProperty("Product_id")]
        public string ProductID { get; set; }

        public Product Product { get; set; }

        [JsonProperty("Selling_price")]
        public int SellingPrice { get; set; }

        [JsonProperty("Purchase_price")]
        public int PurchasePrice { get; set; }

    }
}
