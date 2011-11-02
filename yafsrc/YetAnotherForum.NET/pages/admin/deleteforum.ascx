<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.deleteforum"
    CodeBehind="deleteforum.ascx.cs" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_DELETEFORUM" />
                <asp:Label ID="ForumNameTitle" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="header2" height="30" colspan="2">
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="MOVE_TOPICS" LocalizedPage="ADMIN_DELETEFORUM" />
            </td>
            <td class="post">
                <asp:CheckBox ID="MoveTopics" runat="server" AutoPostBack="true"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="NEW_FORUM" LocalizedPage="ADMIN_DELETEFORUM" />
                <strong></strong>
                <br />
            </td>
            <td class="post">
                <asp:DropDownList Width="250" ID="ForumList" runat="server" Enabled="false">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postfooter" align="center" colspan="2">
                <asp:Button ID="Delete" runat="server" CssClass="pbutton"></asp:Button>&nbsp;
                <asp:Button ID="Cancel" runat="server" CssClass="pbutton"></asp:Button>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
	<ContentTemplate>
		<asp:Timer ID="UpdateStatusTimer" runat="server" Enabled="false" Interval="4000" OnTick="UpdateStatusTimer_Tick" />
	
	</ContentTemplate>
</asp:UpdatePanel>

<div>
	<div id="DeleteForumMessage" style="display:none" class="ui-overlay">
		<div class="ui-widget ui-widget-content ui-corner-all">
		<h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DELETE_TITLE" LocalizedPage="ADMIN_DELETEFORUM" /></h2>
		<p><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="DELETE_MSG" LocalizedPage="ADMIN_DELETEFORUM" /></p>
		<div align="center">
			<asp:Image ID="LoadingImage" runat="server" alt="Processing..." />
		</div>
		<br />
		</div>
	</div>
</div>

<YAF:SmartScroller ID="SmartScroller1" runat="server" />
