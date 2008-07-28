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

			sb.AppendLine("<!-- BEGIN spoiler -->");
			sb.AppendLine(@"<div class=""spoilertitle"">");
			sb.AppendFormat(@"<input type=""button"" value=""Show Spoiler"" class=""spoilerbutton"" name=""{0}"" onclick='toggleSpoiler(this,""{1}"");' /></div><div class=""spoilerbox"" id=""{1}"" style=""display:none"">", this.GetUniqueID("spoilerBtn"), this.GetUniqueID("spoil_"));
			sb.AppendLine( this.Parameters["inner"] );
			sb.AppendLine("</div>");
			sb.AppendLine("<!-- END spoiler -->");

			writer.Write( sb.ToString() );
		}
	}
}