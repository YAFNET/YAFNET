<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.attachments" Codebehind="attachments.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="8">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_ATTACHMENTS" />
			</td>
		</tr>
		<asp:Repeater runat="server" ID="List">
			<HeaderTemplate>
				<tr class="header2">
					<td>
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="FORUM" />
					</td>
					<td>
						<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TOPIC" />
					</td>
					<td>
						<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="POSTED" LocalizedPage="ADMIN_ATTACHMENTS" />
					</td>
					<td>
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="FILENAME" />
					</td>
					<td align="right">
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="DOWNLOADS" />
					</td>
					<td>
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="CONTENT_TYPE" />
					</td>
					<td align="right">
						<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="SIZE" />
					</td>
					<td>
						&nbsp;
					</td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr class="post">
					<td>
						<a target="_top" href='<%# YafBuildLink.GetLink(ForumPages.topics,"f={0}",Eval("ForumID")) %>'>
							<%# Eval("ForumName") %>
						</a>
					</td>
					<td>
						<a target="_top" href='<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}",Eval("TopicID")) %>'>
							<%# Eval("TopicName") %>
						</a>
					</td>
					<td>
						<%# this.Get<IDateTime>().FormatDateTimeShort(Eval( "Posted")) %>
					</td>
					<td>
						<%# Eval( "FileName") %>
					</td>
					<td align="right">
						<%# Eval( "Downloads") %>
					</td>
					<td>
						<%# Eval( "ContentType") %>
					</td>
					<td align="right">
						<%# Eval( "Bytes") %>
					</td>
					<td>
						<asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "AttachmentID") %>'><%# this.GetText("DELETE")%></asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
