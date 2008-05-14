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
using System.Collections.Generic;
using System.Text;

namespace YAF.Classes.Data
{
	[Serializable()]
	public class UserFlags : FlagsBase
	{
		#region Constructors

		public UserFlags() : this(0) { }
		public UserFlags(UserFlags.Flags flags) : this((int)flags) { }
		public UserFlags(object bitValue) : this((int)bitValue) { }
		public UserFlags(int bitValue) : base(bitValue) { }
		public UserFlags(params bool[] bits) : base(bits) { }

		#endregion


		#region Operators

		public static implicit operator UserFlags(int newBitValue)
		{
			return new UserFlags(newBitValue);
		}

		public static implicit operator UserFlags(UserFlags.Flags flags)
		{
			return new UserFlags(flags);
		}

		#endregion


		#region Flags Enumeration

		/// <summary>
		/// Use for bit comparisons
		/// </summary>
		public enum Flags : int
		{
			IsHostAdmin = 1,
			IsApproved = 2,
			IsGuest = 4,
			IsCaptchaExcluded = 8,
			IsActiveExcluded = 16,
			/* for future use
			xxxxx = 32,
			xxxxx = 64,
			xxxxx = 128,
			xxxxx = 256,
			xxxxx = 512
			 */
		}

		#endregion


		#region Single Flags (can be 32 of them)

		/// <summary>
		/// Gets or sets whether user is host administrator.
		/// </summary>
		public bool IsHostAdmin // int value 1
		{
			get { return this[0]; }
			set { this[0] = value; }
		}


		/// <summary>
		/// Gets or sets whether user is approved for posting.
		/// </summary>
		public bool IsApproved // int value 2
		{
			get { return this[1]; }
			set { this[1] = value; }
		}


		/// <summary>
		/// Gets or sets whether user is guest, i.e. not registered and logged in.
		/// </summary>
		public bool IsGuest // int value 4
		{
			get { return this[2]; }
			set { this[2] = value; }
		}


		/// <summary>
		/// Gets or sets whether user is guest, i.e. not registered and logged in.
		/// </summary>
		public bool IsCaptchaExcluded // int value 8
		{
			get { return this[3]; }
			set { this[3] = value; }
		}

		/// <summary>
		/// Gets or sets whether user is excluded from the "Active Users" list on the forum pages.
		/// </summary>
		public bool IsActiveExcluded // int value 16
		{
			get { return this [4]; }
			set { this [4] = value; }
		}

		#endregion
	}
}
