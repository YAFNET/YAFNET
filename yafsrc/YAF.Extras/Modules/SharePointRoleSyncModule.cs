/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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
  /// Summary description for SharePoineRoleSyncModule
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
        HttpContext.Current.Session["UserSharePointSynced"] = user.UserName;
      }
      else if (user == null && HttpContext.Current.Session["UserSharePointSynced"] != null)
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
                  var userGroups = spUser.Groups.Cast<SPGroup>().ToList();

                  // sync to SharePoint
                  foreach (var groupName in groups)
                  {
                    if (webGroups.Exists(g => g.Name.Equals(groupName)) && !userGroups.Any(g => g.Name.Equals(groupName)))
                    {
                      System.Diagnostics.Trace.WriteLine("Adding to SharePoint Group: " + groupName);
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