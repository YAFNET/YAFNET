<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.extensions" Codebehind="extensions.ascx.cs" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/ExtensionsImport.ascx" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/ExtensionsEdit.ascx" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
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
                    <i class="fa fa-puzzle-piece fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EXTENSIONS" />
                </div>
                <div class="card-body">
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
                    <span class="float-right">
                    <YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" CssClass="btn-sm"
                            CommandName='edit' CommandArgument='<%# this.Eval( "ID") %>'
                            TitleLocalizedTag="EDIT"
                            Icon="edit"
                            TextLocalizedTag="EDIT"
                            runat="server">
					    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm"
                                    CommandName='delete' CommandArgument='<%# this.Eval( "ID") %>'
                                    TitleLocalizedTag="DELETE"
                                    Icon="trash"
                                    TextLocalizedTag="DELETE"
                                    runat="server"
                                    ReturnConfirmText='<%# this.GetText("ADMIN_EXTENSIONS", "CONFIRM_DELETE") %>'>
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
                    <YAF:ThemeButton runat="server" CommandName='add' ID="Linkbutton3" Type="Primary"
                                     Icon="plus-square" TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_EXTENSIONS"></YAF:ThemeButton>
                    &nbsp;
                    <YAF:ThemeButton runat="server" Icon="upload" DataTarget="ExtensionsImportDialog"  ID="Linkbutton5" Type="Info"
                                     TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_EXTENSIONS"> </YAF:ThemeButton>
                    &nbsp;
                    <YAF:ThemeButton runat="server" CommandName='export' ID="Linkbutton4" Type="Warning"
                                     Icon="download" TextLocalizedTag="EXPORT" TextLocalizedPage="ADMIN_EXTENSIONS"></YAF:ThemeButton>
        	 </FooterTemplate>
    	 </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

<modal:Import ID="ImportDialog" runat="server" />
<modal:Edit ID="EditDialog" runat="server" />
