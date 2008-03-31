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
	public class ForumFlags : FlagsBase
	{
		#region Constructors

		public ForumFlags() : this(0) { }
		public ForumFlags(ForumFlags.Flags flags) : this((int)flags) { }
		public ForumFlags(object bitValue) : this((int)bitValue) { }
		public ForumFlags(int bitValue) : base(bitValue) { }
		public ForumFlags(params bool[] bits) : base(bits) { }

		#endregion


		#region Operators

		public static implicit operator ForumFlags(int newBitValue)
		{
			return new ForumFlags(newBitValue);
		}

		public static implicit operator ForumFlags(ForumFlags.Flags flags)
		{
			return new ForumFlags(flags);
		}

		#endregion


		#region Flags Enumeration

		/// <summary>
		/// Use for bit comparisons
		/// </summary>
		public enum Flags : int
		{
			IsLocked		=	1,
			IsHidden		=	2,
			IsTest			=	4,
			IsModerated		=	8
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
		/// Gets or sets whether forum allows locked. No posting/activity can be made in locked forums.
		/// </summary>
		public bool IsLocked // int value 1
		{
			get { return this[0]; }
			set { this[0] = value; }
		}


		/// <summary>
		/// Gets or sets whether forum is hidden to users without read access.
		/// </summary>
		public bool IsHidden // int value 2
		{
			get { return this[1]; }
			set { this[1] = value; }
		}


		/// <summary>
		/// Gets or sets whether forum does not count to users' postcount.
		/// </summary>
		public bool IsTest // int value 4
		{
			get { return this[2]; }
			set { this[2] = value; }
		}


		/// <summary>
		/// Gets or sets whether forum is moderated. Posts in moderated posts has to be approved by moderator before they are published.
		/// </summary>
		public bool IsModerated // int value 8
		{
			get { return this[3]; }
			set { this[3] = value; }
		}

		#endregion
	}
}
