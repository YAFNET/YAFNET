<%@ Control Language="c#" CodeFile="pmessage.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.pmessage" %>
<%@ Register TagPrefix="uc1" TagName="smileys" Src="../controls/smileys.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
		</td>
	</tr>
	<tr id="PreviewRow" runat="server" visible="false">
		<td class="postformheader" valign="top">
			<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="previewtitle" />
		</td>
		<td class="post" valign="top" id="PreviewCell" runat="server">
		</td>
	</tr>
	<tr id="ToRow" runat="server">
		<td width="30%" class="postformheader">
			<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="to" />
		</td>
		<td width="70%" class="post">
			<asp:TextBox ID="To" runat="server" />
			<asp:DropDownList runat="server" ID="ToList" Visible="false" />
			<asp:Button runat="server" ID="FindUsers" OnClick="FindUsers_Click" />
			<asp:Button runat="server" ID="AllUsers" OnClick="AllUsers_Click" />
			<asp:Button runat="server" ID="Clear" OnClick="Clear_Click" Visible="false" />
			<asp:Label ID="MultiReceiverInfo" runat="server" Visible="false" />			
		</td>
	</tr>
	<tr>
		<td class="postformheader">
			<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="subject" />
		</td>
		<td class="post">
			<asp:TextBox ID="Subject" runat="server" /></td>
	</tr>
	<tr>
		<td class="postformheader" valign="top">
			<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="message" />
			<uc1:smileys runat="server" OnClick="insertsmiley" ID="SmileysBox" />
		</td>
		<td id="EditorLine" class="post" runat="server">
			<!-- editor goes here -->
		</td>
	</tr>
	<tr>
		<td class="postfooter" colspan="2" align="center">
			<asp:Button ID="Save" CssClass="pbutton" runat="server" OnClick="Save_Click" />
			<asp:Button ID="Preview" CssClass="pbutton" runat="server" OnClick="Preview_Click" />
			<asp:Button ID="Cancel" CssClass="pbutton" runat="server" OnClick="Cancel_Click" />
		</td>
	</tr>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
