<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Subscriptions"CodeBehind="Subscriptions.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Constants" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>

    <div class="col">
<asp:UpdatePanel ID="PreferencesUpdatePanel" runat="server">
    <ContentTemplate>
        <div class="row">
            <div class="col">
                <div class="card mb-3">
                    <div class="card-header">
                        <i class="fa fa-envelope fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="TITLE" />
                    </div>
                    <div class="card-body">
                        
                            <h5 class="card-title">
                                <YAF:LocalizedLabel ID="LocalizedLabel200" runat="server" LocalizedTag="NOTIFICATIONSELECTION" />
                            </h5>
                        <div class="custom-control custom-radio">
                            <asp:RadioButtonList ID="rblNotificationType" runat="server" AutoPostBack="true"
                                                     OnSelectedIndexChanged="rblNotificationType_SelectionChanged"
                                                     RepeatLayout="UnorderedList"
                                                     CssClass="list-unstyled">
                                </asp:RadioButtonList>
                            </div>
                            <asp:PlaceHolder runat="server" id="DailyDigestRow">
                                <asp:CheckBox ID="DailyDigestEnabled" runat="server" 
                                              CssClass="custom-control custom-checkbox"
                                              Text='<%# this.GetText("DAILY_DIGEST") %>'/>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" id="PMNotificationRow">
                                <asp:CheckBox ID="PMNotificationEnabled" runat="server" 
                                              CssClass="custom-control custom-checkbox" 
                                              Text='<%# this.GetText("EDIT_PROFILE","PM_EMAIL_NOTIFICATION") %>' />
                            </asp:PlaceHolder>
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
                            <i class="fa fa-comments fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="forums" />
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
                                    <li class="list-group-item list-group-item-action">
                                        <asp:CheckBox ID="unsubf" runat="server" CssClass="custom-control custom-checkbox d-inline-block" Text="&nbsp;" />
                                        <asp:Label ID="tfid" runat="server" Text='<%# this.Eval("Item1.ID") %>'
                                               Visible="false" />
                                        <a href="<%# BuildLink.GetLinkNotEscaped(ForumPages.topics, "f={0}&name={1}",  this.Eval("Item1.ForumID"), this.Eval("Item2.Name"))%>">
                                                <%# this.HtmlEncode(this.Eval("Item2.Name"))%></a>
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
                            <i class="fa fa-comments fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="topics" />
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
                                    <li class="list-group-item list-group-item-action">
                                        <asp:CheckBox ID="unsubx" runat="server" CssClass="custom-control custom-checkbox d-inline-block" Text="&nbsp;">
                                        </asp:CheckBox>
                                        <asp:Label ID="ttid" runat="server" Text='<%# this.Eval("Item1.ID") %>'
                                               Visible="false" />
                                            <a href="<%# BuildLink.GetLinkNotEscaped(ForumPages.Posts, "t={0}", this.Eval("Item1.TopicID"))%>">
                                                <%# this.HtmlEncode(this.Eval("Item2.TopicName"))%></a>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" UsePostBack="True" />
                        </div>
                        <div class="card-footer text-center">
                            <YAF:ThemeButton ID="UnsubscribeTopics" runat="server" 
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