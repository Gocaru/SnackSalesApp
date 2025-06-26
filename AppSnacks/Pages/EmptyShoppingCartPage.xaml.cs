namespace AppSnacks.Pages;

public partial class EmptyShoppingCartPage : ContentPage
{
	public EmptyShoppingCartPage()
	{
		InitializeComponent();
	}

    private async void BtnGoBack_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}