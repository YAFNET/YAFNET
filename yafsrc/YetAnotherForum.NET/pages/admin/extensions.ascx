<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.extensions" Codebehind="extensions.ascx.cs" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/ExtensionsImport.ascx" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/ExtensionsEdit.ascx" %>


<YAF:PageLinks ID="PageLinks" runat="server" />

    <div class="row">
    <div class="col-xl-12">
        <h1><asp:Label ID="ExtensionTitle" runat="server" OnLoad="ExtensionTitleLoad">
                          <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EXTENSIONS" />
                        </asp:Label></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-puzzle-piece fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EXTENSIONS" />
                </div>
                <div class="card-body">
                    <asp:Repeater ID="list" runat="server">
                       <HeaderTemplate>
                           <ul class="list-group">
        	 </HeaderTemplate>
        <ItemTemplate>
            <li class="list-group-item list-group-item-action list-group-item-menu">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1">
                        *.<%# this.HtmlEncode(this.Eval("extension")) %>
                    </h5>
                </div>
                <small>
                    <YAF:ThemeButton ID="ThemeButtonEdit" 
                                     Type="Info" 
                                     Size="Small"
                                     CommandName="edit" CommandArgument='<%# this.Eval( "ID") %>'
                                     TitleLocalizedTag="EDIT"
                                     Icon="edit"
                                     TextLocalizedTag="EDIT"
                                     runat="server">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" 
                                     Type="Danger" 
                                     Size="Small"
                                     CommandName="delete" CommandArgument='<%# this.Eval( "ID") %>'
                                     TitleLocalizedTag="DELETE"
                                     Icon="trash"
                                     TextLocalizedTag="DELETE"
                                     runat="server"
                                     ReturnConfirmText='<%# this.GetText("ADMIN_EXTENSIONS", "CONFIRM_DELETE") %>'>
                    </YAF:ThemeButton>
                </small>
                <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                    <YAF:ThemeButton ID="ThemeButton1" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     CommandName="edit" CommandArgument='<%# this.Eval( "ID") %>'
                                     TitleLocalizedTag="EDIT"
                                     Icon="edit"
                                     TextLocalizedTag="EDIT"
                                     runat="server">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButton2" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     CommandName="delete" CommandArgument='<%# this.Eval( "ID") %>'
                                     TitleLocalizedTag="DELETE"
                                     Icon="trash"
                                     TextLocalizedTag="DELETE"
                                     runat="server"
                                     ReturnConfirmText='<%# this.GetText("ADMIN_EXTENSIONS", "CONFIRM_DELETE") %>'>
                    </YAF:ThemeButton>
                    <div class="dropdown-divider"></div>
                    <YAF:ThemeButton runat="server" 
                                     CommandName="add" 
                                     ID="Linkbutton3" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     Icon="plus-square" 
                                     TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_EXTENSIONS"></YAF:ThemeButton>
                    <div class="dropdown-divider"></div>
                    <YAF:ThemeButton runat="server" 
                                     Icon="upload"   
                                     DataToggle="modal" 
                                     DataTarget="ExtensionsImportDialog"
                                     ID="Linkbutton5" 
                                     Type="None" 
                                     CssClass="dropdown-item" 
                                     TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_EXTENSIONS"> </YAF:ThemeButton>
                    <YAF:ThemeButton runat="server" 
                                     CommandName="export" 
                                     ID="Linkbutton4" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     Icon="download"  
                                     TextLocalizedTag="EXPORT" TextLocalizedPage="ADMIN_EXTENSIONS"></YAF:ThemeButton>
                </div>
            </li>
        	 </ItemTemplate>
        <FooterTemplate>
                </ul>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton runat="server" 
                                     CommandName="add" 
                                     ID="Linkbutton3" 
                                     Type="Primary"
                                     Icon="plus-square" 
                                     CssClass="mt-1"
                                     TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_EXTENSIONS"></YAF:ThemeButton>
                    <YAF:ThemeButton runat="server" 
                                     Icon="upload"   
                                     DataToggle="modal" 
                                     DataTarget="ExtensionsImportDialog"
                                     ID="Linkbutton5" 
                                     Type="Info" 
                                     CssClass="mt-1"
                                     TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_EXTENSIONS"> </YAF:ThemeButton>
                    <YAF:ThemeButton runat="server" 
                                     CommandName="export" 
                                     ID="Linkbutton4" 
                                     Type="Warning"
                                     Icon="download"  
                                     CssClass="mt-1"
                                     TextLocalizedTag="EXPORT" TextLocalizedPage="ADMIN_EXTENSIONS"></YAF:ThemeButton>
        	 </FooterTemplate>
    	 </asp:Repeater>
                </div>
            </div>
        </div>
    </div>



<modal:Import ID="ImportDialog" runat="server" />
<modal:Edit ID="EditDialog" runat="server" />
