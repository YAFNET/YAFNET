/* Yet Another Forum.NET
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
namespace YAF.Data.MsSql.Search
{
    using System.Collections.Generic;
    using System.Text;

    using YAF.Types;
    using YAF.Types.Extensions;

    /// <summary>
  /// The search builder extensions.
  /// </summary>
  public static class SearchBuilderExtensions
  {
    #region Public Methods

    /// <summary>
    /// The build sql.
    /// </summary>
    /// <param name="conditions">
    /// The conditions.
    /// </param>
    /// <param name="surroundWithParathesis">
    /// The surround with parathesis.
    /// </param>
    /// <returns>
    /// The build sql.
    /// </returns>
    [NotNull]
    public static string BuildSql([NotNull] this IEnumerable<SearchCondition> conditions, bool surroundWithParathesis)
    {
      var sb = new StringBuilder();

      conditions.ForEachFirst(
        (item, isFirst) =>
          {
            sb.Append(" ");
            if (!isFirst)
            {
              sb.Append(item.ConditionType.GetStringValue());
              sb.Append(" ");
            }

            if (surroundWithParathesis)
            {
              sb.AppendFormat("({0})", (object)item.Condition);
            }
            else
            {
              sb.Append((string)item.Condition);
            }
          });

      return sb.ToString();
    }

    #endregion
  }
}