using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HybridWebView
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();


            string jquery = @" <script src='https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js'></script>";
            string webView = @"<script src=""HtmlWebView.js""></script>";

            string html = $"<html><head>{jquery} {webView}</head>" +
                           "Html Link1 : <a href='http://www.google.com'>Click here</a> </br>" +
                           "Html Link2: <a href='https://xamarincodingtutorial.blogspot.com'>Xamarin Coding Tutorial</a> </html>";
            hybridWebView.Source = html;
           hybridWebView.RegisterAction(data => ShowAction(data));
        }

        public void ShowAction(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                try
                {
                    Browser.OpenAsync(data, BrowserLaunchMode.SystemPreferred);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
