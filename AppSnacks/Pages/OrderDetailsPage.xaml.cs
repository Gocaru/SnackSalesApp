using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class OrderDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavouritesService _favouritesService;
    private bool _loginPageDisplayed = false;

    public OrderDetailsPage(int orderId,
                              decimal total,
                              ApiService apiService,
                              IValidator validator, FavouritesService favouritesService)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favouritesService = favouritesService;
        LblTotal.Text = " €" + total;

        GetOrderDetail(orderId);

    }

    private async void GetOrderDetail(int orderId)
    {
        try
        {
            var (orderDetails, errorMessage) = await _apiService.GetOrderDetails(orderId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (orderDetails is null)
            {
                await DisplayAlert("Error", errorMessage ?? "Unable to retrieve order details.", "OK");
                return;
            }
            else
            {
                CvOrderDetails.ItemsSource = orderDetails;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "An error occurred while retrieving the details. Please try again later.", "OK");
        }

    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favouritesService));

    }
}