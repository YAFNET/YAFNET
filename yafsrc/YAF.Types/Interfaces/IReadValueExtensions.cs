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
namespace YAF.Types.Interfaces
{
  #region Using

    using System;

    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The i read value extensions.
  /// </summary>
  public static class IReadValueExtensions
  {
    #region Public Methods

    /// <summary>
    /// Gets a value with a default value...
    /// </summary>
    /// <param name="readValue">
    /// </param>
    /// <param name="key">
    /// </param>
    /// <param name="getValue">
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T Get<T>([NotNull] this IReadValue<T> readValue, [NotNull] string key, [NotNull] Func<T> getValue)
    {
      CodeContracts.VerifyNotNull(readValue, "readValue");
      CodeContracts.VerifyNotNull(key, "key");
      CodeContracts.VerifyNotNull(getValue, "getValue");

      var value = readValue.Get(key);

      return Equals(value, default(T)) ? getValue() : value;
    }

    /// <summary>
    /// Gets a value with a default value...
    /// </summary>
    /// <param name="readValue">
    /// </param>
    /// <param name="key">
    /// </param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T Get<T>([NotNull] this IReadValue<T> readValue, [NotNull] string key, [CanBeNull] T defaultValue)
    {
      CodeContracts.VerifyNotNull(readValue, "readValue");
      CodeContracts.VerifyNotNull(key, "key");

      var value = readValue.Get(key);

      return Equals(value, default(T)) ? defaultValue : value;
    }

    /// <summary>
    /// The get as bool.
    /// </summary>
    /// <param name="readValue">
    /// The read value.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <returns>
    /// The get as bool.
    /// </returns>
    public static bool GetAsBool([NotNull] this IReadValue<string> readValue, [NotNull] string key, bool defaultValue)
    {
      CodeContracts.VerifyNotNull(readValue, "readValue");
      CodeContracts.VerifyNotNull(key, "key");

      var value = readValue.Get(key);

      return Equals(value, null) ? defaultValue : Convert.ToBoolean(value.ToLower());
    }

    #endregion
  }
}