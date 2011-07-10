<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.moderating" CodeBehind="moderating.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<%@ Register TagPrefix="YAF" TagName="TopicLine" Src="../controls/TopicLine.ascx" %>
<table class="content" cellspacing="1" cellpadding="0" width="100%">
    <tr>
        <td class="header1" colspan="4">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="MEMBERS" LocalizedPage="MODERATE" />
        </td>
    </tr>
    <tr class="header2">
        <td>
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="USER" LocalizedPage="MODERATE" />
        </td>
        <td align="center">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ACCEPTED" LocalizedPage="MODERATE" />
        </td>
        <td>
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ACCESSMASK" LocalizedPage="MODERATE" />
        </td>
        <td>
            &nbsp;
        </td>
    </tr>
    <asp:Repeater runat="server" ID="UserList" OnItemCommand="UserList_ItemCommand">
        <ItemTemplate>
            <tr class="post">
                <td>
                    <%# (Eval("DisplayName") != DBNull.Value) ? Eval("DisplayName") : Eval("DisplayName")%>
                </td>
                <td align="center">
                    <%# Eval("Accepted") %>
                </td>
                <td>
                    <%# Eval("Access") %>
                </td>
                <td>
                    <asp:LinkButton runat="server" Text='<%# this.GetText("EDIT")%>' CommandName="edit" CommandArgument='<%# Eval("UserID") %>' />
                    |
                    <asp:LinkButton runat="server" Text='<%# this.GetText("REMOVE")%>' OnLoad="DeleteUser_Load"
                        CommandName="remove" CommandArgument='<%# Eval("UserID") %>' />
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr class="footer1">
        <td colspan="4">
            <asp:LinkButton runat="server" ID="AddUser" Text="Invite User" OnClick="AddUser_Click" />
        </td>
    </tr>
</table>
<br />
<YAF:ThemeButton ID="DeleteTopic" runat="server" CssClass="yafcssbigbutton rightItem"
    TextLocalizedTag="BUTTON_DELETETOPIC" TitleLocalizedTag="BUTTON_DELETETOPIC_TT"
    OnLoad="Delete_Load" OnClick="DeleteTopics_Click" />
<YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" UsePostBack="True" />
<table class="content" cellspacing="1" cellpadding="0" width="100%">
    <tr>
        <td class="header1" colspan="6">
            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="title" LocalizedPage="MODERATE" />
        </td>
    </tr>
    <tr>
        <td class="header2" width="1%">
            &nbsp;
        </td>
        <td class="header2" width="1%">
            &nbsp;
        </td>
        <td class="header2" align="left">
            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="topics" LocalizedPage="MODERATE" />
        </td>
        <td class="header2" align="center" width="7%">
            <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="replies" LocalizedPage="MODERATE" />
        </td>
        <td class="header2" align="center" width="7%">
            <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="views" LocalizedPage="MODERATE" />
        </td>
        <td class="header2" align="center" width="15%">
            <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="lastpost" LocalizedPage="MODERATE" />
        </td>
    </tr>
    <asp:Repeater ID="topiclist" runat="server" OnItemCommand="topiclist_ItemCommand">
        <ItemTemplate>
            <YAF:TopicLine runat="server" DataRow="<%# Container.DataItem %>" AllowSelection="true" />
        </ItemTemplate>
    </asp:Repeater>
    <tr>
        <td class="footer1" colspan="6">
            &nbsp;
        </td>
    </tr>
</table>
<YAF:ThemeButton ID="DeleteTopics2" runat="server" CssClass="yafcssbigbutton rightItem"
    TextLocalizedTag="BUTTON_DELETETOPIC" TitleLocalizedTag="BUTTON_DELETETOPIC_TT"
    OnLoad="Delete_Load" OnClick="DeleteTopics_Click" />
<YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" UsePostBack="True" />
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
