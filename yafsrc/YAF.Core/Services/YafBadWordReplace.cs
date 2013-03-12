/* YetAnotherForum.NET
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
namespace YAF.Core.Services
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Text.RegularExpressions;

  using YAF.Classes.Data;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The yaf bad word replace.
  /// </summary>
  public class YafBadWordReplace : IBadWordReplace
  {
    #region Constants and Fields

    /// <summary>
    ///   The _options.
    /// </summary>
    private const RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Compiled
                               /*| RegexOptions.Singleline | RegexOptions.Multiline*/;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YafBadWordReplace"/> class.
    /// </summary>
    /// <param name="objectStore">
    /// The object Store.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public YafBadWordReplace([NotNull] IObjectStore objectStore, [NotNull] ILogger logger)
    {
      this.ObjectStore = objectStore;
      this.Logger = logger;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Logger.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    /// Gets or sets ObjectStore.
    /// </summary>
    public IObjectStore ObjectStore { get; set; }

    /// <summary>
    ///   Gets ReplaceItems.
    /// </summary>
    public IEnumerable<BadWordReplaceItem> ReplaceItems
    {
      get
      {
        var replaceItems = this.ObjectStore.GetOrSet(
          Constants.Cache.ReplaceWords, 
          () =>
            {
              var replaceWords = LegacyDb.replace_words_list(YafContext.Current.PageBoardID, null).AsEnumerable();

              // move to collection...
              return
                replaceWords.Select(
                  row => new BadWordReplaceItem(row.Field<string>("goodword"), row.Field<string>("badword"), _options)).ToList();
            });

        return replaceItems;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IBadWordReplace

    /// <summary>
    /// Searches through SearchText and replaces "bad words" with "good words"
    /// as defined in the database.
    /// </summary>
    /// <param name="searchText">
    /// The string to search through.
    /// </param>
    /// <returns>
    /// The replace.
    /// </returns>
    /// <exception cref="Exception">
    /// <c>Exception</c>.
    /// </exception>
    [NotNull]
    public string Replace([NotNull] string searchText)
    {
      if (searchText.IsNotSet())
      {
        return searchText;
      }

      string strReturn = searchText;

      foreach (BadWordReplaceItem item in this.ReplaceItems)
      {
        try
        {
            if (item.BadWordRegEx != null && item.Active)
          {
            strReturn = item.BadWordRegEx.Replace(strReturn, item.GoodWord);
          }
        }

#if DEBUG
        catch (Exception e)
        {
          throw new Exception("Bad Word Regular Expression Failed: " + e.Message, e);
        }

#else
        catch (Exception x)
        {
          // disable this regular expression henceforth...
          item.Active = false;
          this.Logger.Warn(x, "Couldn't run RegEx for Bad Word Replace value: {0}", item.BadWordRegEx);
        }

#endif
      }

      return strReturn;
    }

    #endregion

    #endregion
  }
}