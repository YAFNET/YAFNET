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
namespace YAF.Core
{
  #region Using

  using System.Text;
  using System.Text.RegularExpressions;

  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The i replace blocks extensions.
  /// </summary>
  public static class IReplaceBlocksExtensions
  {
    #region Constants and Fields

    /// <summary>
    /// The _options.
    /// </summary>
    private static readonly RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

    /// <summary>
    /// The _reg ex html.
    /// </summary>
    private static readonly Regex _regExHtml =
      new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", _options | RegexOptions.Compiled);

    #endregion

    #region Public Methods

    /// <summary>
    /// Pull replacement blocks from the text
    /// </summary>
    /// <param name="replaceBlocks">
    /// The replace Blocks.
    /// </param>
    /// <param name="strText">
    /// The str Text.
    /// </param>
    public static void ReplaceHtmlFromText(this IReplaceBlocks replaceBlocks, ref string strText)
    {
      var sb = new StringBuilder(strText);

      ReplaceHtmlFromText(replaceBlocks, ref sb);

      strText = sb.ToString();
    }

    /// <summary>
    /// The get replacements from text.
    /// </summary>
    /// <param name="replaceBlocks">
    /// The replace Blocks.
    /// </param>
    /// <param name="sb">
    /// The sb.
    /// </param>
    public static void ReplaceHtmlFromText(this IReplaceBlocks replaceBlocks, ref StringBuilder sb)
    {
      Match m = _regExHtml.Match(sb.ToString());

      while (m.Success)
      {
        // add it to the list...
        int index = replaceBlocks.Add(m.Groups[0].Value);

        // replacement lookup code
        string replace = replaceBlocks.Get(index);

        // remove the replaced item...
        sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

        // insert the replaced value back in...
        sb.Insert(m.Groups[0].Index, replace);

        // text = text.Substring( 0, m.Groups [0].Index ) + replace + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
        m = _regExHtml.Match(sb.ToString());
      }
    }

    #endregion
  }
}