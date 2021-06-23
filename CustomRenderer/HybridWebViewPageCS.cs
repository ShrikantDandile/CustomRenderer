using System;
using Xamarin.Forms;

namespace CustomRenderer
{
    public class HybridWebViewPageCS : ContentPage
    {
        public HybridWebViewPageCS()
        {
            var hybridWebView = new HybridWebView
            {
                Uri = "http://i.digitalinfobytes.com:27824/test.html?isMobileApp=true&devicetype=ios"
            };

            hybridWebView.RegisterAction(data => CallInAppPurchase(data));

            Padding = new Thickness(0, 40, 0, 0);
            Content = hybridWebView;
        }

        private void CallInAppPurchase(string data)
        {
            DisplayAlert("Alert", "Hello " + data, "OK");
        }
    }
}
