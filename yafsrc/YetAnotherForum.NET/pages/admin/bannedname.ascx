<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.bannedname" Codebehind="bannedname.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Core.Extensions" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/BannedNameImport.ascx" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/BannedNameEdit.ascx" %>



<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDNAME" /></h1>
    </div>
</div>
<div class="row">
    <div class="col-xl-12">
             <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-hand-paper fa-fw text-secondary pr-1"></i>
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                        LocalizedTag="TITLE" 
                                        LocalizedPage="ADMIN_BANNEDNAME" />
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
                                                   LocalizedTag="MASK" LocalizedPage="ADMIN_BANNEDNAME" />
                                    <asp:TextBox ID="SearchInput" runat="server" 
                                                 CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <YAF:ThemeButton ID="search" runat="server" 
                                                     Type="Primary" 
                                                     TextLocalizedTag="BTNSEARCH" 
                                                     TextLocalizedPage="SEARCH" 
                                                     Icon="search"
                                                     OnClick="Search_Click">
                                    </YAF:ThemeButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-body">
    <asp:Repeater ID="list" runat="server" OnItemCommand="List_ItemCommand">
		<HeaderTemplate>
            <ul class="list-group">
			</HeaderTemplate>
		<ItemTemplate>
            <li class="list-group-item list-group-item-action list-group-item-menu">
                <div class="d-flex w-100 justify-content-between">
                    <asp:HiddenField ID="fID" Value='<%# this.Eval("ID") %>' runat="server"/>
                    <h5 class="mb-1 text-break">
                        <asp:Label ID="MaskBox" Text='<%# this.Eval("Mask") %>' runat="server"></asp:Label>
                    </h5>
                    <small class="d-none d-md-block">
                        <span class="font-weight-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="SINCE" LocalizedPage="ADMIN_BANNEDNAME" />
                        </span>
                        <%# this.Get<IDateTime>().FormatDateTime(this.Eval("Since")) %>
                    </small>
                </div>
                <p class="mb-1">
                    <span class="font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="REASON" LocalizedPage="ADMIN_BANNEDNAME" />
                    </span>
                    <%# this.Eval("Reason") %>
                </p>
                <small>
                    <YAF:ThemeButton ID="ThemeButtonEdit" 
                                     Type="Info" Size="Small" 
                                     CommandName="edit" CommandArgument='<%# this.Eval("ID") %>'
                                     TextLocalizedTag="EDIT"
                                     TitleLocalizedTag="EDIT" 
                                     Icon="edit" runat="server"></YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" 
                                     Type="Danger" Size="Small" 
                                     CommandName="delete" CommandArgument='<%# this.Eval("ID") %>'
                                     TextLocalizedTag="DELETE" ReturnConfirmText='<%# this.GetText("ADMIN_BANNEDIP", "MSG_DELETE") %>'
                                     TitleLocalizedTag="DELETE" Icon="trash" runat="server"></YAF:ThemeButton>
                </small>
                <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                    <YAF:ThemeButton ID="ThemeButton1" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     CommandName="edit" CommandArgument='<%# this.Eval("ID") %>'
                                     TextLocalizedTag="EDIT"
                                     TitleLocalizedTag="EDIT" 
                                     Icon="edit" runat="server"></YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButton2" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     CommandName="delete" CommandArgument='<%# this.Eval("ID") %>'
                                     TextLocalizedTag="DELETE" ReturnConfirmText='<%# this.GetText("ADMIN_BANNEDIP", "MSG_DELETE") %>'
                                     TitleLocalizedTag="DELETE" Icon="trash" runat="server"></YAF:ThemeButton>
                    <div class="dropdown-divider"></div>
                    <YAF:ThemeButton runat="server" 
                                     Icon="plus-square" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     TextLocalizedTag="ADD_IP" TextLocalizedPage="ADMIN_BANNEDNAME" 
                                     CommandName="add"></YAF:ThemeButton>
                    <div class="dropdown-divider"></div>
                    <YAF:ThemeButton runat="server" 
                                     Icon="upload" 
                                     DataToggle="modal" 
                                     DataTarget="ImportDialog" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     TextLocalizedTag="IMPORT_IPS" TextLocalizedPage="ADMIN_BANNEDNAME"></YAF:ThemeButton>
                    <YAF:ThemeButton runat="server" 
                                     CommandName="export" 
                                     ID="Linkbutton4" 
                                     Type="None" 
                                     CssClass="dropdown-item"
                                     Icon="download" 
                                     TextLocalizedPage="ADMIN_BANNEDIP" TextLocalizedTag="EXPORT"></YAF:ThemeButton>
                </div>
            </li>
			</ItemTemplate>
		<FooterTemplate>
        </ul>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton runat="server" 
                                     Icon="plus-square" 
                                     Type="Primary"
                                     TextLocalizedTag="ADD_IP" TextLocalizedPage="ADMIN_BANNEDNAME" 
                                     CommandName="add"></YAF:ThemeButton>
                    <YAF:ThemeButton runat="server" 
                                     Icon="upload" 
                                     DataToggle="modal" 
                                     DataTarget="ImportDialog" 
                                     Type="Info"
                                     TextLocalizedTag="IMPORT_IPS" TextLocalizedPage="ADMIN_BANNEDNAME"></YAF:ThemeButton>
                    <YAF:ThemeButton runat="server" 
                                     CommandName="export" 
                                     ID="Linkbutton4" 
                                     Type="Warning" 
                                     Icon="download" 
                                     TextLocalizedPage="ADMIN_BANNEDIP" TextLocalizedTag="EXPORT"></YAF:ThemeButton>
                </div>
            </div>

			</FooterTemplate>
		</asp:Repeater>
	 <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
            </div>
        </div>
    </div>
</div>



<modal:Import ID="ImportDialog" runat="server" />
<modal:Edit ID="EditDialog" runat="server" />
