/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
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
			CodeContracts.VerifyNotNull(dataSet, "dataSet");
			CodeContracts.VerifyNotNull(basicTableName, "basicTableName");

			return dataSet.Tables[DataExtensions.GetObjectName(basicTableName)];
		}

		#endregion
	}
}