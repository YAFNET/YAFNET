<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.ViewThanksList" Codebehind="ViewThanksList.ascx.cs" %>
<table class="command" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td>
            <YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
        </td>
    </tr>
</table>
<asp:Repeater ID="ThanksRes" runat="server">
    <HeaderTemplate>
        <table class="content" cellspacing="1" cellpadding="0" width="100%">
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="header2">
            <td colspan="2">
                <b>
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="topic" />
                </b><a href="<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}&m={1}#post{1}",DataBinder.Eval(Container.DataItem, "TopicID"),DataBinder.Eval(Container.DataItem, "MessageID")) %>">
                    <%# DataBinder.Eval(Container.DataItem, "Topic") %>
                </a>
            </td>
        </tr>
        <tr class="postheader">
            <td width="140px" id="NameCell" valign="top">
                <a name="<%# DataBinder.Eval(Container.DataItem, "MessageID") %>" /><b>
                    <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# DataBinder.Eval(Container.DataItem, "UserID") %> ' />
                </b>
                <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( DataBinder.Eval(Container.DataItem, "UserID") )%>'
                    Style="vertical-align: bottom" UserID='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' />
            </td>
            <td width="80%" class="postheader">
                <b>
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="POSTED" />
                </b>
                <%# YafServices.DateTime.FormatDateTime( ( System.DateTime ) DataBinder.Eval( Container.DataItem, "Posted" ) )%>
            </td>
        </tr>
        <tr class="post">
            <td colspan="2">
                <YAF:MessagePostData ID="MessagePostPrimary" runat="server" ShowAttachments="false"
                    ShowSignature="false" DataRow="<%# ( System.Data.DataRowView )Container.DataItem %>">
                </YAF:MessagePostData>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="header2">
            <td colspan="2">
                <b>
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="topic" />
                </b><a href="<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}&m={1}#post{1}",DataBinder.Eval(Container.DataItem, "TopicID"),DataBinder.Eval(Container.DataItem, "MessageID")) %>">
                    <%# DataBinder.Eval(Container.DataItem, "Topic") %>
                </a>
            </td>
        </tr>
        <tr class="postheader">
            <td width="140px" id="NameCell" valign="top">
                <a name="<%# DataBinder.Eval(Container.DataItem, "MessageID") %>" /><b>
                    <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' />
                </b>
                <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( DataBinder.Eval(Container.DataItem, "UserID") )%>'
                    Style="vertical-align: bottom" UserID='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' />
            </td>
            <td width="80%" class="postheader">
                <b>
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="POSTED" />
                </b>
                <%# YafServices.DateTime.FormatDateTime( ( System.DateTime ) DataBinder.Eval( Container.DataItem, "Posted" ) )%>
            </td>
        </tr>
        <tr class="post_alt">
            <td colspan="2">
                <YAF:MessagePostData ID="MessagePostAlt" runat="server" ShowAttachments="false" ShowSignature="false"
                    DataRow="<%# ( System.Data.DataRowView )Container.DataItem %>">
                </YAF:MessagePostData>
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        <tr>
            <td class="footer1" colspan="2">
            </td>
        </tr>
        </table>
    </FooterTemplate>
</asp:Repeater>
<asp:PlaceHolder ID="NoResults" runat="Server" Visible="false">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="postheader" colspan="2" align="center">
                <br />
                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="NO_THANKS" />
                <br />
            </td>
        </tr>
        <tr>
            <td class="footer1" colspan="2">
            </td>
        </tr>
    </table>
</asp:PlaceHolder>
<table class="command" width="100%" cellspacing="0" cellpadding="0">
    <tr>
        <td>
            <YAF:Pager runat="server" ID="PagerBottom" LinkedPager="PagerTop" OnPageChange="Pager_PageChange" />
        </td>
    </tr>
</table>
