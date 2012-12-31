<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.search" CodeBehind="search.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Register TagPrefix="YAF" TagName="DialogBox" Src="../controls/DialogBox.ascx" %>
<%@ Register Namespace="nStuff.UpdateControls" Assembly="nStuff.UpdateControls" TagPrefix="nStuff" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<script type="text/javascript">
    function EndRequestHandler(sender, args) {
        jQuery().YafModalDialog.Close({ Dialog: '#<%=LoadingModal.ClientID%>' });
    }
    function ShowLoadingDialog() {
        jQuery().YafModalDialog.Show({ Dialog: '#<%=LoadingModal.ClientID%>', ImagePath: '<%=YafForumInfo.GetURLToResource("images/")%>' }); 
    }
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
</script>
<nStuff:UpdateHistory ID="UpdateHistory" runat="server" OnNavigate="OnUpdateHistoryNavigate" />
<table cellpadding="0" cellspacing="1" class="content searchContent" width="100%">
    <tr>
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel runat="server" LocalizedTag="title" />
        </td>
    </tr>
    <tr>
        <td align="center" class="postheader" colspan="2">
            <asp:DropDownList ID="listForum" runat="server" />
            <asp:DropDownList ID="listResInPage" runat="server" />
        </td>
    </tr>
    <tr>
        <td align="right" class="postheader" width="35%">
            <YAF:LocalizedLabel runat="server" LocalizedTag="postedby" />
        </td>
        <td align="left" class="postheader">
            <asp:TextBox ID="txtSearchStringFromWho" runat="server" Width="350px" />
            <asp:DropDownList ID="listSearchFromWho" runat="server" />
        </td>
    </tr>
    <tr>
        <td align="right" class="postheader" width="35%">
            <YAF:LocalizedLabel runat="server" LocalizedTag="posts" />
        </td>
        <td align="left" class="postheader">
            <asp:TextBox ID="txtSearchStringWhat" runat="server" Width="350px" />
            <asp:DropDownList ID="listSearchWhat" runat="server" />
        </td>
    </tr>
    <tr>
        <td align="center" class="postfooter" colspan="2">
            <asp:Button ID="btnSearch" runat="server" CssClass="pbutton" OnClick="btnSearch_Click"
                OnClientClick="ShowLoadingDialog(); return true;" Visible="false" />
            <asp:Button ID="btnSearchExt1" runat="server" CssClass="pbutton" Visible="false"
                OnClick="BtnExtSearch1_Click" />
            <asp:Button ID="btnSearchExt2" runat="server" CssClass="pbutton" Visible="false"
                OnClick="BtnExtSearch2_Click" />
        </td>
    </tr>
</table>
<br />
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSearch" />
    </Triggers>
    <ContentTemplate>
        <YAF:Pager runat="server" ID="Pager" OnPageChange="Pager_PageChange" />
        <asp:Repeater ID="SearchRes" runat="server" OnItemDataBound="SearchRes_ItemDataBound">
            <HeaderTemplate>
                <table class="content" cellspacing="1" cellpadding="0" width="100%">
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="RESULTS" />
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="header2">           
                    <td colspan="2">
                        <strong>
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="topic" />
                        </strong><a title='<%# this.GetText("COMMON", "VIEW_TOPIC") %>' href="<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}", Container.DataItemToField<int>("TopicID")) %>">
                            <%# HtmlEncode(Container.DataItemToField<string>("Topic")) %>
                        </a>
                        <a id="AncPost"  href="<%# YafBuildLink.GetLink(ForumPages.posts,"m={0}#post{0}", Container.DataItemToField<int>("MessageID")) %>" runat="server">&nbsp;
                           <img id="ImgPost" runat="server" title='<%#  this.GetText("GO_LAST_POST") %>' alt='<%#  this.GetText("GO_TO_LASTPOST") %>' src='<%#  GetThemeContents("ICONS", "ICON_LATEST") %>' />
                        </a>
                    </td>
                </tr>
                <tr class="postheader">
                    <td width="140px" id="NameCell" valign="top">
                        <a name="<%# Container.DataItemToField<int>("MessageID") %>" /><strong>
                            <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# Container.DataItemToField<int>("UserID") %>' />
                        </strong>
                        <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# this.Get<YafBoardSettings>().ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( Container.DataItemToField<int>("UserID") )%>'
                            Style="vertical-align: bottom" UserID='<%# Container.DataItemToField<int>("UserID") %>' />
                    </td>
                    <td width="80%" class="postheader">
                        <strong>
                            <YAF:LocalizedLabel runat="server" LocalizedTag="POSTED" />
                        </strong>
                        <YAF:DisplayDateTime id="LastVisitDateTime" runat="server" DateTime='<%# Container.DataItemToField<DateTime>("Posted") %>'></YAF:DisplayDateTime>
                    </td>
                </tr>
                <tr class="post">
                    <td colspan="2">
                        <YAF:MessagePostData ID="MessagePostPrimary" runat="server" ShowAttachments="false"
                            ShowSignature="false" HighlightWords="<%# this.HighlightSearchWords %>" DataRow="<%# Container.DataItem %>">
                        </YAF:MessagePostData>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="header2">
                    <td colspan="2">
                        <strong>
                            <YAF:LocalizedLabel runat="server" LocalizedTag="topic" />
                        </strong><a title='<%# this.GetText("COMMON", "VIEW_TOPIC") %>' alt='<%# this.GetText("COMMON", "VIEW_TOPIC") %>' href="<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}", Container.DataItemToField<int>("TopicID")) %>">
                            <%# Container.DataItemToField<string>("Topic") %>
                        </a>
                         <a id="AncAltPost"    href="<%# YafBuildLink.GetLink(ForumPages.posts,"m={0}#post{0}", Container.DataItemToField<int>("MessageID")) %>" >&nbsp;
                           <img id="ImgAltPost" title='<%#  this.GetText("GO_LAST_POST") %>' alt='<%#  this.GetText("GO_TO_LASTPOST") %>' src='<%#  GetThemeContents("ICONS", "ICON_LATEST") %>' />
                        </a>
                    </td>
                </tr>
                <tr class="postheader">
                    <td width="140px" id="NameCell" valign="top">
                        <a name="<%# Container.DataItemToField<int>("MessageID") %>" /><strong>
                            <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# Container.DataItemToField<int>("UserID") %>' />
                        </strong>
                        <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# this.Get<YafBoardSettings>().ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( Container.DataItemToField<int>("UserID") )%>'
                            Style="vertical-align: bottom" UserID='<%# Container.DataItemToField<int>("UserID") %>' />
                    </td>
                    <td width="80%" class="postheader">
                        <strong>
                            <YAF:LocalizedLabel runat="server" LocalizedTag="POSTED" />
                        </strong>
                        <YAF:DisplayDateTime id="LastVisitDateTime" runat="server" DateTime='<%# Container.DataItemToField<DateTime>("Posted") %>'></YAF:DisplayDateTime>
                    </td>
                </tr>
                <tr class="post_alt">
                    <td colspan="2">
                        <YAF:MessagePostData ID="MessagePostAlt" runat="server" ShowAttachments="false" ShowSignature="false"
                            HighlightWords="<%# this.HighlightSearchWords %>" DataRow="<%# Container.DataItem %>">
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
                    <td class="header1" colspan="2">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="RESULTS" />
                    </td>
                </tr>
                <tr>
                    <td class="postheader" colspan="2" align="center">
                        <br />
                        <YAF:LocalizedLabel runat="server" LocalizedTag="NO_SEARCH_RESULTS" />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="footer1" colspan="2">
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>
        <YAF:Pager ID="Pager1" runat="server" LinkedPager="Pager" />
    </ContentTemplate>
</asp:UpdatePanel>

<YAF:MessageBox ID="LoadingModal" runat="server"></YAF:MessageBox>

<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
