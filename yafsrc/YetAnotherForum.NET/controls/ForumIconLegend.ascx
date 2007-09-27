<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="ForumIconLegend.ascx.cs" Inherits="YAF.Controls.ForumIconLegend" %>

<table style="padding: 2px; margin: 2px" width="100%">
	<tr>
		<td>
			<asp:Image ID="Forum_New" Style="vertical-align: middle" runat="server" /> <%# PageContext.Localization.GetText("ICONLEGEND","New_Posts") %>
			<asp:Image ID="Forum" Style="vertical-align: middle" runat="server" /> <%# PageContext.Localization.GetText("ICONLEGEND","No_New_Posts") %>
			<asp:Image ID="Forum_Locked" Style="vertical-align: middle" runat="server" /> <%# PageContext.Localization.GetText("ICONLEGEND","Forum_Locked") %>
		</td>
	</tr>
</table>
