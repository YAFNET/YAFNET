/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
using System;

namespace YAF.Types.Objects
{
  #region Using

  using System.Data;

  #endregion

  /// <summary>
  /// The typed smiley list.
  /// </summary>
  [Serializable]
  public class TypedSmileyList
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedSmileyList"/> class.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    public TypedSmileyList([NotNull] DataRow row)
    {
      this.SmileyID = row.Field<int?>("SmileyID");
      this.BoardID = row.Field<int?>("BoardID");
      this.Code = row.Field<string>("Code");
      this.Icon = row.Field<string>("Icon");
      this.Emoticon = row.Field<string>("Emoticon");
      this.SortOrder = Convert.ToInt32(row["SortOrder"]);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedSmileyList"/> class.
    /// </summary>
    /// <param name="smileyid">
    /// The smileyid.
    /// </param>
    /// <param name="boardid">
    /// The boardid.
    /// </param>
    /// <param name="code">
    /// The code.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="emoticon">
    /// The emoticon.
    /// </param>
    /// <param name="sortorder">
    /// The sortorder.
    /// </param>
    public TypedSmileyList(int? smileyid, int? boardid, [NotNull] string code, [CanBeNull] string icon, [CanBeNull] string emoticon, int? sortorder)
    {
      this.SmileyID = smileyid;
      this.BoardID = boardid;
      this.Code = code;
      this.Icon = icon;
      this.Emoticon = emoticon;
      this.SortOrder = sortorder;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets BoardID.
    /// </summary>
    public int? BoardID { get; set; }

    /// <summary>
    /// Gets or sets Code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets Emoticon.
    /// </summary>
    public string Emoticon { get; set; }

    /// <summary>
    /// Gets or sets Icon.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Gets or sets SmileyID.
    /// </summary>
    public int? SmileyID { get; set; }

    /// <summary>
    /// Gets or sets SortOrder.
    /// </summary>
    public int? SortOrder { get; set; }

    #endregion
  }
}