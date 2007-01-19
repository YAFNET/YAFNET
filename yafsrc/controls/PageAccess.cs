using System;
using System.Data;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for PageLinks.
	/// </summary>
	public class PageAccess : YAF.Classes.Base.BaseControl
	{
		private void Page_Load(object sender, System.EventArgs e) 
		{
		}

		override protected void OnInit(EventArgs e)
		{
			this.Load += new System.EventHandler(this.Page_Load);
			base.OnInit(e);
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			writer.WriteLine(PageContext.Localization.GetText(PageContext.ForumPostAccess ? "can_post" : "cannot_post"));
			writer.WriteLine("<br/>");
			writer.WriteLine(PageContext.Localization.GetText(PageContext.ForumReplyAccess ? "can_reply" : "cannot_reply"));
			writer.WriteLine("<br/>");
			writer.WriteLine(PageContext.Localization.GetText(PageContext.ForumDeleteAccess ? "can_delete" : "cannot_delete"));
			writer.WriteLine("<br/>");
			writer.WriteLine(PageContext.Localization.GetText(PageContext.ForumEditAccess ? "can_edit" : "cannot_edit"));
			writer.WriteLine("<br/>");
			writer.WriteLine(PageContext.Localization.GetText(PageContext.ForumPollAccess ? "can_poll" : "cannot_poll"));
			writer.WriteLine("<br/>");
			writer.WriteLine(PageContext.Localization.GetText(PageContext.ForumVoteAccess ? "can_vote" : "cannot_vote"));
			writer.WriteLine("<br/>");
		}
	}
}
