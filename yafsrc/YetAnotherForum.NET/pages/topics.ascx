<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.topics" Codebehind="topics.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Register TagPrefix="YAF" TagName="ForumList" Src="../controls/ForumList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="TopicLine" Src="../controls/TopicLine.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<asp:PlaceHolder runat="server" ID="SubForums" Visible="false">
    <div class="row">
        <div class="col">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-comments fa-fw"></i>&nbsp;<%= this.GetSubForumTitle()%>
                </div>
                <div class="card-body">
                    <YAF:ForumList AltLastPost="<%# this.LastPostImageTT %>" runat="server" ID="ForumList" />
                </div>
            </div>
        </div>
    </div>
</asp:PlaceHolder>
<div class="row mb-3">
    <div class="col">
        <YAF:Pager runat="server" ID="Pager" UsePostBack="False" />
    </div>
    <div class="col">
        <YAF:ThemeButton ID="moderate1" runat="server"
                         CssClass="float-right mr-1"
                         TextLocalizedTag="BUTTON_MODERATE" TitleLocalizedTag="BUTTON_MODERATE_TT"
                         Type="Secondary"
                         Icon="tasks"/>
        <YAF:ThemeButton ID="NewTopic1" runat="server" 
                         CssClass="float-right mr-1"
                         TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" 
                         OnClick="NewTopic_Click"
                         Icon="plus"/>
    </div>
</div>
<div class="row">
    <div class="col">
        <div class="card mb-3 mt-3">
            <div class="card-header">
                <i class="fa fa-comments fa-fw"></i>&nbsp;<asp:Label ID="PageTitle" runat="server"></asp:Label>
            </div>
            <div class="card-body">
                <asp:PlaceHolder runat="server" ID="NoPostsPlaceHolder">
                    <YAF:Alert runat="server" Type="info">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="NO_TOPICS"></YAF:LocalizedLabel>
                    </YAF:Alert>
                </asp:PlaceHolder>
                <asp:Repeater ID="Announcements" runat="server">
                    <ItemTemplate>
                        <YAF:TopicLine runat="server" AltLastPost="<%# this.LastPostImageTT %>" DataRow="<%# Container.DataItem %>" />
                    </ItemTemplate>
                    <SeparatorTemplate>
                        <div class="row">
                            <div class="col">
                                <hr/>
                            </div>
                        </div>
                    </SeparatorTemplate>
                </asp:Repeater>
                <asp:Repeater ID="TopicList" runat="server">
                    <ItemTemplate>
                        <YAF:TopicLine runat="server" AltLastPost="<%# this.LastPostImageTT %>" DataRow="<%# Container.DataItem %>" />
                    </ItemTemplate>
                    <SeparatorTemplate>
                        <div class="row">
                            <div class="col">
                                <hr/>
                            </div>
                        </div>
                    </SeparatorTemplate>
                </asp:Repeater>
            </div>
            <div class="card-footer">
                <div class="mb-1 form-inline">
                    <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="showtopics" />: 
                    &nbsp;<asp:DropDownList ID="ShowList" runat="server" AutoPostBack="True" CssClass="standardSelectMenu" />
                </div>
                <asp:PlaceHolder ID="ForumJumpHolder" runat="server">
                        <div class="mb-1 form-inline">
                            <YAF:LocalizedLabel ID="ForumJumpLabel" runat="server" LocalizedTag="FORUM_JUMP" />: 
                            &nbsp;<YAF:ForumJump ID="ForumJump1" runat="server" />
                        </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="ForumSearchHolder" runat="server">
                    <div class="mb-1 form-inline">
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="SEARCH_FORUM" />:
                        &nbsp;<asp:TextBox id="forumSearch" runat="server"></asp:TextBox>
                        &nbsp;<YAF:ThemeButton ID="forumSearchOK" runat="server" Size="Small"
                                               TextLocalizedTag="OK" TitleLocalizedTag="OK_TT" OnClick="ForumSearch_Click" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</div>
<div class="row mb-3">
    <div class="col">
        <div class="btn-group float-right" role="group" aria-label="Tools">
            <YAF:ThemeButton runat="server" ID="WatchForum"
                             Type="Secondary" 
                             Size="Small"
                             Icon="eye"
                             TextLocalizedTag="WATCHFORUM"/>
            <YAF:ThemeButton runat="server" 
                             Type="Secondary" 
                             Size="Small" 
                             Icon="flag-checkered"
                             ID="MarkRead"
                             TextLocalizedTag="MARKREAD"/>
            <YAF:RssFeedLink ID="RssFeed" runat="server" 
                             FeedType="Topics"
                             Visible="<%# this.Get<IPermissions>().Check(this.PageContext.BoardSettings.TopicsFeedAccess) %>"  
            />
        </div>
    </div>
</div>
<div class="row mb-3">
    <div class="col">
        <YAF:ForumUsers runat="server" />
    </div>
</div>

<div class="row mb-3">
    <div class="col">
            <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="Pager" UsePostBack="False" />
    </div>
    <div class="col">
            <YAF:ThemeButton ID="moderate2" runat="server" 
                             CssClass="float-right mr-1"
                             TextLocalizedTag="BUTTON_MODERATE" TitleLocalizedTag="BUTTON_MODERATE_TT"
                             Type="Secondary"
                             Icon="tasks"/>
            <YAF:ThemeButton ID="NewTopic2" runat="server" 
                             CssClass="float-right mr-1"
                             TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" 
                             OnClick="NewTopic_Click"
                             Icon="plus"/>
    </div>
</div>

<div class="float-left">
    <YAF:IconLegend ID="IconLegend1" runat="server" />
</div>
<div class="float-right">
    <YAF:PageAccess ID="PageAccess1" runat="server" />
</div>