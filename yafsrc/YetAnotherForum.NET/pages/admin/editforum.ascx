<%@ Control language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editforum" Codebehind="editforum.ascx.cs" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_EDITFORUM" />
				<asp:label id="ForumNameTitle" runat="server"></asp:label></td>
		</tr>
        <tr>
	      <td class="header2" height="30" colspan="2"></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="CATEGORY" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
			<td class="post">
				<asp:dropdownlist Width="250" id="CategoryList" runat="server" OnSelectedIndexChanged="Category_Change" DataValueField="CategoryID" DataTextField="Name"></asp:dropdownlist></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="PARENT_FORUM" LocalizedPage="ADMIN_EDITFORUM" />
              <strong></strong><br />
				</td>
			<td class="post">
				<asp:dropdownlist Width="250" id="ParentList" runat="server"></asp:dropdownlist></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
			<td class="post">
				<asp:textbox Width="250" id="Name" runat="server" cssclass="edit"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
			<td class="post">
				<asp:textbox Width="250" id="Description" runat="server" cssclass="edit"></asp:textbox></td>
		</tr>			
		<tr>
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="REMOTE_URL" LocalizedPage="ADMIN_EDITFORUM" />
              <strong></strong><br />
				</td>
			<td class="post">
				<asp:textbox Width="250" id="remoteurl" runat="server" cssclass="edit"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="THEME" LocalizedPage="ADMIN_EDITFORUM" />
              </td>
			<td class="post">
				<asp:Dropdownlist Width="250" id="ThemeList" runat="server"></asp:Dropdownlist></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_EDITFORUM" />
              <strong></strong><br />
				</td>
			<td class="post">
				<asp:textbox id="SortOrder"  Width="250" MaxLength="5" runat="server"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="HIDE_NOACESS" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
			<td class="post">
				<asp:checkbox id="HideNoAccess" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="LOCKED" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
			<td class="post">
				<asp:checkbox id="Locked" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="NO_POSTSCOUNT" LocalizedPage="ADMIN_EDITFORUM" />
              </td>
			<td class="post">
				<asp:checkbox id="IsTest" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="PRE_MODERATED" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
			<td class="post">
				<asp:checkbox id="Moderated" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader">
			  <YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="FORUM_IMAGE" LocalizedPage="ADMIN_EDITFORUM" />
			</td>			
			<td class="post">
			  <asp:DropDownList Width="250" ID="ForumImages" runat="server" />
			  <img align="middle" runat="server" id="Preview" />
			</td>
		</tr>		
		<tr visible="false" runat="server">
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="STYLES" LocalizedPage="ADMIN_EDITFORUM" />
            <strong></strong><br />
				</td>
			<td class="post">
				<asp:textbox Width="250" id="Styles" runat="server"></asp:textbox></td>
		</tr>		
		<tr id="NewGroupRow" runat="server">
			<td class="postheader">
              <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="INITAL_MASK" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
			<td class="post">
				<asp:dropdownlist Width="250" id="AccessMaskID" ondatabinding="BindData_AccessMaskID" runat="server"></asp:dropdownlist></td>
		</tr>
		<asp:repeater id="AccessList" runat="server">
			<HeaderTemplate>
				<tr>
					<td class="header1" colspan="2">
                      <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_EDITFORUM" />
                    </td>
				</tr>
				<tr class="header2">
					<td><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="Group" LocalizedPage="ADMIN_EDITFORUM" /></td>
					<td><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ACCESS_MASK" LocalizedPage="ADMIN_EDITFORUM" /></td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="postheader">
						<asp:label id="GroupID" visible="false" runat="server" text='<%# Eval( "GroupID") %>'>
						</asp:label>
						<%# Eval( "GroupName") %>
					</td>
					<td class="post">
						<asp:dropdownlist Width="250" runat="server" id="AccessMaskID" ondatabinding="BindData_AccessMaskID" onprerender="SetDropDownIndex" value='<%# Eval("AccessMaskID") %>'/>
						...
					</td>
				</tr>
			</ItemTemplate>
		</asp:repeater>
		<tr>
			<td class="postfooter" align="center" colspan="2">
				<asp:button id="Save" runat="server" CssClass="pbutton"></asp:button>&nbsp;
				<asp:Button id="Cancel" runat="server" CssClass="pbutton"></asp:Button></td>
		</tr>
	</table>
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />
