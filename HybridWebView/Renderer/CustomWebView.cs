using System;
using Xamarin.Forms;

namespace HybridWebView.Renderer
{
    public class CustomWebView : View
    {
        Action<string> action;

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            propertyName: "Source",
            returnType: typeof(string),
            declaringType: typeof(CustomWebView),
            defaultValue: default(string),
            propertyChanged: ItemsSourcePropertyChanged
            );
        private static void ItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomWebView)bindable;
            if (control != null)
            {
                control.Source = newValue.ToString();
            }
        }

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        public void InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return;
            }
            action.Invoke(data);
        }
    }
}
