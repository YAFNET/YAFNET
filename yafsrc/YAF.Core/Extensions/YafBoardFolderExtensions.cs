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

namespace YAF.Core.Extensions
{
	using YAF.Classes;
	using YAF.Types;
	using YAF.Types.Constants;

	/// <summary>
	/// The yaf board folder extensions.
	/// </summary>
	public static class YafBoardFolderExtensions
	{
		#region Public Methods

		/// <summary>
		/// The from folder.
		/// </summary>
		/// <param name="yafBoardFolders">
		/// The yaf board folders.
		/// </param>
		/// <param name="location">
		/// The location.
		/// </param>
		/// <returns>
		/// The from folder.
		/// </returns>
		public static string FromFolder([NotNull] this YafBoardFolders yafBoardFolders, YafFolder location)
		{
			CodeContracts.ArgumentNotNull(yafBoardFolders, "yafBoardFolders");

			switch (location)
			{
				case YafFolder.Uploads:
					return yafBoardFolders.Uploads;
				case YafFolder.Avatars:
					return yafBoardFolders.Avatars;
				case YafFolder.Categories:
					return yafBoardFolders.Categories;
				case YafFolder.Emoticons:
					return yafBoardFolders.Emoticons;
				case YafFolder.Forums:
					return yafBoardFolders.Forums;
				case YafFolder.Images:
					return yafBoardFolders.Images;
				case YafFolder.Ranks:
					return yafBoardFolders.Ranks;
				case YafFolder.Themes:
					return yafBoardFolders.Themes;
			}

			return yafBoardFolders.BoardFolder;
		}

		#endregion
	}
}