/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Web;

  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// FormatMsg provides functions related to formatting the post messages.
  /// </summary>
  public static class FormatMsg
  {
    /* Ederon : 6/16/2007 - conventions */
    #region Constants and Fields

    /// <summary>
    /// format message regex
    /// </summary>
    private static RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

    #endregion

    #region Public Methods

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
    /// The method to detect a forbidden BBCode tag from delimited by 'delim' list 
    /// 'stringToMatch'
    /// </summary>
    /// <param name="stringToClear">
    /// Input string
    /// </param>
    /// <param name="stringToMatch">
    /// String with delimiter
    /// </param>
    /// <param name="delim">
    /// </param>
    /// <returns>
    /// Returns a string containing a forbidden BBCode or a null string
    /// </returns>
    public static string BBCodeForbiddenDetector(string stringToClear, string stringToMatch, char delim)
    {
      bool checker = false;
      if (string.IsNullOrEmpty(stringToMatch))
      {
        checker = true;
      }

      string tagForbidden = null;
      bool detectedTag = false;
      string[] codes = stringToMatch.Split(delim);
      char[] charray = stringToClear.ToCharArray();
      int openPosition = 0;
      int currentPosition = 0;

      // Loop through char array i will be current poision
      for (int i = 0; i < charray.Length; i++)
      {
        if (i >= currentPosition)
        {
          // bbcode token is detected
          if (charray[i] == '[')
          {
            openPosition = i;

            // we loop to find closing bracket, beginnin with i position
            for (int j = i; j < charray.Length - 1; j++)
            {
              // closing bracket found
              if (charray[j] == ']')
              {
                // we should reset the value in each cycle 
                // if an opening bracket was found
                detectedTag = false;
                string res = null;

                // now we loop through out allowed bbcode list
                for (int k = 0; k < codes.Length; k++)
                {
                  // closing bracket is in position 'j' opening is in pos 'i'
                  // we should not take into account closing bracket
                  // as we have tags like '[URL='
                  for (int l = openPosition; l < j; l++)
                  {
                    res = res + charray[l].ToString().ToUpper();
                  }

                  if (checker)
                  {
                    return "ALL";
                  }

                  // detect if the tag from list was found
                  detectedTag = res.Contains("[" + codes[k].ToUpper().Trim()) ||
                                res.Contains("[/" + codes[k].ToUpper().Trim());
                  res = string.Empty;

                  // if so we go out from k-loop after we should go out from j-loop too
                  if (detectedTag)
                  {
                    currentPosition = j + 1;
                    break;
                  }
                }

                currentPosition = j + 1;

                // we didn't found the allowed tag in k-loop through allowed list,
                // so the tag is forbidden one and we should exit
                if (!detectedTag)
                {
                  tagForbidden = stringToClear.Substring(i, j - i + 1).ToUpper();
                  return tagForbidden;
                }

                if (detectedTag)
                {
                  break;
                }
              }

              // continue to loop
            }
          }
        }
      }

      return null;
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
    /// The formatted message.
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
    /// The formated message.
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
    /// The formatted message.
    /// </returns>
    public static string FormatMessage(
      string message, MessageFlags messageFlags, bool targetBlankOverride, DateTime messageLastEdited)
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
      ReplaceRules ruleEngine =
        ReplaceRulesCreator.GetInstance(new[] { messageFlags.IsBBCode, targetBlankOverride, useNoFollow });

      // see if the rules are already populated...
      if (!ruleEngine.HasRules)
      {
        // populate

        // get rules for YafBBCode and Smilies
        YafBBCode.CreateBBCodeRules(ref ruleEngine, messageFlags.IsBBCode, targetBlankOverride, useNoFollow);

        // add email rule
        // vzrus: it's freezing  when post body contains full email adress.
        // the fix provided by community 
        var email =
          new VariableRegexReplaceRule(
            @"(?<before>^|[ ]|<br/>)(?<inner>(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6})", 
            "${before}<a href=\"mailto:${inner}\">${inner}</a>", 
            _options, 
            new[] { "before" });

        email.RuleRank = 10;

        ruleEngine.AddRule(email);

        // URLs Rules
        string target = (YafContext.Current.BoardSettings.BlankLinks || targetBlankOverride)
                          ? "target=\"_blank\""
                          : string.Empty;
        string nofollow = useNoFollow ? "rel=\"nofollow\"" : string.Empty;

        var url =
          new VariableRegexReplaceRule(
            @"(?<before>^|[ ]|<br/>)(?<!href="")(?<!src="")(?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?+%#&=;:,]*)?)", 
            "${before}<a {0} {1} href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>".Replace("{0}", target).Replace(
              "{1}", nofollow), 
            _options, 
            new[] { "before" }, 
            new[] { string.Empty }, 
            50);

        url.RuleRank = 10;
        ruleEngine.AddRule(url);

        url =
          new VariableRegexReplaceRule(
            @"(?<before>^|[ ]|<br/>)(?<!href="")(?<!src="")(?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=+;,:#~$]*[^.<])?)", 
            "${before}<a {0} {1} href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>".Replace("{0}", target).Replace(
              "{1}", nofollow), 
            _options, 
            new[] { "before" }, 
            new[] { string.Empty }, 
            50);
        url.RuleRank = 10;
        ruleEngine.AddRule(url);

        url =
          new VariableRegexReplaceRule(
            @"(?<before>^|[ ]|<br/>)(?<!http://)(?<inner>www\.(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%+#&=;,]*)?)", 
            "${before}<a {0} {1} href=\"http://${inner}\" title=\"http://${inner}\">${innertrunc}</a>".Replace(
              "{0}", target).Replace("{1}", nofollow), 
            _options, 
            new[] { "before" }, 
            new[] { string.Empty }, 
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
    /// 
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
      var message = new MessageCleaned();

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
                var flags = new MessageFlags { IsBBCode = true, IsSmilies = true };

                // process message... clean html, strip html, remove bbcode, etc...
                returnMsg =
                  StringHelper.RemoveMultipleWhitespace(
                    BBCodeHelper.StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(returnMsg))));

                if (String.IsNullOrEmpty(returnMsg))
                {
                  returnMsg = string.Empty;
                }
                else
                {
                  // get string without punctuation
                  string keywordCleaned =
                    new string(returnMsg.Where(c => !char.IsPunctuation(c) || char.IsWhiteSpace(c)).ToArray()).Trim().ToLower();

                  if (keywordCleaned.Length > 5)
                  {
                    // create keywords...
                    keywordList = keywordCleaned.StringToList(' ', commonWords);

                    // clean up the list a bit...
                    keywordList =
                      keywordList.RemoveEmptyStrings().RemoveSmallStrings(5).Where(x => !Char.IsNumber(x[0])).Distinct()
                        .ToList();

                    // sort...
                    keywordList.Sort();

                    // get maximum of 50 keywords...
                    if (keywordList.Count > 50)
                    {
                      keywordList = keywordList.GetRange(0, 50);
                    }
                  }
                }
              }

              return new MessageCleaned(StringHelper.Truncate(returnMsg, 255), keywordList);
            });
      }

      return message;
    }

    /// <summary>
    /// The get smilies.
    /// </summary>
    /// <returns>
    /// Table with list of smiles
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
    /// The method to detect a forbidden HTML code from delimited by 'delim' list
    /// </summary>
    /// <param name="stringToClear">
    /// </param>
    /// <param name="stringToMatch">
    /// </param>
    /// <param name="delim">
    /// </param>
    /// <returns>
    /// Returns a forbidden HTML tag or a null string
    /// </returns>
    public static string HTMLTagForbiddenDetector(string stringToClear, string stringToMatch, char delim)
    {
      bool checker = false;
      if (string.IsNullOrEmpty(stringToMatch))
      {
        checker = true;
      }

      char[] charray = stringToClear.ToCharArray();
      for (int i = 0; i < charray.Length; i++)
      {
        if (charray[i] == '<')
        {
          string[] allowedHTMLTags = stringToMatch.Split(delim);
          for (int j = 0; j < allowedHTMLTags.Length; j++)
          {
            if (checker)
            {
              return "ALL";
            }

            string toFind = null;
            for (int k = 0; k < allowedHTMLTags[j].Length; k++)
            {
              toFind = toFind + charray[i + k + 1].ToString();
            }

            if (toFind == allowedHTMLTags[j])
            {
              return toFind;
            }
          }
        }
      }

      return null;
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
    /// The repaired html.
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
        string[] allowedTags = YafContext.Current.BoardSettings.AcceptedHTML.Split(',');

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

    /// <summary>
    /// Surrounds a word list with prefix/postfix. Case insensitive.
    /// </summary>
    /// <param name="message">
    /// </param>
    /// <param name="wordList">
    /// </param>
    /// <param name="prefix">
    /// </param>
    /// <param name="postfix">
    /// </param>
    /// <returns>
    /// The surround word list.
    /// </returns>
    public static string SurroundWordList(string message, IList<string> wordList, string prefix, string postfix)
    {
      const RegexOptions regexOptions = RegexOptions.IgnoreCase;

      foreach (string word in wordList)
      {
        if (word.Length > 3)
        {
          var regEx = new Regex(String.Format("({0})", word.ToLower().ToRegExString()), regexOptions);

          var matches = regEx.Matches(message).Cast<Match>().ToList().OrderByDescending(x => x.Index);

          foreach (Match m in matches)
          {
            message = message.Insert(m.Index + m.Length, postfix);
            message = message.Insert(m.Index, prefix);
          }
        }
      }

      return message;
    }

    /// <summary>
    /// Function to check a max word length, used i.e. in topic names.
    /// </summary>
    /// <param name="message">
    /// The raw string to format
    /// </param>
    /// <returns>
    /// The formatted string
    /// </returns>
    public static bool WordLengthChecker(string message)
    {
      // Set it to limit length
      int maxWordLength = YafContext.Current.BoardSettings.MaxWordLength;

      if (message.Length > 0)
      {
        string[] messageArray = message.Split(' ');
        for (uint i = 0; i < messageArray.Length; i++)
        {
          if (!string.IsNullOrEmpty(messageArray[i].Trim()))
          {
            if (messageArray[i].Length > maxWordLength)
            {
              return false;
            }
          }
        }
      }

      return true;
    }

    #endregion

    /// <summary>
    /// The message cleaned class (internal)
    /// </summary>
    [Serializable]
    public class MessageCleaned
    {
      #region Constructors and Destructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MessageCleaned"/> class.
      /// </summary>
      public MessageCleaned()
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="MessageCleaned"/> class.
      /// </summary>
      /// <param name="messageTruncated">
      /// The message truncated.
      /// </param>
      /// <param name="messageKeywords">
      /// The message keywords.
      /// </param>
      public MessageCleaned(string messageTruncated, List<string> messageKeywords)
      {
        this.MessageTruncated = messageTruncated;
        this.MessageKeywords = messageKeywords;
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets MessageKeywords.
      /// </summary>
      public List<string> MessageKeywords { get; set; }

      /// <summary>
      /// Gets or sets MessageTruncated.
      /// </summary>
      public string MessageTruncated { get; set; }

      #endregion
    }
  }
}