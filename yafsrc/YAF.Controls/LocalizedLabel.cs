/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace YAF.Controls
{
	/// <summary>
	/// Makes a very simple localized label
	/// </summary>
	public class LocalizedLabel : BaseControl
	{
		protected string _localizedPage = string.Empty;
		protected string _localizedTag = string.Empty;
		protected string _param0 = string.Empty;
		protected string _param1 = string.Empty;
		protected string _param2 = string.Empty;

		public LocalizedLabel()
			: base()
		{
		}

		/// <summary>
		/// Shows the localized text string (if available)
		/// </summary>
		/// <param name="output"></param>
		protected override void Render( HtmlTextWriter output )
		{
			output.BeginRender();
			output.Write( String.Format( GetCurrentItem(), _param0, _param1, _param2) );
			output.EndRender();
		}

		protected string GetCurrentItem()
		{
			if ( !String.IsNullOrEmpty( _localizedPage ) && !String.IsNullOrEmpty( _localizedTag ) )
			{
				return PageContext.Localization.GetText( LocalizedPage, LocalizedTag );
			}
			else if ( !String.IsNullOrEmpty( _localizedTag ) )
			{
				return PageContext.Localization.GetText( LocalizedTag );
			}

			return null;
		}

		public string LocalizedPage
		{
			get { return _localizedPage; }
			set { _localizedPage = value; }
		}

		public string LocalizedTag
		{
			get { return _localizedTag; }
			set { _localizedTag = value; }
		}

		public string Param0
		{
			get { return _param0; }
			set { _param0 = value; }
		}

		public string Param1
		{
			get { return _param1; }
			set { _param1 = value; }
		}

		public string Param2
		{
			get { return _param2; }
			set { _param2 = value; }
		}
	}
}
