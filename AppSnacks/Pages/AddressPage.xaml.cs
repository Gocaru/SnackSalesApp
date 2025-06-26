namespace AppSnacks.Pages;

public partial class AddressPage : ContentPage
{
	public AddressPage()
	{
		InitializeComponent();
	}


    protected override void OnAppearing()
    {
        base.OnAppearing();
        CarregarDadosSalvos();
    }


    private void CarregarDadosSalvos()  
    {
        if (Preferences.ContainsKey("name"))
            EntName.Text = Preferences.Get("name", string.Empty);

        if (Preferences.ContainsKey("address"))
            EntAddress.Text = Preferences.Get("address", string.Empty);

        if (Preferences.ContainsKey("phone"))
            EntPhone.Text = Preferences.Get("phone", string.Empty);
    }


    private void BtnSave_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("name", EntName.Text);
        Preferences.Set("address", EntAddress.Text);
        Preferences.Set("phone", EntPhone.Text);
        Navigation.PopAsync();
    }
}