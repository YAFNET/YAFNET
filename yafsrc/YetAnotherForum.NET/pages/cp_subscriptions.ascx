<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.cp_subscriptions"CodeBehind="cp_subscriptions.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu ID="ProfileMenu1" runat="server" />
    </div>
    <div class="col">
<asp:UpdatePanel ID="PreferencesUpdatePanel" runat="server">
    <ContentTemplate>
        <div class="row">
            <div class="col">
                <div class="card mb-3">
                    <div class="card-header">
                        <i class="fa fa-envelope fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="TITLE" />
                    </div>
                    <div class="card-body">
                        <form>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="rblNotificationType">
                                    <YAF:LocalizedLabel ID="LocalizedLabel200" runat="server" LocalizedTag="NOTIFICATIONSELECTION" />
                                </asp:Label>
                                <asp:RadioButtonList ID="rblNotificationType" runat="server" AutoPostBack="true"
                                                     OnSelectedIndexChanged="rblNotificationType_SelectionChanged">
                                </asp:RadioButtonList>
                            </div>
                            <asp:PlaceHolder runat="server" id="DailyDigestRow">
                                <div class="form-check">
                                    <asp:CheckBox ID="DailyDigestEnabled" runat="server" CssClass="form-check-input" />
                                    <asp:Label runat="server" AssociatedControlID="DailyDigestEnabled" CssClass="form-check-label">
                                        <YAF:LocalizedLabel ID="LocalizedLabel199" runat="server" LocalizedTag="DAILY_DIGEST" />
                                    </asp:Label>
                                    
                                </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" id="PMNotificationRow">
                                <div class="form-check">
                                    <asp:CheckBox ID="PMNotificationEnabled" runat="server" CssClass="form-check-input" />
                                    <asp:Label runat="server" AssociatedControlID="PMNotificationEnabled" CssClass="form-check-label">
                                        <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedPage="CP_EDITPROFILE"
                                                        LocalizedTag="PM_EMAIL_NOTIFICATION" />
                                    </asp:Label>
                                    
                                </div>
                            </asp:PlaceHolder>
                            
                        </form>
                    </div>
                    <div class="card-footer text-center">
                        <YAF:ThemeButton ID="SaveUser" runat="server" OnClick="SaveUser_Click"
                                         TextLocalizedTag="SAVE"
                                         Type="Primary"
                                         Icon="save"/>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdatePanel ID="SubscriptionsUpdatePanel" runat="server">
    <ContentTemplate>
        <asp:PlaceHolder ID="SubscribeHolder" runat="server">
            <asp:PlaceHolder runat="server" ID="ForumsHolder">
            <div class="row">
                <div class="col">
                    <div class="card mb-3">
                        <div class="card-header">
                            <i class="fa fa-comments fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="forums" />
                        </div>
                        <div class="card-body">
                            <asp:Repeater ID="ForumList" runat="server">
                                <HeaderTemplate>
                                    <ul class="list-group list-group-flush">
                                </HeaderTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <li class="list-group-item">
                                        <asp:CheckBox ID="unsubf" runat="server" />
                                        <asp:Label ID="tfid" runat="server" Text='<%# Container.DataItemToField<int>("WatchForumID") %>'
                                               Visible="false" />
                                        <a href="<%# YafBuildLink.GetLinkNotEscaped(ForumPages.topics, "f={0}&name={1}",  Container.DataItemToField<int>("ForumID"), Container.DataItemToField<string>("ForumName"))%>">
                                                <%#
    this.HtmlEncode(Container.DataItemToField<string>("ForumName"))%></a>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <div class="card-footer text-center">
                            <YAF:ThemeButton ID="UnsubscribeForums" runat="server" 
                                             OnClick="UnsubscribeForums_Click"
                                             TextLocalizedTag="unsubscribe"
                                             Type="Danger"
                                             Icon="trash"/>
                        </div>
                    </div>
                </div>
            </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="TopicsHolder">
            <div class="row">
                <div class="col">
                    <div class="card mb-3">
                        <div class="card-header">
                            <i class="fa fa-comments fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="topics" />
                        </div>
                        <div class="card-body">
                            <YAF:Pager ID="PagerTop" runat="server" PageSize="25" OnPageChange="PagerTop_PageChange"
                                       UsePostBack="True" />
                            <asp:Repeater ID="TopicList" runat="server">
                                <HeaderTemplate>
                                    <ul class="list-group list-group-flush">
                                </HeaderTemplate>
                                <FooterTemplate>
                                </ul>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <li class="list-group-item">
                                        <asp:CheckBox ID="unsubx" runat="server" />
                                        <asp:Label ID="ttid" runat="server" Text='<%# Container.DataItemToField<int>("WatchTopicID") %>'
                                               Visible="false" />
                                            <a href="<%# YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "t={0}", Container.DataItemToField<int>("TopicID"))%>">
                                                <%#
    this.HtmlEncode(Container.DataItemToField<string>("TopicName"))%></a>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" UsePostBack="True" />
                        </div>
                        <div class="card-footer text-center">
                            <YAF:themeButton ID="UnsubscribeTopics" runat="server" 
                                             OnClick="UnsubscribeTopics_Click"
                                             TextLocalizedTag="unsubscribe"
                                             Type="Danger"
                                             Icon="trash"/>
                        </div>
                    </div>
                </div>
            </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
    </ContentTemplate>
</asp:UpdatePanel>
    </div>
</div>