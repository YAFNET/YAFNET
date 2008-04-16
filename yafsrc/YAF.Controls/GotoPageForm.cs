using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

namespace YAF.Controls
{
	public class GotoPageForumEventArgs : EventArgs
	{
		public GotoPageForumEventArgs( int gotoPage )
			: base()
		{
			this.GotoPage = gotoPage;
		}

		private int _gotoPage;

		public int GotoPage
		{
			get { return _gotoPage; }
			set { _gotoPage = value; }
		}
	}

	public class GotoPageForm : BaseControl
	{
		private Panel _mainPanel = new Panel();
		private TextBox _gotoTextBox = new TextBox();
		private Button _gotoButton = new Button();
		private Label _headerText = new Label();

		public GotoPageForm()
			: base()
		{
			this.Init += new EventHandler( GotoPageForm_Init );
			this.Load += new EventHandler( GotoPageForm_Load );
		}

		void GotoPageForm_Load( object sender, EventArgs e )
		{
			// localization has to be done in here so as to not attempt
			// to localize before the class has been created
			if ( !String.IsNullOrEmpty( PageContext.Localization.TransPage ) )
			{
				_headerText.Text = PageContext.Localization.GetText( "COMMON", "GOTOPAGE_HEADER" );
				_gotoButton.Text = PageContext.Localization.GetText( "COMMON", "GO" );
			}
			else
			{
				// non-localized for admin pages
				_headerText.Text = "Goto Page...";
				_gotoButton.Text = "Go";
			}
		}

		void GotoPageForm_Init( object sender, EventArgs e )
		{
			BuildForm();
		}

		protected void BuildForm()
		{
			this.Controls.Add( _mainPanel );

			_mainPanel.CssClass = "gotoBase";

			HtmlGenericControl divHeader = new HtmlGenericControl( "div" );

			divHeader.Attributes.Add( "class", "gotoHeader" );
			divHeader.ID = GetExtendedID( "divHeader" );

			_mainPanel.Controls.Add( divHeader );

			_headerText.ID = GetExtendedID( "headerText" );

			divHeader.Controls.Add( _headerText );

			HtmlGenericControl divInner = new HtmlGenericControl( "div" );
			divInner.Attributes.Add( "class", "gotoInner" );

			_mainPanel.Controls.Add( divInner );

			_gotoTextBox.ID = GetExtendedID( "GotoTextBox" );
			_gotoTextBox.Style.Add( HtmlTextWriterStyle.Width, "30px" );

			divInner.Controls.Add( _gotoTextBox );

			_gotoButton.ID = GetExtendedID( "GotoButton" );
			_gotoButton.Style.Add( HtmlTextWriterStyle.Width, "30px" );
			_gotoButton.CausesValidation = false;
			_gotoButton.UseSubmitBehavior = false;
			_gotoButton.Click += new EventHandler( _gotoButton_Click );			

			divInner.Controls.Add( _gotoButton );
		}

		protected override void Render( HtmlTextWriter writer )
		{
			writer.WriteLine( String.Format( @"<div id=""{0}"" style=""display:none"" class=""gotoPageForm"">", this.ClientID ) );

			base.Render( writer );

			writer.WriteLine( "</div>" );
		}

		void _gotoButton_Click( object sender, EventArgs e )
		{
			if ( GotoPageClick != null )
			{
				// attempt to parse the page value...
				if ( int.TryParse( _gotoTextBox.Text.Trim(), out _gotoPageValue ) )
				{
					// valid, fire the event...
					GotoPageClick( this, new GotoPageForumEventArgs( GotoPageValue ) );
				}
			}
			// clear the old value...
			_gotoTextBox.Text = "";
		}

		public event EventHandler<GotoPageForumEventArgs> GotoPageClick;

		private int _gotoPageValue;

		public int GotoPageValue
		{
			get { return _gotoPageValue; }
			set { _gotoPageValue = value; }
		}
	}
}
