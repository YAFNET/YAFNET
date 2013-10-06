/* Yet Another Forum.net
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

namespace YAF.Types.Extensions
{
	#region Using

    using System.Collections.Generic;
    using System.Text;

    using YAF.Types;

    #endregion

	/// <summary>
	/// The bit bool extensions.
	/// </summary>
	public static class BitBoolExtensions
	{
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
				int tV = 0x00000001 << bitShift;
				bitValue ^= tV;
			}

			return bitValue;
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
			CodeContracts.VerifyNotNull(hashedBytes, "hashedBytes");

			var hashedSB = new StringBuilder((hashedBytes.Length * 2) + 2);

			foreach (byte b in hashedBytes)
			{
				hashedSB.AppendFormat("{0:X2}", b);
			}

			return hashedSB.ToString();
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

		#endregion
	}
}