<%@ Control Language="c#" Inherits="Rainbow.UI.WebControls.PortalModuleControl" %>
<%@ Register TagPrefix="yaf" Namespace="yaf" Assembly="yaf" %>

<script runat="server">
	private void Page_Load(object sender, System.EventArgs e)
	{
		yafForum.Rainbow = Rainbow.HttpUrlBuilder.BuildUrl(TabId);
	}
	public override Guid GuidID
	{
		get
		{
			return new Guid("{5F582DF5-5567-438d-8E2D-07D012EE80FB}");
		}
	}
	protected override void OnInit(System.EventArgs e)
	{
		ModuleTitle = new Rainbow.UI.WebControls.DesktopModuleTitle();
		Controls.AddAt(0,ModuleTitle);
		base.OnInit(e);
	}
</script>

<yaf:forum runat="server" id="yafForum" />
