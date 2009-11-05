#region Usings

using System;
using System.Data;
using System.Web;
using System.Web.Security;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using YAF;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

#endregion

namespace yaf_dnn
{
    // TODO: forum setup
    /// <summary>
    /// Summary description for DotNetNukeModule.
    /// </summary>
    public partial class DotNetNukeModule : PortalModuleBase, IActionable
    {
        private bool _createNewBoard;
        private PortalSettings _portalSettings;

        private PortalSettings CurrentPortalSettings
        {
            get
            {
                if (this._portalSettings == null)
                    this._portalSettings = PortalController.GetCurrentPortalSettings();

                return this._portalSettings;
            }
        }
        
        public string SessionUserKeyName
        {
            get
            {
                return String.Format("yaf_dnn_boardid{0}_userid{1}_portalid{2}",
                                     this.Forum1.BoardID,
                                     UserId,
                                     CurrentPortalSettings.PortalId);
            }
        }

        public CDefault BasePage
        {
            get { return (CDefault) Page; }
        }

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                //Change
                //actions.Add(GetNextActionID(), "Edit YAF Settings", ModuleActionType.AddContent, String.Empty, String.Empty, EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                //actions.Add(GetNextActionID(), "Edit YAF Settings", ModuleActionType.AddContent, string.Empty, string.Empty, EditUrl(), false, SecurityAccessLevel.Host, true, false);

                actions.Add(GetNextActionID(),
                            "Edit YAF Settings",
                            ModuleActionType.AddContent,
                            String.Empty,
                            String.Empty,
                            EditUrl(),
                            false,
                            SecurityAccessLevel.Host,
                            true,
                            false);
               

                return actions;
            }
        }

        #endregion

        private void DotNetNukeModule_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
                return;

            // Check for user
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return;

            try
            {
                //Get current Dnn user
                UserInfo dnnUserInfo = UserController.GetUserById(CurrentPortalSettings.PortalId, UserId);

                //get the user from the membership provider
                MembershipUser dnnUser = Membership.GetUser(dnnUserInfo.Username, true);

                if (dnnUser == null)
                    return;

                // see if the roles have been syncronized...
                if (Session[SessionUserKeyName + "_rolesloaded"] == null)
                {
                    bool roleChanged = false;
                    foreach (string role in dnnUserInfo.Roles)
                    {
                        if (!Roles.RoleExists(role))
                        {
                            Roles.CreateRole(role);
                            roleChanged = true;
                        }

                        if (!Roles.IsUserInRole(dnnUserInfo.Username, role))
                            Roles.AddUserToRole(dnnUserInfo.Username, role);
                    }

                    if (roleChanged)
                        MarkRolesChanged();

                    //if (YafContext.Current.Settings != null && YafContext.Current.PageUserID > 0 &&
                    //    !UserMembershipHelper.IsGuestUser(YafContext.Current.PageUserID))
                    //    RoleMembershipHelper.UpdateForumUser(dnnUser, YafContext.Current.Settings.BoardID);

                    Session[SessionUserKeyName + "_rolesloaded"] = true;
                }

                //Admin or Host user?
                if ((dnnUserInfo.IsSuperUser || dnnUserInfo.UserID == this._portalSettings.AdministratorId) &&
                    this._createNewBoard)
                    CreateNewBoard(dnnUserInfo, dnnUser);

                int yafUserId;

                try
                {
                    yafUserId = DB.user_get(this.Forum1.BoardID, dnnUser.ProviderUserKey);
                    if (yafUserId > 0)
                        Session[SessionUserKeyName + "_userSync"] = true;
                }
                catch (Exception)
                {
                    yafUserId = 0;
                    Session[SessionUserKeyName + "_userSync"] = null;
                }

                // Has this user been registered in YAF already?
                if (Session[SessionUserKeyName + "_userSync"] != null)
                    return;

                if (yafUserId == 0)
                    yafUserId = CreateYafUser(dnnUserInfo, dnnUser);

                // super admin check...
                if (dnnUserInfo.IsSuperUser)
                    CreateYafHostUser(yafUserId);

                if (YafContext.Current.Settings != null)
                    //RoleMembershipHelper.UpdateForumUser(dnnUser, YafContext.Current.Settings.BoardID);
                    RoleMembershipHelper.UpdateForumUser(dnnUser, this.Forum1.BoardID);

                                
                YafContext.Current.Cache.Clear();
                DataCache.ClearCache();
                Session.Clear();
                Response.Redirect(Globals.NavigateURL(), true);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        private static void MarkRolesChanged()
        {
            RolePrincipal rolePrincipal;
            if (Roles.CacheRolesInCookie)
            {
                string roleCookie = "";

                HttpCookie cookie = HttpContext.Current.Request.Cookies[Roles.CookieName];
                if (cookie != null)
                    roleCookie = cookie.Value;

                rolePrincipal = new RolePrincipal(HttpContext.Current.User.Identity, roleCookie);
            }
            else
                rolePrincipal = new RolePrincipal(HttpContext.Current.User.Identity);

            rolePrincipal.SetDirty();
        }

        private void CreateYafHostUser(int yafUserId)
        {
            // get this user information...
            DataTable userInfo = DB.user_list(this.Forum1.BoardID, yafUserId, null, null, null);

            if (userInfo.Rows.Count <= 0)
                return;

            DataRow row = userInfo.Rows[0];

            if (Convert.ToBoolean(row["IsHostAdmin"]))
                return;

            // fix the ishostadmin flag...
            UserFlags userFlags = new UserFlags(row["Flags"]);
            userFlags.IsHostAdmin = true;

            // update...
            DB.user_adminsave(this.Forum1.BoardID,
                              yafUserId,
                              row["Name"],
                              row["Email"],
                              userFlags.BitValue,
                              row["RankID"]);
        }

        private int CreateYafUser(UserInfo dnnUserInfo, MembershipUser dnnUser)
        {
            YafContext.Current.Cache.Clear();
            // setup roles
            //RoleMembershipHelper.SetupUserRoles(this.Forum1.BoardID, dnnUser.UserName);

            // create the user in the YAF DB so profile can ge created...
            int? userID = RoleMembershipHelper.CreateForumUser(dnnUser, this.Forum1.BoardID);
            if (userID == null)
                return 0;

            // create profile
            YafUserProfile userProfile = YafUserProfile.GetProfile(dnnUser.UserName);
            // setup their inital profile information
            userProfile.Initialize(dnnUser.UserName, true);
            userProfile.RealName = dnnUserInfo.Profile.FullName;
            userProfile.Location = dnnUserInfo.Profile.Country;
            userProfile.Homepage = dnnUserInfo.Profile.Website;
            userProfile.Save();
            


            int yafUserId = UserMembershipHelper.GetUserIDFromProviderUserKey(dnnUser.ProviderUserKey);

            // save the time zone...
            DB.user_save(yafUserId,
                         this.Forum1.BoardID,
                         null,
                         null,
                         GetUserTimeZoneOffset(dnnUserInfo, CurrentPortalSettings),
                         null,
                         null,
                         null,
                         null,
                         null);

            return yafUserId;
        }

        private void CreateNewBoard(UserInfo dnnUserInfo, MembershipUser dnnUser)
        {
            // Add new admin users to group
            if (!Roles.IsUserInRole(dnnUserInfo.Username, "Administrators"))
                Roles.AddUserToRole(dnnUserInfo.Username, "Administrators");

            if (dnnUserInfo.IsSuperUser)
            {
                //This is HOST and probably the first board.
                //The install routine already created the first board.
                //Make sure Module settings are in place
                ModuleController objForumSettings = new ModuleController();
                objForumSettings.UpdateModuleSetting(ModuleId, "forumboardid", "1");
                objForumSettings.UpdateModuleSetting(ModuleId, "forumcategoryid", string.Empty);
                this.Forum1.BoardID = 1;
            }
            else
            {
                // This is an admin adding a new forum.

                string newBoardName = string.Format("New Forum - Module {0}", ModuleId);
                //Create the board
                DB.board_create(dnnUserInfo.Username, dnnUser.ProviderUserKey, newBoardName, "DotNetNuke", "DotNetNuke");

                //The newly created board will be the last one in the DB and have the highest BoardID
                DataTable tbl = DB.board_list(null);
                int largestBoardID = 0;
                foreach (DataRow row in tbl.Rows)
                {
                    if (Convert.ToInt32(row["BoardID"]) > largestBoardID)
                        largestBoardID = Convert.ToInt32(row["BoardID"]);
                }
                //Assign the new forum to this module
                ModuleController objForumSettings = new ModuleController();
                objForumSettings.UpdateModuleSetting(ModuleId, "forumboardid", largestBoardID.ToString());
                objForumSettings.UpdateModuleSetting(ModuleId, "forumcategoryid", string.Empty);
                this.Forum1.BoardID = largestBoardID;
            }
        }

        private void Forum1_PageTitleSet(object sender, ForumPageTitleArgs e)
        {
            BasePage.Title = e.Title + " - " + BasePage.Title;
        }

        protected override void OnInit(EventArgs e)
        {
            Load += DotNetNukeModule_Load;
            this.Forum1.PageTitleSet += Forum1_PageTitleSet;
            //Get current BoardID
            try
            {
                this._createNewBoard = false;
                // This will create an error if there is no setting for forumboardid
                this.Forum1.BoardID = int.Parse(Settings["forumboardid"].ToString());

                string cID = Settings["forumcategoryid"].ToString();
                if (cID != string.Empty)
                    this.Forum1.CategoryID = int.Parse(cID);
            }
            catch (Exception)
            {
                //A forum does not exist for this module
                //Create a new board
                this._createNewBoard = true;
                //Forum1.BoardID = 1;
            }

            base.OnInit(e);
        }

        protected override void OnError(EventArgs e)
        {
            Exception x = Server.GetLastError();
            DB.eventlog_create(YafContext.Current.PageUserID, this, x);
            base.OnError(e);
        }

        private static int GetUserTimeZoneOffset(UserInfo userInfo, PortalSettings portalSettings)
        {
            int timeZone;
            if ((userInfo != null) && (userInfo.UserID != Null.NullInteger))
                timeZone = userInfo.Profile.TimeZone;
            else
                timeZone = portalSettings.TimeZoneOffset;
            return timeZone;
        }
    }
}