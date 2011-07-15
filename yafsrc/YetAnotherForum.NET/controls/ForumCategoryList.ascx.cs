namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Utils;

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
            var markAll = (LinkButton)sender;

            object categoryId = null;

            int icategoryId;
            if (int.TryParse(markAll.CommandArgument, out icategoryId))
            {
                categoryId = icategoryId;
            }

            DataTable dt = LegacyDb.forum_listread(
                this.PageContext.PageBoardID, this.PageContext.PageUserID, categoryId, null, false, false);

            foreach (DataRow row in dt.Rows)
            {
                if (this.Get<YafBoardSettings>().UseReadTrackingByDatabase)
                {
                    this.Get<IReadTracking>().SetForumRead(this.PageContext.PageUserID, row["ForumID"].ToType<int>());
                }
                else
                {
                    this.Get<IYafSession>().SetForumRead(row["ForumID"].ToType<int>(), DateTime.UtcNow);
                }
            }

            this.BindData();
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
            this.BindData();
        }

        /// <summary>
        /// Bind Data
        /// </summary>
        private void BindData()
        {
            DataSet ds = this.Get<IDBBroker>().BoardLayout(
                this.PageContext.PageBoardID, this.PageContext.PageUserID, this.PageContext.PageCategoryID, null);

            this.CategoryList.DataSource = ds.Tables[MsSqlDbAccess.GetObjectName("Category")];
            this.CategoryList.DataBind();
        }
        #endregion
    }
}