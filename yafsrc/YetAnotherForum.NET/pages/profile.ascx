<%@ Control Language="c#" CodeFile="profile.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.profile" %>
<%@ Import Namespace="YAF.Classes.Core"%>
<%@ Register TagPrefix="uc1" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SuspendUser" Src="../controls/EditUsersSuspend.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ForumAccess" Src="../controls/ForumProfileAccess.ascx" %>
<yaf:pagelinks runat="server" id="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="2">
            <yaf:localizedlabel runat="server" localizedtag="profile" />
            <asp:Label ID="UserName" runat="server" />
        </td>
    </tr>
    <tr class="post">
        <td colspan="2">
            <yaf:themebutton id="PM" runat="server" cssclass="yafcssimagebutton" textlocalizedpage="POSTS" textlocalizedtag="PM" imagethemetag="PM" />
            <yaf:themebutton id="Email" runat="server" cssclass="yafcssimagebutton" textlocalizedpage="POSTS" textlocalizedtag="EMAIL" imagethemetag="EMAIL" />
            <yaf:themebutton id="Home" runat="server" cssclass="yafcssimagebutton" textlocalizedpage="POSTS" textlocalizedtag="HOME" imagethemetag="HOME" />
            <yaf:themebutton id="Blog" runat="server" cssclass="yafcssimagebutton" textlocalizedpage="POSTS" textlocalizedtag="BLOG" imagethemetag="BLOG" />
            <yaf:themebutton id="MSN" runat="server" cssclass="yafcssimagebutton" textlocalizedpage="POSTS" textlocalizedtag="MSN" imagethemetag="MSN" />
            <yaf:themebutton id="AIM" runat="server" cssclass="yafcssimagebutton" textlocalizedpage="POSTS" textlocalizedtag="AIM" imagethemetag="AIM" />
            <yaf:themebutton id="YIM" runat="server" cssclass="yafcssimagebutton" textlocalizedpage="POSTS" textlocalizedtag="YIM" imagethemetag="YIM" />
            <yaf:themebutton id="ICQ" runat="server" cssclass="yafcssimagebutton" textlocalizedpage="POSTS" textlocalizedtag="ICQ" imagethemetag="ICQ" />
            <yaf:themebutton id="Skype" runat="server" cssclass="yafcssimagebutton" textlocalizedpage="POSTS" textlocalizedtag="SKYPE" imagethemetag="SKYPE" />
            <yaf:themebutton id="AdminUserButton" runat="server" cssclass="yaflittlebutton" visible="false" textlocalizedtag="ADMIN_USER" navigateurl='<%# YafBuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", Request.QueryString.Get("u") ) %>'>
            </yaf:themebutton>
        </td>
    </tr>
    <tr class="post">
        <td valign="top" rowspan="2">
            <ajaxtoolkit:tabcontainer runat="server" id="ProfileTabs" cssclass="ajax__tab_yaf">
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
                                    <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedTag="SKYPE" />
                                </td>
                                <td class="post">
                                    <asp:Label ID="lblskype" runat="server" />
                                </td>
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
            </ajaxtoolkit:tabcontainer>
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <yaf:smartscroller id="SmartScroller1" runat="server" />
</div>
