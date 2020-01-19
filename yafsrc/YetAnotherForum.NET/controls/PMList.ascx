<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.PMList" EnableTheming="true" Codebehind="PMList.ascx.cs" EnableViewState="true" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Core.Extensions" %>

<YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />

<div class="btn-group mb-2">
    <YAF:ThemeButton ID="Sort" runat="server"
                     CssClass="dropdown-toggle"
                     Type="Secondary"
                     DataToggle="dropdown"
                     TextLocalizedTag="SORT_BY"
                     Icon="sort"
                     Visible='<%# this.Messages.Items.Count > 0 %>'/>
    <div class="dropdown-menu">
        <YAF:ThemeButton ID="SortFromAsc" runat="server"
                         CssClass="dropdown-item"
                         Type="None" 
                         OnClick="FromLinkAsc_Click"
                         TextLocalizedTag='<%# this.View == PmView.Outbox ? "TO_ASC" : "FROM_ASC" %>'/>
        <YAF:ThemeButton ID="SortFromDesc" runat="server"
                         CssClass="dropdown-item"
                         Type="None" 
                         OnClick="FromLinkDesc_Click"
                         TextLocalizedTag='<%# this.View == PmView.Outbox ? "TO_DESC" : "FROM_DESC" %>'/>
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
                        <asp:HiddenField ID="MessageID" runat="server" Value='<%# this.Eval("UserPMessageID") %>' />
                        <asp:CheckBox runat="server" ID="ItemCheck" 
                                       Text="&nbsp;"
                                       CssClass="custom-control custom-checkbox d-inline-flex"/>
                        <YAF:Icon runat="server"
                                  IconName='<%# this.Eval("IsRead").ToType<bool>() ? "envelope-open" : "envelope" %>'
                                  IconType='<%# this.Eval("IsRead").ToType<bool>() ? "text-secondary" : "text-success" %>'></YAF:Icon>
                        <a href='<%# this.GetMessageLink(this.Eval("UserPMessageID")) %>'>
                        <%# this.HtmlEncode(this.Eval("Subject")) %>
                        </a>
                    </h5>
                    <small class="d-none d-md-block">
                        <span class="font-weight-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="DATE" />
                        </span>
                        <YAF:DisplayDateTime ID="PostedDateTime" runat="server" 
                                             DateTime='<%# Container.DataItemToField<DateTime>("Created") %>'></YAF:DisplayDateTime>
                    </small>
                </div>
                <p class="mb-1">
                    <span class="font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag='<%# this.View == PmView.Outbox ? "TO" : "FROM" %>' />:
                    </span>
                    <YAF:UserLink ID="UserLink1" runat="server" 
                                  UserID='<%# (this.View == PmView.Outbox ? this.Eval("ToUserID") : this.Eval("FromUserID" )).ToType<int>() %>' />
                </p>
            </li>
    </ItemTemplate>
</asp:Repeater>

<asp:UpdatePanel ID="upPanExport" runat="server">
    <ContentTemplate>
        <div class="btn-toolbar mt-3" role="toolbar">
            <div class="btn-group mr-2" role="group">
                <YAF:ThemeButton runat="server" ID="MarkAsRead" 
                                 Size="Small"
                                 TextLocalizedTag="MARK_ALL_ASREAD" 
                                 OnClick="MarkAsRead_Click"
                                 Type="Secondary" 
                                 Icon="eye"/>
            </div>
            <div class="btn-group mr-2" role="group">
                <YAF:ThemeButton runat="server" ID="ArchiveSelected" 
                                 Size="Small"
                                 TextLocalizedTag="ARCHIVESELECTED" 
                                 OnClick="ArchiveSelected_Click"
                                 Type="Secondary"
                                 Icon="archive" />
                <YAF:ThemeButton runat="server" ID="ArchiveAll" 
                                 Size="Small"
                                 TextLocalizedTag="ARCHIVEALL" 
                                 ReturnConfirmText='<%#this.GetText("CONFIRM_ARCHIVEALL") %>'
                                 OnClick="ArchiveAll_Click"
                                 Type="Secondary" Icon="archive" />
            </div>
            <div class="btn-group mr-2" role="group">
                <YAF:ThemeButton runat="server" ID="ExportSelected" 
                                 Size="Small"
                                 TextLocalizedTag="EXPORTSELECTED" 
                                 OnClick="ExportSelected_Click" 
                                 Type="Secondary" 
                                 Icon="file-export"/>
                <YAF:ThemeButton runat="server" ID="ExportAll"
                                 Size="Small"
                                 TextLocalizedTag="EXPORTALL" 
                                 OnClick="ExportAll_Click" 
                                 Type="Secondary" 
                                 Icon="file-export" />
            </div>
            <div class="btn-group" role="group">
                <YAF:ThemeButton runat="server" ID="DeleteSelected" 
                             Size="Small"
                             TextLocalizedTag="DELETESELECTED" 
                             ReturnConfirmText='<%#this.GetText("CONFIRM_DELETE") %>'
                             OnClick="DeleteSelected_Click"
                             Type="Secondary" 
                             Icon="trash" />
                <YAF:ThemeButton runat="server" ID="DeleteAll" 
                             Size="Small"
                             TextLocalizedTag="DELETEALL" 
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

<YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />

<asp:Label id="lblExportType" runat="server"></asp:Label>
<div class="custom-control custom-radio custom-control-inline">
    <asp:RadioButtonList runat="server" id="ExportType" 
                         RepeatLayout="UnorderedList"
                         CssClass="list-unstyled">
        <asp:ListItem Text="XML" Selected="True" Value="xml"></asp:ListItem>
        <asp:ListItem Text="CSV" Value="csv"></asp:ListItem>
        <asp:ListItem Text="Text" Value="txt"></asp:ListItem>
    </asp:RadioButtonList>
</div>

<YAF:Alert runat="server" ID="NoMessage" Type="info">
    <YAF:LocalizedLabel runat="server" LocalizedTag="NO_MESSAGES"></YAF:LocalizedLabel>
</YAF:Alert>

</div>



<div class="card-footer">
    <small class="text-muted">
        <asp:Label ID="PMInfoLink" runat="server"></asp:Label>
    </small>
</div>
