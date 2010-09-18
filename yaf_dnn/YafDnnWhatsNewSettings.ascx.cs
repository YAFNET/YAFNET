// 
// DotNetNuke® - http://www.dotnetnuke.com 
// Copyright (c) 2002-2010 
// by DotNetNuke Corporation 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software. 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE. 
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;

namespace YAF.DotNetNuke
{

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

        #region "Base Method Implementations"

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
            FillYafInstances();

            try
            {
                if (!IsPostBack)
                {
                    if (YafInstances.Items.Count > 0)
                    {
                        if (!string.IsNullOrEmpty((string)TabModuleSettings["YafPage"]) &&
                            !string.IsNullOrEmpty((string)TabModuleSettings["YafModuleId"]))
                        {
                            YafInstances.SelectedValue = string.Format("{0}-{1}",
                                                                          TabModuleSettings["YafPage"],
                                                                          TabModuleSettings["YafModuleId"]);
                        }
                    }

                    if (!string.IsNullOrEmpty((string)TabModuleSettings["YafMaxPosts"]))
                    {
                        txtMaxResult.Text = (string) TabModuleSettings["YafMaxPosts"];
                    }
                    else
                    {
                        txtMaxResult.Text = "10";
                    }
                }
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        /// <summary>
        /// Fill DropDownList with Portal Tabs
        /// </summary>
        private void FillYafInstances()
        {
            TabController objTabController = new TabController();

            List<TabInfo> objTabs = TabController.GetPortalTabs(PortalSettings.PortalId, -1, true, true);

            DesktopModuleInfo objDesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName("YetAnotherForumDotNet",
                                                                                        PortalId);

            if (objDesktopModuleInfo != null)
            {
                foreach (TabInfo objTab in objTabs)
                {
                    if (objTab != null)
                    {
                        if (!objTab.IsDeleted)
                        {
                            ModuleController objModules = new ModuleController();

                            foreach (
                                KeyValuePair<int, ModuleInfo> pair in objModules.GetTabModules(objTab.TabID)
                                )
                            {
                                ModuleInfo objModule = pair.Value;

                                if (!objModule.IsDeleted)
                                {
                                    if (objModule.DesktopModuleID == objDesktopModuleInfo.DesktopModuleID)
                                    {
                                        if (!objModule.IsDeleted)
                                        {
                                            string strPath = objTab.TabName;
                                            TabInfo objTabSelected = objTab;

                                            while (objTabSelected.ParentId != Null.NullInteger)
                                            {
                                                objTabSelected = objTabController.GetTab(
                                                    objTabSelected.ParentId, objTab.PortalID, false);
                                                if (objTabSelected == null)
                                                {
                                                    break;
                                                }
                                                strPath = string.Format("{0} -> {1}", objTabSelected.TabName,
                                                                        strPath);
                                            }

                                            ListItem objListItem = new ListItem
                                            {
                                                Value =
                                                    string.Format("{0}-{1}",
                                                                  objModule.TabID,
                                                                  objModule.
                                                                      ModuleID),
                                                Text =
                                                    string.Format("{0} -> {1}",
                                                                  strPath,
                                                                  objModule.
                                                                      ModuleTitle)
                                            };

                                            YafInstances.Items.Add(objListItem);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

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
                ModuleController objModules = new ModuleController();

                if (YafInstances.Items.Count > 0)
                {

                    /*if (ddLTabs.SelectedValue != null && ddLModules.SelectedValue != null)
                    {
                        objModules.UpdateTabModuleSetting(TabModuleId, "YafPage", ddLTabs.SelectedValue);
                        objModules.UpdateTabModuleSetting(TabModuleId, "YafModuleId", ddLModules.SelectedValue);
                    }*/

                    string[] values = YafInstances.SelectedValue.Split(Convert.ToChar("-"));

                    if (values.Length == 2)
                    {
                        objModules.UpdateTabModuleSetting(TabModuleId, "YafPage", values[0]);
                        objModules.UpdateTabModuleSetting(TabModuleId, "YafModuleId", values[1]);
                    }
                }


                if (IsNumeric(txtMaxResult.Text) || !string.IsNullOrEmpty(txtMaxResult.Text))
                {
                    objModules.UpdateTabModuleSetting(TabModuleId, "YafMaxPosts", txtMaxResult.Text);
                }
                else
                {
                    objModules.UpdateTabModuleSetting(TabModuleId, "YafMaxPosts", "10");
                }

            }

            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        /// <summary>
        /// Check if Object is a Number
        /// </summary>
        /// <param name="valueToCheck">Object to Check</param>
        /// <returns>Returns bool Value</returns>
        private static bool IsNumeric(object valueToCheck)
        {
            double dummy;
            string inputValue = Convert.ToString(valueToCheck);

            bool numeric = double.TryParse(inputValue, NumberStyles.Any, null, out dummy);

            return numeric;
        }
    }

}

