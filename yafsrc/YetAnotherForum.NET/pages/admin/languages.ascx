<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.languages" Codebehind="languages.ascx.cs" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="8">
				Languages
			</td>
		</tr>
		<asp:Repeater runat="server" ID="List">
			<HeaderTemplate>
				<tr class="header2">
                    <td>
						Language English Name
					</td>
                    <td>
						Culture Tag
					</td>
                     <td>
						Language Native Name
					</td>
					<td>
						File
					</td>
                    <td>
						&nbsp;
					</td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr class="post">
                    <td>
						<%# Eval("CultureEnglishName")%>
					</td>
                    <td>
						<%# Eval("CultureTag")%>
					</td>
                     <td>
						<%# Eval("CultureNativeName")%>
					</td>
					<td>
						<%# Eval("CultureFile")%>
					</td>
                    <td>
						<asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval("CultureFile")%>'>Edit</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
