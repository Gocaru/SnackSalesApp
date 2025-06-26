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

        bool saveAddress = Preferences.ContainsKey("address");

        if (saveAddress)
        {
            string name = Preferences.Get("name", string.Empty);
            string address = Preferences.Get("address", string.Empty);
            string phone = Preferences.Get("phone", string.Empty);

            LblAddress.Text = $"{name}\n{address} \n{phone}";
        }
        else
        {
            LblAddress.Text = "Enter your address";
        }

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



    private async void BtnDecrement_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem cartItem)
        {
            if (cartItem.Quantity == 1) return;
            else
            {
                cartItem.Quantity--;
                UpdateTotalPrice();
                await _apiService.UpdateCartItemQuantity(cartItem.ProductId, "decrease");
            }
        }

    }

    private async void BtnIncrement_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem cartItem)
        {
            cartItem.Quantity++;
            UpdateTotalPrice();
            await _apiService.UpdateCartItemQuantity(cartItem.ProductId, "increase");
        }

    }

    private async void BtnRemove_Clicked(object sender, EventArgs e)
    {
        if (sender is not ImageButton button || button.BindingContext is not ShoppingCartItem cartItem)
        {
            return;
        }

        bool reply = await DisplayAlert("Confirm",
                          "Are you sure you want to remove this item from the cart?", "Yes", "No");
        if (!reply)
        {
            return;
        }

        var result = await _apiService.UpdateCartItemQuantity(cartItem.ProductId, "remove");

        if (result.Data)
        {
            ShoppingCartItems.Remove(cartItem);
            UpdateTotalPrice();
        }
        else
        {
            await DisplayAlert("Error", result.ErrorMessage ?? "Could not remove the item. Please try again.", "OK");
        }
    }

    private void BtnEditAddress_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AddressPage());
    }

    private void TapConfirmOrder_Tapped(object sender, TappedEventArgs e)
    {

    }

  
}