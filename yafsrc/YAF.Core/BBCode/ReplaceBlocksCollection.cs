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
namespace YAF.Core.BBCode
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    /// <summary>
  /// Handles the collection of replacement tags and can also pull the HTML out of the text making a new replacement tag
  /// </summary>
  public class ReplaceBlocksCollection : IReplaceBlocks
  {
    #region Constants and Fields

    /// <summary>
    ///   The _replacement dictionary.
    /// </summary>
    private readonly Dictionary<int, string> _replacementDictionary;

    /// <summary>
    ///   The _current index.
    /// </summary>
    private int _currentIndex;

    /// <summary>
    ///   The _random instance.
    /// </summary>
    private int _randomInstance;

    /// <summary>
    ///  REPLACEMENT UNIQUE VALUE -- USED TO CREATE A UNIQUE VALUE TO REPLACE -- IT IS NOT SUPPOSED TO BE HUMAN READABLE.
    /// </summary>
    private string _replaceFormat = "÷ñÒ{1}êÖ{0}õæ÷";

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ReplaceBlocksCollection" /> class.
    /// </summary>
    public ReplaceBlocksCollection()
    {
      this._replacementDictionary = new Dictionary<int, string>();
      this.RandomizeInstance();
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets ReplacementDictionary.
    /// </summary>
    public Dictionary<int, string> ReplacementDictionary
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
    public int Add(string newItem)
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
    public string Get(int index)
    {
      return this._replaceFormat.FormatWith(index, this._randomInstance);
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
        sb.Replace(this.Get(index), this._replacementDictionary[index]);
      }

      text = sb.ToString();
    }

    #endregion
  }
}