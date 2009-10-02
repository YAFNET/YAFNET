/* Yet Another Forum.NET
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
using System.Text;
using System.Web;
using System.Web.UI;

namespace YAF.Classes.Pattern
{
	// Singleton factory implementation
	public static class PageSingleton<T> where T : class, new()
	{
		// static constructor, 
		//runtime ensures thread safety
		static PageSingleton()
		{
			// create the single instance 
			//_instance = GetInstance();
		}

		static private T GetInstance()
		{
			if ( HttpContext.Current == null )
			{
				if ( _instance == null )
				{
					_instance = (T)Activator.CreateInstance( typeof( T ), true );
				}
				return _instance;
			}			

			string typeStr = typeof( T ).ToString();

			return (T)( HttpContext.Current.Items[typeStr] ?? ( HttpContext.Current.Items[typeStr] = (T)Activator.CreateInstance( typeof( T ), true ) ) );
		}

		private static T _instance = null;
		public static T Instance
		{
			private set { _instance = value; }
			get
			{
				return GetInstance();
			}
		}
	}
}