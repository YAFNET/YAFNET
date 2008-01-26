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
	public class MessageFlags : TopicFlags
	{
		#region Constructors

		public MessageFlags() : this(Flags.IsHtml | Flags.IsBBCode | Flags.IsSmilies) { }
		public MessageFlags(MessageFlags.Flags flags) : this((int)flags) { }
		public MessageFlags(object bitValue) : this((int)bitValue) { }
		public MessageFlags(int bitValue) : base(bitValue) { }
		public MessageFlags(params bool[] bits) : base(bits) { }

		#endregion


		#region Operators

		public static implicit operator MessageFlags( int newBitValue )
		{
			return new MessageFlags(newBitValue);
		}

		public static implicit operator MessageFlags(MessageFlags.Flags flags)
		{
			return new MessageFlags(flags);
		}

		#endregion


		#region Flags Enumeration

		/// <summary>
		/// Use for bit comparisons
		/// </summary>
		public new enum Flags : int
		{
			IsHtml = 1,
			IsBBCode = 2,
			IsSmilies = 4,
			IsDeleted = 8,
			IsApproved = 16,
			IsLocked = 32,
			NotFormatted = 64,
			IsReportedAbuse = 128,
			IsReportedSpam = 256,
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
		/// Gets or sets whether message allows HTML.
		/// </summary>
		public bool IsHtml // int value 1
		{
			get { return this[0]; }
			set { this[0] = value; }
		}


		/// <summary>
		/// Gets or sets whether message allows BB code.
		/// </summary>
		public bool IsBBCode // int value 2
		{
			get { return this[1]; }
			set { this[1] = value; }
		}


		/// <summary>
		/// Gets or sets whether message allows smilies.
		/// </summary>
		public bool IsSmilies // int value 4
		{
			get { return this[2]; }
			set { this[2] = value; }
		}


		/// <summary>
		/// Gets or sets whether message is deleted.
		/// </summary>
		// inheritted
		//public bool IsDeleted { get; set; }// int value 8


		/// <summary>
		/// Gets or sets whether message is approved for publishing.
		/// </summary>
		public bool IsApproved // int value 16
		{
			get { return this[4]; }
			set { this[4] = value; }
		}

		/// <summary>
		/// Gets or sets whether message is locked. Locked messages cannot be modified/deleted/replied to.
		/// </summary>
		public override bool IsLocked // int value 32
		{
			get { return this[5]; }
			set { this[5] = value; }
		}

		/// <summary>
		/// Gets or sets whether message is not formatted.
		/// </summary>
		public bool NotFormatted // int value 64
		{
			get { return this[6]; }
			set { this[6] = value; }
		}

        /// <summary>
		/// Gets or sets whether message is reported as abusive.
        /// </summary>
		public bool IsReportedAbuse // int value 128
        {
			get { return this[7]; }
			set { this[7] = value; }
        }

        /// <summary>
		///Gets or sets whether message is reported as spam.
        /// </summary>
		public bool IsReportedSpam // int value 256
        {
			get { return this[8]; }
			set { this[8] = value; }
        }

		/// <summary>
		/// Gets or sets whether message is persistent. Persistent messages cannot be purged.
		/// </summary>
		// inheritted
		//public bool IsPersistent { get; set; } // int value 512

		#endregion
	}
}
