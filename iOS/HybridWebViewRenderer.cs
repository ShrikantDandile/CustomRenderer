using System.ComponentModel;
using System.IO;
using CustomRenderer;
using CustomRenderer.iOS;
using Foundation;
using WebKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace CustomRenderer.iOS
{
    public class HybridWebViewRenderer : WkWebViewRenderer, IWKScriptMessageHandler
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
        WKUserContentController userController;

        public HybridWebViewRenderer() : this(new WKWebViewConfiguration())
        {
        }

        public HybridWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            userController = config.UserContentController;
            var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
            userController.AddUserScript(script);
            userController.AddScriptMessageHandler(this, "invokeAction");
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("invokeAction");
                HybridWebView hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }

            if (e.NewElement != null)
            {
                //string filename = Path.Combine(NSBundle.MainBundle.BundlePath, $"Content/{((HybridWebView)Element).Uri}");
               // LoadRequest(new NSUrlRequest(new NSUrl(filename, false)));
                HybridWebView hybridWebView = e.NewElement as HybridWebView;
                var idn = new System.Globalization.IdnMapping();
                var dotnetUri = new System.Uri(((HybridWebView)Element).Uri);
                _url = ((HybridWebView)Element).Uri;
                NSUrl nsUrl = new NSUrl(dotnetUri.Scheme, idn.GetAscii(dotnetUri.DnsSafeHost), dotnetUri.PathAndQuery);
                e.NewElement.PropertyChanged += OnElementPropertyChanged;
                LoadRequest(new NSUrlRequest(nsUrl));
            }
        }
        string _url;

        private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Preferences.ContainsKey("instanceId"))
            {
                var instId = Preferences.Get("instanceId", string.Empty);
                var deviceType = DeviceInfo.Platform.ToString();
                if (!string.IsNullOrEmpty(instId))
                {
                    _url = "http://i.digitalinfobytes.com/Login.html?isMobileApp=true&instanceid=" + instId + "&devicetype=" + deviceType;
                }
            }
            if (((HybridWebView)Element).Uri != _url)
            {
                ((HybridWebView)Element).Uri = _url;
                var idn = new System.Globalization.IdnMapping();
                var dotnetUri = new System.Uri(((HybridWebView)Element).Uri);
                NSUrl nsUrl = new NSUrl(dotnetUri.Scheme, idn.GetAscii(dotnetUri.DnsSafeHost), dotnetUri.PathAndQuery);
                LoadRequest(new NSUrlRequest(nsUrl));
            }
        }
        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            ((HybridWebView)Element).InvokeAction(message.Body.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((HybridWebView)Element).Cleanup();
            }
            base.Dispose(disposing);
        }
    }
}
