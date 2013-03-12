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
namespace YAF.Types
{
  using System;
  using System.Text.RegularExpressions;

  /// <summary>
  /// The bad word replace item.
  /// </summary>
  [Serializable]
  public class BadWordReplaceItem
  {
    #region Constants and Fields

    /// <summary>
    ///   The _active lock.
    /// </summary>
    private readonly object _activeLock = new object();

    /// <summary>
    ///   The _bad word.
    /// </summary>
    private readonly string _badWord;

    /// <summary>
    ///   The _good word.
    /// </summary>
    private readonly string _goodWord;

    /// <summary>
    ///   Regular expression object associated with this replace item...
    /// </summary>
    private readonly Regex _regEx;

    /// <summary>
    ///   The _active.
    /// </summary>
    private bool _active = true;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BadWordReplaceItem"/> class.
    /// </summary>
    /// <param name="goodWord">
    /// The good word.
    /// </param>
    /// <param name="badWord">
    /// The bad word.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    public BadWordReplaceItem([NotNull] string goodWord, [NotNull] string badWord, RegexOptions options)
    {
      this.Options = options;
      this._goodWord = goodWord;
      this._badWord = badWord;

      try
      {
          this._regEx = new Regex(badWord, options);
      }
      catch (Exception)
      {
          this._regEx = null;
      }
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether Active.
    /// </summary>
    public bool Active
    {
      get
      {
        bool value;

        lock (this._activeLock)
        {
          value = this._active;
        }

        return value;
      }

      set
      {
        lock (this._activeLock)
        {
          this._active = value;
        }
      }
    }

    /// <summary>
    ///   Gets BadWord.
    /// </summary>
    public string BadWord
    {
      get
      {
        return this._badWord;
      }
    }

    /// <summary>
    ///   Gets BadWordRegEx.
    /// </summary>
    public Regex BadWordRegEx
    {
      get
      {
        return this._regEx;
      }
    }

    /// <summary>
    ///   Gets GoodWord.
    /// </summary>
    public string GoodWord
    {
      get
      {
        return this._goodWord;
      }
    }

    /// <summary>
    /// Gets or sets Options.
    /// </summary>
    public RegexOptions Options { get; protected set; }

    #endregion
  }
}