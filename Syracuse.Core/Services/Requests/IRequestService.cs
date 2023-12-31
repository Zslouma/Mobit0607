﻿using Syracuse.Mobitheque.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Syracuse.Mobitheque.Core.Services.Requests
{
    public interface IRequestService
    {
        /// <summary>
        /// Get cookies of current httpClient.
        /// </summary>
        /// <returns>Current cookies</returns>
        IEnumerable<Cookie> GetCookies(string url = null, CookieContainer cookiesTempo = null);

        /// <summary>
        /// Get cookies of current httpClient. in CookieContainer format
        /// </summary>
        /// <returns></returns>
        CookieContainer GetCookieContainer();

        void LoadCookies(Cookie[] cookies);

        void ResetCookies();

        void InitializeHttpClient(string url);

        void clearCookies();

        void clearCookies(Cookie cotarget);

        Task<string> GetRedirectURL(string originalURL, string defaultURL = "https://user-images.githubusercontent.com/24848110/33519396-7e56363c-d79d-11e7-969b-09782f5ccbab.png");

        Task<LoginStatus> Authentication(string useraccount, string password, string baseUrl, Action<Exception> error = null);

        Task<UrlWithAuthenticationStatus> GetUrlWithAuthenticationTransfert(Uri uri);

        Task<AccountSummary> GetSummary( string baseUrl = null, Cookie[] CookiesArray = null, Action<Exception> error = null);

        Task<LoansResult> GetLoans(Action<Exception> error = null);

        Task<BookingResult> GetBookings(Action<Exception> error = null);

        Task<InstanceResult<List < UserDemands >>> GetUserDemands(Action<Exception> error = null);

        Task<SearchResult> Search(SearchOptions options, Action<Exception> error = null);

        Task<CheckAvailabilityResult> CheckAvailability(CheckAvailabilityOptions options, Action<Exception> error = null);

        Task<SearchLibraryResult> SearchLibrary(SearchLibraryOptions options, Action<Exception> error = null);

        Task<PlaceReservationResult> PlaceReservation(PlaceReservationOptions options, Action<Exception> error = null);

        Task<RenewLoanResult> RenewLoans(LoanOptions options, Action<Exception> error = null);

        Task<CancelBookingResult> CancelBooking(BookingOptions options, Action<Exception> error = null);

        Task<InstanceResult<string>> RenderAccountWebFrame(AccountWebFrameOptions options, Action<Exception> error = null);

        Task<InstanceResult<AgendaResult>> GetEventById(EventByIdOptions options, Action<Exception> error = null);
    }
}
