<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminMenu.ascx.cs" Inherits="YAF.Controls.AdminMenu" %>

<%@ Import Namespace="YAF.Types.Constants" %>

<li class="nav-item dropdown">
    <YAF:ThemeButton runat="server" ID="AdminDropdown"
                     Type="None"
                     DataToggle="dropdown"
                     TextLocalizedTag="ADMIN"
                     TextLocalizedPage="ADMINMENU"
                     NavigateUrl="<%# BuildLink.GetLink(ForumPages.Admin_Admin) %>">
    </YAF:ThemeButton>
    <ul class="dropdown-menu" aria-labelledby="hostDropdown">
        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Admin ? " active" : ""%>">
            <a href="<%= BuildLink.GetLink(ForumPages.Admin_Admin) %>">
                <i class="fa fa-tachometer-alt fa-fw"></i>&nbsp;
                <YAF:LocalizedLabel runat="server" 
                                    LocalizedTag="admin_admin" LocalizedPage="ADMINMENU">
                </YAF:LocalizedLabel>
            </a>
        </li>
    <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.Admin_Settings || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_Forums || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_EditForum || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_EditCategory || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_Forums || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_ReplaceWords || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_BBCodes || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_BBCode_Edit || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_Languages ||
                                                 this.PageContext.ForumPageType == ForumPages.Admin_EditLanguage  ? " active" : ""%>">
           <a href="#" data-toggle="dropdown" class="dropdown-toggle"><i class="fa fa-cogs fa-fw"></i>&nbsp;
               <YAF:LocalizedLabel runat="server"
                                   LocalizedTag="Settings" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
           </a>
       <ul class="dropdown-menu">
           <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Settings ? " active" : ""%>">
                                        <a href="<%= BuildLink.GetLink(ForumPages.Admin_Settings) %>">
                                <i class="fa fa-cogs fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_boardsettings" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                                    </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Forums ||
                                                    this.PageContext.ForumPageType == ForumPages.Admin_EditForum ||
                                                    this.PageContext.ForumPageType == ForumPages.Admin_EditCategory ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.Admin_Forums) %>">
                                <i class="fa fa-comments fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_forums" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_ReplaceWords ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.Admin_ReplaceWords) %>">
                                <i class="fa fa-sticky-note fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_replacewords" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
           <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_BBCodes ||
                                                    this.PageContext.ForumPageType == ForumPages.Admin_BBCode_Edit ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.Admin_BBCodes) %>">
                                <i class="fa fa-plug fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_bbcode" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Languages ||
                                                    this.PageContext.ForumPageType == ForumPages.Admin_EditLanguage ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.Admin_Languages) %>">
                                <i class="fa fa-language fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_languages" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                                </ul>
                            </li>
                        <!---->
    <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.Admin_SpamLog || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_SpamWords || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_BannedEmails || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_BannedIps || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_BannedNames ? " active" : ""%>">
                            <a href="#" 
                               data-toggle="dropdown" 
                               class="dropdown-toggle"><i class="fa fa-shield-alt fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                           LocalizedTag="Spam_Protection" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                            <ul class="dropdown-menu">
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_SpamLog ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_SpamLog) %>">
                                <i class="fa fa-book fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_spamlog" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                                </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_SpamWords ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.Admin_SpamWords) %>">
                                <i class="fa fa-shield-alt fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_spamwords" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_BannedEmails ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.Admin_BannedEmails) %>">
                                <i class="fa fa-hand-paper fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_bannedemail" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_BannedIps ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.Admin_BannedIps) %>">
                                <i class="fa fa-hand-paper fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_bannedip" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_BannedNames ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.Admin_BannedNames) %>">
                                <i class="fa fa-hand-paper fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_bannedname" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                                </ul>
                        <!---->
    <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.Admin_AccessMasks || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_EditAccessMask || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_Groups || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_EditGroup || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_Ranks ||
                                                 this.PageContext.ForumPageType == ForumPages.Admin_Users || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_EditUser ||
                                                 this.PageContext.ForumPageType == ForumPages.Admin_EditRank || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_Medals || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_EditMedal || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_Mail || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_Digest ? " active" : ""%>">
                            <a href="#" 
                               data-toggle="dropdown" 
                               class="dropdown-toggle"><i class="fa fa-users fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                           LocalizedTag="UsersandRoles" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                            <ul class="dropdown-menu">
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_AccessMasks ||
                                                            this.PageContext.ForumPageType == ForumPages.Admin_EditAccessMask ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_AccessMasks) %>">
                                    <i class="fa fa-universal-access fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_accessmasks" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Groups ||
                                                            this.PageContext.ForumPageType == ForumPages.Admin_EditGroup ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_Groups) %>">
                                    <i class="fa fa-users fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_groups" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_EditUser ||
                                                            this.PageContext.ForumPageType == ForumPages.Admin_Users ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_Users) %>">
                                    <i class="fa fa-users fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_users" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Ranks ||
                                                            this.PageContext.ForumPageType == ForumPages.Admin_EditRank ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_Ranks) %>">
                                    <i class="fa fa-graduation-cap fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_ranks" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Medals ||
                                                            this.PageContext.ForumPageType == ForumPages.Admin_EditMedal ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_Medals) %>">
                                    <i class="fa fa-medal fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_medals" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Mail ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_Mail) %>">
                                    <i class="fa fa-at fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_mail" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Digest ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_Digest) %>">
                                    <i class="fa fa-envelope fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_digest" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                            </ul>
                        </li>
                        <!---->
    <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.Admin_Prune || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_Restore || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_TaskManager || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_EventLog || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_RestartApp ? " active" : ""%>">
                            <a href="#" data-toggle="dropdown" class="dropdown-toggle"><i class="fa fa-recycle fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                           LocalizedTag="Maintenance" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                            <ul class="dropdown-menu">
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Prune ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_Prune) %>">
                                    <i class="fa fa-trash fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_prune" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Restore ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_Restore) %>">
                                        <i class="fa fa-trash-restore fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="ADMIN_RESTORE" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Pm ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_Pm) %>">
                                    <i class="fa fa-envelope-square fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_pm" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_TaskManager ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_TaskManager) %>">
                                    <i class="fa fa-tasks fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_taskmanager" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_EventLog ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_EventLog) %>">
                                    <i class="fa fa-book fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_eventlog" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_RestartApp ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.Admin_RestartApp) %>">
                                    <i class="fa fa-sync fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_restartapp" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                            </ul>
                        </li>
                            
                            <!---->
    <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.Admin_ReIndex || 
                                                 this.PageContext.ForumPageType == ForumPages.Admin_RunSql ? " active" : ""%>">
                                <a href="#" data-toggle="dropdown" class="dropdown-toggle"><i class="fa fa-database fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                               LocalizedTag="Database" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                                <ul class="dropdown-menu">
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_ReIndex ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.Admin_ReIndex) %>">
                                        <i class="fa fa-database fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_reindex" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_RunSql ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.Admin_RunSql) %>">
                                        <i class="fa fa-database fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_runsql" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                </ul>
                            </li>
                            
                            <!---->
                            <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.Admin_NntpRetrieve || 
                                                                         this.PageContext.ForumPageType == ForumPages.Admin_NntpForums || 
                                                                         this.PageContext.ForumPageType == ForumPages.Admin_NntpServers ? " active" : ""%>">
                                <a href="#" data-toggle="dropdown" class="dropdown-toggle"><i class="fa fa-newspaper fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                               LocalizedTag="NNTP" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                                <ul class="dropdown-menu">
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_NntpServers ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.Admin_NntpServers) %>">
                                        <i class="fa fa-newspaper fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_nntpservers" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_NntpForums ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.Admin_NntpForums) %>">
                                        <i class="fa fa-newspaper fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_nntpforums" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_NntpRetrieve ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.Admin_NntpRetrieve) %>">
                                        <i class="fa fa-newspaper fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_nntpretrieve" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                </ul>
                            </li>
                            
                             <!---->
                            <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.Admin_Version ? " active" : ""%>">
                                <a href="#" data-toggle="dropdown" class="dropdown-toggle"><i class="fa fa-download fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                               LocalizedTag="Upgrade" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                                <ul class="dropdown-menu">
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Version ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.Admin_Version) %>">
                                        <i class="fa fa-info fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_version" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.Admin_Admin ? " active" : ""%>"><a href="<%= this.ResolveUrl("~/install/default.aspx") %>">
                                        <i class="fa fa-download fa-fw"></i>&nbsp;
                                        Upgrade
                                    </a></li>
                                </ul>
                            </li>
    </ul>
</li>