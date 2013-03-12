/* Yet Another Forum.NET
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
namespace YAF.Types.Objects
{
  #region Using

  using System;
  using System.Data;

  #endregion

  /// <summary>
  /// The typed all thanks.
  /// </summary>
  [Serializable]
  public class TypedAllThanks
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedAllThanks"/> class.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    public TypedAllThanks(DataRow row)
    {
      this.MessageID = row.Field<int?>("MessageID");
      this.FromUserID = row.Field<int?>("FromUserID");
      this.ThanksDate = row.Field<DateTime?>("ThanksDate");
      this.ThanksFromUserNumber = row.Field<int?>("ThanksFromUserNumber");
      this.ThanksToUserNumber = row.Field<int?>("ThanksToUserNumber");
      this.ThanksToUserPostsNumber = row.Field<int?>("ThanksToUserPostsNumber");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedAllThanks"/> class.
    /// </summary>
    /// <param name="messageid">
    /// The messageid.
    /// </param>
    /// <param name="fromuserid">
    /// The fromuserid.
    /// </param>
    /// <param name="thanksdate">
    /// The thanksdate.
    /// </param>
    /// <param name="thanksfromusernumber">
    /// The thanksfromusernumber.
    /// </param>
    /// <param name="thankstousernumber">
    /// The thankstousernumber.
    /// </param>
    /// <param name="thankstouserpostsnumber">
    /// The thankstouserpostsnumber.
    /// </param>
    public TypedAllThanks(
      int? messageid, 
      int? fromuserid, 
      DateTime? thanksdate, 
      int? thanksfromusernumber, 
      int? thankstousernumber, 
      int? thankstouserpostsnumber)
    {
      this.MessageID = messageid;
      this.FromUserID = fromuserid;
      this.ThanksDate = thanksdate;
      this.ThanksFromUserNumber = thanksfromusernumber;
      this.ThanksToUserNumber = thankstousernumber;
      this.ThanksToUserPostsNumber = thankstouserpostsnumber;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets FromUserID.
    /// </summary>
    public int? FromUserID { get; set; }

    /// <summary>
    /// Gets or sets MessageID.
    /// </summary>
    public int? MessageID { get; set; }

    /// <summary>
    /// Gets or sets ThanksDate.
    /// </summary>
    public DateTime? ThanksDate { get; set; }

    /// <summary>
    /// Gets or sets ThanksFromUserNumber.
    /// </summary>
    public int? ThanksFromUserNumber { get; set; }

    /// <summary>
    /// Gets or sets ThanksToUserNumber.
    /// </summary>
    public int? ThanksToUserNumber { get; set; }

    /// <summary>
    /// Gets or sets ThanksToUserPostsNumber.
    /// </summary>
    public int? ThanksToUserPostsNumber { get; set; }

    #endregion
  }
}