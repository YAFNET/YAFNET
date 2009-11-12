/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Runtime.Serialization;

namespace YAF.Classes.Data
{
  /// <summary>
  /// Abstract class as a foundation for various flags implementations
  /// </summary>
  [Serializable()]
  public abstract class FlagsBase : ISerializationSurrogate
  {
    // integer value stores up to 32 flags/bits
    /// <summary>
    /// The _bit value.
    /// </summary>
    protected int _bitValue;

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="FlagsBase"/> class. 
    /// Creates new instance with all bits set to false (integer 0).
    /// </summary>
    public FlagsBase()
      : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlagsBase"/> class. 
    /// Creates new instance and initialize it with value of bitValue parameter.
    /// </summary>
    /// <param name="bitValue">
    /// Inicialization integer value.
    /// </param>
    public FlagsBase(int bitValue)
    {
      this._bitValue = bitValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlagsBase"/> class. 
    /// Creates new instance with bits set according to param array.
    /// </summary>
    /// <param name="bits">
    /// Boolean values to initialize class with. If their number is lower than 32, remaining bits are set to false. If more than 32 values is specified, excess values are ignored.
    /// </param>
    public FlagsBase(params bool[] bits)
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

    /// <summary>
    /// Gets or sets bit at position specified by index.
    /// </summary>
    /// <param name="index">Zero-based index of bit to get or set.</param>
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


    /// <summary>
    /// Gets or sets integer value of flags.
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

    #region ISerializationSurrogate Members

    /// <summary>
    /// The get object data.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="info">
    /// The info.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
      var flags = obj as FlagsBase;
      info.AddValue("BitValue", flags.BitValue);
    }

    /// <summary>
    /// The set object data.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="info">
    /// The info.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <returns>
    /// The set object data.
    /// </returns>
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
      var flags = obj as FlagsBase;
      flags.BitValue = info.GetInt32("BitValue");

      return null;
    }

    #endregion

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
      if (bitShift > 31)
      {
        bitShift %= 31;
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
      if (bitShift > 31)
      {
        bitShift %= 31;
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
    /// Creates an integer value from an array of booleans.
    /// </summary>
    /// <param name="arrayBool">
    /// array of boolean
    /// </param>
    /// <returns>
    /// bit field of the array
    /// </returns>
    public static int GetIntFromBoolArray(bool[] arrayBool)
    {
      int finalValue = 0;

      for (int i = 0; i < arrayBool.Length; i++)
      {
        finalValue = SetBitFromBool(finalValue, i, arrayBool[i]);
      }

      return finalValue;
    }

    /*
		#region Operators

		public static implicit operator FlagsBase(int newBitValue)
		{
			FlagsBase flags = new FlagsBase(newBitValue);
			return flags;
		}

		#endregion
		*/

    /// <summary>
    /// Converts a Flag Enum to the associated index value.
    /// </summary>
    /// <param name="theEnum">
    /// </param>
    /// <returns>
    /// The enum to index.
    /// </returns>
    public int EnumToIndex(Enum theEnum)
    {
      return Convert.ToInt32(Math.Sqrt(Convert.ToInt32(theEnum))) - 1;
    }
  }
}