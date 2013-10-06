/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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

      int keyHash = originalKey.GetHashCode();

      // make positive if negative...
      if (keyHash < 0)
      {
        keyHash = -keyHash;
      }

      // get the lock item id (value between 0 and objectCount)
      int lockItemId = keyHash % LockObjectCount;

      // init the lock object if it hasn't been created yet...
      return this.LockCacheItems[lockItemId] ?? (this.LockCacheItems[lockItemId] = new object());
    }

    #endregion

    #endregion
  }
}