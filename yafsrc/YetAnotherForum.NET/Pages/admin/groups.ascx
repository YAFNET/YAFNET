<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.groups" Codebehind="groups.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_GROUPS" /></h1>
    </div>
    </div>
    <div class="row" runat="server" Visible="<%# this.RoleListNet.Items.Count > 0 %>">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-users fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                                                               LocalizedTag="PROVIDER_ROLES" 
                                                                               LocalizedPage="ADMIN_GROUPS" />
                </div>
                <div class="card-body">
                    <YAF:Alert runat="server" Type="danger">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                            LocalizedTag="NOTE_DELETE" 
                                            LocalizedPage="ADMIN_GROUPS" />
                    </YAF:Alert>
		<asp:Repeater ID="RoleListNet" runat="server" OnItemCommand="RoleListNetItemCommand">
			<HeaderTemplate>
                <ul class="list-group">
			</HeaderTemplate>
			<ItemTemplate>
                 <li class="list-group-item list-group-item-action">
                 <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1 text-break">
                        <%# Container.DataItem %>
                    </h5>
                     <small class="text-muted">
                         <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedPage="ADMIN_GROUPS" LocalizedTag="UNLINKED" />
                     </small>
                 </div>
                     <small>
                    <YAF:ThemeButton ID="ThemeButtonAdd" runat="server"
                                     Type="Info" 
                                     Size="Small"
                                     CommandName="add" CommandArgument="<%# Container.DataItem %>"
                                     TitleLocalizedTag="ADD_ROLETOYAF"
                                     TitleLocalizedPage="ADMIN_GROUPS"
                                     TextLocalizedTag="ADD_ROLETOYAF"
                                     TextLocalizedPage="ADMIN_GROUPS"
                                     Icon="plus-circle">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" runat="server"
                                     Type="Danger" 
                                     Size="Small"
                                     CommandName="delete" CommandArgument="<%# Container.DataItem %>"
                                     TitleLocalizedTag="DELETE"
                                     Icon="trash"
                                     TextLocalizedTag="DELETE"
                                     ReturnConfirmText='<%# this.GetText("ADMIN_GROUPS", "CONFIRM_DELETE") %>'>
                    </YAF:ThemeButton>
                     </small>
            </li>
			</ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
		</asp:Repeater>
                </div>

            </div>
        </div>
    </div>

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-users fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                                               LocalizedTag="HEADER" 
                                                                               LocalizedPage="ADMIN_GROUPS" />
                </div>
                <div class="card-body">
                    <YAF:Alert runat="server" Type="danger">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                            LocalizedTag="NOTE_DELETE_LINKED" 
                                            LocalizedPage="ADMIN_GROUPS" />
                    </YAF:Alert>
		<asp:Repeater ID="RoleListYaf" runat="server" OnItemCommand="RoleListYafItemCommand">
			<HeaderTemplate>
                <ul class="list-group">
			</HeaderTemplate>
			<ItemTemplate>
                 <li class="list-group-item list-group-item-action list-group-item-menu">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1 text-break">
                        <i class="fa fa-users fa-fw"></i>&nbsp;<%# this.Eval( "Name" ) %>
                    </h5>
                    <small class="text-muted">
                        <%# this.GetLinkedStatus((YAF.Types.Models.Group)Container.DataItem )%>
                    </small>
                </div>
                <p>
                    <asp:PlaceHolder runat="server"
                                     Visible="<%# ((YAF.Types.Models.Group)Container.DataItem).Description.IsSet() %>">
                        <YAF:LocalizedLabel ID="HelpLabel6" runat="server" 
                                            LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_EDITGROUP">
                        </YAF:LocalizedLabel>
                        &nbsp;<%# this.Eval("Description") %>
                    </asp:PlaceHolder>
                    <ul class="list-inline">
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel  ID="HelpLabel12" runat="server" 
                                                 LocalizedTag="PRIORITY" LocalizedPage="ADMIN_EDITGROUP" />&nbsp;
                            <asp:Label ID="Label11" runat="server" 
                                       CssClass='<%# this.GetItemColorString(this.Eval( "SortOrder" ).ToString()) %>'>
                                <%# this.Eval("SortOrder").ToString()%>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" 
                                                LocalizedTag="IS_GUEST" LocalizedPage="ADMIN_GROUPS" />:&nbsp;
                            <asp:Label ID="Label2" runat="server" 
                                       CssClass='<%# this.GetItemColor(this.Eval( "Flags" ).BinaryAnd(2)) %>'>
                                <%# this.GetItemName(this.Eval( "Flags" ).BinaryAnd(2)) %>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" 
                                                LocalizedTag="IS_START" LocalizedPage="ADMIN_GROUPS" />:&nbsp;
                            <asp:Label ID="Label1" runat="server" 
                                       CssClass='<%# this.GetItemColor(this.Eval( "Flags" ).BinaryAnd(4)) %>'>
                                <%# this.GetItemName(this.Eval( "Flags" ).BinaryAnd(4)) %>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" 
                                                LocalizedTag="IS_MOD" LocalizedPage="ADMIN_GROUPS" />:&nbsp;
                            <asp:Label ID="Label3" runat="server" 
                                       CssClass='<%# this.GetItemColor(this.Eval( "Flags" ).BinaryAnd(8)) %>'>
                                <%# this.GetItemName(this.Eval( "Flags" ).BinaryAnd(8)) %>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" 
                                                LocalizedTag="IS_ADMIN" LocalizedPage="ADMIN_GROUPS" />:&nbsp;
                            <asp:Label ID="Label4" runat="server" 
                                       CssClass='<%# this.GetItemColor(this.Eval( "Flags" ).BinaryAnd(1)) %>'>
                                <%# this.GetItemName(this.Eval( "Flags" ).BinaryAnd(1)) %>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" 
                                                LocalizedTag="PMS" LocalizedPage="ADMIN_GROUPS" />:&nbsp;
                            <asp:Label ID="Label6" runat="server" 
                                       CssClass='<%# this.GetItemColorString(this.Eval("PMLimit").ToType<int>() == int.MaxValue ? "\u221E" : this.Eval("PMLimit").ToString()) %>'>
                                <%# this.Eval("PMLimit").ToType<int>() == int.MaxValue ? "\u221E" : this.Eval("PMLimit").ToString()%>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel  ID="HelpLabel10" runat="server" 
                                                 LocalizedTag="ALBUM_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />&nbsp;
                            <asp:Label ID="Label9" runat="server" 
                                       CssClass='<%# this.GetItemColorString(this.Eval( "UsrAlbums" ).ToString()) %>'>
                                <%# this.Eval("UsrAlbums").ToString()%>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel  ID="HelpLabel11" runat="server" 
                                                 LocalizedTag="IMAGES_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />
                            <asp:Label ID="Label10" runat="server" 
                                       CssClass='<%# this.GetItemColorString(this.Eval( "UsrAlbumImages" ).ToString()) %>'>
                                <%# this.Eval("UsrAlbumImages").ToString()%>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel  ID="HelpLabel13" runat="server" 
                                                 LocalizedTag="STYLE" LocalizedPage="ADMIN_EDITGROUP" />&nbsp;
                            <asp:Label ID="Label12" runat="server" 
                                       CssClass="<%# this.GetItemColorString(((YAF.Types.Models.Group)Container.DataItem).Style) %>">
                                <%#((YAF.Types.Models.Group)Container.DataItem).Style.IsSet() && this.Eval("Style").ToString().Trim().Length > 0 ? "" : this.GetItemName(false)%>
                            </asp:Label>
                            <code><%# ((YAF.Types.Models.Group)Container.DataItem).Style %></code>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel ID="HelpLabel7" runat="server" 
                                                LocalizedTag="SIGNATURE_LENGTH" LocalizedPage="ADMIN_EDITGROUP" />&nbsp;
                            <asp:Label ID="Label5" runat="server" 
                                       CssClass="<%# this.GetItemColorString(((YAF.Types.Models.Group)Container.DataItem).UsrSigChars.ToString()) %>">
                                <%# ((YAF.Types.Models.Group)Container.DataItem).UsrSigChars.ToString().IsSet() ? this.Eval("UsrSigChars").ToString() : this.GetItemName(false) %>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel ID="HelpLabel8" runat="server" 
                                                LocalizedTag="SIG_BBCODES" LocalizedPage="ADMIN_EDITGROUP" />
                            <asp:Label ID="Label7" runat="server" 
                                       CssClass="<%# this.GetItemColorString(((YAF.Types.Models.Group)Container.DataItem).UsrSigBBCodes) %>">
                                <%# ((YAF.Types.Models.Group)Container.DataItem).UsrSigBBCodes.IsSet() ? this.Eval("UsrSigBBCodes").ToString() : this.GetItemName(false) %>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item"><YAF:LocalizedLabel ID="HelpLabel9" runat="server" 
                                                                         LocalizedTag="SIG_HTML" LocalizedPage="ADMIN_EDITGROUP" />
                            <asp:Label ID="Label8" runat="server" 
                                       CssClass="<%# this.GetItemColorString(((YAF.Types.Models.Group)Container.DataItem).UsrSigHTMLTags) %>">
                                <%#  ((YAF.Types.Models.Group)Container.DataItem).UsrSigHTMLTags.IsSet() ? this.Eval("UsrSigHTMLTags").ToString() : this.GetItemName(false)%>
                            </asp:Label>

                        </li>
                    </ul>
                </p>
                <small>
                    <YAF:ThemeButton ID="ThemeButtonEdit" runat="server" 
                                     Type="Info" 
                                     Size="Small"
                                     CommandName="edit" CommandArgument='<%# this.Eval( "ID") %>'
                                     TitleLocalizedTag="EDIT"
                                     Icon="edit"
                                     TextLocalizedTag="EDIT">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" runat="server" 
                                     Type="Danger" 
                                     Size="Small" 
                                     Visible='<%#!this.Eval( "Flags" ).BinaryAnd(2)%>'
                                     CommandName="delete" CommandArgument='<%# this.Eval( "ID") %>'
                                     ReturnConfirmText='<%# this.GetText("ADMIN_GROUPGS", "CONFIRM_DELETE") %>'
                                     TitleLocalizedTag="DELETE"
                                     Icon="trash"
                                     TextLocalizedTag="DELETE">
                    </YAF:ThemeButton>
                    <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                        <YAF:ThemeButton ID="ThemeButton1" runat="server" 
                                         Type="None"
                                         CssClass="dropdown-item"
                                         CommandName="edit" CommandArgument='<%# this.Eval( "ID") %>'
                                         TitleLocalizedTag="EDIT"
                                         Icon="edit"
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButton2" runat="server" 
                                         Type="None"
                                         CssClass="dropdown-item"
                                         Visible='<%#!this.Eval( "Flags" ).BinaryAnd(2)%>'
                                         CommandName="delete" CommandArgument='<%# this.Eval( "ID") %>'
                                         ReturnConfirmText='<%# this.GetText("ADMIN_GROUPGS", "CONFIRM_DELETE") %>'
                                         TitleLocalizedTag="DELETE"
                                         Icon="trash"
                                         TextLocalizedTag="DELETE">
                        </YAF:ThemeButton>
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="NewGroup" runat="server" 
                                         OnClick="NewGroupClick" 
                                         Type="None"
                                         CssClass="dropdown-item"
                                         Icon="plus-square" 
                                         TextLocalizedTag="NEW_ROLE">
                        </YAF:ThemeButton>
                    </div>
                </small>
            </li>
			</ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
		</asp:Repeater>
                </div>
            <div class="card-footer text-center">
				    <YAF:ThemeButton ID="NewGroup" runat="server" 
                                     OnClick="NewGroupClick" 
                                     Type="Primary"
				                     Icon="plus-square" 
                                     TextLocalizedTag="NEW_ROLE">
                    </YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>