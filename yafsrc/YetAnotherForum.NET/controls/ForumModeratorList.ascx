<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="ForumModeratorList.ascx.cs" Inherits="YAF.Controls.ForumModeratorList" %>
<asp:Repeater ID="ModeratorList" runat="server">
	<ItemTemplate>
		<asp:PlaceHolder ID="ModeratorUser" runat="server" Visible='<%# Convert.ToInt32(((System.Data.DataRow)Container.DataItem)["IsGroup"]) == 0 %>'>
			<YAF:UserLink ID="ModeratorUserLink" runat="server" UserID='<%# Convert.ToInt32(((System.Data.DataRow)Container.DataItem)["ModeratorID"]) %>' UserName='<%# ((System.Data.DataRow)Container.DataItem)["ModeratorName"].ToString() %>'/>
		</asp:PlaceHolder>
		<asp:PlaceHolder ID="ModeratorGroup" runat="server" Visible='<%# Convert.ToInt32(((System.Data.DataRow)Container.DataItem)["IsGroup"]) != 0 %>'>
			<b><%# ((System.Data.DataRow)Container.DataItem)["ModeratorName"].ToString() %></b>
		</asp:PlaceHolder>
	</ItemTemplate>
	<SeparatorTemplate>, </SeparatorTemplate>
</asp:Repeater>
<asp:PlaceHolder ID="BlankDash" runat="server">
-
</asp:PlaceHolder>