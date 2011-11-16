/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

namespace YAF.Classes.Data
{
	using YAF.Types;

	/// <summary>
	/// The db connection ui param.
	/// </summary>
	public class DbConnectionUIParam
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DbConnectionUIParam"/> class.
		/// </summary>
		/// <param name="id">
		/// The id.
		/// </param>
		/// <param name="name">
		/// The name.
		/// </param>
		/// <param name="value">
		/// The value.
		/// </param>
		/// <param name="visible">
		/// The visible.
		/// </param>
		public DbConnectionUIParam(int id, [NotNull] string name = null, [NotNull] string value = null, bool visible = false)
		{
			this.ID = id;
			this.Label = name ?? string.Empty;
			this.DefaultValue = value ?? string.Empty;
			this.Visible = visible;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets DefaultValue.
		/// </summary>
		public string DefaultValue { get; protected set; }

		/// <summary>
		/// Gets or sets ID.
		/// </summary>
		public int ID { get; protected set; }

		/// <summary>
		/// Gets or sets Label.
		/// </summary>
		public string Label { get; protected set; }

		/// <summary>
		/// Gets or sets a value indicating whether Visible.
		/// </summary>
		public bool Visible { get; protected set; }

		#endregion
	}
}