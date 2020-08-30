<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Profile.Subscriptions" CodeBehind="Subscriptions.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Extensions" %>

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
                        <YAF:IconHeader runat="server"
                                        IconName="envelope"
                                        LocalizedPage="SUBSCRIPTIONS"/>
                    </div>
                    <div class="card-body">
                        <h5 class="card-title">
                                <YAF:LocalizedLabel ID="LocalizedLabel200" runat="server" LocalizedTag="NOTIFICATIONSELECTION" />
                            </h5>
                        <div class="form-check">
                            <asp:RadioButtonList ID="rblNotificationType" runat="server" 
                                                 AutoPostBack="true"
                                                 OnSelectedIndexChanged="rblNotificationType_SelectionChanged"
                                                 RepeatLayout="UnorderedList"
                                                 CssClass="list-unstyled">
                                </asp:RadioButtonList>
                        </div>
                        <asp:PlaceHolder runat="server" id="DailyDigestRow">
                            <asp:CheckBox ID="DailyDigestEnabled" runat="server" 
                                          CssClass="form-check"
                                          Text='<%# this.GetText("DAILY_DIGEST") %>'/>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" id="PMNotificationRow">
                            <asp:CheckBox ID="PMNotificationEnabled" runat="server" 
                                          CssClass="form-check" 
                                          Text='<%# this.GetText("EDIT_PROFILE","PM_EMAIL_NOTIFICATION") %>' />
                        </asp:PlaceHolder>
                        <div class="text-lg-center mt-3">
                            <YAF:ThemeButton ID="SaveUser" runat="server" OnClick="SaveUser_Click"
                                             TextLocalizedTag="SAVE"
                                             Type="Primary"
                                             Icon="save"/>
                        </div>
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
                            <div class="row justify-content-between align-items-center">
                                <div class="col-auto">
                                    <YAF:IconHeader runat="server"
                                                    IconName="comments"
                                                    LocalizedTag="FORUMS"
                                                    LocalizedPage="SUBSCRIPTIONS"/>
                                </div>
                                <div class="col-auto">
                                    <div class="input-group input-group-sm mr-2" role="group">
                                        <div class="input-group-text">
                                            <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                                        </div>
                                        <asp:DropDownList runat="server" ID="PageSizeForums"
                                                          AutoPostBack="True"
                                                          OnSelectedIndexChanged="PageSizeForumsSelectedIndexChanged"
                                                          CssClass="form-select">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-body p-0">
                            <asp:Repeater ID="ForumList" runat="server">
                                <HeaderTemplate>
                                    <ul class="list-group list-group-flush">
                                </HeaderTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <li class="list-group-item list-group-item-action">
                                        <asp:CheckBox ID="unsubf" runat="server" CssClass="form-check d-inline-block" Text="&nbsp;" />
                                        <asp:Label ID="tfid" runat="server" Text='<%# this.Eval("Item1.ID") %>'
                                               Visible="false" />
                                        <a href="<%# BuildLink.GetForumLink(this.Eval("Item1.ForumID").ToType<int>(), this.Eval("Item2.Name").ToString())%>">
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
                <div class="row justify-content-end">
                    <div class="col-auto">
                        <YAF:Pager ID="PagerForums" runat="server" 
                                   OnPageChange="PagerForums_PageChange" 
                                   UsePostBack="True" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="TopicsHolder">
            <div class="row">
                <div class="col">
                    <div class="card mb-3">
                        <div class="card-header">
                            <div class="row justify-content-between align-items-center">
                                <div class="col-auto">
                                    <YAF:IconHeader runat="server"
                                                    IconName="comment"
                                                    LocalizedTAG="TOPICS"
                                                    LocalizedPage="SUBSCRIPTIONS" />
                                </div>
                                <div class="col-auto">
                                    <div class="input-group input-group-sm mr-2" role="group">
                                        <div class="input-group-text">
                                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="SHOW" />:
                                        </div>
                                        <asp:DropDownList runat="server" ID="PageSizeTopics"
                                                          AutoPostBack="True"
                                                          OnSelectedIndexChanged="PageSizeTopicsSelectedIndexChanged"
                                                          CssClass="form-select">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-body p-0">
                            <asp:Repeater ID="TopicList" runat="server">
                                <HeaderTemplate>
                                    <ul class="list-group list-group-flush">
                                </HeaderTemplate>
                                <FooterTemplate>
                                </ul>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <li class="list-group-item list-group-item-action">
                                        <asp:CheckBox ID="unsubx" runat="server" CssClass="form-check d-inline-block" Text="&nbsp;">
                                        </asp:CheckBox>
                                        <asp:Label ID="ttid" runat="server" Text='<%# this.Eval("Item1.ID") %>'
                                               Visible="false" />
                                            <a href="<%# BuildLink.GetTopicLink(this.Eval("Item1.TopicID").ToType<int>(), this.Eval("Item2.TopicName").ToString())%>">
                                                <%# this.HtmlEncode(this.Eval("Item2.TopicName"))%></a>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
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
                <div class="row justify-content-end">
                    <div class="col-auto">
                        <YAF:Pager ID="PagerTopics" runat="server" 
                                   OnPageChange="PagerTopics_PageChange" 
                                   UsePostBack="True" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
    </ContentTemplate>
</asp:UpdatePanel>
    </div>
</div>