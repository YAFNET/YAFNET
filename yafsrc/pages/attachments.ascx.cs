using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf.pages
{
	/// <summary>
	/// Summary description for attachments.
	/// </summary>
	public class attachments : ForumPage
	{
		private DataRow forum, topic;
		protected Repeater List;
		protected Button Back, Upload;
		protected HtmlInputFile File;
		protected controls.PageLinks PageLinks;

		public attachments() : base("ATTACHMENTS")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			using(DataTable dt = DB.forum_list(PageBoardID,PageForumID))
				forum = dt.Rows[0];
			topic = DB.topic_info(PageTopicID);

			if(!IsPostBack) 
			{
				if(!PageInfo.ForumModeratorAccess && !PageInfo.ForumUploadAccess)
					Data.AccessDenied();

				if(!PageInfo.ForumReadAccess)
					Data.AccessDenied();

				if((bool)topic["IsLocked"]) 
					Data.AccessDenied(/*"The topic is closed."*/);

				if((bool)forum["Locked"]) 
					Data.AccessDenied(/*"The forum is closed."*/);

				// Check that non-moderators only edit messages they have written
				if(!PageInfo.ForumModeratorAccess) 
					using(DataTable dt = DB.message_list(Request.QueryString["m"])) 
						if((int)dt.Rows[0]["UserID"] != PageUserID) 
							Data.AccessDenied(/*"You didn't post this message."*/);
		
				PageLinks.AddLink(Config.BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(PageCategoryName,Forum.GetLink(Pages.forum,"c={0}",PageCategoryID));
				PageLinks.AddForumLinks(PageForumID);
				PageLinks.AddLink(PageTopicName,Forum.GetLink(Pages.posts,"t={0}",PageTopicID));
				PageLinks.AddLink(GetText("TITLE"),Request.RawUrl);

				Back.Text = GetText("BACK");
				Upload.Text = GetText("UPLOAD");

				BindData();
			}
		}

		private void BindData() 
		{
			List.DataSource = DB.attachment_list(Request.QueryString["m"],null);
			DataBind();
		}

		protected void Delete_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = String.Format("return confirm('{0}')",GetText("ASK_DELETE"));
		}

		private void Back_Click(object sender, System.EventArgs e) 
		{
			Forum.Redirect(Pages.posts,"m={0}#{0}",Request.QueryString["m"]);
		}

		private void List_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch(e.CommandName) 
			{
				case "delete":
					DB.attachment_delete(e.CommandArgument);
					BindData();
					break;
			}
		}

		private void Upload_Click(object sender, System.EventArgs e) 
		{
			try 
			{
				CheckValidFile(File);
				SaveAttachment(Request.QueryString["m"],File);
				BindData();
			}
			catch(Exception x) 
			{
				AddLoadMessage(x.Message);
				return;
			}
		}

		private void CheckValidFile(HtmlInputFile file) 
		{
			if(file.PostedFile==null || file.PostedFile.FileName.Trim().Length==0 || file.PostedFile.ContentLength==0)
				return;

			string filename = file.PostedFile.FileName;
			int pos = filename.LastIndexOfAny(new char[]{'/','\\'});
			if(pos>=0)
				filename = filename.Substring(pos+1);
			pos = filename.LastIndexOf('.');
			if(pos>=0) 
			{
				switch(filename.Substring(pos+1).ToLower()) 
				{
					default:
						break;
					case "asp":
					case "aspx":
					case "ascx":
					case "config":
					case "php":
					case "php3":
					case "js":
					case "vb":
					case "vbs":
						throw new Exception(String.Format(GetText("fileerror"),filename));
				}
			}
		}

		private void SaveAttachment(object messageID,HtmlInputFile file) 
		{
			if(file.PostedFile==null || file.PostedFile.FileName.Trim().Length==0 || file.PostedFile.ContentLength==0)
				return;

			string sUpDir = Request.MapPath(Config.ConfigSection["uploaddir"]);

			string filename = file.PostedFile.FileName;
			int pos = filename.LastIndexOfAny(new char[]{'/','\\'});
			if(pos>=0)
				filename = filename.Substring(pos+1);

			bool useFileTable = false;
			int maxFileSize = -1;
			using(DataTable dt=DB.system_list()) 
			{
				foreach(DataRow row in dt.Rows) 
				{
					useFileTable = (bool)row["UseFileTable"];
					if(!row.IsNull("MaxFileSize"))
						maxFileSize = (int)row["MaxFileSize"];
				}
			}

			if(maxFileSize>=0 && file.PostedFile.ContentLength>maxFileSize) 
				throw new Exception(GetText("ERROR_TOOBIG"));

			if(useFileTable) 
			{
				DB.attachment_save(messageID,filename,file.PostedFile.ContentLength,file.PostedFile.ContentType,file.PostedFile.InputStream);
			} 
			else 
			{
				file.PostedFile.SaveAs(String.Format("{0}{1}.{2}",sUpDir,messageID,filename));
				DB.attachment_save(messageID,filename,file.PostedFile.ContentLength,file.PostedFile.ContentType,null);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			Back.Click +=new EventHandler(Back_Click);
			Upload.Click += new EventHandler(Upload_Click);
			List.ItemCommand += new RepeaterCommandEventHandler(List_ItemCommand);
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
