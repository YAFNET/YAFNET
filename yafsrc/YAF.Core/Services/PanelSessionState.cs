/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Services
{
  using System;
  using System.Web;

  using YAF.Core.Context;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

    /// <summary>
  /// The panel session state.
  /// </summary>
  public class PanelSessionState : IPanelSessionState
  {
    #region Indexers

    /// <summary>
    ///   Gets panel session state.
    /// </summary>
    /// <param name = "panelID">panelID</param>
    /// <returns></returns>
    public CollapsiblePanelState this[[NotNull] string panelID]
    {
        // Ederon : 7/14/2007
        get
        {
            var sessionPanelID = $"panelstate_{panelID}";

            // try to get panel state from session state first
            if (BoardContext.Current.Get<HttpSessionStateBase>()[sessionPanelID] != null)
            {
                return (CollapsiblePanelState)BoardContext.Current.Get<HttpSessionStateBase>()[sessionPanelID];
            }

            // if no panel state info is in session state, try cookie
            if (BoardContext.Current.Get<HttpRequestBase>().Cookies[sessionPanelID] != null)
            {
                try
                {
                    // we must convert string to int, better get is safe
                    if (BoardContext.Current.Get<HttpRequestBase>() != null)
                    {
                        return (CollapsiblePanelState)int.Parse(
                            BoardContext.Current.Get<HttpRequestBase>().Cookies[sessionPanelID].Value);
                    }
                }
                catch
                {
                    // in case cookie has wrong value
                    if (BoardContext.Current.Get<HttpRequestBase>() != null)
                    {
                        BoardContext.Current.Get<HttpRequestBase>().Cookies.Remove(sessionPanelID); // scrap wrong cookie
                    }

                    return CollapsiblePanelState.None;
                }
            }

            return CollapsiblePanelState.None;
        }

        // Ederon : 7/14/2007
        set
        {
            var sessionPanelID = $"panelstate_{panelID}";

            BoardContext.Current.Get<HttpSessionStateBase>()[sessionPanelID] = value;

            // create persistent cookie with visibility setting for panel
            var c = new HttpCookie(sessionPanelID, ((int)value).ToString()) { Expires = System.DateTime.UtcNow.AddYears(1) };
            BoardContext.Current.Get<HttpResponseBase>().SetCookie(c);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The toggle panel state.
    /// </summary>
    /// <param name="panelID">
    /// The panel id.
    /// </param>
    /// <param name="defaultState">
    /// The default state.
    /// </param>
    public void TogglePanelState([NotNull] string panelID, CollapsiblePanelState defaultState)
    {
      var currentState = this[panelID];

      if (currentState == CollapsiblePanelState.None)
      {
        currentState = defaultState;
      }

      if (currentState == CollapsiblePanelState.Collapsed)
      {
        this[panelID] = CollapsiblePanelState.Expanded;
      }
      else if (currentState == CollapsiblePanelState.Expanded)
      {
        this[panelID] = CollapsiblePanelState.Collapsed;
      }
    }

    #endregion
  }
}