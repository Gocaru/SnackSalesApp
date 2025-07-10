using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppSnacks.Models
{
    public class OrderDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("subTotal")]
        public decimal SubTotal { get; set; }

        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("productImage")]
        public string? ProductImage { get; set; }

        public string ImagePath => AppConfig.BaseUrl + ProductImage;

        [JsonPropertyName("productPrice")]
        public decimal ProductPrice { get; set; }
    }
}
