<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.boards" Codebehind="boards.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                    LocalizedTag="TITLE" 
                                    LocalizedPage="ADMIN_BOARDS" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-globe fa-fw text-secondary pr-1"></i>
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                        LocalizedTag="TITLE"
                                        LocalizedPage="ADMIN_BOARDS" />
                </div>
                <div class="card-body">
                    <asp:Repeater ID="List" runat="server">
		                <HeaderTemplate>
                            <ul class="list-group">
		    </HeaderTemplate>
			<ItemTemplate>
                <li 
                    class='list-group-item list-group-item-action <%# this.Eval("ID").ToType<int>() != this.PageContext.PageBoardID ? "" : "active" %>'>
				<div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1">
                        <%# this.HtmlEncode(this.Eval( "Name")) %>
                    </h5>
                    <small>
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                            LocalizedTag="ID" 
                                            LocalizedPage="ADMIN_BOARDS" />: <%# this.Eval( "ID") %>
                    </small>
                </div>
                <small>
					    <YAF:ThemeButton ID="ThemeButtonEdit" runat="server"
                                         Type="Info" 
                                         Size="Small"
                                         CommandName="edit" 
                                         CommandArgument='<%# this.Eval( "ID") %>'
                                         TitleLocalizedTag="EDIT"
                                         TextLocalizedTag="EDIT"
                                         Icon="edit">
					    </YAF:ThemeButton>
                        &nbsp;
                        <YAF:ThemeButton ID="ThemeButtonDelete" runat="server" 
                                         Type="Danger" 
                                         Size="Small"
                                         CommandName="delete" 
                                         CommandArgument='<%# this.Eval( "ID") %>'
                                         TitleLocalizedTag="DELETE"
                                         TextLocalizedTag="DELETE"
                                         Icon="trash"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_BOARDS", "CONFIRM_DELETE") %>'>
                        </YAF:ThemeButton>
                    </small>
                </li>
			</ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
		</asp:Repeater>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="New" runat="server" 
                                     Type="Primary" 
                                     TextLocalizedTag="NEW_BOARD"
                                     Icon="plus-square"></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>


