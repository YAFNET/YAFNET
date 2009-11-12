namespace YAF.Controls
{
  using System;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Utilities;

  /// <summary>
  /// The last posts.
  /// </summary>
  public partial class LastPosts : BaseUserControl
  {
    /// <summary>
    /// Gets or sets TopicID.
    /// </summary>
    public long? TopicID
    {
      get
      {
        if (ViewState["TopicID"] != null)
        {
          return Convert.ToInt32(ViewState["TopicID"]);
        }

        return null;
      }

      set
      {
        ViewState["TopicID"] = value;
      }
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
      YafContext.Current.PageElements.RegisterJsBlockStartup(
        this.LastPostUpdatePanel, "DisablePageManagerScrollJs", JavaScriptBlocks.DisablePageManagerScrollJs);

      BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      if (TopicID.HasValue)
      {
        this.repLastPosts.DataSource = DB.post_list_reverse10(TopicID);
      }
      else
      {
        this.repLastPosts.DataSource = null;
      }

      DataBind();
    }

    /// <summary>
    /// The last post update timer_ tick.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LastPostUpdateTimer_Tick(object sender, EventArgs e)
    {
      BindData();
    }
  }
}