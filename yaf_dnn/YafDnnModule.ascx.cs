#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
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
using DotNetNuke.Services.Localization;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;


#endregion

namespace YAF.DotNetNuke
{
    /// <summary>
    /// Summary description for DotNetNukeModule.
    /// </summary>
    public partial class YafDnnModule : PortalModuleBase, IActionable
    {
        private bool _createNewBoard;
        private PortalSettings _portalSettings;
        
        private Forum forum1;

        private PortalSettings CurrentPortalSettings
        {
            get { return _portalSettings ?? (_portalSettings = PortalController.GetCurrentPortalSettings()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SessionUserKeyName
        {
            get
            {
                return String.Format("yaf_dnn_boardid{0}_userid{1}_portalid{2}",
                                     forum1.BoardID,
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
                ModuleActionCollection actions = new ModuleActionCollection
                                                     {
                                                         {
                                                             GetNextActionID(), "Edit YAF Settings",
                                                             ModuleActionType.AddContent, String.Empty, String.Empty,
                                                             EditUrl(), false, SecurityAccessLevel.Host, true, false
                                                             },
                                                         {
                                                             GetNextActionID(), "DNN User Importer",
                                                             ModuleActionType.AddContent, String.Empty, String.Empty,
                                                             EditUrl("Import"), false, SecurityAccessLevel.Host, true,
                                                             false
                                                             }
                                                     };

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
            {
                return;
            }
            
            try
            {
                // Get current Dnn user (DNN 4)
                 UserInfo dnnUserInfo = UserController.GetUser(CurrentPortalSettings.PortalId, UserId, false);
                    
                // Get current Dnn user (DNN 5)
                 //UserInfo dnnUserInfo= UserController.GetUserById(CurrentPortalSettings.PortalId, UserId);  

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

                    Session[SessionUserKeyName + "_rolesloaded"] = true;
                }

                //Admin or Host user?
                if ((dnnUserInfo.IsSuperUser || dnnUserInfo.UserID == _portalSettings.AdministratorId) &&
                    _createNewBoard)
                    CreateNewBoard(dnnUserInfo, dnnUser);

                int yafUserId;

                try
                {
                    yafUserId = DB.user_get(forum1.BoardID, dnnUser.ProviderUserKey);
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
                    RoleMembershipHelper.UpdateForumUser(dnnUser, YafContext.Current.Settings.BoardID);

                YafContext.Current.Cache.Clear();

                DataCache.ClearPortalCache(_portalSettings.PortalId, true);

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
            DataTable userInfo = DB.user_list(forum1.BoardID, yafUserId, null, null, null);

            if (userInfo.Rows.Count <= 0)
                return;

            DataRow row = userInfo.Rows[0];

            if (Convert.ToBoolean(row["IsHostAdmin"]))
                return;

            // fix the ishostadmin flag...
            UserFlags userFlags = new UserFlags(row["Flags"]) {IsHostAdmin = true};

            // update...
            DB.user_adminsave(forum1.BoardID,
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
            //RoleMembershipHelper.SetupUserRoles(forum1.BoardID, dnnUser.UserName);

            // create the user in the YAF DB so profile can ge created...
            int? userId = RoleMembershipHelper.CreateForumUser(dnnUser, forum1.BoardID);

            if (userId == null)
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

            // Save User
            DB.user_save(yafUserId, 
                         forum1.BoardID,
                        null, null, null, GetUserTimeZoneOffset(dnnUserInfo, PortalSettings), null, null, null, null, null, null, null, null, null, null);

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
                forum1.BoardID = 1;
            }
            else
            {
                // This is an admin adding a new forum.

                string newBoardName = string.Format("New Forum - Module {0}", ModuleId);

                //Create the board
                
                YafCultureInfo yafCultureInfo = GetYafCultureInfo(Localization.GetPageLocale(CurrentPortalSettings));

                int largestBoardId = DB.board_create(
                    dnnUserInfo.Username,
                    dnnUserInfo.Email,
                    dnnUser.ProviderUserKey,
                    newBoardName,
                    yafCultureInfo.sCulture,
                                yafCultureInfo.sLanguageFile,
                    "DotNetNuke",
                    "DotNetNuke");

                //Assign the new forum to this module
                ModuleController objForumSettings = new ModuleController();
                objForumSettings.UpdateModuleSetting(ModuleId, "forumboardid", largestBoardId.ToString());
                objForumSettings.UpdateModuleSetting(ModuleId, "forumcategoryid", string.Empty);
                forum1.BoardID = largestBoardId;
            }
        }

        /*private void Forum1_PageTitleSet(object sender, ForumPageTitleArgs e)
        {
           BasePage.Title = e.Title + " - " + BasePage.Title;
        }*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();            

            base.OnInit(e);
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitializeComponent()
        {
            if (AJAX.IsInstalled())
            {
                AJAX.RegisterScriptManager();
            }

            forum1 = new Forum();

            pnlModuleContent.Controls.Add(forum1);
            
            Load += DotNetNukeModule_Load;
            //forum1.PageTitleSet += Forum1_PageTitleSet;

            //Get current BoardID
            try
            {
                _createNewBoard = false;
                // This will create an error if there is no setting for forumboardid
                forum1.BoardID = int.Parse(Settings["forumboardid"].ToString());

                SetDnnLangToYaf();

                string cId = Settings["forumcategoryid"].ToString();

                if (cId != string.Empty)
                    forum1.CategoryID = int.Parse(cId);
            }
            catch (Exception)
            {
                //A forum does not exist for this module
                //Create a new board
                _createNewBoard = true;
                //forum1.BoardID = 1;
            }
        }

        /// <summary>
        /// Change YAF Language based on DNN Language, 
        /// this will override the YAF Language Setting 
        /// </summary>
        private static void SetDnnLangToYaf()
        {
            try
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;

                string sLangCode = currentCulture.TwoLetterISOLanguageName;

                switch (sLangCode)
                {
                    case "de":
                        YafContext.Current.BoardSettings.Language = "german-du.xml";
                        break;
                    case "zh":
                        YafContext.Current.BoardSettings.Language = "china.xml";
                        break;
                    case "cz":
                        YafContext.Current.BoardSettings.Language = "czech.xml";
                        break;
                    case "dk":
                        YafContext.Current.BoardSettings.Language = "danish.xml";
                        break;
                    case "nl":
                        YafContext.Current.BoardSettings.Language = "dutch.xml";
                        break;
                    case "fi":
                        YafContext.Current.BoardSettings.Language = "finnish.xml";
                        break;
                    case "fr":
                        YafContext.Current.BoardSettings.Language = "french.xml";
                        break;
                    case "he":
                        YafContext.Current.BoardSettings.Language = "hebrew.xml";
                        break;
                    case "it":
                        YafContext.Current.BoardSettings.Language = "italian.xml";
                        break;
                    case "lt":
                        YafContext.Current.BoardSettings.Language = "lithuanian.xml";
                        break;
                    case "nr":
                        YafContext.Current.BoardSettings.Language = "norwegian.xml";
                        break;
                    case "fa":
                        YafContext.Current.BoardSettings.Language = "persian.xml";
                        break;
                    case "pl":
                        YafContext.Current.BoardSettings.Language = "polish.xml";
                        break;
                    case "pt":
                        YafContext.Current.BoardSettings.Language = "portugues.xml";
                        break;
                    case "ro":
                        YafContext.Current.BoardSettings.Language = "romanian.xml";
                        break;
                    case "ru":
                        YafContext.Current.BoardSettings.Language = "russian.xml";
                        break;
                    case "sk":
                        YafContext.Current.BoardSettings.Language = "slovak.xml";
                        break;
                    case "es":
                        YafContext.Current.BoardSettings.Language = "spanish.xml";
                        break;
                    case "sv":
                        YafContext.Current.BoardSettings.Language = "swedish.xml";
                        break;
                    case "tr":
                        YafContext.Current.BoardSettings.Language = "turkish.xml";
                        break;
                    case "vi":
                        YafContext.Current.BoardSettings.Language = "vietnam.xml";
                        break;
                    default:
                        YafContext.Current.BoardSettings.Language = "english.xml";
                        break;
                }
            }
            catch (Exception)
            {
                YafContext.Current.BoardSettings.Language = "english.xml";
            }
            
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
        
        private static Dictionary<string, string> GetYafCultures()
        {
            Dictionary<string, string> yafCultures = new Dictionary<string, string>();
            DataTable cult = StaticDataHelper.Cultures();

            foreach (DataRow row in cult.Rows)
            {
                try
                {
                    yafCultures.Add(row["CultureTag"].ToString(), row["CultureFile"].ToString());
                }
                catch
                {
                    continue;
                }
            }

            if (yafCultures.Count == 0)
            {
                yafCultures.Add("en", "english.xml");
            }

            return yafCultures;
        }

        private static YafCultureInfo GetYafCultureInfo(CultureInfo cultureInfo)
        {
            string culture = "en";
            string lngFile = "english.xml";
            Dictionary<string, string> cultures = GetYafCultures();
            YafCultureInfo yafCultureInfo = new YafCultureInfo();

            if (cultureInfo != null)
            {
                if (cultures.ContainsKey(cultureInfo.TwoLetterISOLanguageName))
                {
                    culture = cultureInfo.TwoLetterISOLanguageName;
                    lngFile = cultures[cultureInfo.TwoLetterISOLanguageName];
                }
                else if (cultures.ContainsKey(cultureInfo.Name))
                {
                    culture = cultureInfo.Name;
                    lngFile = cultures[cultureInfo.Name];
                }
            }

            yafCultureInfo.sCulture = culture;
            yafCultureInfo.sLanguageFile = lngFile;

            return yafCultureInfo;
        }

        #region Nested type: YafCultureInfo

        private struct YafCultureInfo
        {
            public string sCulture;
            public string sLanguageFile;
        }

        #endregion
    }
}