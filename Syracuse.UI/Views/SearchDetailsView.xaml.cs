using MvvmCross.Forms.Views;
using Syracuse.Mobitheque.Core.ViewModels;
using Xamarin.Forms.Xaml;
using Syracuse.Mobitheque.Core.Models;
using System;
using Xamarin.Forms;
using System.Windows.Input;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Syracuse.Mobitheque.Core;
using System.Collections.Generic;

namespace Syracuse.Mobitheque.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchDetailsView : MvxContentPage<SearchDetailsViewModel>
    {

        public SearchDetailsView()
        {
            InitializeComponent();
        }
        private void carouselView_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            var model = e.CurrentItem as Result;
        }

        private async void carouselView_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            var CurrentPosition = e.CurrentPosition;
            await this.ViewModel.RaiseAllPropertiesChanged();
            if (e.CurrentPosition > e.PreviousPosition)
            {
                if (e.CurrentPosition + 1 >= this.ViewModel.EndDataPosition && e.CurrentPosition + 1 < this.ViewModel.ItemsSource.Count)
                {
                    this.ViewModel.EndDataPosition = this.ViewModel.EndDataPosition + 10 < this.ViewModel.ItemsSource.Count ? this.ViewModel.EndDataPosition + 10 : this.ViewModel.ItemsSource.Count - 1;
                  await  this.ViewModel.FormateToCarrousel(e.CurrentPosition, this.ViewModel.EndDataPosition);
                }
            }
            else
            {
                if (e.CurrentPosition <= this.ViewModel.StartDataPosition && e.CurrentPosition > 0)
                {
                    this.ViewModel.StartDataPosition = this.ViewModel.StartDataPosition - 10 >= 0 ? this.ViewModel.StartDataPosition - 10 : 0;
                    await this.ViewModel.FormateToCarrousel(this.ViewModel.StartDataPosition, e.CurrentPosition);
                }
                if (e.CurrentPosition < e.PreviousPosition - 1)
                {
                    this.carouselView.Position = e.PreviousPosition;
                    CurrentPosition = e.PreviousPosition;
                }
            }
            this.ViewModel.Position = CurrentPosition;
            Console.WriteLine("OpenBrowser_OnClicked " + e.CurrentPosition);
            this.ViewModel.IsPositionVisible = true;
        }
       
        private async void OpenBrowser_OnClicked(object sender, EventArgs e)
        {
            string url = ((Button)sender).CommandParameter as string;
            Uri uri;
            try
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    uri = await this.ViewModel.GetUrlTransfert(uri);
                    //await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                    this.ViewModel.OpenWebBrowser(uri.ToString());


                }
                else
                {
                    uri = await this.ViewModel.RelativeUriToAbsolute(url);
                    uri = await this.ViewModel.GetUrlTransfert(uri);
                    this.ViewModel.OpenWebBrowser(uri.ToString());
                    //await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

                }
            }
            catch (Exception)
            {
                await DisplayAlert(ApplicationResource.Warning, String.Format(ApplicationResource.ErrorOccurred), ApplicationResource.ButtonValidation);
            }

        }

        private async void HoldingButton_Clicked(object sender, EventArgs e)
        {
            List<HoldingPlace> commandParameters = ((Button)sender).CommandParameter as List<HoldingPlace>;
            Holdings data = ((Button)sender).BindingContext as Holdings;
            List<string> holdiongplaces = new List<string>();
            if (commandParameters != null && commandParameters.Count > 0)
            {
                foreach (var item in commandParameters)
                {
                    holdiongplaces.Add(item.Libelle);
                }
                string value = await DisplayActionSheet(ApplicationResource.HoldingLibraryChoice, ApplicationResource.Cancel, null, holdiongplaces.ToArray());
                if (value == ApplicationResource.Cancel)
                {
                    return;
                }
                else
                {
                    foreach (var item in commandParameters)
                    {
                        if (item.Libelle == value)
                        {
                            data.HoldingPlace = item.Code;
                            data.Site = item.Libelle;
                            data.SiteCode = item.Code;
                        }
                    }
                }

            }
            bool answer = await DisplayAlert(ApplicationResource.Warning, String.Format(ApplicationResource.HoldingChoice, data.Site), ApplicationResource.Book, ApplicationResource.Cancel);
            if (answer)
            {
                await this.ViewModel.Holding(data);
            }
        }

        private void WebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            this.ViewModel.AbsoluteIndicatorVisibility = true;
            this.ViewModel.RaiseAllPropertiesChanged();
        }

        private void WebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            var item = sender as WebView;
            this.ViewModel.AbsoluteIndicatorVisibility = false;
        }

        protected override void OnBindingContextChanged()
        {
            (this.DataContext as SearchDetailsViewModel).OnDisplayAlert += SearchDetailsView_OnDisplayAlert;
            (this.DataContext as SearchDetailsViewModel).OnDisplayAlertMult += SearchDetailsView_OnDisplayAlertMult;
            base.OnBindingContextChanged();
        }

        private void SearchDetailsView_OnDisplayAlert(string title, string message, string button) => this.DisplayAlert(title, message, button);
        private Task<bool> SearchDetailsView_OnDisplayAlertMult(string title, string message, string buttonYes, string buttonNo)
        {
            var res = this.DisplayAlert(title, message, buttonYes, buttonNo);
            return res;
        }


        


    }
}