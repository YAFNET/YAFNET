/* YetAnotherForum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Modules
{
  #region Using

  using System;
  using System.Linq;
  using System.Collections.Generic;
  using System.Web;
  using System.Web.Security;

  using Microsoft.SharePoint;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;

  #endregion

  /// <summary>
  /// Summary description for MOSSRoleSyncModule
  /// </summary>
  [YafModule("Microsoft Sharepoint Role Synchronization Module", "Tiny Gecko", 1)]
  public class SharepointRoleSyncModule : SimpleBaseModule
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SharepointRoleSyncModule"/> class.
    /// </summary>
    public SharepointRoleSyncModule()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets SharePointUrl.
    /// </summary>
    public string SharePointUrl { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The init forum.
    /// </summary>
    public override void InitForum()
    {
      this.SharePointUrl = Config.GetConfigValueAsString("YAF.SharePointURL") ??
                           HttpContext.Current.Request.Url.ToString();

      this.PageContext.PagePreLoad += new EventHandler<EventArgs>(this.PageContext_PagePreLoad);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The page context_ page pre load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void PageContext_PagePreLoad(object sender, EventArgs e)
    {
      MembershipUser user = UserMembershipHelper.GetUser(false);

      if (user != null && HttpContext.Current.Session["UserSharePointSynced"] == null)
      {
        // sync roles with Sharepoint...
        this.SyncUserToGroups(user.UserName, YafContext.Current.CurrentRoles.GetRolesForUser(user.UserName));

        // don't need to do this again for the remainder of the session...
        HttpContext.Current.Session["UserSharePointSynced"] = true;
      }
      else if (HttpContext.Current.Session["UserSharePointSynced"] != null)
      {
        HttpContext.Current.Session.Remove("UserSharePointSynced");
      }
    }

    /// <summary>
    /// Sync a user to Sharepoint groups
    /// </summary>
    /// <param name="userName">
    /// The user Name.
    /// </param>
    /// <param name="groups">
    /// The groups.
    /// </param>
    private void SyncUserToGroups(string userName, string[] groups)
    {
      // Executes this method with Full Control rights even if the user does not otherwise have Full Control
      SPSecurity.RunWithElevatedPrivileges(
        () =>
        {
          // Don't use context to create the spSite object since it won't create the object with elevated privileges but with the privileges of the user who execute the this code, which may casues an exception
          using (var spSite = new SPSite(this.SharePointUrl))
          {
            using (SPWeb spWeb = spSite.OpenWeb())
            {
              try
              {
                // Allow updating of some sharepoint lists, (here spUsers, spGroups etc...)
                spWeb.AllowUnsafeUpdates = true;

                SPUser spUser = spWeb.EnsureUser(userName);

                if (spUser != null)
                {
                  var webGroups = spWeb.Groups.Cast<SPGroup>().ToList();
                  var userGroups = spUser.OwnedGroups.Cast<SPGroup>().ToList();

                  // sync to SharePoint
                  foreach (var groupName in groups)
                  {
                    if (webGroups.Exists(g => g.Name.Equals(groupName)) && !userGroups.Any(g => g.Name.Equals(groupName)))
                    {
                      webGroups.Where(g => g.Name.Equals(groupName)).SingleOrDefault().AddUser(spUser);
                    }
                  }
                }
              }
              catch (Exception ex)
              {
                // Error handling logic should go here
                DB.eventlog_create(null, this.GetType().ToString(), ex.ToString());
              }
              finally
              {
                spWeb.AllowUnsafeUpdates = false;
              }
            }
          }
        });
    }

    #endregion
  }
}