﻿using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Navigation.EventArguments;
using Syracuse.Mobitheque.Core.Models;
using Syracuse.Mobitheque.Core.Services.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Syracuse.Mobitheque.Core.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly IRequestService requestService;

        private readonly IMvxNavigationService navigationService;

        private ObservableCollection<MenuNavigation> menuItemList;
        public ObservableCollection<MenuNavigation> MenuItemList
        {
            get => this.menuItemList;
            set => SetProperty(ref this.menuItemList, value);
        }

        private Dictionary<string, string> DictionaryViewModelLabel = new Dictionary<string, string>();

        private IMvxAsyncCommand<string> showDetailPageCommand;
        public IMvxAsyncCommand<string> ShowDetailPageCommand
        {
            get => this.showDetailPageCommand;
            set => SetProperty(ref this.showDetailPageCommand, value);
        }

        private String displayName;
        public String DisplayName
        {
            get => this.displayName;
            set
            {
                SetProperty(ref this.displayName, value);
            }
        }

        private String library;

        public String Library
        {
            get => this.library;
            set
            {
                SetProperty(ref this.library, value);
            }
        }

        private List<StandartViewList> standartViewLists;

        public List<StandartViewList> StandartViewLists
        {
            get => this.standartViewLists;
            set
            {
                SetProperty(ref this.standartViewLists, value);
            }
        }

        private bool isKm = false;

        public bool IsKm
        {
            get => this.isKm;
            set
            {
                SetProperty(ref this.isKm, value);
            }
        }


        public MenuViewModel(IMvxNavigationService navigationService,
            IRequestService requestService)
        {
            this.DictionaryViewModelLabel.Add("Syracuse.Mobitheque.Core.ViewModels.BookingViewModel", ApplicationResource.Bookings);
            this.DictionaryViewModelLabel.Add("Syracuse.Mobitheque.Core.ViewModels.LoansViewModel", ApplicationResource.Loans);
            this.navigationService = navigationService;
            this.navigationService.AfterNavigate += LoansNavigation;
            this.requestService = requestService;
        }

        public void AddStandardView()
        {
            UnicodeEncoding unicode = new UnicodeEncoding();
            foreach (var item in this.StandartViewLists)
            {
                this.menuItemList.Add(new MenuNavigation() { Text = item.ViewName, IconFontAwesome = item.ViewIcone });
            }
        }

        public async Task NavigationStandardView( string name)
        {
            UnicodeEncoding unicode = new UnicodeEncoding();
            foreach (var item in this.StandartViewLists)
            {
                if (name == item.ViewName)
                {
                    SearchOptions searchOptions = new SearchOptions();
                    searchOptions.Query = new SearchOptionsDetails()
                    {
                        QueryString = item.ViewQuery,
                        ScenarioCode = item.ViewScenarioCode,
                    };
                    searchOptions.PageTitle = name;
                    searchOptions.PageIcone = item.ViewIcone;
                    await this.navigationService.Navigate<StandardViewModel, SearchOptions>(searchOptions);
                }
            }
        }

        public override async void Prepare()
        {

            CookiesSave user = await App.Database.GetActiveUser();
            this.StandartViewLists = await App.Database.GetActiveStandartView(user);
            if (user != null)
            {
                this.IsKm = user.IsKm;
                this.Library = user.Library;
                if (string.IsNullOrEmpty(user?.DisplayName))
                {
                    AccountSummary account = await requestService.GetSummary();
                    user.DisplayName = account?.D?.AccountSummary?.DisplayName;
                    await App.Database.SaveItemAsync(user);
                    this.DisplayName = user.DisplayName;
                }
                else
                {
                    this.DisplayName = user.DisplayName;
                }
            }
            this.menuItemList = new ObservableCollection<MenuNavigation>() { };
            this.menuItemList.Add(new MenuNavigation() { Text = ApplicationResource.Home, IconFontAwesome = "\uf015", IsSelected = true });
            this.menuItemList.Add(new MenuNavigation() { Text = ApplicationResource.Account, IconFontAwesome = "\uf007" });
            this.menuItemList.Add(new MenuNavigation() { Text = ApplicationResource.OtherAccount, IconFontAwesome = "\uf0c0" });
            if (!IsKm)
            {
                this.menuItemList.Add(new MenuNavigation() { Text = ApplicationResource.Bookings, IconFontAwesome = "\uf017" });
                this.menuItemList.Add(new MenuNavigation() { Text = ApplicationResource.Loans, IconFontAwesome = "\uf02d" });
            }
            this.menuItemList.Add(new MenuNavigation() { Text = ApplicationResource.Scan, IconFontAwesome = "\uf02a" });
            this.menuItemList.Add(new MenuNavigation() { Text = ApplicationResource.Library, IconFontAwesome = "\uf66f" });
            this.AddStandardView();
            this.menuItemList.Add(new MenuNavigation() { Text = ApplicationResource.About, IconFontAwesome = "\uf05a" });
            this.menuItemList.Add(new MenuNavigation() { Text = ApplicationResource.Disconnect, IconFontAwesome = "\uf011" });
            await this.RaiseAllPropertiesChanged();
            this.ShowDetailPageCommand = new MvxAsyncCommand<string>(this.ShowDetailPageAsync);
            
        }

        private async Task RefreshMenuItem( string name)
        {
            var MenuItemListtempo = this.MenuItemList;
            foreach (var item in MenuItemListtempo)
            {
                if (item.Text == name)
                {
                    item.IsSelected = true;
                }
                else
                {
                    item.IsSelected = false;
                }
            }
            this.MenuItemList = new ObservableCollection<MenuNavigation>();
            this.MenuItemList = MenuItemListtempo;
            await this.RaiseAllPropertiesChanged();
        }
        private async Task ShowDetailPageAsync(string name)
        {
            await this.RefreshMenuItem(name);
            if (name == ApplicationResource.Home)
                _ = this.navigationService.Navigate<HomeViewModel>();
            else if (name == ApplicationResource.Bookings)
                _ = this.navigationService.Navigate<BookingViewModel>();
            else if (name == ApplicationResource.Scan)
                _ = this.navigationService.Navigate<BarcodeSearchModel>();
            else if (name == ApplicationResource.Loans)
                _ = this.navigationService.Navigate<LoansViewModel>();
            else if (name == ApplicationResource.Account)
                _ = this.navigationService.Navigate<MyAccountViewModel>();
            else if (name == ApplicationResource.OtherAccount)
                _ = this.navigationService.Navigate<OtherAccountViewModel>();
            else if (name == ApplicationResource.Disconnect)
            {
                var user = await App.Database.GetActiveUser();
                user.Active = false;
                await App.Database.SaveItemAsync(user);
                await this.navigationService.Navigate<SelectLibraryViewModel>();
            }
            else if (name == ApplicationResource.Library)
                await this.navigationService.Navigate<LibraryViewModel>();
            else if (name == ApplicationResource.About)
                await this.navigationService.Navigate<AboutViewModel>();
            await this.NavigationStandardView(name);
            /*
             * Close left side menu. 
             */
            if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
                masterDetailPage.IsPresented = false;
            else if (Application.Current.MainPage is NavigationPage navigationPage && 
                     navigationPage.CurrentPage is MasterDetailPage nestedMasterDetail)
                nestedMasterDetail.IsPresented = false;
        }

        /// <summary>
        /// Permet de detecter une navigation hors menu et de changer l'affichage du menu en conséquent 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LoansNavigation(object sender, IMvxNavigateEventArgs e)
        {
            
            var key = e.ViewModel.ToString();
            if (DictionaryViewModelLabel.ContainsKey(key)) {
                this.RefreshMenuItem(DictionaryViewModelLabel[key]).Wait();
            }

        }
    }
}
