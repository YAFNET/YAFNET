/* YetAnotherForum.NET
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using YAF.Classes;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
	public class BadWordReplaceItem
	{
		private readonly Object _activeLock = new Object();

		private string _goodWord;
		public string GoodWord
		{
			get
			{
				return _goodWord;
			}
		}

		private string _badWord;
		public string BadWord
		{
			get
			{
				return _badWord;
			}
		}

		private bool _active = true;
		public bool Active
		{
			get
			{
        bool value;

				lock (_activeLock)
        {
					value = _active;
        }

        return value;
			}
			set
			{
				lock (_activeLock)
        {
					_active = value;
        }  
			}
		}

		public BadWordReplaceItem( string goodWord, string badWord )
		{
			_goodWord = goodWord;
			_badWord = badWord;
		}
	}

	public class YafBadWordReplace
	{
		private const RegexOptions _options = RegexOptions.IgnoreCase /*| RegexOptions.Singleline | RegexOptions.Multiline*/;

		public List<BadWordReplaceItem> ReplaceItems
		{
			get
			{
				string cacheKey = YafCache.GetBoardCacheKey(Constants.Cache.ReplaceWords);

				List<BadWordReplaceItem> replaceItems = (List<BadWordReplaceItem>)YafContext.Current.Cache[cacheKey];

				if (replaceItems == null)
				{
					DataTable replaceWordsDataTable = DB.replace_words_list( YafContext.Current.PageBoardID, null );

					replaceItems = new List<BadWordReplaceItem>();

					// move to collection...
					foreach ( DataRow row in replaceWordsDataTable.Rows )
					{
						replaceItems.Add( new BadWordReplaceItem( row["goodword"].ToString(), row["badword"].ToString() ) );
					}

					YafContext.Current.Cache.Add( cacheKey, replaceItems, null, DateTime.Now.AddMinutes( 30 ), TimeSpan.Zero,
					                      System.Web.Caching.CacheItemPriority.Low, null );
				}

				return replaceItems;
			}
		}

		/// <summary>
		/// Searches through SearchText and replaces "bad words" with "good words"
		/// as defined in the database.
		/// </summary>
		/// <param name="searchText">The string to search through.</param>
		public string Replace(string searchText)
		{
			if (String.IsNullOrEmpty(searchText)) return searchText;

			string strReturn = searchText;

			foreach (BadWordReplaceItem item in ReplaceItems)
			{
				try
				{
					if ( item.Active )
						strReturn = Regex.Replace(strReturn, item.BadWord, item.GoodWord, _options);
				}
#if DEBUG
				catch (Exception e)
				{
					throw new Exception("Bad Word Regular Expression Failed: " + e.Message, e);
				}
#else
				catch (Exception x)
				{
					// disable this regular expression henceforth...
					item.Active = false;
          YAF.Classes.Data.DB.eventlog_create( null, "BadWordReplace", x, YAF.Classes.Data.EventLogTypes.Warning );
				}
#endif
			}

			return strReturn;
		}
	}
}
