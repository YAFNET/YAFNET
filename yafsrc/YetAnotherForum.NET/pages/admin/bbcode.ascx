<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bbcode" Codebehind="BBCode.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/BBCodeImport.ascx" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BBCODE" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-plug fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BBCODE" />
                </div>
                <div class="card-body">
                    <asp:Repeater ID="bbCodeList" runat="server" OnItemCommand="BbCodeListItemCommand">
                        <HeaderTemplate>
                            <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                                <table class="table">
                                    <thead>
                                        <tr>
                    <th>
                      &nbsp;
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_BBCODE" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_BBCODE" />
                    </th>
                    <th>
                        &nbsp;
                    </th>
                                        </tr>
                                    </thead>
                        </HeaderTemplate>
                        <ItemTemplate>
            <tr>
                <td>
                    <asp:CheckBox ID="chkSelected" runat="server" />
                    <asp:HiddenField ID="hiddenBBCodeID" runat="server" Value='<%# this.Eval("ID") %>' />
                </td>
                <td>
                    <strong><%# this.Eval("Name") %></strong></td>
                <td>
                    <strong><%# this.Get<IBBCode>().LocalizeCustomBBCodeElement(this.Eval("Description").ToString())%></strong></td>
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
                                    ReturnConfirmText='<%# this.GetText("ADMIN_BBCODE", "CONFIRM_DELETE") %>'  runat="server">
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
                                     Icon="plus-square" TextLocalizedTag="ADD" TextLocalizedPage="ADMIN_BBCODE"></YAF:ThemeButton>
                    &nbsp;
                    <YAF:ThemeButton runat="server" Icon="upload" DataTarget="BBCodeImportDialog" ID="Linkbutton5" Type="Info"
                                     TextLocalizedTag="IMPORT" TextLocalizedPage="ADMIN_BBCODE"></YAF:ThemeButton>
                    &nbsp;
                    <YAF:ThemeButton runat="server" CommandName='export' ID="Linkbutton4" Type="Warning"
                                     Icon="download" TextLocalizedTag="EXPORT" TextLocalizedPage="ADMIN_BBCODE"></YAF:ThemeButton>
                </div>
        	            </FooterTemplate>
    	            </asp:Repeater>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

<modal:Import ID="ImportDialog" runat="server" />