// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataLoadable.cs" company="">
//   
// </copyright>
// <summary>
//   The i data loadable.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Types.Interfaces
{
	using System.Collections.Generic;

	/// <summary>
	/// The i data loadable.
	/// </summary>
	public interface IDataLoadable
	{
		#region Public Methods

		/// <summary>
		/// The load from dictionary.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary.
		/// </param>
		/// <param name="clear">
		/// The clear.
		/// </param>
		void LoadFromDictionary([NotNull] IDictionary<string, object> dictionary, bool clear = true);

		#endregion
	}
}