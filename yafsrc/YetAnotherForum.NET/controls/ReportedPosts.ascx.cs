namespace YAF.Controls
{
  #region Using

  using System;

  using YAF.Classes.Data;
  using YAF.Types;

  #endregion

  /// <summary>
  /// The reported posts.
  /// </summary>
  public partial class ReportedPosts : BaseReportedPosts
  {
    #region Methods

    /// <summary>
    /// The page_ load.
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
        this.ReportedPostsRepeater.DataSource = DB.message_listreporters(this.MessageID);
        this.ReportedPostsRepeater.DataBind();
      }
    }

    #endregion
  }
}