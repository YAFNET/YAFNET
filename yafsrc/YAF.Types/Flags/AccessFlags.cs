/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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
namespace YAF.Types.Flags
{
  using System;

  /// <summary>
  /// The access flags.
  /// </summary>
  public class AccessFlags : FlagsBase
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessFlags"/> class.
    /// </summary>
    public AccessFlags()
      : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public AccessFlags(Flags flags)
      : this((int) flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public AccessFlags(object bitValue)
      : this((int) bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public AccessFlags(int bitValue)
      : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public AccessFlags(params bool[] bits)
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
    public static implicit operator AccessFlags(int newBitValue)
    {
      var flags = new AccessFlags(newBitValue);
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
    public static implicit operator AccessFlags(Flags flags)
    {
      return new AccessFlags(flags);
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
      /// The read access.
      /// </summary>
      ReadAccess = 1, 

      /// <summary>
      /// The post access.
      /// </summary>
      PostAccess = 2, 

      /// <summary>
      /// The reply access.
      /// </summary>
      ReplyAccess = 4, 

      /// <summary>
      /// The priority access.
      /// </summary>
      PriorityAccess = 8, 

      /// <summary>
      /// The poll access.
      /// </summary>
      PollAccess = 16, 

      /// <summary>
      /// The vote access.
      /// </summary>
      VoteAccess = 32, 

      /// <summary>
      /// The moderator access.
      /// </summary>
      ModeratorAccess = 64, 

      /// <summary>
      /// The edit access.
      /// </summary>
      EditAccess = 128, 

      /// <summary>
      /// The delete access.
      /// </summary>
      DeleteAccess = 256, 

      /// <summary>
      /// The upload access.
      /// </summary>
      UploadAccess = 512, 

      /// <summary>
      /// The download access.
      /// </summary>
      DownloadAccess = 1024
    }

    #endregion

    #region Single Flags (can be 32 of them)

    /// <summary>
    /// Gets or sets read access right.
    /// </summary>
    public bool ReadAccess
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
    /// Gets or sets post access right.
    /// </summary>
    public bool PostAccess
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
    /// Gets or sets reply access right.
    /// </summary>
    public bool ReplyAccess
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
    /// Gets or sets priority access right.
    /// </summary>
    public bool PriorityAccess
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
    /// Gets or sets poll access right.
    /// </summary>
    public bool PollAccess
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
    /// Gets or sets vote access right.
    /// </summary>
    public bool VoteAccess
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

    /// <summary>
    /// Gets or sets moderator access right.
    /// </summary>
    public bool ModeratorAccess
    {
      // int value 64
      get
      {
        return this[6];
      }

      set
      {
        this[6] = value;
      }
    }

    /// <summary>
    /// Gets or sets edit access right.
    /// </summary>
    public bool EditAccess
    {
      // int value 128
      get
      {
        return this[7];
      }

      set
      {
        this[7] = value;
      }
    }

    /// <summary>
    ///Gets or sets delete access right.
    /// </summary>
    public bool DeleteAccess
    {
      // int value 256
      get
      {
        return this[8];
      }

      set
      {
        this[8] = value;
      }
    }

    /// <summary>
    /// Gets or sets upload access right.
    /// </summary>
    public bool UploadAccess
    {
      // int value 512
      get
      {
        return this[9];
      }

      set
      {
        this[9] = value;
      }
    }


    /// <summary>
    /// Gets or sets download access right.
    /// </summary>
    public bool DownloadAccess
    {
      // int value 512
      get
      {
        return this[10];
      }

      set
      {
        this[10] = value;
      }
    }

    #endregion
  }
}