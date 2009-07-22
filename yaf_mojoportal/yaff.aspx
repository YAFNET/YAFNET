<%@ Page Language="C#" MasterPageFile="~/App_MasterPages/layout.Master" Inherits="yaf_mojo.YafModule" %>

<%@ Register TagPrefix="yaf" Namespace="yaf" Assembly="yaf" %>
<%@ Register TagPrefix="yc" Namespace="yaf.controls" Assembly="yaf" %>
<asp:Content ContentPlaceHolderID="leftContent" ID="MPLeftPane" runat="server" />
<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
	<mp:cornerroundertop id="ctop1" runat="server" />
	<asp:Panel ID="pnlForum" runat="server" CssClass="panelwrapper forumview">
		<img src="/Modules/yaf/images/yaflogo.jpg" runat="server" id="imgBanner" alt="" />
		<yaf:forum runat="server" id="forum" />
	</asp:Panel>
	<mp:cornerrounderbottom id="cbottom1" runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="rightContent" ID="MPRightPane" runat="server" />
<asp:Content ContentPlaceHolderID="pageEditContent" ID="MPPageEdit" runat="server" />
