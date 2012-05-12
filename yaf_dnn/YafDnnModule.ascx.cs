/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.DotNetNuke
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;

    using global::DotNetNuke.Common;
    using global::DotNetNuke.Common.Utilities;
    using global::DotNetNuke.Entities.Modules;
    using global::DotNetNuke.Entities.Modules.Actions;
    using global::DotNetNuke.Entities.Portals;
    using global::DotNetNuke.Entities.Profile;
    using global::DotNetNuke.Entities.Users;
    using global::DotNetNuke.Framework;
    using global::DotNetNuke.Security;
    using global::DotNetNuke.Security.Roles;
    using global::DotNetNuke.Services.Exceptions;
    using global::DotNetNuke.Services.Localization;
    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.DotNetNuke.Controller;
    using YAF.DotNetNuke.Utils;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The DotNetNuke Module Class.
    /// </summary>
    public partial class YafDnnModule : PortalModuleBase, IActionable
    {
        #region Constants and Fields

        /// <summary>
        /// The _create new board.
        /// </summary>
        private bool createNewBoard;

        /// <summary>
        /// The _portal settings.
        /// </summary>
        private PortalSettings portalSettings;

        /// <summary>
        /// The forum 1.
        /// </summary>
        private Forum forum1;

        /// <summary>
        /// The basePage
        /// </summary>
        private CDefault basePage;

        #endregion

        #region Properties

        /// <summary>
        ///  Gets Add Menu Entries to Module Container
        /// </summary>
        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection
                    {
                        {
                            this.GetNextActionID(), 
                            Localization.GetString("EditYafSettings.Text", this.LocalResourceFile), 
                            ModuleActionType.AddContent, string.Empty, string.Empty, this.EditUrl(), 
                            false, 
                            SecurityAccessLevel.Host, 
                            true, 
                            false
                            },
                        {
                            this.GetNextActionID(), Localization.GetString("UserImporter.Text", this.LocalResourceFile),
                            ModuleActionType.AddContent, string.Empty, string.Empty, this.EditUrl("Import"), false,
                            SecurityAccessLevel.Host, true, false
                            }
                    };

                return actions;
            }
        }

        /// <summary>
        /// Gets the session user key name.
        /// </summary>
        public string SessionUserKeyName
        {
            get
            {
                return "yaf_dnn_boardid{0}_userid{1}_portalid{2}".FormatWith(
                    this.forum1.BoardID, this.UserId, this.CurrentPortalSettings.PortalId);
            }
        }

        /// <summary>
        /// Gets the Base Page
        /// </summary>
        public CDefault BasePage
        {
            get
            {
                return this.basePage ?? (this.basePage = GetDefault(this));
            }
        }

        /// <summary>
        /// Gets YafCultures
        /// </summary>
        private static List<YafCultureInfo> YafCultures
        {
            get
            {
                return GetYafCultures();
            }
        }

        /// <summary>
        /// Gets CurrentPortalSettings.
        /// </summary>
        private PortalSettings CurrentPortalSettings
        {
            get
            {
                return this.portalSettings ?? (this.portalSettings = PortalController.GetCurrentPortalSettings());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on error.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnError(EventArgs e)
        {
            Exception x = this.Server.GetLastError();
            LegacyDb.eventlog_create(YafContext.Current.PageUserID, this, x);

            base.OnError(e);
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();

            base.OnInit(e);
        }

        /// <summary>
        /// Get Default CDefault
        /// </summary>
        /// <param name="c">
        /// The Control c
        /// </param>
        /// <returns>The Control</returns>
        private static CDefault GetDefault(Control c)
        {
            Control parent = c.Parent;

            if (parent != null)
            {
                if (parent is CDefault)
                {
                    return (CDefault)parent;
                }

                return GetDefault(parent);
            }

            return null;
        }

        /// <summary>
        /// The get yaf cultures.
        /// </summary>
        /// <returns>
        /// Dictonary with Yaf Cultures
        /// </returns>
        private static List<YafCultureInfo> GetYafCultures()
        {
            List<YafCultureInfo> yafCultures = new List<YafCultureInfo>();
            DataTable cult = StaticDataHelper.Cultures();

            foreach (DataRow row in cult.Rows)
            {
                try
                {
                    yafCultures.Add(
                        new YafCultureInfo
                            {
                                Culture = row["CultureTag"].ToString(), LanguageFile = row["CultureFile"].ToString() 
                            });
                }
                catch
                {
                    continue;
                }
            }

            if (yafCultures.Count == 0)
            {
                yafCultures.Add(new YafCultureInfo { Culture = "en", LanguageFile = "english.xml" });
            }

            return yafCultures;
        }
        
        /// <summary>
        /// The get yaf culture info.
        /// </summary>
        /// <param name="cultureInfo">
        /// The culture info.
        /// </param>
        /// <returns>
        /// The Yaf Culture
        /// </returns>
        private static YafCultureInfo GetYafCultureInfo(CultureInfo cultureInfo)
        {
            string culture = "en";
            string lngFile = "english.xml";

            YafCultureInfo yafCultureInfo = new YafCultureInfo();

            if (cultureInfo != null)
            {
                if (YafCultures.Find(yafCult => yafCult.Culture.Equals(cultureInfo.TwoLetterISOLanguageName)) != null)
                {
                    culture = cultureInfo.TwoLetterISOLanguageName;
                    lngFile =
                        YafCultures.Find(yafCult => yafCult.Culture.Equals(cultureInfo.TwoLetterISOLanguageName)).LanguageFile;
                }
                else if (YafCultures.Find(yafCult => yafCult.Culture.Equals(cultureInfo.Name)) != null)
                {
                    culture = cultureInfo.Name;
                    lngFile = YafCultures.Find(yafCult => yafCult.Culture.Equals(cultureInfo.Name)).LanguageFile;
                }
            }

            yafCultureInfo.Culture = culture;
            yafCultureInfo.LanguageFile = lngFile;

            return yafCultureInfo;
        }

        /// <summary>
        /// Change YAF Language based on DNN Language, 
        ///   this will override the YAF Language Setting
        /// </summary>
        private static void SetDnnLangToYaf()
        {
            try
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;

                string langCode = currentCulture.Name;

                YafContext.Current.Get<YafBoardSettings>().Language =
                    YafCultures.Find(yafCult => yafCult.Culture.Equals(langCode)) != null
                        ? YafCultures.Find(yafCult => yafCult.Culture.Equals(langCode)).LanguageFile
                        : "english.xml";
            }
            catch (Exception)
            {
                YafContext.Current.Get<YafBoardSettings>().Language = "english.xml";
            }
        }

        /// <summary>
        /// The create new board.
        /// </summary>
        /// <param name="dnnUserInfo">
        /// The dnn user info.
        /// </param>
        /// <param name="dnnUser">
        /// The dnn user.
        /// </param>
        private void CreateNewBoard(UserInfo dnnUserInfo, MembershipUser dnnUser)
        {
            // Add new admin users to group
            /*if (!RoleMembershipHelper.IsUserInRole(dnnUserInfo.Username, this.portalSettings.AdministratorRoleName))
            {
                try
                {
                    RoleMembershipHelper.AddUserToRole(dnnUserInfo.Username, this.portalSettings.AdministratorRoleName);
                }
                catch
                {
                    // TODO :Dont do anything when user is already in role ?!
                }
            }*/

            if (dnnUserInfo.IsSuperUser)
            {
                // This is HOST and probably the first board.
                // The install routine already created the first board.
                // Make sure Module settings are in place
                ModuleController objForumSettings = new ModuleController();
                objForumSettings.UpdateModuleSetting(this.ModuleId, "forumboardid", "1");
                objForumSettings.UpdateModuleSetting(this.ModuleId, "forumcategoryid", string.Empty);
                this.forum1.BoardID = 1;
            }
            else
            {
                // This is an admin adding a new forum.
                string newBoardName = "{0} Forums".FormatWith(this.PortalSettings.PortalName);

                // Create the board
                YafCultureInfo yafCultureInfo = GetYafCultureInfo(
                    Localization.GetPageLocale(this.CurrentPortalSettings));

                int largestBoardId = LegacyDb.board_create(
                    dnnUserInfo.Username,
                    dnnUserInfo.Email,
                    dnnUser.ProviderUserKey,
                    newBoardName,
                    yafCultureInfo.Culture,
                    yafCultureInfo.LanguageFile,
                    "DotNetNuke",
                    "DotNetNuke",
                    string.Empty,
                    dnnUserInfo.IsSuperUser);

                // Assign the new forum to this module
                ModuleController objForumSettings = new ModuleController();

                objForumSettings.UpdateModuleSetting(this.ModuleId, "forumboardid", largestBoardId.ToString());
                objForumSettings.UpdateModuleSetting(this.ModuleId, "forumcategoryid", string.Empty);

                this.forum1.BoardID = largestBoardId;
            }
        }

        /// <summary>
        /// The create yaf host user.
        /// </summary>
        /// <param name="yafUserId">
        /// The yaf user id.
        /// </param>
        private void CreateYafHostUser(int yafUserId)
        {
            // get this user information...
            DataTable userInfo = LegacyDb.user_list(this.forum1.BoardID, yafUserId, null, null, null);

            if (userInfo.Rows.Count <= 0)
            {
                return;
            }

            DataRow row = userInfo.Rows[0];

            if (Convert.ToBoolean(row["IsHostAdmin"]))
            {
                return;
            }

            // fix the ishostadmin flag...
            UserFlags userFlags = new UserFlags(row["Flags"]) { IsHostAdmin = true };

            // update...
            LegacyDb.user_adminsave(
                this.forum1.BoardID,
                yafUserId,
                row["Name"],
                row["DisplayName"],
                row["Email"],
                userFlags.BitValue,
                row["RankID"]);
        }        

        /// <summary>
        /// The dotnetnuke module load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DotNetNukeModule_Load(object sender, EventArgs e)
        {
            if (this.Page.IsPostBack)
            {
                return;
            }

            // Check for user
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return;
            }

            // Check if Yaf Profile exists as dnn profile Definition
            if (this.Session["{0}_profileproperties".FormatWith(this.CurrentPortalSettings.PortalId)] == null)
            {
                if (
                    ProfileController.GetPropertyDefinitionsByCategory(
                        this.CurrentPortalSettings.PortalId, "YAF Profile").Count == 0)
                {
                    Profile.AddYafProfileDefinitions(this.CurrentPortalSettings.PortalId);
                }
                else
                {
                    this.Session["{0}_profileproperties".FormatWith(this.CurrentPortalSettings.PortalId)] = true;
                }
            }

            try
            {
                this.VerifyUser();
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        /// <summary>
        /// Check if roles are syncronized and the user is added to them
        /// </summary>
        /// <param name="dnnUser">The Current DNN User</param>
        /// <param name="yafUserId">The yaf user id.</param>
        private void CheckForRoles(UserInfo dnnUser, int yafUserId)
        {
           // see if the roles have been syncronized...
            /*if (this.Session["{0}_rolesloaded".FormatWith(this.SessionUserKeyName)] != null)
            {
                return;
            }*/

            var yafBoardRoles = Data.GetYafBoardRoles(this.forum1.BoardID);

            var yafUserRoles = Data.GetYafUserRoles(this.forum1.BoardID, yafUserId);

            var roleController = new RoleController();

            foreach (var role in roleController.GetRolesByUser(dnnUser.UserID, this.CurrentPortalSettings.PortalId))
            {
                RoleInfo yafRoleFound = null;

                var updateRole = false;

                // First Create role in yaf if not exists
                if (!yafBoardRoles.Any(yafRoleA => yafRoleA.RoleName.Equals(role)))
                {
                    // If not Create Role in YAF
                    yafRoleFound = new RoleInfo
                        {
                            RoleID = (int)RoleSyncronizer.CreateYafRole(role, this.forum1.BoardID), 
                            RoleName = role
                        };

                    updateRole = true;
                }
                else
                {
                    if (!yafUserRoles.Any(yafRoleC => yafRoleC.RoleName.Equals(role)))
                    {
                        yafRoleFound = yafBoardRoles.Find(yafRole => yafRole.RoleName.Equals(role));
                        updateRole = true;
                    }
                }

                if (updateRole && yafRoleFound != null)
                {
                    // add/remove user to yaf role ?!
                    RoleSyncronizer.UpdateUserRole(yafRoleFound, yafUserId, dnnUser.Username, true);
                }
            }

            this.Session["{0}_rolesloaded".FormatWith(this.SessionUserKeyName)] = true;
        }

        /// <summary>
        /// Check if the DNN User exists in yaf, and if the Profile is up to date.
        /// </summary>
        private void VerifyUser()
        {
            // Get current Dnn user (DNN 5)
            var dnnUserInfo = UserController.GetCurrentUserInfo();

            // get the user from the membership provider
            MembershipUser dnnUser = Membership.GetUser(dnnUserInfo.Username, true);

            if (dnnUser == null)
            {
                return;
            }

            // Admin or Host user?
            if ((dnnUserInfo.IsSuperUser || dnnUserInfo.UserID == this.portalSettings.AdministratorId) &&
                this.createNewBoard)
            {
                this.CreateNewBoard(dnnUserInfo, dnnUser);
            }

            int yafUserId;

            try
            {
                yafUserId = LegacyDb.user_get(this.forum1.BoardID, dnnUser.ProviderUserKey);

                if (yafUserId > 0)
                {
                    this.Session["{0}_userSync".FormatWith(this.SessionUserKeyName)] = true;
                }
            }
            catch (Exception)
            {
                yafUserId = 0;
                this.Session["{0}_userSync".FormatWith(this.SessionUserKeyName)] = null;
            }

            this.CheckForRoles(dnnUserInfo, yafUserId);

            // Load Auto Sync Setting
            bool autoSyncProfile = true;

            if ((string)this.Settings["AutoSyncProfile"] != null)
            {
                bool.TryParse((string)this.Settings["AutoSyncProfile"], out autoSyncProfile);
            }

            if (yafUserId > 0 && autoSyncProfile)
            {
                ProfileSyncronizer.UpdateUserProfile(
                    yafUserId, dnnUserInfo, dnnUser, this.CurrentPortalSettings.PortalId, this.forum1.BoardID);
            }

            // Has this user been registered in YAF already?);
            if (this.Session["{0}_userSync".FormatWith(this.SessionUserKeyName)] != null)
            {
                return;
            }

            if (yafUserId == 0)
            {
                yafUserId = UserImporter.CreateYafUser(dnnUserInfo, dnnUser, this.forum1.BoardID, this.portalSettings);
            }

            // super admin check...
            if (dnnUserInfo.IsSuperUser)
            {
                this.CreateYafHostUser(yafUserId);
            }

            if (YafContext.Current.Settings != null)
            {
                RoleMembershipHelper.UpdateForumUser(dnnUser, YafContext.Current.Settings.BoardID);
            }

            YafContext.Current.Get<IDataCache>().Clear();

            DataCache.ClearPortalCache(this.CurrentPortalSettings.PortalId, true);

            this.Session.Clear();
            this.Response.Redirect(Globals.NavigateURL(), true);
        }

        /// <summary>
        /// Change Page Title
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Forum1_PageTitleSet(object sender, ForumPageTitleArgs e)
        {
            int removeTabName = 1;

            if (this.Settings["RemoveTabName"] != null)
            {
                removeTabName = this.Settings["RemoveTabName"].ToType<int>();
            }

            // Check if Tab Name/Title Matches the Forum Board Name
            if (removeTabName.Equals(2))
            {
                if (YafContext.Current.Get<YafBoardSettings>().Name.Equals(this.CurrentPortalSettings.ActiveTab.TabName))
                {
                    removeTabName = 1;
                }

                if (this.CurrentPortalSettings.ActiveTab.Title.IsSet())
                {
                    if (
                        YafContext.Current.Get<YafBoardSettings>().Name.Equals(
                            this.CurrentPortalSettings.ActiveTab.Title))
                    {
                        removeTabName = 1;
                    }
                }
            }

            // Remove Tab Title(Name) if already in the Page Title
            if (removeTabName.Equals(1))
            {
                this.BasePage.Title =
                    this.BasePage.Title.Replace(
                        "> {0}".FormatWith(this.CurrentPortalSettings.ActiveTab.TabName), string.Empty);
            }

            /*this.BasePage.Title =
                this.BasePage.Title.Replace(
                    "> {0}".FormatWith(
                        !string.IsNullOrEmpty(this.CurrentPortalSettings.ActiveTab.Title)
                            ? this.CurrentPortalSettings.ActiveTab.Title
                            : this.CurrentPortalSettings.ActiveTab.TabName),
                    string.Empty);*/

            // Remove Portal Name if already in the Page Title
            /*this.BasePage.Title = this.BasePage.Title.Replace(
                   this.CurrentPortalSettings.PortalName, string.Empty);*/
        }

        /// <summary>
        /// The initialize component.
        /// </summary>
        private void InitializeComponent()
        {
            if (AJAX.IsInstalled())
            {
                AJAX.RegisterScriptManager();
            }

            this.forum1 = new Forum();

            this.pnlModuleContent.Controls.Add(this.forum1);

            this.Load += this.DotNetNukeModule_Load;
            this.forum1.PageTitleSet += this.Forum1_PageTitleSet;

            // Get current BoardID
            try
            {
                this.createNewBoard = false;

                // This will create an error if there is no setting for forumboardid
                this.forum1.BoardID = int.Parse(this.Settings["forumboardid"].ToString());

                // Inherit Language from Dnn?
                bool ineritDnnLang = true;

                if ((string)this.Settings["InheritDnnLanguage"] != null)
                {
                    bool.TryParse((string)this.Settings["InheritDnnLanguage"], out ineritDnnLang);
                }

                if (ineritDnnLang)
                {
                    SetDnnLangToYaf();
                }

                string categoryId = this.Settings["forumcategoryid"].ToString();

                if (!string.IsNullOrEmpty(categoryId))
                {
                    this.forum1.CategoryID = int.Parse(categoryId);
                }

                // Override Theme?
                bool overrideTheme = false;

                if ((string)this.Settings["OverrideTheme"] != null)
                {
                    bool.TryParse((string)this.Settings["OverrideTheme"], out overrideTheme);
                }

                if (overrideTheme)
                {
                    var forumThemeFile = this.Settings["forumtheme"].ToString();

                    if (!string.IsNullOrEmpty(forumThemeFile))
                    {
                        YafContext.Current.Page["ForumTheme"] = forumThemeFile;
                    }
                }
            }
            catch (Exception)
            {
                // A forum does not exist for this module
                // Create a new board
                this.createNewBoard = true;

                // forum1.BoardID = 1;
            }
        }

        #endregion
    }
}