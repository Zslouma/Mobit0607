﻿using Newtonsoft.Json;
using Refit;
using Syracuse.Mobitheque.Core.Models;
using Syracuse.Mobitheque.Core.Services.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Syracuse.Mobitheque.Core.Services.Requests
{
    /*
     * The aim of this service is to wrap a IRefitRequests object
     * in order to register him as a singleton is the IoC Container.
     */
    public class RequestService : IRequestService
    {
        private Uri httpUri;
        private IRefitRequests requests;
        private CookieContainer cookies;
        private HttpClientHandler handler;
        private String token;
        private readonly IConfigService configService;

        public RequestService(IConfigService configService)
        {
            this.configService = configService;
            this.cookies = new CookieContainer();
            this.handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = this.cookies

            };
            this.token = this.Timestamp();
        }
        /// <summary>
        /// Vérifie si l'url passé en paramétre posséde une redirection ou non
        /// </summary>
        /// <param name="originalURL"></param>
        /// <param name="defaultURL"></param>
        /// <returns></returns>
        public async Task<string> GetRedirectURL(string originalURL, string defaultURL = "https://user-images.githubusercontent.com/24848110/33519396-7e56363c-d79d-11e7-969b-09782f5ccbab.png")
        {
            try
            {
                var tempohandler = new HttpClientHandler()
                {
                    UseCookies = true,
                    CookieContainer = this.cookies
                };
                tempohandler.AllowAutoRedirect = false;
                tempohandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                HttpClient httpClient = new HttpClient(tempohandler);
                httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();
                httpClient.DefaultRequestHeaders.CacheControl.NoStore = true;
                httpClient.DefaultRequestHeaders.CacheControl.NoCache = true;

                // Get the response ...
                using (var webResponse = (HttpResponseMessage)await httpClient.GetAsync(originalURL))
                {
                    // Now look to see if it's a redirect
                    if ((int)webResponse.StatusCode >= 300 && (int)webResponse.StatusCode <= 399)
                    {
                        string uriString = webResponse.RequestMessage.RequestUri.ToString();
                        return uriString;

                    }
                    else if ((int)webResponse.StatusCode >= 200 && (int)webResponse.StatusCode <= 299)
                    {
                        string uriString = webResponse.RequestMessage.RequestUri.ToString();
                        return uriString;
                    }
                    else
                    {
                        return defaultURL;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(defaultURL);
                return defaultURL;
            }
        }

        public String Timestamp()
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds).ToString();
        }



        #region Cookies
        public void clearCookies()
        {
            foreach (Cookie co in cookies.GetCookies(this.httpUri))
            {
                co.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            }
        }
        public void clearCookies(Cookie cotarget)
        {
            foreach (Cookie co in cookies.GetCookies(this.httpUri))
            {
                if (co == cotarget)
                {
                    co.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));
                }
            }
        }

        public CookieContainer GetCookieContainer()
        {
            return this.cookies;
        }

        public IEnumerable<Cookie> GetCookies(string url = null, CookieContainer cookiesTempo = null)
        {
            if (!String.IsNullOrEmpty(url))
            {
                this.InitializeHttpClient(url);
            }
            foreach (Cookie cookie in cookiesTempo == null ? this.cookies.GetCookies(this.httpUri) : cookiesTempo.GetCookies(new Uri(url))){
                yield return cookie;
            }
        }
        public async Task UpdateCookies()
        {
            CookiesSave b = await App.Database.GetActiveUser();
            var cookiesGetted = GetCookies().ToArray();
            List<Cookie> cookiesGettedTempo = new List<Cookie> { };
            List<String> name = new List<String> { };
            foreach (var cook in cookiesGetted.OrderByDescending(cook => cook.Expires))
            {
                if (!name.Contains(cook.Name))
                {
                    name.Add(cook.Name);
                    if (cook.Expires < DateTime.Now)
                    {
                        cook.Expires = cook.TimeStamp.AddMonths(1);
                    }
                    cookiesGettedTempo.Add(cook);
                }
                else
                {
                    cook.Expires = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                }

            }
            cookiesGetted = cookiesGettedTempo.ToArray();
            b.Cookies = JsonConvert.SerializeObject(cookiesGetted);
            await App.Database.SaveItemAsync(b);
            var cookiestempo = JsonConvert.DeserializeObject<Cookie[]>(b.Cookies);
            this.LoadCookies(cookiestempo);

        }
        public void LoadCookies(Cookie[] cookies)
        {
            this.cookies = new CookieContainer();
            foreach (Cookie cookie in cookies)
            {
                this.cookies.Add(cookie);
            }
            this.handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = this.cookies
            };
        }

        public void ResetCookies()
        {
            this.cookies = new CookieContainer();
            this.handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = this.cookies

            };
        }


        #endregion



        public async Task<LoginStatus> Authentication(string useraccount, string password, string baseUrl, Action<Exception> error = null)
        {
            this.ResetCookies();
            this.InitializeHttpClient(baseUrl);


            try
            {
                var status = await this.requests.Authentication<LoginStatus>(new Dictionary<string, object>() {
                                                                              { "username", useraccount},
                                                                              { "password", password},
                                                                              { "rememberMe", true}});
                return status;
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
                return null;
            }
        }
        /// <summary>
        /// Fournie une url possédant un token d'Authentication lorsque c'est possible
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<UrlWithAuthenticationStatus> GetUrlWithAuthenticationTransfert(Uri uri)
        {

            await this.InitializeHttpClient();

            UrlWithAuthenticationStatus status;
            try
            {
                UrlWithAuthenticationTransfertOptions transfertOptions = new UrlWithAuthenticationTransfertOptions(uri.ToString());
                status = await this.requests.GetUrlWithAuthenticationTransfert<UrlWithAuthenticationStatus>(transfertOptions);
                return status;
            }
            catch (Exception)
            {
                status = new UrlWithAuthenticationStatus();
                return status;
            }

        }


        private async Task InitializeHttpClient()
        {
            CookiesSave user = await App.Database.GetActiveUser();
            if (user != null)
            {
                if (user.LibraryUrl.Last() == '/')
                {
                    user.LibraryUrl = user.LibraryUrl.Remove(user.LibraryUrl.Length - 1);
                }
                this.httpUri = new Uri(user.LibraryUrl);

                await this.UpdateCookies();

                HttpClient httpClient = new HttpClient(this.handler)
                {
                    BaseAddress = this.httpUri
                };
                httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();
                httpClient.DefaultRequestHeaders.CacheControl.NoStore = true;
                httpClient.DefaultRequestHeaders.CacheControl.NoCache = true;
                this.requests = RestService.For<IRefitRequests>(httpClient);
            }
        }

        public void InitializeHttpClient(string url)
        {
            if (url.Last() == '/')
            {
                url = url.Remove(url.Length - 1);
            }
            this.httpUri = new Uri(url);

            HttpClient httpClient = new HttpClient(this.handler)
            {
                BaseAddress = this.httpUri
            };
            httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();
            httpClient.DefaultRequestHeaders.CacheControl.NoStore = true;
            httpClient.DefaultRequestHeaders.CacheControl.NoCache = true;
            this.requests = RestService.For<IRefitRequests>(httpClient);
        }

        public async Task<AccountSummary> GetSummary( string baseUrl = null, Cookie[] CookiesArray = null, Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection" + App.AppState.NetworkConnection);
            }
            if (baseUrl != null && CookiesArray != null)
            {
                this.ResetCookies();
                this.InitializeHttpClient(baseUrl);
                this.LoadCookies(CookiesArray);


            }
            else
            {
                await this.InitializeHttpClient();
            }
            
            try
            {
                var status = await this.requests.GetSummary<AccountSummary>(this.token);
                if (status.Success)
                {
                    await UpdateCookies();
                }
                return status;
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
                return null;
            }
        }

        public async Task<SearchResult> Search(SearchOptions options, Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection" + App.AppState.NetworkConnection);
            }
            await this.InitializeHttpClient();
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.Query.ScenarioCode == "")
            {
                options.Query.ScenarioCode = (await App.Database.GetActiveUser()).SearchScenarioCode;
            }
            try
            {
                var status = await this.requests.Search<SearchResult>(options);

                await UpdateCookies();

                return status;
            }
            catch (Exception ex)
            {
                var status = new SearchResult();
                status.Errors = new Error[1];
                if (!App.AppState.NetworkConnection)
                {
                    status.Errors[0] = new Error(ApplicationResource.NetworkDisable);
                }
                else
                {
                    status.Errors[0] = new Error(ApplicationResource.ErrorOccurred);
                }
                error?.Invoke(ex);
                return status;
            }


        }

        public async Task<CheckAvailabilityResult> CheckAvailability(CheckAvailabilityOptions options, Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection" + App.AppState.NetworkConnection);
            }
            await this.InitializeHttpClient();
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.Query.ScenarioCode == "")
            {
                options.Query.ScenarioCode = (await App.Database.GetActiveUser()).SearchScenarioCode;
            }
            try
            {
                var status = await this.requests.CheckAvailability<CheckAvailabilityResult>(options);

                await UpdateCookies();

                return status;
            }
            catch (Exception ex)
            {
                var status = new CheckAvailabilityResult();
                status.Errors = new Error[1];
                if (!App.AppState.NetworkConnection)
                {
                    status.Errors[0] = new Error(ApplicationResource.NetworkDisable);
                }
                else
                {
                    status.Errors[0] = new Error(ApplicationResource.ErrorOccurred);
                }
                error?.Invoke(ex);
                return status;
            }
        }

        public async Task<SearchLibraryResult> SearchLibrary(SearchLibraryOptions options, Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection" + App.AppState.NetworkConnection);
            }
            await this.InitializeHttpClient();

            if (options == null)
                throw new ArgumentNullException(nameof(options));
            try
            {
                var status = await this.requests.SearchLibrary<SearchLibraryResult>(options);

                await UpdateCookies();

                return status;
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
                return null;
            }
        }

        public async Task<LoansResult> GetLoans(Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection" + App.AppState.NetworkConnection);
            }
            await this.InitializeHttpClient();
            try
            {
                var timestamp = this.Timestamp();
                this.token = this.Timestamp();
                var status = await this.requests.GetLoans<LoansResult>(timestamp, this.token);

                await UpdateCookies();

                return status;
            }
            catch (Exception ex)
            {
                var status = new LoansResult();
                status.Errors = new Error[1];
                if (!App.AppState.NetworkConnection)
                {
                    status.Errors[0] = new Error(ApplicationResource.NetworkDisable);
                }
                else
                {
                    status.Errors[0] = new Error(ApplicationResource.ErrorOccurred);
                }
                error?.Invoke(ex);
                return status;
            }
        }

        public async Task<BookingResult> GetBookings(Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection" + App.AppState.NetworkConnection);
            }
            await this.InitializeHttpClient();
            try
            {
                var timestamp = this.Timestamp();
                this.token = this.Timestamp();
                var status = await this.requests.GetBookings<BookingResult>(timestamp, this.token);

                await UpdateCookies();

                return status;
            }
            catch (Exception ex)
            {
                var status = new BookingResult();
                status.Errors = new Error[1];
                if (!App.AppState.NetworkConnection)
                {
                    status.Errors[0] = new Error(ApplicationResource.NetworkDisable);
                }
                else
                {
                    status.Errors[0] = new Error(ApplicationResource.ErrorOccurred);
                }
                error?.Invoke(ex);
                return status;
            }
        }

        public async Task<InstanceResult<List<UserDemands>>> GetUserDemands(Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection" + App.AppState.NetworkConnection);
            }
            await this.InitializeHttpClient();
            try
            {
                var timestamp = this.Timestamp();
                this.token = this.Timestamp();
                var status = await this.requests.GetUserDemands<InstanceResult<List<UserDemands>>>(timestamp, this.token);

                await UpdateCookies();

                return status;
            }
            catch (Exception ex)
            {
                var status = new InstanceResult<List<UserDemands>>();
                status.Errors = new Error[1];
                if (!App.AppState.NetworkConnection)
                {
                    status.Errors[0] = new Error(ApplicationResource.NetworkDisable);
                }
                else
                {
                    status.Errors[0] = new Error(ApplicationResource.ErrorOccurred);
                }
                error?.Invoke(ex);
                return status;
            }
        }

        public async Task<RenewLoanResult> RenewLoans(LoanOptions options, Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection" + App.AppState.NetworkConnection);
            }
            await this.InitializeHttpClient();
            try
            {
                this.token = this.Timestamp();
                var status = await this.requests.RenewLoans<RenewLoanResult>(options);

                await UpdateCookies();

                return status;
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
                return null;
            }
        }

        public async Task<CancelBookingResult> CancelBooking(BookingOptions options, Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection " + App.AppState.NetworkConnection);
            }
            await this.InitializeHttpClient();
            try
            {
                this.token = this.Timestamp();
                var status = await this.requests.CancelBooking<CancelBookingResult>(options);
                await UpdateCookies();
                return status;
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
                return null;
            }
        }

        public async Task<PlaceReservationResult> PlaceReservation(PlaceReservationOptions options, Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection" + App.AppState.NetworkConnection);
            }
            await this.InitializeHttpClient();
            try
            {
                this.token = this.Timestamp();
                var status = await this.requests.PlaceReservation<PlaceReservationResult>(options);
                await UpdateCookies();
                return status;
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
                return null;
            }
        }

        public async Task<InstanceResult<string>> RenderAccountWebFrame(AccountWebFrameOptions options, Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection" + App.AppState.NetworkConnection);
            }
            await this.InitializeHttpClient();
            try
            {
                this.token = this.Timestamp();
                List<string> collectionstring = new List<string>();
                string codebare = "";
                string username = "";
                var status = await this.requests.RenderAccountWebFrame<InstanceResult<string>>(options);
                if (status.Success)
                {
                    string regex = "<span [^>]*class=(\"|')user-name(\"|')>(.*?)</span>";
                    Match UserNameMatch = new Regex(regex, RegexOptions.Singleline | RegexOptions.Compiled).Match(status.D);
                    username = UserNameMatch.Groups[3].Value;
                    regex = "<p [^>]*class=(\"|')myaccount-profile-entry(\"|')>(.*?)</p>";
                    MatchCollection matches = new Regex(regex, RegexOptions.Singleline | RegexOptions.Compiled).Matches(status.D);
                    regex = "<span [^>]*class=(\"|')account-value(\"|')>(.*?)</span>";
                    foreach (Match match in matches)
                    {
                        collectionstring.Add(match.Value);
                        if (match.Value.Contains("Code-barres"))
                        {
                            Match barecode = new Regex(regex, RegexOptions.Singleline | RegexOptions.Compiled).Match(match.Value);
                            codebare = barecode.Groups[3].Value;
                        }
                    }
                    await UpdateCookies();
                    CookiesSave user = await App.Database.GetActiveUser();
                    if (username != "") user.Username = username;
                    if (codebare != "") user.CodeBare = codebare;
                    await App.Database.SaveItemAsync(user);
                }
                return status;
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
                return new InstanceResult<string>();
            }
        }

        public async Task<InstanceResult<AgendaResult>> GetEventById(EventByIdOptions options, Action<Exception> error = null)
        {
            if (!App.AppState.NetworkConnection)
            {
                Debug.WriteLine("NetworkConnection" + App.AppState.NetworkConnection);
            }
            await this.InitializeHttpClient();
            try
            {
                this.token = this.Timestamp();
                var status = await this.requests.GetEventById<InstanceResult<AgendaResult>>(options);
                await UpdateCookies();
                return status;
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
                return null;
            }
        }
    }
}
