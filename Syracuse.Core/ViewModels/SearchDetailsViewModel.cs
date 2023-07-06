using MvvmCross.Commands;
using MvvmCross.Navigation;
using Syracuse.Mobitheque.Core.Models;
using Syracuse.Mobitheque.Core.Services.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Syracuse.Mobitheque.Core.ViewModels
{

    public class SearchDetailsViewModel : BaseViewModel<SearchDetailsParameters, SearchResult>
    {
        private readonly IRequestService requestService;

        private readonly IMvxNavigationService navigationService;

        private SearchLibraryResult library;

        public bool success { get; set; } = false;

        private string authorDate;

        private string star;
        public string Star
        {
            get => this.star;
            set { SetProperty(ref this.star, value); }
        }

        public string AuthorDate
        {
            get => this.authorDate;
            set { SetProperty(ref this.authorDate, value); }
        }

        private string desc;
        public string Desc
        {
            get => this.desc;
            set { SetProperty(ref this.desc, value); }
        }
        private ObservableCollection<Result> itemsSource;
        public ObservableCollection<Result> ItemsSource
        {
            get => this.itemsSource;
            set { SetProperty(ref this.itemsSource, value); }
        }
        private Result currentItem;
        public Result CurrentItem
        {
            get => this.currentItem;
            set { SetProperty(ref this.currentItem, value); }
        }
        private int position;
        public int Position
        {
            get => this.position;
            set
            {
                this.DisplayPosition = (value + 1).ToString() + " / " + this.NbrResults;
                SetProperty(ref this.position, value);
            }
        }

        private int startDataPosition;
        public int StartDataPosition
        {
            get => this.startDataPosition;
            set
            {
                SetProperty(ref this.startDataPosition, value);
            }
        }

        private int endDataPosition;
        public int EndDataPosition
        {
            get => this.endDataPosition;
            set
            {
                SetProperty(ref this.endDataPosition, value);
            }
        }

        private string displayPosition = "";
        public string DisplayPosition
        {
            get => this.displayPosition;
            set { SetProperty(ref this.displayPosition, value); }
        }

        private string nbrResults;
        public string NbrResults
        {
            get => this.nbrResults;
            set { SetProperty(ref this.nbrResults, value); }
        }

        private string query;
        public string Query
        {
            get => this.query;
            set { SetProperty(ref this.query, value); }
        }

        private SearchOptions searchOptions;
        public SearchOptions SearchOptions
        {
            get => this.searchOptions;
            set { SetProperty(ref this.searchOptions, value); }
        }

        private bool inLoadMore = false;
        public bool InLoadMore
        {
            get => this.inLoadMore;
            set { SetProperty(ref this.inLoadMore, value); }
        }

        private bool isAgenda = true;
        public bool IsAgenda
        {
            get => this.isAgenda;
            set { SetProperty(ref this.isAgenda, value); }
        }


        public SearchLibraryResult Library
        {
            get => this.library;
            set { SetProperty(ref this.library, value); }
        }

        private bool isBusy = true;
        public bool IsBusy
        {
            get => this.isBusy;
            set
            {
                SetProperty(ref this.isBusy, value);
            }
        }

        private bool isPositionVisible = true;
        public bool IsPositionVisible
        {
            get => this.isPositionVisible;
            set
            {
                SetProperty(ref this.isPositionVisible, value);
            }
        }

        private bool isCarouselVisibility = false;
        public bool IsCarouselVisibility
        {
            get => this.isCarouselVisibility;
            set
            {
                SetProperty(ref this.isCarouselVisibility, value);
            }
        }


        private bool reversIsKm = false;
        public bool ReversIsKm
        {
            get => this.reversIsKm;
            set { SetProperty(ref this.reversIsKm, value); }
        }

        private bool absoluteIndicatorVisibility;
        public bool AbsoluteIndicatorVisibility
        {
            get => this.absoluteIndicatorVisibility;
            set { SetProperty(ref this.absoluteIndicatorVisibility, value); }
        }

        public CookiesSave user { get; set; }

        private MvxAsyncCommand<string> searchCommand;
        public MvxAsyncCommand<string> SearchCommand => this.searchCommand ??
        (this.searchCommand = new MvxAsyncCommand<string>((text) => this.PerformSearch(text)));

        public CookieContainer cookie { get { return this.requestService.GetCookieContainer(); } }

        public SearchDetailsViewModel(IMvxNavigationService navigationService, IRequestService requestService)
        {
            this.navigationService = navigationService;
            this.requestService = requestService;
        }

        private string setStar(long star)
        {
            if (star == 1) return "https://upload.wikimedia.org/wikipedia/commons/d/dd/Star_rating_1_of_5.png";
            else if (star == 2) return "https://upload.wikimedia.org/wikipedia/commons/9/95/Star_rating_2_of_5.png";
            else if (star == 3) return "https://upload.wikimedia.org/wikipedia/commons/2/2f/Star_rating_3_of_5.png";
            else if (star == 4) return "https://upload.wikimedia.org/wikipedia/commons/f/fa/Star_rating_4_of_5.png";
            else if (star == 5) return "https://upload.wikimedia.org/wikipedia/commons/1/17/Star_rating_5_of_5.png";
            return null;
        }
        async public override void Prepare(SearchDetailsParameters parameter)
        {
            try
            {
                this.IsBusy = true;
                this.IsCarouselVisibility = false;
                await this.CanHolding();
                this.SearchOptions = parameter.searchOptions;
                this.NbrResults = parameter.nbrResults;
                var parameterTempo = parameter.parameter;
                this.ItemsSource = new ObservableCollection<Result>(parameterTempo[1].D.Results);
                var tempo = parameterTempo[1].D.Results.ToList().FindIndex(x => x == parameterTempo[0].D.Results[0]);
                this.StartDataPosition = tempo - 10 >= 0 ? tempo - 10 : 0;
                this.EndDataPosition = tempo + 10 < parameterTempo[1].D.Results.Length ? tempo + 10 : parameterTempo[1].D.Results.Length - 1;
                await this.FormateToCarrousel(this.StartDataPosition, this.EndDataPosition, false);
                this.CurrentItem = this.ItemsSource[tempo];
                this.Position = tempo;
                this.IsPositionVisible = true;
                if (tempo >= (this.ItemsSource.Count() - 5) && int.Parse(this.NbrResults) > this.ItemsSource.Count)
                {
                    await LoadMore(false);
                }
                this.ItemsSource[0] = this.ItemsSource[0].Clone();
                await this.RaisePropertyChanged(nameof(this.ItemsSource));
                await this.RaisePropertyChanged(nameof(this.CurrentItem));
                await this.RaisePropertyChanged(nameof(this.Position));
                await this.RaiseAllPropertiesChanged();
                this.IsCarouselVisibility = true;
                this.IsBusy = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex.Message : " + ex.Message);
                throw;
            }
        }

        public async Task FormateToCarrousel(Result[] results)
        {
            foreach (var resultTempo in results)
            {
                if (resultTempo.Resource.Desc != null)
                    resultTempo.DisplayValues.Desc = resultTempo.Resource.Desc;
                resultTempo.DisplayValues.Star = this.setStar(resultTempo.Resource.AvNt);
                if (resultTempo.DisplayValues.Star == null)
                {
                    resultTempo.DisplayValues.DisplayStar = false;
                }
                else
                {
                    resultTempo.DisplayValues.DisplayStar = true;
                }
                if (resultTempo.HasViewerDr)
                {
                    if (this.user == null)
                    {
                        this.user = await App.Database.GetActiveUser();
                    }
                    try
                    {
                        if (resultTempo.FieldList.NumberOfDigitalNotices != null && resultTempo.FieldList.NumberOfDigitalNotices.Length > 0 && resultTempo.FieldList.NumberOfDigitalNotices[0] > 0)
                        {
                            resultTempo.FieldList.UrlViewerDR = this.user.LibraryUrl + "/digital-viewer/c-" + resultTempo.FieldList.Identifier[0];
                        }
                        else if (resultTempo.FieldList.DigitalReadyIsEntryPoint != null && resultTempo.FieldList.DigitalReadyIsEntryPoint.Length > 0 && Convert.ToInt32(resultTempo.FieldList.DigitalReadyIsEntryPoint[0]) > 0)
                        {
                            resultTempo.FieldList.UrlViewerDR = this.user.LibraryUrl + "/digital-viewer/d-" + resultTempo.FieldList.Identifier[0];
                        }
                        else
                        {
                            resultTempo.HasDigitalReady = false;
                            resultTempo.HasViewerDr = false;
                            resultTempo.FieldList.NumberOfDigitalNotices = null;
                            resultTempo.FieldList.DigitalReadyIsEntryPoint = null;
                        }
                    }
                    catch (Exception)
                    {
                        resultTempo.HasDigitalReady = false;
                        resultTempo.HasViewerDr = false;
                        resultTempo.FieldList.NumberOfDigitalNotices = null;
                        resultTempo.FieldList.DigitalReadyIsEntryPoint = null;
                    }
                    // Génération des url du Viewer DR 

                }
                resultTempo.DisplayValues.SeekForHoldings = resultTempo.SeekForHoldings && this.ReversIsKm;
                Console.WriteLine("resultTempo.Resource.RscBase : " + resultTempo.Resource.RscBase);
                await PerformSearch(resultTempo.Resource.RscId, resultTempo.Resource.RscBase);
                this.BuildHoldingsStatements();
                this.BuildHoldings();
                resultTempo.DisplayValues.Library = this.Library;
                resultTempo.DisplayValues.Library.success = resultTempo.DisplayValues.Library.success && resultTempo.DisplayValues.SeekForHoldings;
                if (resultTempo.DisplayValues.Library.success)
                {
                    if (resultTempo.DisplayValues.Library.Dataa.fieldList.sys_base.Contains("DILICOM"))
                    {
                        resultTempo.FieldList.GetZipLabel = ApplicationResource.OnlineConsult;
                        resultTempo.FieldList.ZIPURL = new string[] { resultTempo.FriendlyUrl.ToString() };
                        resultTempo.FieldList.GetZipUri = resultTempo.FriendlyUrl.ToString();
                    }

                }
                this.ItemsSource.Add(resultTempo);
            }
        }
        public async Task FormateToCarrousel(int start, int end, bool endIsBusy = true)
        {
            try
            {
                this.IsBusy = true;
                for (int i = start; i <= end; i++)
                {
                    try
                    {
                        Console.WriteLine("FormateToCarrousel :" + i);
                        if (this.ItemsSource[i].Resource.Desc != null)
                            this.ItemsSource[i].DisplayValues.Desc = this.ItemsSource[i].Resource.Desc;
                        this.ItemsSource[i].DisplayValues.Star = this.setStar(this.ItemsSource[i].Resource.AvNt);
                        if (this.ItemsSource[i].DisplayValues.Star == null)
                        {
                            this.ItemsSource[i].DisplayValues.DisplayStar = false;
                        }
                        else
                        {
                            this.ItemsSource[i].DisplayValues.DisplayStar = true;
                        }
                        if (this.ItemsSource[i].HasViewerDr)
                        {
                            if (this.user == null)
                            {
                                this.user = await App.Database.GetActiveUser();
                            }
                            // Génération des url du Viewer DR 
                            try
                            {
                                if (this.ItemsSource[i].FieldList.NumberOfDigitalNotices != null && this.ItemsSource[i].FieldList.NumberOfDigitalNotices.Length > 0 && this.ItemsSource[i].FieldList.NumberOfDigitalNotices[0] > 0)
                                {
                                    this.ItemsSource[i].FieldList.UrlViewerDR = this.user.LibraryUrl + "/digital-viewer/c-" + this.ItemsSource[i].FieldList.Identifier[0];
                                }
                                else if (this.ItemsSource[i].FieldList.DigitalReadyIsEntryPoint != null && this.ItemsSource[i].FieldList.DigitalReadyIsEntryPoint.Length > 0 && Convert.ToInt32(this.ItemsSource[i].FieldList.DigitalReadyIsEntryPoint[0]) > 0)
                                {
                                    this.ItemsSource[i].FieldList.UrlViewerDR = this.user.LibraryUrl + "/digital-viewer/d-" + this.ItemsSource[i].FieldList.Identifier[0];
                                }
                                else
                                {
                                    this.ItemsSource[i].HasDigitalReady = false;
                                    this.ItemsSource[i].HasViewerDr = false;
                                    this.ItemsSource[i].FieldList.NumberOfDigitalNotices = null;
                                    this.ItemsSource[i].FieldList.DigitalReadyIsEntryPoint = null;
                                }
                            }
                            catch (Exception)
                            {
                                this.ItemsSource[i].HasDigitalReady = false;
                                this.ItemsSource[i].HasViewerDr = false;
                                this.ItemsSource[i].FieldList.NumberOfDigitalNotices = null;
                                this.ItemsSource[i].FieldList.DigitalReadyIsEntryPoint = null;
                            }

                        }
                        this.ItemsSource[i].DisplayValues.SeekForHoldings = this.ItemsSource[i].SeekForHoldings && this.ReversIsKm;
                         
                        await PerformSearch(this.ItemsSource[i].Resource.RscId, this.ItemsSource[i].Resource.RscBase);
                        if (this.itemsSource[i].Resource.RscBase == "CALENDAR")
                        { this.IsAgenda = false; }
                        this.BuildHoldingsStatements();
                        this.BuildHoldings();
                        this.ItemsSource[i].DisplayValues.Library = this.Library;
                        this.ItemsSource[i].DisplayValues.Library.success = this.ItemsSource[i].DisplayValues.Library.success && this.ItemsSource[i].DisplayValues.SeekForHoldings;
                        if (this.ItemsSource[i].DisplayValues.Library.success)
                        {
                            if (this.ItemsSource[i].DisplayValues.Library.Dataa.fieldList.sys_base.Contains("DILICOM"))
                            {
                                this.ItemsSource[i].FieldList.GetZipLabel = ApplicationResource.OnlineConsult;
                                this.ItemsSource[i].FieldList.ZIPURL = new string[] { this.ItemsSource[i].FriendlyUrl.ToString() };
                                this.ItemsSource[i].FieldList.GetZipUri = this.ItemsSource[i].FriendlyUrl.ToString();
                            }

                        }
                        
                    }
                    catch (Exception e)
                    {
                        var tempo = this.ItemsSource[i];
                        Console.WriteLine(e.Message);
                    }

                }
                var result = this.ItemsSource;
                this.ItemsSource = new ObservableCollection<Result>(result);
                await this.RaisePropertyChanged(nameof(ItemsSource));
                if (endIsBusy)
                {
                    this.IsBusy = false;
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }

        public void BuildHoldingsStatements()
        {
            if (this.Library.success && this.Library.Dataa.HoldingsStatements.Count > 0)
            {
                foreach (var item in this.Library.Dataa.HoldingsStatements)
                {
                    Dictionary<string, bool> DisplayHoldingsStatements = new Dictionary<string, bool>();
                    List<string> strValues = new List<string>();
                    foreach (var value in this.Library.Dataa.DisplayHoldingsStatements)
                    {
                        if (value.Value)
                        {
                            var itemObject = item.GetType().GetProperty(value.Key)?.GetValue(item, null);
                            string itemValue = itemObject != null ? itemObject.ToString() : "";
                            if (String.IsNullOrEmpty(itemValue))
                            {
                                DisplayHoldingsStatements.Add(value.Key, false);
                              
                                continue;
                            }
                            else
                            {
                                if (value.Key != "Site")
                                {
                                    strValues.Add(itemValue);
                                }
                            }
                        }
                        DisplayHoldingsStatements.Add(value.Key, value.Value);

                    }
                    if (strValues.Count > 0)
                    {
                        item.DisplayValue = String.Join(" | ", strValues);
                    }
                    item.DisplayHoldingsStatements = DisplayHoldingsStatements;
                }
            }
        }
        public void BuildHoldings()
        {
            if (this.Library.success && this.Library.Dataa.Holdings.Count > 0)
            {
                foreach (var item in this.Library.Dataa.Holdings)
                {
                    Dictionary<string, bool> DisplayHoldings = new Dictionary<string, bool>();
                    List<string> strValues = new List<string>();
                    foreach (var value in this.Library.Dataa.DisplayHoldings)
                    {
                        if (value.Value)
                        {
                            var itemObject = item.GetType().GetProperty(value.Key)?.GetValue(item, null);
                            string itemValue = itemObject != null ? itemObject.ToString() : "";
                            if (String.IsNullOrEmpty(itemValue))
                            {
                                DisplayHoldings.Add(value.Key, false);
                              
                                continue;
                            }
                            else
                            {
                                if (value.Key != "Site")
                                {
                                    strValues.Add(itemValue);
                                }
                            }
                        }
                        DisplayHoldings.Add(value.Key, value.Value);

                    }
                    if (strValues.Count > 0)
                    {
                        item.DisplayValue = String.Join(" | ", strValues);
                    }
                    if (item.IsHaveWhenBack)
                    {
                        item.DisponibilityText = String.Format(ApplicationResource.HoldingDisponibilityTextDateBack, item.WhenBack, item.Site);
                    }
                    else
                    {
                        item.DisponibilityText = String.Format(ApplicationResource.HoldingDisponibilityText, item.Site);
                    }
                    item.DisplayHoldings = DisplayHoldings;
                }
            }
        }

        public async Task CanHolding()
        {
            var user = await App.Database.GetActiveUser();
            this.ReversIsKm = !user.IsKm;
        }

        public async Task NavigationBack()
        {
            await navigationService.Close(this);
        }

        public async Task<Uri> RelativeUriToAbsolute(string uri)
        {
            var user = await App.Database.GetActiveUser();
            string url = user.DomainUrl + uri;
            Uri reslt = new Uri(url);
            return reslt;
        }

        public async Task<Uri> GetUrlTransfert(Uri uri)
        {
            UrlWithAuthenticationStatus status = await this.requestService.GetUrlWithAuthenticationTransfert(uri);
            if (status.Success)
            {
                uri = new Uri(await this.requestService.GetRedirectURL(status.D.ToString(), uri.ToString()));
                return uri;
            }
            else
            {
                return uri;
            }

        }

        public async Task LoadMore(bool endIsBusy = true)
        {
            this.InLoadMore = true;
            this.SearchOptions.Query.Page += 1;
            SearchResult search = await this.requestService.Search(this.SearchOptions);
            if (search != null && !search.Success)
            {
                this.DisplayAlert(ApplicationResource.Error, search.Errors?[0]?.Msg, ApplicationResource.ButtonValidation);
                return;
            }
            else
            {
                await this.FormateToCarrousel(search?.D?.Results);
            }

            if (endIsBusy)
            {
                this.IsBusy = false;
            }
            this.InLoadMore = false;
        }

        private async Task PerformSearch(string search = null, string docbase = "SYRACUSE")
        {
            try
            {
                if (docbase == null || docbase == "")
                {
                    docbase = "SYRACUSE";
                }
                if (search == null)
                {
                    search = this.Query;
                }
                else
                {
                    this.Query = search;
                }
                var options = new SearchLibraryOptionsDetails()
                {
                    RscId = search,
                    Docbase = docbase,
                };
                var res = await this.requestService.SearchLibrary(new SearchLibraryOptions() { Record = options });
                this.Library = res;

            }
            catch (Exception e)
            {
                this.Library = new SearchLibraryResult();
                Console.WriteLine(e.Message);
            }

        }

        public async Task Holding(Holdings data)
        {
            var options = new HoldingItem(data);

            try
            {
                PlaceReservationResult res = await this.requestService.PlaceReservation(new PlaceReservationOptions() { HoldingItem = options });
                if (res == null)
                {
                    this.DisplayAlert(ApplicationResource.Error, ApplicationResource.ErrorOccurred, ApplicationResource.ButtonValidation);
                }
                else if (!res.Success)
                {
                    this.DisplayAlert(ApplicationResource.Error, ApplicationResource.FailBookingRequest, ApplicationResource.ButtonValidation);
                }
                else
                {
                    await PerformSearch(null);
                    this.DisplayAlert(ApplicationResource.Success, ApplicationResource.SuccessBookingRequest, ApplicationResource.ButtonValidation);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public void OpenWebBrowser(string parameter)
        {
            this.navigationService.Navigate<WebAndCookiesViewModel, string>(parameter);
        }


    }
}
