<%@ Register TagPrefix="yaf" Namespace="yaf" Assembly="yaf" %>
<%@ Register TagPrefix="Portal" TagName="Title" Src="~/controls/DesktopModuleTitle.ascx"%>
<%@ Control language="c#" Inherits="yaf_dnn.DotNetNukeModule" CodeBehind="DotNetNukeModule.ascx.cs" AutoEventWireup="false" %>
<portal:title EditText="Edit" runat="server" id="Title1" />
<asp:Panel ID="pnlModuleContent" Runat="server">
	<yaf:forum id="Forum1" runat="server"></yaf:forum>
</asp:Panel>
