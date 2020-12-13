<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.BBCodes" Codebehind="BBCodes.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/BBCodeImport.ascx" %>


<YAF:PageLinks ID="PageLinks" runat="server" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <div class="row justify-content-between align-items-center">
                        <div class="col-auto">
                            <YAF:IconHeader runat="server"
                                            IconName="plug"
                                            LocalizedTag="HEADER" 
                                            LocalizedPage="ADMIN_BBCODE"></YAF:IconHeader>
                        </div>
                        <div class="col-auto">
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
                <asp:Repeater ID="bbCodeList" runat="server" OnItemCommand="BbCodeListItemCommand">
                    <HeaderTemplate>
                        <div class="card-body">
                            <ul class="list-group">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item list-group-item-action list-group-item-menu">
                                 <div class="d-flex w-100 justify-content-between">
                                    <h5 class="mb-1">
                                        <asp:CheckBox ID="chkSelected" runat="server"
                                                      Text='<%# this.Eval("Name") %>'
                                                      CssClass="form-check" />
                                        <asp:HiddenField ID="hiddenBBCodeID" runat="server" Value='<%# this.Eval("ID") %>' />
                                    </h5>
                                </div>
                                <p class="mb-1">
                                    <%# this.Get<IBBCode>().LocalizeCustomBBCodeElement(this.Eval("Description").ToString())%></p>
                                <small>
                                    <div class="btn-group btn-group-sm">
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
                                                         ReturnConfirmText='<%# this.GetText("ADMIN_BBCODE", "CONFIRM_DELETE") %>'  runat="server">
                                        </YAF:ThemeButton>
                                    </div>
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
                                                     ReturnConfirmText='<%# this.GetText("ADMIN_BBCODE", "CONFIRM_DELETE") %>'  runat="server">
                                    </YAF:ThemeButton>
                                    <div class="dropdown-divider"></div>
                                    <YAF:ThemeButton runat="server" 
                                                     CommandName="add" ID="Linkbutton3" 
                                                     Type="None" 
                                                     CssClass="dropdown-item"
                                                     Icon="plus-square" 
                                                     TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_BBCODE" />
                                    <div class="dropdown-divider"></div>
                                    <YAF:ThemeButton runat="server" Icon="upload" 
                                                     DataTarget="BBCodeImportDialog"  
                                                     DataToggle="modal"  
                                                     ID="Linkbutton5" 
                                                     Type="None" 
                                                     CssClass="dropdown-item"
                                                     TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_BBCODE" />
                                    <YAF:ThemeButton runat="server" CommandName="export" ID="Linkbutton4" 
                                                     Type="None" 
                                                     CssClass="dropdown-item"
                                                     Icon="download" 
                                                     TextLocalizedTag="EXPORT" TextLocalizedPage="ADMIN_BBCODE" />
                                </div>
                        </li>
        	        </ItemTemplate>
                    <FooterTemplate>
                       </ul>
                    </div>
                    <div class="card-footer text-center">
                        <YAF:ThemeButton runat="server"  ID="Linkbutton3" 
                                         CssClass="mb-1"
                                         CommandName="add"
                                         Type="Primary"
                                         Icon="plus-square" 
                                         TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_BBCODE" />
                        <YAF:ThemeButton runat="server" ID="Linkbutton5"
                                         CssClass="mb-1"
                                         Icon="upload" 
                                         DataTarget="BBCodeImportDialog"  
                                         DataToggle="modal"  
                                         Type="Info"
                                         TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_BBCODE" />
                        <YAF:ThemeButton runat="server" ID="Linkbutton4"  
                                         CssClass="mb-1"
                                         CommandName="export" 
                                         Type="Warning"
                                         Icon="download" 
                                         TextLocalizedTag="EXPORT" TextLocalizedPage="ADMIN_BBCODE" />
                    </div>
        	            </FooterTemplate>
    	            </asp:Repeater>
            </div>
        </div>
    </div>
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
    </div>
</div>

<modal:Import ID="ImportDialog" runat="server" />