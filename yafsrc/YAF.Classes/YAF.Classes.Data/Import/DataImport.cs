using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace YAF.Classes.Data.Import
{
	static public class DataImport
	{
		public static int BBCodeExtensionImport( int boardId, System.IO.Stream imputStream )
		{
			int importedCount = 0;

			// import extensions...
			DataSet dsBBCode = new DataSet();
			dsBBCode.ReadXml( imputStream );

			if ( dsBBCode.Tables ["YafBBCode"] != null &&
						dsBBCode.Tables ["YafBBCode"].Columns ["Name"] != null &&
						dsBBCode.Tables ["YafBBCode"].Columns ["SearchRegex"] != null &&
						dsBBCode.Tables ["YafBBCode"].Columns ["ExecOrder"] != null )
			{
				DataTable bbcodeList = YAF.Classes.Data.DB.bbcode_list( boardId, null );
				// import any extensions that don't exist...
				foreach ( DataRow row in dsBBCode.Tables ["YafBBCode"].Rows )
				{
					string name = row ["Name"].ToString();

					if ( bbcodeList.Select( String.Format( "Name = '{0}'", name ) ).Length == 0 )
					{
						// add this bbcode...
						DB.bbcode_save( null, boardId, row ["Name"], row ["Description"], row ["OnClickJS"], row ["DisplayJS"], row ["EditJS"], row ["DisplayCSS"], row ["SearchRegex"], row ["ReplaceRegex"], row ["Variables"], row ["UseModule"], row ["ModuleClass"], row ["ExecOrder"] );
						importedCount++;
					}
				}
			}
			else
			{
				throw Exception( "Import stream is not expected format." );
			}

			return importedCount;
		}

		public static int FileExtensionImport( int boardId, System.IO.Stream imputStream )
		{
			int importedCount = 0;

			DataSet dsExtensions = new DataSet();
			dsExtensions.ReadXml( imputStream );

			if ( dsExtensions.Tables ["YafExtension"] != null && dsExtensions.Tables ["YafExtension"].Columns ["Extension"] != null )
			{
				DataTable extensionList = YAF.Classes.Data.DB.extension_list( boardId );
				// import any extensions that don't exist...
				foreach ( DataRow row in dsExtensions.Tables ["YafExtension"].Rows )
				{
					string ext = row ["Extension"].ToString();

					if ( extensionList.Select( String.Format( "Extension = '{0}'", ext ) ).Length == 0 )
					{
						// add this...
						DB.extension_save( null, boardId, ext );
						importedCount++;
					}
				}
			}
			else
			{
				throw Exception( "Import stream is not expected format." );
			}

			return importedCount;
		}
	}
}
