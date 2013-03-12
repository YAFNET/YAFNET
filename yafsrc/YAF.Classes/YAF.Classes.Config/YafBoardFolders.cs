/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
  /// The yaf board folders.
  /// </summary>
  public class YafBoardFolders
  {
    /// <summary>
    /// Gets Current.
    /// </summary>
    public static YafBoardFolders Current
    {
      get
      {
        return PageSingleton<YafBoardFolders>.Instance;
      }
    }

    /// <summary>
    /// Gets BoardFolder.
    /// </summary>
    public string BoardFolder
    {
      get
      {
        if (Config.MultiBoardFolders)
        {
          return String.Format(Config.BoardRoot + "{0}/", YafControlSettings.Current.BoardID);
        }
        else
        {
          return Config.BoardRoot;
        }
      }
    }

    /// <summary>
    /// Gets Uploads.
    /// </summary>
    public string Uploads
    {
      get
      {
        return String.Concat(BoardFolder, "Uploads");
      }
    }

    /// <summary>
    /// Gets Themes.
    /// </summary>
    public string Themes
    {
      get
      {
        return String.Concat(BoardFolder, "Themes");
      }
    }

    /// <summary>
    /// Gets Images.
    /// </summary>
    public string Images
    {
      get
      {
        return String.Concat(BoardFolder, "Images");
      }
    }

    /// <summary>
    /// Gets Avatars.
    /// </summary>
    public string Avatars
    {
      get
      {
        return String.Concat(BoardFolder, "Images/Avatars");
      }
    }

    /// <summary>
    /// Gets Categories.
    /// </summary>
    public string Categories
    {
      get
      {
        return String.Concat(BoardFolder, "Images/Categories");
      }
    }

    /// <summary>
    /// Gets Categories.
    /// </summary>
    public string Forums
    {
        get
        {
            return String.Concat(BoardFolder, "Images/Forums");
        }
    }

    /// <summary>
    /// Gets Emoticons.
    /// </summary>
    public string Emoticons
    {
      get
      {
        return String.Concat(BoardFolder, "Images/Emoticons");
      }
    }

    /// <summary>
    /// Gets Medals.
    /// </summary>
    public string Medals
    {
      get
      {
        return String.Concat(BoardFolder, "Images/Medals");
      }
    }

    /// <summary>
    /// Gets Ranks.
    /// </summary>
    public string Ranks
    {
      get
      {
        return String.Concat(BoardFolder, "Images/Ranks");
      }
    }
  }
}