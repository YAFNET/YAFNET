<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.groups" Codebehind="groups.ascx.cs" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="YAF.Core" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="AdminMenu">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<asp:Repeater ID="RoleListNet" runat="server" OnItemCommand="RoleListNet_ItemCommand">
			<HeaderTemplate>
				<tr>
					<td class="header1" colspan="7">
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="PROVIDER_ROLES" LocalizedPage="ADMIN_GROUPS" />
					</td>
				</tr>
				<tr>
					<td colspan="7" class="header2" style="text-align:center">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="NOTE_DELETE" LocalizedPage="ADMIN_GROUPS" />
					</td>
				</tr>
				<tr>
					<td class="header2" colspan="6">
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NAME" LocalizedPage="COMMON" />
					</td>
					<td class="header2">
						&nbsp;
					</td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="post" colspan="6">
						<%# Container.DataItem %>
						(Unlinked)
					</td>
					<td class="post">
						<asp:LinkButton ID="LinkButtonAdd" runat="server" CommandName="add" CommandArgument='<%# Container.DataItem %>'>Add to YAF</asp:LinkButton>
						|
						<asp:LinkButton ID="LinkButtonDelete" runat="server" OnLoad="Delete_Load" CommandName="delete"
							CommandArgument='<%# Container.DataItem %>'>Delete Role</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<asp:Repeater ID="RoleListYaf" runat="server" OnItemCommand="RoleListYaf_ItemCommand">
			<HeaderTemplate>
				<tr>
					<td class="header1" colspan="7">
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_GROUPS" />
					</td>
				</tr>
				<tr>
					<td colspan="7" class="header2" style="text-align:center">
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="NOTE_DELETE_LINKED" LocalizedPage="ADMIN_GROUPS" />
					</td>
				</tr>
				<tr>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="NAME" LocalizedPage="COMMON" />
					</td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="IS_GUEST" LocalizedPage="ADMIN_GROUPS" />
					</td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="IS_START" LocalizedPage="ADMIN_GROUPS" />
					</td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="IS_MOD" LocalizedPage="ADMIN_GROUPS" />
					</td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="IS_ADMIN" LocalizedPage="ADMIN_GROUPS" />
					</td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="PMS" LocalizedPage="ADMIN_GROUPS" />
					</td>
					<td class="header2">
						&nbsp;
					</td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="post">
						<%# Eval( "Name" ) %>
						(<%# GetLinkedStatus( (DataRowView) Container.DataItem )%>)
					</td>
					<td class="post">
                      <asp:Label ID="Label2" runat="server" ForeColor='<%# GetItemColor(this.Eval( "Flags" ).BinaryAnd(2)) %>'><%# GetItemName(this.Eval( "Flags" ).BinaryAnd(2)) %></asp:Label>
					</td>
					<td class="post">
                     <asp:Label ID="Label1" runat="server" ForeColor='<%# GetItemColor(this.Eval( "Flags" ).BinaryAnd(4)) %>'><%# GetItemName(this.Eval( "Flags" ).BinaryAnd(4)) %></asp:Label>
					</td>
					<td class="post">
						 <asp:Label ID="Label3" runat="server" ForeColor='<%# GetItemColor(this.Eval( "Flags" ).BinaryAnd(8)) %>'><%# GetItemName(this.Eval( "Flags" ).BinaryAnd(8)) %></asp:Label>
					</td>
					<td class="post">
						 <asp:Label ID="Label4" runat="server" ForeColor='<%# GetItemColor(this.Eval( "Flags" ).BinaryAnd(1)) %>'><%# GetItemName(this.Eval( "Flags" ).BinaryAnd(1)) %></asp:Label>
					</td>
					<td class="post">
						<%# ((Convert.ToInt32(Eval("Flags")) & 1) == 1 ? "\u221E".ToString() : Eval("PMLimit").ToString())%>
					</td>
					<td class="post">
						<asp:LinkButton ID="LinkButtonEdit" runat="server" Visible='<%#(this.Eval( "Flags" ).BinaryAnd(2) == true ? false : true)%>'
							CommandName="edit" CommandArgument='<%# Eval( "GroupID") %>'>
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="EDIT" />
                         </asp:LinkButton>
						|
						<asp:LinkButton ID="LinkButtonDelete" runat="server" OnLoad="Delete_Load" Visible='<%#(this.Eval( "Flags" ).BinaryAnd(2) == true ? false : true)%>'
							CommandName="delete" CommandArgument='<%# Eval( "GroupID") %>'>
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DELETE" />
                        </asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td class="footer1" colspan="7" align="center">
				<asp:Button ID="NewGroup" runat="server" OnClick="NewGroup_Click" CssClass="pbutton"></asp:Button>
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
