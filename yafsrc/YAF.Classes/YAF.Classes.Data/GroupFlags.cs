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
using System.Collections.Generic;
using System.Text;

namespace YAF.Classes.Data
{
	[Serializable()]
	public class GroupFlags : FlagsBase
	{
		#region Constructors

		public GroupFlags() : this(0) { }
		public GroupFlags(GroupFlags.Flags flags) : this((int)flags) { }
		public GroupFlags(object bitValue) : this((int)bitValue) { }
		public GroupFlags(int bitValue) : base(bitValue) { }
		public GroupFlags(params bool[] bits) : base(bits) { }

		#endregion


		#region Operators

		public static implicit operator GroupFlags(int newBitValue)
		{
			return new GroupFlags(newBitValue);
		}

		public static implicit operator GroupFlags(GroupFlags.Flags flags)
		{
			return new GroupFlags(flags);
		}

		#endregion


		#region Flags Enumeration

		/// <summary>
		/// Use for bit comparisons
		/// </summary>
		public enum Flags : int
		{
			IsAdmin = 1,
			IsGuest = 2,
			IsStart = 4,
			IsModerator = 8
			/* for future use
			xxxxx = 16,
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
		/// Gets or sets whether group/role has administrator privilegies
		/// </summary>
		public bool IsAdmin // int value 1
		{
			get { return this[0]; }
			set { this[0] = value; }
		}


		/// <summary>
		/// Gets or sets whether group/role is guest role.
		/// </summary>
		public bool IsGuest // int value 2
		{
			get { return this[1]; }
			set { this[1] = value; }
		}


		/// <summary>
		/// Gets or sets whether group/role is starting role for new users.
		/// </summary>
		public bool IsStart // int value 4
		{
			get { return this[2]; }
			set { this[2] = value; }
		}


		/// <summary>
		/// Gets or sets whether group/role has moderator privilegies.
		/// </summary>
		public bool IsModerator // int value 8
		{
			get { return this[3]; }
			set { this[3] = value; }
		}

		#endregion
	}
}
