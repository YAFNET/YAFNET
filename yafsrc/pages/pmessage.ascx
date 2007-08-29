<%@ Control language="c#" CodeFile="pmessage.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.pmessage" %>
<%@ Register TagPrefix="uc1" TagName="smileys" Src="../controls/smileys.ascx" %>
<YAF:PageLinks runat="server" id="PageLinks"/>

<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2"><%= GetText("title") %></td>
	</tr>
	<tr id="PreviewRow" runat="server" visible="false">
		<td class="postformheader" valign="top"><%= GetText("previewtitle") %></td>
		<td class="post" valign="top" id="PreviewCell" runat="server"></td>
	</tr>
	<tr id="ToRow" runat="server">
		<td width="30%" class="postformheader"><%= GetText("to") %></td>
		<td width="70%" class="post">
			<asp:TextBox id="To" runat="server"/>
			<asp:DropDownList runat="server" id="ToList" visible="false"/>
			<asp:button runat="server" id="FindUsers" OnClick="FindUsers_Click" />
			<asp:button runat="server" id="AllUsers" OnClick="AllUsers_Click" />
			<asp:button runat="server" id="Clear" OnClick="Clear_Click" Visible="false" />
		</td>
	</tr>
	<tr>
		<td class="postformheader"><%= GetText("subject") %></td>
		<td class="post"><asp:TextBox id="Subject" runat="server"/></td>
	</tr>
	<tr>
		<td class="postformheader" valign="top">
			<%= GetText("message") %>
			<uc1:smileys runat="server" onclick="insertsmiley" ID="SmileysBox" />
		</td>
		<td id="EditorLine" class="post" runat="server">
			<!-- editor goes here -->	
		</td>
	</tr>
	<tr>
		<td class="postfooter" colspan="2" align="center">
			<asp:Button id="Save" cssclass="pbutton" runat="server" OnClick="Save_Click" />
			<asp:Button id="Preview" cssclass="pbutton" runat="server" OnClick="Preview_Click" />
			<asp:Button id="Cancel" cssclass="pbutton" runat="server" OnClick="Cancel_Click" />
		</td>
	</tr>
</table>

<YAF:SmartScroller id="SmartScroller1" runat="server" />
