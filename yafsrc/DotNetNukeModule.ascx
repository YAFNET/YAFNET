<%@ Control language="c#" Inherits="yaf_dnn.DotNetNukeModule" CodeBehind="DotNetNukeModule.ascx.cs" AutoEventWireup="false" %>
<%@ Register TagPrefix="Portal" TagName="Title" Src="~/controls/DesktopModuleTitle.ascx"%>
<%@ Register TagPrefix="yaf" Namespace="yaf" Assembly="yaf" %>

<portal:title EditText="Edit" runat="server" id="Title1" />
<asp:Panel ID="pnlModuleContent" Runat="server">
<yaf:forum runat="server"/>
</asp:Panel>

