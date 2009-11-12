using System;
using YAF.Classes.Pattern;

namespace YAF.Classes
{
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