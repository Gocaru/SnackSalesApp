using AppSnacks.Models;
using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class FavouritesPage : ContentPage
{

    private readonly FavouritesService _favouritesService;
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public FavouritesPage(ApiService apiService, IValidator validator, FavouritesService favouritesService)
	{
		InitializeComponent();
        _favouritesService = favouritesService;
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProdutosFavoritos();
    }

    private async Task GetProdutosFavoritos()
    {
        try
        {
            var favouritesProducts = await _favouritesService.ReadAllAsync();

            if (favouritesProducts is null || favouritesProducts.Count == 0)
            {
                CvProducts.ItemsSource = null;//limpa a lista atual
                LblWarning.IsVisible = true; //mostra o aviso
            }
            else
            {
                CvProducts.ItemsSource = favouritesProducts;
                LblWarning.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected erros occured: {ex.Message}", "OK");
        }
    }



    private void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as FavouriteProduct;

        if (currentSelection == null) return;

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.ProductId,
                                                     currentSelection.Name!,
                                                     _apiService, _validator, _favouritesService));

        ((CollectionView)sender).SelectedItem = null;
    }


}