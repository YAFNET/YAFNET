using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace YAF.Pages
{
	public partial class showsmilies : YAF.Classes.Base.ForumPage
	{
		// constructor
		public showsmilies() : base("SHOWSMILIES") { }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack && !PageContext.IsGuest)
			{
				ShowToolBar = false;

				try
				{
					Parent.Parent.FindControl("imgBanner").Visible = false;
				}
				catch
				{
					// control is nested more than anticipated
				}

				BindData();
			}
		}

		private void BindData()
		{
			List.DataSource = YAF.Classes.Data.DB.smiley_listunique(PageContext.PageBoardID);
			DataBind();
		}

		protected string GetSmileyScript(string code, string icon)
		{
			code = code.ToLower();
			code = code.Replace("&", "&amp;");
			code = code.Replace("\"", "&quot;");
			code = code.Replace("'", "\\'");

			return String.Format( "javascript:{0}('{1} ','{3}images/emoticons/{2}');", "insertsmiley", code, icon, YAF.Classes.Utils.YafForumInfo.ForumRoot );
		}
	}
}