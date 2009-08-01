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

		protected void MarkAll_Click(object sender, System.EventArgs e)
		{
			Mession.LastVisit = DateTime.Now;
		}
	}
}
