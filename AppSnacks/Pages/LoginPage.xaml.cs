using AppSnacks.Validations;
using AppSnacks.Services;

namespace AppSnacks.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavouritesService _favouritesService;
    public LoginPage(ApiService apiService, IValidator validator, FavouritesService favouritesService)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favouritesService = favouritesService;
    }

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            await DisplayAlert("Error", "Inform your email", "Cancel");
            return;
        }

        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            await DisplayAlert("Error", "Inform your password", "Cancel");
            return;
        }

        var response = await _apiService.Login(EntEmail.Text, EntPassword.Text);

        if (!response.HasError)
        {
            Application.Current!.MainPage = new AppShell(_apiService, _validator, _favouritesService);
        }
        else
        {
            await DisplayAlert("Error", "Something went wrong!", "Cancel");
        }

    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {

        await Navigation.PushAsync(new RegistrationPage(_apiService, _validator, _favouritesService));

    }
}