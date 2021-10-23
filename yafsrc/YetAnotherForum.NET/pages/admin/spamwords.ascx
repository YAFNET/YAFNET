<%@ Control Language="c#" Debug="true" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.SpamWords" Codebehind="SpamWords.ascx.cs" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/SpamWordsImport.ascx" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/SpamWordsEdit.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <div class="row justify-content-between align-items-center">
                    <div class="col-auto">
                        <YAF:IconHeader runat="server"
                                        IconName="shield-alt"
                                        LocalizedPage="ADMIN_SPAMWORDS"></YAF:IconHeader>
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
                            <div class="btn-group btn-group-sm" role="group">
                        <YAF:ThemeButton runat="server"
                                         CssClass="dropdown-toggle"
                                         DataToggle="dropdown"
                                         Size="Small"
                                         Type="Secondary"
                                         Icon="filter"
                                         TextLocalizedTag="FILTER_DROPDOWN"
                                         TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
                        <div class="dropdown-menu dropdown-menu-end dropdown-menu-lg-start">
                            <div class="px-3 py-1">
                                <div class="mb-3">
                                    <YAF:HelpLabel ID="HelpLabel1" runat="server"
                                                   AssociatedControlID="SearchInput"
                                                   LocalizedTag="MASK" LocalizedPage="ADMIN_SPAMWORDS" />
                                    <asp:TextBox runat="server" ID="SearchInput"
                                                 CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="mb-3 d-grid gap-2">
                                    <YAF:ThemeButton ID="search" runat="server"
                                                     Type="Primary"
                                                     TextLocalizedTag="BTNSEARCH"
                                                     TextLocalizedPage="SEARCH"
                                                     Icon="search"
                                                     OnClick="SearchClick">
                                    </YAF:ThemeButton>
                                </div>
                            </div>
                            </div>
                                </div>
                        </div>
                    </div>
                    </div>
            </div>
                <div class="card-body">
                    <asp:Repeater ID="list" runat="server">
                <HeaderTemplate>
                    <ul class="list-group">
        </HeaderTemplate>
        <ItemTemplate>
            <li class="list-group-item list-group-item-action text-break list-group-item-menu">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1"><%# this.HtmlEncode(this.Eval("spamword")) %></h5>
                </div>
                <small>
                    <div class="btn-group btn-group-sm">
                        <YAF:ThemeButton ID="btnEdit"
                                         Type="Info"
                                         Size="Small"
                                         CommandName="edit"
                                         CommandArgument='<%# this.Eval("ID") %>'
                                         TextLocalizedTag="EDIT"
                                         TitleLocalizedTag="EDIT"
                                         Icon="edit"
                                         runat="server">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButtonDelete"
                                         Type="Danger"
                                         Size="Small"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_SPAMWORDS", "MSG_DELETE") %>'
                                         CommandName="delete"
                                         TextLocalizedTag="DELETE"
                                         CommandArgument='<%# this.Eval( "ID") %>'
                                         TitleLocalizedTag="DELETE"
                                         Icon="trash" runat="server">
                        </YAF:ThemeButton>
                    </div>
                </small>
                <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                    <YAF:ThemeButton ID="ThemeButton1"
                                     Type="None"
                                     CssClass="dropdown-item"
                                     CommandName="edit" CommandArgument='<%# this.Eval("ID") %>'
                                     TextLocalizedTag="EDIT" TitleLocalizedTag="EDIT" Icon="edit"
                                     runat="server">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButton2"
                                     Type="None"
                                     CssClass="dropdown-item"
                                     ReturnConfirmText='<%# this.GetText("ADMIN_SPAMWORDS", "MSG_DELETE") %>' CommandName="delete"
                                     TextLocalizedTag="DELETE"
                                     CommandArgument='<%# this.Eval( "ID") %>' TitleLocalizedTag="DELETE" Icon="trash" runat="server">
                    </YAF:ThemeButton>
                    <div class="dropdown-divider"></div>
                    <YAF:ThemeButton runat="server"
                                     Icon="plus-square"
                                     Type="None"
                                     CssClass="dropdown-item"
                                     TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_SPAMWORDS"
                                     OnClick="AddClick"></YAF:ThemeButton>
                    <div class="dropdown-divider"></div>
                    <YAF:ThemeButton runat="server"
                                     Icon="upload"
                                     DataToggle="modal"
                                     DataTarget="SpamWordsImportDialog"
                                     Type="None"
                                     CssClass="dropdown-item"
                                     TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_SPAMWORDS"></YAF:ThemeButton>
                    <YAF:ThemeButton runat="server" ID="Linkbutton4"
                                     OnClick="ExportClick"
                                     Type="None"
                                     CssClass="dropdown-item"
                                     Icon="download"
                                     TextLocalizedPage="ADMIN_SPAMWORDS" TextLocalizedTag="EXPORT"></YAF:ThemeButton>
                </div>
            </li>
        </ItemTemplate>
        <FooterTemplate>
                </ul>
        </FooterTemplate>
            </asp:Repeater>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server"
                                 CssClass="mb-1"
                                 Icon="plus-square"
                                 Type="Primary"
                                 TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_SPAMWORDS"
                                 OnClick="AddClick"></YAF:ThemeButton>
                <YAF:ThemeButton runat="server"
                                 CssClass="mb-1"
                                 Icon="upload"
                                 DataToggle="modal"
                                 DataTarget="SpamWordsImportDialog"
                                 Type="Info"
                                 TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_SPAMWORDS"></YAF:ThemeButton>
                <YAF:ThemeButton runat="server" ID="Linkbutton4"
                                 CssClass="mb-1"
                                 OnClick="ExportClick"
                                 Type="Warning"
                                 Icon="download"
                                 TextLocalizedPage="ADMIN_SPAMWORDS" TextLocalizedTag="EXPORT"></YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager ID="PagerTop" runat="server"
                   OnPageChange="PagerTopChange" />
    </div>
</div>



<modal:Import ID="ImportDialog" runat="server" />
<modal:Edit ID="EditDialog" runat="server" />