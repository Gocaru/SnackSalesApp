using SQLite;

namespace AppSnacks.Models
{
    public class FavouriteProduct
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Detail { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsFavourite { get; set; }
    }
}
