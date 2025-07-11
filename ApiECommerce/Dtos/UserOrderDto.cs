using System.Text.Json.Serialization;

namespace ApiECommerce.Dtos
{
    public class UserOrderDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        [JsonPropertyName("orderDate")]
        public DateTime OrderDate { get; set; }
    }
}
