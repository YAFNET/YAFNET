/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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
namespace YAF.Core
{
    using YAF.Core.BBCode;
    using YAF.Types;
  using YAF.Types.Interfaces;

  /// <summary>
  /// The ibb code extensions.
  /// </summary>
  public static class IBBCodeExtensions
  {
    #region Public Methods

    /// <summary>
    /// For backwards compatibility
    /// </summary>
    /// <param name="bbCode">
    /// The bb Code.
    /// </param>
    /// <param name="message">
    /// the message to add smiles to.
    /// </param>
    /// <returns>
    /// The add smiles.
    /// </returns>
    public static string AddSmiles([NotNull] this IBBCode bbCode, [NotNull] string message)
    {
      var layers = new ProcessReplaceRules();
      bbCode.AddSmiles(layers);

      // apply...
      layers.Process(ref message);
      return message;
    }

    /// <summary>
    /// Creates the rules that convert <see cref="YafBBCode"/> to HTML
    /// </summary>
    /// <param name="ruleEngine">
    /// The rule Engine.
    /// </param>
    /// <param name="doFormatting">
    /// The do Formatting.
    /// </param>
    /// <param name="targetBlankOverride">
    /// The target Blank Override.
    /// </param>
    /// <param name="useNoFollow">
    /// The use No Follow.
    /// </param>
    public static void CreateBBCodeRules(this IBBCode bbcode, IProcessReplaceRules ruleEngine, bool doFormatting, bool targetBlankOverride, bool useNoFollow)
    {
      bbcode.CreateBBCodeRules(ruleEngine, doFormatting, targetBlankOverride, useNoFollow, true);
    }

    #endregion
  }
}