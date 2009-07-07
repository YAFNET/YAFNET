<%@ Control Language="c#" CodeFile="profile.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.profile" %>
<%@ Register TagPrefix="uc1" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SuspendUser" Src="../controls/EditUsersSuspend.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ForumAccess" Src="../controls/ForumProfileAccess.ascx" %>
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
            <YAF:ThemeButton ID="Skype" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                TextLocalizedTag="SKYPE" ImageThemeTag="SKYPE" />
            <YAF:ThemeButton ID="AdminUserButton" runat="server" CssClass="yaflittlebutton" Visible="false"
                TextLocalizedTag="ADMIN_USER" NavigateUrl='<%# YafBuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", Request.QueryString.Get("u") ) %>'>
            </YAF:ThemeButton>
        </td>
    </tr>
    <tr class="post">
        <td valign="top" rowspan="2">
            <ajaxToolkit:TabContainer runat="server" ID="ProfileTabs" CssClass="ajax__tab_yaf">
                <ajaxToolkit:TabPanel runat="server" ID="AboutTab" HeaderText="About">
                    <ContentTemplate>
                        <table width="100%" cellspacing="1" cellpadding="0">
                            <tr>
                                <td width="50%" class="postheader">
                                    <b>
                                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="username" />
                                    </b>
                                </td>
                                <td width="50%" class="post">
                                    <asp:Label ID="Name" runat="server" />
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
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="StatisticsTab" HeaderText="Statistics">
                    <ContentTemplate>
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
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="AvatarTab">
                    <ContentTemplate>
                        <table align="center" width="100%" cellspacing="1" cellpadding="0">
                            <tr>
                                <td class="post" colspan="2" align="center">
                                    <asp:Image ID="Avatar" runat="server" CssClass="avatarimage" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="Last10PostsTab">
                    <ContentTemplate>
                    <YAF:ThemeButton ID="SearchUser" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
                        TextLocalizedTag="SEARCHUSER" ImageThemeTag="SEARCH" />  
                        <br style="clear:both" />                  
                        <table width="100%" cellspacing="1" cellpadding="0">
                            <asp:Repeater ID="LastPosts" runat="server">
                                <ItemTemplate>
                                    <tr class="postheader">
                                        <td class="small" align="left" colspan="2">
                                            <b>
                                                <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="topic" />
                                            </b><a href='<%# YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.posts,"t={0}",DataBinder.Eval(Container.DataItem,"TopicID")) %>'>
                                                <%# General.BadWordReplace(Convert.ToString(DataBinder.Eval(Container.DataItem,"Subject"))) %>
                                            </a>
                                            <br />
                                            <b>
                                                <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="posted" />
                                            </b>
                                            <%# YafDateTime.FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %>
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
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="ModerateTab" HeaderText="Moderation" Visible="false">
                    <ContentTemplate>
                        <uc1:ForumAccess runat="server" ID="ForumAccessControl" />
                        <table width="100%" cellspacing="1" cellpadding="0">
                            <tr class="header2">
                                <td class="header2" colspan="2">
                                    User Moderation
                                </td>
                            </tr>
                        </table>                            
                        <uc1:SuspendUser runat="server" ID="SuspendUserControl" ShowHeader="False" />
                        <uc1:SignatureEdit runat="server" ID="SignatureEditControl" ShowHeader="False" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
