using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppSnacks.Models
{
    public class ProfileImage
    {
        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }
        public string? ImagePath => AppConfig.BaseUrl + ImageUrl;

    }
}
