<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Topics" Codebehind="Topics.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="ServiceStack" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<%@ Register TagPrefix="YAF" TagName="ForumList" Src="../controls/ForumList.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<asp:PlaceHolder runat="server" ID="SubForums" Visible="false">
    <div class="row">
        <div class="col">
            <div class="card mb-3">
                <div class="card-header d-flex align-items-center">
                    <YAF:CollapseButton ID="CollapsibleImage" runat="server"
                                        PanelID='<%# "forumPanel{0}".Fmt(this.PageContext.PageForumID) %>'
                                        AttachedControlID="body" 
                                        CssClass="pl-0">
                    </YAF:CollapseButton>
                    <YAF:Icon runat="server"
                              IconName="comments"
                              IconType="text-secondary pr-1"></YAF:Icon>
                    <%= this.GetSubForumTitle()%>
                </div>
                <div class="card-body" id="body" runat="server">
                    <YAF:ForumList runat="server" ID="ForumList" />
                </div>
            </div>
        </div>
    </div>
</asp:PlaceHolder>
<div class="row justify-content-end">
    <div class="col-auto mb-1">
        <YAF:ThemeButton ID="moderate1" runat="server"
                         CssClass="mr-1"
                         TextLocalizedTag="BUTTON_MODERATE" TitleLocalizedTag="BUTTON_MODERATE_TT"
                         Type="Secondary"
                         Icon="tasks"/>
        <YAF:ThemeButton ID="NewTopic1" runat="server" 
                         TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" 
                         Icon="plus"/>
    </div>
</div>
<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <div class="row justify-content-between align-items-center">
                    <div class="col-auto">
                        <YAF:Icon runat="server"
                                  IconName="comments"
                                  IconType="text-secondary pr-1"></YAF:Icon>
                        <asp:Label ID="PageTitle" runat="server"></asp:Label>
                    </div>
                    <div class="col-auto">
                        <YAF:ThemeButton runat="server"
                                         CssClass="dropdown-toggle"
                                         DataToggle="dropdown"
                                         Size="Small"
                                         Type="Secondary"
                                         Icon="filter"
                                         TextLocalizedTag="FILTER_DROPDOWN"
                                         TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
                        <div class="dropdown-menu dropdown-menu-right dropdown-menu-lg-left">
                            <div class="px-3 py-1 dropdown-sm">
                                <div class="mb-3">
                                    <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                                    <asp:DropDownList runat="server" ID="PageSize"
                                                      AutoPostBack="True"
                                                      OnSelectedIndexChanged="PageSizeSelectedIndexChanged"
                                                      CssClass="select2-select">
                                    </asp:DropDownList>
                                </div>
                                <div class="mb-3">
                                    <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" 
                                                        LocalizedTag="showtopics" />: 
                                    <asp:DropDownList ID="ShowList" runat="server" 
                                          AutoPostBack="True" 
                                          CssClass="select2-select" />
                                </div>
                                <asp:PlaceHolder ID="ForumJumpHolder" runat="server">
                                    <div class="mb-3">
                                        <YAF:LocalizedLabel ID="ForumJumpLabel" runat="server" 
                                            LocalizedTag="FORUM_JUMP" />:
                                        <YAF:ForumJump ID="ForumJump1" runat="server" />
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="ForumSearchHolder" runat="server">
                                    <div class="mb-3">
                                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" 
                                            LocalizedTag="SEARCH_FORUM" />:
                                        <div class="input-group">
                                            <asp:TextBox id="forumSearch" runat="server"
                                                         CssClass="form-control"/>
                                            <YAF:ThemeButton ID="forumSearchOK" runat="server" 
                                                             Type="Secondary"
                                                             Icon="search"
                                                             TextLocalizedTag="OK"
                                                             TitleLocalizedTag="OK_TT" 
                                                             OnClick="ForumSearch_Click" />
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <asp:PlaceHolder runat="server" ID="NoPostsPlaceHolder">
                    <YAF:Alert runat="server" Type="info">
                        <YAF:LocalizedLabel runat="server" 
                                            LocalizedTag="NO_TOPICS"></YAF:LocalizedLabel>
                    </YAF:Alert>
                </asp:PlaceHolder>
                <asp:Repeater ID="Announcements" runat="server">
                    <ItemTemplate>
                        <%# this.CreateTopicLine((System.Data.DataRowView)Container.DataItem) %>
                    </ItemTemplate>
                    <SeparatorTemplate>
                        <div class="row">
                            <div class="col">
                                <hr/>
                            </div>
                        </div>
                    </SeparatorTemplate>
                    <FooterTemplate>
                        <asp:PlaceHolder runat="server" Visible="<%# this.Announcements.Items.Count > 0 %>">
                            <div class="row">
                                <div class="col">
                                    <hr />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Repeater ID="TopicList" runat="server">
                    <ItemTemplate>
                        <%# this.CreateTopicLine((System.Data.DataRowView)Container.DataItem) %>
                    </ItemTemplate>
                    <SeparatorTemplate>
                        <div class="row">
                            <div class="col">
                                <hr />
                            </div>
                        </div>
                    </SeparatorTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>

<div class="d-flex flex-row-reverse mb-3">
    <div>
        <div class="btn-group" role="group" aria-label="Tools">
            <YAF:ThemeButton runat="server" ID="WatchForum"
                             Type="Secondary" 
                             Size="Small"
                             Icon="eye"
                             TextLocalizedTag="WATCHFORUM"/>
            <YAF:ThemeButton runat="server" 
                             Type="Secondary" 
                             Size="Small" 
                             Icon="glasses"
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

<div class="row justify-content-between">
    <div class="col-auto mb-1">
            <YAF:Pager ID="Pager" runat="server"
                       UsePostBack="False" />
    </div>
    <div class="col-auto mb-1">
            <YAF:ThemeButton ID="moderate2" runat="server" 
                             CssClass="mr-1"
                             TextLocalizedTag="BUTTON_MODERATE" TitleLocalizedTag="BUTTON_MODERATE_TT"
                             Type="Secondary"
                             Icon="tasks"/>
            <YAF:ThemeButton ID="NewTopic2" runat="server" 
                             TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" 
                             Icon="plus"/>
    </div>
</div>