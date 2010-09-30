namespace YAF.Classes.Data
{
  using System.Collections.Generic;
  using System.Text;

  using YAF.Classes.Extensions;
  using YAF.Classes.Pattern;

  using EnumExtensions = YAF.Classes.Utils.EnumExtensions;

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
              sb.Append(EnumExtensions.GetStringValue(item.ConditionType));
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