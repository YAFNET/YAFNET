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
	using YAF.Utils.Extensions;

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

			this.Get<IReadTrackCurrentUser>().SetForumRead(dt.AsEnumerable().Select(r => r["ForumID"].ToType<int>()));

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

			this.CategoryList.DataSource = ds.GetTable("Category");
			this.CategoryList.DataBind();
		}

		/// <summary>
		/// Column count
		/// </summary>
		protected int ColumnCount()
		{
			int cnt = 5;
			if (this.Get<YafBoardSettings>().ShowModeratorList && this.Get<YafBoardSettings>().ShowModeratorListAsColumn)
			{
				cnt++;
			}
			return cnt;
		}

		#endregion
	}
}