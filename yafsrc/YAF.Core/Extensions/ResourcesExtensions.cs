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

  using YAF.Types;

  #endregion

  /// <summary>
  /// The resources extensions.
  /// </summary>
  public static class ResourcesExtensions
  {
    #region Public Methods

    /// <summary>
    /// The get hours offset.
    /// </summary>
    /// <param name="lanuageResource">
    /// The resource.
    /// </param>
    /// <returns>
    /// The get hours offset.
    /// </returns>
    public static decimal GetHoursOffset(this LanuageResourcesPageResource lanuageResource)
    {
      // calculate hours -- can use prefix of either UTC or GMT...
      decimal hours = 0;

      try
      {
        hours = Convert.ToDecimal(lanuageResource.tag.Replace("UTC", string.Empty).Replace("GMT", string.Empty));
      }
      catch (FormatException)
      {
        hours =
          Convert.ToDecimal(lanuageResource.tag.Replace(".", ",").Replace("UTC", string.Empty).Replace("GMT", string.Empty));
      }

      return hours;
    }

    #endregion
  }
}