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

namespace YAF.Classes.Data
{
	public class RankFlags : FlagsBase
	{
		#region Constructors

		public RankFlags() : this(0) { }
		public RankFlags(RankFlags.Flags flags) : this((int)flags) { }
		public RankFlags(object bitValue) : this((int)bitValue) { }
		public RankFlags(int bitValue) : base(bitValue) { }
		public RankFlags(params bool[] bits) : base(bits) { }

		#endregion


		#region Operators

		public static implicit operator RankFlags(int newBitValue)
		{
			return new RankFlags(newBitValue);
		}

		public static implicit operator RankFlags(RankFlags.Flags flags)
		{
			return new RankFlags(flags);
		}

		#endregion


		#region Flags Enumeration

		/// <summary>
		/// Use for bit comparisons
		/// </summary>
		public enum Flags : int
		{
			IsStart	=	1,
			IsLadder	=	2
			/* for future use
			xxxxx = 4,
			xxxxx = 8,
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
		/// Gets or sets whether rank is default starting rank of new users.
		/// </summary>
		public bool IsStart // int value 1
		{
			get { return this[0]; }
			set { this[0] = value; }
		}


		/// <summary>
		/// Gets or sets whether rank is ladder rank.
		/// </summary>
		public bool IsLadder // int value 2
		{
			get { return this[1]; }
			set { this[1] = value; }
		}

		#endregion
	}
}
