/* Yet Another Forum.net
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
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Web;
  using YAF.Classes.Utils;

  /// <summary>
  /// Replace Rules Interface
  /// </summary>
  public interface IReplaceRules
  {
    /// <summary>
    /// Gets a value indicating whether any rules have been added.
    /// </summary>
    bool HasRules
    {
      get;
    }

    /// <summary>
    /// The add rule.
    /// </summary>
    /// <param name="newRule">
    /// The new rule.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    void AddRule(BaseReplaceRule newRule);

    /// <summary>
    /// Process text using the rules.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    void Process(ref string text);
  }

  /// <summary>
  /// Base Replace Rules Interface
  /// </summary>
  public interface IBaseReplaceRule
  {
    /// <summary>
    /// Gets RuleDescription.
    /// </summary>
    string RuleDescription
    {
      get;
    }

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    void Replace(ref string text, ref HtmlReplacementCollection replacement);
  }

  /// <summary>
  /// Provides a way to handle layers of replacements rules
  /// </summary>
  public class ReplaceRules : ICloneable, IReplaceRules
  {
    /// <summary>
    /// The _need sort.
    /// </summary>
    private bool _needSort = false;

    /// <summary>
    /// The _rules list.
    /// </summary>
    private List<IBaseReplaceRule> _rulesList;

    /// <summary>
    /// The _rules lock.
    /// </summary>
    private object _rulesLock = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReplaceRules"/> class.
    /// </summary>
    public ReplaceRules()
    {
      this._rulesList = new List<IBaseReplaceRule>();
    }

    #region ICloneable Members

    /// <summary>
    /// This clone method is a Deep Clone -- including all data.
    /// </summary>
    /// <returns>
    /// The clone.
    /// </returns>
    public object Clone()
    {
      var copyReplaceRules = new ReplaceRules();

      // move the rules over...
      var ruleArray = new IBaseReplaceRule[this._rulesList.Count];
      this._rulesList.CopyTo(ruleArray);
      copyReplaceRules._rulesList.InsertRange(0, ruleArray);
      copyReplaceRules._needSort = this._needSort;

      return copyReplaceRules;
    }

    #endregion

    #region IReplaceRules Members

    /// <summary>
    /// Gets a value indicating whether any rules have been added.
    /// </summary>
    public bool HasRules
    {
      get
      {
        lock (this._rulesLock)
        {
          return this._rulesList.Count > 0;
        }
      }
    }

    /// <summary>
    /// The add rule.
    /// </summary>
    /// <param name="newRule">
    /// The new rule.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    public void AddRule(BaseReplaceRule newRule)
    {
      if (newRule == null)
      {
        throw new ArgumentNullException("newRule");
      }

      lock (this._rulesLock)
      {
        this._rulesList.Add(newRule);
        this._needSort = true;
      }
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public void Process(ref string text)
    {
      if (String.IsNullOrEmpty(text))
      {
        return;
      }

      // sort the rules according to rank...
      if (this._needSort)
      {
        lock (this._rulesLock)
        {
          this._rulesList.Sort();
          this._needSort = false;
        }
      }

      // make the replacementCollection for this instance...
      var mainCollection = new HtmlReplacementCollection();

      // get as local list...
      var localRulesList = new List<IBaseReplaceRule>();

      lock (this._rulesLock)
      {
        localRulesList.AddRange(this._rulesList);
      }

      // apply all rules...
      foreach (BaseReplaceRule rule in localRulesList)
      {
        rule.Replace(ref text, ref mainCollection);
      }

      // reconstruct the html
      mainCollection.Reconstruct(ref text);
    }

    #endregion
  }

  /// <summary>
  /// Base class for all replacement rules.
  /// Provides compare functionality based on the rule rank.
  /// Override replace to handle replacement differently.
  /// </summary>
  public abstract class BaseReplaceRule : IComparable, IBaseReplaceRule
  {
    /// <summary>
    /// The rule rank.
    /// </summary>
    public int RuleRank = 50;

    #region IBaseReplaceRule Members

    /// <summary>
    /// Gets RuleDescription.
    /// </summary>
    public virtual string RuleDescription
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public virtual void Replace(ref string text, ref HtmlReplacementCollection replacement)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IComparable Members

    /// <summary>
    /// The compare to.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <returns>
    /// The compare to.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// </exception>
    public int CompareTo(object obj)
    {
      if (obj is BaseReplaceRule)
      {
        var otherRule = obj as BaseReplaceRule;

        if (this.RuleRank > otherRule.RuleRank)
        {
          return 1;
        }
        else if (this.RuleRank < otherRule.RuleRank)
        {
          return -1;
        }

        return 0;
      }
      else
      {
        throw new ArgumentException("Object is not of type BaseReplaceRule.");
      }
    }

    #endregion
  }

  /// <summary>
  /// Not regular expression, just a simple replace
  /// </summary>
  public class SimpleReplaceRule : BaseReplaceRule
  {
    /// <summary>
    /// The _find.
    /// </summary>
    private string _find;

    /// <summary>
    /// The _replace.
    /// </summary>
    private string _replace;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleReplaceRule"/> class.
    /// </summary>
    /// <param name="find">
    /// The find.
    /// </param>
    /// <param name="replace">
    /// The replace.
    /// </param>
    public SimpleReplaceRule(string find, string replace)
    {
      this._find = find;
      this._replace = replace;

      // lower the rank by default
      this.RuleRank = 100;
    }

    /// <summary>
    /// Gets RuleDescription.
    /// </summary>
    public override string RuleDescription
    {
      get
      {
        return String.Format("Find = \"{0}\"", this._find);
      }
    }

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    public override void Replace(ref string text, ref HtmlReplacementCollection replacement)
    {
      int index = -1;

      do
      {
        index = text.IndexOf(this._find);
        if (index >= 0)
        {
          // replace it...
          int replaceIndex = replacement.AddReplacement(new HtmlReplacementBlock(this._replace));
          text = text.Substring(0, index) + replacement.GetReplaceValue(replaceIndex) + text.Substring(index + this._find.Length);
        }
      }
 while (index >= 0);
    }
  }

  /// <summary>
  /// For basic regex with no variables
  /// </summary>
  public class SimpleRegexReplaceRule : BaseReplaceRule
  {
    /// <summary>
    /// The _reg ex replace.
    /// </summary>
    protected string _regExReplace;

    /// <summary>
    /// The _reg ex search.
    /// </summary>
    protected Regex _regExSearch;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleRegexReplaceRule"/> class.
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
    public SimpleRegexReplaceRule(string regExSearch, string regExReplace, RegexOptions regExOptions)
    {
      this._regExSearch = new Regex(regExSearch, regExOptions);
      this._regExReplace = regExReplace;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    public SimpleRegexReplaceRule(Regex regExSearch, string regExReplace)
    {
      this._regExSearch = regExSearch;
      this._regExReplace = regExReplace;
    }

    /// <summary>
    /// Gets RuleDescription.
    /// </summary>
    public override string RuleDescription
    {
      get
      {
        return String.Format("RegExSearch = \"{0}\"", this._regExSearch);
      }
    }

    /// <summary>
    /// The get inner value.
    /// </summary>
    /// <param name="innerValue">
    /// The inner value.
    /// </param>
    /// <returns>
    /// The get inner value.
    /// </returns>
    protected virtual string GetInnerValue(string innerValue)
    {
      return innerValue;
    }

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    public override void Replace(ref string text, ref HtmlReplacementCollection replacement)
    {
      var sb = new StringBuilder(text);

      Match m = this._regExSearch.Match(text);
      while (m.Success)
      {
        string tStr = this._regExReplace.Replace("${inner}", GetInnerValue(m.Groups["inner"].Value));

        // pulls the htmls into the replacement collection before it's inserted back into the main text
        replacement.GetReplacementsFromText(ref tStr);

        // remove old bbcode...
        sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

        // insert replaced value(s)
        sb.Insert(m.Groups[0].Index, tStr);

        // text = text.Substring( 0, m.Groups [0].Index ) + tStr + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
        m = this._regExSearch.Match(sb.ToString());
      }

      text = sb.ToString();
    }
  }

  /// <summary>
  /// For basic regex with no variables
  /// </summary>
  public class SingleRegexReplaceRule : SimpleRegexReplaceRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SingleRegexReplaceRule"/> class.
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
    public SingleRegexReplaceRule(string regExSearch, string regExReplace, RegexOptions regExOptions)
      : base(regExSearch, regExReplace, regExOptions)
    {
    }

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    public override void Replace(ref string text, ref HtmlReplacementCollection replacement)
    {
      var sb = new StringBuilder(text);

      Match m = this._regExSearch.Match(text);
      while (m.Success)
      {
        // just replaces with no "inner"
        int replaceIndex = replacement.AddReplacement(new HtmlReplacementBlock(this._regExReplace));

        // remove old bbcode...
        sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

        // insert replaced value(s)
        sb.Insert(m.Groups[0].Index, replacement.GetReplaceValue(replaceIndex));

        // text = text.Substring( 0, m.Groups [0].Index ) + replacement.GetReplaceValue( replaceIndex ) + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
        m = this._regExSearch.Match(sb.ToString());
      }

      text = sb.ToString();
    }
  }

  /// <summary>
  /// For complex regex with variable/default and truncate support
  /// </summary>
  public class VariableRegexReplaceRule : SimpleRegexReplaceRule
  {
    /// <summary>
    /// The _truncate length.
    /// </summary>
    protected int _truncateLength = 0;

    /// <summary>
    /// The _variable defaults.
    /// </summary>
    protected string[] _variableDefaults = null;

    /// <summary>
    /// The _variables.
    /// </summary>
    protected string[] _variables = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="varDefaults">
    /// The var defaults.
    /// </param>
    /// <param name="truncateLength">
    /// The truncate length.
    /// </param>
    public VariableRegexReplaceRule(Regex regExSearch, string regExReplace, string[] variables, string[] varDefaults, int truncateLength)
      : base(regExSearch, regExReplace)
    {
      this._variables = variables;
      this._variableDefaults = varDefaults;
      this._truncateLength = truncateLength;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="varDefaults">
    /// The var defaults.
    /// </param>
    public VariableRegexReplaceRule(Regex regExSearch, string regExReplace, string[] variables, string[] varDefaults)
      : this(regExSearch, regExReplace, variables, varDefaults, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    public VariableRegexReplaceRule(Regex regExSearch, string regExReplace, string[] variables)
      : this(regExSearch, regExReplace, variables, null, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
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
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="varDefaults">
    /// The var defaults.
    /// </param>
    /// <param name="truncateLength">
    /// The truncate length.
    /// </param>
    public VariableRegexReplaceRule(
      string regExSearch, string regExReplace, RegexOptions regExOptions, string[] variables, string[] varDefaults, int truncateLength)
      : base(regExSearch, regExReplace, regExOptions)
    {
      this._variables = variables;
      this._variableDefaults = varDefaults;
      this._truncateLength = truncateLength;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
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
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="varDefaults">
    /// The var defaults.
    /// </param>
    public VariableRegexReplaceRule(string regExSearch, string regExReplace, RegexOptions regExOptions, string[] variables, string[] varDefaults)
      : this(regExSearch, regExReplace, regExOptions, variables, varDefaults, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
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
    /// <param name="variables">
    /// The variables.
    /// </param>
    public VariableRegexReplaceRule(string regExSearch, string regExReplace, RegexOptions regExOptions, string[] variables)
      : this(regExSearch, regExReplace, regExOptions, variables, null, 0)
    {
    }

    /// <summary>
    /// Override to change default variable handling...
    /// </summary>
    /// <param name="variableName">
    /// </param>
    /// <param name="variableValue">
    /// </param>
    /// <param name="handlingValue">
    /// variable transfermation desired
    /// </param>
    /// <returns>
    /// The manage variable value.
    /// </returns>
    protected virtual string ManageVariableValue(string variableName, string variableValue, string handlingValue)
    {
      if (!String.IsNullOrEmpty(handlingValue))
      {
        switch (handlingValue.ToLower())
        {
          case "decode":
            variableValue = HttpUtility.HtmlDecode(variableValue);
            break;
          case "encode":
            variableValue = HttpUtility.HtmlEncode(variableValue);
            break;
        }
      }

      return variableValue;
    }

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    public override void Replace(ref string text, ref HtmlReplacementCollection replacement)
    {
      var sb = new StringBuilder(text);

      Match m = this._regExSearch.Match(text);
      while (m.Success)
      {
        var innerReplace = new StringBuilder(this._regExReplace);
        int i = 0;

        foreach (string tVar in this._variables)
        {
          string varName = tVar;
          string handlingValue = String.Empty;

          if (varName.Contains(":"))
          {
            // has handling section
            string[] tmpSplit = varName.Split(':');
            varName = tmpSplit[0];
            handlingValue = tmpSplit[1];
          }

          string tValue = m.Groups[varName].Value;

          if (this._variableDefaults != null && tValue.Length == 0)
          {
            // use default instead
            tValue = this._variableDefaults[i];
          }

          innerReplace.Replace("${" + varName + "}", ManageVariableValue(varName, tValue, handlingValue));
          i++;
        }

        innerReplace.Replace("${inner}", m.Groups["inner"].Value);

        if (this._truncateLength > 0)
        {
          // special handling to truncate urls
          innerReplace.Replace("${innertrunc}", StringHelper.TruncateMiddle(m.Groups["inner"].Value, this._truncateLength));
        }

        // pulls the htmls into the replacement collection before it's inserted back into the main text
        replacement.GetReplacementsFromText(ref innerReplace);

        // remove old bbcode...
        sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

        // insert replaced value(s)
        sb.Insert(m.Groups[0].Index, innerReplace.ToString());

        // text = text.Substring( 0, m.Groups [0].Index ) + tStr + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
        m = this._regExSearch.Match(sb.ToString());
      }

      text = sb.ToString();
    }
  }

  /// <summary>
  /// For the font size with replace
  /// </summary>
  public class FontSizeRegexReplaceRule : VariableRegexReplaceRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FontSizeRegexReplaceRule"/> class.
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
    public FontSizeRegexReplaceRule(string regExSearch, string regExReplace, RegexOptions regExOptions)
      : base(regExSearch, 
             regExReplace, 
             regExOptions, 
             new[]
               {
                 "size"
               }, 
             new[]
               {
                 "5"
               })
    {
      this.RuleRank = 25;
    }

    /// <summary>
    /// The get font size.
    /// </summary>
    /// <param name="inputStr">
    /// The input str.
    /// </param>
    /// <returns>
    /// The get font size.
    /// </returns>
    private string GetFontSize(string inputStr)
    {
      int[] sizes = {
                      50, 70, 80, 90, 100, 120, 140, 160, 180
                    };
      int size = 5;

      // try to parse the input string...
      int.TryParse(inputStr, out size);

      if (size < 1)
      {
        size = 1;
      }

      if (size > sizes.Length)
      {
        size = 5;
      }

      return sizes[size - 1].ToString() + "%";
    }

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
      if (variableName == "size")
      {
        return GetFontSize(variableValue);
      }

      return variableValue;
    }
  }

  /// <summary>
  /// For the font size with replace
  /// </summary>
  public class PostTopicRegexReplaceRule : VariableRegexReplaceRule
  {
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
      : base(regExSearch, 
             regExReplace, 
             regExOptions, 
             new[]
               {
                 "post", "topic"
               })
    {
      this.RuleRank = 200;
    }

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
  }

  /// <summary>
  /// Simple code block regular express replace
  /// </summary>
  public class CodeRegexReplaceRule : SimpleRegexReplaceRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    public CodeRegexReplaceRule(Regex regExSearch, string regExReplace)
      : base(regExSearch, regExReplace)
    {
      // default high rank...
      this.RuleRank = 2;
    }

    /// <summary>
    /// This just overrides how the inner value is handled
    /// </summary>
    /// <param name="innerValue">
    /// </param>
    /// <returns>
    /// The get inner value.
    /// </returns>
    protected override string GetInnerValue(string innerValue)
    {
      innerValue = innerValue.Replace("  ", "&nbsp; ");
      innerValue = innerValue.Replace("  ", " &nbsp;");
      innerValue = innerValue.Replace("\t", "&nbsp; &nbsp;&nbsp;");
      innerValue = innerValue.Replace("[", "&#91;");
      innerValue = innerValue.Replace("]", "&#93;");
      innerValue = innerValue.Replace("<", "&lt;");
      innerValue = innerValue.Replace(">", "&gt;");
      innerValue = innerValue.Replace("\r\n", "<br/>");
      return innerValue;
    }

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    public override void Replace(ref string text, ref HtmlReplacementCollection replacement)
    {
      Match m = this._regExSearch.Match(text);
      while (m.Success)
      {
        string tStr = this._regExReplace.Replace("${inner}", GetInnerValue(m.Groups["inner"].Value));

        int replaceIndex = replacement.AddReplacement(new HtmlReplacementBlock(tStr));
        text = text.Substring(0, m.Groups[0].Index) + replacement.GetReplaceValue(replaceIndex) + text.Substring(m.Groups[0].Index + m.Groups[0].Length);
        m = this._regExSearch.Match(text);
      }
    }
  }

  /// <summary>
  /// Syntax Highlighted code block regular express replace
  /// </summary>
  public class SyntaxHighlightedCodeRegexReplaceRule : SimpleRegexReplaceRule
  {
    /// <summary>
    /// The _syntax highlighter.
    /// </summary>
    private HighLighter _syntaxHighlighter = new HighLighter();

    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxHighlightedCodeRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    public SyntaxHighlightedCodeRegexReplaceRule(Regex regExSearch, string regExReplace)
      : base(regExSearch, regExReplace)
    {
      this._syntaxHighlighter.ReplaceEnter = true;
      this.RuleRank = 1;
    }

    /// <summary>
    /// This just overrides how the inner value is handled
    /// </summary>
    /// <param name="innerValue">
    /// </param>
    /// <returns>
    /// The get inner value.
    /// </returns>
    protected override string GetInnerValue(string innerValue)
    {
      return innerValue;
    }

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    public override void Replace(ref string text, ref HtmlReplacementCollection replacement)
    {
      Match m = this._regExSearch.Match(text);
      while (m.Success)
      {
        string inner = this._syntaxHighlighter.ColorText(
          GetInnerValue(m.Groups["inner"].Value), HttpContext.Current.Server.MapPath(YafForumInfo.ForumFileRoot + "defs/"), m.Groups["language"].Value);
        string tStr = this._regExReplace.Replace("${inner}", inner);

        // pulls the htmls into the replacement collection before it's inserted back into the main text
        int replaceIndex = replacement.AddReplacement(new HtmlReplacementBlock(tStr));

        text = text.Substring(0, m.Groups[0].Index) + replacement.GetReplaceValue(replaceIndex) + text.Substring(m.Groups[0].Index + m.Groups[0].Length);
        m = this._regExSearch.Match(text);
      }
    }
  }

  /// <summary>
  /// Handles the collection of replacement tags and can also pull the HTML out of the text making a new replacement tag
  /// </summary>
  public class HtmlReplacementCollection
  {
    /// <summary>
    /// The _current index.
    /// </summary>
    private int _currentIndex = 0;

    /// <summary>
    /// The _options.
    /// </summary>
    private RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

    /// <summary>
    /// The _random instance.
    /// </summary>
    private int _randomInstance;

    /// <summary>
    /// The _replace format.
    /// </summary>
    private string _replaceFormat = "÷ñÒ{1}êÖ{0}õæ÷";

    /// <summary>
    /// The _replacement dictionary.
    /// </summary>
    private Dictionary<int, HtmlReplacementBlock> _replacementDictionary;

    /// <summary>
    /// The _rgx html.
    /// </summary>
    private Regex _rgxHtml;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlReplacementCollection"/> class.
    /// </summary>
    public HtmlReplacementCollection()
    {
      this._replacementDictionary = new Dictionary<int, HtmlReplacementBlock>();
      this._rgxHtml = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", this._options);

      RandomizeInstance();
    }

    /// <summary>
    /// Gets ReplacementDictionary.
    /// </summary>
    public Dictionary<int, HtmlReplacementBlock> ReplacementDictionary
    {
      get
      {
        return this._replacementDictionary;
      }
    }

    /// <summary>
    /// get a random number for the instance
    /// so it's harder to guess the replacement format
    /// </summary>
    public void RandomizeInstance()
    {
      var rand = new Random();
      this._randomInstance = rand.Next();
    }

    /// <summary>
    /// The add replacement.
    /// </summary>
    /// <param name="newItem">
    /// The new item.
    /// </param>
    /// <returns>
    /// The add replacement.
    /// </returns>
    public int AddReplacement(HtmlReplacementBlock newItem)
    {
      this._replacementDictionary.Add(this._currentIndex, newItem);
      return this._currentIndex++;
    }

    /// <summary>
    /// The get replace value.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// The get replace value.
    /// </returns>
    public string GetReplaceValue(int index)
    {
      return String.Format(this._replaceFormat, index, this._randomInstance);
    }

    /// <summary>
    /// Reconstructs the text from the collection elements...
    /// </summary>
    /// <param name="text">
    /// </param>
    public void Reconstruct(ref string text)
    {
      var sb = new StringBuilder(text);

      foreach (int index in this._replacementDictionary.Keys)
      {
        sb.Replace(GetReplaceValue(index), this._replacementDictionary[index].Tag);
      }

      text = sb.ToString();
    }

    /// <summary>
    /// Pull replacement blocks from the text
    /// </summary>
    /// <param name="strText">
    /// The str Text.
    /// </param>
    public void GetReplacementsFromText(ref string strText)
    {
      var sb = new StringBuilder(strText);

      Match m = this._rgxHtml.Match(strText);
      while (m.Success)
      {
        // add it to the list...
        int index = AddReplacement(new HtmlReplacementBlock(m.Groups[0].Value));

        // replacement lookup code
        string replace = GetReplaceValue(index);

        // remove the replaced item...
        sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

        // insert the replaced value back in...
        sb.Insert(m.Groups[0].Index, replace);

        // text = text.Substring( 0, m.Groups [0].Index ) + replace + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
        m = this._rgxHtml.Match(sb.ToString());
      }

      strText = sb.ToString();
    }

    /// <summary>
    /// The get replacements from text.
    /// </summary>
    /// <param name="sb">
    /// The sb.
    /// </param>
    public void GetReplacementsFromText(ref StringBuilder sb)
    {
      Match m = this._rgxHtml.Match(sb.ToString());
      while (m.Success)
      {
        // add it to the list...
        int index = AddReplacement(new HtmlReplacementBlock(m.Groups[0].Value));

        // replacement lookup code
        string replace = GetReplaceValue(index);

        // remove the replaced item...
        sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

        // insert the replaced value back in...
        sb.Insert(m.Groups[0].Index, replace);

        // text = text.Substring( 0, m.Groups [0].Index ) + replace + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
        m = this._rgxHtml.Match(sb.ToString());
      }
    }
  }

  /// <summary>
  /// Simple class that doesn't do anything except store a tag.
  /// Why a class? Because I may want to add to it someday...
  /// </summary>
  public class HtmlReplacementBlock
  {
    /// <summary>
    /// The _tag.
    /// </summary>
    private string _tag;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlReplacementBlock"/> class.
    /// </summary>
    /// <param name="tag">
    /// The tag.
    /// </param>
    public HtmlReplacementBlock(string tag)
    {
      this._tag = tag;
    }

    /// <summary>
    /// Gets Tag.
    /// </summary>
    public string Tag
    {
      get
      {
        return this._tag;
      }
    }
  }
}