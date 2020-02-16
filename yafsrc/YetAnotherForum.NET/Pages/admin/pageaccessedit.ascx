<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.pageaccessedit" Codebehind="pageaccessedit.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                LocalizedTag="TITLE"  
                                LocalizedPage="ADMIN_PAGEACCESSEDIT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-building fa-fw text-secondary pr-1"></i>
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                        LocalizedTag="HEADER" 
                                        LocalizedPage="ADMIN_PAGEACCESSEDIT" />
                    <YAF:LocalizedLabel ID="UserNameLabel" runat="server" 
                                        LocalizedTag="USERNAME" 
                                        LocalizedPage="ADMIN_PAGEACCESSEDIT" />:
                    <asp:Label ID="UserName" runat="server" 
                               CssClass="font-weight-bold" />
                </div>
                <div class="card-body">
                    <asp:Repeater ID="AccessList" OnItemDataBound="AccessList_OnItemDataBound" runat="server">
                        <HeaderTemplate>
                            <ul class="list-group">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item list-group-item-action">
                                <span class="font-weight-bold">
                                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                        LocalizedTag="PAGE" 
                                                        LocalizedPage="ADMIN_PAGEACCESSEDIT" />:
                                </span>
                                <asp:Label ID="PageName" runat="server" 
                                           AssociatedControlID="ReadAccess" />
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox  ID="ReadAccess" runat="server" 
                                                   Text='<%# this.GetText("CANACCESS") %>'/>
                                </div>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <div class="card-footer text-center">
				    <YAF:ThemeButton ID="Save" runat="server" 
                                     OnClick="SaveClick" 
                                     CssClass="mt-1"
                                     Type="Primary"
				                     Icon="save" 
                                     TextLocalizedTag="SAVE" 
                                     TextLocalizedPage="ADMIN_PAGEACCESSEDIT" />&nbsp;
                    <YAF:ThemeButton ID="GrantAll" runat="server" 
                                     OnClick="GrantAllClick" 
                                     CssClass="mt-1"
                                     Type="Info"
                                     Icon="check" 
                                     TextLocalizedTag="GRANTALL" 
                                     TextLocalizedPage="ADMIN_PAGEACCESSEDIT" />&nbsp;
                    <YAF:ThemeButton ID="RevokeAll" runat="server" 
                                     OnClick="RevokeAllClick" 
                                     CssClass="mt-1"
                                     Type="Danger"
                                     Icon="trash" 
                                     TextLocalizedTag="REVOKEALL" 
                                     TextLocalizedPage="ADMIN_PAGEACCESSEDIT" />&nbsp;
				    <YAF:ThemeButton ID="Cancel" runat="server" 
                                     OnClick="CancelClick" 
                                     CssClass="mt-1"
                                     Type="Secondary"
				                     Icon="times" 
                                     TextLocalizedTag="CANCEL" 
                                     TextLocalizedPage="ADMIN_PAGEACCESSEDIT" />
                </div>
            </div>
        </div>
    </div>


