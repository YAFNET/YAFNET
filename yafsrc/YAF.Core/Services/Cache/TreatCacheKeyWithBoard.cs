/* YetAnotherForum.NET
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
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The treat cache key with board.
  /// </summary>
  public class TreatCacheKeyWithBoard : ITreatCacheKey
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TreatCacheKeyWithBoard"/> class.
    /// </summary>
    /// <param name="haveBoardId">
    /// The board id.
    /// </param>
    public TreatCacheKeyWithBoard([NotNull] IHaveBoardID haveBoardId)
    {
      CodeContracts.VerifyNotNull(haveBoardId, "haveBoardId");

      this.HaveBoardId = haveBoardId;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets HaveBoardId.
    /// </summary>
    public IHaveBoardID HaveBoardId { get; set; }

    #endregion

    #region Implemented Interfaces

    #region ITreatCacheKey

    /// <summary>
    /// The treat.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The treat.
    /// </returns>
    public string Treat(string key)
    {
      return "{0}${1}".FormatWith(key, this.HaveBoardId.BoardID);
    }

    #endregion

    #endregion
  }
}