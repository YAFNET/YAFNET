/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
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
namespace YAF.Core.Nntp
{
  using System;

  /// <summary>
  /// The newsgroup.
  /// </summary>
  public class Newsgroup : IComparable
  {
    /// <summary>
    /// The group.
    /// </summary>
    protected string group;

    /// <summary>
    /// The high.
    /// </summary>
    protected int high;

    /// <summary>
    /// The low.
    /// </summary>
    protected int low;

    /// <summary>
    /// Initializes a new instance of the <see cref="Newsgroup"/> class.
    /// </summary>
    public Newsgroup()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Newsgroup"/> class.
    /// </summary>
    /// <param name="group">
    /// The group.
    /// </param>
    /// <param name="low">
    /// The low.
    /// </param>
    /// <param name="high">
    /// The high.
    /// </param>
    public Newsgroup(string group, int low, int high)
    {
      this.group = group;
      this.low = low;
      this.high = high;
    }

    /// <summary>
    /// Gets or sets Group.
    /// </summary>
    public string Group
    {
      get
      {
        return this.group;
      }

      set
      {
        this.group = value;
      }
    }

    /// <summary>
    /// Gets or sets Low.
    /// </summary>
    public int Low
    {
      get
      {
        return this.low;
      }

      set
      {
        this.low = value;
      }
    }

    /// <summary>
    /// Gets or sets High.
    /// </summary>
    public int High
    {
      get
      {
        return this.high;
      }

      set
      {
        this.high = value;
      }
    }

    #region IComparable Members

    /// <summary>
    /// The compare to.
    /// </summary>
    /// <param name="r">
    /// The r.
    /// </param>
    /// <returns>
    /// The compare to.
    /// </returns>
    public int CompareTo(object r)
    {
      return this.Group.CompareTo(((Newsgroup) r).Group);
    }

    #endregion
  }
}