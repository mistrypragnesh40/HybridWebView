using System;
using System.ComponentModel;
using Android.Content;
using Android.Service.Controls;
using Android.Webkit;
using HybridWebView.Droid.Renderer;
using HybridWebView.Renderer;
using Java.Interop;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(HybridWebViewRenderer))]
namespace HybridWebView.Droid.Renderer
{
    public class HybridWebViewRenderer : ViewRenderer<CustomWebView, Android.Webkit.WebView>
    {
        const string JavascriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";
        Context _context;
        public string BaseUrl = "file:///android_asset/";
        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Source")
            {
                Control.LoadDataWithBaseURL(BaseUrl, Element.Source, "text/html", "UTF-8", null);
            }
        }
        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as CustomWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var webView = new Android.Webkit.WebView(_context);
                    webView.Settings.JavaScriptEnabled = true;
                    webView.SetWebViewClient(new JavascriptWebViewClient($"javascript: {JavascriptFunction}"));
                    SetNativeControl(webView);
                }
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                Control.LoadDataWithBaseURL(BaseUrl, Element.Source, "text/html", "UTF-8", null);
            }
        }
    }


    /// <summary>
    /// JavascriptWebViewClient
    /// </summary>
    public class JavascriptWebViewClient : WebViewClient
    {
        string _javascript;

        public JavascriptWebViewClient(string javascript)
        {
            _javascript = javascript;
        }

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);
            view.EvaluateJavascript(_javascript, null);
        }
    }

    /// <summary>
    /// JsBridge Class
    /// </summary>
    public class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<HybridWebViewRenderer> hybridWebViewRenderer;

        public JSBridge(HybridWebViewRenderer hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<HybridWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            HybridWebViewRenderer hybridRenderer;

            if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
            {
                hybridRenderer.Element.InvokeAction(data);
            }
        }
    }
}
