using System.Text.Json.Serialization;

namespace ApiECommerce.Dtos
{
    public class OrderDetailDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("subTotal")]
        public decimal SubTotal { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("productImage")]
        public string ProductImage { get; set; }

        [JsonPropertyName("productPrice")]
        public decimal ProductPrice { get; set; }
    }
}
