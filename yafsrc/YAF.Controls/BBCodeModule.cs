/* Yet Another Forum.NET
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
using System.Collections.Generic;
using YAF.Classes.Data;
using YAF.Controls;

namespace YAF.Modules
{
  using YAF.Classes.Core;

  /// <summary>
  /// The yaf bb code control.
  /// </summary>
  public class YafBBCodeControl : BaseControl
  {
    /// <summary>
    /// The _current message flags.
    /// </summary>
    protected MessageFlags _currentMessageFlags = null;

    /// <summary>
    /// The _display user id.
    /// </summary>
    protected int? _displayUserId = null;

    /// <summary>
    /// The _parameters.
    /// </summary>
    protected Dictionary<string, string> _parameters = new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets Parameters.
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

    /// <summary>
    /// Gets or sets CurrentMessageFlags.
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
    /// Gets or sets DisplayUserID.
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
    /// The process bb code string.
    /// </summary>
    /// <param name="bbCodeString">
    /// The bb code string.
    /// </param>
    /// <returns>
    /// The process bb code string.
    /// </returns>
    protected string ProcessBBCodeString(string bbCodeString)
    {
      return YafFormatMessage.FormatMessage(bbCodeString, CurrentMessageFlags);
    }

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
    protected string LocalizedString(string tag, string defaultStr)
    {
      if (PageContext.Localization.GetTextExists("BBCODEMODULE", tag))
      {
        return PageContext.Localization.GetText("BBCODEMODULE", tag);
      }

      return defaultStr;
    }
  }
}