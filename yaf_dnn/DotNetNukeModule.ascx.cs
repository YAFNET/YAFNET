namespace yaf_dnn
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DotNetNuke;

	/// <summary>
	///		Summary description for DotNetNukeModule.
	/// </summary>
	public class DotNetNukeModule : PortalModuleControl
	{
		private void DotNetNukeModule_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}
		override protected void OnInit(EventArgs e)
		{
			Load += new EventHandler(DotNetNukeModule_Load);
			base.OnInit(e);
		}
	}
}
