/* Yet Another Forum.NET
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using global::DotNetNuke.Common.Utilities;

    using global::DotNetNuke.Entities.Modules;

    using global::DotNetNuke.Entities.Tabs;

    using global::DotNetNuke.Services.Exceptions;

    using YAF.Types.Extensions;

    #endregion

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Settings class manages Module Settings
    /// </summary>
    /// <history> 
    /// </history>
    /// -----------------------------------------------------------------------------
    public partial class YafDnnWhatsNewSettings : ModuleSettingsBase
    {
        #region Public Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// <history> 
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            this.FillYafInstances();

            try
            {
                if (this.IsPostBack)
                {
                    return;
                }

                if (this.YafInstances.Items.Count > 0)
                {
                    if (!string.IsNullOrEmpty((string)this.TabModuleSettings["YafPage"])
                        && !string.IsNullOrEmpty((string)this.TabModuleSettings["YafModuleId"]))
                    {
                        this.YafInstances.SelectedValue = "{0}-{1}".FormatWith(
                            this.TabModuleSettings["YafPage"], this.TabModuleSettings["YafModuleId"]);
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
                    bool yafUseRelativeTime;
                    bool.TryParse((string)this.TabModuleSettings["YafUseRelativeTime"], out yafUseRelativeTime);

                    this.UseRelativeTime.Checked = yafUseRelativeTime;
                }

                if (!string.IsNullOrEmpty((string)TabModuleSettings["YafWhatsNewHeader"]))
                {
                    this.HtmlHeader.Text = (string)TabModuleSettings["YafWhatsNewHeader"];
                }
                else
                {
                    this.HtmlHeader.Text = "<ul>";
                }

                if (!string.IsNullOrEmpty((string)TabModuleSettings["YafWhatsNewItemTemplate"]))
                {
                    this.HtmlItem.Text = (string)TabModuleSettings["YafWhatsNewItemTemplate"];
                }
                else
                {
                    this.HtmlItem.Text =
                        "<li class=\"YafPosts\">[LASTPOSTICON]&nbsp;<strong>[TOPICLINK]</strong>&nbsp;([FORUMLINK])<br />[BYTEXT]&nbsp;[LASTUSERLINK]&nbsp;[LASTPOSTEDDATETIME]</li>";
                }

                if (!string.IsNullOrEmpty((string)TabModuleSettings["YafWhatsNewFooter"]))
                {
                    this.HtmlFooter.Text = (string)TabModuleSettings["YafWhatsNewFooter"];
                }
                else
                {
                    this.HtmlFooter.Text = "</ul>";
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

                objModules.UpdateTabModuleSetting(
                    this.TabModuleId, "YafUseRelativeTime", this.UseRelativeTime.Checked.ToString());

                if (!string.IsNullOrEmpty(this.HtmlHeader.Text))
                {
                    objModules.UpdateTabModuleSetting(this.TabModuleId, "YafWhatsNewHeader", this.HtmlHeader.Text);
                }

                if (!string.IsNullOrEmpty(this.HtmlItem.Text))
                {
                    objModules.UpdateTabModuleSetting(this.TabModuleId, "YafWhatsNewItemTemplate", this.HtmlItem.Text);
                }

                if (!string.IsNullOrEmpty(this.HtmlFooter.Text))
                {
                    objModules.UpdateTabModuleSetting(this.TabModuleId, "YafWhatsNewFooter", this.HtmlFooter.Text);
                }
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

                    if (objModule.IsDeleted || objModule.DesktopModuleID != objDesktopModuleInfo.DesktopModuleID
                        || objModule.IsDeleted)
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

                        strPath = "{0} -> {1}".FormatWith(objTabSelected.TabName, strPath);
                    }

                    var objListItem = new ListItem
                        {
                            Value = "{0}-{1}".FormatWith(objModule.TabID, objModule.ModuleID),
                            Text = "{0} -> {1}".FormatWith(strPath, objModule.ModuleTitle)
                        };

                    this.YafInstances.Items.Add(objListItem);
                }
            }
        }

        #endregion
    }
}