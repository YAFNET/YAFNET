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
using System.Web;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for SuspendCheckModule
	/// </summary>
	[YafModule( "Suspend Check Module", "Tiny Gecko", 1 )]
	public class SuspendCheckModule : SimpleBaseModule
	{
		public SuspendCheckModule()
		{

		}

		public override void InitBeforePage()
		{
			PageContext.PagePreLoad += new EventHandler<EventArgs>( PageContext_PagePreLoad );
		}

		void PageContext_PagePreLoad( object sender, EventArgs e )
		{
			// check for suspension if enabled...
			if ( PageContext.Globals.IsSuspendCheckEnabled && PageContext.IsSuspended )
			{
				if ( PageContext.SuspendedUntil < DateTime.Now )
				{
					YAF.Classes.Data.DB.user_suspend( PageContext.PageUserID, null );
					HttpContext.Current.Response.Redirect( General.GetSafeRawUrl() );
				}

				YafBuildLink.RedirectInfoPage( InfoMessage.Suspended );
			}
		}
	}
}