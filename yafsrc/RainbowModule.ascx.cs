namespace yaf_rainbow
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Rainbow.UI.WebControls;

	/// <summary>
	///		Summary description for RainbowModule.
	/// </summary>
	public class RainbowModule : PortalModuleControl
	{
		public override Guid GuidID
		{
			get
			{
				return new Guid("{5F582DF5-5567-438d-8E2D-07D012EE80FB}");
			}
		}
		protected override void OnInit(System.EventArgs e)
		{
			ModuleTitle = new DesktopModuleTitle();
			Controls.AddAt(0,ModuleTitle);
			base.OnInit(e);
		}
	}
}
