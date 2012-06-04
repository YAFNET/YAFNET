/* Yet Another Forum.net
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

namespace YAF.Core.Services
{
	using System.Web;
	using System.IO;

	using YAF.Classes;
	using YAF.Core.Extensions;
	using YAF.Types;
	using YAF.Types.Attributes;
	using YAF.Types.Constants;
	using YAF.Types.Interfaces;

	/// <summary>
	/// The local hosted file system.
	/// </summary>
	public class LocalHostedFileSystem : IFileSystem
	{
		#region Constants and Fields

		/// <summary>
		/// The _http server utility base.
		/// </summary>
		private readonly HttpServerUtilityBase _httpServerUtilityBase;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalHostedFileSystem"/> class.
		/// </summary>
		/// <param name="httpServerUtilityBase">
		/// The http server utility base.
		/// </param>
		public LocalHostedFileSystem([NotNull] HttpServerUtilityBase httpServerUtilityBase)
		{
			this._httpServerUtilityBase = httpServerUtilityBase;
		}

		#endregion

		#region Implemented Interfaces

		#region IFileSystem

		/// <summary>
		/// The delete.
		/// </summary>
		/// <param name="folder">
		/// The folder.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		public void Delete(YafFolder folder, string fileName)
		{
			var fullFileName = Path.Combine(this.GetMappedFolder(folder), fileName);

			File.Delete(fullFileName);
		}

		/// <summary>
		/// The exists.
		/// </summary>
		/// <param name="folder">
		/// The folder.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		/// <returns>
		/// The exists.
		/// </returns>
		public bool Exists(YafFolder folder, string fileName)
		{
			var fullFileName = Path.Combine(this.GetMappedFolder(folder), fileName);

			return File.Exists(fullFileName);
		}

		/// <summary>
		/// The load.
		/// </summary>
		/// <param name="folder">
		/// The folder.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		public byte[] Load(YafFolder folder, string fileName)
		{
			var fullFileName = Path.Combine(this.GetMappedFolder(folder), fileName);

			return File.ReadAllBytes(fullFileName);
		}

		/// <summary>
		/// The save.
		/// </summary>
		/// <param name="folder">
		/// The folder.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		/// <param name="data">
		/// The data.
		/// </param>
		public void Save(YafFolder folder, string fileName, byte[] data)
		{
			var fullFileName = Path.Combine(this.GetMappedFolder(folder), fileName);

			File.WriteAllBytes(fullFileName, data);
		}

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// The get mapped folder.
		/// </summary>
		/// <param name="folder">
		/// The folder.
		/// </param>
		/// <returns>
		/// The get mapped folder.
		/// </returns>
		private string GetMappedFolder(YafFolder folder)
		{
			var folderMapped =
				this._httpServerUtilityBase.MapPath(
					string.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.FromFolder(folder)));

			if (!Directory.Exists(folderMapped))
			{
				// create it...
				Directory.CreateDirectory(folderMapped);
			}

			return folderMapped;
		}

		#endregion
	}
}