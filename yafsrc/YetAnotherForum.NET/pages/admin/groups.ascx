<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.groups" Codebehind="groups.ascx.cs" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_GROUPS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-users fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="PROVIDER_ROLES" LocalizedPage="ADMIN_GROUPS" />
                </div>
                <div class="card-body">
                    <div class="alert alert-danger" role="alert">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="NOTE_DELETE" LocalizedPage="ADMIN_GROUPS" />
                    </div>
		<asp:Repeater ID="RoleListNet" runat="server" OnItemCommand="RoleListNetItemCommand">
			<HeaderTemplate>
			    <div class="table-responsive">
	<table class="table">
				<tr>
                    <thead>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_COMMON" />
					</th>
					<th>&nbsp;

					</th>
                    </thead>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td>
						<%# Container.DataItem %>
						(<YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedPage="ADMIN_GROUPS" LocalizedTag="UNLINKED" />)
					</td>
					<td>
					    <span class="float-right">
						<YAF:ThemeButton ID="ThemeButtonAdd" Type="Info" CssClass="btn-sm"
                            CommandName='add' CommandArgument='<%# Container.DataItem %>'
                            TitleLocalizedTag="ADD_ROLETOYAF"
                            TitleLocalizedPage="ADMIN_GROUPS"
                            TextLocalizedTag="ADD_ROLETOYAF"
                            TextLocalizedPage="ADMIN_GROUPS"
                            Icon="plus-circle"
                            runat="server">
					    </YAF:ThemeButton>
						<YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm"
                                    CommandName='delete' CommandArgument='<%# Container.DataItem %>'
                                    TitleLocalizedTag="DELETE"
                                    Icon="trash"
                                    TextLocalizedTag="DELETE"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_GROUPS", "CONFIRM_DELETE") %>'
						            runat="server">
                                </YAF:ThemeButton>
                            </span>
					</td>
				</tr>
			</ItemTemplate>
            <FooterTemplate>
                </table></div>

            </FooterTemplate>
		</asp:Repeater>
                </div></div>
                        <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-users fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_GROUPS" />
                </div>
                <div class="card-body">
                    <div class="alert alert-danger" role="alert">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NOTE_DELETE_LINKED" LocalizedPage="ADMIN_GROUPS" />
                    </div>
		<asp:Repeater ID="RoleListYaf" runat="server" OnItemCommand="RoleListYafItemCommand">
			<HeaderTemplate>
                <div class="table-responsive">
	<table class="table">
				<tr>
				<tr>
                    <thead>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_GROUPS" />
					</th>
					<th>&nbsp;

					</th>
                    </thead>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td>
                       <i class="fa fa-users fa-fw"></i>&nbsp;
						<%# this.Eval( "Name" ) %>
						(<%# this.GetLinkedStatus( (DataRowView) Container.DataItem )%>)&nbsp;&nbsp;
					</td>
                    <td>
                        <span class="float-right">
						<YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" CssClass="btn-sm"
                            CommandName='edit' CommandArgument='<%# this.Eval( "GroupID") %>'
                            TitleLocalizedTag="EDIT"
                            Icon="edit"
                            TextLocalizedTag="EDIT"
                            runat="server">
					    </YAF:ThemeButton>
						<YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm" Visible='<%#(!this.Eval( "Flags" ).BinaryAnd(2))%>'
                                    CommandName='delete' CommandArgument='<%# this.Eval( "GroupID") %>'
                                    TitleLocalizedTag="DELETE"
                                    Icon="trash"
                                    TextLocalizedTag="DELETE" runat="server">
                                </YAF:ThemeButton>
                            </span>
					</td>
                     </tr>
                    <tr>
					<td colspan="2">
                     <YAF:LocalizedLabel ID="HelpLabel6" Visible='<%# this.Eval("Description").ToString().IsSet() %>' runat="server" LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_EDITGROUP">
                         </YAF:LocalizedLabel>
                          &nbsp;<%# this.Eval("Description").ToString() %>&nbsp;
                    <br />
                    <YAF:LocalizedLabel  ID="HelpLabel12" runat="server" LocalizedTag="PRIORITY" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label11" runat="server" CssClass='<%# this.GetItemColorString(this.Eval( "SortOrder" ).ToString()) %>'><%# this.Eval("SortOrder").ToString()%></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="IS_GUEST" LocalizedPage="ADMIN_GROUPS" />&nbsp;
                    <asp:Label ID="Label2" runat="server" CssClass='<%# this.GetItemColor(this.Eval( "Flags" ).BinaryAnd(2)) %>'><%# this.GetItemName(this.Eval( "Flags" ).BinaryAnd(2)) %></asp:Label>&nbsp;|&nbsp;
				    <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="IS_START" LocalizedPage="ADMIN_GROUPS" />&nbsp;
                    <asp:Label ID="Label1" runat="server" CssClass='<%# this.GetItemColor(this.Eval( "Flags" ).BinaryAnd(4)) %>'><%# this.GetItemName(this.Eval( "Flags" ).BinaryAnd(4)) %></asp:Label>&nbsp;|&nbsp;
				    <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="IS_MOD" LocalizedPage="ADMIN_GROUPS" />&nbsp;
					<asp:Label ID="Label3" runat="server" CssClass='<%# this.GetItemColor(this.Eval( "Flags" ).BinaryAnd(8)) %>'><%# this.GetItemName(this.Eval( "Flags" ).BinaryAnd(8)) %></asp:Label>&nbsp;|&nbsp;
					<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="IS_ADMIN" LocalizedPage="ADMIN_GROUPS" />&nbsp;
					<asp:Label ID="Label4" runat="server" CssClass='<%# this.GetItemColor(this.Eval( "Flags" ).BinaryAnd(1)) %>'><%# this.GetItemName(this.Eval( "Flags" ).BinaryAnd(1)) %></asp:Label>&nbsp;|&nbsp;
					<YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="PMS" LocalizedPage="ADMIN_GROUPS" />&nbsp;
					 <asp:Label ID="Label6" runat="server" CssClass='<%# this.GetItemColorString((Convert.ToInt32(this.Eval("PMLimit")) == int.MaxValue) ? "\u221E" : this.Eval("PMLimit").ToString()) %>'><%# ((Convert.ToInt32(Eval("PMLimit")) == int.MaxValue) ? "\u221E" : this.Eval("PMLimit").ToString())%></asp:Label>&nbsp;|&nbsp;
                    <br />
                    <YAF:LocalizedLabel  ID="HelpLabel10" runat="server" LocalizedTag="ALBUM_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label9" runat="server" CssClass='<%# this.GetItemColorString(this.Eval( "UsrAlbums" ).ToString()) %>'><%# this.Eval("UsrAlbums").ToString()%></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel  ID="HelpLabel11" runat="server" LocalizedTag="IMAGES_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label10" runat="server" CssClass='<%# this.GetItemColorString(this.Eval( "UsrAlbumImages" ).ToString()) %>'><%# this.Eval("UsrAlbumImages").ToString()%></asp:Label>&nbsp;|&nbsp;
                    <br />
                    <YAF:LocalizedLabel  ID="HelpLabel13" runat="server" LocalizedTag="STYLE" LocalizedPage="ADMIN_EDITGROUP" />&nbsp;
                    <asp:Label ID="Label12" runat="server" CssClass='<%# this.GetItemColorString(this.Eval( "Style" ).ToString()) %>'><%# this.Eval("Style").ToString().IsSet() && (this.Eval("Style").ToString().Trim().Length > 0) ? "" : this.GetItemName(false)%></asp:Label>&nbsp;
                    <YAF:RoleRankStyles ID="RoleRankStylesGroups" RawStyles='<%# this.Eval( "Style" ).ToString() %>' runat="server" />
                     <br />
                    <YAF:LocalizedLabel ID="HelpLabel7" runat="server" LocalizedTag="SIGNATURE_LENGTH" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label5" runat="server" CssClass='<%# this.GetItemColorString(this.Eval( "UsrSigChars" ).ToString()) %>'><%# this.Eval("UsrSigChars").ToString().IsSet() ? this.Eval("UsrSigChars").ToString() : this.GetItemName(false) %></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel ID="HelpLabel8" runat="server" LocalizedTag="SIG_BBCODES" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label7" runat="server" CssClass='<%# this.GetItemColorString(this.Eval( "UsrSigBBCodes" ).ToString()) %>'><%# this.Eval("UsrSigBBCodes").ToString().IsSet() ? this.Eval("UsrSigBBCodes").ToString() : this.GetItemName(false) %></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel ID="HelpLabel9" runat="server"  LocalizedTag="SIG_HTML" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label8" runat="server" CssClass='<%# this.GetItemColorString(this.Eval( "UsrSigHTMLTags" ).ToString()) %>'><%#  this.Eval("UsrSigHTMLTags").ToString().IsSet() ? this.Eval("UsrSigHTMLTags").ToString() : this.GetItemName(false)%></asp:Label>&nbsp;|&nbsp;
                    </td>
                    </tr>
				</tr>
			</ItemTemplate>
            <FooterTemplate>
                </table></div>
                </div>
            </FooterTemplate>
		</asp:Repeater>
                     <div class="card-footer text-lg-center">
				    <YAF:ThemeButton ID="NewGroup" runat="server" OnClick="NewGroupClick" Type="Primary"
				                     Icon="plus-square" TextLocalizedTag="NEW_ROLE"></YAF:ThemeButton>
                </div>
            </div>
        </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
