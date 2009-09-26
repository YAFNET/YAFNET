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
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Modules
{
	[YafModule( "Mobile Theme Module", "Tiny Gecko", 1 )]
	public class MobileThemeModule : IBaseModule
	{
		private object _forumControlObj;
		public object ForumControlObj
		{
			get
			{
				return _forumControlObj;
			}
			set
			{
				_forumControlObj = value;
			}
		}

		public void Init()
		{
			YafContext.Current.AfterInit += new EventHandler<EventArgs>( Current_AfterInit );
		}

		void Current_AfterInit( object sender, EventArgs e )
		{
			// see if this is a mobile device...
			if ( HttpContext.Current != null && UserAgentHelper.IsMobileDevice( HttpContext.Current.Request.UserAgent ) )
			{
				// change the theme to mobile...
				//TODO: Add Mobile Theme Code
			}
		}

		#region IDisposable Members

		public void Dispose()
		{

		}

		#endregion
	}
}
