using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Data;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class ForumCategoryList : YAF.Classes.Core.BaseUserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			DataSet ds = YafServices.DBBroker.BoardLayout(PageContext.PageBoardID, PageContext.PageUserID, PageContext.PageCategoryID, null);
			CategoryList.DataSource = ds.Tables[YafDBAccess.GetObjectName("Category")];
			CategoryList.DataBind();
		}

		protected void MarkAll_Click(object sender, System.EventArgs e)
		{
			Mession.LastVisit = DateTime.Now;
		}
	}
}
