<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.ProfileDefinitions" Codebehind="ProfileDefinitions.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Extensions" %>


<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/EditProfileDefinition.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <asp:Repeater ID="List" runat="server" OnItemCommand="ListItemCommand">
            <HeaderTemplate>
                <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="id-card"
                                    LocalizedPage="ADMIN_PROFILEDEFINITIONS"></YAF:IconHeader>
                </div>
                <div class="card-body">
                <ul class="list-group">
            </HeaderTemplate>
            <ItemTemplate>
				<li class="list-group-item list-group-item-action list-group-item-menu">
                    <div class="d-flex w-100 justify-content-between">
					<h5 class="mb-1">
                        <%# this.Eval("Name") %>
                    </h5>
                        <small class="d-none d-md-block">
                            <span class="font-weight-bold">
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="REQUIRED"></YAF:LocalizedLabel>
                            </span>
                            <span class='badge bg-<%# this.Eval("Required").ToType<bool>() ? "danger" : "secondary" %>'>
                                <%# this.GetText(this.Eval("Required").ToType<bool>() ? "YES" : "NO") %>
                            </span>
                        </small>
                    </div>
                    <p>
                    </p>
                    <small>
                        <div class="btn-group btn-group-sm">
                            <YAF:ThemeButton ID="ThemeButtonEdit" runat="server"
                                             Type="Info" 
                                             Size="Small"
                                             CommandName="edit" 
                                             CommandArgument='<%# this.Eval( "ID") %>'
                                             TitleLocalizedTag="EDIT"
                                             Icon="edit"
                                             TextLocalizedTag="EDIT">
                            </YAF:ThemeButton>
                            <YAF:ThemeButton ID="ThemeButtonDelete" runat="server"
                                             Type="Danger" 
                                             Size="Small"
                                             CommandName="delete" 
                                             CommandArgument='<%# this.Eval( "ID") %>'
                                             TitleLocalizedTag="DELETE"
                                             Icon="trash"
                                             TextLocalizedTag="DELETE"
                                             ReturnConfirmText='<%# this.GetText("ADMIN_PROFILEDEFINITIONS", "CONFIRM_DELETE") %>'>
                            </YAF:ThemeButton>
                        </div>
                        
                        <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                            <YAF:ThemeButton ID="ThemeButton1" runat="server"
                                             Type="None" 
                                             CssClass="dropdown-item"
                                             CommandName="edit" 
                                             CommandArgument='<%# this.Eval( "ID") %>'
                                             TitleLocalizedTag="EDIT"
                                             Icon="edit"
                                             TextLocalizedTag="EDIT">
                            </YAF:ThemeButton>
                            <YAF:ThemeButton ID="ThemeButton2" runat="server"
                                             Type="None"
                                             CssClass="dropdown-item"
                                             CommandName="delete" 
                                             CommandArgument='<%# this.Eval( "ID") %>'
                                             TitleLocalizedTag="DELETE"
                                             Icon="trash"
                                             TextLocalizedTag="DELETE"
                                             ReturnConfirmText='<%# this.GetText("ADMIN_PROFILEDEFINITIONS", "CONFIRM_DELETE") %>'>
                            </YAF:ThemeButton>
                            <div class="dropdown-divider"></div>
                            <YAF:ThemeButton ID="New" runat="server" 
                                             CommandName="new"
                                             Type="None" 
                                             CssClass="dropdown-item"
                                             Icon="plus-square" 
                                             TextLocalizedTag="NEW_DEF" />
                        </div>
                    </small>
                </li>
			</ItemTemplate>
            <FooterTemplate>
                </ul>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="New" runat="server" 
                                 CommandName="new"
                                 Type="Primary" 
                                 Icon="plus-square" 
                                 TextLocalizedTag="NEW_DEF" />
                    </div>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</div>

<modal:Edit ID="EditDialog" runat="server" />