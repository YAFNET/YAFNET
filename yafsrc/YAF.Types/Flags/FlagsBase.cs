/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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
      for (var i = 0; i < Math.Min(bits.Length, 31); i++)
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
        var tV = 0x00000001 << bitShift;
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