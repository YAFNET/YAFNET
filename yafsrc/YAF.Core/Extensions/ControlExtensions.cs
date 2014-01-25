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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Web;
  using System.Web.UI;

  using YAF.Core;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Utils.Helpers.StringUtils;

  #endregion

  /// <summary>
  /// The control extensions.
  /// </summary>
  public static class ControlExtensions
  {
    #region Public Methods

    /// <summary>
    /// Creates a ID Based on the Control Structure
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get extended id.
    /// </returns>
    public static string GetExtendedID(this Control currentControl, string prefix)
    {
      string createdID = null;

      if (currentControl.ID.IsSet())
      {
        createdID = currentControl.ID + "_";
      }

      if (prefix.IsSet())
      {
        createdID += prefix;
      }
      else
      {
        createdID += Guid.NewGuid().ToString().Substring(0, 5);
      }

      return createdID;
    }

    /// <summary>
    /// Creates a Unique ID
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get unique id.
    /// </returns>
    public static string GetUniqueID(this Control currentControl, string prefix)
    {
      if (prefix.IsSet())
      {
        return prefix + Guid.NewGuid().ToString().Substring(0, 5);
      }
      else
      {
        return Guid.NewGuid().ToString().Substring(0, 10);
      }
    }

    /// <summary>
    /// The html encode.
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <returns>
    /// The html encode.
    /// </returns>
    public static string HtmlEncode(this Control currentControl, object data)
    {
      return HttpContext.Current.Server.HtmlEncode(data.ToString());
    }

    /// <summary>
    /// Gets PageContext.
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    public static YafContext PageContext(this Control currentControl)
    {
      if (currentControl.Site != null && currentControl.Site.DesignMode)
      {
        // design-time, return null...
        return null;
      }

      return YafContext.Current;
    }

    #endregion
  }
}