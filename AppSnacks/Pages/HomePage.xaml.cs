using AppSnacks.Models;
using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavouritesService _favouritesService;
    private bool _loginPageDisplayed = false;
    private bool _isDataLoaded = false;

    public HomePage(ApiService apiService, IValidator validator, FavouritesService favouritesService)
    {
        InitializeComponent();
        LblUserName.Text = "Hi, " + Preferences.Get("username", string.Empty);
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
        _favouritesService = favouritesService;
        Title = AppConfig.homePageTitle;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_isDataLoaded)
        {
            await LoadDataAsync();
            _isDataLoaded = true;
        }

    }

    private async Task LoadDataAsync()
    {
        var categoriesTask = GetCategoriesList();
        var bestSellersTask = GetBestSellers();
        var popularsTask = GetPopulars();

        await Task.WhenAll(categoriesTask, bestSellersTask, popularsTask);

    }

    private async Task<IEnumerable<Category>> GetCategoriesList()
    {
        try
        {
            var (categories, errorMessage) = await _apiService.GetCategories();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Category>();
            }

            if (categories == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Unable to retrieve the categories.", "OK");
                return Enumerable.Empty<Category>();
            }

            CvCategories.ItemsSource = categories;
            return categories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            return Enumerable.Empty<Category>();
        }
    }

    private async Task<IEnumerable<Product>> GetBestSellers()
    {
        try
        {

            var (products, errorMessage) = await _apiService.GetBestSellingProductsAsync();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Unable to retrieve the products.", "OK");
                return Enumerable.Empty<Product>();
            }

            CvBestSellers.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task<IEnumerable<Product>> GetPopulars()
    {
        try
        {

            var (products, errorMessage) = await _apiService.GetPopularProductsAsync();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Unable to retrieve the products.", "OK");
                return Enumerable.Empty<Product>();
            }

            CvPopulars.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favouritesService));
    }

    private void CvCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Category;

        if (currentSelection == null) return;

        Navigation.PushAsync(new ProductsListPage(currentSelection.Id,
                                                    currentSelection.Name!,
                                                    _apiService,
                                                    _validator, 
                                                    _favouritesService));

        ((CollectionView)sender).SelectedItem = null;
    }

    private void CvBestSellers_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            NavigateToProductDetailsPage(collectionView, e);
        }
    }

    private void CvPopulars_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            NavigateToProductDetailsPage(collectionView, e);
        }
    }

    private void NavigateToProductDetailsPage(CollectionView collectionView, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;

        if (currentSelection == null)
            return;

        Navigation.PushAsync(new ProductDetailsPage(
                                 currentSelection.Id, currentSelection.Name!, _apiService, _validator, _favouritesService
        ));

        collectionView.SelectedItem = null;

    }
}
