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
	public class TopicFlags : FlagsBase
	{
		#region Constructors

		public TopicFlags() : this(0) { }
		public TopicFlags(TopicFlags.Flags flags) : this((int)flags) { }
		public TopicFlags(object bitValue) : this((int)bitValue) { }
		public TopicFlags(int bitValue) : base(bitValue) { }
		public TopicFlags(params bool[] bits) : base(bits) { }

		#endregion


		#region Operators

		public static implicit operator TopicFlags( int newBitValue )
		{
			return new TopicFlags(newBitValue);
		}

		public static implicit operator TopicFlags(TopicFlags.Flags flags)
		{
			return new TopicFlags(flags);
		}

		#endregion


		#region Flags Enumeration

		/// <summary>
		/// Use for bit comparisons
		/// </summary>
		public enum Flags : int
		{
			IsDeleted = 8,
			IsLocked = 32,
			IsPersistent = 512
			/* for future use
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
		/// Gets or sets whether topic is locked. Locked topics cannot be modified/deleted/replied to.
		/// </summary>
		public virtual bool IsLocked // int value 1
		{
			get { return this[0]; }
			set { this[0] = value; }
		}

		/// <summary>
		/// Gets or sets whether topic is deleted.
		/// </summary>
		public virtual bool IsDeleted // int value 8
		{
			get { return this[3]; }
			set { this[3] = value; }
		}

		/// <summary>
		/// Gets or sets whether topic is persistent. Persistent topics cannot be purged.
		/// </summary>
		public virtual bool IsPersistent // int value 512
		{
			get { return this[9]; }
			set { this[9] = value; }
		}

		#endregion
	}
}
