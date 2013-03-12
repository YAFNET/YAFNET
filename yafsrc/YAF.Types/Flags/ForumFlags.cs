/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
namespace YAF.Types.Flags
{
  using System;

  /// <summary>
  /// The forum flags.
  /// </summary>
  [Serializable]
  public class ForumFlags : FlagsBase
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumFlags"/> class.
    /// </summary>
    public ForumFlags()
      : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public ForumFlags(Flags flags)
      : this((int) flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public ForumFlags(object bitValue)
      : this((int) bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public ForumFlags(int bitValue)
      : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public ForumFlags(params bool[] bits)
      : base(bits)
    {
    }

    #endregion

    #region Operators

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="newBitValue">
    /// The new bit value.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator ForumFlags(int newBitValue)
    {
      return new ForumFlags(newBitValue);
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator ForumFlags(Flags flags)
    {
      return new ForumFlags(flags);
    }

    #endregion

    #region Flags Enumeration

    /// <summary>
    /// Use for bit comparisons
    /// </summary>
    [Flags]
    public enum Flags : int
    {
      /// <summary>
      /// The is locked.
      /// </summary>
      IsLocked = 1, 

      /// <summary>
      /// The is hidden.
      /// </summary>
      IsHidden = 2, 

      /// <summary>
      /// The is test.
      /// </summary>
      IsTest = 4, 

      /// <summary>
      /// The is moderated.
      /// </summary>
      IsModerated = 8

      /* for future use
			xxxxx = 16,
			xxxxx = 32,
			xxxxx = 64,
			xxxxx = 128,
			xxxxx = 256,
			xxxxx = 512
			 */
    }

    #endregion

    #region Single Flags (can be 32 of them)

    /// <summary>
    /// Gets or sets whether forum allows locked. No posting/activity can be made in locked forums.
    /// </summary>
    public bool IsLocked
    {
      // int value 1
      get
      {
        return this[0];
      }

      set
      {
        this[0] = value;
      }
    }


    /// <summary>
    /// Gets or sets whether forum is hidden to users without read access.
    /// </summary>
    public bool IsHidden
    {
      // int value 2
      get
      {
        return this[1];
      }

      set
      {
        this[1] = value;
      }
    }


    /// <summary>
    /// Gets or sets whether forum does not count to users' postcount.
    /// </summary>
    public bool IsTest
    {
      // int value 4
      get
      {
        return this[2];
      }

      set
      {
        this[2] = value;
      }
    }


    /// <summary>
    /// Gets or sets whether forum is moderated. Posts in moderated posts has to be approved by moderator before they are published.
    /// </summary>
    public bool IsModerated
    {
      // int value 8
      get
      {
        return this[3];
      }

      set
      {
        this[3] = value;
      }
    }

    #endregion
  }
}