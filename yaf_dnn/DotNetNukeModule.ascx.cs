using System;
using System.Data;
using System.Web;
using System.Web.Security;
using DotNetNuke;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace yaf_dnn
{
	/// <summary>
	/// Summary description for DotNetNukeModule.
	/// </summary>
	public partial class DotNetNukeModule : DotNetNuke.Entities.Modules.PortalModuleBase, IActionable
	{
		private bool createNewBoard = false;

		DotNetNuke.Entities.Portals.PortalSettings _portalSettings = null;
		DotNetNuke.Entities.Portals.PortalSettings CurrentPortalSettings
		{
			get
			{
				if (_portalSettings == null)
				{
					_portalSettings = (DotNetNuke.Entities.Portals.PortalSettings)HttpContext.Current.Items["PortalSettings"];
				}

				return _portalSettings;
			}
		}

		public string SessionUserKeyName
		{
			get
			{
				return String.Format("yaf_dnn_boardid{0}_userid{1}_portalid{2}", Forum1.BoardID, this.UserId, CurrentPortalSettings.PortalId);
			}
		}

		private void DotNetNukeModule_Load(object sender, System.EventArgs e)
		{
			// Check for user
			if (HttpContext.Current.User.Identity.IsAuthenticated)
			{
				//Get current Dnn user
				DotNetNuke.Entities.Users.UserInfo DnnUserInfo = DotNetNuke.Entities.Users.UserController.GetUser(CurrentPortalSettings.PortalId, this.UserId, false);

				// see if the roles have been syncronized...
				if (Session[SessionUserKeyName + "_rolesloaded"] == null)
				{
					//Get roles from DNN for this user and put into aspnet role tables
					foreach (string _rolename in DnnUserInfo.Roles)
						if (_rolename != null)
						{
							//Create role if not existing
							if (!Roles.RoleExists(_rolename))
							{
								Roles.CreateRole(_rolename);

							}
							//Add user to role if not already there
							if (!Roles.IsUserInRole(DnnUserInfo.Username, _rolename))
							{
								Roles.AddUserToRole(DnnUserInfo.Username, _rolename);
							}
						}

					Session[SessionUserKeyName + "_rolesloaded"] = true;
				}

				//get the user from the membership provider
				//Need the user provider user key
				MembershipUser DnnUser = Membership.GetUser(DnnUserInfo.Username, true);

				//Admin or Host user?
				if ((DnnUserInfo.IsSuperUser || DnnUserInfo.UserID == _portalSettings.AdministratorId) && createNewBoard)
				{
					CreateNewBoard(DnnUserInfo, DnnUser);
				}

				// Has this user been registered in YAF already?
				if (Session[SessionUserKeyName + "_userSync"] == null)
				{
					int yafUserId = 0;

					try
					{
						yafUserId = DB.user_get(Forum1.BoardID, DnnUser.ProviderUserKey);
					}
					catch (Exception)
					{
						// create the user in the YAF DB so profile can ge created...
						int? userID = RoleMembershipHelper.CreateForumUser(DnnUser, Forum1.BoardID);

						// create profile
						YafUserProfile userProfile = YafContext.Current.GetProfile(DnnUserInfo.Username);
						// setup their inital profile information
						userProfile.RealName = DnnUserInfo.Profile.FullName;
						userProfile.Location = DnnUserInfo.Profile.Country;
						userProfile.Homepage = DnnUserInfo.Profile.Website;
						userProfile.Save();

						yafUserId = UserMembershipHelper.GetUserIDFromProviderUserKey(DnnUser.ProviderUserKey);

						// save the time zone...
						YAF.Classes.Data.DB.user_save(yafUserId, Forum1.BoardID, null, null, Convert.ToInt32(DnnUserInfo.Profile.TimeZone), null, null, null, null, null);
					}

					// super admin check...
					if (DnnUserInfo.IsSuperUser)
					{
						// get this user information...
						DataTable userInfo = DB.user_list(Forum1.BoardID, yafUserId, null, null, null);

						if (userInfo.Rows.Count > 0)
						{
							DataRow row = userInfo.Rows[0];

							if (Convert.ToBoolean(row["IsHostAdmin"]) == false)
							{
								// fix the ishostadmin flag...
								UserFlags userFlags = new UserFlags(row["Flags"]);
								userFlags.IsHostAdmin = true;

								// update...
								DB.user_adminsave(Forum1.BoardID, yafUserId, row["Name"], row["Email"], userFlags.BitValue, row["RankID"]);
							}
						}
					}

					Session[SessionUserKeyName + "_userSync"] = true;
				}
			}
		}

		private void CreateNewBoard(DotNetNuke.Entities.Users.UserInfo DnnUserInfo, MembershipUser DnnUser)
		{
			// Add new admin users to group
			if (!Roles.IsUserInRole(DnnUserInfo.Username, "Administrators"))
			{
				Roles.AddUserToRole(DnnUserInfo.Username, "Administrators");
			}

			if (DnnUserInfo.IsSuperUser)
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

		void Forum1_PageTitleSet(object sender, YAF.Classes.Base.ForumPageArgs e)
		{
			BasePage.Title = e.Title + " - " + BasePage.Title;

		}

		override protected void OnInit(EventArgs e)
		{
			Load += new EventHandler(DotNetNukeModule_Load);
			Forum1.PageTitleSet += new EventHandler<YAF.Classes.Base.ForumPageArgs>(Forum1_PageTitleSet);
			//Get current BoardID
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
			//sync roles from DNN to YAF

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
