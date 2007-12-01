/* Yet Another Forum.NET
 * Copyright (C) 2006-2007 Jaben Cargman
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

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for ForumJump.
	/// </summary>
	public class PopMenu : BaseControl, System.Web.UI.IPostBackEventHandler
	{
		private string _control = string.Empty;
		private Hashtable _items = new Hashtable();

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

		protected string ControlID
		{
			get
			{
				return string.Format( "{0}_{1}", Parent.ClientID, _control );
			}
		}

		public void AddItem( string title, string script )
		{
			_items.Add( title, script );
		}

		public void Attach( System.Web.UI.WebControls.WebControl ctl )
		{
			ctl.Attributes ["onclick"] = string.Format( "yaf_popit('{0}')", this.ClientID );
			ctl.Attributes ["onmouseover"] = string.Format( "yaf_mouseover('{0}')", this.ClientID );
		}

		public void Attach( UserLink userLinkControl )
		{
			userLinkControl.OnClick = string.Format( "yaf_popit('{0}')", this.ClientID );
			userLinkControl.OnMouseOver = string.Format( "yaf_mouseover('{0}')", this.ClientID );
		}

		private void Page_Load( object sender, System.EventArgs e )
		{
			/*
			if ( this.Visible )
			{
				Page.ClientScript.RegisterStartupScript( ClientID, string.Format( "<script language='javascript'>yaf_initmenu('{0}');</script>", ControlID ) );
			}
			*/
		}

		override protected void OnInit( EventArgs e )
		{
			this.Load += new System.EventHandler( this.Page_Load );
			this.PreRender += new EventHandler( PopMenu_PreRender );
			base.OnInit( e );
		}

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
		}

		private void PopMenu_PreRender( object sender, EventArgs e )
		{
			if ( !this.Visible )
				return;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendFormat( @"<div class=""yafpopupmenu"" id=""{0}"" style=""position:absolute;z-index:100;left:0;top:0;visibility:hidden;"">", this.ClientID );
			sb.Append( "<ul>" );

			foreach ( string key in _items.Keys )
			{
				sb.AppendFormat( @"<li class=""popupitem"" onmouseover=""mouseHover(this,true)"" onmouseout=""mouseHover(this,false)"" onclick=""{1}""><nobr>{0}</nobr></li>", _items [key], Page.ClientScript.GetPostBackClientHyperlink( this, key ) );
			}
			sb.AppendFormat( "</ul></div>" );

			Page.ClientScript.RegisterStartupScript( this.GetType(), ClientID + "_menuscript", sb.ToString() );
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
}
