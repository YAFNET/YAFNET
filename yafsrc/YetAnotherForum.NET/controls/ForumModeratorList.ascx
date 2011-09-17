<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumModeratorList" Codebehind="ForumModeratorList.ascx.cs" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>

<asp:Repeater ID="ModeratorList" runat="server">
    <HeaderTemplate>
     <YAF:ThemeImage ID="ModImage" ThemeTag="MODS_SMALL" runat="server" />
     <%# this.GetText("DEFAULT", "MODERATORS")%>:
    </HeaderTemplate>
    <ItemTemplate>
        <asp:PlaceHolder ID="ModeratorUser" runat="server" Visible='<%# ((DataRow)Container.DataItem)["IsGroup"].ToType<int>() == 0 %>'>
            <YAF:UserLink ID="ModeratorUserLink" runat="server" UserID='<%# ((DataRow)Container.DataItem)["ModeratorID"].ToType<int>() %>'
                /></asp:PlaceHolder><asp:PlaceHolder
                ID="ModeratorGroup" runat="server" Visible='<%# ((DataRow)Container.DataItem)["IsGroup"].ToType<int>() != 0 %>'><strong><%# ((DataRow)Container.DataItem)["ModeratorName"].ToString() %></strong></asp:PlaceHolder></ItemTemplate>
    <SeparatorTemplate>, </SeparatorTemplate>
</asp:Repeater>
<asp:PlaceHolder ID="BlankDash" runat="server">- </asp:PlaceHolder>
