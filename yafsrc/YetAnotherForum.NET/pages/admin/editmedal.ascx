<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.editmedal" Codebehind="editmedal.ascx.cs" %>
<%@ Import Namespace="YAF.Classes.Core"%>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="AdminMenu1" runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="4">
				Add/Edit Medal</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Name:</b><br />
				Name of medal.</td>
			<td class="post" colspan="2">
				<asp:TextBox Style="width: 300px" ID="Name" runat="server" MaxLength="100" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Name" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Description:</b><br />
				Description of medal.</td>
			<td class="post" colspan="2">
				<asp:TextBox Style="width: 300px; height: 100px;" ID="Description" TextMode="MultiLine"
					runat="server" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Description" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Message:</b><br />
				Default message.</td>
			<td class="post" colspan="2">
				<asp:TextBox Style="width: 300px" ID="Message" runat="server" MaxLength="100" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Message" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Category:</b><br />
				Medal's category.</td>
			<td class="post" colspan="2">
				<asp:TextBox Style="width: 300px" ID="Category" MaxLength="50" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Medal Image:</b><br />
				This image will be shown in medal's info.</td>
			<td class="post" colspan="2">
				<asp:DropDownList ID="MedalImage" runat="server" />
				<img style="vertical-align: top;" runat="server" id="MedalPreview" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Ribbon Bar Image:</b><br />
				This image will be shown in medal's info (optional).</td>
			<td class="post" colspan="2">
				<asp:DropDownList ID="RibbonImage" runat="server" />
				<img style="vertical-align: top;" runat="server" id="RibbonPreview" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Small Medal Image:</b><br />
				This image will be shown in user box.</td>
			<td class="post" colspan="2">
				<asp:DropDownList ID="SmallMedalImage" runat="server" />
				<img style="vertical-align: top;" runat="server" id="SmallMedalPreview" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Small Ribbon Bar Image:</b><br />
				This image will be shown in user box (optional).</td>
			<td class="post" colspan="2">
				<asp:DropDownList ID="SmallRibbonImage" runat="server" />
				<img style="vertical-align: top;" runat="server" id="SmallRibbonPreview" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Sort Order:</b><br />
				Default sort order of a medal.</td>
			<td class="post" colspan="2">
				<asp:TextBox Style="width: 50px" ID="SortOrder" MaxLength="5" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Show Message:</b><br />
				Means that message describing why user received medal will be shown/hidden.</td>
			<td class="post" colspan="2">
				<asp:CheckBox ID="ShowMessage" runat="server" Checked="true" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Allow Ribbon Bar:</b><br />
				Means that ribbon bar display of this medal will be allowed/disallowed.</td>
			<td class="post" colspan="2">
				<asp:CheckBox ID="AllowRibbon" runat="server" Checked="true" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Allow Hiding:</b><br />
				Means that users will be allowed/disallowed to hide this medal.</td>
			<td class="post" colspan="2">
				<asp:CheckBox ID="AllowHiding" runat="server" Checked="true" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<b>Allow Re-Ordering:</b><br />
				Means that users will be allowed/disallowed to change order of this medal.</td>
			<td class="post" colspan="2">
				<asp:CheckBox ID="AllowReOrdering" runat="server" Checked="true" /></td>
		</tr>
		<tr>
			<td class="postfooter" align="center" colspan="11">
				<asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click" ValidationGroup="Medal" />&nbsp;
				<asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click" /></td>
		</tr>
		<asp:Repeater ID="GroupList" runat="server" OnItemCommand="GroupList_ItemCommand">
			<HeaderTemplate>
				<tr>
					<td class="header1" colspan="4">
						Group Holders</td>
				</tr>
				<tr>
					<td class="header2">
						Group</td>
					<td class="header2" colspan="2">
						Message</td>
					<td class="header2">
						Command</td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="post">
						<%# FormatGroupLink(Container.DataItem) %>
					</td>
					<td class="post" colspan="2">
						<%# Eval("Message") %>
					</td>
					<td class="post">
						<asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval("GroupID") %>'>Edit</asp:LinkButton>
						|
						<asp:LinkButton runat="server" CommandName="remove" CommandArgument='<%# Eval("GroupID") %>'
							OnLoad="GroupRemove_Load">Remove</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr runat="server" id="AddGroupRow" visible="false">
			<td class="footer1" colspan="4">
				<asp:LinkButton runat="server" OnClick="AddGroup_Click">Add Group</asp:LinkButton>
			</td>
		</tr>
		<asp:Panel runat="server" ID="AddGroupPanel" Visible="false">
			<tr>
				<td class="footer1" colspan="4" style="text-align: center;">
					<b>
						<asp:Label runat="server" ID="GroupMedalEditTitle" /></b>
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<b>Group:</b></td>
				<td class="post" colspan="2">
					<asp:DropDownList runat="server" ID="AvailableGroupList" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<b>Message:</b><br />
					Overrides default if specified (Optional)
				</td>
				<td class="post" colspan="2">
					<asp:TextBox Style="width: 300px" ID="GroupMessage" runat="server" MaxLength="100" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<b>Sort Order:</b><br />
					Overrides default if specified (Optional)
				</td>
				<td class="post" colspan="2">
					<asp:TextBox Style="width: 75px" ID="GroupSortOrder" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<b>Show Only Ribbon Bar:</b><br />
					If checked, only ribbon bar is displayed in user box.
				</td>
				<td class="post" colspan="2">
					<asp:CheckBox runat="server" ID="GroupOnlyRibbon" Checked="false" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<b>Hide:</b><br />
					If checked, medal is not displayed in user box.
				</td>
				<td class="post" colspan="2">
					<asp:CheckBox runat="server" ID="GroupHide" Checked="false" />
				</td>
			</tr>
			<tr>
				<td class="footer1" colspan="4" style="text-align: center;">
					<asp:Button runat="server" Text="Save" OnClick="AddGroupSave_Click" ID="AddGroupSave" />
					<asp:Button runat="server" Text="Cancel" OnClick="AddGroupCancel_Click" />
				</td>
			</tr>
		</asp:Panel>
		<asp:Repeater ID="UserList" runat="server" OnItemCommand="UserList_ItemCommand">
			<HeaderTemplate>
				<tr>
					<td class="header1" colspan="4">
						Individual Holders</td>
				</tr>
				<tr>
					<td class="header2">
						User</td>
					<td class="header2">
						Message</td>
					<td class="header2">
						Date Awarded</td>
					<td class="header2">
						Command</td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="post">
						<%# FormatUserLink(Container.DataItem) %>
					</td>
					<td class="post">
						<%# Eval("Message") %>
					</td>
					<td class="post">
						<%# YafServices.DateTime.FormatDateTimeTopic((DateTime)Eval("DateAwarded")) %>
					</td>
					<td class="post">
						<asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval("UserID") %>'>Edit</asp:LinkButton>
						|
						<asp:LinkButton runat="server" CommandName="remove" CommandArgument='<%# Eval("UserID") %>'
							OnLoad="UserRemove_Load">Remove</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr runat="server" id="AddUserRow" visible="false">
			<td class="footer1" colspan="4">
				<asp:LinkButton runat="server" OnClick="AddUser_Click">Add User</asp:LinkButton>
			</td>
		</tr>
		<asp:Panel runat="server" ID="AddUserPanel" Visible="false">
			<tr>
				<td class="footer1" colspan="4" style="text-align: center;">
					<b>
						<asp:Label runat="server" ID="UserMedalEditTitle" /></b>
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					User:</td>
				<td class="post" colspan="2">
					<asp:TextBox Style="width: 125px" ID="UserName" runat="server" />
					<asp:DropDownList runat="server" ID="UserNameList" Visible="false" />
					<asp:Button runat="server" ID="FindUsers" Text="Find Users" OnClick="FindUsers_Click" />
					<asp:Button runat="server" ID="Clear" Text="Clear" OnClick="Clear_Click" Visible="false" />
					<asp:TextBox Visible="false" ID="UserID" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<b>Message:</b><br />
					Overrides default if specified (Optional)
				</td>
				<td class="post" colspan="2">
					<asp:TextBox Style="width: 300px" ID="UserMessage" runat="server" MaxLength="100" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<b>Sort Order:</b><br />
					Overrides default if specified (Optional)
				</td>
				<td class="post" colspan="2">
					<asp:TextBox Style="width: 75px" ID="UserSortOrder" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<b>Show Only Ribbon Bar:</b><br />
					If checked, only ribbon bar is displayed in user box.
				</td>
				<td class="post" colspan="2">
					<asp:CheckBox runat="server" ID="UserOnlyRibbon" Checked="false" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<b>Hide:</b><br />
					If checked, medal is not displayed in user box.
				</td>
				<td class="post" colspan="2">
					<asp:CheckBox runat="server" ID="UserHide" Checked="false" />
				</td>
			</tr>
			<tr>
				<td class="footer1" colspan="4" style="text-align: center;">
					<asp:Button runat="server" Text="Save" OnClick="AddUserSave_Click" ID="AddUserSave" />
					<asp:Button runat="server" Text="Cancel" OnClick="AddUserCancel_Click" />
				</td>
			</tr>
		</asp:Panel>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
