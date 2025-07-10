using AppSnacks.Services;

namespace AppSnacks.Pages;

public partial class MyAccountPage : ContentPage
{
    private readonly ApiService _apiService;

    private const string UsernameKey = "username";
    private const string UserEmailKey = "useremail";
    private const string UserPhoneKey = "userphone";

    public MyAccountPage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        UpdateUserInfo();
        ImgBtnProfile.Source = await GetProfileImageAsync();
    }

    private void UpdateUserInfo()
    {
        LblUsername.Text = Preferences.Get(UsernameKey, string.Empty);
        EntName.Text = LblUsername.Text;
        EntEmail.Text = Preferences.Get(UserEmailKey, string.Empty);
        EntPhone.Text = Preferences.Get(UserPhoneKey, string.Empty);
    }

    private async Task<string?> GetProfileImageAsync()
    {
        string standardImage = AppConfig.StandardProfileImage;

        var (response, errorMessage) = await _apiService.GetProfileUserImage();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Error", "Unauthorized", "OK");
                    return standardImage;
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


    private async void BtnSave_Clicked(object sender, EventArgs e)
    {
        // Salva as informações alteradas pelo usuário nas preferências
        Preferences.Set(UsernameKey, EntName.Text);
        Preferences.Set(UserEmailKey, EntEmail.Text);
        Preferences.Set(UserPhoneKey, EntPhone.Text);
        await DisplayAlert("Information Saved", "Your information has been successfully saved!", "OK");

    }

}