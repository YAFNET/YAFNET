<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.posts" Codebehind="posts.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Controls" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Register TagPrefix="YAF" TagName="DisplayPost" Src="../controls/DisplayPost.ascx" %>
<%@ Register TagPrefix="YAF" TagName="DisplayConnect" Src="../controls/DisplayConnect.ascx" %>
<%@ Register TagPrefix="YAF" TagName="DisplayAd" Src="../controls/DisplayAd.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PollList" Src="../controls/PollList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="SimilarTopics" Src="../controls/SimilarTopics.ascx" %>
<%@ Register TagPrefix="modal" TagName="QuickReply" Src="../Dialogs/QuickReply.ascx" %>

<YAF:PageLinks ID="PageLinks" runat="server" />

<YAF:PollList ID="PollList" TopicId='<%# this.PageContext.PageTopicID %>' 
              ShowButtons='<%# this.ShowPollButtons() %>' 
              Visible='<%# this.PollGroupId() > 0 %>' 
              PollGroupId='<%# this.PollGroupId() %>' runat="server"/>

<a id="top"></a>

<div class="row mb-3">
    <div class="col-3 align-self-start">
        <YAF:Pager ID="Pager" runat="server" UsePostBack="False" />
    </div>
    <div class="col align-self-end">
        <span id="dvFavorite1">
                <YAF:ThemeButton ID="TagFavorite1" runat="server" 
                                 Type="Secondary"
                                 TextLocalizedTag="BUTTON_TAGFAVORITE" TitleLocalizedTag="BUTTON_TAGFAVORITE_TT"
                                 Icon="star" />
            </span>        
            <YAF:ThemeButton ID="MoveTopic1" runat="server" 
                             Type="Secondary"
                             OnClick="MoveTopic_Click" 
                             TextLocalizedTag="BUTTON_MOVETOPIC" TitleLocalizedTag="BUTTON_MOVETOPIC_TT"
                             Icon="arrows-alt" />
            <YAF:ThemeButton ID="UnlockTopic1" runat="server" 
                             Type="Warning"
                             OnClick="UnlockTopic_Click" 
                             TextLocalizedTag="BUTTON_UNLOCKTOPIC" TitleLocalizedTag="BUTTON_UNLOCKTOPIC_TT"
                             Icon="lock-open" />
            <YAF:ThemeButton ID="LockTopic1" runat="server" 
                             Type="Warning"
                             OnClick="LockTopic_Click" 
                             TextLocalizedTag="BUTTON_LOCKTOPIC" TitleLocalizedTag="BUTTON_LOCKTOPIC_TT"
                             Icon="lock" />
            <YAF:ThemeButton ID="DeleteTopic1" runat="server" 
                             Type="Danger"
                             OnClick="DeleteTopic_Click"
                             ReturnConfirmText='<%# this.GetText("confirm_deletetopic") %>'
                             TextLocalizedTag="BUTTON_DELETETOPIC" TitleLocalizedTag="BUTTON_DELETETOPIC_TT"
                             Icon="trash" />
            <YAF:ThemeButton ID="NewTopic1" runat="server" 
                             Type="Secondary"
                             OnClick="NewTopic_Click" 
                             TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT"
                             Icon="comment" />
            <YAF:ThemeButton ID="PostReplyLink1" runat="server" 
                             Type="Primary"
                             OnClick="PostReplyLink_Click" 
                             TextLocalizedTag="BUTTON_POSTREPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT"
                             Icon="reply" />
            <YAF:ThemeButton ID="QuickReplyLink1" runat="server" 
                             Type="Primary"
                             TextLocalizedTag="QUICKREPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT"
                             Icon="reply" DataTarget="QuickReplyDialog"/>
    </div>
</div>
<div class="clearfix mb-3">
    <nav class="navbar navbar-expand-lg navbar-light bg-light navbar-round">
        <asp:HyperLink ID="TopicLink" runat="server" CssClass="navbar-brand">
            <asp:Label ID="TopicTitle" runat="server" />
        </asp:HyperLink>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
            <ul class="navbar-nav">
                <li class="nav-item">
                    <YAF:ThemeButton runat="server" ID="PrevTopic"
                                     Type="Link"
                                     OnClick="PrevTopic_Click"
                                     TextLocalizedTag="PREVTOPIC"
                                     TitleLocalizedTag="PREVTOPIC"
                                     Icon="arrow-circle-left">
                    </YAF:ThemeButton>
                </li>
                <li class="nav-item">
                    <YAF:ThemeButton runat="server" ID="NextTopic"
                                     Type="Link"
                                     OnClick="NextTopic_Click"
                                     TextLocalizedTag="NEXTTOPIC"
                                     TitleLocalizedTag="NEXTTOPIC"
                                     Icon="arrow-circle-right">
                    </YAF:ThemeButton>
                </li>

            </ul>
            <ul class="navbar-nav  ml-auto">
                <li class="nav-item dropdown">
                    <div id="fb-root"></div>
                    <YAF:ThemeButton runat="server" ID="ShareLink"
                                     TextLocalizedTag="SHARE" TitleLocalizedTag="SHARE_TOOLTIP"
                                     Icon="share" 
                                     Type="Link" 
                                     CssClass="dropdown-toggle"
                                     DataToggle="dropdown">
                    </YAF:ThemeButton>
                    <YAF:PopMenu ID="ShareMenu" runat="server" Control="ShareLink" />
                </li>
                <li class="nav-item dropdown">
                    <YAF:ThemeButton runat="server" ID="OptionsLink"
                                     TextLocalizedTag="TOOLS" TitleLocalizedTag="OPTIONS_TOOLTIP"
                                     Icon="cog"
                                     Type="Link"
                                     CssClass="dropdown-toggle"
                                     DataToggle="dropdown">
                    </YAF:ThemeButton>
                    <asp:UpdatePanel ID="PopupMenuUpdatePanel" runat="server" style="display:inline">
                        <ContentTemplate>
                            <span id="WatchTopicID" runat="server" visible="false"></span>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <YAF:PopMenu runat="server" ID="OptionsMenu" Control="OptionsLink" />
                </li>
                <li class="nav-item dropdown">
                    <asp:PlaceHolder ID="ViewOptions" runat="server">
                        <YAF:ThemeButton runat="server" ID="ViewLink"
                                         TextLocalizedTag="VIEW" TitleLocalizedTag="VIEW_TOOLTIP"
                                         Icon="book"
                                         Type="Link"
                                         CssClass="dropdown-toggle"
                                         DataToggle="dropdown">
                        </YAF:ThemeButton>
                    
                    </asp:PlaceHolder>
                    <YAF:PopMenu ID="ViewMenu" runat="server" Control="ViewLink" />
                </li>
                <li class="nav-item">
                    <YAF:ThemeButton ID="ImageMessageLink" runat="server" Type="Link" Icon="fast-forward"
                                     TextLocalizedTag="GO_LAST_POST" TextLocalizedPage="DEFAULT">
                    </YAF:ThemeButton>
                </li>
                <li class="nav-item">
                    <YAF:ThemeButton ID="ImageLastUnreadMessageLink" runat="server" Type="Link" Icon="step-forward"
                                     TextLocalizedTag="GO_LASTUNREAD_POST" TextLocalizedPage="DEFAULT">
                    </YAF:ThemeButton>
                </li>
            </ul>
        </div>
    </nav>
</div>
<asp:Repeater ID="MessageList" runat="server" OnItemCreated="MessageList_OnItemCreated">
    <ItemTemplate>
        <table class="content postContainer" width="100%">
            <%# GetThreadedRow(Container.DataItem) %>
            <YAF:DisplayPost ID="DisplayPost1" runat="server" DataRow="<%# Container.DataItem %>"
                Visible="<%#IsCurrentMessage(Container.DataItem)%>" PostCount="<%# Container.ItemIndex %>" CurrentPage="<%# Pager.CurrentPageIndex %>" IsThreaded="<%#IsThreaded%>" />
        </table>
        <YAF:DisplayAd ID="DisplayAd" runat="server" Visible="False" />
        <YAF:DisplayConnect ID="DisplayConnect" runat="server" Visible="False" />
    </ItemTemplate>
    <AlternatingItemTemplate>        
        <table class="content postContainer_Alt" width="100%">
            <%# GetThreadedRow(Container.DataItem) %>
            <YAF:DisplayPost ID="DisplayPostAlt" runat="server" DataRow="<%# Container.DataItem %>"
                IsAlt="True" Visible="<%#IsCurrentMessage(Container.DataItem)%>" PostCount="<%# Container.ItemIndex %>" CurrentPage="<%# Pager.CurrentPageIndex %>" IsThreaded="<%#IsThreaded%>" />
        </table>
        <YAF:DisplayAd ID="DisplayAd" runat="server" Visible="False" />
        <YAF:DisplayConnect ID="DisplayConnect" runat="server" Visible="False" />
    </AlternatingItemTemplate>
</asp:Repeater>
<table class="header2 postNavigation" width="100%"  id="tbFeeds" runat="server" visible="<%# this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().PostsFeedAccess) %>">
<tr>
<td class="post">
    <YAF:RssFeedLink ID="RssFeed" runat="server" 
                     FeedType="Posts"  
                     AdditionalParameters='<%# "t={0}".FormatWith(this.PageContext.PageTopicID) %>' 
                     Visible="<%# this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().PostsFeedAccess) %>" 
                     />
</td>
</tr>
</table>                           
<table class="content postForumUsers" width="100%">
    <YAF:ForumUsers ID="ForumUsers1" runat="server" />
</table>
<YAF:SimilarTopics ID="SimilarTopics"  runat="server" Topic='<%# this.PageContext.PageTopicName %>'>
</YAF:SimilarTopics>
<table cellpadding="0" cellspacing="0" class="command" width="100%">
    <tr>
        <td align="left">
            <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="Pager" UsePostBack="false" />
        </td>
        <td>
            <span id="dvFavorite2">
                <YAF:ThemeButton ID="TagFavorite2" runat="server" CssClass="yafcssbigbutton rightItem button-favorite"
                    TextLocalizedTag="BUTTON_TAGFAVORITE" TitleLocalizedTag="BUTTON_TAGFAVORITE_TT" />
            </span>        
            <YAF:ThemeButton ID="MoveTopic2" runat="server" CssClass="yafcssbigbutton rightItem button-move"
                OnClick="MoveTopic_Click" TextLocalizedTag="BUTTON_MOVETOPIC" TitleLocalizedTag="BUTTON_MOVETOPIC_TT" />
            <YAF:ThemeButton ID="UnlockTopic2" runat="server" CssClass="yafcssbigbutton rightItem button-unlock"
                OnClick="UnlockTopic_Click" TextLocalizedTag="BUTTON_UNLOCKTOPIC" TitleLocalizedTag="BUTTON_UNLOCKTOPIC_TT" />
            <YAF:ThemeButton ID="LockTopic2" runat="server" CssClass="yafcssbigbutton rightItem button-lock"
                OnClick="LockTopic_Click" TextLocalizedTag="BUTTON_LOCKTOPIC" TitleLocalizedTag="BUTTON_LOCKTOPIC_TT" />
            <YAF:ThemeButton ID="DeleteTopic2" runat="server" CssClass="yafcssbigbutton rightItem button-delete"
                OnClick="DeleteTopic_Click" ReturnConfirmText='<%# this.GetText("confirm_deletetopic") %>' TextLocalizedTag="BUTTON_DELETETOPIC"
                TitleLocalizedTag="BUTTON_DELETETOPIC_TT" />
            <YAF:ThemeButton ID="NewTopic2" runat="server" CssClass="yafcssbigbutton rightItem button-newtopic"
                OnClick="NewTopic_Click" TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" />
            <YAF:ThemeButton ID="PostReplyLink2" runat="server" CssClass="yafcssbigbutton rightItem button-reply"
                OnClick="PostReplyLink_Click" TextLocalizedTag="BUTTON_POSTREPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT" />
            <YAF:ThemeButton ID="QuickReplyLink2" runat="server" CssClass="btn btn-primary rightItem button-reply"
                             TextLocalizedTag="QUICKREPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT"
                             Icon="reply" DataTarget="QuickReplyDialog"/>
        </td>
    </tr>
</table>
<YAF:PageLinks ID="PageLinksBottom" runat="server" LinkedPageLinkID="PageLinks" />
<asp:PlaceHolder ID="ForumJumpHolder" runat="server">
    <div class="float-right">
        <YAF:LocalizedLabel ID="ForumJumpLabel" runat="server" LocalizedTag="FORUM_JUMP" />
        &nbsp;<YAF:ForumJump ID="ForumJump1" runat="server" />
    </div>
</asp:PlaceHolder>
<div class="float-right">
    <YAF:PageAccess ID="PageAccess1" runat="server" />
</div>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
<modal:QuickReply ID="QuickReplyDialog" runat="server" />