using AppSnacks.Pages;
using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private readonly FavouritesService _favouritesService;

        public AppShell(ApiService apiService, IValidator validator, FavouritesService favouritesService)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;
            _favouritesService = favouritesService;
            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService, _validator, _favouritesService);
            var shoppingCartPage = new ShoppingCartPage(_apiService, _validator, _favouritesService);
            var favouritesPage = new FavouritesPage(_apiService, _validator, _favouritesService);
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
