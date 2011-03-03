using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YAF.Utils
{
  using YAF.Types;

  public static class BitBoolExtensions
  {
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

      return ((bitValue >> bitShift) & 0x00000001) == 1;
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
        int tV = (int)0x00000001 << bitShift;
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
    public static int ToIntOfBits(this IEnumerable<bool> arrayBool)
    {
      int finalValue = 0;
      arrayBool.ForEachIndex((b, i) => finalValue = SetBitFromBool(finalValue, i, b));
      
      return finalValue;
    }

    /// <summary>
    /// The to hex string.
    /// </summary>
    /// <param name="hashedBytes">
    /// The hashed bytes.
    /// </param>
    /// <returns>
    /// The to hex string.
    /// </returns>
    [NotNull]
    public static string ToHexString([NotNull] this byte[] hashedBytes)
    {
      CodeContracts.ArgumentNotNull(hashedBytes, "hashedBytes");

      var hashedSB = new StringBuilder((hashedBytes.Length * 2) + 2);

      foreach (byte b in hashedBytes)
      {
        hashedSB.AppendFormat("{0:X2}", b);
      }

      return hashedSB.ToString();
    }
  }
}
