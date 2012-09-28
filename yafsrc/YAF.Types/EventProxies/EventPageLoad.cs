/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Types.EventProxies
{
	using System.Collections.Generic;
	using System.Dynamic;

	using YAF.Types.Interfaces;

    /// <summary>
	/// The page load event.
	/// </summary>
	public class InitPageLoadEvent : IAmEvent
	{
		#region Constants and Fields

		/// <summary>
		/// The _the expando data.
		/// </summary>
		private readonly ExpandoObject _theExpandoData = new ExpandoObject();

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="InitPageLoadEvent"/> class.
		/// </summary>
		public InitPageLoadEvent()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets Data.
		/// </summary>
		public dynamic Data
		{
			get
			{
				return this._theExpandoData;
			}
		}

		/// <summary>
		///   Gets or sets PageLoadData.
		/// </summary>
		public IDictionary<string, object> DataDictionary
		{
			get
			{
				return (IDictionary<string, object>)this._theExpandoData;
			}
		}

		#endregion
	}
}