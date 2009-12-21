/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
namespace YAF.Classes.UI
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Web;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// FormatMsg provides functions related to formatting the post messages.
  /// </summary>
  public static class FormatMsg
  {
    /* Ederon : 6/16/2007 - conventions */

    /// <summary>
    /// format message regex
    /// </summary>
    private static RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

    /// <summary>
    /// For backwards compatibility
    /// </summary>
    /// <param name="message">
    /// the message to add smiles to.
    /// </param>
    /// <returns>
    /// The add smiles.
    /// </returns>
    public static string AddSmiles(string message)
    {
      var layers = new ReplaceRules();
      AddSmiles(ref layers);

      // apply...
      layers.Process(ref message);
      return message;
    }

    /// <summary>
    /// Adds smiles replacement rules to the collection from the DB
    /// </summary>
    /// <param name="rules">
    /// The rules.
    /// </param>
    public static void AddSmiles(ref ReplaceRules rules)
    {
      DataTable dtSmileys = GetSmilies();
      int codeOffset = 0;

      foreach (DataRow row in dtSmileys.Rows)
      {
        string code = row["Code"].ToString();
        code = code.Replace("&", "&amp;");
        code = code.Replace(">", "&gt;");
        code = code.Replace("<", "&lt;");
        code = code.Replace("\"", "&quot;");

        // add new rules for smilies...
        var lowerRule = new SimpleReplaceRule(
          code.ToLower(), 
          String.Format(
            "<img src=\"{0}\" alt=\"{1}\" />", 
            YafBuildLink.Smiley(Convert.ToString(row["Icon"])), 
            HttpContext.Current.Server.HtmlEncode(row["Emoticon"].ToString())));
        var upperRule = new SimpleReplaceRule(
          code.ToUpper(), 
          String.Format(
            "<img src=\"{0}\" alt=\"{1}\" />", 
            YafBuildLink.Smiley(Convert.ToString(row["Icon"])), 
            HttpContext.Current.Server.HtmlEncode(row["Emoticon"].ToString())));

        // increase the rank as we go...
        lowerRule.RuleRank = lowerRule.RuleRank + 100 + codeOffset;
        upperRule.RuleRank = upperRule.RuleRank + 100 + codeOffset;

        rules.AddRule(lowerRule);
        rules.AddRule(upperRule);

        // add a bit more rank
        codeOffset++;
      }
    }

    /// <summary>
    /// The get smilies.
    /// </summary>
    /// <returns>
    /// </returns>
    public static DataTable GetSmilies()
    {
      string cacheKey = YafCache.GetBoardCacheKey(Constants.Cache.Smilies);
      var smiliesTable = YafContext.Current.Cache[cacheKey] as DataTable;

      // check if there is value cached
      if (smiliesTable == null)
      {
        // get the smilies table from the db...
        smiliesTable = DB.smiley_list(YafContext.Current.PageBoardID, null);

        // cache it for 60 minutes...
        YafContext.Current.Cache.Insert(cacheKey, smiliesTable, null, DateTime.Now.AddMinutes(60), TimeSpan.Zero);
      }

      return smiliesTable;
    }

    /// <summary>
    /// The format message.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="messageFlags">
    /// The message flags.
    /// </param>
    /// <returns>
    /// The format message.
    /// </returns>
    public static string FormatMessage(string message, MessageFlags messageFlags)
    {
      return FormatMessage(message, messageFlags, false);
    }

    /// <summary>
    /// The format message.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="messageFlags">
    /// The message flags.
    /// </param>
    /// <param name="targetBlankOverride">
    /// The target blank override.
    /// </param>
    /// <returns>
    /// The format message.
    /// </returns>
    public static string FormatMessage(string message, MessageFlags messageFlags, bool targetBlankOverride)
    {
      return FormatMessage(message, messageFlags, targetBlankOverride, DateTime.Now);
    }

    /// <summary>
    /// The format message.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="messageFlags">
    /// The message flags.
    /// </param>
    /// <param name="targetBlankOverride">
    /// The target blank override.
    /// </param>
    /// <param name="messageLastEdited">
    /// The message last edited.
    /// </param>
    /// <returns>
    /// The format message.
    /// </returns>
    public static string FormatMessage(string message, MessageFlags messageFlags, bool targetBlankOverride, DateTime messageLastEdited)
    {
      bool useNoFollow = YafContext.Current.BoardSettings.UseNoFollowLinks;

      // check to see if no follow should be disabled since the message is properly aged
      if (useNoFollow && YafContext.Current.BoardSettings.DisableNoFollowLinksAfterDay > 0)
      {
        TimeSpan messageAge = messageLastEdited - DateTime.Now;
        if (messageAge.Days > YafContext.Current.BoardSettings.DisableNoFollowLinksAfterDay)
        {
          // disable no follow
          useNoFollow = false;
        }
      }

      // do html damage control
      message = RepairHtml(message, messageFlags.IsHtml);

      // get the rules engine from the creator...
      ReplaceRules ruleEngine = ReplaceRulesCreator.GetInstance(
        new[]
          {
            messageFlags.IsBBCode, targetBlankOverride, useNoFollow
          });

      // see if the rules are already populated...
      if (!ruleEngine.HasRules)
      {
        // populate

        // get rules for YafBBCode and Smilies
        YafBBCode.CreateBBCodeRules(ref ruleEngine, messageFlags.IsBBCode, targetBlankOverride, useNoFollow);

        // add email rule
        // vzrus: it's freezing  when post body contains full email adress.
        // the fix provided by community 
        var email = new VariableRegexReplaceRule(
          @"(?<before>^|[ ]|<br/>)(?<inner>(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6})",
          "${before}<a href=\"mailto:${inner}\">${inner}</a>",
          _options,
          new[]
            { 
              "before" 
            });   
  
        email.RuleRank = 10;

        ruleEngine.AddRule(email);

        // URLs Rules
        string target = (YafContext.Current.BoardSettings.BlankLinks || targetBlankOverride) ? "target=\"_blank\"" : string.Empty;
        string nofollow = useNoFollow ? "rel=\"nofollow\"" : string.Empty;

        var url =
          new VariableRegexReplaceRule(
            @"(?<before>^|[ ]|<br/>)(?<!href="")(?<!src="")(?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?+%#&=;:,]*)?)", 
            "${before}<a {0} {1} href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>".Replace("{0}", target).Replace("{1}", nofollow), 
            _options, 
            new[]
              {
                "before"
              }, 
            new[]
              {
                string.Empty
              }, 
            50);

        url.RuleRank = 10;
        ruleEngine.AddRule(url);

        url =
          new VariableRegexReplaceRule(
            @"(?<before>^|[ ]|<br/>)(?<!href="")(?<!src="")(?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=+;,:#~$]*[^.<])?)", 
            "${before}<a {0} {1} href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>".Replace("{0}", target).Replace("{1}", nofollow), 
            _options, 
            new[]
              {
                "before"
              }, 
            new[]
              {
                string.Empty
              }, 
            50);
        url.RuleRank = 10;
        ruleEngine.AddRule(url);

        url = new VariableRegexReplaceRule(
          @"(?<before>^|[ ]|<br/>)(?<!http://)(?<inner>www\.(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%+#&=;,]*)?)",
          "${before}<a {0} {1} href=\"http://${inner}\" title=\"http://${inner}\">${innertrunc}</a>".Replace("{0}", target).Replace("{1}", nofollow),
          _options,
          new[]
            {
              "before"
            },
          new[]
            {
              string.Empty
            },
          50);
        url.RuleRank = 10;
        ruleEngine.AddRule(url);
      }

      // process...
      ruleEngine.Process(ref message);

      message = YafServices.BadWordReplace.Replace(message);

      return message;
    }

    /// <summary>
    /// The get cleaned topic message. Caches cleaned topic message by TopicID.
    /// </summary>
    /// <param name="topicMessage">
    /// The message to clean.
    /// </param>
    /// <param name="topicId"> 
    /// The topic id.
    /// </param>
    /// <returns>
    /// The get cleaned topic message.
    /// </returns>
    public static MessageCleaned GetCleanedTopicMessage(object topicMessage, object topicId)
    {
      if (topicMessage == null)
      {
        throw new ArgumentNullException("topicMessage", "topicMessage is null.");
      }

      if (topicId == null)
      {
        throw new ArgumentNullException("topicId", "topicId is null.");
      }

      // get the common words for the language -- should be all lower case.
      List<string> commonWords = YafContext.Current.Localization.GetText("COMMON", "COMMON_WORDS").StringToList(',');

      string cacheKey = String.Format(Constants.Cache.FirstPostCleaned, YafContext.Current.PageBoardID, topicId);
      MessageCleaned message = new MessageCleaned();

      if (!topicMessage.IsNullOrEmptyDBField())
      {
        message = YafContext.Current.Cache.GetItem<MessageCleaned>(
          cacheKey, 
          YafContext.Current.BoardSettings.FirstPostCacheTimeout, 
          () =>
          {
            string returnMsg = topicMessage.ToString();
            var keywordList = new List<string>();

            if (!String.IsNullOrEmpty(returnMsg))
            {
              var flags = new MessageFlags
                {
                  IsBBCode = true, 
                  IsSmilies = true
                };

              // process message... clean html, strip html, remove bbcode, etc...
              returnMsg = StringHelper.RemoveMultipleWhitespace(BBCodeHelper.StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(returnMsg))));

              if (String.IsNullOrEmpty(returnMsg))
              {
                returnMsg = string.Empty;
              }
              else
              {
                // get string without punctuation
                string keywordCleaned = new string(returnMsg.Where(c => !char.IsPunctuation(c) || char.IsWhiteSpace(c)).ToArray()).ToLower();

                // create keywords...
                keywordList = keywordCleaned.StringToList(' ', commonWords);

                // clean up the list a bit...
                keywordList = keywordList.RemoveEmptyStrings().RemoveSmallStrings(5).Where(x => !Char.IsNumber(x[0])).Distinct().ToList();

                // sort...
                keywordList.Sort();

                // get maximum of 50 keywords...
                if (keywordList.Count > 50)
                {
                  keywordList = keywordList.GetRange(0, 50);
                }
              }
            }

            return new MessageCleaned(StringHelper.Truncate(returnMsg, 255), keywordList);
          });
      }

      return message;
    }

    /// <summary>
    /// Removes nested YafBBCode quotes from the given message body.
    /// </summary>
    /// <param name="body">
    /// Message body test to remove nested quotes from
    /// </param>
    /// <returns>
    /// A version of <paramref name="body"/> that contains no nested quotes.
    /// </returns>
    public static string RemoveNestedQuotes(string body)
    {
      RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;
      var quote = new Regex(@"\[quote(\=[^\]]*)?\](.*?)\[/quote\]", options);

      // remove quotes from old messages
      return quote.Replace(body, string.Empty).TrimStart();
    }

    /// <summary>
    /// The repair html.
    /// </summary>
    /// <param name="html">
    /// The html.
    /// </param>
    /// <param name="allowHtml">
    /// The allow html.
    /// </param>
    /// <returns>
    /// The repair html.
    /// </returns>
    public static string RepairHtml(string html, bool allowHtml)
    {
      if (!allowHtml)
      {
        html = YafBBCode.EncodeHTML(html);
      }
      else
      {
        // get allowable html tags
        string tStr = YafContext.Current.BoardSettings.AcceptedHTML;
        string[] allowedTags = tStr.Split(',');

        RegexOptions options = RegexOptions.IgnoreCase;

        MatchCollection m = Regex.Matches(html, "<.*?>", options);

        for (int i = m.Count - 1; i >= 0; i--)
        {
          string tag = html.Substring(m[i].Index + 1, m[i].Length - 1).Trim().ToLower();

          if (!HtmlHelper.IsValidTag(tag, allowedTags))
          {
            html = html.Remove(m[i].Index, m[i].Length);

            // just don't show this tag for now

            // string tmp = System.Web.HttpContext.Current.Server.HtmlEncode(html.Substring(m[i].Index,m[i].Length));
            // html = html.Insert(m[i].Index,tmp);
          }
        }
      }

      return html;
    }

    [Serializable]
    public class MessageCleaned
    {
      public MessageCleaned()
      {
      }

      public MessageCleaned(string messageTruncated, List<string> messageKeywords)
      {
        this.MessageTruncated = messageTruncated;
        this.MessageKeywords = messageKeywords;
      }

      public string MessageTruncated
      {
        get;
        set;
      }

      public List<string> MessageKeywords
      {
        get;
        set;
      }
    }
  }
}