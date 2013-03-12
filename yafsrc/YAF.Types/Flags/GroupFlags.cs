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
  /// The group flags.
  /// </summary>
  [Serializable]
  public class GroupFlags : FlagsBase
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupFlags"/> class.
    /// </summary>
    public GroupFlags()
      : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public GroupFlags(Flags flags)
      : this((int) flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public GroupFlags(object bitValue)
      : this((int) bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public GroupFlags(int bitValue)
      : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public GroupFlags(params bool[] bits)
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
    public static implicit operator GroupFlags(int newBitValue)
    {
      return new GroupFlags(newBitValue);
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator GroupFlags(Flags flags)
    {
      return new GroupFlags(flags);
    }

    #endregion

    #region Flags Enumeration

    /// <summary>
    /// Use for bit comparisons
    /// </summary>
    [Flags]
    public enum Flags : int
    {
      None = 0,

      /// <summary>
      /// The is admin.
      /// </summary>
      IsAdmin = 1, 

      /// <summary>
      /// The is guest.
      /// </summary>
      IsGuest = 2, 

      /// <summary>
      /// The is start.
      /// </summary>
      IsStart = 4, 

      /// <summary>
      /// The is moderator.
      /// </summary>
      IsModerator = 8,

      /// <summary>
      /// The is hidden.
      /// </summary>
      IsHidden = 16,

      /// <summary>
      /// The is user group.
      /// </summary>
      IsUserGroup = 32

      /* for future use		
			xxxxx = 64,
			xxxxx = 128,
			xxxxx = 256,
			xxxxx = 512
			 */
    }

    #endregion

    #region Single Flags (can be 32 of them)

    /// <summary>
    /// Gets or sets whether group/role has administrator privilegies
    /// </summary>
    public bool IsAdmin
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
    /// Gets or sets whether group/role is guest role.
    /// </summary>
    public bool IsGuest
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
    /// Gets or sets whether group/role is starting role for new users.
    /// </summary>
    public bool IsStart
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
    /// Gets or sets whether group/role has moderator privilegies.
    /// </summary>
    public bool IsModerator
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

    /// <summary>
    /// Gets or sets whether group/role has moderator privilegies.
    /// </summary>
    public bool IsHidden
    {
        // int value 16
        get
        {
            return this[4];
        }

        set
        {
            this[4] = value;
        }
    }

    /// <summary>
    /// Gets or sets whether this is a user group.
    /// </summary>
    public bool IsUserGroup
    {
        // int value 32
        get
        {
            return this[5];
        }

        set
        {
            this[5] = value;
        }
    }

    #endregion
  }
}