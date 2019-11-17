<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumModeratorList" Codebehind="ForumModeratorList.ascx.cs" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<asp:Repeater ID="ModeratorList" runat="server">
    <HeaderTemplate>
        <ul class="list-inline">
            <li class="list-inline-item">
                <span class="font-weight-bold">
                    <i class="fa fa-user-secret fa-fw"></i>&nbsp;<%# this.GetText("DEFAULT", "MODERATORS")%>:
                </span>
            </li>
    </HeaderTemplate>
    <ItemTemplate>
        <asp:PlaceHolder ID="ModeratorUser" runat="server" Visible='<%# ((DataRow)Container.DataItem)["IsGroup"].ToType<int>() == 0 %>'>
            <li class="list-inline-item">
                <YAF:UserLink ID="ModeratorUserLink" runat="server" 
                          UserID='<%# ((DataRow)Container.DataItem)["ModeratorID"].ToType<int>()  %>' 
                          ReplaceName='<%#  ((DataRow)Container.DataItem)[this.Get<YafBoardSettings>().EnableDisplayName ? "ModeratorDisplayName" : "ModeratorName"].ToString() %>' />
            </li>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ModeratorGroup" runat="server" 
                         Visible='<%# ((DataRow)Container.DataItem)["IsGroup"].ToType<int>() != 0 %>'>
            <li class="list-inline-item">
                <%#  ((DataRow)Container.DataItem)[this.Get<YafBoardSettings>().EnableDisplayName ? "ModeratorDisplayName": "ModeratorName"].ToString()%>
            </li>
        </asp:PlaceHolder>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
<asp:PlaceHolder ID="BlankDash" runat="server">- </asp:PlaceHolder>
