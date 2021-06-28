using System;
using System.ComponentModel;
using System.IO;
using Foundation;
using HybridWebView.iOS.Renderer;
using HybridWebView.Renderer;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(HybridWebViewRenderer))]
namespace HybridWebView.iOS.Renderer
{
    public class HybridWebViewRenderer : ViewRenderer<CustomWebView, WKWebView>, IWKScriptMessageHandler
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
        WKUserContentController userController;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Source")
            {
                if (Element.Source != null)
                {
                    string contentDirectoryPath = Path.Combine(NSBundle.MainBundle.BundlePath);
                    Control.LoadHtmlString(Element.Source, new NSUrl(contentDirectoryPath, true));
                }
            }
        }
        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("invokeAction");
                var hybridWebView = e.OldElement as CustomWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    userController = new WKUserContentController();
                    var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
                    userController.AddUserScript(script);
                    userController.AddScriptMessageHandler(this, "invokeAction");

                    var config = new WKWebViewConfiguration { UserContentController = userController };
                    var webView = new WKWebView(Frame, config);
                    SetNativeControl(webView);
                }
                if (Element.Source != null)
                {
                    string contentDirectoryPath = Path.Combine(NSBundle.MainBundle.BundlePath);
                    Control.LoadHtmlString(Element.Source, new NSUrl(contentDirectoryPath, true));
                }
            }
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            if (message.Body != null)
            {
                Element.InvokeAction(message.Body.ToString());
            }
        }
    }
}