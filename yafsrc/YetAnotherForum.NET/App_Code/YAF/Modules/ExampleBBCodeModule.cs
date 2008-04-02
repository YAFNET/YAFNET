using System;
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
	public class ExampleBBCodeModule : YafBBCodeControl
	{
		public ExampleBBCodeModule()
			: base()
		{

		}

		protected override void Render( HtmlTextWriter writer )
		{
			writer.Write( "Hello, you wrote this: " + this.Parameters["inner"] );			
		}
	}
}