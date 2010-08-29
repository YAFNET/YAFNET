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
  #region Using

  using System;
  using System.Collections.Generic;

  #endregion

  /// <summary>
  /// Provides a way to handle layers of replacements rules
  /// </summary>
  public class ReplaceRules : ICloneable, IReplaceRules
  {
    #region Constants and Fields

    /// <summary>
    ///   The _rules list.
    /// </summary>
    private readonly List<IBaseReplaceRule> _rulesList;

    /// <summary>
    ///   The _rules lock.
    /// </summary>
    private readonly object _rulesLock = new object();

    /// <summary>
    ///   The _need sort.
    /// </summary>
    private bool _needSort;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ReplaceRules" /> class.
    /// </summary>
    public ReplaceRules()
    {
      this._rulesList = new List<IBaseReplaceRule>();
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets a value indicating whether any rules have been added.
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

    #endregion

    #region Implemented Interfaces

    #region ICloneable

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

    #region IReplaceRules

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

    #endregion
  }
}