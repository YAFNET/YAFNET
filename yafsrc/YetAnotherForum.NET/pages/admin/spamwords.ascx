<%@ Control Language="c#" Debug="true" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.spamwords" Codebehind="spamwords.ascx.cs" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/SpamWordsImport.ascx" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/SpamWordsEdit.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
    <div class="row">
        <div class="col-xl-12">

            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMWORDS" /></h1>
        </div>
    </div>
    <div class="row">
    <div class="col-xl-12">
        <div class="alert alert-warning" role="alert">
            <YAF:LocalizedLabel ID="LocalizedLabelRequirementsText" runat="server"
                                LocalizedTag="NOTE" LocalizedPage="ADMIN_SPAMWORDS">
            </YAF:LocalizedLabel>
        </div>
        <div class="card mb-3">
            <div class="card-header">
                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="SEARCH" LocalizedPage="TOOLBAR" />
            </div>
            <div class="card-body">
                <h4>
                    <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="SPAM" LocalizedPage="ADMIN_SPAMWORDS_EDIT" />
                </h4>
                <p>
                    <asp:TextBox ID="SearchInput" runat="server" Width="90%" CssClass="form-control"></asp:TextBox>
                </p>
            </div>
            <div class="card-footer text-lg-center">
                <YAF:ThemeButton ID="search" runat="server"  Type="Primary" CssClass="btn-sm"
                                 TextLocalizedTag="BTNSEARCH" TextLocalizedPage="SEARCH" Icon="search"
                                 OnClick="SearchClick">
                </YAF:ThemeButton>
            </div>
        </div>

    <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTopChange" />
            <asp:Repeater ID="list" runat="server">
                <HeaderTemplate>
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-shield-alt fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMWORDS" />
					</div>
                <div class="card-body">
                    <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                        <table class="table">
                            <tr>
                                <thead>
                                    <th>
                                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SPAM" LocalizedPage="ADMIN_SPAMWORDS" />
					                </th>
					                <th class="header2">
					                    &nbsp;
					                </th>
                                </thead>
                            </tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td>
					<%# this.HtmlEncode(this.Eval("spamword")) %>
				</td>
				<td>
				    <span class="float-right">
					<YAF:ThemeButton ID="btnEdit" Type="Info" CssClass="btn-sm" CommandName='edit' CommandArgument='<%# this.Eval("ID") %>'
                        TextLocalizedTag="EDIT" TitleLocalizedTag="EDIT" Icon="edit" 
                        runat="server">
					</YAF:ThemeButton>
					<YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm" 
                        ReturnConfirmText='<%# this.GetText("ADMIN_SPAMWORDS", "MSG_DELETE") %>' CommandName='delete'
                        TextLocalizedTag="DELETE"
                        CommandArgument='<%# this.Eval( "ID") %>' TitleLocalizedTag="DELETE" Icon="trash" runat="server">
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
                    <YAF:ThemeButton runat="server" Icon="plus-square" Type="Primary"
                                     TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_SPAMWORDS" CommandName="add"></YAF:ThemeButton>
					&nbsp;
                    <YAF:ThemeButton runat="server" Icon="upload" DataTarget="SpamWordsImportDialog" Type="Info"
                        TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_SPAMWORDS"></YAF:ThemeButton>
					&nbsp;
					<YAF:ThemeButton runat="server" CommandName='export' ID="Linkbutton4" 
                        Type="Warning" Icon="download" TextLocalizedPage="ADMIN_SPAMWORDS" TextLocalizedTag="EXPORT"></YAF:ThemeButton>
                </div>
            </div>
		</FooterTemplate>
            </asp:Repeater>
	    <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
	    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

<modal:Import ID="ImportDialog" runat="server" />
<modal:Edit ID="EditDialog" runat="server" />