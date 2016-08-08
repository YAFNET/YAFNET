<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.extensions" Codebehind="extensions.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1 class="page-header"><asp:Label ID="ExtensionTitle" runat="server" OnLoad="ExtensionTitle_Load">
                          <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EXTENSIONS" />
                        </asp:Label></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card card-primary-outline">
                <div class="card-header card-primary">
                    <i class="fa fa-puzzle-piece fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EXTENSIONS" />
                </div>
                <div class="card-block">
                    <asp:Repeater ID="list" runat="server">
                       <HeaderTemplate>
                           <div class="table-responsive">
      	                      <table class="table">
        	 </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <strong>*.<%# this.HtmlEncode(this.Eval("extension")) %></strong>
                </td>
                <td>
                    <span class="pull-right">
                    <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="btn btn-info btn-sm"
                            CommandName='edit' CommandArgument='<%# this.Eval( "extensionId") %>'
                            TitleLocalizedTag="EDIT"
                            Icon="edit"
                            TextLocalizedTag="EDIT"
                            runat="server">
					    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" CssClass="btn btn-danger btn-sm"
                                    CommandName='delete' CommandArgument='<%# this.Eval( "extensionId") %>'
                                    TitleLocalizedTag="DELETE"
                                    Icon="trash"
                                    TextLocalizedTag="DELETE"
                                    OnLoad="Delete_Load"  runat="server">
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
                    <asp:LinkButton runat="server" CommandName='add' ID="Linkbutton3" CssClass="btn btn-primary" OnLoad="addLoad"></asp:LinkButton>
                    &nbsp;
                    <asp:LinkButton runat="server" CommandName='import' ID="Linkbutton5" CssClass="btn btn-info" OnLoad="importLoad"> </asp:LinkButton>
                    &nbsp;
                    <asp:LinkButton runat="server" CommandName='export' ID="Linkbutton4" CssClass="btn btn-warning" OnLoad="exportLoad"></asp:LinkButton>
        	 </FooterTemplate>
    	 </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
