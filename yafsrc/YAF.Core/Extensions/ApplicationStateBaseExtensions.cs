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

  using System;
  using System.Web;

  using YAF.Types;

  #endregion

  /// <summary>
  /// The http application state base extensions.
  /// </summary>
  public static class HttpApplicationStateBaseExtensions
  {
    #region Public Methods

    /// <summary>
    /// The get or set.
    /// </summary>
    /// <param name="httpApplicationState">
    /// The http application state.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="getValue">
    /// The get value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T GetOrSet<T>(
      [NotNull] this HttpApplicationStateBase httpApplicationState, [NotNull] string key, [NotNull] Func<T> getValue)
    {
      CodeContracts.VerifyNotNull(httpApplicationState, "httpApplicationState");
      CodeContracts.VerifyNotNull(key, "key");
      CodeContracts.VerifyNotNull(getValue, "getValue");

      var item = httpApplicationState[key];

      if (Equals(item, default(T)))
      {
        try
        {
          httpApplicationState.Lock();

          item = httpApplicationState[key];

          if (Equals(item, default(T)))
          {
            item = getValue();
            httpApplicationState[key] = item;
          }
        }
        finally
        {
          httpApplicationState.UnLock();
        }
      }

      return (T)item;
    }

    /// <summary>
    /// The set.
    /// </summary>
    /// <param name="httpApplicationState">
    /// The http application state.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public static void Set<T>(
      [NotNull] this HttpApplicationStateBase httpApplicationState, [NotNull] string key, [NotNull] T value)
    {
      CodeContracts.VerifyNotNull(httpApplicationState, "httpApplicationState");
      CodeContracts.VerifyNotNull(key, "key");

      try
      {
        httpApplicationState.Lock();
        httpApplicationState[key] = value;
      }
      finally
      {
        httpApplicationState.UnLock();
      }
    }

    #endregion
  }
}