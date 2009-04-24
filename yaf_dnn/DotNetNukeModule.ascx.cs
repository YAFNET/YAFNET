using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Services.Search;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Exceptions;
using YAF.Classes;
using YAF.Classes.Data;
using YAF.Classes.Base;

namespace yaf_dnn
{
/// <summary>
    ///        Summary description for DotNetNukeModule.
    /// </summary>
    public partial class DotNetNukeModule : DotNetNuke.Entities.Modules.PortalModuleBase, IActionable
    {
        private bool m_isSuperAdmin;
        private bool createNewBoard = false;

        private void DotNetNukeModule_Load(object sender, System.EventArgs e)
        {
            try
            {
                createNewBoard = false;
                // This will create an error if there is no setting for forumboardid
                Forum1.BoardID = int.Parse(Settings["forumboardid"].ToString());

                string cID = Settings["forumcategoryid"].ToString();
                if (cID != string.Empty)
                    Forum1.CategoryID = int.Parse(cID);
            }
            catch (Exception)
            {
                //A forum does not exist for this module
                //Create a new board
                createNewBoard = true;
                //Forum1.BoardID = 1;
            }
            
            // Check for user
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                DotNetNuke.Entities.Users.UserInfo DnnUserInfo;
                //Get current portal settings
                DotNetNuke.Entities.Portals.PortalSettings _portalSettings = (DotNetNuke.Entities.Portals.PortalSettings)HttpContext.Current.Items["PortalSettings"];
                //Get current user
                DnnUserInfo = DotNetNuke.Entities.Users.UserController.GetUser(_portalSettings.PortalId, this.UserId, false);
                
                //get the roles from dnn for this user and copy them to the aspnet roles if required.
                DotNetNuke.Security.Roles.RoleController _DnnRoleController = new DotNetNuke.Security.Roles.RoleController();
                ArrayList DnnRoleList;
                DnnRoleList = _DnnRoleController.GetUserRoles(_portalSettings.PortalId, DnnUserInfo.UserID);
                foreach (DotNetNuke.Entities.Users.UserRoleInfo rolenamelist in DnnRoleList)
                    {
                        if (!Roles.RoleExists(rolenamelist.RoleName))
                        {
                            Roles.CreateRole(rolenamelist.RoleName);
                            Roles.AddUserToRole(DnnUserInfo.Username, rolenamelist.RoleName);
                        }
                        else
                        {
                            if (!Roles.IsUserInRole(DnnUserInfo.Username, rolenamelist.RoleName))
                            {
                                Roles.AddUserToRole(DnnUserInfo.Username, rolenamelist.RoleName);
                            }
                        }
                    }
             
                //Admin or Host user?
                if (DnnUserInfo.UserID == _portalSettings.AdministratorId | DnnUserInfo.IsSuperUser)
                    {
                    //Do we have to create a new board
                    if (createNewBoard == true)
                    {
                        // Add new admin users to group
                        if (!Roles.IsUserInRole(DnnUserInfo.Username, "Administrators"))
                        {
                            Roles.AddUserToRole(DnnUserInfo.Username, "Administrators");
                        }

                        if (m_isSuperAdmin)
                        {
                            //This is HOST and probably the first board.
                            //The install routine already created the first board.
                            //Make sure Module settings are in place
                            DotNetNuke.Entities.Modules.ModuleController objForumSettings = new DotNetNuke.Entities.Modules.ModuleController();
                            objForumSettings.UpdateModuleSetting(ModuleId, "forumboardid", "1");
                            objForumSettings.UpdateModuleSetting(ModuleId, "forumcategoryid", string.Empty);
                            Forum1.BoardID = 1;
                        }
                        else
                        {
                            // This is an admin adding a new forum.

                            //get the user from the membership provider
                            //Need the user provider user key
                            MembershipUser DnnUser = Membership.GetUser(DnnUserInfo.Username, true);
                           
                            string newBoardName = "New Forum - Module " + ModuleId.ToString();
                            //Create the board
                            DB.board_create(DnnUserInfo.Username, DnnUser.ProviderUserKey, newBoardName, "DotNetNuke", "DotNetNuke");

                            //The newly created board will be the last one in the DB and have the highest BoardID
                            DataTable tbl = DB.board_list(null);
                            int largestBoardID = 0;
                            foreach (DataRow row in tbl.Rows)
                            {
                                if (Convert.ToInt32(row["BoardID"]) > largestBoardID)
                                {
                                    largestBoardID = Convert.ToInt32(row["BoardID"]);
                                }
                            }
                            //Assign the new forum to this module
                            DotNetNuke.Entities.Modules.ModuleController objForumSettings = new DotNetNuke.Entities.Modules.ModuleController();
                            objForumSettings.UpdateModuleSetting(ModuleId, "forumboardid", largestBoardID.ToString());
                            objForumSettings.UpdateModuleSetting(ModuleId, "forumcategoryid", string.Empty);
                            Forum1.BoardID = largestBoardID;
                        }
                    }
                }
            }
        }

        void Forum1_PageTitleSet(object sender, YAF.Classes.Base.ForumPageArgs e)
        {
            BasePage.Title = e.Title + " - " + BasePage.Title;
        }

        override protected void OnInit(EventArgs e)
        {
            Load += new EventHandler(DotNetNukeModule_Load);
            Forum1.PageTitleSet += new EventHandler<YAF.Classes.Base.ForumPageArgs>(Forum1_PageTitleSet);
            base.OnInit(e);
        }

        public DotNetNuke.Framework.CDefault BasePage
        {
            get
            {
                return (DotNetNuke.Framework.CDefault)this.Page;
            }
        }

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                //Change
                //actions.Add(GetNextActionID(), "Edit YAF Settings", ModuleActionType.AddContent, String.Empty, String.Empty, EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                actions.Add(GetNextActionID(), "Edit YAF Settings", ModuleActionType.AddContent, String.Empty, String.Empty, EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Host, true, false);
                return actions;
            }
        }

        #endregion
    }
}
