using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiECommerce.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(200)]
        [Required]
        public string? UrlImage { get; set; }

        [JsonIgnore]
        public ICollection<Product>? Products { get; set; }
    }
}
