using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace YAF.Controls
{
	public class ThemeButton : BaseControl, IPostBackEventHandler
	{
		protected string _cssClass = "yafcssbutton";
		protected string _text;
		protected string _link;
		protected string _titlePage = string.Empty;
		protected string _titleTag = string.Empty;
		protected static object _clickEvent = new object();
		protected ThemeImage _themeImage = new ThemeImage();
		protected LocalizedLabel _localizedLabel = new LocalizedLabel();

		public ThemeButton()
			: base()
		{
			this.Load += new EventHandler( ThemeButton_Load );
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
			if ( !String.IsNullOrEmpty( _cssClass ) ) output.WriteAttribute( "class", _cssClass );
			if ( !String.IsNullOrEmpty( title ) ) output.WriteAttribute( "title", title );
			if ( !String.IsNullOrEmpty( _link ) )
			{
				output.WriteAttribute( "href", _link );
			}
			else
			{
				//string.Format("javascript:__doPostBack('{0}','{1}')",this.ClientID,""));
				output.WriteAttribute( "href", Page.ClientScript.GetPostBackClientHyperlink( this, "" ) );
			}

			// IE fix
			output.WriteAttribute("onclick", "this.blur();");
			output.Write( HtmlTextWriter.TagRightChar );
			output.WriteFullBeginTag( "span" );

			// render the optional controls (if any)
			base.Render( output );

			output.WriteEndTag( "span" );
			output.WriteEndTag( "a" );
			output.EndRender();
		}

		protected string GetLocalizedTitle()
		{
			if ( !String.IsNullOrEmpty( _titlePage ) && !String.IsNullOrEmpty( _titleTag ) )
			{
				return PageContext.Localization.GetText( _titlePage, _titleTag );
			}
			else if ( !String.IsNullOrEmpty( _titleTag ) )
			{
				return PageContext.Localization.GetText( _titleTag );
			}

			return null;
		}

		protected virtual void OnClick( EventArgs e )
		{
			EventHandler handler = ( EventHandler ) Events [_clickEvent];
			if ( handler != null )
				handler( this, e );
		}

		protected virtual void RaisePostBackEvent( string eventArgument )
		{
			throw new NotImplementedException();
		}

		void IPostBackEventHandler.RaisePostBackEvent( string ea )
		{
			OnClick( EventArgs.Empty );
		}

		public event EventHandler Click
		{
			add { Events.AddHandler( _clickEvent, value ); }
			remove { Events.RemoveHandler( _clickEvent, value ); }
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
			get { return _cssClass; }
			set { _cssClass = value; }
		}

		/// <summary>
		/// Setting the link property will make this control non-postback.
		/// </summary>
		public string Link
		{
			get { return _link; }
			set { _link = value; }
		}

		/// <summary>
		/// Localized Page for the optional link description (title)
		/// </summary>
		public string TitleLocalizedPage
		{
			get { return _titlePage; }
			set { _titlePage = value; }
		}

		/// <summary>
		/// Localized Tag for the optional link description (title)
		/// </summary>
		public string TitleLocalizedTag
		{
			get { return _titleTag; }
			set { _titleTag = value; }
		}
	}
}
