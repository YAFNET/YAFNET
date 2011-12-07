// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The data set extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Utils.Extensions
{
	using System.Data;

	using YAF.Types;

	/// <summary>
	/// The data set extensions.
	/// </summary>
	public static class DataSetExtensions
	{
		#region Public Methods

		/// <summary>
		/// The get table.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		/// <param name="basicTableName">
		/// The basic table name.
		/// </param>
		/// <returns>
		/// </returns>
		public static DataTable GetTable([NotNull] this DataSet dataSet, [NotNull] string basicTableName)
		{
			CodeContracts.ArgumentNotNull(dataSet, "dataSet");
			CodeContracts.ArgumentNotNull(basicTableName, "basicTableName");

			return dataSet.Tables[DataExtensions.GetObjectName(basicTableName)];
		}

		#endregion
	}
}