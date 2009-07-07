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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace YAF.Controls
{
	public class ThemeButton : BaseControl, IPostBackEventHandler
	{
		protected AttributeCollection _attributeCollection;
		protected static object _clickEvent = new object();
		protected static object _commandEvent = new object();
		protected ThemeImage _themeImage = new ThemeImage();
		protected LocalizedLabel _localizedLabel = new LocalizedLabel();

		public ThemeButton()
			: base()
		{
			this.Load += new EventHandler( ThemeButton_Load );
			_attributeCollection = new AttributeCollection( ViewState );
		}

		/// <summary>
		/// Setup the controls before render
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ThemeButton_Load( object sender, EventArgs e )
		{
			if ( !String.IsNullOrEmpty( _themeImage.ThemeTag ) )
			{
				// add the theme image...
				this.Controls.Add( _themeImage );
			}

			// render the text if available
			if ( !String.IsNullOrEmpty( _localizedLabel.LocalizedTag ) )
			{
				this.Controls.Add( _localizedLabel );
			}	
		}

		protected override void Render( HtmlTextWriter output )
		{
			// get the title...
			string title = GetLocalizedTitle();

			output.BeginRender();
			output.WriteBeginTag( "a" );
			output.WriteAttribute( "id", this.ClientID );
			if ( !String.IsNullOrEmpty( CssClass ) )
			{
				output.WriteAttribute( "class", CssClass );
			}
			if ( !String.IsNullOrEmpty( title ) )
			{
				output.WriteAttribute( "title", title );
			}
			else if ( !String.IsNullOrEmpty( TitleNonLocalized ) )
			{
				output.WriteAttribute( "title", TitleNonLocalized );
			}
			if ( !String.IsNullOrEmpty( NavigateUrl ) )
			{
				output.WriteAttribute( "href", NavigateUrl.Replace( "&", "&amp;" ) );
			}
			else
			{
				//string.Format("javascript:__doPostBack('{0}','{1}')",this.ClientID,""));
				output.WriteAttribute( "href", Page.ClientScript.GetPostBackClientHyperlink( this, "" ) );
			}

			bool wroteOnClick = false;

			// handle additional attributes (if any)
			if ( _attributeCollection.Count > 0 )
			{
				// add attributes...
				foreach ( string key in _attributeCollection.Keys )
				{
					// get the attribute and write it...
					if ( key.ToLower() == "onclick" )
					{
						// special handling... add to it...
						output.WriteAttribute( key, string.Format("{0};{1}", _attributeCollection [key], "this.blur();" ));
						wroteOnClick = true;
					}
					else if (key.ToLower().StartsWith("on") || key.ToLower() == "rel" || key.ToLower() == "target" )
					{
						// only write javascript attributes -- and a few other attributes...
						output.WriteAttribute( key, _attributeCollection [key] );
					}
				}
			}

			// IE fix
			if ( !wroteOnClick ) output.WriteAttribute( "onclick", "this.blur();" );
			output.Write( HtmlTextWriter.TagRightChar );

			output.WriteBeginTag( "span" );
			output.Write( HtmlTextWriter.TagRightChar );
			// render the optional controls (if any)
			base.Render( output );
			output.WriteEndTag( "span" );

			output.WriteEndTag( "a" );
			output.EndRender();
		}

		protected string GetLocalizedTitle()
		{
			if ( this.Site != null && this.Site.DesignMode == true && !String.IsNullOrEmpty( TitleLocalizedTag ) )
			{
				return String.Format( "[TITLE:{0}]", TitleLocalizedTag );
			}
			else if ( !String.IsNullOrEmpty( TitleLocalizedPage ) && !String.IsNullOrEmpty( TitleLocalizedTag ) )
			{
				return PageContext.Localization.GetText( TitleLocalizedPage, TitleLocalizedTag );
			}
			else if ( !String.IsNullOrEmpty( TitleLocalizedTag ) )
			{
				return PageContext.Localization.GetText( TitleLocalizedTag );
			}

			return null;
		}

		protected virtual void OnClick( EventArgs e )
		{
			EventHandler handler = ( EventHandler ) Events [_clickEvent];
			if ( handler != null )
				handler( this, e );
		}

		protected virtual void OnCommand( CommandEventArgs e )
		{
			CommandEventHandler handler = (CommandEventHandler) Events[_commandEvent];

			if (handler != null)
				handler( this, e );

			this.RaiseBubbleEvent( this, e );
		}

		void IPostBackEventHandler.RaisePostBackEvent( string eventArgument )
		{
			OnCommand( new CommandEventArgs( CommandName, CommandArgument ) );
			OnClick( EventArgs.Empty );
		}
		
		public event EventHandler Click
		{
			add { Events.AddHandler( _clickEvent, value ); }
			remove { Events.RemoveHandler( _clickEvent, value ); }
		}

		public event CommandEventHandler Command
		{
			add { Events.AddHandler( _commandEvent, value ); }
			remove { Events.RemoveHandler( _commandEvent, value ); }
		}

		/// <summary>
		/// ThemePage for the optional button image
		/// </summary>
		public string ImageThemePage
		{
			get { return _themeImage.ThemePage; }
			set { _themeImage.ThemePage = value; }
		}

		/// <summary>
		/// ThemeTag for the optional button image
		/// </summary>
		public string ImageThemeTag
		{
			get { return _themeImage.ThemeTag; }
			set { _themeImage.ThemeTag = value; }
		}

		/// <summary>
		/// Localized Page for the optional button text
		/// </summary>
		public string TextLocalizedPage
		{
			get { return _localizedLabel.LocalizedPage; }
			set { _localizedLabel.LocalizedPage = value; }
		}

		/// <summary>
		/// Localized Tag for the optional button text
		/// </summary>
		public string TextLocalizedTag
		{
			get { return _localizedLabel.LocalizedTag; }
			set { _localizedLabel.LocalizedTag = value; }
		}

		/// <summary>
		/// Defaults to "yafcssbutton"
		/// </summary>
		public string CssClass
		{
			get { return ( ViewState ["CssClass"] != null ) ? ViewState ["CssClass"] as string : "yafcssbutton"; }
			set { ViewState ["CssClass"] = value; }
		}

		/// <summary>
		/// Setting the link property will make this control non-postback.
		/// </summary>
		public string NavigateUrl
		{
			get { return ( ViewState ["NavigateUrl"] != null ) ? ViewState ["NavigateUrl"] as string : string.Empty; }
			set { ViewState ["NavigateUrl"] = value; }
		}

		/// <summary>
		/// Localized Page for the optional link description (title)
		/// </summary>
		public string TitleLocalizedPage
		{
			get { return ( ViewState ["TitleLocalizedPage"] != null ) ? ViewState ["TitleLocalizedPage"] as string : "BUTTON"; }
			set { ViewState ["TitleLocalizedPage"] = value; }
		}

		/// <summary>
		/// Localized Tag for the optional link description (title)
		/// </summary>
		public string TitleLocalizedTag
		{
			get { return ( ViewState ["TitleLocalizedTag"] != null ) ? ViewState ["TitleLocalizedTag"] as string : string.Empty; }
			set { ViewState ["TitleLocalizedTag"] = value; }
		}

		/// <summary>
		/// Non-localized Title for optional link description
		/// </summary>
		public string TitleNonLocalized
		{
			get { return ( ViewState ["TitleNonLocalized"] != null ) ? ViewState ["TitleNonLocalized"] as string : string.Empty; }
			set { ViewState ["TitleNonLocalized"] = value; }
		}

		public AttributeCollection Attributes
		{
			get { return _attributeCollection; }
		}

		public string CommandName
		{
			get
			{
				if ( ViewState ["commandName"] != null )
					return ViewState ["commandName"].ToString();

				return null;
			}
			set { ViewState ["commandName"] = value; }
		}

		public string CommandArgument
		{
			get
			{
				if ( ViewState ["commandArgument"] != null )
					return ViewState ["commandArgument"].ToString();

				return null;
			}
			set { ViewState ["commandArgument"] = value; }
		}
	}
}
