using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;
using YAF.Classes.UI;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	///		Summary description for LastPosts.
	/// </summary>
	public partial class lastposts : YAF.Classes.Base.ForumPage
	{

		public lastposts() : base("POSTMESSAGE")
		{
			ShowToolBar = false;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!PageContext.ForumReadAccess)
				YafBuildLink.AccessDenied();

			if (Request.QueryString["t"] != null)
			{
				repLastPosts.DataSource = YAF.Classes.Data.DB.post_list_reverse10(Request.QueryString["t"]);
				repLastPosts.DataBind();
			}
		}

		protected string FormatBody(object o) 
		{
			DataRowView row = (DataRowView)o;
			string html = FormatMsg.FormatMessage(row["Message"].ToString(),new MessageFlags(Convert.ToInt32(row["Flags"])));

			string messageSignature = row["Signature"].ToString();
			if (messageSignature != string.Empty) 
			{
				MessageFlags flags = new MessageFlags();
				flags.IsHTML = false;

				messageSignature = FormatMsg.FormatMessage(messageSignature,flags);
				html += "<br/><hr noshade/>" + messageSignature;
			}

			return html;
		}
	}
}
