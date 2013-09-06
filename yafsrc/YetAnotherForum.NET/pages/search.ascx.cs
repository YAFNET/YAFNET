/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core.Data;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces.Data;

    using nStuff.UpdateControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Search Page.
    /// </summary>
    public partial class search : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The _search what cleaned.
        /// </summary>
        private string _searchWhatCleaned;

        /// <summary>
        ///   The _search who cleaned.
        /// </summary>
        private string _searchWhoCleaned;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "search" /> class. 
        ///   The search page constructor.
        /// </summary>
        public search()
            : base("SEARCH")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets HighlightSearchWords.
        /// </summary>
        [CanBeNull]
        protected List<string> HighlightSearchWords
        {
            get
            {
                if (this.ViewState["HighlightWords"] == null)
                {
                    this.ViewState["HighlightWords"] = new List<string>();
                }

                return this.ViewState["HighlightWords"] as List<string>;
            }

            set
            {
                this.ViewState["HighlightWords"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether SearchHandled.
        /// </summary>
        protected bool SearchHandled { get; set; }

        /// <summary>
        ///   Gets or sets SearchWhatCleaned.
        /// </summary>
        protected string SearchWhatCleaned
        {
            get
            {
                return this._searchWhatCleaned
                       ?? (this._searchWhatCleaned = this.txtSearchStringWhat.Text.Trim().RemoveMultipleWhitespace().RemoveMultipleSingleQuotes());
            }

            set
            {
                this._searchWhatCleaned = value;
            }
        }

        /// <summary>
        ///   Gets or sets SearchWhoCleaned.
        /// </summary>
        protected string SearchWhoCleaned
        {
            get
            {
                return this._searchWhoCleaned
                       ?? (this._searchWhoCleaned = this.txtSearchStringFromWho.Text.Trim().RemoveMultipleWhitespace().RemoveMultipleSingleQuotes());
            }

            set
            {
                this._searchWhoCleaned = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // Setup Syntax Highlight JS
            YafContext.Current.PageElements.RegisterJsResourceInclude(
                "syntaxhighlighter", "js/jquery.syntaxhighligher.js");
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.syntaxhighligher.css");
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "syntaxhighlighterjs", JavaScriptBlocks.SyntaxHighlightLoadJs);

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles btn Click eventn for external search button 1
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BtnExtSearch1_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsValidSearchRequest())
            {
                return;
            }

            if (this.Get<YafBoardSettings>().ExternalSearchInNewWindow)
            {
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "openBrowserTabJs", "window.open('{0}', '', '');".FormatWith(this.GetExtSearchLink(1)));
            }
            else
            {
                this.Response.Redirect(this.GetExtSearchLink(1));
            }
        }

        /// <summary>
        /// Handles btn Click eventn for external search button 2
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BtnExtSearch2_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsValidSearchRequest())
            {
                return;
            }

            if (this.Get<YafBoardSettings>().ExternalSearchInNewWindow)
            {
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "openBrowserTabJs", "window.open('{0}', '', '');".FormatWith(this.GetExtSearchLink(2)));
            }
            else
            {
                this.Response.Redirect(this.GetExtSearchLink(2));
            }
        }

        /// <summary>
        /// The is search text too large.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The TRUE is a search text too large.
        /// </returns>
        protected bool IsSearchTextTooLarge([NotNull] string text)
        {
            return text.Length > this.Get<YafBoardSettings>().SearchStringMaxLength;
        }

        /// <summary>
        /// Check if the  search text too small.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// Returns if the Text is to small or not
        /// </returns>
        protected bool IsSearchTextTooSmall([NotNull] string text)
        {
            return text.Length < this.Get<YafBoardSettings>().SearchStringMinLength;
        }

        /// <summary>
        /// The is valid search request.
        /// </summary>
        /// <returns>
        /// Returns if its a valid search request.
        /// </returns>
        protected bool IsValidSearchRequest()
        {
            bool whatValid = this.IsValidSearchText(this.SearchWhatCleaned.Trim());
            bool whoValid = !string.IsNullOrEmpty(this.SearchWhoCleaned.Trim());

            // they are both valid...
            if (whoValid && whatValid)
            {
                return true;
            }

            if (!whoValid && whatValid)
            {
                // makes sure no value is used...
                this.SearchWhoCleaned = string.Empty;

                // valid search...
                return true;
            }

            // !whatValid is always true... could be removed but left for clarity.
            if (whoValid && !whatValid)
            {
                // make sure no value is used...
                this.SearchWhatCleaned = string.Empty;

                // valid search...
                return true;
            }

            bool searchTooSmall = false;
            bool searchTooLarge = false;

            if (this.SearchWhoCleaned.Trim().IsNotSet() && this.IsSearchTextTooSmall(this.SearchWhatCleaned))
            {
                searchTooSmall = true;
            }
            else if (this.SearchWhatCleaned.Trim().IsNotSet() && this.SearchWhoCleaned.Trim().IsNotSet())
            {
                searchTooSmall = true;
            }
            else if (this.SearchWhoCleaned.Trim().IsNotSet() && this.IsSearchTextTooLarge(this.SearchWhatCleaned.Trim()))
            {
                searchTooLarge = true;
            }
            else if (this.SearchWhatCleaned.Trim().IsNotSet()
                     && this.IsSearchTextTooLarge(this.SearchWhoCleaned.Trim()))
            {
                searchTooLarge = true;
            }

            // search may not be valid for some reason...
            if (searchTooSmall)
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted(
                        "SEARCH_CRITERIA_ERROR_MIN", this.Get<YafBoardSettings>().SearchStringMinLength));
            }
            else if (searchTooLarge)
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted(
                        "SEARCH_CRITERIA_ERROR_MAX", this.Get<YafBoardSettings>().SearchStringMaxLength));
            }

            return false;
        }

        /// <summary>
        /// The is valid search text.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        /// <returns>
        /// Returns if the search text is valid.
        /// </returns>
        protected bool IsValidSearchText([NotNull] string searchText)
        {
            return searchText.IsSet() && !this.IsSearchTextTooSmall(searchText)
                   && !this.IsSearchTextTooLarge(searchText)
                   && Regex.IsMatch(searchText, this.Get<YafBoardSettings>().SearchStringPattern);
        }

        /// <summary>
        /// The on update history navigate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="nStuff.UpdateControls.HistoryEventArgs"/> instance containing the event data.</param>
        protected void OnUpdateHistoryNavigate([NotNull] object sender, [NotNull] HistoryEventArgs e)
        {
            int pageNumber, pageSize;

            string[] pagerData = e.EntryName.Split('|');

            if (pagerData.Length < 2 || !int.TryParse(pagerData[0], out pageNumber)
                || !int.TryParse(pagerData[1], out pageSize) || this.Get<IYafSession>().SearchData == null)
            {
                return;
            }

            // use existing page...
            this.Pager.CurrentPageIndex = pageNumber;

            // and existing page size...
            this.Pager.PageSize = pageSize;

            // count...
            this.Pager.Count = this.Get<IYafSession>().SearchData.AsEnumerable().Count();

            // bind existing search
            this.SearchBindData(false);

            // use existing search data...
            this.SearchUpdatePanel.Update();
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
            this.btnSearch.Text = "{0}".FormatWith(this.GetText("btnsearch"));

            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ExternalSearchPermissions))
            {
                // vzrus: If an exteranl search only - it should be disabled. YAF doesn't have a forum id as a token in post links. 
                if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().SearchPermissions))
                {
                    this.listForum.Enabled = false;
                }

                if (!string.IsNullOrEmpty(this.Get<YafBoardSettings>().SearchEngine1)
                    && (!string.IsNullOrEmpty(this.Get<YafBoardSettings>().SearchEngine1Parameters)))
                {
                    this.btnSearchExt1.Visible = true;
                    this.btnSearchExt1.Text =
                        this.btnSearchExt1.ToolTip =
                        this.GetText("btnsearch_external").FormatWith(
                            this.Get<YafBoardSettings>().SearchEngine1Parameters.Split('^')[0]);
                }

                if (!string.IsNullOrEmpty(this.Get<YafBoardSettings>().SearchEngine2)
                    && (!string.IsNullOrEmpty(this.Get<YafBoardSettings>().SearchEngine2Parameters)))
                {
                    this.btnSearchExt2.Visible = true;
                    this.btnSearchExt2.Text =
                        this.btnSearchExt2.ToolTip =
                        this.GetText("btnsearch_external").FormatWith(
                            this.Get<YafBoardSettings>().SearchEngine2Parameters.Split('^')[0]);
                }
            }

            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().SearchPermissions))
            {
                this.btnSearch.Visible = true;
            }

            // Load result dropdown
            this.listResInPage.Items.Add(new ListItem(this.GetText("result5"), "5"));
            this.listResInPage.Items.Add(new ListItem(this.GetText("result10"), "10"));
            this.listResInPage.Items.Add(new ListItem(this.GetText("result25"), "25"));
            this.listResInPage.Items.Add(new ListItem(this.GetText("result50"), "50"));

            // Load searchwhere dropdown
            // listSearchWhere.Items.Add( new ListItem( GetText( "posts" ), "0" ) );
            // listSearchWhere.Items.Add( new ListItem( GetText( "postedby" ), "1" ) );

            // Load listSearchFromWho dropdown
            this.listSearchFromWho.Items.Add(new ListItem(this.GetText("match_exact"), "2"));
            this.listSearchFromWho.Items.Add(new ListItem(this.GetText("match_any"), "1"));
            this.listSearchFromWho.Items.Add(new ListItem(this.GetText("match_all"), "0"));

            // Load listSearchWhat dropdown
            this.listSearchWhat.Items.Add(new ListItem(this.GetText("match_exact"), "2"));
            this.listSearchWhat.Items.Add(new ListItem(this.GetText("match_any"), "1"));
            this.listSearchWhat.Items.Add(new ListItem(this.GetText("match_all"), "0"));

            this.listSearchFromWho.SelectedIndex = 0;
            this.listSearchWhat.SelectedIndex = 2;

            // Load forum's combo
            // listForum.Items.Add( new ListItem( GetText( "allforums" ), "-1" ) );
            // DataTable dt = YAF.Classes.Data.DB.forum_listread( PageContext.PageBoardID, PageContext.PageUserID, null, null, YafContext.Current.BoardSettings.UseStyledNicks );

            // int nOldCat = 0;
            // for ( int i = 0; i < dt.Rows.Count; i++ )
            // {
            // DataRow row = dt.Rows [i];
            // if ( ( int ) row ["CategoryID"] != nOldCat )
            // {
            // nOldCat = ( int ) row ["CategoryID"];
            // listForum.Items.Add( new ListItem( ( string ) row ["Category"], "-1" ) );
            // }
            // listForum.Items.Add( new ListItem( " - " + ( string ) row ["Forum"], row ["ForumID"].ToString() ) );
            // }
            this.LoadingModal.Title = this.GetText("LOADING");
            this.LoadingModal.MessageText = this.GetText("LOADING_SEARCH");
            this.LoadingModal.Icon = YafForumInfo.GetURLToResource("images/loader.gif");

            this.Page.Form.DefaultButton = this.btnSearch.UniqueID;

            this.listForum.DataSource = LegacyDb.forum_listall_sorted(
                this.PageContext.PageBoardID, this.PageContext.PageUserID);
            this.listForum.DataValueField = "ForumID";
            this.listForum.DataTextField = "Title";
            this.listForum.DataBind();
            this.listForum.Items.Insert(0, new ListItem(this.GetText("allforums"), "0"));

            bool doSearch = false;

            string searchString = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("search");
            if (searchString.IsSet() && searchString.Length < 50)
            {
                this.txtSearchStringWhat.Text = searchString;
                doSearch = true;
            }

            string forumString = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("forum");

            if (searchString.IsSet() && this.listForum.Enabled)
            {
                try
                {
                    this.listForum.SelectedValue = forumString;
                }
                catch (Exception)
                {
                    this.listForum.SelectedValue = "0";
                }
            }

            var postedBy = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("postedby");

            if (postedBy.IsSet() && postedBy.Length < 50)
            {
                this.txtSearchStringFromWho.Text = postedBy;
                doSearch = true;
            }

            // set the search box size via the max settings in the boardsettings.
            if (this.Get<YafBoardSettings>().SearchStringMaxLength > 0)
            {
                this.txtSearchStringWhat.MaxLength = this.Get<YafBoardSettings>().SearchStringMaxLength;
                this.txtSearchStringFromWho.MaxLength = this.Get<YafBoardSettings>().SearchStringMaxLength;
            }

            if (doSearch)
            {
                this.SearchBindData(true);
            }
        }

        /// <summary>
        /// The pager_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.SmartScroller1.RegisterStartupReset();
            this.SearchBindData(false);
        }

        /// <summary>
        /// The search res_ item data bound.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void SearchRes_ItemDataBound([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            var cell = (HtmlTableCell)e.Item.FindControl("CounterCol");

            if (cell == null)
            {
                return;
            }

            string messageID = cell.InnerText;
            int rowCount = e.Item.ItemIndex + 1 + (this.Pager.CurrentPageIndex * this.Pager.PageSize);
            cell.InnerHtml = "<a href=\"{1}\">{0}</a><a href=\"{1}\">".FormatWith(
                rowCount, YafBuildLink.GetLink(ForumPages.posts, "m={0}#{0}", messageID));
        }

        /// <summary>
        /// The btn search_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnSearch_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.SearchUpdatePanel.Visible = true;
            this.SearchBindData(true);
        }

        /// <summary>
        /// A method to get ready external search link.
        /// </summary>
        /// <param name="i">
        /// A Command option.
        /// </param>
        /// <returns>
        /// An external search link with already inserted searchparameters.
        /// </returns>
        [NotNull]
        private string GetExtSearchLink(int i)
        {
            string searchEngine = string.Empty;
            string searchParams = string.Empty;

            switch (i)
            {
                case 1:
                    searchEngine = this.Get<YafBoardSettings>().SearchEngine1;
                    searchParams = this.Get<YafBoardSettings>().SearchEngine1Parameters;
                    break;
                case 2:
                    searchEngine = this.Get<YafBoardSettings>().SearchEngine2;
                    searchParams = this.Get<YafBoardSettings>().SearchEngine2Parameters;
                    break;
            }

            return this.TransformExtSearchLink(searchEngine, searchParams);
        }

        /// <summary>
        /// Replaces placeholder in a refined parameter expression and inserts it in the search engine string
        /// </summary>
        /// <param name="searchParamsOptArrayj">
        /// A parameter string
        /// </param>
        /// <param name="searchEngine">
        /// An output Search engine String
        /// </param>
        /// <param name="searchParamsDefArraySep">
        /// A search word separator
        /// </param>
        private void MatchParameterHelper(
            [NotNull] ref string searchParamsOptArrayj,
            [NotNull] ref string searchEngine,
            [NotNull] string searchParamsDefArraySep)
        {
            searchParamsOptArrayj = searchParamsOptArrayj.Replace(
                "{Word}",
                this.txtSearchStringWhat.Text
                +
                (!string.IsNullOrEmpty(this.txtSearchStringFromWho.Text)
                     ? " " + this.txtSearchStringFromWho.Text
                     : string.Empty));

            // Add "+"  between words
            searchParamsOptArrayj = searchParamsOptArrayj.Replace(" ", searchParamsDefArraySep);
            searchParamsOptArrayj = searchParamsOptArrayj.Replace(
                searchParamsDefArraySep + searchParamsDefArraySep, searchParamsDefArraySep);

            // Replace here parameters in main string
            searchEngine = searchEngine.Replace(searchParamsOptArrayj.Split('=')[0] + "=", searchParamsOptArrayj);
        }

        /// <summary>
        /// The search bind data.
        /// </summary>
        /// <param name="newSearch">
        /// The new search.
        /// </param>
        private void SearchBindData(bool newSearch)
        {
#if (!DEBUG)
            try
            {
#endif
            if (newSearch && !this.IsValidSearchRequest())
            {
                return;
            }

            if (newSearch || this.Get<IYafSession>().SearchData == null)
            {
                var sw = (SearchWhatFlags)Enum.Parse(typeof(SearchWhatFlags), this.listSearchWhat.SelectedValue);
                var sfw = (SearchWhatFlags)Enum.Parse(typeof(SearchWhatFlags), this.listSearchFromWho.SelectedValue);
                int forumId = int.Parse(this.listForum.SelectedValue);

                var context = new CompleteSearchContext(
                    this.SearchWhatCleaned,
                    this.SearchWhoCleaned,
                    sfw,
                    sw,
                    this.PageContext.PageUserID,
                    this.PageContext.PageBoardID,
                    this.Get<YafBoardSettings>().ReturnSearchMax,
                    this.Get<YafBoardSettings>().UseFullTextSearch,
                    this.Get<YafBoardSettings>().EnableDisplayName,
                    forumId);

                var searchResults = this.Get<ISearch>().Execute(context).ToArray();

                if (newSearch)
                {
                    // setup highlighting
                    this.SetupHighlightWords(sw);
                }

                this.Get<IYafSession>().SearchData = searchResults;

                this.Pager.CurrentPageIndex = 0;
                this.Pager.PageSize = int.Parse(this.listResInPage.SelectedValue);
                this.Pager.Count = searchResults.AsEnumerable().Count();

                bool areResults = this.Pager.Count > 0;

                this.SearchRes.Visible = areResults;
                this.NoResults.Visible = !areResults;
            }

            this.UpdateHistory.AddEntry("{0}|{1}".FormatWith(this.Pager.CurrentPageIndex, this.Pager.PageSize));

            var pagedData = this.Get<IYafSession>().SearchData.GetPaged(this.Pager);

            // only load required messages
            this.Get<YafDbBroker>().LoadMessageText(pagedData);

            this.SearchRes.DataSource = pagedData;
            this.SearchRes.DataBind();
#if (!DEBUG)
            }
            catch (Exception x)
            {
                this.Logger.Log(this.PageContext.PageUserID, this, x);

                this.PageContext.AddLoadMessage(
                    this.PageContext.IsAdmin ? "{0}".FormatWith(x) : "An error occurred while searching.");
            }
#endif
        }

        /// <summary>
        /// Sets up highlighting of the search keywords.
        /// </summary>
        /// <param name="searchFlags">
        /// The search Flags.
        /// </param>
        private void SetupHighlightWords(SearchWhatFlags searchFlags)
        {
            this.HighlightSearchWords.Clear();

            switch (searchFlags)
            {
                case SearchWhatFlags.ExactMatch:
                    this.HighlightSearchWords.Add(this.SearchWhatCleaned);
                    break;
                case SearchWhatFlags.AllWords:
                case SearchWhatFlags.AnyWords:
                    this.HighlightSearchWords.AddRange(this.SearchWhatCleaned.Split(' ').ToList().Where(x => x.IsSet()));
                    break;
            }
        }

        /// <summary>
        /// A search string.
        /// </summary>
        /// <param name="searchEngine">
        /// A search engine template string.
        /// </param>
        /// <param name="searchParams">
        /// A parameters template string
        /// </param>
        /// <returns>
        /// Returns a ready search engine string.
        /// </returns>
        [NotNull]
        private string TransformExtSearchLink([NotNull] string searchEngine, [NotNull] string searchParams)
        {
            searchEngine = searchEngine.Replace("{ResultsPerPage}", this.listResInPage.SelectedValue);

            string url = this.ForumURL.TrimEnd('/').Replace("www.", string.Empty);

            if (Config.IsMojoPortal)
            {
                url = BaseUrlBuilder.BaseUrl.TrimEnd('/').Replace("www.", string.Empty);
            }

            // int forumID = int.Parse(this.listForum.SelectedValue);
            searchEngine = searchEngine.Replace("{Site}", url);

            //// searchEngine = searchEngine.Replace("{Language}", this.PageContext.CultureUser.Substring(0,2));           
            string[] searchParamsDefArray = searchParams.Split('^');

            // some parameters are reserved for future use                  
            // the search engine name (Google)
            // symbol to separate a forum name (?)
            // parameters common delimiter (&)
            // search words common delimiter (+) - searchParamsDefArray[3]
            // Example: "Google^?^&^+^;^AnyWord:as_oq={Word}^AllWords:as_q={Word}^ExactFrase:as_epq={Word}^WithoutWords:as_eq={Word}"
            for (int i = 5; i < searchParamsDefArray.Length; i++)
            {
                // Example: AnyWord:as_oq={Word}^AllWords:as_q={Word}^ExactFrase:as_epq={Word}^WithoutWords:as_eq={Word}              
                string[] searchParamsAggreageArray = searchParamsDefArray[i].Split(':');

                // Parameter Name  like  AnyWord   -    searchParamsAggreageArray[0] - AnyWord
                string[] searchParamsOptArray = searchParamsAggreageArray[1].Split('/');
                for (int j = 0; j < searchParamsOptArray.Length; j++)
                {
                    // Example: as_oq={Word}
                    // Example: text={Word}/wordforms=all 
                    switch (this.listSearchWhat.SelectedValue)
                    {
                        case "0":

                            // "match_all"
                            if (searchParamsAggreageArray[0] == "AllWords")
                            {
                                this.MatchParameterHelper(
                                    ref searchParamsOptArray[j], ref searchEngine, searchParamsDefArray[3]);
                            }

                            break;
                        case "1":

                            // "match_any"
                            if (searchParamsAggreageArray[0] == "AnyWord")
                            {
                                this.MatchParameterHelper(
                                    ref searchParamsOptArray[j], ref searchEngine, searchParamsDefArray[3]);
                            }

                            break;
                        case "2":

                            // "match_exact"
                            if (searchParamsAggreageArray[0] == "ExactFrase")
                            {
                                this.MatchParameterHelper(
                                    ref searchParamsOptArray[j], ref searchEngine, searchParamsDefArray[3]);
                            }

                            break;
                    }
                }
            }

            // Remove all remaining brackets
            while (searchEngine.Contains('{'))
            {
                int startPos = searchEngine.IndexOf('{');
                int endPos = searchEngine.IndexOf('}');
                searchEngine = searchEngine.Remove(startPos, endPos - startPos + 1);
            }

            return searchEngine;
        }

        #endregion
    }
}