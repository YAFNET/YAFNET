using System;
using System.Data;
using System.IO;

namespace YAF.Classes.Data.Import
{
  /// <summary>
  /// The data import.
  /// </summary>
  public static class DataImport
  {
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
    /// The bb code extension import.
    /// </returns>
    /// <exception cref="Exception">
    /// </exception>
    public static int BBCodeExtensionImport(int boardId, Stream imputStream)
    {
      int importedCount = 0;

      // import extensions...
      var dsBBCode = new DataSet();
      dsBBCode.ReadXml(imputStream);

      if (dsBBCode.Tables["YafBBCode"] != null && dsBBCode.Tables["YafBBCode"].Columns["Name"] != null &&
          dsBBCode.Tables["YafBBCode"].Columns["SearchRegex"] != null && dsBBCode.Tables["YafBBCode"].Columns["ExecOrder"] != null)
      {
        DataTable bbcodeList = LegacyDb.bbcode_list(boardId, null);

        // import any extensions that don't exist...
        foreach (DataRow row in dsBBCode.Tables["YafBBCode"].Rows)
        {
          string name = row["Name"].ToString();

          if (bbcodeList.Select(String.Format("Name = '{0}'", name)).Length == 0)
          {
            // add this bbcode...
            LegacyDb.bbcode_save(
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
    /// The file extension import.
    /// </returns>
    /// <exception cref="Exception">
    /// </exception>
    public static int FileExtensionImport(int boardId, Stream imputStream)
    {
      int importedCount = 0;

      var dsExtensions = new DataSet();
      dsExtensions.ReadXml(imputStream);

      if (dsExtensions.Tables["YafExtension"] != null && dsExtensions.Tables["YafExtension"].Columns["Extension"] != null)
      {
        DataTable extensionList = LegacyDb.extension_list(boardId);

        // import any extensions that don't exist...
        foreach (DataRow row in dsExtensions.Tables["YafExtension"].Rows)
        {
          string ext = row["Extension"].ToString();

          if (extensionList.Select(String.Format("Extension = '{0}'", ext)).Length == 0)
          {
            // add this...
            LegacyDb.extension_save(null, boardId, ext);
            importedCount++;
          }
        }
      }
      else
      {
        throw new Exception("Import stream is not expected format.");
      }

      return importedCount;
    }
  }
}