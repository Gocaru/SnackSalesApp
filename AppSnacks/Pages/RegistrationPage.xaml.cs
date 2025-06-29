using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class RegistrationPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavouritesService _favouritesService;

    public RegistrationPage(ApiService apiService, IValidator validator, FavouritesService favouritesService)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favouritesService = favouritesService;
    }

    private async void BtnSignup_ClickedAsync(object sender, EventArgs e)
    {
        if (await _validator.Validate(EntName.Text, EntEmail.Text, EntPhone.Text, EntPassword.Text))
        {
            var response = await _apiService.RegisterUserAsync(EntName.Text, EntEmail.Text,
                                                           EntPhone.Text, EntPassword.Text);


            if (!response.HasError)
            {
                await DisplayAlert("Notice", "Your account has been successfully created!", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favouritesService));
            }
            else
            {
                await DisplayAlert("Error", "Something went wrong!!!", "Cancel");
            }
        }
        else
        {
            string errorMessage = "";
            errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
            errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            errorMessage += _validator.PhoneError != null ? $"\n- {_validator.PhoneError}" : "";
            errorMessage += _validator.PasswordError != null ? $"\n- {_validator.PasswordError}" : "";

            await DisplayAlert("Error", errorMessage, "OK");
        }

    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favouritesService));
    }
}