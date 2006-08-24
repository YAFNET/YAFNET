using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf.pages
{
	/// <summary>
	///		Summary description for LastPosts.
	/// </summary>
	public partial class lastposts : ForumPage
	{

		public lastposts() : base("POSTMESSAGE")
		{
			ShowToolBar = false;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!ForumReadAccess)
				Data.AccessDenied();

			if (Request.QueryString["t"] != null)
			{
				repLastPosts.DataSource = DB.post_list_reverse10(Request.QueryString["t"]);
				repLastPosts.DataBind();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		protected string FormatBody(object o) 
		{
			DataRowView row = (DataRowView)o;
			string html = FormatMsg.FormatMessage(this,row["Message"].ToString(),new MessageFlags(Convert.ToInt32(row["Flags"])));

			string messageSignature = row["Signature"].ToString();
			if (messageSignature != string.Empty) 
			{
				MessageFlags flags = new MessageFlags();
				flags.IsHTML = false;

				messageSignature = FormatMsg.FormatMessage(this,messageSignature,flags);
				html += "<br/><hr noshade/>" + messageSignature;
			}

			return html;
		}
	}
}
