using AppSnacks.Models;
using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class ProductsListPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _categoryId;
    private bool _loginPageDisplayed = false;

    public ProductsListPage(int categoryId, string categoryName,
                              ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _categoryId = categoryId;
        Title = categoryName ?? "Products";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductsList(_categoryId);
    }

    private async Task<IEnumerable<Product>> GetProductsList(int categoryId) 
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProductsByCategoryAsync(categoryId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products is null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not retrieve the categories.", "OK");
                return Enumerable.Empty<Product>();
            }

            CvProducts.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"An unexpected error has occurred: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }

    }


    private async Task DisplayLoginPage() 
    { 
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;

        if (currentSelection == null) return;

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.Id,
                                                    currentSelection.Name!,
                                                    _apiService,
                                                    _validator));
        ((CollectionView)sender).SelectedItem = null;
    }
}