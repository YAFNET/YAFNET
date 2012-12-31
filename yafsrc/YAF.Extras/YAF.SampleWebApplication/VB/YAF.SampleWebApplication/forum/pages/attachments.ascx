<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.attachments" Codebehind="attachments.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator">
</div>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="3">
			<YAF:LocalizedLabel ID="Title" LocalizedTag="TITLE" runat="server" />
		</td>
	</tr>
	<asp:Repeater runat="server" ID="List" OnItemCommand="List_ItemCommand">
		<HeaderTemplate>
			<tr>
				<td class="header2">
					<YAF:LocalizedLabel ID="Filename" LocalizedTag="FILENAME" runat="server" />
				</td>
				<td class="header2" align="right">
					<YAF:LocalizedLabel ID="Size" LocalizedTag="SIZE" runat="server" />
				</td>
				<td class="header2">
					&nbsp;
				</td>
			</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post">
					<%# Eval( "FileName") %>
				</td>
				<td class="post" align="right">
					<%# (int)Eval("Bytes") / 1024%> Kb
				</td>
				<td class="post">
					<asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "AttachmentID") %>'><%# this.GetText("DELETE") %></asp:LinkButton>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
	<tr id="uploadtitletr" runat="server">
		<td class="header2">
			<YAF:LocalizedLabel ID="UploadTitle" LocalizedTag="UPLOAD_TITLE" runat="server" />
		</td>
		<td class="header2">
			&nbsp;
		</td>
		<td class="header2">
			&nbsp;
		</td>
	</tr>
	<tr id="selectfiletr" runat="server">
		<td class="postheader">
			<YAF:LocalizedLabel ID="SelectFile" LocalizedTag="SELECT_FILE" runat="server" />
		</td>
		<td class="post">
			<input type="file" id="File" class="pbutton" runat="server" />
            <asp:PlaceHolder ID="UploadNodePlaceHold" runat="server"><br /><em><asp:Label ID="UploadNote" runat="server"></asp:Label></em></asp:PlaceHolder>
		</td>
		<td class="post">
			<asp:Button runat="server" CssClass="pbutton" ID="Upload" OnClick="Upload_Click" />
		</td>
	</tr>
	<tr>
		<td class="header2">
			<YAF:LocalizedLabel ID="ExtensionTitle" LocalizedTag="ALLOWED_EXTENSIONS" runat="server" />
		</td>
		<td class="header2">
			&nbsp;
		</td>
		<td class="header2">
			&nbsp;
		</td>
	</tr>
	<tr>
		<td class="post" colspan="3">
			<asp:Label ID="ExtensionsList" runat="server"></asp:Label>
		</td>
	</tr>
	<tr class="footer1">
		<td colspan="3" align="center">
			<asp:Button runat="server" CssClass="pbutton" ID="Back" OnClick="Back_Click" />
		</td>
	</tr>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
