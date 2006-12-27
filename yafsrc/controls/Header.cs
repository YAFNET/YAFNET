using System;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for Header.
	/// </summary>
	public class Header : BaseControl
	{
		private string m_html = "";
		private bool m_rendered = false;

		/// <summary>
		/// Status information about the header class.
		/// </summary>
		public string Info
		{
			set
			{
				m_html = value;
				if ( m_rendered )
					throw new ApplicationException( "Header already rendered." );
			}
		}

		/// <summary>
		/// Renders the header.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter that we are using.</param>
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			writer.Write( m_html );
			m_rendered = true;
		}
	}
}
