<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumModeratorList" Codebehind="ForumModeratorList.ascx.cs" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<asp:Repeater ID="ModeratorList" runat="server">
    <HeaderTemplate>
        <p><small>
        <span class="font-weight-bold">
            <i class="fa fa-user-secret fa-fw"></i>&nbsp;<%# this.GetText("DEFAULT", "MODERATORS")%>:
        </span>
    </HeaderTemplate>
    <ItemTemplate>
        <asp:PlaceHolder ID="ModeratorUser" runat="server" Visible='<%# ((DataRow)Container.DataItem)["IsGroup"].ToType<int>() == 0 %>'>
            <YAF:UserLink ID="ModeratorUserLink" runat="server" 
                          UserID='<%# ((DataRow)Container.DataItem)["ModeratorID"].ToType<int>()  %>' 
                          ReplaceName='<%# this.Get<YafBoardSettings>().EnableDisplayName ? ((DataRow)Container.DataItem)["ModeratorDisplayName"].ToString() : ((DataRow)Container.DataItem)["ModeratorName"].ToString() %>' />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ModeratorGroup" runat="server" 
                         Visible='<%# ((DataRow)Container.DataItem)["IsGroup"].ToType<int>() != 0 %>'>
            <%# this.Get<YafBoardSettings>().EnableDisplayName ? ((DataRow)Container.DataItem)["ModeratorDisplayName"].ToString() : ((DataRow)Container.DataItem)["ModeratorName"].ToString()%>
        </asp:PlaceHolder>
    </ItemTemplate>
    <FooterTemplate>
        </small></p>
    </FooterTemplate>
</asp:Repeater>
<asp:PlaceHolder ID="BlankDash" runat="server">- </asp:PlaceHolder>
