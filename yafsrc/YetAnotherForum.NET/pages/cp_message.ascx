<%@ Control Language="c#" CodeFile="cp_message.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.cp_message" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:Repeater ID="Inbox" runat="server" OnItemCommand="Inbox_ItemCommand">
	<HeaderTemplate>
		<table class="content" cellspacing="1" cellpadding="0" width="100%">
	</HeaderTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
	<SeparatorTemplate>
		<tr class="postsep">
			<td colspan="2" style="height: 7px">
			</td>
		</tr>
	</SeparatorTemplate>
	<ItemTemplate>
		<tr>
			<td class="header1" colspan="2">
				<%# HtmlEncode(Eval("Subject")) %>
			</td>
		</tr>
		<tr>
			<td class="postheader">
				<b><YAF:UserLink ID="FromUserLink" runat="server" UserID='<%# Convert.ToInt32(Eval( "FromUserID" )) %>' UserName='<%# Convert.ToString(Eval( "FromUser" )) %>' /></b>
			</td>
			<td class="postheader" width="80%">
				<table cellspacing="0" cellpadding="0" width="100%">
					<tr>
						<td>
							<b>
								<%# GetText("posted") %>
							</b>
							<%# YafDateTime.FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Created"]) %>
						</td>
						<td>
						    <YAF:ThemeButton ID="DeleteMessage" runat="server" CssClass="yaflittlebutton" CommandName="delete" CommandArgument='<%# Eval("UserPMessageID") %>' TextLocalizedTag="BUTTON_DELETE" TitleLocalizedTag="BUTTON_DELETE_TT" OnLoad="ThemeButtonDelete_Load" />
						    <YAF:ThemeButton ID="ReplyMessage" runat="server" CssClass="yaflittlebutton" CommandName="reply" CommandArgument='<%# Eval("UserPMessageID") %>' TextLocalizedTag="BUTTON_REPLY" TitleLocalizedTag="BUTTON_REPLY_TT" />
						    <YAF:ThemeButton ID="QuoteMessage" runat="server" CssClass="yaflittlebutton" CommandName="quote" CommandArgument='<%# Eval("UserPMessageID") %>' TextLocalizedTag="BUTTON_QUOTE" TitleLocalizedTag="BUTTON_QUOTE_TT" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td class="post">
				&nbsp;</td>
			<td class="post" valign="top">
				<%# FormatMsg.FormatMessage(Eval("Body") as string, Convert.ToInt32(Eval("Flags"))) %>
			</td>
		</tr>
	</ItemTemplate>
</asp:Repeater>
<div id="DivSmartScroller">
    <YAF:SmartScroller id="SmartScroller1" runat="server" />
</div>
