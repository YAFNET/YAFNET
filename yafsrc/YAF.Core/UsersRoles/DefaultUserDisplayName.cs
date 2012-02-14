/* Yet Another Forum.NET
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
namespace YAF.Core
{
	#region Using

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using YAF.Classes.Pattern;
	using YAF.Core.Data;
	using YAF.Types;
	using YAF.Types.Constants;
	using YAF.Types.Interfaces;
	using YAF.Types.Objects;
	using YAF.Utils;
	using YAF.Utils.Extensions;

	#endregion

	/// <summary>
	/// The default user display name.
	/// </summary>
	public class DefaultUserDisplayName : IUserDisplayName, IHaveServiceLocator
	{
		private readonly IDbFunction _dbFunction;

		public IServiceLocator ServiceLocator { get; set; }

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultUserDisplayName"/> class.
		/// </summary>
		public DefaultUserDisplayName(IServiceLocator serviceLocator, IDbFunction dbFunction)
		{
			_dbFunction = dbFunction;
			ServiceLocator = serviceLocator;
		}

		#endregion

		#region Properties

		/// <summary>
		///   Gets UserDisplayNameCollection.
		/// </summary>
		private IThreadSafeDictionary<int, string> UserDisplayNameCollection
		{
			get
			{
				return
					this.Get<IObjectStore>().GetOrSet(
						Constants.Cache.UsersDisplayNameCollection, () => new ThreadSafeDictionary<int, string>());
			}
		}

		#endregion

		#region Implemented Interfaces

		#region IUserDisplayName

		/// <summary>
		/// Remove the item from collection
		/// </summary>
		/// <param name="userId">
		/// </param>
		public void Clear(int userId)
		{
			// update collection...
			if (this.UserDisplayNameCollection.ContainsKey(userId))
			{
				this.UserDisplayNameCollection.Remove(userId);
			}
		}

		/// <summary>
		/// Remove all the items from the collection
		/// </summary>
		public void Clear()
		{
			// update collection...
			this.UserDisplayNameCollection.Clear();
		}

		/// <summary>
		/// The find.
		/// </summary>
		/// <param name="contains">
		/// The contains.
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		public IDictionary<int, string> Find([NotNull] string contains)
		{
			IEnumerable<TypedUserFind> found;

			if (YafContext.Current.BoardSettings.EnableDisplayName)
			{
				found =
					((DataTable)
					 this._dbFunction.GetData.user_find(YafContext.Current.PageBoardID, true, null, null, contains, null, null)).
						Typed<TypedUserFind>().ToList();
			}
			else
			{
				found =
					((DataTable)
					 this._dbFunction.GetData.user_find(YafContext.Current.PageBoardID, true, contains, null, null, null, null)).
						Typed<TypedUserFind>().ToList();
			}

			return found.ToDictionary(k => k.UserID ?? 0, v => v.DisplayName);
		}

		/// <summary>
		/// The get id.
		/// </summary>
		/// <param name="name">
		/// The name.
		/// </param>
		/// <returns>
		/// </returns>
		public int? GetId([NotNull] string name)
		{
			int? userId = null;

			if (name.IsNotSet())
			{
				return userId;
			}

			var keyValue =
				this.UserDisplayNameCollection.ToList().Where(
					x => x.Value.IsSet() && x.Value.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

			if (keyValue.IsNotNull())
			{
				userId = keyValue.Key;
			}
			else
			{
				// find the username...
				if (YafContext.Current.BoardSettings.EnableDisplayName)
				{
					var user =
						((DataTable)
						 this._dbFunction.GetData.user_find(YafContext.Current.PageBoardID, false, null, null, name, null, null)).
							Typed<TypedUserFind>().FirstOrDefault();

					if (user != null)
					{
						userId = user.UserID ?? 0;
						this.UserDisplayNameCollection.MergeSafe(userId.Value, user.DisplayName);
					}
				}
				else
				{
					var user =
						((DataTable)
						 this._dbFunction.GetData.user_find(YafContext.Current.PageBoardID, false, name, null, null, null, null)).
							Typed<TypedUserFind>().FirstOrDefault();

					if (user != null)
					{
						userId = user.UserID ?? 0;
						this.UserDisplayNameCollection.MergeSafe(userId.Value, user.DisplayName);
					}
				}
			}

			return userId;
		}

		/// <summary>
		/// The get.
		/// </summary>
		/// <param name="userId">
		/// The user id.
		/// </param>
		/// <returns>
		/// The get.
		/// </returns>
		public string GetName(int userId)
		{
			string displayName;

			if (!this.UserDisplayNameCollection.TryGetValue(userId, out displayName))
			{
				var row = UserMembershipHelper.GetUserRowForID(userId, true);

				if (row != null)
				{
					if (YafContext.Current.BoardSettings.EnableDisplayName)
					{
						displayName = row.Field<string>("DisplayName");
					}

					if (displayName.IsNotSet())
					{
						// revert to their user name...
						displayName = row.Field<string>("Name");
					}

					this.UserDisplayNameCollection.MergeSafe(userId, displayName);
				}
			}

			return displayName;
		}

		#endregion

		#endregion
	}
}