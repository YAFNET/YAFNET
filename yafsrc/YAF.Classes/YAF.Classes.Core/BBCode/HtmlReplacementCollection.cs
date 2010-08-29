/* Yet Another Forum.net
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
namespace YAF.Classes.Core.BBCode
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Text.RegularExpressions;

  /// <summary>
  /// Handles the collection of replacement tags and can also pull the HTML out of the text making a new replacement tag
  /// </summary>
  public class HtmlReplacementCollection
  {
    #region Constants and Fields

    /// <summary>
    ///   The _replacement dictionary.
    /// </summary>
    private readonly Dictionary<int, HtmlReplacementBlock> _replacementDictionary;

    /// <summary>
    ///   The _rgx html.
    /// </summary>
    private readonly Regex _rgxHtml;

    /// <summary>
    ///   The _current index.
    /// </summary>
    private int _currentIndex;

    /// <summary>
    ///   The _options.
    /// </summary>
    private RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

    /// <summary>
    ///   The _random instance.
    /// </summary>
    private int _randomInstance;

    /// <summary>
    ///   The _replace format.
    /// </summary>
    private string _replaceFormat = "÷ñÒ{1}êÖ{0}õæ÷";

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "HtmlReplacementCollection" /> class.
    /// </summary>
    public HtmlReplacementCollection()
    {
      this._replacementDictionary = new Dictionary<int, HtmlReplacementBlock>();
      this._rgxHtml = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", this._options);

      this.RandomizeInstance();
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets ReplacementDictionary.
    /// </summary>
    public Dictionary<int, HtmlReplacementBlock> ReplacementDictionary
    {
      get
      {
        return this._replacementDictionary;
      }
    }

    #endregion

    #region Public Methods

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
        int index = this.AddReplacement(new HtmlReplacementBlock(m.Groups[0].Value));

        // replacement lookup code
        string replace = this.GetReplaceValue(index);

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
        int index = this.AddReplacement(new HtmlReplacementBlock(m.Groups[0].Value));

        // replacement lookup code
        string replace = this.GetReplaceValue(index);

        // remove the replaced item...
        sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

        // insert the replaced value back in...
        sb.Insert(m.Groups[0].Index, replace);

        // text = text.Substring( 0, m.Groups [0].Index ) + replace + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
        m = this._rgxHtml.Match(sb.ToString());
      }
    }

    /// <summary>
    /// get a random number for the instance
    ///   so it's harder to guess the replacement format
    /// </summary>
    public void RandomizeInstance()
    {
      var rand = new Random();
      this._randomInstance = rand.Next();
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
        sb.Replace(this.GetReplaceValue(index), this._replacementDictionary[index].Tag);
      }

      text = sb.ToString();
    }

    #endregion
  }
}