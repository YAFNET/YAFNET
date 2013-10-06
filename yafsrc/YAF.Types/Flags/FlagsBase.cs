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
  #region Using

  using System;
  using System.Runtime.Serialization;
  using System.Security;

  #endregion

  /// <summary>
  /// Abstract class as a foundation for various flags implementations
  /// </summary>
  [Serializable]
  public abstract class FlagsBase
  {
    #region Constants and Fields

    /// <summary>
    ///   integer value stores up to 64 flags/bits
    /// </summary>
    protected int _bitValue;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "FlagsBase" /> class. 
    ///   Creates new instance with all bits set to false (integer 0).
    /// </summary>
    public FlagsBase()
      : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlagsBase"/> class. 
    ///   Creates new instance and initialize it with value of bitValue parameter.
    /// </summary>
    /// <param name="bitValue">
    /// Initialize integer value.
    /// </param>
    /// <summary>
    /// Initializes a new instance of the <see cref="FlagsBase"/> class. 
    ///   Creates new instance and initialize it with value of bitValue parameter.
    /// </summary>
    /// <param name="bitValue">
    /// Initialize integer value.
    /// </param>
    public FlagsBase(int bitValue)
    {
      this._bitValue = bitValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlagsBase"/> class. 
    ///   Creates new instance with bits set according to param array.
    /// </summary>
    /// <param name="bits">
    /// Boolean values to initialize class with. If their number is lower than 32, remaining bits are set to false. If more than 32 values is specified, excess values are ignored.
    /// </param>
    public FlagsBase([NotNull] params bool[] bits)
      : this(0)
    {
      // process up to 32 parameters
      for (int i = 0; i < Math.Min(bits.Length, 31); i++)
      {
        // set this bit
        this[i] = bits[i];
      }
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets integer value of flags.
    /// </summary>
    public int BitValue
    {
      get
      {
        return this._bitValue;
      }

      set
      {
        this._bitValue = value;
      }
    }

    #endregion

    #region Indexers

    /// <summary>
    ///   Gets or sets bit at position specified by index.
    /// </summary>
    /// <param name = "index">Zero-based index of bit to get or set.</param>
    /// <returns>Boolean value indicating whether bit at position specified by index is set or not.</returns>
    public bool this[int index]
    {
      get
      {
        return GetBitAsBool(this._bitValue, index);
      }

      set
      {
        this._bitValue = SetBitFromBool(this._bitValue, index, value);
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets boolean indicating whether bit on bitShift position in bitValue integer is set or not.
    /// </summary>
    /// <param name="bitValue">
    /// Integer value.
    /// </param>
    /// <param name="bitShift">
    /// Zero-based position of bit to get.
    /// </param>
    /// <returns>
    /// Returns boolean indicating whether bit at bitShift position is set or not.
    /// </returns>
    public static bool GetBitAsBool(int bitValue, int bitShift)
    {
      if (bitShift > 63)
      {
        bitShift %= 63;
      }

      if (((bitValue >> bitShift) & 0x00000001) == 1)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Sets or unsets bit of bitValue integer at position specified by bitShift, depending on value parameter.
    /// </summary>
    /// <param name="bitValue">
    /// Integer value.
    /// </param>
    /// <param name="bitShift">
    /// Zero-based position of bit to set.
    /// </param>
    /// <param name="value">
    /// New boolean value of bit.
    /// </param>
    /// <returns>
    /// Returns new integer value with bit at position specified by bitShift parameter set to value.
    /// </returns>
    public static int SetBitFromBool(int bitValue, int bitShift, bool value)
    {
      if (bitShift > 63)
      {
        bitShift %= 63;
      }

      if (GetBitAsBool(bitValue, bitShift) != value)
      {
        // toggle that value using XOR
        int tV = 0x00000001 << bitShift;
        bitValue ^= tV;
      }

      return bitValue;
    }

    /// <summary>
    /// Converts a Flag Enum to the associated index value.
    /// </summary>
    /// <param name="theEnum">
    /// </param>
    /// <returns>
    /// The enum to index.
    /// </returns>
    public int EnumToIndex([NotNull] Enum theEnum)
    {
      CodeContracts.VerifyNotNull(theEnum, "theEnum");

      return Convert.ToInt32(Math.Sqrt(Convert.ToInt32(theEnum))) - 1;
    }

    #endregion
  }
}