<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.Languages" Codebehind="Languages.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="language"
                                    LocalizedPage="ADMIN_LANGUAGES"></YAF:IconHeader>
                </div>
                <div class="card-body">
        <asp:Repeater runat="server" ID="List">
            <HeaderTemplate>
                <div class="table-responsive">
                <table class="table tablesorter table-bordered table-striped" id="language-table">
                    <thead class="table-light">
                        <tr>
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="LANG_NAME" LocalizedPage="ADMIN_LANGUAGES" />
                            </th>
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="CULTURE_TAG" LocalizedPage="ADMIN_LANGUAGES" />
                            </th>
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="NATIVE_NAME" LocalizedPage="ADMIN_LANGUAGES" />
                            </th>
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="FILENAME" />
                            </th>
                            <th>
                                &nbsp;
                            </th>
                        </tr>
                    </thead>
                <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# this.Eval("CultureEnglishName")%>
                    </td>
                    <td>
                        <%# this.Eval("CultureTag")%>
                    </td>
                     <td>
                        <%# this.Eval("CultureNativeName")%>
                    </td>
                    <td>
                        <%# this.Eval("CultureFile")%>
                    </td>
                    <td>
                        <span class="float-end">
                        <YAF:ThemeButton ID="btnEdit"
                            Type="Info" Size="Small"
                            CommandName="edit"
                            CommandArgument='<%# this.Eval("CultureFile")%>'
                            TitleLocalizedTag="EDIT"
                            Icon="edit"
                            TextLocalizedTag="EDIT"
                            runat="server">
                        </YAF:ThemeButton>
                            </span>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                 </table>
                        </div>
                        </div>
            <div class="card-footer">
                <div id="LanguagesPager" class="row justify-content-between align-items-center">
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
                </FooterTemplate>
        </asp:Repeater>
            </div>
        </div>
    </div>
    </div>


