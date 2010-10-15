namespace YAF.Controls
{
  #region Using

  using System;
  using Classes.Data;

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
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        ReportedPostsRepeater.DataSource = DB.message_listreporters(this.MessageID);
        ReportedPostsRepeater.DataBind();
      }
    }

    #endregion
  }
}