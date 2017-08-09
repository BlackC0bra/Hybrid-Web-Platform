﻿﻿using Xamarin.Forms;

namespace XamlExample
{
	public partial class XamlExamplePage : ContentPage
	{
        const string Host = "https://mobile.bet.pt/";

		public XamlExamplePage()
		{
			InitializeComponent();

            WebView.SetCookie(Host, "is_native_app", "true");
		}


		protected override void OnAppearing()
		{
			base.OnAppearing();
            WebView.LoadPage(new System.Uri(Host));
		}
	}
}
