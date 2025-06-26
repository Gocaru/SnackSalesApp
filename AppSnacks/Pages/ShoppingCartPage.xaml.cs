using AppSnacks.Models;
using AppSnacks.Services;
using AppSnacks.Validations;
using System.Collections.ObjectModel;

namespace AppSnacks.Pages;

public partial class ShoppingCartPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isNavigatingToEmptyCartPage = false;


    private ObservableCollection<ShoppingCartItem>
        ShoppingCartItems = new ObservableCollection<ShoppingCartItem>();

    public ShoppingCartPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetShoppingCartItems();
    }

    private async Task<bool> GetShoppingCartItems()
    {
        try
        {
            var userId = Preferences.Get("userid", 0);
            var (shoppingCartItems, errorMessage) = await
                     _apiService.GetShoppingCartItems(userId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                // Redirecionar para a pagina de login
                await DisplayLoginPage();
                return false;
            }

            if (shoppingCartItems == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not retrieve the items from the shopping cart.", "OK");
                return false;
            }

            ShoppingCartItems.Clear();
            foreach (var item in shoppingCartItems)
            {
                ShoppingCartItems.Add(item);
            }

            CvShoppingCart.ItemsSource = ShoppingCartItems;
            UpdateTotalPrice(); // Atualizar o preco total ap?s atualizar os itens do carrinho

            if (!ShoppingCartItems.Any())
            {
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            return false;
        }
    }



    private async Task UpdateTotalPrice()
    {
        try
        {
             var total = ShoppingCartItems.Sum(item => item.Price * item.Quantity);
                LblTotal.Text = total.ToString("C");
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"An error occurred while updating the total price: {ex.Message}", "OK");
        }

    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }



    private void BtnDecrement_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnIncrement_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnDelete_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnEditAddress_Clicked(object sender, EventArgs e)
    {

    }

    private void TapConfirmOrder_Tapped(object sender, TappedEventArgs e)
    {

    }
}