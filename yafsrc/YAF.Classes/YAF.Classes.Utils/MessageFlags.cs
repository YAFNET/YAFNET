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
		private int _bitValue;

		public MessageFlags()
			: this( 23 )
		{

		}

		public MessageFlags( int bitValue )
		{
			_bitValue = bitValue;
		}

		static public bool GetBitAsBool( int bitValue, int bitShift )
		{
			if ( bitShift > 31 ) bitShift %= 31;
			if ( ( ( bitValue >> bitShift ) & 0x00000001 ) == 1 ) return true;
			return false;
		}

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

		public static implicit operator MessageFlags( int newBitValue )
		{
			MessageFlags mf = new MessageFlags( newBitValue );
			return mf;
		}

		public int BitValue
		{
			get { return _bitValue; }
			set { _bitValue = value; }
		}

		public bool this [int index]
		{
			get { return GetBitAsBool( _bitValue, index ); }
			set { _bitValue = SetBitFromBool( _bitValue, index, value ); }
		}

		// actual flags here -- can be a total of 31
		public bool IsHTML
		{
			get { return GetBitAsBool( _bitValue, 0 ); }
			set { _bitValue = SetBitFromBool( _bitValue, 0, value ); }
		}

		public bool IsBBCode
		{
			get { return GetBitAsBool( _bitValue, 1 ); }
			set { _bitValue = SetBitFromBool( _bitValue, 1, value ); }
		}

		public bool IsSmilies
		{
			get { return GetBitAsBool( _bitValue, 2 ); }
			set { _bitValue = SetBitFromBool( _bitValue, 2, value ); }
		}

		public bool IsDeleted
		{
			get { return GetBitAsBool( _bitValue, 3 ); }
			set { _bitValue = SetBitFromBool( _bitValue, 3, value ); }
		}

		public bool IsApproved
		{
			get { return GetBitAsBool( _bitValue, 4 ); }
			set { _bitValue = SetBitFromBool( _bitValue, 4, value ); }
		}

		/// <summary>
		/// This post is locked -- nothing can be done to it
		/// </summary>
		public bool IsLocked
		{
			get { return GetBitAsBool( _bitValue, 5 ); }
			set { _bitValue = SetBitFromBool( _bitValue, 5, value ); }
		}

		/// <summary>
		/// Setting so that the message isn't formatted at all
		/// </summary>
		public bool NotFormatted
		{
			get { return GetBitAsBool( _bitValue, 6 ); }
			set { _bitValue = SetBitFromBool( _bitValue, 6, value ); }
		}

        /// <summary>
        /// This post has been reported as abusive
        /// </summary>
        public bool IsReported
        {
            get { return GetBitAsBool(_bitValue, 7); }
            set { _bitValue = SetBitFromBool(_bitValue, 7, value); }
        }

        /// <summary>
        /// This post has been reported as spam
        /// </summary>
        public bool IsReportedSpam
        {
            get { return GetBitAsBool(_bitValue, 8); }
            set { _bitValue = SetBitFromBool(_bitValue, 8, value); }
        }

	}
}
