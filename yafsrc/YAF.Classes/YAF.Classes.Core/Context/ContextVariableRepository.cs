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
using System.Web.Security;
using YAF.Classes.Data;
using YAF.Classes.Pattern;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
	/// <summary>
	/// Place to put helper properties for context variables inside.
	/// </summary>
	public class ContextVariableRepository
	{
		private TypeDictionary _dic = null;
		protected TypeDictionary Vars
		{
			get
			{
				return _dic;
			}
		}

		public ContextVariableRepository( TypeDictionary dictionary )
		{
			_dic = dictionary;
		}

		/// <summary>
		/// Flag set if the system should check if the user is suspended and redirect appropriately. Defaults to true.
		/// Setting to false effectively disables suspend checking.
		/// </summary>
		public bool IsSuspendCheckEnabled
		{
			set
			{
				Vars["IsSuspendCheckEnabled"] = value;
			}
			get
			{
				return Vars.AsBoolean( "IsSuspendCheckEnabled" ) ?? true;
			}
		}
	}
}
