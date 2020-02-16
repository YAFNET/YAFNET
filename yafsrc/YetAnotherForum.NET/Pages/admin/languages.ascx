<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.languages" Codebehind="languages.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_LANGUAGES" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-language fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_LANGUAGES" />
                </div>
                <div class="card-body">
		<asp:Repeater runat="server" ID="List">
			<HeaderTemplate>
                <div class="table-responsive">
			    <table class="table tablesorter table-bordered table-striped" id="language-table">
                    <thead class="thead-light">
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
                        <span class="float-right">
                        <YAF:ThemeButton ID="btnEdit"
                            Type="Info" Size="Small"
                            CommandName='edit'
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
                </tbody>
        </table></div>
                </FooterTemplate>
		</asp:Repeater>
            </div>
        </div>
    </div>
    </div>


