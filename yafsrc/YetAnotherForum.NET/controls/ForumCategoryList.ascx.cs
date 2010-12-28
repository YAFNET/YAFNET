namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The forum category list.
  /// </summary>
  public partial class ForumCategoryList : BaseUserControl
  {
    #region Methods

    /// <summary>
    /// The mark all_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void MarkAll_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafContext.Current.Get<IYafSession>().LastVisit = DateTime.UtcNow;
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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      DataSet ds = this.Get<IDBBroker>().BoardLayout(
        this.PageContext.PageBoardID, this.PageContext.PageUserID, this.PageContext.PageCategoryID, null);
      this.CategoryList.DataSource = ds.Tables[MsSqlDbAccess.GetObjectName("Category")];
      this.CategoryList.DataBind();
    }

    #endregion
  }
}