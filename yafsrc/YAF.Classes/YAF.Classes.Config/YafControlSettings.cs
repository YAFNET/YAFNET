/* Yet Another Forum.net
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
using System;
using YAF.Types;

namespace YAF.Classes
{
  using YAF.Classes.Pattern;

  /// <summary>
  /// Class provides glue/settings transfer between YAF forum control and base classes
  /// </summary>
  public class YafControlSettings
  {
    /* Ederon : 6/16/2007 - conventions */

    /// <summary>
    /// The _board id.
    /// </summary>
    private int _boardId;

    /// <summary>
    /// The _category id.
    /// </summary>
    private int _categoryId;

    /// <summary>
    /// The _locked forum.
    /// </summary>
    private int _lockedForum = 0;

    /// <summary>
    /// The _popup.
    /// </summary>
    private bool _popup = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="YafControlSettings"/> class.
    /// </summary>
    public YafControlSettings()
    {
      if (!int.TryParse(Config.CategoryID, out this._categoryId))
      {
        this._categoryId = 0; // Ederon : 6/16/2007 - changed from 1 to 0
      }
      if (!int.TryParse(Config.BoardID, out this._boardId))
      {
        this._boardId = 1;
      }
    }

    /// <summary>
    /// Gets Current.
    /// </summary>
    public static YafControlSettings Current
    {
      get
      {
        return PageSingleton<YafControlSettings>.Instance;
      }
    }

    /// <summary>
    /// Gets or sets BoardID.
    /// </summary>
    public int BoardID
    {
      get
      {
        return this._boardId;
      }

      set
      {
        this._boardId = value;
      }
    }

    /// <summary>
    /// Gets or sets CategoryID.
    /// </summary>
    public int CategoryID
    {
      get
      {
        return this._categoryId;
      }

      set
      {
        this._categoryId = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Popup.
    /// </summary>
    public bool Popup
    {
      get
      {
        return this._popup;
      }

      set
      {
        this._popup = value;
      }
    }

    /// <summary>
    /// Gets or sets LockedForum.
    /// </summary>
    public int LockedForum
    {
      get
      {
        return this._lockedForum;
      }

      set
      {
        this._lockedForum = value;
      }
    }
  }
}