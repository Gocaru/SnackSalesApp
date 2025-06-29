using AppSnacks.Validations;
using AppSnacks.Pages;
using AppSnacks.Services;

namespace AppSnacks
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private readonly FavouritesService _favouritesService;

        public App(ApiService apiService, IValidator validator, FavouritesService favouritesService)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
            _favouritesService = favouritesService; 
            SetMainPage();
        }

        private void SetMainPage()
        {
            var accessToken = Preferences.Get("accessToken", string.Empty);

            if(string.IsNullOrEmpty(accessToken))
            {
                MainPage = new NavigationPage(new LoginPage(_apiService, _validator, _favouritesService));
                return;
            }

            MainPage = new AppShell(_apiService, _validator, _favouritesService);
        }
    }
}
