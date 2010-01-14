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
namespace YAF.Classes.Core
{
  using System;
  using System.Collections;
  using System.Data;
  using System.Web;
  using System.Web.Security;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;

  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// The class that all Yaf forum pages are based on.
  /// </summary>
  public class ForumPage : UserControl
  {
    #region Variables

    // cache for this page
    private readonly Hashtable _pageCache;

    private bool _noDataBase;

    private bool _showToolBar = Config.ShowToolBar;

    private bool _showFooter = Config.ShowFooter;

    private readonly string _transPage = string.Empty;

    public event EventHandler<ForumPageRenderedArgs> ForumPageRendered;

    protected bool _isAdminPage;

    public bool IsAdminPage
    {
      get
      {
        return this._isAdminPage;
      }
    }

    protected bool _isRegisteredPage = false;

    public bool IsRegisteredPage
    {
      get
      {
        return this._isRegisteredPage;
      }
    }

    private bool _allowAsPopup = false;

    public bool AllowAsPopup
    {
      get
      {
        return this._allowAsPopup;
      }
      protected set
      {
        this._allowAsPopup = value;
      }
    }

    private IYafHeader _header = null;

    public IYafHeader ForumHeader
    {
      get
      {
        return this._header;
      }
      set
      {
        this._header = value;
      }
    }

    private IYafFooter _footer = null;

    public IYafFooter ForumFooter
    {
      get
      {
        return this._footer;
      }
      set
      {
        this._footer = value;
      }
    }

    #endregion

    #region Constructor and events

    /// <summary>
    /// Constructor
    /// </summary>
    public ForumPage()
      : this("")
    {
    }

    public ForumPage(string transPage)
    {
      // create empty hashtable for cache entries
      this._pageCache = new Hashtable();

      this._transPage = transPage;
      this.Init += new EventHandler(this.ForumPage_Init);
      this.Load += new EventHandler(this.ForumPage_Load);
      this.Unload += new EventHandler(this.ForumPage_Unload);
      this.Error += new EventHandler(this.ForumPage_Error);
      this.PreRender += new EventHandler(this.ForumPage_PreRender);
    }

    private void ForumPage_Error(object sender, EventArgs e)
    {
      // This doesn't seem to work...
      Exception x = this.Server.GetLastError();
      DB.eventlog_create(this.PageContext.PageUserID, this, x);
      if (!YafForumInfo.IsLocal)
      {
        CreateMail.CreateLogEmail(this.Server.GetLastError());
      }
    }

    /// <summary>
    /// Called first to initialize the context
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ForumPage_Init(object sender, EventArgs e)
    {
      // fire init event...
      YafContext.Current.ForumPageInit(this, new EventArgs());

      if (this._noDataBase)
      {
        return;
      }

#if DEBUG
      QueryCounter.Reset();
#endif

      // set the current translation page...
      YafContext.Current.InstanceFactory.GetInstance<LocalizationHandler>().TranslationPage = this._transPage;

      // fire preload event...
      YafContext.Current.ForumPagePreLoad(this, new EventArgs());
    }

    /// <summary>
    /// Called when page is loaded
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ForumPage_Load(object sender, EventArgs e)
    {
      if (this.PageContext.BoardSettings.DoUrlReferrerSecurityCheck)
      {
        Security.CheckRequestValidity(this.Request);
      }
    }

    /// <summary>
    /// Called when the page is unloaded
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ForumPage_Unload(object sender, EventArgs e)
    {
      // release cache
      if (this._pageCache != null)
      {
        this._pageCache.Clear();
      }
    }

    #endregion

    #region Render Functions

    protected void InsertCssRefresh(Control addTo)
    {
      // make the style sheet link controls.
      addTo.Controls.Add(ControlHelper.MakeCssIncludeControl(YafForumInfo.GetURLToResource("css/forum.css")));
      addTo.Controls.Add(ControlHelper.MakeCssIncludeControl(YafContext.Current.Theme.BuildThemePath("theme.css")));

      if (this.ForumHeader.RefreshURL != null && this.ForumHeader.RefreshTime >= 0)
      {
        var refresh = new HtmlMeta();
        refresh.HttpEquiv = "Refresh";
        refresh.Content = String.Format("{1};url={0}", this.ForumHeader.RefreshURL, this.ForumHeader.RefreshTime);

        addTo.Controls.Add(refresh);
      }
    }

    protected Control _topPageControl = null;

    public Control TopPageControl
    {
      get
      {
        if (this._topPageControl == null)
        {
          if (this.Page != null && this.Page.Header != null)
          {
            this._topPageControl = this.Page.Header;
          }
          else
          {
            this._topPageControl = ControlHelper.FindControlRecursiveBoth(this, "YafHead") ??
                                   this.ForumHeader.ThisControl;
          }
        }

        return this._topPageControl;
      }
    }

    protected void SetupHeaderElements()
    {
      this.InsertCssRefresh(this.TopPageControl);
    }

    private void ForumPage_PreRender(object sender, EventArgs e)
    {
      // sets up the head elements in addition to the Css and image elements
      this.SetupHeaderElements();
      // setup the forum control header & footer properties
      this.ForumHeader.SimpleRender = !this.ShowToolBar;
      this.ForumFooter.SimpleRender = !this.ShowFooter;
    }

    /// <summary>
    /// Writes the document
    /// </summary>
    /// <param name="writer"></param>
    protected override void Render(HtmlTextWriter writer)
    {
      base.Render(writer);
      // handle additional rendering if desired...
      if (this.ForumPageRendered != null)
      {
        this.ForumPageRendered(this, new ForumPageRenderedArgs(writer));
      }
    }

    #endregion

    #region Page/User properties
    /// <summary>
    /// Set to true if this is the start page. Should only be set by the page that initialized the database.
    /// </summary>
    protected bool NoDataBase
    {
      set
      {
        _noDataBase = value;
      }
    }

    public void RedirectNoAccess()
    {
      YafServices.Permissions.HandleRequest(ViewPermissions.RegisteredUsers);
    }
    #endregion

    #region Page Cache

    /// <summary>
    /// Gets cache associated with this page.
    /// </summary>
    public Hashtable PageCache
    {
      get { return _pageCache; }
    }

    #endregion

    #region Other
    /// <summary>
    /// Adds a message that is displayed to the user when the page is loaded.
    /// </summary>
    /// <param name="msg">The message to display</param>
    public string RefreshURL
    {
      get
      {
        if (ForumHeader != null) return ForumHeader.RefreshURL;
        return null;
      }

      set
      {
        if (this.ForumHeader != null) this.ForumHeader.RefreshURL = value;
      }
    }

    public int RefreshTime
    {
      get
      {
        if (ForumHeader != null) return ForumHeader.RefreshTime;
        return 0;
      }

      set
      {
        if (this.ForumHeader != null) this.ForumHeader.RefreshTime = value;
      }
    }

    /// <summary>
    /// Gets info whether page should be hidden to guest users when forum admin requires login.
    /// </summary>
    public virtual bool IsProtected
    {
      get { return true; }
    }

    #endregion

    #region Layout functions
    /// <summary>
    /// Set to false if you don't want the menus at top and bottom. Only admin pages will set this to false
    /// </summary>
    public bool ShowToolBar
    {
      get
      {
        return _showToolBar;
      }

      protected set
      {
        _showToolBar = value;
      }
    }

    public bool ShowFooter
    {
      get
      {
        return _showFooter;
      }

      protected set
      {
        _showFooter = value;
      }
    }
    #endregion

    public static object IsNull(string value)
    {
      if (value == null || value.ToLower() == string.Empty)
        return DBNull.Value;
      else
        return value;
    }

    #region PageInfo class

    [Obsolete("Useless property that always returns true. Do not use anymore.")]
    public bool CanLogin
    {
      get
      {
        return true;
      }
    }

    public MembershipUser User
    {
      get
      {
        return PageContext.User;
      }
    }

    public string LoadMessage
    {
      get
      {
        return PageContext.LoadMessage.LoadString;
      }
    }

    #region Theme Helper Functions
    /// <summary>
    /// Get a value from the currently configured forum theme.
    /// </summary>
    /// <param name="page">Page to look under</param>
    /// <param name="tag">Theme item</param>
    /// <returns>Converted Theme information</returns>
    public string GetThemeContents(string page, string tag)
    {
      return PageContext.Theme.GetItem(page, tag);
    }

    /// <summary>
    /// Get a value from the currently configured forum theme.
    /// </summary>
    /// <param name="page">Page to look under</param>
    /// <param name="tag">Theme item</param>
    /// <param name="defaultValue">Value to return if the theme item doesn't exist</param>
    /// <returns>Converted Theme information or Default Value if it doesn't exist</returns>
    public string GetThemeContents(string page, string tag, string defaultValue)
    {
      return PageContext.Theme.GetItem(page, tag, defaultValue);
    }

    /// <summary>
    /// Gets the collapsible panel image url (expanded or collapsed). 
    /// 
    /// <param name="panelID">ID of collapsible panel</param>
    /// <param name="defaultState">Default Panel State</param>
    /// </summary>
    /// <returns>Image URL</returns>
    public string GetCollapsiblePanelImageURL(string panelID, PanelSessionState.CollapsiblePanelState defaultState)
    {
      return PageContext.Theme.GetCollapsiblePanelImageURL(panelID, defaultState);
    }

    #endregion

    #region Localization Helper Functions

    public string GetTextFormatted(string text, params object[] args)
    {
      return PageContext.Localization.GetTextFormatted(text, args);
    }

    public string GetText(string text)
    {
      return PageContext.Localization.GetText(text);
    }

    public string GetText(string page, string text)
    {
      return PageContext.Localization.GetText(page, text);
    }

    #endregion

    /// <summary>
    /// Gets the current forum Context (helper reference)
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        return YafContext.Current;
      }
    }

    public string ForumURL
    {
      get
      {
        return YafBuildLink.GetLink(ForumPages.forum, true);
      }
    }

    #endregion

    public string HtmlEncode(object data)
    {
      if (data == null || !(data is string)) return null;
      return Server.HtmlEncode(data.ToString());
    }

    protected virtual void CreatePageLinks()
    {
      // Page link creation goes to this method (overloads in descendant classes)
    }
  }
}