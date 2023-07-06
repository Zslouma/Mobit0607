using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Syracuse.Mobitheque.Core.Models
{
    public class SearchResult
    {
        [JsonProperty("errors")]
        public Error[] Errors { get; set; }

        [JsonProperty("message")]
        public object Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("d")]
        public D D { get; set; }
    }

    public partial class D
    {
        [JsonProperty("FacetCollectionList")]
        public FacetCollectionList[] FacetCollectionList { get; set; }

        [JsonProperty("HtmlResult")]
        public string HtmlResult { get; set; }

        [JsonProperty("Query")]
        public Query Query { get; set; }

        [JsonProperty("Results")]
        public Result[] Results { get; set; }

        [JsonProperty("ScenarioDisplayMode")]
        public ScenarioDisplayMode[] ScenarioDisplayMode { get; set; }

        [JsonProperty("SearchInfo")]
        public SearchInfo SearchInfo { get; set; }

        [JsonProperty("Sorts")]
        public Sort[] Sorts { get; set; }

        [JsonProperty("SpellChecking")]
        public SpellChecking SpellChecking { get; set; }
    }

    public partial class FacetCollectionList
    {
        [JsonProperty("FacetContains")]
        public object FacetContains { get; set; }

        [JsonProperty("FacetDisplayMode")]
        public long FacetDisplayMode { get; set; }

        [JsonProperty("FacetDisplayed")]
        public long FacetDisplayed { get; set; }

        [JsonProperty("FacetField")]
        public string FacetField { get; set; }

        [JsonProperty("FacetId")]
        public long FacetId { get; set; }

        [JsonProperty("FacetLabel")]
        public string FacetLabel { get; set; }

        [JsonProperty("FacetList")]
        public FacetList[] FacetList { get; set; }

        [JsonProperty("FacetSortOrder")]
        public long FacetSortOrder { get; set; }

        [JsonProperty("IsMultiselectable")]
        public bool IsMultiselectable { get; set; }

        [JsonProperty("IsPreselectable")]
        public bool IsPreselectable { get; set; }

        [JsonProperty("IsSearchable")]
        public bool IsSearchable { get; set; }

        [JsonProperty("NullValues")]
        public bool NullValues { get; set; }

        [JsonProperty("RecoveryThresHold")]
        public object RecoveryThresHold { get; set; }

        [JsonProperty("Rights")]
        public string Rights { get; set; }
    }

    public partial class FacetList
    {
        [JsonProperty("Count")]
        public long Count { get; set; }

        [JsonProperty("IsSelected")]
        public bool IsSelected { get; set; }

        [JsonProperty("Label")]
        public string Label { get; set; }

        [JsonProperty("DisplayLabel", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayLabel { get; set; }
    }

    public partial class Query
    {
        [JsonProperty("FacetFilter")]
        public string FacetFilter { get; set; }

        [JsonProperty("ForceSearch")]
        public bool ForceSearch { get; set; }

        [JsonProperty("Page")]
        public long Page { get; set; }

        [JsonProperty("QueryGuid")]
        public Guid QueryGuid { get; set; }

        [JsonProperty("QueryString")]
        public string QueryString { get; set; }

        [JsonProperty("ResultSize")]
        public long ResultSize { get; set; }

        [JsonProperty("ScenarioCode")]
        public string ScenarioCode { get; set; }

        [JsonProperty("ScenarioDisplayMode")]
        public string ScenarioDisplayMode { get; set; }

        [JsonProperty("SearchTerms")]
        public string SearchTerms { get; set; }

        [JsonProperty("SortField")]
        public object SortField { get; set; }

        [JsonProperty("SortOrder")]
        public long SortOrder { get; set; }

        [JsonProperty("TemplateParams")]
        public TemplateParams TemplateParams { get; set; }

        [JsonProperty("UseSpellChecking")]
        public object UseSpellChecking { get; set; }
    }

    public partial class TemplateParams
    {
        [JsonProperty("Scenario")]
        public string Scenario { get; set; }

        [JsonProperty("Scope")]
        public string Scope { get; set; }

        [JsonProperty("Size")]
        public object Size { get; set; }

        [JsonProperty("Source")]
        public string Source { get; set; }

        [JsonProperty("Support")]
        public string Support { get; set; }
    }
    public class Test
    {
        [JsonProperty("AbsoluteUri")]

        public Uri[] AbsoluteUri { get; set; }
    }
    public partial class FieldList
    {
        public string[] Title { get; set; }
        public bool IsInscriptionEvent { get; set; } = false;
        public string[] TypeOfDocument_exact { get; set; }
        public string[] Author { get; set; }

        public string[] Author_exact { get; set; }

        public string[] Funds { get; set; }

        public string[] Ean { get; set; }
        public string[] Date { get; set; }

        public Uri[] ThumbSmall { get; set; }

        public string[] Language { get; set; }

        public int[] NumberOfDigitalNotices { get; set; }

        public string[] DigitalReadyIsEntryPoint { get; set; }

        public string UrlViewerDR { get; set; }

        public string[] ZIPURL { get; set; } = null;

        [JsonProperty("eventCategory_multi_store")]

        public string[] eventLabel_store { get; set; } = null;

        [JsonProperty("CategoryColor_store")]

        public string[] CategoryColor_store { get; set; } = null;

        [JsonProperty("CategoryTextColor_store")]

        public string[] CategoryTextColor_store { get; set; } = null;


        [JsonProperty("imageUrl_store")]

        public string[] imageUrl_store { get; set; } = null;



        [JsonProperty("fullDescription_store")]

        public string[] fullDescription_store { get; set; } = null;


        public bool HasZipUrl
        {
            get { return ZIPURL != null && ZIPURL[0] != null; }
        }

        private string getZipUri { get; set; } = "";
        public string GetZipUri
        {
            get {
                try {
                    if (HasZipUrl)
                    {
                        if (getZipUri == "")
                        {
                            Regex regex = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
                            Match match = regex.Match(ZIPURL[0]);
                            getZipUri = match.Groups[1].ToString();
                            return getZipUri;
                        }
                        else
                        {
                            return getZipUri;
                        }
                        
                    }
                    else
                    {
                        return getZipUri;
                    }
                    }
                catch
                {
                    this.ZIPURL[0] = null ;
                    return getZipUri;
                }
            }
            set
            {
                this.getZipUri = value;
            }
        }
        private string getZipLabel { get; set; } = "";
        public string GetZipLabel
        {
            get
            {
                try
                {
                    if (HasZipUrl)
                    {
                        if (getZipLabel == "")
                        {
                            Regex regex = new Regex("title\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
                            Match match = regex.Match(ZIPURL[0]);
                            getZipLabel = match.Groups[1].ToString();
                            return getZipLabel;
                        }
                        else
                        {
                            return getZipLabel;
                        }

                    }
                    else
                    {
                        return getZipLabel;
                    }
                }
                catch
                {
                    this.ZIPURL[0] = null;
                    return getZipLabel;
                }
            }
            set
            {
                this.getZipLabel = value;
            }
        }

        public string CroppedTitle
        {
            get {
                return System.Net.WebUtility.HtmlDecode(((Title[0].Length > 42) ? string.Format("{0}...", Title[0].Substring(0, 42)) : Title[0]));
            }
        }

        public string Image
        {
            get {
                if (ThumbMedium != null && ThumbMedium[0] != null)
                    return string.Format("{0}", ThumbMedium[0]);
                else if (ThumbSmall != null && ThumbSmall[0] != null)
                    return string.Format("{0}", ThumbSmall[0]);
                else
                    return string.Format("{0}", "https://user-images.githubusercontent.com/24848110/33519396-7e56363c-d79d-11e7-969b-09782f5ccbab.png");
            }
        }


        private bool haveImage { get; set; } = false;
        public bool HaveImage { 
            get { return this.haveImage; }
            set {
                this.haveImage = value;
                this.ReverseHaveImage = !value;   
                } 

        }

        public bool ReverseHaveImage { get; set; } = true;

        public string shortDesc
        {
            get {
                var type    = TypeOfDocument_exact != null ? TypeOfDocument_exact[0] : "";
                var author  = Author != null ? Author[0] : "";
                var tdate   = Date != null ? Date[0] : "";
                var final = "";
                if (type != "")
                    final += type;
                if (author != "" && author != ",")
                    final +=  " | " + author;
                if (tdate != "")
                    final += " | " + tdate;
                return System.Net.WebUtility.HtmlDecode(final);

            }
        }
        [JsonProperty("SubjectLocation")]
        public string[] SubjectLocation { get; set; }
        [JsonProperty("startDate_date")]
        public string[] DateStart_idx { get; set; }
        [JsonProperty("endDate_date")]
        public string[] DateEnd_idx { get; set; }
        [JsonProperty("DateDisplay_store")]
        public string[] DateDisplay { get; set; }
        public string DateTime_String
        { get
            {
                string ret;
                try
                {
                DateTime DateTimeStart = DateTime.ParseExact(DateStart_idx[0], "yyyy-MM-ddTHH:mm:ss", null);
                DateTime DateTimeEnd = DateTime.ParseExact(DateEnd_idx[0], "yyyy-MM-ddTHH:mm:ss", null);
                if (this.DateStart_idx != null)
                {
                    string DateStart = DateTimeStart.Date.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                    string HoursStart = DateTimeStart.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
                    string DateEnd = DateTimeEnd.Date.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                    string HoursEnd = DateTimeEnd.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);

                    if (DateTimeStart.Date != DateTimeEnd.Date)
                    {
                            if (HoursStart == HoursEnd)
                            {
                                ret = String.Format(ApplicationResource.DateToDate, DateStart, DateEnd, HoursStart, HoursEnd);
                            }
                            else
                            {
                                ret = String.Format(ApplicationResource.DateToDateHours, DateStart, DateEnd, HoursStart, HoursEnd);
                            }
                        
                    }
                    else
                    {
                        ret = String.Format(ApplicationResource.DateToDateShort, DateStart, HoursStart, HoursEnd);
                    }
                }
                else
                {
                    ret = "";
                }
                }
                catch (Exception)
                {
                    ret = "";
                }
                return ret;

            }
        }

        public bool HaveDateTime { 
            get{
                return DateDisplay != null;
            } 
        }

        public bool HaveLocation
        {
            get
            {
                return (SubjectLocation != null);
            }
        }

        public bool HaveInformation
        {
            get
            {
                return HaveDateTime || HaveLocation;
            }
        }

        public Uri[] ThumbMedium { get; set; }

        public Uri[] ThumbLarge { get; set; }

        public String[] SubjectTopic_exact { get; set; }

        public String SubjectTopicFirstUpper { get {
                if (SubjectTopic_exact != null)
                {
                    return SubjectTopic_exact[0].ToUpper();
                }
                else
                {
                    return "";
                }
                
            } }

        public String[] Identifier { get; set; } 
    }

    public class DisplayValues
    {
        public string AuthorDate { get; set; }

        public string Star { get; set; }

        public bool HaveStar
        {
            get
            {
                return Star != null && Star != "";
            }
        }


        public bool DisplayStar { get; set; }

        public string Desc { get; set; }

        public bool SeekForHoldings { get; set; }

        public SearchLibraryResult Library { get; set; } = null;

        public bool HasLibrary { get { return Library != null; } }
    }

    public partial class Result
    {
        [JsonProperty("CustomResult")]
        public string CustomResult { get; set; }

        [JsonProperty("FriendlyUrl")]
        public Uri FriendlyUrl { get; set; }

        [JsonProperty("GroupedResults")]
        public object[] GroupedResults { get; set; }

        [JsonProperty("HasDigitalReady")]
        public bool HasDigitalReady { get; set; }

        private bool hasViewerDr { get; set; } = false;

        public bool HasViewerDr
        {
            get
            {
                if (HasDigitalReady)
                {
                    this.hasViewerDr = true;
                }
                else
                {
                    if (FieldList.DigitalReadyIsEntryPoint != null && Convert.ToInt32(FieldList.DigitalReadyIsEntryPoint[0]) > 0)
                    {   
                        this.hasViewerDr = true;
                        HasDigitalReady = true;
                    }
                    else if (FieldList.NumberOfDigitalNotices != null && FieldList.NumberOfDigitalNotices[0] > 0)
                    {
                        this.hasViewerDr = true;
                        HasDigitalReady = true;
                    }
                }
                return hasViewerDr;
            }
            set
            {
                this.hasViewerDr = value;
            }
        }

        [JsonProperty("HasPrimaryDocs")]
        public bool HasPrimaryDocs { get; set; }

        [JsonProperty("HighLights")]
        public Suggestions HighLights { get; set; }

        [JsonProperty("LinkedResultsTwin")]
        public LinkedResultsTwin LinkedResultsTwin { get; set; }

        [JsonProperty("PrimaryDocs")]
        public PrimaryDocs[] PrimaryDocs { get; set; }

        [JsonProperty("Resource")]
        public Resource Resource { get; set; }

        [JsonProperty("SeekForHoldings")]
        public bool SeekForHoldings { get; set; }

        [JsonProperty("TemplateLabel")]
        public string TemplateLabel { get; set; }

        [JsonProperty("WorksKeyResults")]
        public object[] WorksKeyResults { get; set; }

        [JsonProperty("FieldList")]
        public FieldList FieldList { get; set; }

        public DisplayValues DisplayValues { get; set; } = new DisplayValues();

        public bool HaveDisplayValues { get { return DisplayValues != null; } }


        public Result Clone()
        {
            return new Result
            {
                CustomResult = this.CustomResult,
                FriendlyUrl = this.FriendlyUrl,
                GroupedResults = this.GroupedResults,
                HasDigitalReady = this.HasDigitalReady,
                HasPrimaryDocs = this.HasPrimaryDocs,
                HighLights = this.HighLights,
                LinkedResultsTwin = this.LinkedResultsTwin,
                PrimaryDocs = this.PrimaryDocs,
                Resource = this.Resource,
                SeekForHoldings = this.SeekForHoldings,
                TemplateLabel = this.TemplateLabel,
                WorksKeyResults = this.WorksKeyResults,
                FieldList = this.FieldList,
                DisplayValues = this.DisplayValues,
            };
        }
    }
    public partial class Suggestions
    {
    }

    public partial class LinkedResultsTwin
    {
        [JsonProperty("ListFormat")]
        public object[] ListFormat { get; set; }

        [JsonProperty("Notices")]
        public object[] Notices { get; set; }
    }

    public partial class PrimaryDocs
    {
        [JsonProperty("GlyphClass")]
        public string GlyphClass { get; set; }

        [JsonProperty("Label")]
        public string Label { get; set; }

        [JsonProperty("Link")]
        public string Link { get; set; } = null;

        public Uri ViewerUri { get; set; } = new Uri("https://publiclibrary.syracuse.cloud/Default/digital-viewer/c-516522");

        public bool HasLink
        {
            get { return this.Link != null; }
        }


        [JsonProperty("ResourceKey")]
        public string ResourceKey { get; set; }

    }

    public partial class Resource
    {
        [JsonProperty("AvNt")]
        public long AvNt { get; set; }

        [JsonProperty("BlogPostCategories")]
        public object[] BlogPostCategories { get; set; }

        [JsonProperty("BlogPostTags")]
        public object[] BlogPostTags { get; set; }

        [JsonProperty("Cmts")]
        public object[] Cmts { get; set; }

        [JsonProperty("CmtsCt")]
        public long CmtsCt { get; set; }

        [JsonProperty("Culture", NullValueHandling = NullValueHandling.Ignore)]
        public long? Culture { get; set; }

        [JsonProperty("Dt", NullValueHandling = NullValueHandling.Ignore)]
        public string Dt { get; set; }

        [JsonProperty("Frmt")]
        public string Frmt { get; set; }

        [JsonProperty("IICUB")]
        public bool Iicub { get; set; }

        [JsonProperty("Id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("RscBase")]
        public string RscBase { get; set; }

        [JsonProperty("RscId")]
        public string RscId { get; set; }

        [JsonProperty("RscUid")]
        public long RscUid { get; set; }

        [JsonProperty("Site")]
        public long Site { get; set; }

        [JsonProperty("Status")]
        public long Status { get; set; }

        public string TextStatus
        {
            get
            {
                try
                {
                    return HtmlViewDisponibility.Contains("available indicator") ? ApplicationResource.DisponibilityYes : ApplicationResource.DisponibilityNo;
                }
                catch (Exception)
                {
                    return ApplicationResource.DisponibilityNo;
                }
                
            }
        }

        public string ColorStatus
        {
            get
            {
                try
                {
                    return HtmlViewDisponibility.Contains("available indicator") ? "#97c67d" : "#fdc76b";
                }
                catch (Exception e)
                {
                    return "#fdc76b";
                }
                
            }
        }

        private bool hasViewDisponibility { get; set; } = false;
        public bool HasViewDisponibility
        {
            get
            {
                return this.hasViewDisponibility;
            }
            set
            {
                this.hasViewDisponibility = value;
            }
        }

        private string htmlViewDisponibility { get; set; }
        public string HtmlViewDisponibility
        {
            get { return this.htmlViewDisponibility; }
            set { 
                this.htmlViewDisponibility = value;
                try
                {
                    if (HtmlViewDisponibility != null && HtmlViewDisponibility != "" )
                    {
                        this.HasViewDisponibility = true;
                    }
                    else
                    {
                        this.hasViewDisponibility = false;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("HtmlViewDisponibility Exception ");
                    this.hasViewDisponibility = false;
                }
            }

        }

        [JsonProperty("Tags")]
        public object[] Tags { get; set; }

        [JsonProperty("Ttl")]
        public string Ttl { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Desc", NullValueHandling = NullValueHandling.Ignore)]
        public string Desc { get; set; }

        [JsonProperty("Subj", NullValueHandling = NullValueHandling.Ignore)]
        public string Subj { get; set; }
    }

    public partial class ScenarioDisplayMode
    {
        [JsonProperty("Culture")]
        public long Culture { get; set; }

        [JsonProperty("DisplayCode")]
        public string DisplayCode { get; set; }

        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("IsDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("LibelleAffichageCF")]
        public string LibelleAffichageCf { get; set; }

        [JsonProperty("PageSize")]
        public string PageSize { get; set; }

        [JsonProperty("Site")]
        public long Site { get; set; }

        [JsonProperty("SortOrder")]
        public long SortOrder { get; set; }
    }

    public partial class SearchInfo
    {
        [JsonProperty("AloesSites")]
        public object[] AloesSites { get; set; }

        [JsonProperty("CanUseDsi")]
        public bool CanUseDsi { get; set; }

        [JsonProperty("DetailMode")]
        public bool DetailMode { get; set; }

        [JsonProperty("ExportParamSets")]
        public ExportParamSet[] ExportParamSets { get; set; }

        [JsonProperty("FacetListInfo")]
        public object[] FacetListInfo { get; set; }

        [JsonProperty("GridFilters")]
        public object[] GridFilters { get; set; }

        [JsonProperty("GroupingField")]
        public string GroupingField { get; set; }

        [JsonProperty("NBResults")]
        public long NbResults { get; set; }

        [JsonProperty("Page")]
        public long Page { get; set; }

        [JsonProperty("PageMax")]
        public long PageMax { get; set; }

        [JsonProperty("PageSizeResult")]
        public long[] PageSizeResult { get; set; }

        [JsonProperty("Pagination")]
        public Pagination[] Pagination { get; set; }

        [JsonProperty("ScenarioType")]
        public long ScenarioType { get; set; }

        [JsonProperty("SearchTime")]
        public long SearchTime { get; set; }

        [JsonProperty("SolrInfo")]
        public SolrInfo SolrInfo { get; set; }

        [JsonProperty("TotalTime")]
        public long TotalTime { get; set; }
    }

    public partial class ExportParamSet
    {
        [JsonProperty("Culture")]
        public long Culture { get; set; }

        [JsonProperty("ExportAssembly")]
        public ExportAssembly ExportAssembly { get; set; }

        [JsonProperty("ExportAssemblyId")]
        public long ExportAssemblyId { get; set; }

        [JsonProperty("ExportParams")]
        public ExportParam[] ExportParams { get; set; }

        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Site")]
        public long Site { get; set; }

        [JsonProperty("SortOrder")]
        public long SortOrder { get; set; }
    }

    public partial class ExportAssembly
    {
        [JsonProperty("AssemblyName")]
        public string AssemblyName { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Id")]
        public long Id { get; set; }
    }

    public partial class ExportParam
    {
        [JsonProperty("Culture")]
        public long Culture { get; set; }

        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Site")]
        public long Site { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }
    }

    public partial class Pagination
    {
        [JsonProperty("Type")]
        public long Type { get; set; }

        [JsonProperty("Value")]
        public long Value { get; set; }
    }

    public partial class SolrInfo
    {
        [JsonProperty("SolrInitialization")]
        public string SolrInitialization { get; set; }
    }

    public partial class Sort
    {
        [JsonProperty("Culture")]
        public long Culture { get; set; }

        [JsonProperty("DefaultOrder")]
        public long DefaultOrder { get; set; }

        [JsonProperty("Field")]
        public string Field { get; set; }

        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("IsDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("Label")]
        public string Label { get; set; }

        [JsonProperty("Site")]
        public long Site { get; set; }

        [JsonProperty("SortOrder")]
        public long SortOrder { get; set; }
    }

    public partial class SpellChecking
    {
        [JsonProperty("Collation")]
        public string Collation { get; set; }

        [JsonProperty("Suggestions")]
        public Suggestions Suggestions { get; set; }
    }
}

