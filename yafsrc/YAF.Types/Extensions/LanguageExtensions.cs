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

    using YAF.Types;

    #endregion

  /// <summary>
  /// The using extensions.
  /// </summary>
  public static class LanguageExtensions
  {
    #region Public Methods

    /// <summary>
    /// The using.
    /// </summary>
    /// <param name="anyObj">
    /// The any obj.
    /// </param>
    /// <param name="makeUsing1">
    /// The make using 1.
    /// </param>
    /// <param name="action">
    /// The action.
    /// </param>
    /// <typeparam name="TAny">
    /// </typeparam>
    /// <typeparam name="T1">
    /// </typeparam>
    public static void Using<TAny, T1>(this TAny anyObj, [NotNull] Func<T1> makeUsing1, [NotNull] Action<T1> action)
      where T1 : IDisposable
    {
      CodeContracts.VerifyNotNull(makeUsing1, "makeUsing1");
      CodeContracts.VerifyNotNull(action, "action");

      using (var item1 = makeUsing1())
      {
        action(item1);
      }
    }

    /// <summary>
    /// The using.
    /// </summary>
    /// <param name="anyObj">
    /// The any obj.
    /// </param>
    /// <param name="makeUsing1">
    /// The make using 1.
    /// </param>
    /// <param name="makeUsing2">
    /// The make using 2.
    /// </param>
    /// <param name="action">
    /// The action.
    /// </param>
    /// <typeparam name="TAny">
    /// </typeparam>
    /// <typeparam name="T1">
    /// </typeparam>
    /// <typeparam name="T2">
    /// </typeparam>
    public static void Using<TAny, T1, T2>(
      this TAny anyObj, [NotNull] Func<T1> makeUsing1, [NotNull] Func<T2> makeUsing2, [NotNull] Action<T1, T2> action)
      where T1 : IDisposable where T2 : IDisposable
    {
      CodeContracts.VerifyNotNull(makeUsing1, "makeUsing1");
      CodeContracts.VerifyNotNull(makeUsing2, "makeUsing2");
      CodeContracts.VerifyNotNull(action, "action");

      using (var item1 = makeUsing1())
      {
        using (var item2 = makeUsing2())
        {
          action(item1, item2);
        }
      }
    }

    #endregion
  }
}