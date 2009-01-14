/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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

namespace YAF.Classes.Data
{
	/// <summary>
	/// Abstract class as a foundation for various flags implementations
	/// </summary>
	[Serializable()]
	public abstract class FlagsBase : System.Runtime.Serialization.ISerializationSurrogate
	{
		#region Data Members

		// integer value stores up to 32 flags/bits
		protected int _bitValue;

		#endregion


		#region Constructors

		/// <summary>
		/// Creates new instance with all bits set to false (integer 0).
		/// </summary>
		public FlagsBase() : this(0) { }
		/// <summary>
		/// Creates new instance and initialize it with value of bitValue parameter.
		/// </summary>
		/// <param name="bitValue">Inicialization integer value.</param>
		public FlagsBase(int bitValue)
		{
			_bitValue = bitValue;
		}
		/// <summary>
		/// Creates new instance with bits set according to param array.
		/// </summary>
		/// <param name="bits">Boolean values to initialize class with. If their number is lower than 32, remaining bits are set to false. If more than 32 values is specified, excess values are ignored.</param>
		public FlagsBase(params bool[] bits) : this(0)
		{
			// process up to 32 parameters
			for (int i = 0; i < Math.Min(bits.Length, 31); i++)
			{
				// set this bit
				this[i] = bits[i];
			}
		}

		#endregion


		#region Static Members

		/// <summary>
		/// Gets boolean indicating whether bit on bitShift position in bitValue integer is set or not.
		/// </summary>
		/// <param name="bitValue">Integer value.</param>
		/// <param name="bitShift">Zero-based position of bit to get.</param>
		/// <returns>Returns boolean indicating whether bit at bitShift position is set or not.</returns>
		static public bool GetBitAsBool(int bitValue, int bitShift)
		{
			if ( bitShift > 31 ) bitShift %= 31;
			if ( ( ( bitValue >> bitShift ) & 0x00000001 ) == 1 ) return true;
			return false;
		}

		/// <summary>
		/// Sets or unsets bit of bitValue integer at position specified by bitShift, depending on value parameter.
		/// </summary>
		/// <param name="bitValue">Integer value.</param>
		/// <param name="bitShift">Zero-based position of bit to set.</param>
		/// <param name="value">New boolean value of bit.</param>
		/// <returns>Returns new integer value with bit at position specified by bitShift parameter set to value.</returns>
		static public int SetBitFromBool( int bitValue, int bitShift, bool value )
		{
			if ( bitShift > 31 ) bitShift %= 31;

			if ( GetBitAsBool( bitValue, bitShift ) != value )
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
		/// <param name="arrayBool">array of boolean</param>
		/// <returns>bit field of the array</returns>
		static public int GetIntFromBoolArray( bool [] arrayBool )
		{
			int finalValue = 0;

			for ( int i = 0; i < arrayBool.Length; i++ )
			{
				finalValue = SetBitFromBool( finalValue, i, arrayBool [i] );
			}

			return finalValue;
		}

		#endregion

		/*
		#region Operators

		public static implicit operator FlagsBase(int newBitValue)
		{
			FlagsBase flags = new FlagsBase(newBitValue);
			return flags;
		}

		#endregion
		*/

		#region Public Members & Indexers

		/// <summary>
		/// Gets or sets bit at position specified by index.
		/// </summary>
		/// <param name="index">Zero-based index of bit to get or set.</param>
		/// <returns>Boolean value indicating whether bit at position specified by index is set or not.</returns>
		public bool this [int index]
		{
			get { return GetBitAsBool( _bitValue, index ); }
			set { _bitValue = SetBitFromBool( _bitValue, index, value ); }
		}


		/// <summary>
		/// Gets or sets integer value of flags.
		/// </summary>
		public int BitValue
		{
			get { return _bitValue; }
			set { _bitValue = value; }
		}

		#endregion

		#region ISerializationSurrogate Members

		public void GetObjectData( object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
		{
			FlagsBase flags = obj as FlagsBase;
			info.AddValue( "BitValue", flags.BitValue );
		}

		public object SetObjectData( object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector )
		{
			FlagsBase flags = obj as FlagsBase;
			flags.BitValue = info.GetInt32( "BitValue" );

			return null;
		}

		#endregion
	}
}
