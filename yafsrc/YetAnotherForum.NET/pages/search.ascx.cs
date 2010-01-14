/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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
  // YAF.Pages
  using System;
  using System.Data;
  using System.Text.RegularExpressions;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using HistoryEventArgs = nStuff.UpdateControls.HistoryEventArgs;

  /// <summary>
  /// Summary description for topics.
  /// </summary>
  public partial class search : ForumPage
  {
    /// <summary>
    /// The _search handled.
    /// </summary>
    private bool _searchHandled = false;

    /// <summary>
    /// The _search what cleaned.
    /// </summary>
    private string _searchWhatCleaned;

    /// <summary>
    /// The _search who cleaned.
    /// </summary>
    private string _searchWhoCleaned;

    /// <summary>
    /// Initializes a new instance of the <see cref="search"/> class. 
    /// The search page constructor.
    /// </summary>
    public search()
      : base("SEARCH")
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether SearchHandled.
    /// </summary>
    protected bool SearchHandled
    {
      get
      {
        return this._searchHandled;
      }

      set
      {
        this._searchHandled = value;
      }
    }

    /// <summary>
    /// Gets or sets SearchWhatCleaned.
    /// </summary>
    protected string SearchWhatCleaned
    {
      get
      {
        if (this._searchWhatCleaned == null)
        {
          this._searchWhatCleaned = StringHelper.RemoveMultipleSingleQuotes(StringHelper.RemoveMultipleWhitespace(this.txtSearchStringWhat.Text.Trim()));
        }

        return this._searchWhatCleaned;
      }

      set
      {
        this._searchWhatCleaned = value;
      }
    }

    /// <summary>
    /// Gets or sets SearchWhoCleaned.
    /// </summary>
    protected string SearchWhoCleaned
    {
      get
      {
        if (this._searchWhoCleaned == null)
        {
          this._searchWhoCleaned = StringHelper.RemoveMultipleSingleQuotes(StringHelper.RemoveMultipleWhitespace(this.txtSearchStringFromWho.Text.Trim()));
        }

        return this._searchWhoCleaned;
      }

      set
      {
        this._searchWhoCleaned = value;
      }
    }

    /// <summary>
    /// The get server form.
    /// </summary>
    /// <param name="parent">
    /// The parent.
    /// </param>
    /// <returns>
    /// </returns>
    private HtmlForm GetServerForm(ControlCollection parent)
    {
      HtmlForm tmpHtmlForm = null;

      foreach (Control child in parent)
      {
        Type t = child.GetType();
        if (t == typeof(HtmlForm))
        {
          return (HtmlForm) child;
        }

        if (child.HasControls())
        {
          tmpHtmlForm = GetServerForm(child.Controls);
          if (tmpHtmlForm != null && tmpHtmlForm.ClientID != null)
          {
            return tmpHtmlForm;
          }
        }
      }

      return null;
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(GetText("TITLE"), string.Empty);
        this.btnSearch.Text = GetText("btnsearch");

        // Load result dropdown
        this.listResInPage.Items.Add(new ListItem(GetText("result5"), "5"));
        this.listResInPage.Items.Add(new ListItem(GetText("result10"), "10"));
        this.listResInPage.Items.Add(new ListItem(GetText("result25"), "25"));
        this.listResInPage.Items.Add(new ListItem(GetText("result50"), "50"));

        // Load searchwhere dropdown
        // listSearchWhere.Items.Add( new ListItem( GetText( "posts" ), "0" ) );
        // listSearchWhere.Items.Add( new ListItem( GetText( "postedby" ), "1" ) );

        // Load listSearchFromWho dropdown
        this.listSearchFromWho.Items.Add(new ListItem(GetText("match_all"), "0"));
        this.listSearchFromWho.Items.Add(new ListItem(GetText("match_any"), "1"));
        this.listSearchFromWho.Items.Add(new ListItem(GetText("match_exact"), "2"));

        // Load listSearchWhat dropdown
        this.listSearchWhat.Items.Add(new ListItem(GetText("match_all"), "0"));
        this.listSearchWhat.Items.Add(new ListItem(GetText("match_any"), "1"));
        this.listSearchWhat.Items.Add(new ListItem(GetText("match_exact"), "2"));

        this.listSearchFromWho.SelectedIndex = 0;
        this.listSearchWhat.SelectedIndex = 0;

        // Load forum's combo
        // listForum.Items.Add( new ListItem( GetText( "allforums" ), "-1" ) );
        // DataTable dt = YAF.Classes.Data.DB.forum_listread( PageContext.PageBoardID, PageContext.PageUserID, null, null );

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
        this.LoadingModal.Title = GetText("LOADING");

        this.listForum.DataSource = DB.forum_listall_sorted(PageContext.PageBoardID, PageContext.PageUserID);
        this.listForum.DataValueField = "ForumID";
        this.listForum.DataTextField = "Title";
        this.listForum.DataBind();
        this.listForum.Items.Insert(0, new ListItem(GetText("allforums"), "0"));

        bool doSearch = false;

        string searchString = Request.QueryString["search"];
        if (!String.IsNullOrEmpty(searchString) && searchString.Length < 50)
        {
          this.txtSearchStringWhat.Text = searchString;
          doSearch = true;
        }

        string postedBy = Request.QueryString["postedby"];
        if (!String.IsNullOrEmpty(postedBy) && postedBy.Length < 50)
        {
          this.txtSearchStringFromWho.Text = postedBy;
          doSearch = true;
        }

        // set the search box size via the max settings in the boardsettings.
        if (PageContext.BoardSettings.SearchStringMaxLength > 0)
        {
          this.txtSearchStringWhat.MaxLength = PageContext.BoardSettings.SearchStringMaxLength;
          this.txtSearchStringFromWho.MaxLength = PageContext.BoardSettings.SearchStringMaxLength;
        }

        if (doSearch)
        {
          SearchBindData(true);
        }
      }
    }

    /// <summary>
    /// The pager_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Pager_PageChange(object sender, EventArgs e)
    {
      this.SmartScroller1.RegisterStartupReset();
      SearchBindData(false);
    }

    /// <summary>
    /// The search bind data.
    /// </summary>
    /// <param name="newSearch">
    /// The new search.
    /// </param>
    private void SearchBindData(bool newSearch)
    {
      try
      {
        if (newSearch && !IsValidSearchRequest())
        {
          return;
        }
        else if (newSearch || Mession.SearchData == null)
        {
          var sw = (SearchWhatFlags) Enum.Parse(typeof(SearchWhatFlags), this.listSearchWhat.SelectedValue);
          var sfw = (SearchWhatFlags) Enum.Parse(typeof(SearchWhatFlags), this.listSearchFromWho.SelectedValue);
          int forumID = int.Parse(this.listForum.SelectedValue);

          DataTable searchDataTable = DB.GetSearchResult(
            SearchWhatCleaned, 
            SearchWhoCleaned, 
            sfw, 
            sw, 
            forumID, 
            PageContext.PageUserID, 
            PageContext.PageBoardID, 
            PageContext.BoardSettings.ReturnSearchMax, 
            PageContext.BoardSettings.UseFullTextSearch);
          this.Pager.CurrentPageIndex = 0;
          this.Pager.PageSize = int.Parse(this.listResInPage.SelectedValue);
          this.Pager.Count = searchDataTable.DefaultView.Count;
          Mession.SearchData = searchDataTable;

          bool bResults = (searchDataTable.DefaultView.Count > 0) ? true : false;

          this.SearchRes.Visible = bResults;
          this.NoResults.Visible = !bResults;
        }

        var pds = new PagedDataSource();
        pds.AllowPaging = true;
        pds.DataSource = Mession.SearchData.DefaultView;
        pds.PageSize = this.Pager.PageSize;
        pds.CurrentPageIndex = this.Pager.CurrentPageIndex;

        this.UpdateHistory.AddEntry(this.Pager.CurrentPageIndex.ToString() + "|" + this.Pager.PageSize);

        this.SearchRes.DataSource = pds;
        this.SearchRes.DataBind();
      }
      catch (Exception x)
      {
        DB.eventlog_create(PageContext.PageUserID, this, x);
        CreateMail.CreateLogEmail(x);

        if (PageContext.IsAdmin)
        {
          PageContext.AddLoadMessage(string.Format("{0}", x));
        }
        else
        {
          PageContext.AddLoadMessage("An error occured while searching.");
        }
      }
    }

    /// <summary>
    /// The btn search_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchUpdatePanel.Visible = true;
      SearchBindData(true);
    }

    /// <summary>
    /// The search res_ item data bound.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SearchRes_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
      var cell = (HtmlTableCell) e.Item.FindControl("CounterCol");
      if (cell != null)
      {
        string messageID = cell.InnerText;
        int rowCount = e.Item.ItemIndex + 1 + (this.Pager.CurrentPageIndex * this.Pager.PageSize);
        cell.InnerHtml = string.Format("<a href=\"{1}\">{0}</a>", rowCount, YafBuildLink.GetLink(ForumPages.posts, "m={0}#{0}", messageID));
      }
    }

    /// <summary>
    /// The is valid search request.
    /// </summary>
    /// <returns>
    /// The is valid search request.
    /// </returns>
    protected bool IsValidSearchRequest()
    {
      bool whatValid = IsValidSearchText(SearchWhatCleaned);
      bool whoValid = IsValidSearchText(SearchWhoCleaned);

      // they are both valid...
      if (whoValid && whatValid)
      {
        return true;
      }

      if (!whoValid && whatValid)
      {
        // makes sure no value is used...
        SearchWhoCleaned = String.Empty;

        // valid search...
        return true;
      }

      // !whatValid is always true... could be removed but left for clarity.
      if (whoValid && !whatValid)
      {
        // make sure no value is used...
        SearchWhatCleaned = String.Empty;

        // valid search...
        return true;
      }

      bool searchTooSmall = false;
      bool searchTooLarge = false;

      if (String.IsNullOrEmpty(SearchWhoCleaned) && IsSearchTextTooSmall(SearchWhatCleaned))
      {
        searchTooSmall = true;
      }
      else if (String.IsNullOrEmpty(SearchWhatCleaned) && IsSearchTextTooSmall(SearchWhoCleaned))
      {
        searchTooSmall = true;
      }
      else if (String.IsNullOrEmpty(SearchWhoCleaned) && IsSearchTextTooLarge(SearchWhatCleaned))
      {
        searchTooLarge = true;
      }
      else if (String.IsNullOrEmpty(SearchWhatCleaned) && IsSearchTextTooLarge(SearchWhoCleaned))
      {
        searchTooLarge = true;
      }

      // search may not be valid for some reason...
      if (searchTooSmall)
      {
        PageContext.AddLoadMessage(GetTextFormatted("SEARCH_CRITERIA_ERROR_MIN", PageContext.BoardSettings.SearchStringMinLength));
      }
      else if (searchTooLarge)
      {
        PageContext.AddLoadMessage(GetTextFormatted("SEARCH_CRITERIA_ERROR_MAX", PageContext.BoardSettings.SearchStringMaxLength));
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
    /// The is valid search text.
    /// </returns>
    protected bool IsValidSearchText(string searchText)
    {
      return !String.IsNullOrEmpty(searchText) && !IsSearchTextTooSmall(searchText) && !IsSearchTextTooLarge(searchText) &&
             Regex.IsMatch(searchText, PageContext.BoardSettings.SearchStringPattern);
    }

    /// <summary>
    /// The is search text too small.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The is search text too small.
    /// </returns>
    protected bool IsSearchTextTooSmall(string text)
    {
      if (text.Length < PageContext.BoardSettings.SearchStringMinLength)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// The is search text too large.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The is search text too large.
    /// </returns>
    protected bool IsSearchTextTooLarge(string text)
    {
      if (text.Length > PageContext.BoardSettings.SearchStringMaxLength)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// The on update history navigate.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void OnUpdateHistoryNavigate(object sender, HistoryEventArgs e)
    {
      int pageNumber, pageSize;

      string[] pagerData = e.EntryName.Split('|');

      if (pagerData.Length >= 2 && int.TryParse(pagerData[0], out pageNumber) && int.TryParse(pagerData[1], out pageSize) && Mession.SearchData != null)
      {
        // use existing page...
        this.Pager.CurrentPageIndex = pageNumber;

        // and existing page size...
        this.Pager.PageSize = pageSize;

        // count...
        this.Pager.Count = Mession.SearchData.DefaultView.Count;

        // bind existing search
        SearchBindData(false);

        // use existing search data...
        this.SearchUpdatePanel.Update();
      }
    }

    /// <summary>
    /// The loading modal text_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LoadingModalText_Load(object sender, EventArgs e)
    {
      this.LoadingModalText.Text = GetText("LOADING_SEARCH");
    }

    /// <summary>
    /// The loading image_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LoadingImage_Load(object sender, EventArgs e)
    {
      this.LoadingImage.ImageUrl = YafForumInfo.GetURLToResource("images/loading-white.gif");
    }
  }
}