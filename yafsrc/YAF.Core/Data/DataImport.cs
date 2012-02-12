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

namespace YAF.Classes.Data.Import
{
	#region Using

	using System;
	using System.Data;
	using System.IO;
	using System.Linq;

	using YAF.Types;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	#endregion

	/// <summary>
	/// The data import.
	/// </summary>
	public class DataImport
	{
		#region Constants and Fields

		/// <summary>
		/// The _db function.
		/// </summary>
		private readonly IDbFunction _dbFunction;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataImport"/> class.
		/// </summary>
		/// <param name="dbFunction">
		/// The db function.
		/// </param>
		public DataImport([NotNull] IDbFunction dbFunction)
		{
			this._dbFunction = dbFunction;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// The bb code extension import.
		/// </summary>
		/// <param name="boardId">
		/// The board id.
		/// </param>
		/// <param name="imputStream">
		/// The imput stream.
		/// </param>
		/// <returns>
		/// Returns How Many Extensions where imported.
		/// </returns>
		/// <exception cref="Exception">
		/// Import stream is not expected format.
		/// </exception>
		public int BBCodeExtensionImport(int boardId, [NotNull] Stream imputStream)
		{
			int importedCount = 0;

			// import extensions...
			var dsBBCode = new DataSet();
			dsBBCode.ReadXml(imputStream);

			if (dsBBCode.Tables["YafBBCode"] != null && dsBBCode.Tables["YafBBCode"].Columns["Name"] != null
			    && dsBBCode.Tables["YafBBCode"].Columns["SearchRegex"] != null
			    && dsBBCode.Tables["YafBBCode"].Columns["ExecOrder"] != null)
			{
				var bbcodeList = LegacyDb.BBCodeList(boardId, null);

				// import any extensions that don't exist...
				foreach (DataRow row in from DataRow row in dsBBCode.Tables["YafBBCode"].Rows
				                        let name = row["Name"].ToString()
				                        where !bbcodeList.Any(b => b.Name == name)
				                        select row)
				{
					// add this bbcode...
					this._dbFunction.Query.bbcode_save(
						null, 
						boardId, 
						row["Name"], 
						row["Description"], 
						row["OnClickJS"], 
						row["DisplayJS"], 
						row["EditJS"], 
						row["DisplayCSS"], 
						row["SearchRegex"], 
						row["ReplaceRegex"], 
						row["Variables"], 
						Convert.ToBoolean(row["UseModule"]), 
						row["ModuleClass"], 
						row["ExecOrder"]);

					importedCount++;
				}
			}
			else
			{
				throw new Exception("Import stream is not expected format.");
			}

			return importedCount;
		}

		/// <summary>
		/// The file extension import.
		/// </summary>
		/// <param name="boardId">
		/// The board id.
		/// </param>
		/// <param name="imputStream">
		/// The imput stream.
		/// </param>
		/// <returns>
		/// Returns How Many Extensions where imported.
		/// </returns>
		/// <exception cref="Exception">
		/// Import stream is not expected format.
		/// </exception>
		public int FileExtensionImport(int boardId, [NotNull] Stream imputStream)
		{
			int importedCount = 0;

			var dsExtensions = new DataSet();
			dsExtensions.ReadXml(imputStream);

			if (dsExtensions.Tables["YafExtension"] != null && dsExtensions.Tables["YafExtension"].Columns["Extension"] != null)
			{
				DataTable extensionList = LegacyDb.extension_list(boardId);

				// import any extensions that don't exist...
				foreach (string ext in
					dsExtensions.Tables["YafExtension"].Rows.Cast<DataRow>().Select(row => row["Extension"].ToString()).Where(
						ext => extensionList.Select("Extension = '{0}'".FormatWith(ext)).Length == 0))
				{
					// add this...
					this._dbFunction.Query.extension_save(null, boardId, ext);
					importedCount++;
				}
			}
			else
			{
				throw new Exception("Import stream is not expected format.");
			}

			return importedCount;
		}

		/// <summary>
		/// Topics the status import.
		/// </summary>
		/// <param name="boardId">
		/// The board id.
		/// </param>
		/// <param name="imputStream">
		/// The imput stream.
		/// </param>
		/// <exception cref="Exception">
		/// Import stream is not expected format.
		/// </exception>
		/// <returns>
		/// Returns the Number of Imported Items.
		/// </returns>
		public int TopicStatusImport(int boardId, [NotNull] Stream imputStream)
		{
			int importedCount = 0;

			// import extensions...
			var dsStates = new DataSet();
			dsStates.ReadXml(imputStream);

			if (dsStates.Tables["YafTopicStatus"] != null && dsStates.Tables["YafTopicStatus"].Columns["TopicStatusName"] != null
			    && dsStates.Tables["YafTopicStatus"].Columns["DefaultDescription"] != null)
			{
				var topicStatusList = (DataTable)this._dbFunction.GetData.TopicStatus_List(boardId);

				// import any topic status that don't exist...
				foreach (DataRow row in
					dsStates.Tables["YafTopicStatus"].Rows.Cast<DataRow>().Where(
						row => topicStatusList.Select("TopicStatusName = '{0}'".FormatWith(row["TopicStatusName"])).Length == 0))
				{
					// add this...
					this._dbFunction.Query.TopicStatus_Save(
						null, boardId, row["TopicStatusName"].ToString(), row["DefaultDescription"].ToString());
					importedCount++;
				}
			}
			else
			{
				throw new Exception("Import stream is not expected format.");
			}

			return importedCount;
		}

		#endregion
	}
}