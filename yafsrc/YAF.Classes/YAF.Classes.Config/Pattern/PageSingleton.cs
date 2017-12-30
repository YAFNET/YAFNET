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
namespace YAF.Classes.Pattern
{
  #region Using

  using System;
  using System.Web;

  #endregion

  /// <summary>
  /// Singleton factory implementation
  /// </summary>
  /// <typeparam name="T">
  /// </typeparam>
  public static class PageSingleton<T>
    where T : class, new()
  {
    // static constructor, 
    // runtime ensures thread safety
    #region Constants and Fields

    /// <summary>
    ///   The _instance.
    /// </summary>
    private static T _instance;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Instance.
    /// </summary>
    public static T Instance
    {
      get
      {
        return GetInstance();
      }

      private set
      {
        _instance = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get instance.
    /// </summary>
    /// <returns>
    /// </returns>
    private static T GetInstance()
    {
      if (HttpContext.Current == null)
      {
        return _instance ?? (_instance = (T)Activator.CreateInstance(typeof(T)));
      }

      string typeStr = typeof(T).ToString();

      return
        (T)
        (HttpContext.Current.Items[typeStr] ??
         (HttpContext.Current.Items[typeStr] = Activator.CreateInstance(typeof(T))));
    }

    #endregion
  }
}