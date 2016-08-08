<%@ Control Language="c#" Debug="true" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.spamwords" Codebehind="spamwords.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
	<asp:Repeater ID="list" runat="server">
		<HeaderTemplate>
    <div class="row">
    <div class="col-xl-12">
        <h1 class="page-header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMWORDS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="alert alert-warning" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabelRequirementsText" runat="server"
                                LocalizedTag="NOTE" LocalizedPage="ADMIN_SPAMWORDS">
                            </YAF:LocalizedLabel>
                        </div>
            <div class="card card-primary-outline">
                <div class="card-header card-primary">
                    <i class="fa fa-shield fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMWORDS" />
					</div>
                <div class="card-block">
                    <div class="alert alert-info hidden-sm-up" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="pull-right"><i class="fa fa-hand-o-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                        <table class="table">
                            <tr>
                                <thead>
                                    <th>
                                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SPAM" LocalizedPage="ADMIN_SPAMWORDS" />
					                </th>
					                <th class="header2">
					                    &nbsp;
					                </th>
                                </thead>
                            </tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td>
					<%# this.HtmlEncode(this.Eval("spamword")) %>
				</td>
				<td>
				    <span class="pull-right">
					<YAF:ThemeButton ID="btnEdit" CssClass="btn btn-info btn-sm" CommandName='edit' CommandArgument='<%# this.Eval("ID") %>'
                        TextLocalizedTag="EDIT"
                        TitleLocalizedTag="EDIT" Icon="edit" runat="server">
					</YAF:ThemeButton>
					<YAF:ThemeButton ID="ThemeButtonDelete" CssClass="btn btn-danger btn-sm" OnLoad="Delete_Load"  CommandName='delete'
                        TextLocalizedTag="DELETE"
                        CommandArgument='<%# this.Eval( "ID") %>' TitleLocalizedTag="DELETE" Icon="trash" runat="server">
					</YAF:ThemeButton>
                        </span>
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
                    </table>
                </div>
			</div>
                <div class="card-footer text-lg-center">
					<asp:LinkButton runat="server" CommandName='add' ID="Linkbutton3" CssClass="btn btn-primary" OnLoad="AddLoad"> </asp:LinkButton>
					&nbsp;
					<asp:LinkButton runat="server" CommandName='import' ID="Linkbutton5" CssClass="btn btn-info" OnLoad="ImportLoad"></asp:LinkButton>
					&nbsp;
					<asp:LinkButton runat="server" CommandName='export' ID="Linkbutton4" CssClass="btn btn-warning" OnLoad="ExportLoad"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
		</FooterTemplate>
	</asp:Repeater>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
