using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Security;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.DotNetNuke
{
    public class YafDnnImportScheduler : SchedulerClient
    {
        private string sInfo = "";
        //private readonly PortalSettings _portalSettings;

        public YafDnnImportScheduler(ScheduleHistoryItem objScheduleHistoryItem)
        {
            ScheduleHistoryItem = objScheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                GetSettings();
               
                // report success to the scheduler framework
                ScheduleHistoryItem.Succeeded = true;

                ScheduleHistoryItem.AddLogNote(sInfo);
            }
            catch (Exception exc)
            {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote("EXCEPTION: " + exc);
                Errored(ref exc);
                Exceptions.LogException(exc);
            }
        }

        private void GetSettings()
        {
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
                sw.WriteLine("<Import PortalId=\"0\" BoardId=\"1\"/>");
                sw.WriteLine("</YafImports>");

                sw.Close();
                file.Close();

                dsSettings.ReadXml(sFile);
            }

            foreach (DataRow oRow in dsSettings.Tables[0].Rows)
            {
                int iPortalId = Convert.ToInt32(oRow["PortalId"]);
                int iBoardId = Convert.ToInt32(oRow["BoardId"]);

                ImportUsers(iBoardId, iPortalId);
            }
        }

        private void ImportUsers(int iBoard, int iPortal)
        {
            int iNewUsers = 0;
            bool bRolesChanged = false;
            try
            {
                foreach (UserInfo dnnUserInfo in UserController.GetUsers(iPortal))
                {
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
                        yafUserId = DB.user_get(iBoard, dnnUser.ProviderUserKey);
                    }
                    catch (Exception)
                    {
                        yafUserId = CreateYafUser(dnnUserInfo, dnnUser, iBoard);
                        iNewUsers++;
                    }

                    // Create user if Not Exist



                    // super admin check...
                    if (dnnUserInfo.IsSuperUser)
                    {
                        CreateYafHostUser(yafUserId, iBoard);

                    }

                    if (YafContext.Current.Settings != null)
                    {
                        RoleMembershipHelper.UpdateForumUser(dnnUser, YafContext.Current.Settings.BoardID);
                    }
                }

                sInfo = string.Format("{0} User(s) Imported", iNewUsers);

                if (bRolesChanged)
                {
                    sInfo += ", but all Roles are syncronized!";
                }
                else
                {
                    sInfo += ", Roles already syncronized!";
                }

                YafContext.Current.Cache.Clear();

                DataCache.ClearCache();
                //Session.Clear();

            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
        }
        private static void CreateYafHostUser(int yafUserId, int iboardId)
        {
            // get this user information...
            DataTable userInfo = DB.user_list(iboardId, yafUserId, null, null, null);

            if (userInfo.Rows.Count <= 0)
                return;

            DataRow row = userInfo.Rows[0];

            if (Convert.ToBoolean(row["IsHostAdmin"]))
                return;

            // fix the ishostadmin flag...
            UserFlags userFlags = new UserFlags(row["Flags"]) { IsHostAdmin = true };

            // update...
            DB.user_adminsave(iboardId,
                              yafUserId,
                              row["Name"],
                              row["Email"],
                              userFlags.BitValue,
                              row["RankID"]);
        }
        private static int CreateYafUser(UserInfo dnnUserInfo, MembershipUser dnnUser, int iBoardId)
        {
            YafContext.Current.Cache.Clear();
            // setup roles
            //RoleMembershipHelper.SetupUserRoles(this.forum1.BoardID, dnnUser.UserName);

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
                        null, null, null, GetUserTimeZoneOffset(dnnUserInfo), null, null, null, null, null, null, null, null, null);

            return yafUserId;
        }
        private static int GetUserTimeZoneOffset(UserInfo userInfo)
        {
            int timeZone;
            if ((userInfo != null) && (userInfo.UserID != Null.NullInteger))
            {
                timeZone = userInfo.Profile.TimeZone;
            }
            else
            {
                timeZone = -480;
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