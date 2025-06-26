using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppSnacks.Models
{
    public class ShoppingCartItem : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get; set; }

        private int quantity;

        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged();    //notifica a Interface do Usuário (UI) que o valor de uma propriedade "quantidade" mudou, para que a UI possa atualizar-se automaticamente.
                }
            }
        }
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? UrlImage { get; set; }

        public string? ImagePath => AppConfig.BaseUrl + UrlImage;


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
