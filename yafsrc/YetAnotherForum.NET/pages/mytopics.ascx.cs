namespace YAF.Pages
{
  #region Using

  using System;

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utilities;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The mytopics.
  /// </summary>
  public partial class mytopics : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the mytopics class.
    /// </summary>
    public mytopics()
      : base("MYTOPICS")
    {
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
      // setup jQuery and Jquery Ui Tabs.
      YafContext.Current.PageElements.RegisterJQuery();
      YafContext.Current.PageElements.RegisterJQueryUI();

      YafContext.Current.PageElements.RegisterJsBlock(
        "TopicsTabsJs", JavaScriptBlocks.JqueryUITabsLoadJs(this.TopicsTabs.ClientID, this.hidLastTab.ClientID, false));

      base.OnPreRender(e);
    }

    /// <summary>
    /// The Page_ Load Event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.IsPostBack)
      {
        this.FavoriteTopicsTabTitle.Visible = !this.PageContext.IsGuest;
        this.FavoriteTopicsTabContent.Visible = !this.PageContext.IsGuest;

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        if (this.PageContext.IsGuest)
        {
          this.PageLinks.AddLink(this.GetText("GUESTTITLE"), string.Empty);
        }
        else
        {
          this.PageLinks.AddLink(this.GetText("MEMBERTITLE"), string.Empty);
        }

        this.ForumJumpHolder.Visible = this.PageContext.BoardSettings.ShowForumJump &&
                                       this.PageContext.Settings.LockedForum == 0;
      }
    }

    #endregion
  }
}