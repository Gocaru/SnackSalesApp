namespace AppSnacks.Pages;

public partial class ConfirmedOrderPage : ContentPage
{
	public ConfirmedOrderPage()
	{
		InitializeComponent();
	}

    private async void BtnGoBack_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}