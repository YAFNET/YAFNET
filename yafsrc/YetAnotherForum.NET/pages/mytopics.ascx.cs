namespace YAF.Pages
{
  #region Using

  using System;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

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
        this.FavoriteTopicsTab.Visible = !this.PageContext.IsGuest;
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

      // Set the DNA Views' titles.
      this.TopicsTabs.Views[0].Text = this.GetText("MyTopics", "ActiveTopics");
      if (!this.PageContext.IsGuest)
      {
        this.TopicsTabs.Views[1].Text = this.GetText("MyTopics", "FavoriteTopics");
      }
    }

    #endregion
  }
}