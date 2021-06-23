using System;
using Xamarin.Forms;

namespace CustomRenderer
{
    public partial class HybridWebViewPage : ContentPage
    {
        public HybridWebViewPage()
        {
            InitializeComponent();

            hybridWebView.RegisterAction(data => InAppPurchase(data));
        }

        private void InAppPurchase(string data)
        {
            DisplayAlert("Alert", "Hello From Page" + data, "OK");
        }
    }
}
