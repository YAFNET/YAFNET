/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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
    ///   The current index.
    /// </summary>
    private int currentIndex;

    /// <summary>
    ///   The random instance.
    /// </summary>
    private int randomInstance;

    /// <summary>
    ///  REPLACEMENT UNIQUE VALUE -- USED TO CREATE A UNIQUE VALUE TO REPLACE -- IT IS NOT SUPPOSED TO BE HUMAN READABLE.
    /// </summary>
    private string replaceFormat = "÷ñÒ{1}êÖ{0}õæ÷";

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ReplaceBlocksCollection" /> class.
    /// </summary>
    public ReplaceBlocksCollection()
    {
      this.ReplacementDictionary = new Dictionary<int, string>();
      this.RandomizeInstance();
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets ReplacementDictionary.
    /// </summary>
    public Dictionary<int, string> ReplacementDictionary { get; }

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
      this.ReplacementDictionary.Add(this.currentIndex, newItem);
      return this.currentIndex++;
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
      return string.Format(this.replaceFormat, index, this.randomInstance);
    }

    /// <summary>
    /// get a random number for the instance
    ///   so it's harder to guess the replacement format
    /// </summary>
    public void RandomizeInstance()
    {
      var rand = new Random();
      this.randomInstance = rand.Next();
    }

    /// <summary>
    /// Reconstructs the text from the collection elements...
    /// </summary>
    /// <param name="text">
    /// </param>
    public void Reconstruct(ref string text)
    {
        var sb = new StringBuilder(text);

        this.ReplacementDictionary.Keys.ForEach(
            index => sb.Replace(this.Get(index), this.ReplacementDictionary[index]));

        text = sb.ToString();
    }

    #endregion
  }
}