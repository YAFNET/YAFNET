using System;
using System.Data;

namespace yaf.controls
{
	/// <summary>
	/// Inherited from LinkButton. Shows only plain text when disabled
	/// </summary>
	public class MyLinkButton : System.Web.UI.WebControls.LinkButton
	{
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			if(!this.Enabled)
				writer.Write(this.Text);
			else
				base.Render(writer);
		}
	}
}
