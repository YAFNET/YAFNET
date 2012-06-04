// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileSystem.cs" company="">
//   
// </copyright>
// <summary>
//   The i file system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Types.Interfaces
{
	using YAF.Types.Constants;

	/// <summary>
	/// The i file system.
	/// </summary>
	public interface IFileSystem
	{
		#region Public Methods

		/// <summary>
		/// The delete.
		/// </summary>
		/// <param name="folder">
		/// The folder.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		void Delete(YafFolder folder, [NotNull] string fileName);

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
		bool Exists(YafFolder folder, [NotNull] string fileName);

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
		byte[] Load(YafFolder folder, [NotNull] string fileName);

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
		void Save(YafFolder folder, [NotNull] string fileName, [NotNull] byte[] data);

		#endregion
	}
}