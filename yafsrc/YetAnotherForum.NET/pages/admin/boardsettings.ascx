<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.boardsettings" Codebehind="boardsettings.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BOARDSETTINGS" />
             </td>
		</tr>
		<tr>
			<td class="header2" colspan="2">
				<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="BOARD_SETUP" LocalizedPage="ADMIN_BOARDSETTINGS" />
             </td>
		</tr>
		<tr>
			<td class="postheader" style="width:50%">
                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="BOARD_NAME" LocalizedPage="ADMIN_BOARDSETTINGS" />
				
            </td>
			<td class="post" style="width:50%">
				<asp:TextBox ID="Name" runat="server" Width="400"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="BOARD_THREADED" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:CheckBox ID="AllowThreaded" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="BOARD_THEME" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:DropDownList ID="Theme" runat="server" Width="400">
				</asp:DropDownList></td>
		</tr>
		<tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="BOARD_MOBILE_THEME" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:DropDownList ID="MobileTheme" runat="server" Width="400">
                    <asp:ListItem Text="[None Selected]" Value=""></asp:ListItem>
				</asp:DropDownList></td>
		</tr>
		<tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="BOARD_THEME_LOGO" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:CheckBox ID="AllowThemedLogo" runat="server"></asp:CheckBox></td>
		</tr>
        <tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="BOARD_JQ_THEME" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:DropDownList ID="JqueryUITheme" runat="server" Width="400">
				</asp:DropDownList></td>
		</tr>
        <tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="BOARD_CDN_HOSTED" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:CheckBox ID="JqueryUIThemeCDNHosted" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="BOARD_CULTURE" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:DropDownList ID="Culture" runat="server" Width="400">
				</asp:DropDownList></td>
		</tr>        	
		<tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="BOARD_TOPIC_DEFAULT" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:DropDownList ID="ShowTopic" runat="server" Width="400">
				</asp:DropDownList></td>
		</tr>		
		<tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="BOARD_FILE_EXTENSIONS" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:DropDownList ID="FileExtensionAllow" runat="server" Width="400"></asp:DropDownList></td>
		</tr>	
       <tr id="PollGroupList" runat="server" visible="false">
		<td class="postformheader" style="width:20%">
			<em>
				<YAF:LocalizedLabel ID="PollGroupListLabel" runat="server" LocalizedTag="pollgroup_list" />
			</em>
		</td>
		<td class="post" style="width:80%">
			<asp:DropDownList ID="PollGroupListDropDown" runat="server" CssClass="edit" Width="400" />			
		</td>
	</tr> 
		<tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="BOARD_EMAIL_ONREGISTER" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:TextBox ID="NotificationOnUserRegisterEmailList" runat="server" Width="400"></asp:TextBox></td>
		</tr>			
		<tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="BOARD_EMAIL_MODS" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:CheckBox ID="EmailModeratorsOnModeratedPost" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="BOARD_ALLOW_DIGEST" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:CheckBox ID="AllowDigestEmail" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="BOARD_DIGEST_NEWUSERS" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:CheckBox ID="DefaultSendDigestEmail" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				 <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="BOARD_DEFAULT_NOTIFICATION" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
			<td class="post">
				<asp:DropDownList ID="DefaultNotificationSetting" runat="server" Width="400">
				</asp:DropDownList></td>
		</tr>
		<tr>
			<td class="postfooter" align="center" colspan="2">
				<asp:Button ID="Save" CssClass="pbutton" runat="server" Text="Save" OnClick="Save_Click"></asp:Button></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
