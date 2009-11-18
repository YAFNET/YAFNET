<%@ Control language="c#" CodeFile="deletemessage.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.deletemessage" %>
<%@ Import Namespace="YAF.Classes.Core"%>

<%@ Register TagPrefix="uc1" TagName="smileys" Src="../controls/smileys.ascx" %>

<YAF:PageLinks runat="server" id="PageLinks" />
<table class="content" cellSpacing="1" cellPadding="4" width="60%" align="center">
	<tr>
		<td class="header1" align="center" colSpan="2"><asp:label id="Title" runat="server" /></td>
	</tr>
	<tr id="SubjectRow" runat="server">
		<td class="postformheader" width="20%"><YAF:LocalizedLabel runat="server" LocalizedTag="subject" /></td>
		<td class="post" width="80%">
			<asp:Label runat="server" ID="Subject" />
		</td>
	</tr>
	<tr id="PreviewRow" runat="server" visible="false">
		<td class="postformheader" valign="top"><YAF:LocalizedLabel runat="server" LocalizedTag="previewtitle" /></td>
		<td class="post" valign="top">
		    <YAF:MessagePost ID="MessagePreview" runat="server"></YAF:MessagePost>
		</td>
	</tr>
	<tr id="DeleteReasonRow" runat="server">
		<td class="postformheader" width="20%"><% = GetReasonText() %>
		</td>
		<td class="post" width="80%"><asp:textbox id="ReasonEditor" runat="server" cssclass="edit" MaxLength="100" /></td>
	</tr>
	<tr id="EraseRow" runat="server" visible="false">
		<td class="postformheader" width="20%"></td>
		<td class="post" width="80%"><asp:CheckBox ID="EraseMessage" runat="server" Checked="false" /><YAF:LocalizedLabel runat="server" LocalizedTag="erasemessage" /></td>
	</tr>
	<tr>
		<td align="center" colSpan="2" class="footer1">
			<asp:button id="Delete" cssclass="pbutton" runat="server" onclick="ToogleDeleteStatus_Click"/>
			<asp:Button id="Cancel" cssclass="pbutton" runat="server" onclick="Cancel_Click" />
			<br>
		</td>
	</tr>
	
</table>
<br>


<asp:repeater id="LinkedPosts" runat="server" visible="false">
<HeaderTemplate>
	<table class="content" cellSpacing="1" cellPadding="0" width="100%" align="center">
		<tr>
			<td class="header2"  align="center" colSpan="1">
			<asp:CheckBox id="DeleteAllPosts" onCheckedChanged="DeleteAllPosts_CheckedChanged1" AutoPostBack="True"  runat="server" />
			 Delete All Posts?</td>
			<td class="header2"  align="center" colSpan="1"><%# GetActionText() %></td>
			
		</tr>
		
</HeaderTemplate>
<FooterTemplate>
	</table>
</FooterTemplate>
<ItemTemplate>
		<tr class="postheader">
			<td width="140"><b><a href="<%# YafBuildLink.GetLink(ForumPages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "UserID")) %>"><%# DataBinder.Eval(Container.DataItem, "UserName") %></a></b>
			</td>
			<td width="80%" class="small" align="left"><b><YAF:LocalizedLabel runat="server" LocalizedTag="posted" /></b> <%# YafServices.DateTime.FormatDateTime( ( System.DateTime ) ( ( System.Data.DataRowView ) Container.DataItem ) ["Posted"] )%></td>
		</tr>
		<tr class="post">
			<td>&nbsp;</td>
			<td valign="top" class="message">
			    <YAF:MessagePostData ID="MessagePost1" runat="server" DataRow="<%# Container.DataItem %>" ShowAttachments="false"></YAF:MessagePostData>
			</td>
		</tr>
</ItemTemplate>
<AlternatingItemTemplate>
		<tr class="postheader">
			<td width="140"><b><a href="<%# YafBuildLink.GetLink(ForumPages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "UserID")) %>"><%# DataBinder.Eval(Container.DataItem, "UserName") %></a></b>
			</td>
			<td width="80%" class="small" align="left"><b><YAF:LocalizedLabel runat="server" LocalizedTag="posted" /></b> <%# YafServices.DateTime.FormatDateTime( ( System.DateTime ) ( ( System.Data.DataRowView ) Container.DataItem ) ["Posted"] )%></td>
		</tr>
		<tr class="post_alt">
			<td>&nbsp;</td>
			<td valign="top" class="message">
				<YAF:MessagePostData ID="MessagePostAlt" runat="server" DataRow="<%# Container.DataItem %>" ShowAttachments="false"></YAF:MessagePostData>
			</td>
		</tr>
</AlternatingItemTemplate>
</asp:repeater>

<!--
<iframe runat="server" Visible="false" id="LastPostsIFrame" name="lastposts" width="100%" height="300" frameborder="0" marginheight="2" marginwidth="2" scrolling="yes"></iframe>
-->
<div id="DivSmartScroller">
    <YAF:SmartScroller id="SmartScroller1" runat="server" />
</div>
