<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.ActiveUsers" Codebehind="ActiveUsers.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Core.Utilities.Helpers" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="users"></YAF:IconHeader>
            </div>
            <div class="card-body">
                <asp:Repeater ID="UserList" runat="server">
                    <HeaderTemplate>
                        <div class="table-responsive">
                            <table class="table tablesorter table-bordered table-striped" id="ActiveUsers">
                                <thead class="table-light">
                                    <tr>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                                                LocalizedTag="username" />
                                        </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabelLatestActions" runat="server"
                                                                LocalizedTag="latest_action" />
                                        </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server"
                                                                LocalizedTag="logged_in" />
                                        </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server"
                                                                LocalizedTag="last_active" />
                                        </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                                                                LocalizedTag="active" />
                                        </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server"
                                                                LocalizedTag="browser" />
                                        </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server"
                                                                LocalizedTag="platform" />
                                        </th>
                                        <th id="Iptd_header1" runat="server"
                                            visible="<%# this.PageContext.IsAdmin %>">
                                            IP
                                        </th>
                                    </tr>
                                </thead>
                            <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                        <tr>
                        <td>
                            <YAF:UserLink ID="NameLink" runat="server"
                                          Suspended="<%# (Container.DataItem as ActiveUser).Suspended %>"
                                          ReplaceName="<%# this.PageContext.BoardSettings.EnableDisplayName ? (Container.DataItem as ActiveUser).UserDisplayName : (Container.DataItem as ActiveUser).UserName %>"
                                          CrawlerName="<%# (Container.DataItem as ActiveUser).IsCrawler ? (Container.DataItem as ActiveUser).Browser : string.Empty %>"
                                          UserID="<%# (Container.DataItem as ActiveUser).UserID %>"
                                          Style="<%# (Container.DataItem as ActiveUser).UserStyle %>"
                                          PostfixText='<%# (Container.DataItem as ActiveUser).IsActiveExcluded ? new Icon{IconName = "user-secret"}.RenderToString() : "" %>'/>
                        </td>
                        <td>
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
                        </td>
                        <td>
                            <%# this.Get<IDateTimeService>().FormatTime((Container.DataItem as ActiveUser).Login) %>
                        </td>
                        <td>
                            <%# this.Get<IDateTimeService>().FormatTime((Container.DataItem as ActiveUser).LastActive) %>
                        </td>
                        <td>
                            <%# this.Get<ILocalization>().GetTextFormatted("minutes", (Container.DataItem as ActiveUser).Active)%>
                        </td>
                        <td><%# (Container.DataItem as ActiveUser).Browser %>
                        </td>
                        <td>
                            <%#(Container.DataItem as ActiveUser).Platform %>
                        </td>
                        <td id="Iptd1" runat="server" visible="<%# this.PageContext.IsAdmin %>">
                             <a id="Iplink1" href="<%# string.Format(this.PageContext.BoardSettings.IPInfoPageURL,IPHelper.GetIp4Address((Container.DataItem as ActiveUser).IP)) %>"
                                title='<%# this.GetText("COMMON","TT_IPDETAILS") %>' target="_blank" runat="server">
                             <%# IPHelper.GetIp4Address((Container.DataItem as ActiveUser).IP)%></a>
                        </td>
                    </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </div>
                        </div>
            <div class="card-footer">
                <div id="ActiveUsersPager" class="row justify-content-between align-items-center">
                <div class="col-auto mb-1">
                    <div class="input-group input-group-sm">
                        <div class="input-group-text">
                            <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                        </div>
                        <select class="pagesize form-select form-select-sm w-25">
                            <option selected="selected" value="5">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedPage="COMMON" LocalizedTag="ENTRIES_5" />
                            </option>
                            <option value="10">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedPage="COMMON" LocalizedTag="ENTRIES_10" />

                            </option>
                            <option value="20">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedPage="COMMON" LocalizedTag="ENTRIES_20" />

                            </option>
                            <option value="25">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedPage="COMMON" LocalizedTag="ENTRIES_25" />

                            </option>
                            <option value="50">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedPage="COMMON" LocalizedTag="ENTRIES_50" />

                            </option>
                        </select>
                    </div>
                </div>
                <div class="col-auto mb-1">
                    <div class="btn-group" role="group">
                        <a href="#" class="first  btn btn-secondary btn-sm"><span><i class="fas fa-angle-double-left"></i></span></a>
                        <a href="#" class="prev  btn btn-secondary btn-sm"><span><i class="fas fa-angle-left"></i></span></a>
                        <input type="button" class="pagedisplay  btn btn-secondary btn-sm disabled"  style="width:150px" />
                        <a href="#" class="next btn btn-secondary btn-sm"><span><i class="fas fa-angle-right"></i></span></a>
                        <a href="#" class="last  btn btn-secondary btn-sm"><span><i class="fas fa-angle-double-right"></i></span></a>
                    </div>
                </div>
            </div>
            </div>
            </div>
        </div>
                            </div>
    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>