/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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