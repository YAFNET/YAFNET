using System;

namespace yaf
{
	/// <summary>
	/// Summary description for Header.
	/// </summary>
	public class Header : BaseControl
	{
		private string	m_html		= "";
		private bool	m_rendered	= false;

		public string Info
		{
			set
			{
				m_html = value;
				if(m_rendered)
					throw new ApplicationException("Header already rendered.");
			}
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			writer.Write(m_html);
			m_rendered = true;
		}
	}

	public class Test : BaseControl
	{
		public Test()
		{
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			string act_rank = "";

			act_rank += "<table width=\"90%\" class=\"content\" cellspacing=\"1\" border=\"0\" cellpadding=\"0\">";
			act_rank += "<tr class=\"header2\"><td>Most active users</td></tr>";
			//act_rank += "<tr class=header2><td colspan=\"2\">User</td>";
			//act_rank += "<td align=\"center\">Posts</td></tr>";
			
			System.Data.DataTable rank = DB.DataProvider.user_activity_rank();
			int i = 1;

			act_rank += "<tr><td class=post><table cellspacing=0 cellpadding=0 align=center>";

			foreach( System.Data.DataRow r in rank.Rows )
			{
				string img = "<img src='/yetanotherforum.net/themes/standard/user_rank1.gif'/>";//string.Format( "<img src=\"{0}\"/>", MyPage.ThemeFile( string.Format( "user_rank{0}.gif", i ) ) );
				i++;
				act_rank += "<tr class=\"post\">";
				
				// Immagine
				act_rank += string.Format( "<td align=\"center\">{0}</td>", img );

				// Nome autore
				act_rank += string.Format( "<td width=\"75%\">&nbsp;<a href='{1}'>{0}</a></td>", r["Name"], Forum.GetLink(Pages.profile,"u={0}",r["ID"]) );

				// Numero post
				act_rank += string.Format( "<td align=\"center\">{0}</td></tr>", r["NumOfPosts"]);

				act_rank += "</tr>";
			}

			act_rank += "</table></td></tr>";

			act_rank += "</table>";
			writer.Write(act_rank);
		}
	}
}
