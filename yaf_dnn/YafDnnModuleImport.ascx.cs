﻿/* Yet Another Forum.NET
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
  #region Using

    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI.WebControls;
    using global::DotNetNuke.Common;
    using global::DotNetNuke.Common.Utilities;
    using global::DotNetNuke.Entities.Modules;
    using global::DotNetNuke.Entities.Portals;
    using global::DotNetNuke.Entities.Users;
    using global::DotNetNuke.Services.Exceptions;
    using global::DotNetNuke.Services.Localization;
    using global::DotNetNuke.Services.Scheduling;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for DotNetNukeModule.
  /// </summary>
  public partial class YafDnnModuleImport : PortalModuleBase
  {
    #region Constants and Fields

    /// <summary>
    /// The type full name.
    /// </summary>
    private const string TypeFullName = "YAF.DotNetNuke.YafDnnImportScheduler, YAF.DotNetNuke.Module";

    /// <summary>
    /// The i board id.
    /// </summary>
    private int iBoardId;

    /// <summary>
    /// Gets or sets The New Users Counter
    /// </summary>
    private int NewUsers
    {
        get
        {
            return this.ViewState["NewUsers"] != null ? (int)this.ViewState["NewUsers"] : 0;
        }

        set
        {
            this.ViewState["NewUsers"] = value;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The add scheduler click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void AddSchedulerClick(object sender, EventArgs e)
    {
      var btn = (Button)sender;
      switch (btn.CommandArgument)
      {
        case "add":
          this.InstallScheduleClient();
          btn.CommandArgument = "update";
          btn.Text = Localization.GetString("UpdateSheduler.Text", this.LocalResourceFile);
          break;
        case "update":
          UpdateScheduleItem(GetIdOfScheduleClient(TypeFullName));
          btn.CommandArgument = "delete";
          btn.Text = Localization.GetString("DeleteShedulerText", this.LocalResourceFile);
          break;
        case "delete":
          RemoveScheduleClient(GetIdOfScheduleClient(TypeFullName));
          btn.CommandArgument = "add";
          btn.Text = Localization.GetString("InstallSheduler.Text", this.LocalResourceFile);
          break;
      }
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      this.Load += this.DotNetNukeModuleImport_Load;

      this.btnImportUsers.Click += this.ImportClick;
      this.Close.Click += this.CloseClick;
      this.btnAddScheduler.Click += this.AddSchedulerClick;

      base.OnInit(e);
    }

    /// <summary>
    /// The get id of schedule client.
    /// </summary>
    /// <param name="typeFullName">
    /// The type full name.
    /// </param>
    /// <returns>
    /// Returns the id of schedule client.
    /// </returns>
    private static int GetIdOfScheduleClient(string typeFullName)
    {
      // get array list of schedule items
      ArrayList schduleItems = SchedulingProvider.Instance().GetSchedule();

      // find schedule item with matching TypeFullName
      foreach (object item in
          schduleItems.Cast<object>().Where(item => ((ScheduleItem)item).TypeFullName == typeFullName))
      {
          return ((ScheduleItem)item).ScheduleID;
      }

      return -1;
    }

    /// <summary>
    /// The get user time zone offset.
    /// </summary>
    /// <param name="userInfo">
    /// The user info.
    /// </param>
    /// <param name="portalSettings">
    /// The portal settings.
    /// </param>
    /// <returns>
    /// Returns the user time zone offset.
    /// </returns>
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

    /// <summary>
    /// The mark roles changed.
    /// </summary>
    private static void MarkRolesChanged()
    {
      RolePrincipal rolePrincipal;
      if (Roles.CacheRolesInCookie)
      {
        string roleCookie = string.Empty;

        HttpCookie cookie = HttpContext.Current.Request.Cookies[Roles.CookieName];

        if (cookie != null)
        {
          roleCookie = cookie.Value;
        }

        rolePrincipal = new RolePrincipal(HttpContext.Current.User.Identity, roleCookie);
      }
      else
      {
        rolePrincipal = new RolePrincipal(HttpContext.Current.User.Identity);
      }

      rolePrincipal.SetDirty();
    }

    /// <summary>
    /// The remove schedule client.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    private static void RemoveScheduleClient(int id)
    {
      // get the item by id
      ScheduleItem item = SchedulingProvider.Instance().GetSchedule(id);

      // tell the provider to remove the item
      SchedulingProvider.Instance().DeleteSchedule(item);
    }

    /// <summary>
    /// The update schedule item.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    private static void UpdateScheduleItem(int id)
    {
      // get the item by id
      ScheduleItem item = SchedulingProvider.Instance().GetSchedule(id);

      // set property on item
      item.TimeLapse = 60;

      // update item
      SchedulingProvider.Instance().UpdateSchedule(item);
    }

    /// <summary>
    /// The close click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void CloseClick(object sender, EventArgs e)
    {
      this.Response.Redirect(Globals.NavigateURL(), true);
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
      DataTable userInfo = LegacyDb.user_list(this.iBoardId, yafUserId, null, null, null);

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
      var userFlags = new UserFlags(row["Flags"]) { IsHostAdmin = true };

      // update...
      LegacyDb.user_adminsave(
        this.iBoardId, yafUserId, row["Name"], row["DisplayName"], row["Email"], userFlags.BitValue, row["RankID"]);
    }

    /// <summary>
    /// The create yaf user.
    /// </summary>
    /// <param name="dnnUserInfo">
    /// The dnn user info.
    /// </param>
    /// <param name="dnnUser">
    /// The dnn user.
    /// </param>
    /// <returns>
    /// Returns the User Id of the new user.
    /// </returns>
    private int CreateYafUser(UserInfo dnnUserInfo, MembershipUser dnnUser)
    {
      YafContext.Current.Get<IDataCache>().Clear();

      // setup roles
      RoleMembershipHelper.SetupUserRoles(this.iBoardId, dnnUser.UserName);

      // create the user in the YAF DB so profile can ge created...
      int? userId = RoleMembershipHelper.CreateForumUser(dnnUser, dnnUserInfo.DisplayName,  this.iBoardId);

      if (userId == null)
      {
        return 0;
      }

      // create profile
      YafUserProfile userProfile = YafUserProfile.GetProfile(dnnUser.UserName);

      // setup their inital profile information
      userProfile.Initialize(dnnUser.UserName, true);
      userProfile.RealName = dnnUserInfo.Profile.FullName;
      userProfile.Country = dnnUserInfo.Profile.Country;
      userProfile.Region = dnnUserInfo.Profile.Region;
      userProfile.City = dnnUserInfo.Profile.City;
      userProfile.Homepage = dnnUserInfo.Profile.Website;
      userProfile.Save();

      int yafUserId = UserMembershipHelper.GetUserIDFromProviderUserKey(dnnUser.ProviderUserKey);

      // Save User
      LegacyDb.user_save(
        yafUserId, 
        this.iBoardId, 
        null, 
        null, 
        null, 
        GetUserTimeZoneOffset(dnnUserInfo, this.PortalSettings), 
        null, 
        null, 
        null,
        null,
        null,
        null, 
        null, 
        null, 
        null, 
        null, 
        null, 
        null);

      return yafUserId;
    }

    /// <summary>
    /// The dot net nuke module import_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void DotNetNukeModuleImport_Load(object sender, EventArgs e)
    {
      this.btnImportUsers.Text = Localization.GetString("ImportNow.Text", this.LocalResourceFile);
      this.Close.Text = Localization.GetString("Close.Text", this.LocalResourceFile);
      this.btnAddScheduler.Text = Localization.GetString("InstallSheduler.Text", this.LocalResourceFile);

      try
      {
        this.iBoardId = int.Parse(this.Settings["forumboardid"].ToString());
      }
      catch (Exception)
      {
        this.iBoardId = 1;
      }

      if (this.IsPostBack || GetIdOfScheduleClient(TypeFullName) <= 0)
      {
        return;
      }

      string sFile = "{0}App_Data/YafImports.xml".FormatWith(HttpRuntime.AppDomainAppPath);

      var dsSettings = new DataSet();

      try
      {
        dsSettings.ReadXml(sFile);
      }
      catch (Exception)
      {
        var file = new FileStream(sFile, FileMode.Create);
        var sw = new StreamWriter(file);

        sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
        sw.WriteLine("<YafImports>");
        sw.WriteLine("<Import PortalId=\"{0}\" BoardId=\"{1}\"/>".FormatWith(this.PortalId, this.iBoardId));
        sw.WriteLine("</YafImports>");

        sw.Close();
        file.Close();
      }

      bool bUpdateXml = false;

      foreach (DataRow oRow in dsSettings.Tables[0].Rows)
      {
        int iPortal = oRow["PortalId"].ToType<int>();
        int iBoard = oRow["BoardId"].ToType<int>();

        if (iPortal.Equals(this.PortalId) && iBoard.Equals(this.iBoardId))
        {
          bUpdateXml = false;
          break;
        }

        bUpdateXml = true;
      }

      if (bUpdateXml)
      {
        DataRow dr = dsSettings.Tables["Import"].NewRow();

        dr["PortalId"] = this.PortalId.ToString();
        dr["BoardId"] = this.iBoardId.ToString();

        dsSettings.Tables[0].Rows.Add(dr);

        dsSettings.WriteXml(sFile);
      }

      this.btnAddScheduler.CommandArgument = "delete";
      this.btnAddScheduler.Text = Localization.GetString("DeleteSheduler.Text", this.LocalResourceFile);
    }

    /// <summary>
    /// The import click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ImportClick(object sender, EventArgs e)
    {
      this.NewUsers = 0;
      bool bRolesChanged = false;

      var users = UserController.GetUsers(this.PortalId);
      users.Sort(new UserComparer());

      try
      {
          foreach (UserInfo dnnUserInfo in users)
        {
          // Get current Dnn user
          // UserInfo dnnUserInfo = UserController.GetUserById(PortalId, UserId);

          // get the user from the membership provider
          MembershipUser dnnUser = Membership.GetUser(dnnUserInfo.Username, true);

          if (dnnUser == null)
          {
            continue;
          }

          bool roleChanged = false;
          foreach (string role in dnnUserInfo.Roles)
          {
              if (!RoleMembershipHelper.RoleExists(role))
              {
                  RoleMembershipHelper.CreateRole(role);
                  roleChanged = true;
              }

              if (RoleMembershipHelper.IsUserInRole(dnnUserInfo.Username, role))
              {
                  continue;
              }

              try
              {
                  RoleMembershipHelper.AddUserToRole(dnnUserInfo.Username, role);
              }
              catch
              {
                  // TODO :Dont do anything when user is already in role ?!
              }
          }

          // Sync Roles
          if (roleChanged)
          {
            bRolesChanged = true;

            MarkRolesChanged();
          }

          int yafUserId = LegacyDb.user_get(this.iBoardId, dnnUser.ProviderUserKey);

            if (yafUserId.Equals(0))
            {
                yafUserId = this.CreateYafUser(dnnUserInfo, dnnUser);
                this.NewUsers++;
            }

          /*try
          {
            yafUserId = LegacyDb.user_get(this.iBoardId, dnnUser.ProviderUserKey);
          }
          catch (Exception)
          {
            // Create user if Not Exist
            yafUserId = this.CreateYafUser(dnnUserInfo, dnnUser);

            this.NewUsers++;
          }*/

          // super admin check...
          if (dnnUserInfo.IsSuperUser)
          {
            this.CreateYafHostUser(yafUserId);
          }

          if (YafContext.Current.Settings != null)
          {
            RoleMembershipHelper.UpdateForumUser(dnnUser, YafContext.Current.Settings.BoardID);
          }
        }

        this.lInfo.Text = Localization.GetString("UsersImported.Text", this.LocalResourceFile).FormatWith(this.NewUsers);

        if (bRolesChanged)
        {
          this.lInfo.Text += Localization.GetString("RolesSynced.Text", this.LocalResourceFile);
        }
        else
        {
          this.lInfo.Text += Localization.GetString("RolesNotSynced.Text", this.LocalResourceFile);
        }

        YafContext.Current.Get<IDataCache>().Clear();

        // DataCache.ClearCache();
        DataCache.ClearPortalCache(this.PortalSettings.PortalId, true);
        this.Session.Clear();
      }
      catch (Exception ex)
      {
        Exceptions.ProcessModuleLoadException(this, ex);
      }
    }

    /// <summary>
    /// The install schedule client.
    /// </summary>
    private void InstallScheduleClient()
    {
      var item = new ScheduleItem
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
          ObjectDependencies = string.Empty, 
          Servers = string.Empty
        };

      // add item
      SchedulingProvider.Instance().AddSchedule(item);

      var dsSettings = new DataSet();

      string sFile = "{0}App_Data/YafImports.xml".FormatWith(HttpRuntime.AppDomainAppPath);

      try
      {
        dsSettings.ReadXml(sFile);
      }
      catch (Exception)
      {
        var file = new FileStream(sFile, FileMode.Create);
        var sw = new StreamWriter(file);

        sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
        sw.WriteLine("<YafImports>");
        sw.WriteLine("<Import PortalId=\"{0}\" BoardId=\"{1}\"/>".FormatWith(this.PortalId, this.iBoardId));
        sw.WriteLine("</YafImports>");

        sw.Close();
        file.Close();
      }
    }

    #endregion
  }
}