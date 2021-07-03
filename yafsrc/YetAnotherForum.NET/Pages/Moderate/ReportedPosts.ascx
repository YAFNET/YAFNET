<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Moderate.ReportedPosts" CodeBehind="ReportedPosts.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>

<YAF:PageLinks runat="server" ID="PageLinks" />


<div class="row">
    <div class="col-xl-12">
        <h1><%# this.PageContext.PageForumName %> - <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="REPORTED" /></h1>
    </div>
</div>
<asp:Repeater ID="List" runat="server" OnItemDataBound="List_OnItemDataBound">
    <ItemTemplate>
        <div class="row">
            <div class="col-xl-12">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:Icon runat="server" IconName="comment" IconType="text-secondary" />
                        <a id="TopicLink"
                           href='<%# this.Get<LinkBuilder>().GetLink(ForumPages.Posts, "t={0}&name={1}", (Container.DataItem as ReportedMessage).TopicID, (Container.DataItem as ReportedMessage).TopicName) %>'
                           runat="server"><%# (Container.DataItem as ReportedMessage).TopicName %></a>
                        <div class="float-end">
                            <span class="fw-bold">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="POSTED" />
                            </span>
                            <%# this.Get<IDateTimeService>().FormatDateShort((Container.DataItem as ReportedMessage).Posted) %>
                            <span class="fw-bold ps-3">
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NUMBERREPORTED" />
                            </span>
                            <%# (Container.DataItem as ReportedMessage).NumberOfReports %>
                            <span class="fw-bold ps-3">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                                                    LocalizedTag="POSTEDBY" LocalizedPage="REPORTPOST" />
                            </span>
                            <YAF:UserLink ID="UserLink1" runat="server"
                                          ReplaceName="<%# this.PageContext.BoardSettings.EnableDisplayName ? (Container.DataItem as ReportedMessage).UserDisplayName : (Container.DataItem as ReportedMessage).UserName %>"
                                          Suspended="<%# (Container.DataItem as ReportedMessage).Suspended %>"
                                          Style="<%# (Container.DataItem as ReportedMessage).UserStyle %>"
                                          UserID="<%# (Container.DataItem as ReportedMessage).UserID %>" />
                            <YAF:ThemeButton ID="AdminUserButton" runat="server"
                                             Size="Small"
                                             Visible="<%# this.PageContext.IsAdmin %>"
                                             TextLocalizedTag="ADMIN_USER" TextLocalizedPage="PROFILE"
                                             NavigateUrl='<%# this.Get<LinkBuilder>().GetLink( ForumPages.Admin_EditUser,"u={0}", (Container.DataItem as ReportedMessage).UserID ) %>'
                                             Icon="users-cog"
                                             Type="Danger">
                            </YAF:ThemeButton>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col">
                                <h6 class="card-subtitle mb-2 text-muted">
                                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="ORIGINALMESSAGE" />
                                </h6>
                                <div class="card bg-light mb-3">
                                    <div class="card-body">
                                        <YAF:MessagePostData ID="MessagePostPrimary" runat="server"
                                                             ShowAttachments="false"
                                                             ShowEditMessage="False" ShowSignature="False">
                                        </YAF:MessagePostData>
                                    </div>
                                </div>
                                <asp:PlaceHolder id="Label1" runat="server"
                                                 visible="<%# (Container.DataItem as ReportedMessage).OriginalMessage != (Container.DataItem as ReportedMessage).Message%>">
                                    <div class="alert alert-warning" role="alert">
                                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MODIFIED" />
                                    </div>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col">
                                <YAF:BaseReportedPosts ID="ReportersList" runat="server"
                                                   MessageID="<%# (Container.DataItem as ReportedMessage).MessageID %>"
                                                   ResolvedBy="<%# (Container.DataItem as ReportedMessage).ResolvedBy %>"
                                                   Resolved="<%# (Container.DataItem as ReportedMessage).Resolved %>"
                                                   ResolvedDate="<%# (Container.DataItem as ReportedMessage).ResolvedDate %>" />
                            </div>
                        </div>
                    </div>
                    <div class="card-footer text-center">
                        <YAF:ThemeButton ID="CopyOverBtn" runat="server"
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="COPYOVER"
                                         CommandName="CopyOver" CommandArgument="<%# (Container.DataItem as ReportedMessage).MessageID %>"
                                         Visible="<%# (Container.DataItem as ReportedMessage).OriginalMessage != (Container.DataItem as ReportedMessage).Message%>"
                                         Icon="copy"
                                         Type="Secondary" />
                        <YAF:ThemeButton ID="DeleteBtn" runat="server"
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="DELETE"
                                         CommandName="Delete" CommandArgument='<%# string.Format("{0};{1}", (Container.DataItem as ReportedMessage).MessageID,  (Container.DataItem as ReportedMessage).TopicID) %>'
                                         ReturnConfirmText='<%# this.GetText("ASK_DELETE") %>'
                                         Icon="trash"
                                         Type="Danger"/>
                        <YAF:ThemeButton ID="ResolveBtn" runat="server"
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="RESOLVED"
                                         CommandName="Resolved" CommandArgument="<%# (Container.DataItem as ReportedMessage).MessageID %>"
                                         Icon="check"
                                         Type="Success" />
                        <YAF:ThemeButton ID="ViewBtn" runat="server"
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="VIEW"
                                         CommandName="View" CommandArgument="<%# (Container.DataItem as ReportedMessage).MessageID %>"
                                         Icon="eye"
                                         Type="Secondary" />
                        <YAF:ThemeButton ID="ViewHistoryBtn" runat="server"
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="HISTORY"
                                         Visible="<%# (Container.DataItem as ReportedMessage).OriginalMessage != (Container.DataItem as ReportedMessage).Message%>"
                                         CommandName="ViewHistory" CommandArgument="<%# (Container.DataItem as ReportedMessage).MessageID %>"
                                         Icon="history"
                                         Type="Secondary" />
                    </div>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

