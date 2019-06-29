<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bbcode" Codebehind="BBCode.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/BBCodeImport.ascx" %>

<YAF:PageLinks ID="PageLinks" runat="server" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BBCODE" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <asp:Repeater ID="bbCodeList" runat="server" OnItemCommand="BbCodeListItemCommand">
                    <HeaderTemplate>
                        <div class="card-header">
                            <i class="fa fa-plug fa-fw"></i>&nbsp;<YAF:LocalizedLabel 
                                                                      ID="LocalizedLabel2" 
                                                                      runat="server" 
                                                                      LocalizedTag="HEADER" 
                                                                      LocalizedPage="ADMIN_BBCODE" />
                        </div>
                        <div class="card-body">
                            <ul class="list-group">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item list-group-item-action">
                                 <div class="d-flex w-100 justify-content-between">
                                    <h5 class="mb-1">
                                        <asp:CheckBox ID="chkSelected" runat="server" />
                                        <asp:HiddenField ID="hiddenBBCodeID" runat="server" Value='<%# this.Eval("ID") %>' />
                                        <%# this.Eval("Name") %>
                                    </h5>
                                </div>
                                <p class="mb-1">
                                    <%# this.Get<IBBCode>().LocalizeCustomBBCodeElement(this.Eval("Description").ToString())%></strong></p>
                                <small>
                                     <YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" Size="Small"
                                                         CommandName='edit' CommandArgument='<%# this.Eval( "ID") %>'
                                                         TitleLocalizedTag="EDIT"
                                                         Icon="edit"
                                                         TextLocalizedTag="EDIT"
                                                         runat="server">
                                    </YAF:ThemeButton>
                                    <YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" Size="Small"
                                                     CommandName='delete' CommandArgument='<%# this.Eval( "ID") %>'
                                                     TitleLocalizedTag="DELETE"
                                                     Icon="trash"
                                                     TextLocalizedTag="DELETE"
                                                     ReturnConfirmText='<%# this.GetText("ADMIN_BBCODE", "CONFIRM_DELETE") %>'  runat="server">
                                    </YAF:ThemeButton>
                                </small>
                        </li>
        	        </ItemTemplate>
                    <FooterTemplate>
                       </ul>
                    </div>
                    <div class="card-footer text-center">
                        <YAF:ThemeButton runat="server" 
                                         CommandName='add' ID="Linkbutton3" 
                                         Type="Primary"
                                         Icon="plus-square" 
                                         CssClass="mt-1"
                                         TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_BBCODE" />
                        <YAF:ThemeButton runat="server" Icon="upload" 
                                         DataTarget="BBCodeImportDialog"  
                                         DataToggle="modal"  
                                         ID="Linkbutton5" 
                                         Type="Info"
                                         CssClass="mt-1"
                                         TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_BBCODE" />
                        <YAF:ThemeButton runat="server" CommandName='export' ID="Linkbutton4" 
                                         Type="Warning"
                                         Icon="download" 
                                         CssClass="mt-1"
                                         TextLocalizedTag="EXPORT" TextLocalizedPage="ADMIN_BBCODE" />
                    </div>
        	            </FooterTemplate>
    	            </asp:Repeater>
            </div>
        </div>
    </div>

<modal:Import ID="ImportDialog" runat="server" />