using System;
using System.Data;
using System.Collections;
using yaf.pages;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for ForumJump.
	/// </summary>
	public class PopMenu : BaseControl
	{
		private string	m_control = string.Empty;
		private Hashtable	m_items = new Hashtable();

		public string Control 
		{
			set
			{
				m_control = value;
			}
			get
			{
				return m_control;
			}
		}

		protected string ControlID
		{
			get
			{
				return string.Format("{0}_{1}",Parent.ClientID,m_control);
			}
		}

		public void AddItem(string title,string script) {
			m_items.Add(title,script);
		}

		private void Page_Load(object sender, System.EventArgs e) 
		{
			if(this.Visible)
				Page.RegisterStartupScript(ClientID,string.Format("<script language='javascript'>yaf_initmenu('{0}');</script>",ControlID));
		}

		override protected void OnInit(EventArgs e)
		{
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new EventHandler(PopMenu_PreRender);
			base.OnInit(e);
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
		}

		private void PopMenu_PreRender(object sender, EventArgs e)
		{
			if(!this.Visible)
				return;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendFormat("<table class='content' border=\"0\" cellspacing=\"0\" cellpadding=\"4\" id=\"{0}_menu\" style=\"position:absolute;z-index:100;left:0;top:0;width:150;visibility:hidden;padding:0px;border:1px solid #FFFFFF;background-color:#FFFFFF\">",ControlID);
#if true
			foreach(string key in m_items.Keys) 
			{
				sb.AppendFormat("<tr><td class='post' onmouseover=\"mouseHover(this,true)\" onmouseout=\"mouseHover(this,false)\" onclick=\"{1}\">{0}</td></tr>",key,m_items[key]);
			}
#else
			sb.AppendFormat("<tr><td class='post' onmouseover=\"mouseHover(this,true)\" onmouseout=\"mouseHover(this,false)\" onclick=\"mouseClick()\">Show Profile</td></tr>");
			sb.AppendFormat("<tr><td class='post' onmouseover=\"mouseHover(this,true)\" onmouseout=\"mouseHover(this,false)\" onclick=\"mouseClick()\">Send Private Message</td></tr>");
			sb.AppendFormat("<tr><td class='post' onmouseover=\"mouseHover(this,true)\" onmouseout=\"mouseHover(this,false)\" onclick=\"mouseClick()\">Edit User (Admin)</td></tr>");
			sb.AppendFormat("<tr><td class='post' onmouseover=\"mouseHover(this,true)\" onmouseout=\"mouseHover(this,false)\" onclick=\"mouseClick()\">Ban User (Admin)</td></tr>");
			sb.AppendFormat("<tr><td class='post' onmouseover=\"mouseHover(this,true)\" onmouseout=\"mouseHover(this,false)\" onclick=\"mouseClick()\">Delete User (Admin)</td></tr>");
#endif
			sb.AppendFormat("</table>");

			Page.RegisterStartupScript(ClientID+"_menuscript",sb.ToString());
		}
	}
}
