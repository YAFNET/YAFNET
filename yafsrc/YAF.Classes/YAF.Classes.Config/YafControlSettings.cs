/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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
