/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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