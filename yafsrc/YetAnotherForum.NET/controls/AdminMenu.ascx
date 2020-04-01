<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminMenu.ascx.cs" Inherits="YAF.Controls.AdminMenu" %>

<%@ Import Namespace="YAF.Types.Constants" %>

<li class="nav-item dropdown">
    <YAF:ThemeButton runat="server" ID="AdminDropdown"
                     Type="None"
                     DataToggle="dropdown"
                     TextLocalizedTag="ADMIN"
                     TextLocalizedPage="ADMINMENU"
                     NavigateUrl="<%# BuildLink.GetLink(ForumPages.admin_admin) %>">
    </YAF:ThemeButton>
    <ul class="dropdown-menu" aria-labelledby="hostDropdown">
        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_admin ? " active" : ""%>">
            <a href="<%= BuildLink.GetLink(ForumPages.admin_admin) %>">
                <i class="fa fa-tachometer-alt fa-fw"></i>&nbsp;
                <YAF:LocalizedLabel runat="server" 
                                    LocalizedTag="admin_admin" LocalizedPage="ADMINMENU">
                </YAF:LocalizedLabel>
            </a>
        </li>
    <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.admin_boardsettings || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_forums || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_editforum || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_editcategory || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_forums || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_replacewords || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_extensions || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_bbcode || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_bbcode_edit || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_languages ||
                                                 this.PageContext.ForumPageType == ForumPages.admin_editlanguage  ? " active" : ""%>">
           <a href="#" data-toggle="dropdown" class="dropdown-toggle"><i class="fa fa-cogs fa-fw"></i>&nbsp;
               <YAF:LocalizedLabel runat="server"
                                   LocalizedTag="Settings" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
           </a>
       <ul class="dropdown-menu">
           <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_boardsettings ? " active" : ""%>">
                                        <a href="<%= BuildLink.GetLink(ForumPages.admin_boardsettings) %>">
                                <i class="fa fa-cogs fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_boardsettings" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                                    </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_forums ||
                                                    this.PageContext.ForumPageType == ForumPages.admin_editforum ||
                                                    this.PageContext.ForumPageType == ForumPages.admin_editcategory ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_forums) %>">
                                <i class="fa fa-comments fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_forums" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_replacewords ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_replacewords) %>">
                                <i class="fa fa-sticky-note fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_replacewords" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_extensions ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_extensions) %>">
                                <i class="fa fa-puzzle-piece fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_extensions" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_bbcode ||
                                                    this.PageContext.ForumPageType == ForumPages.admin_bbcode_edit ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_bbcode) %>">
                                <i class="fa fa-plug fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_bbcode" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_languages ||
                                                    this.PageContext.ForumPageType == ForumPages.admin_editlanguage ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_languages) %>">
                                <i class="fa fa-language fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_languages" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                                </ul>
                            </li>
                        <!---->
    <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.admin_spamlog || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_spamwords || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_bannedemail || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_bannedip || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_bannedname ? " active" : ""%>">
                            <a href="#" 
                               data-toggle="dropdown" 
                               class="dropdown-toggle"><i class="fa fa-shield-alt fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                           LocalizedTag="Spam_Protection" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                            <ul class="dropdown-menu">
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_spamlog ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_spamlog) %>">
                                <i class="fa fa-book fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_spamlog" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                                </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_spamwords ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_spamwords) %>">
                                <i class="fa fa-shield-alt fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_spamwords" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_bannedemail ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_bannedemail) %>">
                                <i class="fa fa-hand-paper fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_bannedemail" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_bannedip ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_bannedip) %>">
                                <i class="fa fa-hand-paper fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_bannedip" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                        <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_bannedname ? " active" : ""%>">
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_bannedname) %>">
                                <i class="fa fa-hand-paper fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_bannedname" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                            </a>
                        </li>
                                </ul>
                        <!---->
    <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.admin_accessmasks || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_editaccessmask || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_groups || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_editgroup || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_ranks ||
                                                 this.PageContext.ForumPageType == ForumPages.admin_users || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_edituser ||
                                                 this.PageContext.ForumPageType == ForumPages.admin_editrank || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_medals || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_editmedal || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_mail || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_digest ? " active" : ""%>">
                            <a href="#" 
                               data-toggle="dropdown" 
                               class="dropdown-toggle"><i class="fa fa-users fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                           LocalizedTag="UsersandRoles" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                            <ul class="dropdown-menu">
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_accessmasks ||
                                                            this.PageContext.ForumPageType == ForumPages.admin_editaccessmask ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_accessmasks) %>">
                                    <i class="fa fa-universal-access fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_accessmasks" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_groups ||
                                                            this.PageContext.ForumPageType == ForumPages.admin_editgroup ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_groups) %>">
                                    <i class="fa fa-users fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_groups" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_users ||
                                                            this.PageContext.ForumPageType == ForumPages.admin_edituser ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_users) %>">
                                    <i class="fa fa-users fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_users" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_ranks ||
                                                            this.PageContext.ForumPageType == ForumPages.admin_editrank ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_ranks) %>">
                                    <i class="fa fa-graduation-cap fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_ranks" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_medals ||
                                                            this.PageContext.ForumPageType == ForumPages.admin_editmedal ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_medals) %>">
                                    <i class="fa fa-trophy fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_medals" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_mail ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_mail) %>">
                                    <i class="fa fa-at fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_mail" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_digest ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_digest) %>">
                                    <i class="fa fa-envelope fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_digest" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                            </ul>
                        </li>
                        <!---->
    <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.admin_prune || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_restore || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_taskmanager || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_eventlog || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_restartapp ? " active" : ""%>">
                            <a href="#" data-toggle="dropdown" class="dropdown-toggle"><i class="fa fa-recycle fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                           LocalizedTag="Maintenance" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                            <ul class="dropdown-menu">
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_prune ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_prune) %>">
                                    <i class="fa fa-trash fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_prune" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_restore ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_restore) %>">
                                        <i class="fa fa-trash-restore fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="ADMIN_RESTORE" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_pm ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_pm) %>">
                                    <i class="fa fa-envelope-square fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_pm" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_taskmanager ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_taskmanager) %>">
                                    <i class="fa fa-tasks fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_taskmanager" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_eventlog ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_eventlog) %>">
                                    <i class="fa fa-book fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_eventlog" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                                <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_restartapp ? " active" : ""%>">
                                    <a href="<%= BuildLink.GetLink(ForumPages.admin_restartapp) %>">
                                    <i class="fa fa-sync fa-fw"></i>&nbsp;
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="admin_restartapp" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                </a>
                                </li>
                            </ul>
                        </li>
                            
                            <!---->
    <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.admin_reindex || 
                                                 this.PageContext.ForumPageType == ForumPages.admin_runsql ? " active" : ""%>">
                                <a href="#" data-toggle="dropdown" class="dropdown-toggle"><i class="fa fa-database fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                               LocalizedTag="Database" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                                <ul class="dropdown-menu">
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_reindex ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.admin_reindex) %>">
                                        <i class="fa fa-database fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_reindex" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_runsql ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.admin_runsql) %>">
                                        <i class="fa fa-database fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_runsql" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                </ul>
                            </li>
                            
                            <!---->
                            <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.admin_nntpretrieve || 
                                                                         this.PageContext.ForumPageType == ForumPages.admin_nntpforums || 
                                                                         this.PageContext.ForumPageType == ForumPages.admin_nntpservers ? " active" : ""%>">
                                <a href="#" data-toggle="dropdown" class="dropdown-toggle"><i class="fa fa-newspaper fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                               LocalizedTag="NNTP" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                                <ul class="dropdown-menu">
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_nntpservers ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.admin_nntpservers) %>">
                                        <i class="fa fa-newspaper fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_nntpservers" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_nntpforums ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.admin_nntpforums) %>">
                                        <i class="fa fa-newspaper fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_nntpforums" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_nntpretrieve ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.admin_nntpretrieve) %>">
                                        <i class="fa fa-newspaper fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_nntpretrieve" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                </ul>
                            </li>
                            
                             <!---->
                            <li class="dropdown-item dropdown-submenu<%= this.PageContext.ForumPageType == ForumPages.admin_version ? " active" : ""%>">
                                <a href="#" data-toggle="dropdown" class="dropdown-toggle"><i class="fa fa-download fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server"
                                                                               LocalizedTag="Upgrade" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel></a>
                                <ul class="dropdown-menu">
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_version ? " active" : ""%>"><a href="<%= BuildLink.GetLink(ForumPages.admin_version) %>">
                                        <i class="fa fa-info fa-fw"></i>&nbsp;
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_version" LocalizedPage="ADMINMENU"></YAF:LocalizedLabel>
                                    </a></li>
                                    <li class="dropdown-item<%= this.PageContext.ForumPageType == ForumPages.admin_admin ? " active" : ""%>"><a href="<%= this.ResolveUrl("~/install/default.aspx") %>">
                                        <i class="fa fa-download fa-fw"></i>&nbsp;
                                        Upgrade
                                    </a></li>
                                </ul>
                            </li>
    </ul>
</li>