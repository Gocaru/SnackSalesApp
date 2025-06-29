using AppSnacks.Models;
using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class ProductDetailsPage : ContentPage
{
	private readonly ApiService _apiService;
	private readonly IValidator _validator;
    private readonly FavouritesService _favouritesService;
    private int _productId;
	private bool _loginPageDisplayed = false;
    private string? _imageUrl;

	public ProductDetailsPage(int productId, string productName, ApiService apiService, IValidator validator, FavouritesService favouritesService)
	{
		InitializeComponent();
		_apiService = apiService;
		_validator = validator;
        _favouritesService = favouritesService;
        _productId = productId;
        Title = productName ?? "Product detail";
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductDetails(_productId);
        UpdateFavouriteButton();
    }

    private async Task<Product?> GetProductDetails(int productId)
    {
        var (productDetail, errorMessage) = await _apiService.GetProductDetail(productId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        // Verificar se houve algum erro na obtenção das produtos
        if (productDetail == null)
        {
            // Lidar com o erro, exibir mensagem ou logar
            await DisplayAlert("Error", errorMessage ?? "Unable to retrieve the product.", "OK");
            return null;
        }

        if (productDetail != null)
        {
            // Atualizar as propriedades dos controles com os dados do produto
            ProductImage.Source = productDetail.ImagePath;
            LblProductName.Text = productDetail.Name;
            LblProductPrice.Text = productDetail.Price.ToString();
            LblProductDescription.Text = productDetail.Detail;
            LblTotal.Text = productDetail.Price.ToString();
            _imageUrl = productDetail.ImagePath;
        }
        else
        {
            await DisplayAlert("Error", errorMessage ?? "Unable to retrieve the product details.", "OK");
            return null;
        }
        return productDetail;

    }

    private async void ImageBtnFavourite_Clicked(object sender, EventArgs e)
    {
        try
        {
            var existFavourite = await _favouritesService.ReadAsync(_productId);
            if (existFavourite is not null)
            {
                await _favouritesService.DeleteAsync(existFavourite);
            }
            else
            {
                var produtoFavorito = new FavouriteProduct()
                {
                    ProductId = _productId,
                    IsFavourite = true,
                    Detail = LblProductDescription.Text,
                    Name = LblProductName.Text,
                    Price = Convert.ToDecimal(LblProductPrice.Text),
                    ImageUrl = _imageUrl
                };

                await _favouritesService.CreateAsync(produtoFavorito);
            }
            UpdateFavouriteButton();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occured: {ex.Message}", "OK");
        }
    }


    private async void UpdateFavouriteButton()
    {
        var existeFavorito = await
               _favouritesService.ReadAsync(_productId);

        if (existeFavorito is not null)
            ImageBtnFavourite.Source = "heartfill";
        else
            ImageBtnFavourite.Source = "heart";
    }


    private void BtnRemove_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) &&
           decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            // Decrementa a quantidade, e n o permite que seja menor que 1
            quantity = Math.Max(1, quantity - 1);
            LblQuantity.Text = quantity.ToString();

            // Calcula o pre o total
            var total = quantity * unitPrice;
            LblTotal.Text = total.ToString();
        }
        else
        {
            // Tratar caso as convers es falhem
            DisplayAlert("Error", "Invalid values", "OK");
        }

    }

    private void BtnAdd_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) &&
      decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            // Incrementa a quantidade
            quantity++;
            LblQuantity.Text = quantity.ToString();

            // Calcula o preço total
            var total = quantity * unitPrice;
            LblTotal.Text = total.ToString(); // Formata como moeda
        }
        else
        {
            // Tratar caso as conversões falhem
            DisplayAlert("Error", "Invalid values", "OK");
        }

    }

    private async void BtnIncludeInShoppingCart_Clicked(object sender, EventArgs e)
    {
        try
        {
            var shoppingCart = new ShoppingCart()
            {
                Quantity = Convert.ToInt32(LblQuantity.Text),
                UnitPrice = Convert.ToDecimal(LblProductPrice.Text),
                Total = Convert.ToDecimal(LblTotal.Text),
                ProductId = _productId,
                ClientId = Preferences.Get("usuarioid", 0)
            };
            var response = await _apiService.AddItemInCart(shoppingCart);
            if (response.Data)
            {
                await DisplayAlert("Success", "Item added to the cart!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", $"Failed to add item: {response.ErrorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }

    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favouritesService));
    }

    
}