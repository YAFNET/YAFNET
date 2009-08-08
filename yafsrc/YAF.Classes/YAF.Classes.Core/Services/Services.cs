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
using System.Text;
using YAF.Classes.Utils;
using YAF.Modules;

namespace YAF.Classes.Core
{
	public static class YafServices
	{
		public static YafBadWordReplace BadWordReplace
		{
			get
			{
				return YafContext.Current.InstanceFactory.GetInstance<YafBadWordReplace>();
			}
		}

		public static YafPermissions Permissions
		{
			get
			{
				return YafContext.Current.InstanceFactory.GetInstance<YafPermissions>();
			}
		}

		public static YafSendMail SendMail
		{
			get
			{
				return YafContext.Current.InstanceFactory.GetInstance<YafSendMail>();
			}
		}

		public static YafDBBroker DBBroker
		{
			get
			{
				return YafContext.Current.InstanceFactory.GetInstance<YafDBBroker>();
			}
		}

		public static YafDateTime DateTime
		{
			get
			{
				return YafContext.Current.InstanceFactory.GetInstance<YafDateTime>();
			}
		}

		public static YafStopWatch StopWatch
		{
			get
			{
				return YafContext.Current.InstanceFactory.GetInstance<YafStopWatch>();
			}
		}

		public static YafInitializeDb InitializeDb
		{
			get
			{
				return YafContext.Current.InstanceFactory.GetInstance<YafInitializeDb>();
			}
		}

		public static YafCheckBannedIps BannedIps
		{
			get
			{
				return YafContext.Current.InstanceFactory.GetInstance<YafCheckBannedIps>();
			}
		}
	}
}
