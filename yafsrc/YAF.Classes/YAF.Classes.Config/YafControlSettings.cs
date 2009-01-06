using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

namespace YAF.Classes
{
	/// <summary>
	/// Class provides glue/settings transfer between YAF forum control and base classes
	/// </summary>
	public class YafControlSettings
	{
		/* Ederon : 6/16/2007 - conventions */
		private int _boardID;
		private int _categoryID;
		private int _lockedForum = 0;

		private static YafControlSettings _currentInstance = new YafControlSettings();
		public static YafControlSettings Current
		{
			get
			{
				Page currentPage = HttpContext.Current.Handler as Page;

				if ( currentPage == null )
				{
					// only really used for the send mail thread.
					// since it's not inside a page. An instance is
					// returned that's for the whole process.
					return _currentInstance;
				}

				// save the yafContext in the currentpage items or just retreive from the page context
				return ( currentPage.Items ["YafControlSettingsPage"] ?? ( currentPage.Items ["YafControlSettingsPage"] = new YafControlSettings() ) ) as YafControlSettings;
			}
		}

		public YafControlSettings()
		{
			if ( !int.TryParse( Config.CategoryID, out _categoryID ) )
				_categoryID = 0; // Ederon : 6/16/2007 - changed from 1 to 0

			if ( !int.TryParse( Config.BoardID, out _boardID ) )
				_boardID = 1;
		}

		public int BoardID
		{
			get
			{
				return _boardID;
			}
			set
			{
				_boardID = value;
			}
		}

		public int CategoryID
		{
			get
			{
				return _categoryID;
			}
			set
			{
				_categoryID = value;
			}
		}

		public int LockedForum
		{
			set
			{
				_lockedForum = value;
			}
			get
			{
				return _lockedForum;
			}
		}
	}
}
