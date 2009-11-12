using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

//****************************************************
//		Source Control Author : Justin Wendlandt
//			jwendl@hotmail.com
//		Control Protected by the Creative Commons License
//		http://creativecommons.org/licenses/by-nc-sa/2.0/
//****************************************************

namespace YAF.Classes.UI
{
  /// <summary>
  /// The high lighter.
  /// </summary>
  public class HighLighter
  {
    /* Ederon : 6/16/2007 - conventions */

    // To Replace Enter with <br />
    /// <summary>
    /// The _replace enter.
    /// </summary>
    private bool _replaceEnter;

    // Default Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="HighLighter"/> class.
    /// </summary>
    public HighLighter()
    {
      this._replaceEnter = false;
    }

    /// <summary>
    /// Gets or sets a value indicating whether ReplaceEnter.
    /// </summary>
    public bool ReplaceEnter
    {
      get
      {
        return this._replaceEnter;
      }

      set
      {
        this._replaceEnter = value;
      }
    }

    /// <summary>
    /// The color text.
    /// </summary>
    /// <param name="tmpCode">
    /// The tmp code.
    /// </param>
    /// <param name="pathToDefFile">
    /// The path to def file.
    /// </param>
    /// <param name="language">
    /// The language.
    /// </param>
    /// <returns>
    /// The color text.
    /// </returns>
    /// <exception cref="ApplicationException">
    /// </exception>
    public string ColorText(string tmpCode, string pathToDefFile, string language)
    {
      language = language.ToLower();
      if (language == "c#" || language == "csharp")
      {
        language = "cs";
      }

      language = language.Replace("\"", string.Empty);

      // language = language.Replace("&#8220;", "");
      string tmpOutput = string.Empty;
      string comments = string.Empty;
      bool valid = true;

      var alKeyWords = new ArrayList();
      var alKeyTypes = new ArrayList();

      // cut it off at the pass...
      if (!File.Exists(pathToDefFile + language.ToString() + ".def"))
      {
        return tmpCode;
      }

      // Read def file.
      try
      {
        var sr = new StreamReader(pathToDefFile + language.ToString() + ".def");
        string tmpLine = string.Empty;
        string curFlag = string.Empty;
        while (sr.Peek() != -1)
        {
          tmpLine = sr.ReadLine();
          if (tmpLine != string.Empty)
          {
            if (tmpLine.Substring(0, 1) == "-")
            {
              // Ignore these lines and set the Current Flag
              if (tmpLine.ToLower().IndexOf("keywords") > 0)
              {
                curFlag = "keywords";
              }

              if (tmpLine.ToLower().IndexOf("keytypes") > 0)
              {
                curFlag = "keytypes";
              }

              if (tmpLine.ToLower().IndexOf("comments") > 0)
              {
                curFlag = "comments";
              }
            }
            else
            {
              if (curFlag == "keywords")
              {
                alKeyWords.Add(tmpLine);
              }

              if (curFlag == "keytypes")
              {
                alKeyTypes.Add(tmpLine);
              }

              if (curFlag == "comments")
              {
                comments = tmpLine;
              }
            }
          }
        }

        sr.Close();
      }
      catch (Exception ex)
      {
        string foobar = ex.ToString();
        tmpOutput = "<span class=\"errors\">There was an error opening file " + pathToDefFile + language.ToString() + ".def...</span>";
        valid = false;
        throw new ApplicationException(string.Format("There was an error opening file {0}{1}.def", pathToDefFile, language), ex);
      }

      if (valid == true)
      {
        // Replace Comments
        int lineNum = 0;
        var thisComment = new ArrayList();
        MatchCollection mColl = Regex.Matches(tmpCode, comments, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        foreach (Match m in mColl)
        {
          thisComment.Add(m.ToString());
          tmpCode = tmpCode.Replace(m.ToString(), "[ReplaceComment" + lineNum++ + "]");
        }

        // Replace Strings
        lineNum = 0;
        var thisString = new ArrayList();
        string thisMatch = "\"((\\\\\")|[^\"(\\\\\")]|)+\"";
        mColl = Regex.Matches(tmpCode, thisMatch, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        foreach (Match m in mColl)
        {
          thisString.Add(m.ToString());
          tmpCode = tmpCode.Replace(m.ToString(), "[ReplaceString" + lineNum++ + "]");
        }

        // Replace Chars
        lineNum = 0;
        var thisChar = new ArrayList();
        mColl = Regex.Matches(tmpCode, "\'.*?\'", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        foreach (Match m in mColl)
        {
          thisChar.Add(m.ToString());
          tmpCode = tmpCode.Replace(m.ToString(), "[ReplaceChar" + lineNum++ + "]");
        }

        // Replace KeyWords
        var keyWords = new string[alKeyWords.Count];
        alKeyWords.CopyTo(keyWords);
        string tmpKeyWords = "(?<replacethis>" + String.Join("|", keyWords) + ")";
        tmpCode = Regex.Replace(tmpCode, "\\b" + tmpKeyWords + "\\b(?<!//.*)", "<span class=\"keyword\">${replacethis}</span>");

        // Replace KeyTypes
        var keyTypes = new string[alKeyTypes.Count];
        alKeyTypes.CopyTo(keyTypes);
        string tmpKeyTypes = "(?<replacethis>" + String.Join("|", keyTypes) + ")";
        tmpCode = Regex.Replace(tmpCode, "\\b" + tmpKeyTypes + "\\b(?<!//.*)", "<span class=\"keytype\">${replacethis}</span>");

        lineNum = 0;
        foreach (string m in thisChar)
        {
          tmpCode = tmpCode.Replace("[ReplaceChar" + lineNum++ + "]", "<span class=\"string\">" + m.ToString() + "</span>");
        }

        lineNum = 0;
        foreach (string m in thisString)
        {
          tmpCode = tmpCode.Replace("[ReplaceString" + lineNum++ + "]", "<span class=\"string\">" + m.ToString() + "</span>");
        }

        lineNum = 0;
        foreach (string m in thisComment)
        {
          tmpCode = tmpCode.Replace("[ReplaceComment" + lineNum++ + "]", "<span class=\"comment\">" + m.ToString() + "</span>");
        }

        // Replace Numerics
        tmpCode = Regex.Replace(tmpCode, "(\\d{1,12}\\.\\d{1,12}|\\d{1,12})", "<span class=\"integer\">$1</span>");

        if (this._replaceEnter == true)
        {
          tmpCode = Regex.Replace(tmpCode, "\r", string.Empty);
          tmpCode = Regex.Replace(tmpCode, "\n", "<br />" + Environment.NewLine);
        }

        tmpCode = Regex.Replace(tmpCode, "  ", "&nbsp;&nbsp;");
        tmpCode = Regex.Replace(tmpCode, "\t", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");

        // Create Output
        tmpOutput = "<div class=\"yafcodehighlighting\">" + Environment.NewLine;
        tmpOutput += tmpCode;
        tmpOutput += "</div>" + Environment.NewLine;
      }

      return tmpOutput;
    }
  }
}