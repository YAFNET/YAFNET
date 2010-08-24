#region Usings

using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using System.IO;

#endregion

namespace YAF.DotNetNuke
{
    /// <summary>
    /// Summary description for DotNetNukeModule.
    /// </summary>
    public partial class YafDnnModuleImport : PortalModuleBase
    {

        private int iBoardId;
        private const string TypeFullName = "YAF.DotNetNuke.YafDnnImportScheduler, YAF.DotNetNuke.Module";

        private void DotNetNukeModuleImport_Load(object sender, EventArgs e)
        {
            btnImportUsers.Text = "Import Now";
            Close.Text = "<- Go Back";

            try
            {
                iBoardId = int.Parse(Settings["forumboardid"].ToString());
            }
            catch (Exception)
            {
                iBoardId = 1;
            }

            if (IsPostBack || GetIdOfScheduleClient(TypeFullName) <= 0) return;

            string sFile = string.Format("{0}App_Data/YafImports.xml", HttpRuntime.AppDomainAppPath);

            DataSet dsSettings = new DataSet();

            try
            {
                dsSettings.ReadXml(sFile);
            }
            catch (Exception)
            {
                FileStream file = new FileStream(sFile, FileMode.Create);
                StreamWriter sw = new StreamWriter(file);

                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<YafImports>");
                sw.WriteLine(string.Format("<Import PortalId=\"{0}\" BoardId=\"{1}\"/>", PortalId, iBoardId));
                sw.WriteLine("</YafImports>");

                sw.Close();
                file.Close();
            }

            bool bUpdateXml = false;

            foreach (DataRow oRow in dsSettings.Tables[0].Rows)
            {
                int iPortal = Convert.ToInt32(oRow["PortalId"]);
                int iBoard = Convert.ToInt32(oRow["BoardId"]);

                if (iPortal.Equals(PortalId) && iBoard.Equals(iBoardId))
                {
                    bUpdateXml = false;
                    break;
                }

                bUpdateXml = true;
            }

            if (bUpdateXml)
            {
                DataRow dr = dsSettings.Tables["Import"].NewRow();

                dr["PortalId"] = PortalId.ToString();
                dr["BoardId"] = iBoardId.ToString();

                dsSettings.Tables[0].Rows.Add(dr);

                dsSettings.WriteXml(sFile);

            }

            btnAddScheduler.CommandArgument = "delete";
            btnAddScheduler.Text = "Delete Yaf User Importer Schedule";
        }

        protected override void OnInit(EventArgs e)
        {
            Load += DotNetNukeModuleImport_Load;
            
            btnImportUsers.Click += ImportClick;
            Close.Click += CloseClick;
            btnAddScheduler.Click += AddSchedulerClick;

            base.OnInit(e);
        }

        protected void AddSchedulerClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.CommandArgument)
            {
                case "add":
                    InstallScheduleClient();
                    btn.CommandArgument = "update";
                    btn.Text = "Update Yaf User Importer Schedule";
                    break;
                case "update":
                    UpdateScheduleItem(GetIdOfScheduleClient(TypeFullName));
                    btn.CommandArgument = "delete";
                    btn.Text = "Delete Yaf User Importer Schedule";
                    break;
                case "delete":
                    RemoveScheduleClient(GetIdOfScheduleClient(TypeFullName));
                    btn.CommandArgument = "add";
                    btn.Text = "Install Yaf User Importer Schedule";
                    break;
            }
        }
        private void InstallScheduleClient()
        {
            ScheduleItem item = new ScheduleItem
                                    {
                                        FriendlyName = "YAF DotNetNuke User Importer",
                                        TypeFullName = TypeFullName,
                                        TimeLapse = 1,
                                        TimeLapseMeasurement = "d",
                                        RetryTimeLapse = 1,
                                        RetryTimeLapseMeasurement = "d",
                                        RetainHistoryNum = 5,
                                        AttachToEvent = "None",
                                        CatchUpEnabled = true,
                                        Enabled = true,
                                        ObjectDependencies = "",
                                        Servers = ""
                                    };

            // add item
            SchedulingProvider.Instance().AddSchedule(item);

            DataSet dsSettings = new DataSet();

            string sFile = string.Format("{0}App_Data/YafImports.xml", HttpRuntime.AppDomainAppPath);

            try
            {
                dsSettings.ReadXml(sFile);
            }
            catch (Exception)
            {
                FileStream file = new FileStream(sFile, FileMode.Create);
                StreamWriter sw = new StreamWriter(file);

                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<YafImports>");
                sw.WriteLine(string.Format("<Import PortalId=\"{0}\" BoardId=\"{1}\"/>", PortalId, iBoardId));
                sw.WriteLine("</YafImports>");

                sw.Close();
                file.Close();
            }
        }

        private static void RemoveScheduleClient(int id)
        {
            // get the item by id
            ScheduleItem item = SchedulingProvider.Instance().GetSchedule(id);

            // tell the provider to remove the item
            SchedulingProvider.Instance().DeleteSchedule(item);
        }

        private static void UpdateScheduleItem(int id)
        {
            // get the item by id
            ScheduleItem item = SchedulingProvider.Instance().GetSchedule(id);

            // set property on item
            item.TimeLapse = 60;

            // update item
            SchedulingProvider.Instance().UpdateSchedule(item);
        }
        private static int GetIdOfScheduleClient(string typeFullName)
        {
            // get array list of schedule items
            ArrayList schduleItems = SchedulingProvider.Instance().GetSchedule();

            // find schedule item with matching TypeFullName
            foreach (object item in schduleItems)
            {
                if (((ScheduleItem)item).TypeFullName == typeFullName)
                {
                    return ((ScheduleItem)item).ScheduleID;
                }
            }
            return -1;
        }

        private void CloseClick(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(), true);
        }

        private void ImportClick(object sender, EventArgs e)
        {
            int iNewUsers = 0;
            bool bRolesChanged = false;
            try
            {
                foreach (UserInfo dnnUserInfo in UserController.GetUsers(PortalId))
                {
                    //Get current Dnn user
                   // UserInfo dnnUserInfo = UserController.GetUserById(PortalId, UserId);

                    //get the user from the membership provider
                    MembershipUser dnnUser = Membership.GetUser(dnnUserInfo.Username, true);

                    if (dnnUser == null)
                        return;

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

                    // Sync Roles
                    if (roleChanged)
                    {
                        bRolesChanged = true;

                        MarkRolesChanged();
                    }


                    int yafUserId;

                    try
                    {
                        yafUserId = DB.user_get(iBoardId, dnnUser.ProviderUserKey);
                    }
                    catch (Exception)
                    {
                        yafUserId = CreateYafUser(dnnUserInfo, dnnUser);
                        iNewUsers++;
                    }

                    // Create user if Not Exist



                    // super admin check...
                    if (dnnUserInfo.IsSuperUser)
                    {
                        CreateYafHostUser(yafUserId);

                    }

                    if (YafContext.Current.Settings != null)
                    {
                        RoleMembershipHelper.UpdateForumUser(dnnUser, YafContext.Current.Settings.BoardID);
                    }
                }

                lInfo.Text = string.Format("{0} User(s) Imported", iNewUsers);

                if (bRolesChanged)
                {
                    lInfo.Text += ", but all Roles are syncronized!";
                }
                else
                {
                    lInfo.Text += ", Roles already syncronized!";
                }

                YafContext.Current.Cache.Clear();
                //DataCache.ClearCache();
                DataCache.ClearPortalCache(PortalSettings.PortalId, true);
                Session.Clear();
               
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }
        private void CreateYafHostUser(int yafUserId)
        {
            // get this user information...
            DataTable userInfo = DB.user_list(iBoardId, yafUserId, null, null, null);

            if (userInfo.Rows.Count <= 0)
                return;

            DataRow row = userInfo.Rows[0];

            if (Convert.ToBoolean(row["IsHostAdmin"]))
                return;

            // fix the ishostadmin flag...
            UserFlags userFlags = new UserFlags(row["Flags"]) { IsHostAdmin = true };

            // update...
            DB.user_adminsave(iBoardId,
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
            //RoleMembershipHelper.SetupUserRoles(Forum1.BoardID, dnnUser.UserName);

            // create the user in the YAF DB so profile can ge created...
            int? userId = RoleMembershipHelper.CreateForumUser(dnnUser, iBoardId);

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

            // save the time zone...
            DB.user_save(yafUserId,
                         iBoardId,
                        null, null, null, GetUserTimeZoneOffset(dnnUserInfo, PortalSettings), null, null, null, null, null, null, null, null, null);

            return yafUserId;
        }
        private static int GetUserTimeZoneOffset(UserInfo userInfo, PortalSettings portalSettings)
        {
            int timeZone;
            if ((userInfo != null) && (userInfo.UserID != Null.NullInteger))
            {
                timeZone = userInfo.Profile.TimeZone;
            }
            else
            {
                timeZone = portalSettings.TimeZoneOffset;
            }

            return timeZone;
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

        
    }
}