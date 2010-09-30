namespace YAF.Classes.Data
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Text;

  using YAF.Classes.Extensions;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  public static class SearchBuilderExtensions
  {
    public static string BuildSql(this IEnumerable<SearchCondition> conditions, bool surroundWithParathesis)
    {
      var sb = new StringBuilder();

      conditions.ForEachFirst(
        (item,
        isFirst) =>
          {
            sb.Append(" ");
            if (!isFirst)
            {
              sb.Append(item.ConditionType.GetStringValue());
              sb.Append(" ");
            }

            if (surroundWithParathesis)
            {
              sb.AppendFormat("({0})", item.Condition);
            }
            else
            {
              sb.Append(item.Condition);
            }
          });

      return sb.ToString();
    }
  }

    #region Enums

    /// <summary>
    /// The search condition type.
    /// </summary>
    public enum SearchConditionType
    {
      /// <summary>
      /// The and.
      /// </summary>
      AND, 

      /// <summary>
      /// The or.
      /// </summary>
      OR
    }

    #endregion

  /// <summary>
  /// The search builder.
  /// </summary>
  public class SearchBuilder
  {
    #region Public Methods

    /// <summary>
    /// The build search sql.
    /// </summary>
    /// <param name="toSearchWhat">
    /// The to search what.
    /// </param>
    /// <param name="toSearchFromWho">
    /// The to search from who.
    /// </param>
    /// <param name="searchFromWhoMethod">
    /// The search from who method.
    /// </param>
    /// <param name="searchWhatMethod">
    /// The search what method.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="searchDisplayName">
    /// The search display name.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="maxResults">
    /// The max results.
    /// </param>
    /// <param name="useFullText">
    /// The use full text.
    /// </param>
    /// <param name="forumIds">
    /// The forum ids.
    /// </param>
    /// <returns>
    /// The build search sql.
    /// </returns>
    public string BuildSearchSql([NotNull] string toSearchWhat, [NotNull] string toSearchFromWho, 
                                 SearchWhatFlags searchFromWhoMethod, 
                                 SearchWhatFlags searchWhatMethod, 
                                 int userID, 
                                 bool searchDisplayName, 
                                 int boardId, 
                                 int maxResults, 
                                 bool useFullText, [NotNull] IEnumerable<int> forumIds)
    {
      List<string> builtStatements = new List<string>();

      if (maxResults > 0)
      {
        builtStatements.Add("SET ROWCOUNT {0};".FormatWith(maxResults));
      }

      string searchSql =
        "SELECT a.ForumID, a.TopicID, a.Topic, b.UserID, IsNull(c.Username, b.Name) as Name, c.MessageID, c.Posted, [Message] = '', c.Flags ";
      searchSql +=
        "\r\nfrom {databaseOwner}.{objectQualifier}topic a left join {databaseOwner}.{objectQualifier}message c on a.TopicID = c.TopicID left join {databaseOwner}.{objectQualifier}user b on c.UserID = b.UserID join {databaseOwner}.{objectQualifier}vaccess x on x.ForumID=a.ForumID ";
      searchSql +=
        "\r\nwhere x.ReadAccess<>0 AND x.UserID={0} AND c.IsApproved = 1 AND a.TopicMovedID IS NULL AND a.IsDeleted = 0 AND c.IsDeleted = 0"
          .FormatWith(userID);

      if (forumIds.Any())
      {
        searchSql += " AND a.ForumID IN ({0})".FormatWith(forumIds.ToDelimitedString(","));
      }

      if (toSearchFromWho.IsSet())
      {
        searchSql +=
          "\r\nAND ({0})".FormatWith(
            this.BuildWhoConditions(toSearchFromWho, searchFromWhoMethod, searchDisplayName).BuildSql(true));
      }

      if (toSearchWhat.IsSet())
      {
        builtStatements.Add(searchSql);

        builtStatements.Add(
          "AND ({0})".FormatWith(
            this.BuildWhatConditions(toSearchWhat, searchWhatMethod, "c.Message", useFullText).BuildSql(true)));

        builtStatements.Add("UNION");

        builtStatements.Add(searchSql);

        builtStatements.Add(
          "AND ({0})".FormatWith(
            this.BuildWhatConditions(toSearchWhat, searchWhatMethod, "a.Topic", useFullText).BuildSql(true)));
      }
      else
      {
        builtStatements.Add(searchSql);
      }

      builtStatements.Add("ORDER BY c.Posted DESC");

      string builtSql = builtStatements.ToDelimitedString("\r\n");

      Debug.WriteLine("Build Sql: [{0}]".FormatWith(builtSql));

      return builtSql;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The build search who conditions.
    /// </summary>
    /// <param name="toSearchFromWho">
    /// The to search from who.
    /// </param>
    /// <param name="searchFromWhoMethod">
    /// The search from who method.
    /// </param>
    /// <param name="searchDisplayName">
    /// The search display name.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    protected IEnumerable<SearchCondition> BuildWhatConditions([NotNull] string toSearchWhat, SearchWhatFlags searchWhatMethod, string dbField, bool useFullText)
    {
      CodeContracts.ArgumentNotNull(toSearchWhat, "toSearchWhat");

      toSearchWhat = toSearchWhat.Replace("'", "''").Trim();

      var conditions = new List<SearchCondition>();
      string conditionSql = string.Empty;

      SearchConditionType conditionType = SearchConditionType.AND;

      if (searchWhatMethod == SearchWhatFlags.AnyWords)
      {
        conditionType = SearchConditionType.OR;
      }

      var wordList = new List<string> { toSearchWhat };

      if (searchWhatMethod == SearchWhatFlags.AllWords || searchWhatMethod == SearchWhatFlags.AnyWords)
      {
        wordList =
          toSearchWhat.Replace(@"""", string.Empty).Split(' ').Where(x => x.IsSet()).Select(x => x.Trim()).ToList();
      }

      if (useFullText)
      {
        var list = new List<SearchCondition>();

        list.AddRange(
          wordList.Select(
            word => new SearchCondition { Condition = @"""{0}""".FormatWith(word), ConditionType = conditionType }));

        conditions.Add(
          new SearchCondition()
            {
              Condition =
                "CONTAINS ({1}, N' {0} ')".FormatWith(list.BuildSql(false), dbField),
              ConditionType = conditionType
            });
      }
      else
      {
        conditions.AddRange(
          wordList.Select(
            word =>
            new SearchCondition
              {
                Condition = "{1} LIKE N'%{0}%'".FormatWith(word, dbField),
                ConditionType = conditionType
              }));
      }

      return conditions;
    }

    /// <summary>
    /// The build search who conditions.
    /// </summary>
    /// <param name="toSearchFromWho">
    /// The to search from who.
    /// </param>
    /// <param name="searchFromWhoMethod">
    /// The search from who method.
    /// </param>
    /// <param name="searchDisplayName">
    /// The search display name.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    protected IEnumerable<SearchCondition> BuildWhoConditions([NotNull] string toSearchFromWho, SearchWhatFlags searchFromWhoMethod, bool searchDisplayName)
    {
      CodeContracts.ArgumentNotNull(toSearchFromWho, "toSearchFromWho");

      toSearchFromWho = toSearchFromWho.Replace("'", "''").Trim();

      var conditions = new List<SearchCondition>();
      string conditionSql = string.Empty;

      SearchConditionType conditionType = SearchConditionType.AND;

      if (searchFromWhoMethod == SearchWhatFlags.AnyWords)
      {
        conditionType = SearchConditionType.OR;
      }

      var wordList = new List<string> { toSearchFromWho };

      if (searchFromWhoMethod == SearchWhatFlags.AllWords || searchFromWhoMethod == SearchWhatFlags.AnyWords)
      {
        wordList =
          toSearchFromWho.Replace(@"""", string.Empty).Split(' ').Where(x => x.IsSet()).Select(x => x.Trim()).ToList();
      }

      foreach (string word in wordList)
      {
        int userId;

        if (int.TryParse(word, out userId))
        {
          conditionSql = "c.UserID IN ({0})".FormatWith(userId);
        }
        else
        {
          if (searchFromWhoMethod == SearchWhatFlags.ExactMatch)
          {
            conditionSql = "(c.Username IS NULL AND b.{1} = N'{0}') OR (c.Username = N'{0}')".FormatWith(
              word, searchDisplayName ? "DisplayName" : "Name");
          }
          else
          {
            conditionSql = "(c.Username IS NULL AND b.{1} LIKE N'%{0}%') OR (c.Username LIKE N'%{0}%')".FormatWith(
              word, searchDisplayName ? "DisplayName" : "Name");
          }
        }

        conditions.Add(new SearchCondition { Condition = conditionSql, ConditionType = conditionType });
      }

      return conditions;
    }

    #endregion
  }

    /// <summary>
    /// The search condition.
    /// </summary>
    public class SearchCondition
    {
      #region Constructors and Destructors

      /// <summary>
      /// Initializes a new instance of the <see cref="SearchCondition"/> class.
      /// </summary>
      public SearchCondition()
      {
        this.ConditionType = SearchConditionType.AND;
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets Condition.
      /// </summary>
      public string Condition { get; set; }

      /// <summary>
      /// Gets or sets ConditionType.
      /// </summary>
      public SearchConditionType ConditionType { get; set; }

      #endregion
    }
}