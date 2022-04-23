<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.ActiveUsers" Codebehind="ActiveUsers.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>
<%@ Import Namespace="YAF.Core.Helpers" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <div class="row justify-content-between align-items-center">
                    <div class="col-auto">
                        <YAF:IconHeader runat="server"
                                        IconName="users"></YAF:IconHeader>
                    </div>
                    <div class="col-auto">
                        <div class="btn-toolbar" role="toolbar">
                            <div class="input-group input-group-sm me-2" role="group">
                                <div class="input-group-text">
                                    <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                                </div>
                                <asp:DropDownList runat="server" ID="PageSize"
                                                  AutoPostBack="True"
                                                  OnSelectedIndexChanged="PageSizeSelectedIndexChanged"
                                                  CssClass="form-select">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <asp:Repeater ID="UserList" runat="server">
                    <HeaderTemplate>
                        <ul class="list-group">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item list-group-item-action">
                                <div class="d-flex w-100 justify-content-between text-break">
                                    <h5 class="mb-1">
                                        <YAF:UserLink ID="NameLink" runat="server"
                                                      Suspended="<%# (Container.DataItem as ActiveUser).Suspended %>"
                                                      ReplaceName="<%# this.PageBoardContext.BoardSettings.EnableDisplayName ? (Container.DataItem as ActiveUser).UserDisplayName : (Container.DataItem as ActiveUser).UserName %>"
                                                      CrawlerName="<%# (Container.DataItem as ActiveUser).IsCrawler ? (Container.DataItem as ActiveUser).Browser : string.Empty %>"
                                                      UserID="<%# (Container.DataItem as ActiveUser).UserID %>"
                                                      Style="<%# (Container.DataItem as ActiveUser).UserStyle %>"
                                                      PostfixText='<%# (Container.DataItem as ActiveUser).IsActiveExcluded ? new Icon{IconName = "user-secret"}.RenderToString() : "" %>'/>
                                    </h5>
                                    <asp:PlaceHolder runat="server" ID="IPContent" Visible="<%# this.PageBoardContext.IsAdmin %>">
                                        <small class="d-none d-md-block">
                                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="IP" />:
                                            <span class="badge bg-secondary">
                                                <a id="Iplink1" href="<%# string.Format(this.PageBoardContext.BoardSettings.IPInfoPageURL,IPHelper.GetIpAddressAsString((Container.DataItem as ActiveUser).IP)) %>"
                                                   title='<%# this.GetText("COMMON","TT_IPDETAILS") %>' target="_blank" runat="server"
                                                   class="link-light">
                                                    <%# IPHelper.GetIpAddressAsString((Container.DataItem as ActiveUser).IP)%></a>
                                            </span>
                                        </small>
                                    </asp:PlaceHolder>
                                    
                                </div>
                                <p><strong><YAF:LocalizedLabel ID="LocalizedLabelLatestActions" runat="server"
                                                               LocalizedTag="latest_action" />:</strong>
                                    <YAF:ActiveLocation ID="ActiveLocation2"
                                                        UserID="<%# (Container.DataItem as ActiveUser).UserID %>"
                                                        HasForumAccess="<%# (Container.DataItem as ActiveUser).HasForumAccess %>"
                                                        ForumPage="<%#(Container.DataItem as ActiveUser).ForumPage %>"
                                                        Location="<%#(Container.DataItem as ActiveUser).Location %>"
                                                        ForumID="<%# (Container.DataItem as ActiveUser).ForumID ?? 0  %>"
                                                        ForumName="<%# (Container.DataItem as ActiveUser).ForumName %>"
                                                        TopicID="<%# (Container.DataItem as ActiveUser).TopicID ?? 0  %>"
                                                        TopicName="<%# (Container.DataItem as ActiveUser).TopicName %>"
                                                        LastLinkOnly="false"  runat="server"></YAF:ActiveLocation>
                                </p>
                                <small>
                                    <span>
                                        <strong><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server"
                                                                   LocalizedTag="logged_in" />:</strong>
                                        <%# this.Get<IDateTimeService>().FormatTime((Container.DataItem as ActiveUser).Login) %>
                                    </span>
                                    <span>
                                        <strong><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server"
                                                                    LocalizedTag="last_active" />:</strong>
                                        <%# this.Get<IDateTimeService>().FormatTime((Container.DataItem as ActiveUser).LastActive) %>
                                    </span>
                                    <span>
                                        <strong><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                                                                    LocalizedTag="active" />:</strong>
                                        <%# this.Get<ILocalization>().GetTextFormatted("minutes", (Container.DataItem as ActiveUser).Active)%>
                                    </span>
                                    <span>
                                        <strong><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server"
                                                                    LocalizedTag="browser" />:</strong>
                                        <%# (Container.DataItem as ActiveUser).Browser %>
                                    </span>
                                    <span>
                                        <strong><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server"
                                                                    LocalizedTag="platform" />:</strong>
                                        <%#(Container.DataItem as ActiveUser).Platform %>
                                    </span>
                                </small>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager ID="PagerTop" runat="server"
                   OnPageChange="PagerTopPageChange"/>
    </div>
</div>