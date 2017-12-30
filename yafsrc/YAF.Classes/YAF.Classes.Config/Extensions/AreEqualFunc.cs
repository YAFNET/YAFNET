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
namespace YAF.Utils
{
  #region Using

  using System;
  using System.Collections.Generic;

  using YAF.Types;

  #endregion

  /// <summary>
  /// The are equal func.
  /// </summary>
  /// <typeparam name="T">
  /// </typeparam>
  public class AreEqualFunc<T> : IEqualityComparer<T>
  {
    #region Constants and Fields

    /// <summary>
    /// The _comparer.
    /// </summary>
    private readonly Func<T, T, bool> _comparer;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AreEqualFunc{T}"/> class.
    /// </summary>
    /// <param name="comparer">
    /// The comparer.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    public AreEqualFunc([NotNull] Func<T, T, bool> comparer)
    {
      CodeContracts.VerifyNotNull(comparer, "comparer");

      this._comparer = comparer;
    }

    #endregion

    #region Implemented Interfaces

    #region IEqualityComparer<T>

    /// <summary>
    /// The equals.
    /// </summary>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <returns>
    /// The equals.
    /// </returns>
    public bool Equals(T x, T y)
    {
      return this._comparer(x, y);
    }

    /// <summary>
    /// The get hash code.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <returns>
    /// The get hash code.
    /// </returns>
    public int GetHashCode(T obj)
    {
      return obj.ToString().ToLower().GetHashCode();
    }

    #endregion

    #endregion
  }
}