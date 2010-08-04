<%@ Control Language="c#" CodeFile="cp_subscriptions.ascx.cs" AutoEventWireup="True"
    Inherits="YAF.Pages.cp_subscriptions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:UpdatePanel ID="SubscriptionsUpdatePanel" runat="server">
    <ContentTemplate>
        <table class="content" cellspacing="1" cellpadding="0" width="100%">
            <tr>
                <td class="header1" colspan="5">
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="forums" />
                </td>
            </tr>
            <tr>
                <td class="header2">
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="forum" />
                </td>
                <td class="header2" align="center">
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="topics" />
                </td>
            </tr>
            <asp:Repeater ID="ForumList" runat="server">
                <ItemTemplate>
                    <asp:Label ID="tfid" runat="server" Text='<%# Eval("WatchForumID") %>' Visible="false" />
                    <tr>
                        <td class="post">
                            <%# Eval("ForumName") %>
                        </td>
                        <td class="post" align="center">
                            <asp:CheckBox ID="unsubf" runat="server" /></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr>
                <td class="footer1" colspan="5" align="right">
                    <asp:Button ID="UnsubscribeForums" runat="server" OnClick="UnsubscribeForums_Click" /></td>
            </tr>
        </table>
        <br />
        <table class="content" cellspacing="1" cellpadding="0" width="100%">
            <tr>
                <td class="header1" colspan="5">
                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="topics" />
                </td>
            </tr>
            <tr>
                <td class="header2">
                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="topic" />
                </td>
            </tr>
            <asp:Repeater ID="TopicList" runat="server">
                <ItemTemplate>
                    <asp:Label ID="ttid" runat="server" Text='<%# Eval("WatchTopicID") %>' Visible="false" />
                    <tr>
                        <td class="post">
                            <%# Eval("TopicName") %>
                        </td>
                        <td class="post" align="center">
                            <asp:CheckBox ID="unsubx" runat="server" /></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr>
                <td class="footer1" colspan="5" align="right">
                    <asp:Button ID="UnsubscribeTopics" runat="server" OnClick="UnsubscribeTopics_Click" /></td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
