<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.Groups" Codebehind="Groups.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Types.Flags" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row" runat="server" Visible="<%# this.RoleListNet.Items.Count > 0 %>">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="users"
                                    LocalizedTag="PROVIDER_ROLES"
                                    LocalizedPage="ADMIN_GROUPS"></YAF:IconHeader>
                </div>
                <div class="card-body">
                    <YAF:Alert runat="server" Type="danger">
                        <YAF:Icon runat="server" IconName="info-circle" />
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
                     <small class="text-body-secondary">
                         <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedPage="ADMIN_GROUPS" LocalizedTag="UNLINKED" />
                     </small>
                 </div>
                     <small>
                         <div class="btn-group btn-group-sm">
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
                                              ReturnConfirmTag="CONFIRM_DELETE">
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

            </div>
        </div>
    </div>

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="users"
                                    LocalizedTag="HEADER"
                                    LocalizedPage="ADMIN_GROUPS"></YAF:IconHeader>
                </div>
                <div class="card-body">
                    <YAF:Alert runat="server" Type="danger">
                        <YAF:Icon runat="server" IconName="info-circle" />
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
                    <small class="text-body-secondary">
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
                                       CssClass='<%# this.GetItemColor(((GroupFlags)this.Eval("GroupFlags")).IsGuest) %>'>
                                <%# this.GetItemName(((GroupFlags)this.Eval("GroupFlags")).IsGuest) %>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server"
                                                LocalizedTag="IS_START" LocalizedPage="ADMIN_GROUPS" />:&nbsp;
                            <asp:Label ID="Label1" runat="server"
                                       CssClass='<%# this.GetItemColor(((GroupFlags)this.Eval("GroupFlags")).IsStart) %>'>
                                <%# this.GetItemName(((GroupFlags)this.Eval("GroupFlags")).IsStart) %>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server"
                                                LocalizedTag="IS_MOD" LocalizedPage="ADMIN_GROUPS" />:&nbsp;
                            <asp:Label ID="Label3" runat="server"
                                       CssClass='<%# this.GetItemColor(((GroupFlags)this.Eval("GroupFlags")).IsModerator) %>'>
                                <%# this.GetItemName(((GroupFlags)this.Eval("GroupFlags")).IsModerator) %>
                            </asp:Label>
                        </li>
                        <li class="list-inline-item">
                            <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server"
                                                LocalizedTag="IS_ADMIN" LocalizedPage="ADMIN_GROUPS" />:&nbsp;
                            <asp:Label ID="Label4" runat="server"
                                       CssClass='<%# this.GetItemColor(((GroupFlags)this.Eval("GroupFlags")).IsAdmin) %>'>
                                <%# this.GetItemName(((GroupFlags)this.Eval("GroupFlags")).IsAdmin) %>
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
                    </ul>
                </p>
                <small>
                    <div class="btn-group btn-group-sm">
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
                                         Visible='<%#!((GroupFlags)this.Eval("GroupFlags")).IsGuest%>'
                                         CommandName="delete" CommandArgument='<%# this.Eval( "ID") %>'
                                         ReturnConfirmTag="CONFIRM_DELETE"
                                         TitleLocalizedTag="DELETE"
                                         Icon="trash"
                                         TextLocalizedTag="DELETE">
                        </YAF:ThemeButton>
                    </div>
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
                                         Visible='<%#!((GroupFlags)this.Eval("GroupFlags")).IsGuest && !YAF.Configuration.Config.IsDotNetNuke %>'
                                         CommandName="delete" CommandArgument='<%# this.Eval( "ID") %>'
                                         ReturnConfirmTag="CONFIRM_DELETE"
                                         TitleLocalizedTag="DELETE"
                                         Icon="trash"
                                         TextLocalizedTag="DELETE">
                        </YAF:ThemeButton>
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="NewGroup" runat="server"
                                         Visible="<%# !YAF.Configuration.Config.IsDotNetNuke %>"
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
                                     Visible="<%# !YAF.Configuration.Config.IsDotNetNuke %>"
                                     Type="Primary"
                                     Icon="plus-square"
                                     TextLocalizedTag="NEW_ROLE">
                    </YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>