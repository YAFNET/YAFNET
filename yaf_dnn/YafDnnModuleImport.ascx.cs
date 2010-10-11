namespace YAF.DotNetNuke
{
  #region Using

  using System;
  using System.Collections;
  using System.Data;
  using System.IO;
  using System.Web;
  using System.Web.Security;
  using System.Web.UI.WebControls;

  using global::DotNetNuke.Common;
  using global::DotNetNuke.Common.Utilities;
  using global::DotNetNuke.Entities.Modules;
  using global::DotNetNuke.Entities.Portals;
  using global::DotNetNuke.Entities.Users;
  using global::DotNetNuke.Services.Exceptions;
  using global::DotNetNuke.Services.Scheduling;

  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

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
    /// The get id of schedule client.
    /// </returns>
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
    /// The get user time zone offset.
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
      DataTable userInfo = DB.user_list(this.iBoardId, yafUserId, null, null, null);

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
      DB.user_adminsave(
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
    /// The create yaf user.
    /// </returns>
    private int CreateYafUser(UserInfo dnnUserInfo, MembershipUser dnnUser)
    {
      YafContext.Current.Cache.Clear();

      // setup roles
      RoleMembershipHelper.SetupUserRoles(this.iBoardId, dnnUser.UserName);

      // create the user in the YAF DB so profile can ge created...
      int? userId = RoleMembershipHelper.CreateForumUser(dnnUser, this.iBoardId);

      if (userId == null)
      {
        return 0;
      }

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
      DB.user_save(
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
      this.btnImportUsers.Text = "Import Now";
      this.Close.Text = "<- Go Back";

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

      string sFile = string.Format("{0}App_Data/YafImports.xml", HttpRuntime.AppDomainAppPath);

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
        sw.WriteLine(string.Format("<Import PortalId=\"{0}\" BoardId=\"{1}\"/>", this.PortalId, this.iBoardId));
        sw.WriteLine("</YafImports>");

        sw.Close();
        file.Close();
      }

      bool bUpdateXml = false;

      foreach (DataRow oRow in dsSettings.Tables[0].Rows)
      {
        int iPortal = Convert.ToInt32(oRow["PortalId"]);
        int iBoard = Convert.ToInt32(oRow["BoardId"]);

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
      this.btnAddScheduler.Text = "Delete Yaf User Importer Schedule";
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
      int iNewUsers = 0;
      bool bRolesChanged = false;
      try
      {
        foreach (UserInfo dnnUserInfo in UserController.GetUsers(this.PortalId))
        {
          // Get current Dnn user
          // UserInfo dnnUserInfo = UserController.GetUserById(PortalId, UserId);

          // get the user from the membership provider
          MembershipUser dnnUser = Membership.GetUser(dnnUserInfo.Username, true);

          if (dnnUser == null)
          {
            return;
          }

          bool roleChanged = false;
          foreach (string role in dnnUserInfo.Roles)
          {
            if (!Roles.RoleExists(role))
            {
              Roles.CreateRole(role);
              roleChanged = true;
            }

            if (!Roles.IsUserInRole(dnnUserInfo.Username, role))
            {
              Roles.AddUserToRole(dnnUserInfo.Username, role);
            }
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
            yafUserId = DB.user_get(this.iBoardId, dnnUser.ProviderUserKey);
          }
          catch (Exception)
          {
            yafUserId = this.CreateYafUser(dnnUserInfo, dnnUser);
            iNewUsers++;
          }

          // Create user if Not Exist

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

        this.lInfo.Text = string.Format("{0} User(s) Imported", iNewUsers);

        if (bRolesChanged)
        {
          this.lInfo.Text += ", but all Roles are syncronized!";
        }
        else
        {
          this.lInfo.Text += ", Roles already syncronized!";
        }

        YafContext.Current.Cache.Clear();

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

      string sFile = string.Format("{0}App_Data/YafImports.xml", HttpRuntime.AppDomainAppPath);

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
        sw.WriteLine(string.Format("<Import PortalId=\"{0}\" BoardId=\"{1}\"/>", this.PortalId, this.iBoardId));
        sw.WriteLine("</YafImports>");

        sw.Close();
        file.Close();
      }
    }

    #endregion
  }
}