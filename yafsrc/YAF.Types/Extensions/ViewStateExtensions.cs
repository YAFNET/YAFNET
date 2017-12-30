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
namespace YAF.Types.Extensions
{
  #region Using

    using System;
    using System.Web.UI;

    #endregion

  /// <summary>
  /// The view state extensions.
  /// </summary>
  public static class ViewStateExtensions
  {
    #region Public Methods

    /// <summary>
    /// Returns the converted type (T) if ViewState[key] != <see langword="null"/> or
    /// <paramref name="defaultValue"/> if it's <see langword="null"/>.
    /// </summary>
    /// <param name="viewState">
    /// The view state.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T ToTypeOrDefault<T>(this StateBag viewState, string key, T defaultValue)
    {
      if (viewState[key] != null)
      {
        return viewState[key].ToType<T>();
      }

      return default(T);
    }

    /// <summary>
    /// Returns the converted type (T) if ViewState[key] != <see langword="null"/> or
    /// <paramref name="defaultFunc"/> if it's <see langword="null"/>.
    /// </summary>
    /// <param name="viewState">
    /// The view state.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="defaultFunc">
    /// The default func.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T ToTypeOrFunc<T>(this StateBag viewState, string key, Func<T> defaultFunc)
    {
      if (viewState[key] != null)
      {
        return viewState[key].ToType<T>();
      }

      return defaultFunc();
    }

    #endregion
  }
}