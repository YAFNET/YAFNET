<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Posts" Codebehind="Posts.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="ServiceStack" %>
<%@ Import Namespace="YAF.Core.Extensions" %>

<%@ Register TagPrefix="YAF" TagName="DisplayPost" Src="../controls/DisplayPost.ascx" %>
<%@ Register TagPrefix="YAF" TagName="DisplayConnect" Src="../controls/DisplayConnect.ascx" %>
<%@ Register TagPrefix="YAF" TagName="DisplayAd" Src="../controls/DisplayAd.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PollList" Src="../controls/PollList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="SimilarTopics" Src="../controls/SimilarTopics.ascx" %>
<%@ Register TagPrefix="YAF" TagName="TopicTags" Src="../controls/TopicTags.ascx" %>

<%@ Register TagPrefix="modal" TagName="MoveTopic" Src="../Dialogs/MoveTopic.ascx" %>
<%@ Register TagPrefix="modal" TagName="QuickReply" Src="../Dialogs/QuickReply.ascx" %>


<YAF:PageLinks ID="PageLinks" runat="server" />

<YAF:PollList ID="PollList" TopicId="<%# this.PageContext.PageTopicID %>" 
              ShowButtons="<%# this.ShowPollButtons() %>" 
              Visible="<%# this.PollGroupId() > 0 %>" 
              PollGroupId="<%# this.PollGroupId() %>" runat="server"/>

<a id="top"></a>

<div class="row mb-3">
    <div class="col-md-4">
        <YAF:Pager ID="Pager" runat="server" UsePostBack="False" />
    </div>
    <div class="col-md-8 mt-1 mt-md-0 text-right">
        <div class="mt-n1">
            <span id="dvFavorite1">
                <YAF:ThemeButton ID="TagFavorite1" runat="server"
                                 Type="Secondary"
                                 TextLocalizedTag="BUTTON_TAGFAVORITE" TitleLocalizedTag="BUTTON_TAGFAVORITE_TT"
                                 Icon="star"
                                 IconColor="text-warning"/>
            </span>
            <YAF:ThemeButton ID="Tools1" runat="server"
                CssClass="dropdown-toggle"
                Type="Danger"
                DataToggle="dropdown"
                TextLocalizedTag="MANAGE_TOPIC"
                TextLocalizedPage="POSTS"
                Icon="cogs" />
            <div class="dropdown-menu" aria-labelledby="<%# this.Tools1.ClientID %>">
                <YAF:ThemeButton ID="MoveTopic1" runat="server"
                                 CssClass="dropdown-item"
                                 Type="None" 
                                 DataToggle="modal" 
                                 DataTarget="MoveTopicDialog" 
                                 TextLocalizedTag="BUTTON_MOVETOPIC" TitleLocalizedTag="BUTTON_MOVETOPIC_TT"
                                 Icon="arrows-alt" />
                <YAF:ThemeButton ID="UnlockTopic1" runat="server"
                    Type="None"
                    CssClass="dropdown-item"
                    OnClick="UnlockTopic_Click"
                    TextLocalizedTag="BUTTON_UNLOCKTOPIC" TitleLocalizedTag="BUTTON_UNLOCKTOPIC_TT"
                    Icon="lock-open" />
                <YAF:ThemeButton ID="LockTopic1" runat="server"
                    Type="None"
                    CssClass="dropdown-item"
                    OnClick="LockTopic_Click"
                    TextLocalizedTag="BUTTON_LOCKTOPIC" TitleLocalizedTag="BUTTON_LOCKTOPIC_TT"
                    Icon="lock" />
                <YAF:ThemeButton ID="DeleteTopic1" runat="server"
                    Type="None"
                    CssClass="dropdown-item"
                    OnClick="DeleteTopic_Click"
                    ReturnConfirmText='<%# this.GetText("confirm_deletetopic") %>'
                    TextLocalizedTag="BUTTON_DELETETOPIC" TitleLocalizedTag="BUTTON_DELETETOPIC_TT"
                    Icon="trash" />
            </div>
            <YAF:ThemeButton ID="NewTopic1" runat="server"
                Type="Secondary"
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
                             Icon="reply" 
                             DataToggle="modal"
                             DataTarget="QuickReplyDialog" />
        </div>
    </div>
</div>
<div class="row mb-3">
    <div class="col">
    <nav class="navbar navbar-expand-lg navbar-light bg-light navbar-round">
        <asp:HyperLink ID="TopicLink" runat="server" CssClass="navbar-brand pt-0">
            <asp:Label ID="TopicTitle" runat="server" CssClass="topic-title" />
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
                    <YAF:ThemeButton runat="server" ID="ShareLink"
                                     TextLocalizedTag="SHARE" TitleLocalizedTag="SHARE_TOOLTIP"
                                     Icon="share" 
                                     Type="Link" 
                                     CssClass="dropdown-toggle"
                                     DataToggle="dropdown">
                    </YAF:ThemeButton>
                    <YAF:PopMenu ID="ShareMenu" runat="server" 
                                 Control="ShareLink" />
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
                    <YAF:PopMenu runat="server" ID="OptionsMenu"
                                 Control="OptionsLink" />
                </li>
            </ul>
        </div>
    </nav>
    </div>
</div>
<asp:Repeater ID="MessageList" runat="server" OnItemCreated="MessageList_OnItemCreated">
    <ItemTemplate>
        <YAF:DisplayPost ID="DisplayPost1" runat="server" 
                         DataRow="<%# Container.DataItem.ToType<DataRow>() %>"
                         PostCount="<%# Container.ItemIndex %>" 
                         CurrentPage="<%# this.Pager.CurrentPageIndex %>" />
        <YAF:DisplayAd ID="DisplayAd" runat="server" Visible="False" />
        <YAF:DisplayConnect ID="DisplayConnect" runat="server" Visible="False" />
    </ItemTemplate>
</asp:Repeater>

<asp:PlaceHolder runat="server" Visible="<%# this.Get<IPermissions>().Check(this.Get<BoardSettings>().PostsFeedAccess) %>">
    <div class="row mb-3">
        <div class="col">
            <YAF:RssFeedLink ID="RssFeed" runat="server"
                             FeedType="Posts"  
                             AdditionalParameters='<%# "t={0}".Fmt(this.PageContext.PageTopicID) %>' 
                             Visible="<%# this.Get<IPermissions>().Check(this.Get<BoardSettings>().PostsFeedAccess) %>" 
            />
        </div>
    </div>                         
</asp:PlaceHolder>
<div class="row mb-3">
    <div class="col-md-4">
        <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="Pager" UsePostBack="False" />
    </div>
    <div class="col-md-8 mt-1 mt-md-0 text-right">
        <span id="dvFavorite2">
            <YAF:ThemeButton ID="TagFavorite2" runat="server" 
                             Type="Secondary"
                             TextLocalizedTag="BUTTON_TAGFAVORITE" TitleLocalizedTag="BUTTON_TAGFAVORITE_TT"
                             Icon="star"
                             IconColor="text-warning"/>
        </span>
        <YAF:ThemeButton ID="Tools2" runat="server" 
                             CssClass="dropdown-toggle"
                             Type="Danger"
                             DataToggle="dropdown"
                             TextLocalizedTag="MANAGE_TOPIC"
                             Icon="cogs" />
            <div class="dropdown-menu" aria-labelledby="<%# this.Tools1.ClientID %>">
                <YAF:ThemeButton ID="MoveTopic2" runat="server"
                                 Type="None"
                                 CssClass="dropdown-item"
                                 DataToggle="modal" 
                                 DataTarget="MoveTopicDialog" 
                                 TextLocalizedTag="BUTTON_MOVETOPIC" TitleLocalizedTag="BUTTON_MOVETOPIC_TT"
                                 Icon="arrows-alt" />
                <YAF:ThemeButton ID="UnlockTopic2" runat="server" 
                                 Type="None"
                                 CssClass="dropdown-item"
                                 OnClick="UnlockTopic_Click" 
                                 TextLocalizedTag="BUTTON_UNLOCKTOPIC" TitleLocalizedTag="BUTTON_UNLOCKTOPIC_TT"
                                 Icon="lock-open" />
                <YAF:ThemeButton ID="LockTopic2" runat="server" 
                                 Type="None"
                                 CssClass="dropdown-item"
                                 OnClick="LockTopic_Click" 
                                 TextLocalizedTag="BUTTON_LOCKTOPIC" TitleLocalizedTag="BUTTON_LOCKTOPIC_TT"
                                 Icon="lock" />
                <YAF:ThemeButton ID="DeleteTopic2" runat="server" 
                                 Type="None"
                                 CssClass="dropdown-item"
                                 OnClick="DeleteTopic_Click"
                                 ReturnConfirmText='<%# this.GetText("confirm_deletetopic") %>'
                                 TextLocalizedTag="BUTTON_DELETETOPIC" TitleLocalizedTag="BUTTON_DELETETOPIC_TT"
                                 Icon="trash" />
            </div>
            <YAF:ThemeButton ID="NewTopic2" runat="server" 
                             Type="Secondary"
                             TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT"
                             Icon="comment" />
            <YAF:ThemeButton ID="PostReplyLink2" runat="server" 
                             Type="Primary"
                             OnClick="PostReplyLink_Click" 
                             TextLocalizedTag="BUTTON_POSTREPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT"
                             Icon="reply" />
            <YAF:ThemeButton ID="QuickReplyLink2" runat="server" 
                             Type="Primary"
                             TextLocalizedTag="QUICKREPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT"
                             Icon="reply"  
                             DataToggle="modal" 
                             DataTarget="QuickReplyDialog"/>
    </div>
</div>
<YAF:TopicTags ID="TopicTags"  runat="server" />
<div class="row mb-3">
    <YAF:SimilarTopics ID="SimilarTopics"  runat="server">
    </YAF:SimilarTopics>
    <div class="col">
        <YAF:ForumUsers ID="ForumUsers1" runat="server" />
    </div>
</div>
<YAF:PageLinks ID="PageLinksBottom" runat="server" LinkedPageLinkID="PageLinks" />

<modal:MoveTopic ID="MoveTopicDialog" runat="server" />
<modal:QuickReply ID="QuickReplyDialog" runat="server" />