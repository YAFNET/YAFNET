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
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Classes.UI;
using YAF.Controls;

namespace YAF.Modules
{
	public class YafBBCodeControl : BaseControl
	{
		protected Dictionary<string, string> _parameters = new Dictionary<string, string>();
		public Dictionary<string, string> Parameters
		{
			get { return _parameters; }
			set { _parameters = value; }
		}

		protected MessageFlags _currentMessageFlags = null;
		public MessageFlags CurrentMessageFlags
		{
			get { return _currentMessageFlags; }
			set { _currentMessageFlags = value; }
		}

		protected int? _displayUserId = null;
		public int? DisplayUserID
		{
			get { return _displayUserId; }
			set { _displayUserId = value; }
		}

		protected string ProcessBBCodeString( string bbCodeString )
		{
			return FormatMsg.FormatMessage( bbCodeString, CurrentMessageFlags );
		}

		protected string LocalizedString( string tag, string defaultStr )
		{
			if ( PageContext.Localization.GetTextExists( "BBCODEMODULE", tag ) )
			{
				return PageContext.Localization.GetText( "BBCODEMODULE", tag );
			}

			return defaultStr;
		}
	}
}