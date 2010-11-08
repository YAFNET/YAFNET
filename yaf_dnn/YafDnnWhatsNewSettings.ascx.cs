// DotNetNuke® - http://www.dotnetnuke.com 
// Copyright (c) 2002-2010 
// by DotNetNuke Corporation 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software. 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE. 

namespace YAF.DotNetNuke
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Web.UI.WebControls;

  using global::DotNetNuke.Common.Utilities;
  using global::DotNetNuke.Entities.Modules;
  using global::DotNetNuke.Entities.Tabs;
  using global::DotNetNuke.Services.Exceptions;

  #endregion

  /// -----------------------------------------------------------------------------
  /// <summary>
  /// The Settings class manages Module Settings
  /// </summary>
  /// <remarks>
  /// </remarks>
  /// <history> 
  /// </history>
  /// -----------------------------------------------------------------------------
  partial class YafDnnWhatsNewSettings : ModuleSettingsBase
  {
    #region Public Methods

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// LoadSettings loads the settings from the Database and displays them
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history> 
    /// </history>
    /// -----------------------------------------------------------------------------
    public override void LoadSettings()
    {
      this.FillYafInstances();

      try
      {
        if (!this.IsPostBack)
        {
          if (this.YafInstances.Items.Count > 0)
          {
            if (!string.IsNullOrEmpty((string)this.TabModuleSettings["YafPage"]) &&
                !string.IsNullOrEmpty((string)this.TabModuleSettings["YafModuleId"]))
            {
              this.YafInstances.SelectedValue = string.Format(
                "{0}-{1}", this.TabModuleSettings["YafPage"], this.TabModuleSettings["YafModuleId"]);
            }
          }

          if (!string.IsNullOrEmpty((string)this.TabModuleSettings["YafMaxPosts"]))
          {
            this.txtMaxResult.Text = (string)this.TabModuleSettings["YafMaxPosts"];
          }
          else
          {
            this.txtMaxResult.Text = "10";
          }

          if (!string.IsNullOrEmpty((string)this.TabModuleSettings["YafUseRelativeTime"]))
          {
              bool YafUseRelativeTime;
              bool.TryParse((string)TabModuleSettings["YafUseRelativeTime"], out YafUseRelativeTime);

              this.UseRelativeTime.Checked = YafUseRelativeTime;
          }

            
        }
      }
      catch (Exception exc)
      {
        // Module failed to load 
        Exceptions.ProcessModuleLoadException(this, exc);
      }
    }

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// UpdateSettings saves the modified settings to the Database
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history> 
    /// </history>
    /// -----------------------------------------------------------------------------
    public override void UpdateSettings()
    {
      try
      {
        var objModules = new ModuleController();

        if (this.YafInstances.Items.Count > 0)
        {
          /*if (ddLTabs.SelectedValue != null && ddLModules.SelectedValue != null)
                    {
                        objModules.UpdateTabModuleSetting(TabModuleId, "YafPage", ddLTabs.SelectedValue);
                        objModules.UpdateTabModuleSetting(TabModuleId, "YafModuleId", ddLModules.SelectedValue);
                    }*/
          string[] values = this.YafInstances.SelectedValue.Split(Convert.ToChar("-"));

          if (values.Length == 2)
          {
            objModules.UpdateTabModuleSetting(this.TabModuleId, "YafPage", values[0]);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "YafModuleId", values[1]);
          }
        }

        if (IsNumeric(this.txtMaxResult.Text) || !string.IsNullOrEmpty(this.txtMaxResult.Text))
        {
          objModules.UpdateTabModuleSetting(this.TabModuleId, "YafMaxPosts", this.txtMaxResult.Text);
        }
        else
        {
          objModules.UpdateTabModuleSetting(this.TabModuleId, "YafMaxPosts", "10");
        }

        objModules.UpdateTabModuleSetting(this.TabModuleId, "YafUseRelativeTime", UseRelativeTime.Checked.ToString());
      }
      catch (Exception exc)
      {
        // Module failed to load 
        Exceptions.ProcessModuleLoadException(this, exc);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Check if Object is a Number
    /// </summary>
    /// <param name="valueToCheck">
    /// Object to Check
    /// </param>
    /// <returns>
    /// Returns bool Value
    /// </returns>
    private static bool IsNumeric(object valueToCheck)
    {
      double dummy;
      string inputValue = Convert.ToString(valueToCheck);

      bool numeric = double.TryParse(inputValue, NumberStyles.Any, null, out dummy);

      return numeric;
    }

    /// <summary>
    /// Fill DropDownList with Portal Tabs
    /// </summary>
    private void FillYafInstances()
    {
      var objTabController = new TabController();

      List<TabInfo> objTabs = TabController.GetPortalTabs(this.PortalSettings.PortalId, -1, true, true);

      DesktopModuleInfo objDesktopModuleInfo =
        DesktopModuleController.GetDesktopModuleByModuleName("YetAnotherForumDotNet", this.PortalId);

      if (objDesktopModuleInfo == null)
      {
        return;
      }

      foreach (TabInfo objTab in objTabs)
      {
        if (objTab == null || objTab.IsDeleted)
        {
          continue;
        }

        var objModules = new ModuleController();

        foreach (KeyValuePair<int, ModuleInfo> pair in objModules.GetTabModules(objTab.TabID))
        {
          ModuleInfo objModule = pair.Value;

          if (objModule.IsDeleted || objModule.DesktopModuleID != objDesktopModuleInfo.DesktopModuleID ||
              objModule.IsDeleted)
          {
            continue;
          }

          string strPath = objTab.TabName;
          TabInfo objTabSelected = objTab;

          while (objTabSelected.ParentId != Null.NullInteger)
          {
            objTabSelected = objTabController.GetTab(objTabSelected.ParentId, objTab.PortalID, false);
            if (objTabSelected == null)
            {
              break;
            }

            strPath = string.Format("{0} -> {1}", objTabSelected.TabName, strPath);
          }

          var objListItem = new ListItem
            {
              Value = string.Format("{0}-{1}", objModule.TabID, objModule.ModuleID),
              Text = string.Format("{0} -> {1}", strPath, objModule.ModuleTitle)
            };

          this.YafInstances.Items.Add(objListItem);
        }
      }
    }

    #endregion
  }
}