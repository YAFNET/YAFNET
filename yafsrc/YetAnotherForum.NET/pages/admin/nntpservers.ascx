<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.nntpservers"
    CodeBehind="nntpservers.ascx.cs" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/NntpServerEdit.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPSERVERS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-newspaper fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPSERVERS" />
                </div>
                <div class="card-body">
                    <asp:Repeater ID="RankList" runat="server">
            <HeaderTemplate>
                <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                <table class="table">
                <thead>
                <tr>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_NNTPSERVERS" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ADRESS" LocalizedPage="ADMIN_NNTPSERVERS" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="USERNAME" LocalizedPage="ADMIN_NNTPSERVERS" />
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
                        <%# this.Eval("Name") %>
                    </td>
                    <td>
                        <%# this.Eval("Address") %>
                    </td>
                    <td>
                        <%# this.Eval("UserName") %>
                    </td>
                    <td>
					    <span class="float-right">
                        <YAF:ThemeButton runat="server" CommandName="edit" CommandArgument='<%# this.Eval( "NntpServerID") %>' Type="Info" CssClass="btn-sm"
                                         Icon="edit" TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        &nbsp;
                        <YAF:ThemeButton runat="server"  Type="Danger" CssClass="btn-sm"
                                         CommandName="delete" CommandArgument='<%# this.Eval( "NntpServerID") %>'
                                         Icon="trash" TextLocalizedTag="DELETE"
                                         ReturnConfirmText='<%#  this.GetText("ADMIN_NNTPSERVERS", "DELETE_SERVER") %>'>
                        </YAF:ThemeButton>
					    </span>
                    </td>
                </tr>
            </ItemTemplate>
                        <FooterTemplate></table></div></FooterTemplate>
        </asp:Repeater>
                    </div>
                    <div class="card-footer text-center">
                    <YAF:ThemeButton ID="NewServer" runat="server" Type="Primary" OnClick="NewServerClick"
                                     Icon="plus-square" TextLocalizedTag="NEW_SERVER" />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

<modal:Edit ID="EditDialog" runat="server" />