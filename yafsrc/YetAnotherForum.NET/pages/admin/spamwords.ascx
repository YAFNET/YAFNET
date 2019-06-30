<%@ Control Language="c#" Debug="true" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.spamwords" Codebehind="spamwords.ascx.cs" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/SpamWordsImport.ascx" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/SpamWordsEdit.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">

            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMWORDS" /></h1>
        </div>
    </div>
<div class="row">
    <div class="col-xl-12">
        <YAF:Alert runat="server" Type="warning">
            <YAF:LocalizedLabel ID="LocalizedLabelRequirementsText" runat="server"
                                LocalizedTag="NOTE" LocalizedPage="ADMIN_SPAMWORDS">
            </YAF:LocalizedLabel>
        </YAF:Alert>
        <div class="card mb-3">
            <div class="card-header">
                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="SEARCH" LocalizedPage="TOOLBAR" />
            </div>
            <div class="card-body">
                
                    <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="SPAM" LocalizedPage="ADMIN_SPAMWORDS_EDIT" />
                 
                <p>
                    <asp:TextBox ID="SearchInput" runat="server" Width="90%" CssClass="form-control"></asp:TextBox>
                </p>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="search" runat="server"  Type="Primary" Size="Small"
                                 TextLocalizedTag="BTNSEARCH" TextLocalizedPage="SEARCH" Icon="search"
                                 OnClick="SearchClick">
                </YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>
<div class="row">
    <div class="col-xl-12">

    <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTopChange" />
            <asp:Repeater ID="list" runat="server">
                <HeaderTemplate>
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-shield-alt fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                                                    LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMWORDS" />
					</div>
                <div class="card-body">
                <ul class="list-group">
		</HeaderTemplate>
		<ItemTemplate>
            <li class="list-group-item list-group-item-action text-break">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1"><%# this.HtmlEncode(this.Eval("spamword")) %></h5>
                </div>
                <small>
                    <YAF:ThemeButton ID="btnEdit" Type="Info" Size="Small" CommandName='edit' CommandArgument='<%# this.Eval("ID") %>'
                                     TextLocalizedTag="EDIT" TitleLocalizedTag="EDIT" Icon="edit" 
                                     runat="server">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" Size="Small" 
                                     ReturnConfirmText='<%# this.GetText("ADMIN_SPAMWORDS", "MSG_DELETE") %>' CommandName='delete'
                                     TextLocalizedTag="DELETE"
                                     CommandArgument='<%# this.Eval( "ID") %>' TitleLocalizedTag="DELETE" Icon="trash" runat="server">
                    </YAF:ThemeButton>
                </small>
			</li>
		</ItemTemplate>
		<FooterTemplate>
                </ul>
			</div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton runat="server" 
                                     Icon="plus-square" 
                                     Type="Primary"
                                     TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_SPAMWORDS" 
                                     CommandName="add"></YAF:ThemeButton>
					&nbsp;
                    <YAF:ThemeButton runat="server" 
                                     Icon="upload"   
                                     DataToggle="modal" 
                                     DataTarget="SpamWordsImportDialog" 
                                     Type="Info"
                                     TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_SPAMWORDS"></YAF:ThemeButton>
					&nbsp;
					<YAF:ThemeButton runat="server" 
                                     CommandName='export' 
                                     ID="Linkbutton4"
                                     Type="Warning" 
                                     Icon="download" 
                                     TextLocalizedPage="ADMIN_SPAMWORDS" TextLocalizedTag="EXPORT"></YAF:ThemeButton>
                </div>
            </div>
		</FooterTemplate>
            </asp:Repeater>
	    <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
    </div>
</div>



<modal:Import ID="ImportDialog" runat="server" />
<modal:Edit ID="EditDialog" runat="server" />