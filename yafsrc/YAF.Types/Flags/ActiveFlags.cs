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
  /// The access flags.
  /// </summary>
  public class ActiveFlags : FlagsBase
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveFlags"/> class.
    /// </summary>
    public ActiveFlags()
      : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public ActiveFlags(Flags flags)
      : this((int) flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public ActiveFlags(object bitValue)
      : this((int) bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public ActiveFlags(int bitValue)
      : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public ActiveFlags(params bool[] bits)
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
    public static implicit operator ActiveFlags(int newBitValue)
    {
      var flags = new ActiveFlags(newBitValue);
      return flags;
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator ActiveFlags(Flags flags)
    {
        return new ActiveFlags(flags);
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
      /// The Is Active Now.
      /// </summary>
      IsActiveNow = 1,

      /// <summary>
      /// The Is Guest.
      /// </summary>
      IsGuest = 2,

      /// <summary>
      /// The Is Registered.
      /// </summary>
      IsRegistered = 3, 

      /// <summary>
      /// The Is Crawler.
      /// </summary>
      IsCrawler = 4

    }

    #endregion

    #region Single Flags (can be 32 of them)

    /// <summary>
    /// Gets or sets the user is active right now.
    /// </summary>
    public bool IsActiveNow
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
    /// Gets or sets that user is a guest.
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
    /// Gets or sets the user is a registered one.
    /// </summary>
    public bool IsRegistered
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
    /// Gets or sets that user is a crawler.
    /// </summary>
    public bool IsCrawler
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