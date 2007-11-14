using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

namespace YAF
{
	public partial class _default : System.Web.UI.Page
	{
		private System.Web.UI.LosFormatter _formatter = new System.Web.UI.LosFormatter();
		protected System.Web.UI.WebControls.HiddenField viewstatefld = new System.Web.UI.WebControls.HiddenField();

		protected void Page_Load( object sender, EventArgs e )
		{

		}

		protected override void OnPreRender( EventArgs e )
		{
			base.OnPreRender( e );

			viewstatefld.ID = "__VIEWSTATE_SEO";
			Control ctrl = Page.Form;
			ctrl.Controls.Add( viewstatefld );
		}

		#region ViewState Overrides

		protected override void SavePageStateToPersistenceMedium( object viewState )
		{
			StringWriter stream = new StringWriter();
			_formatter.Serialize( stream, viewState );
			viewstatefld.Value = stream.ToString();
		}

		protected override object LoadPageStateFromPersistenceMedium()
		{
			string viewstate = Request.Form ["ctl00$__VIEWSTATE_SEO"];
			if ( viewstate != null )
			{
				StringReader str = new StringReader( viewstate );
				return _formatter.Deserialize( str );
			}
			return null;
		}

		#endregion
	}
}
