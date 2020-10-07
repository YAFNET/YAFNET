<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.PMList" EnableTheming="true" Codebehind="PMList.ascx.cs" EnableViewState="true" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>

<div class="card-header">
    <div class="row justify-content-between align-items-center">
        <div class="col-auto">
            <YAF:IconHeader runat="server"
                            IconName="inbox"
                            ID="IconHeader"/>
        </div>
        <div class="col-auto">
            <div class="btn-toolbar" role="toolbar">
                <div class="input-group input-group-sm mr-2" role="group">
                    <div class="input-group-text">
                        <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                    </div>
                    <asp:DropDownList runat="server" ID="PageSize"
                                      AutoPostBack="True"
                                      OnSelectedIndexChanged="PageSizeSelectedIndexChanged"
                                      CssClass="form-select">
                    </asp:DropDownList>
                </div>
                <div class="btn-group btn-group-sm">
                   
                    <YAF:ThemeButton ID="Sort" runat="server"
                                     CssClass="dropdown-toggle"
                                     Type="Secondary"
                                     DataToggle="dropdown"
                                     TextLocalizedTag="SORT_BY"
                                     Icon="sort"
                                     Visible="<%# this.Messages.Items.Count > 0 %>"/>
                    <div class="dropdown-menu dropdown-menu-right dropdown-menu-lg-left">
                       
                        <YAF:ThemeButton ID="SortFromAsc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="FromLinkAsc_Click"/>
                        <YAF:ThemeButton ID="SortFromDesc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="FromLinkDesc_Click"/>
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="SortSubjectAsc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="SubjectLinkAsc_Click"
                                         TextLocalizedTag="SUBJECT_ASC" />
                        <YAF:ThemeButton ID="SortSubjectDesc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="SubjectLinkDesc_Click"
                                         TextLocalizedTag="SUBJECT_DESC" />
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="SortDatedAsc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="DateLinkAsc_Click"
                                         TextLocalizedTag="DATE_ASC" />
                        <YAF:ThemeButton ID="SortDateDesc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="DateLinkDesc_Click"
                                         TextLocalizedTag="DATE_DESC" />
                    </div>
                    </div>
            </div>
        </div>
    </div>
</div>
<div class="card-body">
    <asp:Repeater runat="server" ID="Messages">
    <HeaderTemplate>
        <ul class="list-group">
    </HeaderTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
    <ItemTemplate>
        <li class="list-group-item list-group-item-action">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1 text-break">
                        <asp:HiddenField ID="MessageID" runat="server" Value="<%# (Container.DataItem as PagedPm).UserPMessageID %>" />
                        <asp:CheckBox runat="server" ID="ItemCheck" 
                                       Text="&nbsp;"
                                       CssClass="form-check d-inline-flex align-middle"/>
                        <YAF:Icon runat="server"
                                  IconName='<%# (Container.DataItem as PagedPm).IsRead ? "envelope-open" : "envelope" %>'
                                  IconType='<%# (Container.DataItem as PagedPm).IsRead ? "text-secondary" : "text-success" %>'></YAF:Icon>
                        <a href="<%# this.GetMessageLink((Container.DataItem as PagedPm).UserPMessageID) %>">
                        <%# this.HtmlEncode((Container.DataItem as PagedPm).Subject) %>
                        </a>
                    </h5>
                    <small class="d-none d-md-block">
                        <span class="font-weight-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                LocalizedTag="DATE" />
                        </span>
                        <YAF:DisplayDateTime ID="PostedDateTime" runat="server" 
                                             DateTime="<%# (Container.DataItem as PagedPm).Created %>"></YAF:DisplayDateTime>
                    </small>
                </div>
                <p class="mb-1">
                    <span class="font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                            LocalizedTag='<%# this.View == PmView.Outbox ? "TO" : "FROM" %>' />:
                    </span>
                    <YAF:UserLink ID="UserLink1" runat="server"
                                  ReplaceName="<%# this.View == PmView.Outbox ? this.PageContext.BoardSettings.EnableDisplayName ? (Container.DataItem as PagedPm).ToUserDisplayName : (Container.DataItem as PagedPm).ToUser : this.PageContext.BoardSettings.EnableDisplayName ? (Container.DataItem as PagedPm).FromUserDisplayName : (Container.DataItem as PagedPm).FromUser %>"
                                  Suspended="<%# this.View == PmView.Outbox ? (Container.DataItem as PagedPm).ToSuspended : (Container.DataItem as PagedPm).FromSuspended  %>"
                                  Style="<%# this.View == PmView.Outbox ? (Container.DataItem as PagedPm).ToStyle : (Container.DataItem as PagedPm).FromStyle %>"
                                  UserID="<%# this.View == PmView.Outbox ? (Container.DataItem as PagedPm).ToUserID : (Container.DataItem as PagedPm).FromUserID %>" />
                </p>
            </li>
    </ItemTemplate>
</asp:Repeater>

<asp:UpdatePanel ID="upPanExport" runat="server">
    <ContentTemplate>
        <div class="btn-toolbar mt-3" role="toolbar">
            <div class="btn-group mr-2 mb-1" role="group">
                <YAF:ThemeButton runat="server" ID="MarkAsRead" 
                                 Size="Small"
                                 TextLocalizedTag="MARK_ALL_ASREAD"
                                 TitleLocalizedTag="MARK_ALL_ASREAD"
                                 DataToggle="tooltip"
                                 OnClick="MarkAsRead_Click"
                                 Type="Secondary" 
                                 Icon="eye"/>
            </div>
            <div class="btn-group mr-2 mb-1" role="group">
                <YAF:ThemeButton runat="server" ID="ArchiveSelected" 
                                 Size="Small"
                                 TextLocalizedTag="ARCHIVESELECTED" 
                                 TitleLocalizedTag="ARCHIVESELECTED" 
                                 DataToggle="tooltip"
                                 OnClick="ArchiveSelected_Click"
                                 Type="Secondary"
                                 Icon="archive" />
                <YAF:ThemeButton runat="server" ID="ArchiveAll" 
                                 Size="Small"
                                 TextLocalizedTag="ARCHIVEALL" 
                                 TitleLocalizedTag="ARCHIVEALL" 
                                 DataToggle="tooltip"
                                 ReturnConfirmText='<%#this.GetText("CONFIRM_ARCHIVEALL") %>'
                                 OnClick="ArchiveAll_Click"
                                 Type="Secondary" Icon="archive" />
            </div>
            <div class="btn-group mr-2 mb-1" role="group">
                <YAF:ThemeButton runat="server" ID="ExportSelected" 
                                 Size="Small"
                                 TextLocalizedTag="EXPORTSELECTED" 
                                 TitleLocalizedTag="EXPORTSELECTED" 
                                 DataToggle="tooltip"
                                 OnClick="ExportSelected_Click" 
                                 Type="Secondary" 
                                 Icon="file-export"/>
                <YAF:ThemeButton runat="server" ID="ExportAll"
                                 Size="Small"
                                 TextLocalizedTag="EXPORTALL" 
                                 TitleLocalizedTag="EXPORTALL" 
                                 DataToggle="tooltip"
                                 OnClick="ExportAll_Click" 
                                 Type="Secondary" 
                                 Icon="file-export" />
            </div>
            <div class="btn-group mb-1" role="group">
                <YAF:ThemeButton runat="server" ID="DeleteSelected" 
                                 Size="Small"
                                 TextLocalizedTag="DELETESELECTED" 
                                 TitleLocalizedTag="DELETESELECTED" 
                                 DataToggle="tooltip"
                                 ReturnConfirmText='<%#this.GetText("CONFIRM_DELETE") %>'
                                 OnClick="DeleteSelected_Click"
                                 Type="Secondary" 
                                 Icon="trash" />
                <YAF:ThemeButton runat="server" ID="DeleteAll" 
                                 Size="Small"
                                 TextLocalizedTag="DELETEALL" 
                                 TitleLocalizedTag="DELETEALL" 
                                 DataToggle="tooltip"
                                 ReturnConfirmText='<%#this.GetText("CONFIRM_DELETEALL") %>'
                                 OnClick="DeleteAll_Click"
                                 Type="Secondary" 
                                 Icon="trash" />
            </div>
        </div>
        <hr />
    </ContentTemplate> 
    <Triggers>
        <asp:PostBackTrigger ControlID="ExportSelected" />
        <asp:PostBackTrigger ControlID="ExportAll" />
    </Triggers>
</asp:UpdatePanel>
    
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager ID="PagerTop" runat="server" 
                   OnPageChange="PagerTop_PageChange" />
    </div>
</div>

<asp:Label id="lblExportType" runat="server"></asp:Label>
<div class="form-check form-check-inline">
    <asp:RadioButtonList runat="server" id="ExportType" 
                         RepeatLayout="UnorderedList"
                         CssClass="list-unstyled">
        <asp:ListItem Text="XML" Selected="True" Value="xml"></asp:ListItem>
        <asp:ListItem Text="CSV" Value="csv"></asp:ListItem>
    </asp:RadioButtonList>
</div>

<YAF:Alert runat="server" ID="NoMessage" Type="info">
    <YAF:Icon runat="server" 
              IconName="info-circle" />
    <YAF:LocalizedLabel runat="server" 
                        LocalizedTag="NO_MESSAGES" />
</YAF:Alert>
</div>