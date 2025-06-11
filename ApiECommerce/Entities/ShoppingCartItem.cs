using System.ComponentModel.DataAnnotations.Schema;

namespace ApiECommerce.Entities
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        public int ProductId { get; set; }
        public int ClientId { get; set; }
    }
}
