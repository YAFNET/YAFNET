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
	public class MedalFlags : FlagsBase
	{
		#region Constructors

		public MedalFlags() : this(0) { }
		public MedalFlags(MedalFlags.Flags flags) : this((int)flags) { }
		public MedalFlags(object bitValue) : this((int)bitValue) { }
		public MedalFlags(int bitValue) : base(bitValue) { }
		public MedalFlags(params bool[] bits) : base(bits) { }

		#endregion


		#region Operators

		public static implicit operator MedalFlags(int newBitValue)
		{
			return new MedalFlags(newBitValue);
		}

		public static implicit operator MedalFlags(MedalFlags.Flags flags)
		{
			return new MedalFlags(flags);
		}

		#endregion


		#region Flags Enumeration

		/// <summary>
		/// Use for bit comparisons
		/// </summary>
		public enum Flags : int
		{
			ShowMessage = 1,
			AllowRibbon = 2,
			AllowHiding = 4,
			AllowReOrdering = 8,
			/* for future use
			xxxxxxxx = 16,
			xxxxxxxx = 32,
			xxxxxxxx = 64,
			xxxxxxxx = 128,
			xxxxxxxx = 256,
			xxxxxxxx = 512,
			xxxxxxxx = 1024,
			xxxxxxxx = 2048,
			xxxxxxxx = 4096,
			xxxxxxxx = 8192,
			xxxxxxxx = 16384,
			xxxxxxxx = 32768,
			xxxxxxxx = 65536
			 */
		}

		#endregion


		#region Single Flags (can be 32 of them)

		/// <summary>
		/// Gets or sets whether medal message is shown.
		/// </summary>
		public virtual bool ShowMessage // int value 1
		{
			get { return this[0]; }
			set { this[0] = value; }
		}

		/// <summary>
		/// Gets or sets whether medal can be displayed as ribbon bar.
		/// </summary>
		public virtual bool AllowRibbon // int value 2
		{
			get { return this[1]; }
			set { this[1] = value; }
		}

		/// <summary>
		/// Gets or sets whether medal can be hidden by user.
		/// </summary>
		public virtual bool AllowHiding // int value 4
		{
			get { return this[2]; }
			set { this[2] = value; }
		}

		/// <summary>
		/// Gets or sets whether medal can be re-ordered by user.
		/// </summary>
		public virtual bool AllowReOrdering // int value 8
		{
			get { return this[3]; }
			set { this[3] = value; }
		}

		#endregion
	}
}
