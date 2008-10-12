using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace YAF.Modules
{
	public class SpoilerBBCodeModule : YafBBCodeControl
	{
		public SpoilerBBCodeModule()
			: base()
		{

		}

		protected override void Render( HtmlTextWriter writer )
		{
			StringBuilder sb = new StringBuilder();

			string spoilerTitle = this.HtmlEncode( this.LocalizedString( "SPOILERMOD_TOOLTIP", "Click here to show or hide the hidden text (also known as a spoiler)" ) );

			sb.AppendLine("<!-- BEGIN spoiler -->");
			sb.AppendLine(@"<div class=""spoilertitle"">");
			sb.AppendFormat( @"<input type=""button"" value=""{2}"" class=""spoilerbutton"" name=""{0}"" onclick='toggleSpoiler(this,""{1}"");' title=""{3}"" /></div><div class=""spoilerbox"" id=""{1}"" style=""display:none"">", this.GetUniqueID( "spoilerBtn" ), this.GetUniqueID( "spoil_" ), this.HtmlEncode( this.LocalizedString( "SPOILERMOD_SHOW", "Show Spoiler" ) ), spoilerTitle );
			sb.AppendLine( this.Parameters ["inner"] );
			sb.AppendLine("</div>");
			sb.AppendLine("<!-- END spoiler -->");

			writer.Write( sb.ToString() );
		}
	}
}