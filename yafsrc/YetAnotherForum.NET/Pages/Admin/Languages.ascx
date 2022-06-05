<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.Languages" Codebehind="Languages.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <div class="row justify-content-between align-items-center">
                        <div class="col-auto">
                            <YAF:IconHeader runat="server"
                                            IconName="language"
                                            LocalizedPage="ADMIN_LANGUAGES"></YAF:IconHeader>
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
        <asp:Repeater runat="server" ID="List">
            <HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item list-group-item-action list-group-item-menu">
                    <div class="d-flex w-100 justify-content-between text-break">
                        <h5 class="mb-1">
                            <%# this.Eval("CultureEnglishName")%>
                        </h5>
                        <small class="d-none d-md-block">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="CULTURE_TAG" LocalizedPage="ADMIN_LANGUAGES" />:
                            <span class="badge bg-secondary">
                                <%# this.Eval("CultureTag")%>
                            </span>
                        </small>
                    </div>
                    <strong><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="PROGRESS" LocalizedPage="ADMIN_LANGUAGES" />:</strong>
                    <div class="progress">
                        <div class="progress-bar" role="progressbar"
                             style="width: <%# this.Eval("TranslatedPercentage")%>%;"
                             aria-valuenow="<%# this.Eval("TranslatedPercentage")%>" aria-valuemin="0" aria-valuemax="100">
                            <%# this.Eval("TranslatedPercentage")%>% - (<%# this.Eval("TranslatedCount")%> of <%# this.Eval("TagsCount")%>)
                        </div>
                    </div>
                    <p><strong><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="FILENAME" />:</strong>
                        <%# this.Eval("CultureFile")%>
                    </p>
                    <small>
                        <div class="btn-group btn-group-sm">
                            <YAF:ThemeButton ID="btnEdit"
                                             Type="Info" Size="Small"
                                             CommandName="edit"
                                             CommandArgument='<%# this.Eval("CultureFile")%>'
                                             TitleLocalizedTag="EDIT"
                                             Icon="edit"
                                             TextLocalizedTag="EDIT"
                                             runat="server">
                            </YAF:ThemeButton>
                        </div>
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