/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System;

namespace YAF.Core.Helpers;

/// <summary>
/// The collapse panel helper.
/// </summary>
public static class CollapsePanelHelper
{
    /// <summary>
    /// Gets the collapsible panel image url (expanded or collapsed).
    /// </summary>
    /// <param name="panelId">
    /// ID of collapsible panel
    /// </param>
    /// <param name="defaultState">
    /// Default Panel State
    /// </param>
    /// <returns>
    /// Image URL
    /// </returns>
    public static string GetCollapsiblePanelIcon(string panelId, CollapsiblePanelState defaultState)
    {
        ArgumentNullException.ThrowIfNull(panelId);

        var stateValue = BoardContext.Current.Get<ISessionService>().PanelState[panelId];

        if (stateValue != CollapsiblePanelState.None)
        {
            return stateValue == CollapsiblePanelState.Expanded ? "chevron-up" : "chevron-down";
        }

        stateValue = defaultState;

        BoardContext.Current.Get<ISessionService>().PanelState[panelId] = defaultState;

        return stateValue == CollapsiblePanelState.Expanded ? "chevron-up" : "chevron-down";
    }
}