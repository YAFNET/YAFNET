<%@ Control Language="c#" Debug="true" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.replacewords" Codebehind="replacewords.ascx.cs" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/ReplaceWordsImport.ascx" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/ReplaceWordsEdit.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
	<asp:Repeater ID="list" runat="server">
		<HeaderTemplate>
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_REPLACEWORDS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-sticky-note fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_REPLACEWORDS" />
                </div>
                <div class="card-body">
                    <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                        <table class="table">
                            <tr>
                            <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="BAD" LocalizedPage="ADMIN_REPLACEWORDS" />
					</th>
					<th>
                       <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="GOOD" LocalizedPage="ADMIN_REPLACEWORDS" />
					</th>
					<th>
						&nbsp;
					</th>
				</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td>
					<%# this.HtmlEncode(this.Eval("badword")) %>
				</td>
				<td>
					<%# this.HtmlEncode(this.Eval("goodword")) %>
				</td>
				<td>
				    <span class="float-right">
					<YAF:ThemeButton ID="btnEdit" Type="Info" CssClass="btn-sm" CommandName='edit' CommandArgument='<%# this.Eval("ID") %>'
                        TextLocalizedTag="EDIT"
                        TitleLocalizedTag="EDIT" Icon="edit" runat="server">
					</YAF:ThemeButton>
					<YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm" 
					                 CommandName='delete'
                        TextLocalizedTag="DELETE"
                        CommandArgument='<%# this.Eval( "ID") %>' TitleLocalizedTag="DELETE" Icon="trash" runat="server"
					    ReturnConfirmText='<%# this.GetText("ADMIN_REPLACEWORDS", "MSG_DELETE") %>'>
					</YAF:ThemeButton>
                        </span>
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
            </table></div>
                </div>
                <div class="card-footer text-lg-center">
					<YAF:ThemeButton runat="server" CommandName='add' ID="Linkbutton3" Type="Primary"
					                 Icon="plus-square" TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_REPLACEWORDS"> </YAF:ThemeButton>
					&nbsp;
					<YAF:ThemeButton runat="server" Icon="upload" DataTarget="ReplaceWordsImportDialog" ID="Linkbutton5" Type="Info"
					                 TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_REPACEWORDS"></YAF:ThemeButton>
					&nbsp;
					<YAF:ThemeButton runat="server" CommandName='export' ID="Linkbutton4" Type="Warning"
					                 Icon="download" TextLocalizedTag="EXPORT" TextLocalizedPage="ADMIN_REPLACEWORDS"></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>
		</FooterTemplate>
	</asp:Repeater>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

<modal:Import ID="ImportDialog" runat="server" />
<modal:Edit ID="EditDialog" runat="server" />