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
namespace YAF.Core.BBCode.ReplaceRules
{
    using System.Text.RegularExpressions;

    using YAF.Types.Constants;
    using YAF.Utils;

    /// <summary>
  /// For the font size with replace
  /// </summary>
  public class PostTopicRegexReplaceRule : VariableRegexReplaceRule
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PostTopicRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    /// <param name="regExOptions">
    /// The reg ex options.
    /// </param>
    public PostTopicRegexReplaceRule(string regExSearch, string regExReplace, RegexOptions regExOptions)
      : base(regExSearch, regExReplace, regExOptions, new[] { "post", "topic" })
    {
      this.RuleRank = 200;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The manage variable value.
    /// </summary>
    /// <param name="variableName">
    /// The variable name.
    /// </param>
    /// <param name="variableValue">
    /// The variable value.
    /// </param>
    /// <param name="handlingValue">
    /// The handling value.
    /// </param>
    /// <returns>
    /// The manage variable value.
    /// </returns>
    protected override string ManageVariableValue(string variableName, string variableValue, string handlingValue)
    {
      if (variableName == "post" || variableName == "topic")
      {
        int id = 0;
        if (int.TryParse(variableValue, out id))
        {
          if (variableName == "post")
          {
            return YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", id);
          }
          else if (variableName == "topic")
          {
            return YafBuildLink.GetLink(ForumPages.posts, "t={0}", id);
          }
        }
      }

      return variableValue;
    }

    #endregion
  }
}