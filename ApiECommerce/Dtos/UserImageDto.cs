using System.Text.Json.Serialization;

namespace ApiECommerce.Dtos
{
    public class UserImageDto
    {
        [JsonPropertyName("imageUrl")]
        public string? UrlImage { get; set; }
    }
}
