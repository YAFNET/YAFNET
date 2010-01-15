<%@ Control Language="c#" CodeFile="profile.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.profile" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<%@ Register TagPrefix="YAF" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>
<%@ Register TagPrefix="YAF" TagName="SuspendUser" Src="../controls/EditUsersSuspend.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumAccess" Src="../controls/ForumProfileAccess.ascx" %>
<%@ Register TagPrefix="YAF" TagName="BuddyList" Src="../controls/BuddyList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="AlbumList" Src="../controls/AlbumList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2">
			<YAF:LocalizedLabel runat="server" LocalizedTag="profile" />
			<asp:Label ID="UserName" runat="server" />
		</td>
	</tr>
	<tr class="post">
		<td colspan="2">
			<YAF:ThemeButton ID="PM" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
				TextLocalizedTag="PM" ImageThemeTag="PM" />
			<YAF:ThemeButton ID="Email" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
				TextLocalizedTag="EMAIL" ImageThemeTag="EMAIL" />
			<YAF:ThemeButton ID="Home" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
				TextLocalizedTag="HOME" ImageThemeTag="HOME" />
			<YAF:ThemeButton ID="Blog" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
				TextLocalizedTag="BLOG" ImageThemeTag="BLOG" />
			<YAF:ThemeButton ID="MSN" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
				TextLocalizedTag="MSN" ImageThemeTag="MSN" />
			<YAF:ThemeButton ID="AIM" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
				TextLocalizedTag="AIM" ImageThemeTag="AIM" />
			<YAF:ThemeButton ID="YIM" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
				TextLocalizedTag="YIM" ImageThemeTag="YIM" />
			<YAF:ThemeButton ID="ICQ" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
				TextLocalizedTag="ICQ" ImageThemeTag="ICQ" />
			<YAF:ThemeButton ID="XMPP" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
				TextLocalizedTag="XMPP" ImageThemeTag="XMPP" />	
			<YAF:ThemeButton ID="Skype" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
				TextLocalizedTag="SKYPE" ImageThemeTag="SKYPE" />
			<YAF:ThemeButton ID="AdminUserButton" runat="server" CssClass="yaflittlebutton" Visible="false"
				TextLocalizedTag="ADMIN_USER" NavigateUrl='<%# YafBuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", Request.QueryString.Get("u") ) %>'>
			</YAF:ThemeButton>
		</td>
	</tr>
	<tr class="post">
		<td valign="top" rowspan="2">
			<DotNetAge:Tabs ID="ProfileTabs" runat="server" ActiveTabEvent="Click" AsyncLoad="false"
				AutoPostBack="false" Collapsible="false" ContentCssClass="" ContentStyle="" Deselectable="false"
				EnabledContentCache="false" HeaderCssClass="" HeaderStyle="" OnClientTabAdd=""
				OnClientTabDisabled="" OnClientTabEnabled="" OnClientTabLoad="" OnClientTabRemove=""
				OnClientTabSelected="" OnClientTabShow="" SelectedIndex="0" Sortable="false" Spinner="">
				<Animations>
					<DotNetAge:AnimationAttribute Name="HeightTransition" AnimationType="height" Value="toggle" />
				</Animations>
				<Views>
					<DotNetAge:View runat="server" ID="AboutTab" Text="About" NavigateUrl="" HeaderCssClass=""
						HeaderStyle="" Target="_blank">
						<table width="100%" cellspacing="1" cellpadding="0">
							<tr>
								<td width="50%" class="postheader">
									<b>
										<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="username" />
									</b>
								</td>
								<td width="50%" class="post">
									<asp:Label ID="Name" runat="server" />
									<YAF:OnlineStatusImage id="OnlineStatusImage1" runat="server" Style="vertical-align: bottom" />
                            <asp:LinkButton ID="lnkBuddy" runat="server" OnCommand="lnk_AddBuddy"/>
                                <asp:literal ID="ltrApproval" runat="server" Text='<%# PageContext.Localization.GetText("BUDDY","AWAIT_BUDDY_APPROVAL") %>'
                                Visible="false">
                                </asp:literal>
								</td>
							</tr>
							<tr runat="server" id="userGroupsRow">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="groups" />
								</td>
								<td class="post">
									<asp:Repeater ID="Groups" runat="server">
										<ItemTemplate>
											<%# Container.DataItem %>
										</ItemTemplate>
										<SeparatorTemplate>
											,
										</SeparatorTemplate>
									</asp:Repeater>
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="rank" />
								</td>
								<td class="post">
									<asp:Label ID="Rank" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="realname" />
								</td>
								<td class="post" runat="server" id="RealName" />
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="location" />
								</td>
								<td class="post">
									<asp:Label ID="Location" runat="server" />
								</td>
							</tr>
						</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel23" runat="server" LocalizedTag="BIRTHDAY" />
								</td>
								<td class="post">
									<asp:Label ID="Birthday" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="occupation" />
								</td>
								<td class="post" runat="server" id="Occupation" />
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="interests" />
								</td>
								<td class="post" runat="server" id="Interests" />
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="gender" />
								</td>
								<td class="post" runat="server" id="Gender" />
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="MSN" />
								</td>
								<td class="post">
									<asp:Label ID="lblmsn" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="AIM" />
								</td>
								<td class="post">
									<asp:Label ID="lblaim" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="YIM" />
								</td>
								<td class="post">
									<asp:Label ID="lblyim" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedTag="ICQ" />
								</td>
								<td class="post">
									<asp:Label ID="lblicq" runat="server" />
								</td>
							</tr>
								<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel22" runat="server" LocalizedTag="XMPP" />
								</td>
								<td class="post">
									<asp:Label ID="lblxmpp" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedTag="SKYPE" />
								</td>
								<td class="post">
									<asp:Label ID="lblskype" runat="server" />
								</td>
							</tr>
						</table>
					</DotNetAge:View>
					<DotNetAge:View runat="server" ID="StatisticsTab" Text="Statistics" NavigateUrl=""
						HeaderCssClass="" HeaderStyle="" Target="_blank">
						<table width="100%" cellspacing="1" cellpadding="0">
							<tr>
								<td width="50%" class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="joined" />
								</td>
								<td width="50%" class="post">
									<asp:Label ID="Joined" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="lastvisit" />
								</td>
								<td class="post">
									<asp:Label ID="LastVisit" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="numposts" />
								</td>
								<td class="post" runat="server" id="Stats" />
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="THANKSFROM" />
								</td>
								<td class="post">
									<asp:Label ID="ThanksFrom" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedTag="THANKSTOTIMES" />
								</td>
								<td class="post">
									<asp:Label ID="ThanksToTimes" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedTag="THANKSTOPOSTS" />
								</td>
								<td class="post">
									<asp:Label ID="ThanksToPosts" runat="server" />
								</td>
							</tr>
						</table>
					</DotNetAge:View>
					<DotNetAge:View runat="server" ID="AvatarTab" Text="Avatar" NavigateUrl="" HeaderCssClass=""
						HeaderStyle="" Target="_blank">
						<table align="center" width="100%" cellspacing="1" cellpadding="0">
							<tr>
								<td class="post" colspan="2" align="center">
									<asp:Image ID="Avatar" runat="server" CssClass="avatarimage" />
								</td>
							</tr>
						</table>
					</DotNetAge:View>
					<DotNetAge:View runat="server" ID="Last10PostsTab" Text="Last 10 Posts Tab" NavigateUrl=""
						HeaderCssClass="" HeaderStyle="" Target="_blank">
						<YAF:ThemeButton ID="SearchUser" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
							TextLocalizedTag="SEARCHUSER" ImageThemeTag="SEARCH" />
						<br style="clear: both" />
						<table width="100%" cellspacing="1" cellpadding="0">
							<asp:Repeater ID="LastPosts" runat="server">
								<ItemTemplate>
									<tr class="postheader">
										<td class="small" align="left" colspan="2">
											<b>
												<YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="topic" />
											</b><a href='<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}",DataBinder.Eval(Container.DataItem,"TopicID")) %>'>
												<%# YafServices.BadWordReplace.Replace(Convert.ToString(DataBinder.Eval(Container.DataItem,"Subject"))) %>
											</a>
											<br />
											<b>
												<YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="posted" />
											</b>
											<%# YafServices.DateTime.FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %>
										</td>
									</tr>
									<tr class="post">
										<td valign="top" class="message" colspan="2">
											<YAF:MessagePostData ID="MessagePost" runat="server" ShowAttachments="false" DataRow="<%# Container.DataItem %>">
											</YAF:MessagePostData>
										</td>
									</tr>
								</ItemTemplate>
							</asp:Repeater>
						</table>
					</DotNetAge:View>
					<DotNetAge:View runat="server" ID="BuddyListTab" Text="Buddy List" NavigateUrl=""
                        HeaderCssClass="" HeaderStyle="" Target="_blank" Visible="<%# YafContext.Current.BoardSettings.EnableBuddyList %>">
                        <YAF:BuddyList runat="server" ID="BuddyList" />
                    </DotNetAge:View>
                    <DotNetAge:View runat="server" ID="AlbumListTab" NavigateUrl=""
                        HeaderCssClass="" HeaderStyle="" Target="_blank" Visible='<%# YafContext.Current.BoardSettings.EnableAlbum %>'>
                        <YAF:AlbumList runat="server" ID="AlbumList1" Mode="1"/>
                    </DotNetAge:View>                    
					<DotNetAge:View runat="server" ID="ModerateTab" Text="Moderation" NavigateUrl=""
						HeaderCssClass="" HeaderStyle="" Visible="false" Target="_blank">
						<YAF:ForumAccess runat="server" ID="ForumAccessControl" />
						<table width="100%" cellspacing="1" cellpadding="0">
							<tr class="header2">
								<td class="header2" colspan="2">
									User Moderation
								</td>
							</tr>
						</table>
						<YAF:SuspendUser runat="server" ID="SuspendUserControl" ShowHeader="False" />
						<YAF:SignatureEdit runat="server" ID="SignatureEditControl" ShowHeader="False" />
					</DotNetAge:View>
				</Views>
			</DotNetAge:Tabs>
		</td>
	</tr>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
