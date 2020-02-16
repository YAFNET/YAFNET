<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.nntpservers"
    CodeBehind="nntpservers.ascx.cs" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/NntpServerEdit.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPSERVERS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-newspaper fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPSERVERS" />
                </div>
                <div class="card-body">
                    <asp:Repeater ID="RankList" runat="server">
            <HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item list-group-item-action list-group-item-menu">
                    <div class="d-flex w-100 justify-content-between">
                        <h5 class="mb-1">
                            <%# this.Eval("Name") %>
                        </h5>
                    </div>
                    <p class="mb-1">
                        <span class="font-weight-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ADRESS" LocalizedPage="ADMIN_NNTPSERVERS" />:&nbsp;
                        </span>
                        <%# this.Eval("Address") %>
                    </p>
                    <small>
                        <YAF:ThemeButton runat="server" 
                                         CommandName="edit" 
                                         CommandArgument='<%# this.Eval( "ID") %>' 
                                         Type="Info" 
                                         Size="Small"
                                         Icon="edit" 
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton runat="server"  
                                         Type="Danger" 
                                         Size="Small"
                                         CommandName="delete" 
                                         CommandArgument='<%# this.Eval( "ID") %>'
                                         Icon="trash" 
                                         TextLocalizedTag="DELETE"
                                         ReturnConfirmText='<%#  this.GetText("ADMIN_NNTPSERVERS", "DELETE_SERVER") %>'>
                        </YAF:ThemeButton>
                    </small>
                    <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                        <YAF:ThemeButton runat="server" 
                                         CommandName="edit" 
                                         CommandArgument='<%# this.Eval( "ID") %>' 
                                         Type="None" 
                                         CssClass="dropdown-item"
                                         Icon="edit" 
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton runat="server"  
                                         Type="None" 
                                         CssClass="dropdown-item"
                                         CommandName="delete" 
                                         CommandArgument='<%# this.Eval( "ID") %>'
                                         Icon="trash" 
                                         TextLocalizedTag="DELETE"
                                         ReturnConfirmText='<%#  this.GetText("ADMIN_NNTPSERVERS", "DELETE_SERVER") %>'>
                        </YAF:ThemeButton>
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="NewServer" runat="server" 
                                         Type="None" 
                                         CssClass="dropdown-item"
                                         OnClick="NewServerClick"
                                         Icon="plus-square" 
                                         TextLocalizedTag="NEW_SERVER" />
                    </div>
                </li>
            </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
        </asp:Repeater>
                    </div>
                    <div class="card-footer text-center">
                        <YAF:ThemeButton ID="NewServer" runat="server" 
                                         Type="Primary" 
                                         OnClick="NewServerClick"
                                         Icon="plus-square" 
                                         TextLocalizedTag="NEW_SERVER" />
                    </div>
            </div>
        </div>
    </div>



<modal:Edit ID="EditDialog" runat="server" />