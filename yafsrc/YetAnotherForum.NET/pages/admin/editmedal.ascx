<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.editmedal" Codebehind="editmedal.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="AdminMenu1" runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="4">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITMEDAL" />
             </td>
		</tr>
        <tr>
	      <td class="header2" colspan="4" style="height:30px"></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="MEDAL_NAME" LocalizedPage="ADMIN_EDITMEDAL" />
            </td>
			<td class="post" colspan="2">
				<asp:TextBox Style="width: 250px" ID="Name" runat="server" MaxLength="100" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Name" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="MEDAL_DESC" LocalizedPage="ADMIN_EDITMEDAL" />
		    </td>
			<td class="post" colspan="2">
				<asp:TextBox Style="width: 250px;height: 100px;" ID="Description" TextMode="MultiLine"
					runat="server" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Description" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="MEDAL_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
            </td>
			<td class="post" colspan="2">
				<asp:TextBox Style="width: 250px" ID="Message" runat="server" MaxLength="100" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Message" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="MEDAL_CATEGORY" LocalizedPage="ADMIN_EDITMEDAL" />
				</td>
			<td class="post" colspan="2">
				<asp:TextBox Style="width: 250px" ID="Category" MaxLength="50" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="MEDAL_IMAGE" LocalizedPage="ADMIN_EDITMEDAL" />
            </td>
			<td class="post" colspan="2">
				<asp:DropDownList Style="width: 250px" ID="MedalImage" runat="server" />
				<img style="vertical-align: top;" runat="server" id="MedalPreview" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="RIBBON_IMAGE" LocalizedPage="ADMIN_EDITMEDAL" />
            </td>
			<td class="post" colspan="2">
				<asp:DropDownList Style="width: 250px" ID="RibbonImage" runat="server" />
				<img style="vertical-align: top;" runat="server" id="RibbonPreview" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="SMALL_IMAGE" LocalizedPage="ADMIN_EDITMEDAL" />
            </td>
			<td class="post" colspan="2">
				<asp:DropDownList Style="width: 250px" ID="SmallMedalImage" runat="server" />
				<img style="vertical-align: top;" runat="server" id="SmallMedalPreview" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="SMALL_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
            </td>
			<td class="post" colspan="2">
				<asp:DropDownList Style="width: 250px" ID="SmallRibbonImage" runat="server" />
				<img style="vertical-align: top;" runat="server" id="SmallRibbonPreview" />
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_EDITMEDAL" />
            </td>
			<td class="post" colspan="2">
				<asp:TextBox Style="width: 250px" ID="SortOrder" MaxLength="5" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="SHOW_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
            </td>
			<td class="post" colspan="2">
				<asp:CheckBox ID="ShowMessage" runat="server" Checked="true" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="ALLOW_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
            </td>
			<td class="post" colspan="2">
				<asp:CheckBox ID="AllowRibbon" runat="server" Checked="true" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="ALLOW_HIDING" LocalizedPage="ADMIN_EDITMEDAL" />
            </td>
			<td class="post" colspan="2">
				<asp:CheckBox ID="AllowHiding" runat="server" Checked="true" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2">
				<YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="ALLOW_REORDER" LocalizedPage="ADMIN_EDITMEDAL" />
            </td>
			<td class="post" colspan="2">
				<asp:CheckBox ID="AllowReOrdering" runat="server" Checked="true" /></td>
		</tr>
		<tr>
			<td class="footer1" align="center" colspan="11">
				<asp:Button ID="Save" runat="server" OnClick="Save_Click" ValidationGroup="Medal" CssClass="pbutton" />&nbsp;
				<asp:Button ID="Cancel" runat="server" OnClick="Cancel_Click" CssClass="pbutton" /></td>
		</tr>
        </table>

        <table class="content" cellspacing="1" cellpadding="0" width="100%">
		<asp:Repeater ID="GroupList" runat="server" OnItemCommand="GroupList_ItemCommand">
			<HeaderTemplate>
				<tr>
					<td class="header1" colspan="4">
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_EDITMEDAL" />
                    </td>
				</tr>
				<tr>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="GROUP" /></td>
					<td class="header2" colspan="2">
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="MESSAGE" LocalizedPage="COMMON" /></td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="COMMAND" /></td>
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
						<asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval("GroupID") %>'><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="EDIT" /></asp:LinkButton>
						|
						<asp:LinkButton runat="server" CommandName="remove" CommandArgument='<%# Eval("GroupID") %>'
							OnLoad="GroupRemove_Load"><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="REMOVE" /></asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr runat="server" id="AddGroupRow" visible="false" style="text-align:center">
			<td class="footer1" colspan="4">
				<asp:Button runat="server" OnClick="AddGroup_Click" ID="AddGroup" CssClass="pbutton"></asp:Button>
			</td>
		</tr>
		<asp:Panel runat="server" ID="AddGroupPanel" Visible="false">
			<tr>
				<td class="header2" colspan="4" style="text-align: center;">
					<strong>
						<asp:Label runat="server" ID="GroupMedalEditTitle" /></strong>
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="MEDAL_GROUP" LocalizedPage="ADMIN_EDITMEDAL" />
                </td>
				<td class="post" colspan="2">
					<asp:DropDownList style="width: 250px" runat="server" ID="AvailableGroupList" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<YAF:HelpLabel ID="HelpLabel15" runat="server" LocalizedTag="OVERRIDE_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
                </td>
				<td class="post" colspan="2">
					<asp:TextBox Style="width: 250px" ID="GroupMessage" runat="server" MaxLength="100" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<YAF:HelpLabel ID="HelpLabel16" runat="server" LocalizedTag="OVERRIDE_ORDER" LocalizedPage="ADMIN_EDITMEDAL" />
                </td>
				<td class="post" colspan="2">
					<asp:TextBox Style="width: 250px" ID="GroupSortOrder" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<YAF:HelpLabel ID="HelpLabel17" runat="server" LocalizedTag="ONLY_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
                </td>
				<td class="post" colspan="2">
					<asp:CheckBox runat="server" ID="GroupOnlyRibbon" Checked="false" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
                    <YAF:HelpLabel ID="HelpLabel18" runat="server" LocalizedTag="HIDE" LocalizedPage="ADMIN_EDITMEDAL" />
				</td>
				<td class="post" colspan="2">
					<asp:CheckBox runat="server" ID="GroupHide" Checked="false" />
				</td>
			</tr>
			<tr>
				<td class="footer1" colspan="4" style="text-align: center;">
					<asp:Button runat="server"  OnClick="AddGroupSave_Click" ID="AddGroupSave" CssClass="pbutton" />
					<asp:Button runat="server"  OnClick="AddGroupCancel_Click" ID="AddGroupCancel" CssClass="pbutton" />
				</td>
			</tr>
		</asp:Panel>
        </table>

        <table class="content" cellspacing="1" cellpadding="0" width="100%">
		<asp:Repeater ID="UserList" runat="server" OnItemCommand="UserList_ItemCommand">
			<HeaderTemplate>
				<tr>
					<td class="header1" colspan="4">
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_EDITMEDAL" />
                    </td>
				</tr>
				<tr>
					<td class="header2">
					    <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="USERNAME" LocalizedPage="ACTIVEUSERS" />
						(<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="USER" LocalizedPage="MODERATE" />)
                    </td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="MESSAGE" LocalizedPage="COMMON" /></td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="DATE_AWARDED" LocalizedPage="ADMIN_EDITMEDAL" /></td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="COMMAND" /></td>
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
						<%# this.Get<IDateTime>().FormatDateTimeTopic((DateTime)Eval("DateAwarded")) %>
					</td>
					<td class="post">
						<asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval("UserID") %>'><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="EDIT" /></asp:LinkButton>
						|
						<asp:LinkButton runat="server" CommandName="remove" CommandArgument='<%# Eval("UserID") %>'
							OnLoad="UserRemove_Load"><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="REMOVE" /></asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr runat="server" id="AddUserRow" visible="false">
			<td class="footer1" colspan="4" style="text-align:center">
				<asp:Button runat="server" OnClick="AddUser_Click" ID="AddUser" CssClass="pbutton"></asp:Button>
			</td>
		</tr>
		<asp:Panel runat="server" ID="AddUserPanel" Visible="false">
			<tr>
				<td class="header2" colspan="4" style="text-align: center;">
					<strong>
						<asp:Label runat="server" ID="UserMedalEditTitle" /></strong>
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<YAF:HelpLabel ID="HelpLabel23" runat="server" LocalizedTag="MEDAL_USER" LocalizedPage="ADMIN_EDITMEDAL" />
                </td>
				<td class="post" colspan="2">
					<asp:TextBox Style="width: 250px" ID="UserName" runat="server" />
					<asp:DropDownList  Style="width: 250px" runat="server" ID="UserNameList" Visible="false" />
					<asp:Button runat="server" ID="FindUsers" Text="Find Users" OnClick="FindUsers_Click" CssClass="pbutton" />
					<asp:Button runat="server" ID="Clear" Text="Clear" OnClick="Clear_Click" Visible="false" CssClass="pbutton" />
					<asp:TextBox Visible="false" ID="UserID" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<YAF:HelpLabel ID="HelpLabel19" runat="server" LocalizedTag="OVERRIDE_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
				</td>
				<td class="post" colspan="2">
					<asp:TextBox Style="width: 250px" ID="UserMessage" runat="server" MaxLength="100" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<YAF:HelpLabel ID="HelpLabel20" runat="server" LocalizedTag="OVERRIDE_ORDER" LocalizedPage="ADMIN_EDITMEDAL" />
				</td>
				<td class="post" colspan="2">
					<asp:TextBox Style="width: 250px" ID="UserSortOrder" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<YAF:HelpLabel ID="HelpLabel21" runat="server" LocalizedTag="ONLY_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
				</td>
				<td class="post" colspan="2">
					<asp:CheckBox runat="server" ID="UserOnlyRibbon" Checked="false" />
				</td>
			</tr>
			<tr>
				<td class="postheader" colspan="2">
					<YAF:HelpLabel ID="HelpLabel22" runat="server" LocalizedTag="HIDE" LocalizedPage="ADMIN_EDITMEDAL" />
				</td>
				<td class="post" colspan="2">
					<asp:CheckBox runat="server" ID="UserHide" Checked="false" />
				</td>
			</tr>
			<tr>
				<td class="footer1" colspan="4" style="text-align: center;">
					<asp:Button runat="server" OnClick="AddUserSave_Click" ID="AddUserSave" CssClass="pbutton" />
					<asp:Button runat="server" OnClick="AddUserCancel_Click" ID="AddUserCancel" CssClass="pbutton" />
				</td>
			</tr>
		</asp:Panel>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
