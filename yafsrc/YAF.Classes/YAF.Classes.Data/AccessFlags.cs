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
	public class AccessFlags : FlagsBase
	{
		#region Constructors

		public AccessFlags() : this(0) { }
		public AccessFlags(AccessFlags.Flags flags) : this((int)flags) { }
		public AccessFlags(object bitValue) : this((int)bitValue) { }
		public AccessFlags(int bitValue) : base(bitValue) { }
		public AccessFlags(params bool[] bits) : base(bits) { }

		#endregion


		#region Operators

		public static implicit operator AccessFlags(int newBitValue)
		{
			AccessFlags flags = new AccessFlags(newBitValue);
			return flags;
		}

		public static implicit operator AccessFlags(AccessFlags.Flags flags)
		{
			return new AccessFlags(flags);
		}

		#endregion


		#region Flags Enumeration

		/// <summary>
		/// Use for bit comparisons
		/// </summary>
		public enum Flags : int
		{
			ReadAccess = 1,
			PostAccess = 2,
			ReplyAccess = 4,
			PriorityAccess = 8,
			PollAccess = 16,
			VoteAccess = 32,
			ModeratorAccess = 64,
			EditAccess = 128,
			DeleteAccess = 256,
			UploadAccess = 512,
			DownloadAccess = 1024
		}

		#endregion


		#region Single Flags (can be 32 of them)

		/// <summary>
		/// Gets or sets read access right.
		/// </summary>
		public bool ReadAccess // int value 1
		{
			get { return this[0]; }
			set { this[0] = value; }
		}


		/// <summary>
		/// Gets or sets post access right.
		/// </summary>
		public bool PostAccess // int value 2
		{
			get { return this[1]; }
			set { this[1] = value; }
		}


		/// <summary>
		/// Gets or sets reply access right.
		/// </summary>
		public bool ReplyAccess // int value 4
		{
			get { return this[2]; }
			set { this[2] = value; }
		}


		/// <summary>
		/// Gets or sets priority access right.
		/// </summary>
		public bool PriorityAccess // int value 8
		{
			get { return this[3]; }
			set { this[3] = value; }
		}


		/// <summary>
		/// Gets or sets poll access right.
		/// </summary>
		public bool PollAccess // int value 16
		{
			get { return this[4]; }
			set { this[4] = value; }
		}

		/// <summary>
		/// Gets or sets vote access right.
		/// </summary>
		public bool VoteAccess // int value 32
		{
			get { return this[5]; }
			set { this[5] = value; }
		}

		/// <summary>
		/// Gets or sets moderator access right.
		/// </summary>
		public bool ModeratorAccess // int value 64
		{
			get { return this[6]; }
			set { this[6] = value; }
		}

        /// <summary>
		/// Gets or sets edit access right.
        /// </summary>
		public bool EditAccess // int value 128
        {
			get { return this[7]; }
			set { this[7] = value; }
        }

        /// <summary>
		///Gets or sets delete access right.
        /// </summary>
		public bool DeleteAccess // int value 256
        {
			get { return this[8]; }
			set { this[8] = value; }
        }

		/// <summary>
		/// Gets or sets upload access right.
		/// </summary>
		public bool UploadAccess // int value 512
		{
			get { return this[9]; }
			set { this[9] = value; }
		}


		/// <summary>
		/// Gets or sets download access right.
		/// </summary>
		public bool DownloadAccess // int value 512
		{
			get { return this[10]; }
			set { this[10] = value; }
		}

		#endregion
	}
}
