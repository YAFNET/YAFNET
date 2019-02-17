/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2019 Ingo Herbote
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
namespace YAF.Controls
{
  #region Using

    using System.Collections.Generic;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The yaf bb code control.
  /// </summary>
  public class YafBBCodeControl : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _current message flags.
    /// </summary>
    protected MessageFlags _currentMessageFlags;

    /// <summary>
    ///   The _display user id.
    /// </summary>
    protected int? _displayUserId;

    /// <summary>
    ///   The _message id.
    /// </summary>
    protected int? _messageId;

    /// <summary>
    ///   The _parameters.
    /// </summary>
    protected Dictionary<string, string> _parameters = new Dictionary<string, string>();

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets CurrentMessageFlags.
    /// </summary>
    public MessageFlags CurrentMessageFlags
    {
      get
      {
        return this._currentMessageFlags;
      }

      set
      {
        this._currentMessageFlags = value;
      }
    }

    /// <summary>
    ///   Gets or sets MessageID.
    /// </summary>
    public int? MessageID
    {
        get
        {
            return this._messageId;
        }

        set
        {
            this._messageId = value;
        }
    }

    /// <summary>
    ///   Gets or sets DisplayUserID.
    /// </summary>
    public int? DisplayUserID
    {
      get
      {
        return this._displayUserId;
      }

      set
      {
        this._displayUserId = value;
      }
    }

    /// <summary>
    ///   Gets or sets Parameters.
    /// </summary>
    public Dictionary<string, string> Parameters
    {
      get
      {
        return this._parameters;
      }

      set
      {
        this._parameters = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The localized string.
    /// </summary>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <param name="defaultStr">
    /// The default str.
    /// </param>
    /// <returns>
    /// The localized string.
    /// </returns>
    protected string LocalizedString([NotNull] string tag, [NotNull] string defaultStr)
    {
        return this.Get<ILocalization>().GetTextExists("BBCODEMODULE", tag)
                   ? this.GetText("BBCODEMODULE", tag)
                   : defaultStr;
    }

      /// <summary>
    /// The process bb code string.
    /// </summary>
    /// <param name="bbCodeString">
    /// The bb code string.
    /// </param>
    /// <returns>
    /// The process bb code string.
    /// </returns>
    protected string ProcessBBCodeString([NotNull] string bbCodeString)
    {
      return this.Get<IFormatMessage>().FormatMessage(bbCodeString, this.CurrentMessageFlags);
    }

    #endregion
  }
}