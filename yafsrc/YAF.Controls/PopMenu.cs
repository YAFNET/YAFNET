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
using System.Data;
using System.Collections;
using System.Collections.Generic;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for ForumJump.
	/// </summary>
	public class PopMenu : BaseControl, System.Web.UI.IPostBackEventHandler
	{
		private string _control = string.Empty;
		private List<InternalPopMenuItem> _items = new List<InternalPopMenuItem>();

		public string Control
		{
			set
			{
				_control = value;
			}
			get
			{
				return _control;
			}
		}

		public void AddPostBackItem( string argument, string description )
		{
			_items.Add( new InternalPopMenuItem( description, argument, null ) );
		}

		public void AddClientScriptItem( string description, string clientScript )
		{
			_items.Add( new InternalPopMenuItem( description, null, clientScript ) );
		}

		public void Attach( System.Web.UI.WebControls.WebControl ctl )
		{
			ctl.Attributes ["onclick"] = ControlOnClick;
			ctl.Attributes ["onmouseover"] = ControlOnMouseOver;
		}

		public void Attach( UserLink userLinkControl )
		{
			userLinkControl.OnClick = ControlOnClick;
			userLinkControl.OnMouseOver = ControlOnMouseOver;
		}

		public string ControlOnClick
		{
			get
			{
				return string.Format( "yaf_popit('{0}')", this.ClientID );
			}
		}

		public string ControlOnMouseOver
		{
			get
			{
				return string.Format( "yaf_mouseover('{0}')", this.ClientID );
			}
		}

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			if ( !this.Visible )
				return;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendFormat( @"<div class=""yafpopupmenu"" id=""{0}"" style=""position:absolute;z-index:100;left:0;top:0;visibility:hidden;"">", this.ClientID );
			sb.Append( "<ul>" );

			// add the items
			foreach ( InternalPopMenuItem thisItem in _items )
			{
				string onClick;

				if ( !String.IsNullOrEmpty( thisItem.ClientScript ) )
				{
					// postback style...
					onClick = thisItem.ClientScript;
				}
				else
				{
					onClick = Page.ClientScript.GetPostBackClientHyperlink( this, thisItem.PostBackArgument );
				}

				sb.AppendFormat( @"<li class=""popupitem"" onmouseover=""mouseHover(this,true)"" onmouseout=""mouseHover(this,false)"" onclick=""{1}"" style=""white-space:nowrap"">{0}</li>", thisItem.Description, onClick );
			}

			sb.AppendFormat( "</ul></div>" );

			writer.WriteLine( sb.ToString() );

			base.Render( writer );
		}

		#region IPostBackEventHandler
		public event PopEventHandler ItemClick;

		public void RaisePostBackEvent( string eventArgument )
		{
			if ( ItemClick != null )
			{
				ItemClick( this, new PopEventArgs( eventArgument ) );
			}
		}
		#endregion
	}

	public class PopEventArgs : EventArgs
	{
		private string _item;

		public PopEventArgs( string eventArgument )
		{
			_item = eventArgument;
		}

		public string Item
		{
			get
			{
				return _item;
			}
		}
	}

	public delegate void PopEventHandler( object sender, PopEventArgs e );

	public class InternalPopMenuItem
	{
		private string _description = null;
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		private string _postbackArgument = null;
		public string PostBackArgument
		{
			get { return _postbackArgument; }
			set { _postbackArgument = value; }
		}

		private string _clientScript = null;
		public string ClientScript
		{
			get { return _clientScript; }
			set { _clientScript = value; }
		}

		public InternalPopMenuItem( string description, string postbackArgument, string clientScript )
		{
			_description = description;
			_postbackArgument = postbackArgument;
			_clientScript = clientScript;
		}
	}
}
