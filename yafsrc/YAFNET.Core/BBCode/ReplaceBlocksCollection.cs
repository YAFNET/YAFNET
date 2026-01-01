/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Core.BBCode;

using System.Collections.Generic;
using System.Security.Cryptography;

/// <summary>
/// Handles the collection of replacement tags and can also pull the HTML out of the text making a new replacement tag
/// </summary>
public class ReplaceBlocksCollection : IReplaceBlocks
{
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
    private const string ReplaceFormat = "÷ñÒ{1}êÖ{0}õæ÷";

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ReplaceBlocksCollection" /> class.
    /// </summary>
    public ReplaceBlocksCollection()
    {
        this.ReplacementDictionary = [];
        this.RandomizeInstance();
    }

    /// <summary>
    ///   Gets ReplacementDictionary.
    /// </summary>
    public Dictionary<int, string> ReplacementDictionary { get; }

    /// <summary>
    /// The add replacement.
    /// </summary>
    /// <param name="newItem">
    /// The new item.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
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
    /// The <see cref="string"/>.
    /// </returns>
    public string Get(int index)
    {
        return string.Format(ReplaceFormat, index, this.randomInstance);
    }

    /// <summary>
    /// get a random number for the instance
    ///   so it's harder to guess the replacement format
    /// </summary>
    public void RandomizeInstance()
    {
        this.randomInstance = RandomNumberGenerator.GetInt32(1, 1000000000);
    }

    /// <summary>
    /// Reconstructs the text from the collection elements...
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public void Reconstruct(ref string text)
    {
        var sb = new StringBuilder(text);

        this.ReplacementDictionary.Keys.ForEach(
            index => sb.Replace(this.Get(index), this.ReplacementDictionary[index]));

        text = sb.ToString();
    }
}