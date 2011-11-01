<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.profile" Codebehind="profile.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Register TagPrefix="YAF" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>
<%@ Register TagPrefix="YAF" TagName="SuspendUser" Src="../controls/EditUsersSuspend.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumAccess" Src="../controls/ForumProfileAccess.ascx" %>
<%@ Register TagPrefix="YAF" TagName="BuddyList" Src="../controls/BuddyList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="AlbumList" Src="../controls/AlbumList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content profileContent" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2">
			<YAF:LocalizedLabel runat="server" LocalizedTag="profile" />
			<YAF:UserLabel ID="UserLabel1" runat="server" />
		</td>
	</tr>
	<tr class="post">
		<td colspan="2">
			<YAF:ThemeButton  ID="PM" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="PM" ImageThemeTag="PM" TitleLocalizedTag="PM_TITLE" TitleLocalizedPage="POSTS" />
			<YAF:ThemeButton ID="Email" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="EMAIL" ImageThemeTag="EMAIL" TitleLocalizedTag="EMAIL_TITLE" TitleLocalizedPage="POSTS" />
			<YAF:ThemeButton ID="Home" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="HOME" ImageThemeTag="HOME" TitleLocalizedTag="HOME_TITLE" TitleLocalizedPage="POSTS" />
			<YAF:ThemeButton ID="Blog" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="BLOG" ImageThemeTag="BLOG" TitleLocalizedTag="BLOG_TITLE" TitleLocalizedPage="POSTS" />
			<YAF:ThemeButton ID="MSN" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="MSN" ImageThemeTag="MSN" TitleLocalizedTag="MSN_TITLE" TitleLocalizedPage="POSTS" />
			<YAF:ThemeButton ID="AIM" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="AIM" ImageThemeTag="AIM" TitleLocalizedTag="AIM_TITLE" TitleLocalizedPage="POSTS" />
			<YAF:ThemeButton ID="YIM" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="YIM" ImageThemeTag="YIM" TitleLocalizedTag="YIM_TITLE" TitleLocalizedPage="POSTS" />
			<YAF:ThemeButton ID="ICQ" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="ICQ" ImageThemeTag="ICQ" TitleLocalizedTag="ICQ_TITLE" TitleLocalizedPage="POSTS" />
			<YAF:ThemeButton ID="XMPP" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="XMPP" ImageThemeTag="XMPP" TitleLocalizedTag="XMPP_TITLE" TitleLocalizedPage="POSTS" />	
			<YAF:ThemeButton ID="Skype" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="SKYPE" ImageThemeTag="SKYPE" TitleLocalizedTag="SKYPE_TITLE" TitleLocalizedPage="POSTS" />
            <YAF:ThemeButton ID="Facebook" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="FACEBOOK" ImageThemeTag="Facebook2" TitleLocalizedTag="FACEBOOK_TITLE" TitleLocalizedPage="POSTS" />
            <YAF:ThemeButton ID="Twitter" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="TWITTER" ImageThemeTag="Twitter2" TitleLocalizedTag="TWITTER_TITLE" TitleLocalizedPage="POSTS" />
			<YAF:ThemeButton ID="AdminUserButton" runat="server" CssClass="yaflittlebutton" Visible="false"
				TextLocalizedTag="ADMIN_USER" NavigateUrl='<%# YafBuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", this.UserId ) %>'>
			</YAF:ThemeButton>
		</td>
	</tr>
	<tr class="post">
		<td valign="top" rowspan="2">
			<asp:Panel id="ProfileTabs" runat="server">
               <ul>
                 <li><a href="#AboutTab"><YAF:LocalizedLabel ID="LocalizedLabel40" runat="server" LocalizedTag="ABOUT" /></a></li>
		 <li><a href="#StatisticsTab"><YAF:LocalizedLabel ID="LocalizedLabel41" runat="server" LocalizedTag="STATISTICS" /></a></li>
		 <li runat="server" id="AvatarLi"><a href='#<%# this.AvatarTab.ClientID %>'><YAF:LocalizedLabel ID="LocalizedLabel42" runat="server" LocalizedTag="AVATAR" /></a></li>
		 <li><a href="#Last10PostsTab"><YAF:LocalizedLabel ID="LocalizedLabel43" runat="server" LocalizedTag="LAST10" /></a></li>
		 <li><a href="#BuddyListTab"><YAF:LocalizedLabel ID="LocalizedLabel44" runat="server" LocalizedTag='<%# this.UserId == this.PageContext.PageUserID ? "BUDDIES" : "BUDDIESTITLE"%>' /></a></li>		        
		 <li runat="server" id="AlbumListLi"><a href='#<%# this.AlbumListTab.ClientID %>'><YAF:LocalizedLabel ID="LocalizedLabel45" runat="server" LocalizedTag="ALBUMS" /></a></li>	
		 <li runat="server" id="ModerateLi"><a href='#<%# this.ModerateTab.ClientID %>'><YAF:LocalizedLabel ID="LocalizedLabel46" runat="server" LocalizedTag="MODERATION" /></a></li>	
               </ul>
                <div id="AboutTab">
                   <table width="100%" cellspacing="1" cellpadding="0">
							<tr>
								<td width="50%" class="postheader">
									<strong>
										<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="username" />
									</strong>
								</td>
								<td width="50%" class="post">
									<asp:Label ID="Name" runat="server" />
									<YAF:OnlineStatusImage id="OnlineStatusImage1" runat="server" Style="vertical-align: bottom" />
                            <asp:LinkButton ID="lnkBuddy" runat="server" OnCommand="lnk_AddBuddy"/>
                                <asp:literal ID="ltrApproval" runat="server" Text='<%# this.GetText("BUDDY","AWAIT_BUDDY_APPROVAL") %>'
                                Visible="false">
                                </asp:literal>
								</td>
							</tr>
							<tr runat="server" id="userGroupsRow" visible="false">
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
							<tr runat="server" id="RankTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="rank" />
								</td>
								<td class="post">
									<asp:Label ID="Rank" runat="server" />
								</td>
							</tr>
							<tr runat="server" id="RealNameTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="realname" />
								</td>
								<td class="post" runat="server" id="RealName" />
							</tr>
                            <tr runat="server" id="CountryTR" visible="false">
								<td class="postheader">
								<YAF:LocalizedLabel ID="LocalizedLabel27" runat="server" LocalizedTag="country" />
								</td>
								<td class="post">
									<img id="CountryFlagImage" runat="server" alt=""
                                    title=""
                                    src="" />&nbsp;<asp:Label ID="CountryLabel" runat="server" />
								</td>
							</tr>
                            <tr runat="server" id="RegionTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel28" runat="server" LocalizedTag="region" />
								</td>
								<td class="post">
									<asp:Label ID="RegionLabel" runat="server" />
								</td>
							</tr>
                            <tr runat="server" id="CityTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel26" runat="server" LocalizedTag="city" />
								</td>
								<td class="post">
									<asp:Label ID="CityLabel" runat="server" />
								</td>
							</tr>
							<tr runat="server" id="LocationTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="location" />
								</td>
								<td class="post">
									<asp:Label ID="Location" runat="server" />
								</td>
							</tr>                            
							<tr runat="server" id="BirthdayTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel23" runat="server" LocalizedTag="BIRTHDAY" />
								</td>
								<td class="post">
									<asp:Label ID="Birthday" runat="server" />
								</td>
							</tr>
							<tr  runat="server" id="OccupationTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="occupation" />
								</td>
								<td class="post" runat="server" id="Occupation" />
							</tr>
							<tr  runat="server" id="InterestsTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="interests" />
								</td>
								<td class="post" runat="server" id="Interests" />
							</tr>
							<tr runat="server" id="GenderTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="gender" />
								</td>
								<td class="post" runat="server" id="Gender" />
							</tr>
							<tr runat="server" id="MsnTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="MSN" />
								</td>
								<td class="post">
									<asp:Label ID="lblmsn" runat="server" />
								</td>
							</tr>
							<tr runat="server" id="AimTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="AIM" />
								</td>
								<td class="post">
									<asp:Label ID="lblaim" runat="server" />
								</td>
							</tr>
							<tr runat="server" id="YimTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="YIM" />
								</td>
								<td class="post">
									<asp:Label ID="lblyim" runat="server" />
								</td>
							</tr>
							<tr  runat="server" id="IcqTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedTag="ICQ" />
								</td>
								<td class="post">
									<asp:Label ID="lblicq" runat="server" />
								</td>
							</tr>
							<tr  runat="server" id="XmppTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel22" runat="server" LocalizedTag="XMPP" />
								</td>
								<td class="post">
									<asp:Label ID="lblxmpp" runat="server" />
								</td>
							</tr>
							<tr runat="server" id="SkypeTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedTag="SKYPE" />
								</td>
								<td class="post">
									<asp:Label ID="lblskype" runat="server" />
								</td>
							</tr>
                            <tr  runat="server" id="FacebookTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel24" runat="server" LocalizedTag="Facebook" />
								</td>
								<td class="post">
									<asp:Label ID="lblfacebook" runat="server" />
								</td>
							</tr>
                            <tr  runat="server" id="TwitterTR" visible="false">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel25" runat="server" LocalizedTag="Twitter" />
								</td>
								<td class="post">
									<asp:Label ID="lbltwitter" runat="server" />
								</td>
							</tr>
						</table>
                </div>
                <div id="StatisticsTab">
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
									<asp:Label ID="LastVisit" runat="server" Visible="false" />
                                    <YAF:DisplayDateTime id="LastVisitDateTime" runat="server" Visible="false"></YAF:DisplayDateTime>
								</td>
							</tr>
							<tr>
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="numposts" />
								</td>
								<td class="post" runat="server" id="Stats" />
							</tr>
							<tr id="divTF" runat="server" visible="<%# this.Get<YafBoardSettings>().EnableThanksMod %>">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="THANKSFROM" />
								</td>
								<td class="post">
									<asp:Label ID="ThanksFrom" runat="server" />
                                    <asp:LinkButton ID="lnkThanks" runat="server" OnCommand="lnk_ViewThanks"/>
								</td>
							</tr>
							<tr id="divTTT" runat="server" visible="<%# this.Get<YafBoardSettings>().EnableThanksMod %>">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedTag="THANKSTOTIMES" />
								</td>
								<td class="post">
									<asp:Label ID="ThanksToTimes" runat="server" />
								</td>
							</tr>
							<tr id="divTTP" runat="server" visible="<%# this.Get<YafBoardSettings>().EnableThanksMod %>">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedTag="THANKSTOPOSTS" />
								</td>
								<td class="post">
									<asp:Label ID="ThanksToPosts" runat="server" />
								</td>
							</tr>
                            <tr id="divRR" runat="server" visible="<%# this.Get<YafBoardSettings>().EnableUserReputation %>">
								<td class="postheader">
									<YAF:LocalizedLabel ID="LocalizedLabel29" runat="server" LocalizedTag="REPUTATION_RECEIVED" />
								</td>
								<td class="post">
									<asp:Label ID="ReputationReceived" runat="server" />
								</td>
							</tr>
						</table>
                </div>
                <div runat="server" id="AvatarTab">
                  <table align="center" width="100%" cellspacing="1" cellpadding="0">
							<tr>
								<td class="post" colspan="2" align="center">
									<asp:Image ID="Avatar" runat="server" CssClass="avatarimage" />
								</td>
							</tr>
						</table>
                </div>
                <div id="Last10PostsTab">
                  <YAF:ThemeButton ID="SearchUser" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
							TextLocalizedTag="SEARCHUSER" ImageThemeTag="SEARCH" />
						<br style="clear: both" />
						<table width="100%" cellspacing="1" cellpadding="0">
							<asp:Repeater ID="LastPosts" runat="server">
								<ItemTemplate>
									<tr class="postheader">
										<td class="small" align="left" colspan="2">
											<strong>
												<YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="topic" />
											</strong><a title='<%# this.GetText("COMMON", "VIEW_TOPIC") %>' href='<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}",Container.DataItemToField<int>("TopicID")) %>'>
												<%# this.Get<IBadWordReplace>().Replace(HtmlEncode(Container.DataItemToField<string>("Subject"))) %>
											</a>&nbsp;
                                            <a href='<%# YafBuildLink.GetLink(ForumPages.posts,"m={0}#post{0}",Container.DataItemToField<int>("MessageID")) %>'>
												<YAF:ThemeImage ID="ThisPostImage" ThemeTag="ICON_LATEST" LocalizedTitle='<%# this.GetText("DEFAULT", "GO_LAST_POST") %>' UseTitleForEmptyAlt="true"  runat="server" Style="border: 0" />
                                            </a>
											<br />
											<strong>
												<YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="posted" />
											</strong>
											<%# this.Get<IDateTime>().FormatDateTime(Container.DataItemToField<DateTime>("Posted"))%>
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
                </div>
                <div id="BuddyListTab">
                  <YAF:BuddyList runat="server" ID="BuddyList" />
                </div>
                <div runat="server" id="AlbumListTab">
                  <YAF:AlbumList runat="server" ID="AlbumList1" Mode="1"/>
                </div>
                <div runat="server" id="ModerateTab">
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
                </div>
             </asp:Panel>
            <asp:HiddenField runat="server" ID="hidLastTab" Value="0" />
		</td>
	</tr>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
