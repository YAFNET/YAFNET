using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class ForumCategoryList : YAF.Classes.Base.BaseUserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			DataSet ds = YAF.Classes.Utils.DBBroker.board_layout(PageContext.PageBoardID, PageContext.PageUserID, PageContext.PageCategoryID, null);
			CategoryList.DataSource = ds.Tables[DBAccess.GetObjectName("Category")];

			CategoryList.DataBind();
		}

		protected string GetForumIcon(object o)
		{
			DataRow row = (DataRow)o;
			bool locked = (bool)row["Locked"];
			DateTime lastRead = Mession.GetForumRead((int)row["ForumID"]);
			DateTime lastPosted = row["LastPosted"] != DBNull.Value ? (DateTime)row["LastPosted"] : lastRead;

			string img, imgTitle;

			try
			{
				if (locked)
				{
					img = GetThemeContents("ICONS", "FORUM_LOCKED");
					imgTitle = GetText("ICONLEGEND", "Forum_Locked");
				}
				else if (lastPosted > lastRead)
				{
					img = GetThemeContents("ICONS", "FORUM_NEW");
					imgTitle = GetText("ICONLEGEND", "New_Posts");
				}
				else
				{
					img = GetThemeContents("ICONS", "FORUM");
					imgTitle = GetText("ICONLEGEND", "No_New_Posts");
				}
			}
			catch (Exception)
			{
				img = GetThemeContents("ICONS", "FORUM");
				imgTitle = GetText("ICONLEGEND", "No_New_Posts");
			}

			return String.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\"/>", img, imgTitle);
		}
	}
}
