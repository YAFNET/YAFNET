/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2007 Jaben Cargman
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
using System.Collections.Generic;
using System.Text;

namespace YAF.Classes.Utils
{
	public class MessageFlags
	{
		int FBitValue;

		public MessageFlags()
			: this( 23 )
		{

		}

		public MessageFlags( int bitValue )
		{
			FBitValue = bitValue;
		}

		static public bool GetBitAsBool( int bitValue, int bitShift )
		{
			if ( bitShift > 31 ) bitShift %= 31;
			if ( ( ( bitValue >> bitShift ) & 0x00000001 ) == 1 ) return true;
			return false;
		}

		static public int SetBitFromBool( int bitValue, int bitShift, bool bValue )
		{
			if ( bitShift > 31 ) bitShift %= 31;

			if ( GetBitAsBool( bitValue, bitShift ) != bValue )
			{
				// toggle that value using XOR
				int tV = 0x00000001 << bitShift;
				bitValue ^= tV;
			}
			return bitValue;
		}

		public static implicit operator MessageFlags( int newBitValue )
		{
			MessageFlags mf = new MessageFlags( newBitValue );
			return mf;
		}

		public int BitValue
		{
			get { return FBitValue; }
			set { FBitValue = value; }
		}

		public bool this [int index]
		{
			get { return GetBitAsBool( FBitValue, index ); }
			set { FBitValue = SetBitFromBool( FBitValue, index, value ); }
		}

		// actual flags here -- can be a total of 31
		public bool IsHTML
		{
			get { return GetBitAsBool( FBitValue, 0 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 0, value ); }
		}

		public bool IsBBCode
		{
			get { return GetBitAsBool( FBitValue, 1 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 1, value ); }
		}

		public bool IsSmilies
		{
			get { return GetBitAsBool( FBitValue, 2 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 2, value ); }
		}

		public bool IsDeleted
		{
			get { return GetBitAsBool( FBitValue, 3 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 3, value ); }
		}

		public bool IsApproved
		{
			get { return GetBitAsBool( FBitValue, 4 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 4, value ); }
		}

		/// <summary>
		/// This post is locked -- nothing can be done to it
		/// </summary>
		public bool IsLocked
		{
			get { return GetBitAsBool( FBitValue, 5 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 5, value ); }
		}

		/// <summary>
		/// Setting so that the message isn't formatted at all
		/// </summary>
		public bool NotFormatted
		{
			get { return GetBitAsBool( FBitValue, 6 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 6, value ); }
		}
	}
}
