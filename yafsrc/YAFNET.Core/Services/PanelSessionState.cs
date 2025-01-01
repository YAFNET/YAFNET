/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Services;

using System;

/// <summary>
/// The panel session state.
/// </summary>
public class PanelSessionState : IPanelSessionState
{
    /// <summary>
    ///   Gets panel session state.
    /// </summary>
    public CollapsiblePanelState this[string panelID]
    {
        // Ederon : 7/14/2007
        get
        {
            var context = BoardContext.Current.Get<IHttpContextAccessor>().HttpContext;

            var sessionPanelID = $"panelstate_{panelID}";

            // try to get panel state from session state first
            if (context.Session.GetString(sessionPanelID).IsSet())
            {
                return context.Session.GetData<CollapsiblePanelState>(sessionPanelID);
            }

            // if no panel state info is in session state, try cookie
            if (context.Request.Cookies[sessionPanelID] == null)
            {
                return CollapsiblePanelState.None;
            }

            try
            {
                // we must convert string to int, better get is safe
                return (CollapsiblePanelState)int.Parse(
                    context.Request.Cookies[sessionPanelID]);
            }
            catch
            {
                // in case cookie has wrong value
                context.Request.Cookies.Keys
                    .Remove(sessionPanelID); // scrap wrong cookie

                return CollapsiblePanelState.None;
            }
        }

        // Ederon : 7/14/2007
        set
        {
            var context = BoardContext.Current.Get<IHttpContextAccessor>().HttpContext;

            var sessionPanelID = $"panelstate_{panelID}";

            context.Session.SetData(sessionPanelID, value);

            // create persistent cookie with visibility setting for panel
            context.Response.Cookies.Append(
                sessionPanelID,
                ((int)value).ToString(),
                new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) });
        }
    }

    /// <summary>
    /// Toggle Panel State
    /// </summary>
    /// <param name="panelID">
    /// The panel id.
    /// </param>
    /// <param name="defaultState">
    /// The default state.
    /// </param>
    public void TogglePanelState(string panelID, CollapsiblePanelState defaultState)
    {
        var currentState = this[panelID];

        if (currentState == CollapsiblePanelState.None)
        {
            currentState = defaultState;
        }

        this[panelID] = currentState switch
            {
                CollapsiblePanelState.Collapsed => CollapsiblePanelState.Expanded,
                CollapsiblePanelState.Expanded => CollapsiblePanelState.Collapsed,
                _ => this[panelID]
            };
    }
}