namespace YAF.Controls
{
  using System;
  using System.Data;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// The forum category list.
  /// </summary>
  public partial class ForumCategoryList : BaseUserControl
  {
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
      DataSet ds = this.Get<YafDBBroker>().BoardLayout(PageContext.PageBoardID, PageContext.PageUserID, PageContext.PageCategoryID, null);
      this.CategoryList.DataSource = ds.Tables[YafDBAccess.GetObjectName("Category")];
      this.CategoryList.DataBind();
      
    }

    /// <summary>
    /// The mark all_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void MarkAll_Click(object sender, EventArgs e)
    {
      YafContext.Current.Get<YafSession>().LastVisit = DateTime.UtcNow;
    }
  }
}