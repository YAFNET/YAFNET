<%@ Control Language="c#" Debug="true" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.spamwords" Codebehind="spamwords.ascx.cs" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/SpamWordsImport.ascx" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/SpamWordsEdit.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <h1>
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                    LocalizedTag="TITLE" 
                                    LocalizedPage="ADMIN_SPAMWORDS" />
            </h1>
        </div>
    </div>
<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-shield-alt fa-fw text-secondary pr-1"></i>
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                        LocalizedTag="TITLE"
                                        LocalizedPage="ADMIN_SPAMWORDS" />
                    <div class="float-right">
                        <YAF:ThemeButton runat="server"
                                         CssClass="dropdown-toggle"
                                         DataToggle="dropdown"
                                         Type="Secondary"
                                         Icon="filter"
                                         TextLocalizedTag="FILTER_DROPDOWN"
                                         TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
                        <div class="dropdown-menu">
                            <div class="px-3 py-1">
                                <div class="form-group">
                                    <YAF:HelpLabel ID="HelpLabel1" runat="server"
                                                   AssociatedControlID="SearchInput"
                                                   LocalizedTag="MASK" LocalizedPage="ADMIN_SPAMWORDS" />
                                    <asp:TextBox runat="server" ID="SearchInput"
                                                 CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <YAF:ThemeButton ID="search" runat="server"  
                                                     Type="Primary"
                                                     TextLocalizedTag="BTNSEARCH" 
                                                     TextLocalizedPage="SEARCH" 
                                                     Icon="search"
                                                     OnClick="SearchClick">
                                    </YAF:ThemeButton>
                                </div>
                            </div>
                        </div>
                    </div>
					</div>
                <div class="card-body">
    <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTopChange" />
            <asp:Repeater ID="list" runat="server">
                <HeaderTemplate>
                    <ul class="list-group">
		</HeaderTemplate>
		<ItemTemplate>
            <li class="list-group-item list-group-item-action text-break list-group-item-menu">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1"><%# this.HtmlEncode(this.Eval("spamword")) %></h5>
                </div>
                <small>
                    <YAF:ThemeButton ID="btnEdit" Type="Info" Size="Small" CommandName="edit" CommandArgument='<%# this.Eval("ID") %>'
                                     TextLocalizedTag="EDIT" TitleLocalizedTag="EDIT" Icon="edit" 
                                     runat="server">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" Size="Small" 
                                     ReturnConfirmText='<%# this.GetText("ADMIN_SPAMWORDS", "MSG_DELETE") %>' CommandName='delete'
                                     TextLocalizedTag="DELETE"
                                     CommandArgument='<%# this.Eval( "ID") %>' TitleLocalizedTag="DELETE" Icon="trash" runat="server">
                    </YAF:ThemeButton>
                </small>
                <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                    <YAF:ThemeButton ID="ThemeButton1" 
                                     Type="None" 
                                     CssClass="dropdown-item" 
                                     CommandName="edit" CommandArgument='<%# this.Eval("ID") %>'
                                     TextLocalizedTag="EDIT" TitleLocalizedTag="EDIT" Icon="edit" 
                                     runat="server">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButton2" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     ReturnConfirmText='<%# this.GetText("ADMIN_SPAMWORDS", "MSG_DELETE") %>' CommandName='delete'
                                     TextLocalizedTag="DELETE"
                                     CommandArgument='<%# this.Eval( "ID") %>' TitleLocalizedTag="DELETE" Icon="trash" runat="server">
                    </YAF:ThemeButton>
                    <div class="dropdown-divider"></div>
                    <YAF:ThemeButton runat="server" 
                                     Icon="plus-square" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_SPAMWORDS"
                                     OnClick="AddClick"></YAF:ThemeButton>
                    <div class="dropdown-divider"></div>
                    <YAF:ThemeButton runat="server" 
                                     Icon="upload"   
                                     DataToggle="modal" 
                                     DataTarget="SpamWordsImportDialog" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_SPAMWORDS"></YAF:ThemeButton>
                    <YAF:ThemeButton runat="server" ID="Linkbutton4"
                                     OnClick="ExportClick"
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     Icon="download" 
                                     TextLocalizedPage="ADMIN_SPAMWORDS" TextLocalizedTag="EXPORT"></YAF:ThemeButton>
                </div>
			</li>
		</ItemTemplate>
		<FooterTemplate>
                </ul>
        </FooterTemplate>
            </asp:Repeater>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server" 
                                 Icon="plus-square" 
                                 Type="Primary"
                                 TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_SPAMWORDS"
                                 OnClick="AddClick"></YAF:ThemeButton>
                <YAF:ThemeButton runat="server" 
                                 Icon="upload"   
                                 DataToggle="modal" 
                                 DataTarget="SpamWordsImportDialog" 
                                 Type="Info"
                                 TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_SPAMWORDS"></YAF:ThemeButton>
                <YAF:ThemeButton runat="server" ID="Linkbutton4"
                                 OnClick="ExportClick"
                                 Type="Warning" 
                                 Icon="download" 
                                 TextLocalizedPage="ADMIN_SPAMWORDS" TextLocalizedTag="EXPORT"></YAF:ThemeButton>
            </div>
        </div>
	    <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
    </div>
</div>



<modal:Import ID="ImportDialog" runat="server" />
<modal:Edit ID="EditDialog" runat="server" />