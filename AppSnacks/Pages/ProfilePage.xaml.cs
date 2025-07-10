using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavouritesService _favouritesService;
    private bool _loginPageDisplayed = false;


    public ProfilePage(ApiService apiService, IValidator validator, FavouritesService favouritesService)
	{
		InitializeComponent();
        LblUserName.Text = Preferences.Get("username", string.Empty);
        _apiService = apiService;
        _validator = validator;
        _favouritesService = favouritesService;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ImgBtnProfile.Source = await GetProfileImage();
    }

    private async Task<string?> GetProfileImage()
    {
        // Obtenha a imagem padrão do AppConfig
        string standardImage = AppConfig.StandardProfileImage;

        var (response, errorMessage) = await _apiService.GetProfileUserImage();

        // Lida com casos de erro
        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    }
                    break;
                default:
                    await DisplayAlert("Error", errorMessage ?? "Could not retrieve the image.", "OK");
                    return standardImage;
            }
        }

        if (response?.ImageUrl is not null)
        {
            return response.ImagePath;
        }

        return standardImage;
    }


    private async void ImgBtnProfile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imageArray = await SelectImageAsync();
            if (imageArray is null)
            {
                await DisplayAlert("Error", "Could not retrieve the image", "Ok");
                return;
            }
            ImgBtnProfile.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

            var response = await _apiService.UploalUserImage(imageArray);
            if (response.Data)
            {
                await DisplayAlert("", "Image uploaded successfully", "Ok");
            }
            else
            {
                await DisplayAlert("Error", response.ErrorMessage ?? "An unknown error occurred", "Cancel");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unknown error occurred: {ex.Message}", "Ok");
        }

    }

    private async Task<byte[]> SelectImageAsync()
    {
        try
        {
            var archive = await MediaPicker.PickPhotoAsync();

            if (archive is null) return null;

            using (var stream = await archive.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Error", "This feature is not supported on the device", "Ok");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Error", "Permissions not granted to access the camera or gallery", "Ok");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error selecting the image: {ex.Message}", "Ok");
        }
        return null;

    }

    private void TapRequests_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new OrdersPage(_apiService, _validator, _favouritesService));
    }

    private void MyAccount_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new MyAccountPage(_apiService));
    }

    private void Questions_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new QuestionsPage());
    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("accesstoken", string.Empty);
        Application.Current!.MainPage = new NavigationPage(new LoginPage(_apiService, _validator, _favouritesService));
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favouritesService));
    }
}