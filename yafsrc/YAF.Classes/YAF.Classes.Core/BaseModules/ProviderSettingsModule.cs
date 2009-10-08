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
	[YafModule( "Provider Settings Module", "Tiny Gecko", 1 )]
	public class ProviderSettingsModule : IBaseModule
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

		public YafContext PageContext
		{
			get
			{
				return YafContext.Current;
			}
		}

		public void Init()
		{
			YafContext.Current.PageInit += new EventHandler<EventArgs>( Current_PageInit );
		}

		void Current_PageInit( object sender, EventArgs e )
		{
			// initialize the providers...
			if ( !PageContext.CurrentMembership.ApplicationName.Equals( PageContext.BoardSettings.MembershipAppName ) )
				PageContext.CurrentMembership.ApplicationName = PageContext.BoardSettings.MembershipAppName;

			if ( !PageContext.CurrentRoles.ApplicationName.Equals( PageContext.BoardSettings.RolesAppName ) )
				PageContext.CurrentRoles.ApplicationName = PageContext.BoardSettings.RolesAppName;
		}

		#region IDisposable Members

		public void Dispose()
		{

		}

		#endregion
	}
}
