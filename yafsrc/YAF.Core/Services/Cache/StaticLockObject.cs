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
namespace YAF.Core.Services.Cache
{
  #region Using

    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The static lock object.
  /// </summary>
  public class StaticLockObject : IHaveLockObject
  {
    #region Constants and Fields

    /// <summary>
    /// The lock cache items.
    /// </summary>
    protected readonly object[] LockCacheItems;

    /// <summary>
    /// The lock object count.
    /// </summary>
    private const int LockObjectCount = 300;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticLockObject"/> class.
    /// </summary>
    public StaticLockObject()
    {
      this.LockCacheItems = new object[LockObjectCount + 1];
    }

    #endregion

    #region Implemented Interfaces

    #region IReadValue<object>

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="originalKey">
    /// The key.
    /// </param>
    /// <returns>
    /// The get.
    /// </returns>
    [NotNull]
    public object Get([NotNull] string originalKey)
    {
      CodeContracts.VerifyNotNull(originalKey, "key");

      var keyHash = originalKey.GetHashCode();

      // make positive if negative...
      if (keyHash < 0)
      {
        keyHash = -keyHash;
      }

      // get the lock item id (value between 0 and objectCount)
      var lockItemId = keyHash % LockObjectCount;

      // init the lock object if it hasn't been created yet...
      return this.LockCacheItems[lockItemId] ?? (this.LockCacheItems[lockItemId] = new object());
    }

    #endregion

    #endregion
  }
}