/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Core
{
  #region Using

  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The i yaf theme extensions.
  /// </summary>
  public static class IYafThemeExtensions
  {
    #region Public Methods

    /// <summary>
    /// Gets the collapsible panel image url (expanded or collapsed).
    /// </summary>
    /// <param name="theme">
    /// The theme.
    /// </param>
    /// <param name="panelID">
    /// ID of collapsible panel
    /// </param>
    /// <param name="defaultState">
    /// Default Panel State
    /// </param>
    /// <returns>
    /// Image URL
    /// </returns>
    public static string GetCollapsiblePanelImageURL(
      [NotNull] this ITheme theme, [NotNull] string panelID, CollapsiblePanelState defaultState)
    {
      CodeContracts.VerifyNotNull(theme, "theme");
      CodeContracts.VerifyNotNull(panelID, "panelID");

      CollapsiblePanelState stateValue = YafContext.Current.Get<IYafSession>().PanelState[panelID];
      if (stateValue == CollapsiblePanelState.None)
      {
        stateValue = defaultState;
        YafContext.Current.Get<IYafSession>().PanelState[panelID] = defaultState;
      }

      return theme.GetItem("ICONS", stateValue == CollapsiblePanelState.Expanded ? "PANEL_COLLAPSE" : "PANEL_EXPAND");
    }

    #endregion
  }
}