<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.ViewThanksList"
    CodeBehind="ViewThanksList.ascx.cs" %>
<table class="command" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td>
            <YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
        </td>
    </tr>
</table>
<asp:Repeater ID="ThanksRes" runat="server" OnItemCreated="ThanksRes_ItemCreated">
    <HeaderTemplate>
        <table class="content" cellspacing="1" cellpadding="0" width="100%">
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="header2">
            <td colspan="2">
                <b>
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="topic" />
                </b><a href="<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}#post{1}",Container.DataItemToField<int>("TopicID"),Container.DataItemToField<int>("MessageID")) %>">
                    <%# Container.DataItemToField<string>("Topic") %>
                </a>
            </td>
        </tr>
        <tr class="postheader">
            <td width="140px" id="ThanksNumberCell" valign="top" runat="server">
                <%# String.Format(PageContext.Localization.GetText("THANKSNUMBER"),  Container.DataItemToField<int?>("MessageThanksNumber")) %>
            </td>
            <td width="140px" id="NameCell" valign="top" runat="server">
                <a name="<%# Container.DataItemToField<int>("MessageID") %>" /><b>
                    <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# Container.DataItemToField<int>("UserID") %> ' />
                </b>
                <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( Container.DataItemToField<int>("UserID") )%>'
                    Style="vertical-align: bottom" UserID='<%# Container.DataItemToField<int>("UserID") %>' />
            </td>
            <td width="80%" class="postheader">
                <b>
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="POSTED" />
                </b>
                <%# YafServices.DateTime.FormatDateTime( Container.DataItemToField<DateTime>("Posted") )%>
            </td>
        </tr>
        <tr class="<%# this.IsOdd() ? "post_alt" : "post" %>">
            <td colspan="2">
                <YAF:MessagePostData ID="MessagePostPrimary" runat="server" ShowAttachments="false"
                    ShowSignature="false" DataRow="<%# Container.DataItem %>">
                </YAF:MessagePostData>
            </td>
        </tr>
    </ItemTemplate>
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
