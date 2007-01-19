using System;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for Footer.
	/// </summary>
	public class Footer : YAF.Classes.Base.BaseControl
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
			writer.WriteLine(m_html);
			m_rendered = true;
		}
	}
}
