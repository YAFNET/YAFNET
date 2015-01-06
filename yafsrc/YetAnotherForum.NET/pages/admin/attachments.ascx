<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.attachments" Codebehind="attachments.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
     <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="9">
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
						<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="USER" />
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
						<a target="_top" href='<%# YafBuildLink.GetLink(ForumPages.topics,"f={0}&name={1}",Eval("ForumID"), this.Eval("ForumName")) %>'>
							<%# this.HtmlEncode(this.Eval("ForumName")) %>
						</a>
					</td>
					<td>
						<a target="_top" href='<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}",Eval("TopicID")) %>'>
							<%# this.HtmlEncode(this.Eval("TopicName")) %>
						</a>
					</td>
					<td>
						<%# this.Get<IDateTime>().FormatDateTimeShort(this.Eval("Posted")) %>
					</td>
                    <td>
						<YAF:UserLink ID="UserProfileLink" runat="server" UserID='<%# this.Eval("UserID").ToType<int>() %>' />
					</td>
					<td>
						<%# this.HtmlEncode(this.Eval( "FileName")) %>
					</td>
					<td align="right">
						<%# this.Eval( "Downloads") %>
					</td>
					<td>
						<%# this.Eval( "ContentType") %>
					</td>
					<td align="right">
						<%# this.Eval( "Bytes") %>
					</td>
					<td align="right">
						<YAF:ThemeButton ID="ThemeButtonDelete" CssClass="yaflittlebutton" 
                                    CommandName='delete' CommandArgument='<%# this.Eval( "AttachmentID") %>' 
                                    TitleLocalizedTag="DELETE" 
                                    ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON"
                                    TextLocalizedTag="DELETE"
                                    OnLoad="Delete_Load"  runat="server">
                                </YAF:ThemeButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>
     <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
