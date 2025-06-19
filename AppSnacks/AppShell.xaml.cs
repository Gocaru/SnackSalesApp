using AppSnacks.Pages;
using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;

        public AppShell(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;
            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService, _validator);
            var shoppingCartPage = new ShoppingCartPage();
            var favouritesPage = new FavouritesPage();
            var profilePage = new ProfilePage();

            Items.Add(new TabBar
            {
                Items =
            {
                new ShellContent { Title = "Home", Icon = "home", Content = homePage },
                new ShellContent { Title = "ShoppingCartItems", Icon = "cart", Content = shoppingCartPage },
                new ShellContent { Title = "Favourites", Icon = "heart", Content = favouritesPage },
                new ShellContent { Title = "Profile", Icon = "profile", Content = profilePage }
            }
            });
        }
    }

}
