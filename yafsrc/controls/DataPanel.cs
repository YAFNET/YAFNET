#region References
using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using yaf;
#endregion References

namespace yaf.controls
{
	#region DataPanel Class
	/// <summary>
	/// Web control which provides a panel control as a 
	/// collapsible/expandable control container. The control
	/// provides a panel title and action link, either/both 
	/// may provide the expand/collapse action as a hot-link.
	/// </summary>
	/// <remarks>
	/// The control is used by registering the library in the
	/// Visual Studio Toolbox, and then dragging it onto the
	/// ASP.NET form.  For the control to be truly
	/// expandable/collapsible, the style tag 'POSITION: absolute'
	/// should be removed from the HTML control definition, or
	/// else the control should be place in another control
	/// such as a table.
	/// </remarks>
	[
	ToolboxData("<{0}:DataPanel runat=server></{0}:DataPanel>")
	]
	public class DataPanel : Panel, IPostBackDataHandler
	{
		#region Scripts
		/// <summary>
		/// Name of the script block to control the collapsing/expanding
		/// of the control.
		/// </summary>
		private const string _dataPanelScript = "DataPanel";
		/// <summary>
		/// Method to extract the script block text from the control
		/// resources and register it as a client script block.
		/// </summary>
		private void RegisterControlScript() 
		{
			if( ! Page.ClientScript.IsClientScriptBlockRegistered(_dataPanelScript)) 
			{
				Page.ClientScript.RegisterClientScriptBlock( this.GetType(), _dataPanelScript, string.Format( "<script language='javascript' src='{0}'></script>", ForumPage.GetURLToResource( "DataPanel.js" ) ) );
			}
		}

		public yaf.pages.ForumPage ForumPage
		{
			get
			{
				System.Web.UI.Control ctl = Parent;
				System.Web.UI.Control thePage = this;
				while ( ctl.GetType() != typeof( yaf.Forum ) )
				{
					thePage = ctl;
					ctl = ctl.Parent;
				}

				return ( yaf.pages.ForumPage ) thePage;
			}
		}
		#endregion Scripts
		#region Controls
		/// <summary>
		/// The layout table element for the control.
		/// </summary>
		private HtmlTable _controlTable = null;
		/// <summary>
		/// The header row element for the control.
		/// </summary>
		private HtmlTableRow _headerRow = null;
		/// <summary>
		/// The header title cell element for the control.
		/// </summary>
		private HtmlTableCell _titleCell = null;
		/// <summary>
		/// The cell element that contains the expand/collapse action link or
		/// image for the control.
		/// </summary>
		private HtmlTableCell _actionCell = null;
		/// <summary>
		/// The row element for the user content controls.
		/// </summary>
		private HtmlTableRow _contentRow = null;
		/// <summary>
		/// The cell element for the user content panel.
		/// </summary>
		private HtmlTableCell _contentCell = null;
		/// <summary>
		/// The header panel element that is contained by the the
		/// header row element.
		/// </summary>
		private Panel _headerPanel = null;
		/// <summary>
		/// The title hyperlink element of the control.
		/// </summary>
		/// <remarks>
		/// This link is active only if the 
		/// <see cref="AllowTitleExpandCollapse"/> property is true.
		/// </remarks>
		private HyperLink _titleLink = null;
		/// <summary>
		/// The action hyperlink element of the control.
		/// </summary>
		/// <remarks>
		/// This link is active only if the
		/// <see cref="HideActionExpandCollapse"/> is false.
		/// </remarks>
		private HyperLink _actionLink = null;
		/// <summary>
		/// Hidden input field which is set by the client script to
		/// indicate if the control has been expanded or collapsed by the user.
		/// </summary>
		/// <remarks>
		/// If the control is collapsed, the value of this field is "true" (string).
		/// If the control is expanded, the value of this field is "false" (string).
		/// </remarks>
		private HtmlInputHidden _currentState = null;
		#endregion Controls
		#region Constructors
		/// <summary>
		/// Default Constructor.
		/// </summary>
		/// <remarks>
		/// If the <see cref="WebControl.Width">Width</see> of the control is not set,
		/// the value of '100%' is set as default.
		/// </remarks>
		public DataPanel()
		{
			if(base.Width.IsEmpty == true)
			{
				this.Width = Unit.Parse("100%");
			}
		}
		#endregion Constructors
		#region Properties
		/// <summary>
		/// The constant string "AllowTitleExpandCollapse".
		/// </summary>
		/// <remarks>
		/// This value is the key for the property
		/// state parameter in the <see cref="Control.ViewState"/>
		/// object.
		/// </remarks>
		private const string _allowTitleExpandCollapse = 
			"AllowTitleExpandCollapse";
		/// <summary>
		/// Set to true to allow clicking the panel
		/// header title to expand/collapse the content panel.
		/// </summary>
		/// <remarks>
		/// The <see cref="Control.ViewState">ViewState</see> 
		/// object is used to save the
		/// value of this property between postbacks.
		/// </remarks>
		[
		Description("Set to true to allow clicking the panel" +
			"header title to expand/collapse the content panel.")
		, Category("Appearance")
		, DefaultValue(false)
		]
		public bool AllowTitleExpandCollapse
		{
			get 
			{
				if(this.ViewState[_allowTitleExpandCollapse] == null)
					return false;
				return System.Convert.ToBoolean(
					  this.ViewState[_allowTitleExpandCollapse]); 
			}
			set { this.ViewState[_allowTitleExpandCollapse] = value; }
		}
		/// <summary>
		/// The constant string "Collapsed".
		/// </summary>
		/// <remarks>
		/// This value is the key for the property
		/// state parameter in the 
		/// <see cref="Control.ViewState">ViewState</see>
		/// object.
		/// </remarks>
		private const string _collapsed = "Collapsed";
		/// <summary>
		/// Set to true to collapse the content panel, 
		/// else false to expand.
		/// </summary>
		/// <remarks>
		/// The <see cref="Control.ViewState">ViewState</see>
		/// object is used to save the
		/// value of this property between postbacks.
		/// </remarks>
		[
		Description("Set to true to collapse the content panel," +
			" else false to expand.")
		, Category("Appearance")
		, DefaultValue(true)
		]
		public bool Collapsed
		{
			get 
			{ 
				if(this.ViewState[_collapsed] == null)
					return true;
				return System.Convert.ToBoolean(
					  this.ViewState[_collapsed]); 
			} 
			set { this.ViewState[_collapsed] = value; }
		}
		/// <summary>
		/// The constant string "Collapsible".
		/// </summary>
		/// <remarks>
		/// This value is the key for the property
		/// state parameter in the 
		/// <see cref="Control.ViewState">ViewState</see>
		/// object.
		/// </remarks>
		private const string _collapsible = "Collapsible";
		/// <summary>
		/// Set to true to enable the user content panel to
		/// collapse, else false to disable.
		/// </summary>
		/// <remarks>
		/// The <see cref="Control.ViewState">ViewState</see> 
		/// object is used to save the
		/// value of this property between postbacks.
		/// </remarks>
		[
		Description("Set to true to enable the content panel to" +
			" collapse, else false to disable.")
		, Category("Appearance")
		, DefaultValue(true)
		]
		public bool Collapsible
		{
			get 
			{
				if(this.ViewState[_collapsible] == null)
					return true;
				return System.Convert.ToBoolean(
					  this.ViewState[_collapsible]); 
			} 
			set { this.ViewState[_collapsible] = value; }
		}
		/// <summary>
		/// The constant string "CollapseImageUrl".
		/// </summary>
		/// <remarks>
		/// This value is the key for the property
		/// state parameter in the 
		/// <see cref="Control.ViewState">ViewState</see>
		/// object.
		/// </remarks>
		private const string _collapseImageUrl = "CollapseImageUrl";
		/// <summary>
		/// Url of the image to use for the collapse-action link.
		/// </summary>
		/// <remarks>
		/// The <see cref="Control.ViewState">ViewState</see> 
		/// object is used to save the
		/// value of this property between postbacks.
		/// </remarks>
		[
		Description("Url of the image to use for the collapse-" +
			"action link.")
		, Category("Appearance")
		, DefaultValue("")
		]
		public string CollapseImageUrl
		{
			get 
			{
				if(this.ViewState[_collapseImageUrl] == null)
					return "";
				return this.ViewState[_collapseImageUrl].ToString();
			}
			set { this.ViewState[_collapseImageUrl] = value; }
		}
		/// <summary>
		/// The constant string "CollapseText".
		/// </summary>
		/// <remarks>
		/// This value is the key for the property
		/// state parameter in the 
		/// <see cref="Control.ViewState">ViewState</see>
		/// object.
		/// </remarks>
		private const string _collapseText = "CollapseText";
		/// <summary>
		/// Text to use for the collapse-action link.
		/// </summary>
		/// <remarks>
		/// The <see cref="Control.ViewState">ViewState</see> 
		/// object is used to save the
		/// value of this property between postbacks.
		/// </remarks>
		[
		Description("Text to use for the collapse-action link.")
		, Category("Appearance")
		, DefaultValue("Collapse")
		]
		public string CollapseText
		{
			get
			{
				if(this.ViewState[_collapseText] == null)
					return "Collapse";
				return this.ViewState[_collapseText].ToString();
			}
			set { this.ViewState[_collapseText] = value; }
		}
		/// <summary>
		/// The constant string "ExpandImageUrl".
		/// </summary>
		/// <remarks>
		/// This value is the key for the property
		/// state parameter in the 
		/// <see cref="Control.ViewState">ViewState</see>
		/// object.
		/// </remarks>
		private const string _expandImageUrl = "ExpandImageUrl";
		/// <summary>
		/// Url of the image to use for the expand-action link.
		/// </summary>
		/// <remarks>
		/// The <see cref="Control.ViewState">ViewState</see> 
		/// object is used to save the
		/// value of this property between postbacks.
		/// </remarks>
		[
		Description("Url of the image to use for the expand-action link.")
		, Category("Appearance")
		, DefaultValue("")
		]
		public string ExpandImageUrl
		{
			get
			{ 
				if(this.ViewState[_expandImageUrl] == null)
					return "";
				return this.ViewState[_expandImageUrl].ToString();
			}
			set { this.ViewState[_expandImageUrl] = value; }
		}
		/// <summary>
		/// The constant string "ExpandText".
		/// </summary>
		/// <remarks>
		/// This value is the key for the property
		/// state parameter in the 
		/// <see cref="Control.ViewState">ViewState</see>
		/// object.
		/// </remarks>
		private const string _expandText = "ExpandText";
		/// <summary>
		/// Text to use for the expand-action link.
		/// </summary>
		/// <remarks>
		/// The <see cref="Control.ViewState">ViewState</see> 
		/// object is used to save the
		/// value of this property between postbacks.
		/// </remarks>
		[
		Description("Text to use for the expand-action link.")
		, Category("Appearance")
		, DefaultValue("Expand")
		]
		public string ExpandText
		{
			get
			{
				if(this.ViewState[_expandText] == null)
					return "Expand";
				return this.ViewState[_expandText].ToString();
			}
			set { this.ViewState[_expandText] = value; }
		}
		/// <summary>
		/// The constant string "HideActionExpandCollapse".
		/// </summary>
		/// <remarks>
		/// This value is the key for the property
		/// state parameter in the 
		/// <see cref="Control.ViewState">ViewState</see>
		/// object.
		/// </remarks>
		string _hideActionExpandCollapse = "HideActionExpandCollapse";
		/// <summary>
		/// Set to true to hide the text/image action link which expands
		/// or collapses the content panel.
		/// </summary>
		/// <remarks>
		/// The <see cref="Control.ViewState">ViewState</see> 
		/// object is used to save the
		/// value of this property between postbacks.
		/// </remarks>
		[
		Description("Set to true to hide the text/image action link which" +
			" expands or collapses the content panel.")
		, Category("Appearance")
		, DefaultValue(false)
		]
		public bool HideActionExpandCollapse
		{
			get 
			{
				if(this.ViewState[_hideActionExpandCollapse] == null)
					return false;
				return System.Convert.ToBoolean(
					  this.ViewState[_hideActionExpandCollapse]);
			}
			set { this.ViewState[_hideActionExpandCollapse] = value; }
		}
		/// <summary>
		/// The constant string "TitleText".
		/// </summary>
		/// <remarks>
		/// This value is the key for the property
		/// state parameter in the 
		/// <see cref="Control.ViewState">ViewState</see>
		/// object.
		/// </remarks>
		private const string _titleText = "TitleText";
		/// <summary>
		/// Text that will appear on the control title header.
		/// </summary>
		/// <remarks>
		/// The <see cref="Control.ViewState">ViewState</see> 
		/// object is used to save the
		/// value of this property between postbacks.
		/// </remarks>
		[
		Description("Text that will appear on the control title header.")
		, Category("Appearance")
		, DefaultValue("")
		]
		public string TitleText
		{
			get 
			{ 
				if(this.ViewState[_titleText] == null)
					return "";
				return this.ViewState[_titleText].ToString();
			}
			set	{ this.ViewState[_titleText] = value; }
		}
		/// <summary>
		/// The member used for the <see cref="TitleStyle"/>
		/// property.
		/// </summary>
		private Style _titleStyle = new Style();
		/// <summary>
		/// Display style of the control title header.
		/// </summary>
		/// <remarks>
		/// The <see cref="SaveViewState"/> override is used to save
		/// the value of this property between postbacks, while
		/// the <see cref="LoadViewState"/> override is used to restore
		/// the value.
		/// </remarks>
		[
		Description("Display style of the control title header.")
		, Category("Style")
		, DefaultValue(typeof(Style))
		, PersistenceMode(PersistenceMode.Attribute)
		, DesignerSerializationVisibility(
			DesignerSerializationVisibility.Content)
		]
		public Style TitleStyle
		{
			get
			{
				if(_titleStyle == null) 
				{
					_titleStyle = new Style();
				}
				if(this.IsTrackingViewState)
				{
					((IStateManager)_titleStyle).TrackViewState();
					// Force the complete style to be saved in viewstate
					// every time.
					_titleStyle.BackColor = _titleStyle.BackColor;
					_titleStyle.BorderColor = _titleStyle.BorderColor;
					_titleStyle.BorderStyle = _titleStyle.BorderStyle;
					_titleStyle.BorderWidth = _titleStyle.BorderWidth;
					_titleStyle.CssClass = _titleStyle.CssClass;
					_titleStyle.Font.Bold = _titleStyle.Font.Bold;
					_titleStyle.Font.Italic = _titleStyle.Font.Italic;
					_titleStyle.Font.Name = _titleStyle.Font.Name;
					_titleStyle.Font.Names = _titleStyle.Font.Names;
					_titleStyle.Font.Overline = _titleStyle.Font.Overline;
					_titleStyle.Font.Size = _titleStyle.Font.Size;
					_titleStyle.Font.Strikeout = _titleStyle.Font.Strikeout;
					_titleStyle.Font.Underline = _titleStyle.Font.Underline;
					_titleStyle.ForeColor = _titleStyle.ForeColor;
					_titleStyle.Height = _titleStyle.Height;
					_titleStyle.Width = _titleStyle.Width;
				}
				return _titleStyle;
			}
		}
		#endregion Properties
		#region Implementation
		/// <summary>
		/// Converts a <see cref="Color"/> object to an HTML
		/// compliant string.
		/// </summary>
		/// <param name="StyleColor">
		/// The <see cref="Color"/> object to convert.
		/// </param>
		/// <returns>The HTML colour string.</returns>
		private string HtmlColorString(Color StyleColor)
		{
			// Format ARGB
			return "#" + (StyleColor.ToArgb() & 0x00FFFFFF).ToString("X6");
		}
		/// <summary>
		/// Applies a <see cref="Style"/> object to an
		/// <see cref="HtmlTableCell"/> object.
		/// </summary>
		/// <param name="Cell">
		/// <see cref="HtmlTableCell"/> object to apply to.
		/// </param>
		/// <param name="ControlStyle">
		/// The <see cref="Style"/> object to apply.
		/// </param>
		/// The <see cref="Style"/> properties are converted
		/// to the <see cref="HtmlTableCell"/> object 
		/// <see cref="CssStyleCollection"/> attributes.
		private void ApplyStyle(HtmlTableCell Cell, Style ControlStyle)
		{
			string fontNames = "";
			string textDecorations = "";
			// Background colour.
			if(ControlStyle.BackColor.IsEmpty == false)
			{
				Cell.Style.Add("background-color", 
					HtmlColorString(ControlStyle.BackColor));
			}
			// Border colour.
			if(ControlStyle.BorderColor.IsEmpty == false)
			{
				Cell.Style.Add("border-color", 
					HtmlColorString(ControlStyle.BorderColor));
			}
			// Border style.
			if( (ControlStyle.BorderStyle.ToString() != "") &&
				(ControlStyle.BorderStyle.ToString().ToLower() != "notset") )
			{
				Cell.Style.Add("border-style", 
					ControlStyle.BorderStyle.ToString());
			}
			// Border width.
			if(ControlStyle.BorderWidth.IsEmpty == false)
			{
				Cell.Style.Add("border-width", 
					ControlStyle.BorderWidth.ToString());
			}
			// Class.
			if(ControlStyle.CssClass != "")
			{
				Cell.Attributes.Add("class", ControlStyle.CssClass); 
			}
			// Font bold.
			if(ControlStyle.Font.Bold == true)
			{
				Cell.Style.Add("font-weight", "bold"); 
			}
			// Font italic.
			if(ControlStyle.Font.Italic == true)
			{
				Cell.Style.Add("font-style", "italic"); 
			}
			// Font name.
			if(ControlStyle.Font.Name != "")
			{
				fontNames = ControlStyle.Font.Name;
				Cell.Style.Add("font-family", fontNames);
			}
			// Font names.
			if(ControlStyle.Font.Names.Length > 0)
			{
				foreach(string s in ControlStyle.Font.Names)
				{
					if(fontNames.IndexOf(s) == -1)
					{
						fontNames += "," + s;
					}
				}
			}
			if(fontNames != "")
			{
				Cell.Style.Add("font-names", fontNames);
			}
			// Font size.
			if(ControlStyle.Font.Size.IsEmpty == false)
			{
				Cell.Style.Add("font-size", 
					ControlStyle.Font.Size.ToString());
			}
			// Font overline.
			if(ControlStyle.Font.Overline == true)
			{
				if(textDecorations != "") 
					textDecorations += " ";
				textDecorations += "overline";
			}
			// Font strikeout.
			if(ControlStyle.Font.Strikeout == true)
			{
				if(textDecorations != "") 
					textDecorations += " ";
				textDecorations += "line-through";
			}
			// Font underline.
			if(ControlStyle.Font.Underline == true)
			{
				if(textDecorations != "") 
					textDecorations += " ";
				textDecorations += "underline";
			}
			// Font decorations.
			if(textDecorations != "")
			{
				Cell.Style.Add("text-decoration", textDecorations);
			}
			// Font fore colour.
			if(ControlStyle.ForeColor.IsEmpty == false)
			{
				Cell.Style.Add("color", HtmlColorString(ControlStyle.ForeColor));
			}
			// Height.
			if(ControlStyle.Height.IsEmpty == false)
			{
				Cell.Style.Add("height", ControlStyle.Height.ToString());
			}
			// Width.
			if(ControlStyle.Width.IsEmpty == false)
			{
				Cell.Style.Add("width", ControlStyle.Height.ToString());
			}
		}
		/// <summary>
		/// Creates the expand/collapse hyper-links for the control.
		/// </summary>
		/// <param name="UseImages">
		/// True if images are used in the action link (right-hand side
		/// of the control). Otherwise false.
		/// </param>
		/// <param name="TitleIsLink">
		/// True if the title is a link (left-hand side of the control).
		/// Otherwise false.
		/// </param>
		/// <returns>
		/// The hyper-link as a string.
		/// </returns>
		private string PanelUrl(bool UseImages, bool TitleIsLink)
		{
			string titleLink = (TitleIsLink == true) ? 
				this.ID + "_TitleLink" : "";
			if(UseImages == false)
			{
				return
					"javascript:DataPanel_ExpandCollapse('" +
					this.ID + "_ContentRow','" +
					titleLink + "','" +
					this.ID + "_ActionLink','" +
					this.ID + "_CurrentState','" +
					this.CollapseText + "','" +
					this.ExpandText + "');";
			}
			else
			{
				return
					"javascript:DataPanel_ExpandCollapseImage('" +
					this.ID + "_ContentRow','" +
					titleLink + "','" +
					this.ID + "_ActionLink','" +
					this.ID + "_CurrentState','" +
					this.ExpandImageUrl + "','" +
					this.CollapseImageUrl + "','" +
					this.CollapseText + "','" +
					this.ExpandText + "');";
			}
		}
		/// <summary>
		/// Updates the control appearance state based on the
		/// various properties of the control.
		/// </summary>
		private void UpdateControlsState()
		{
			// Is the title a hyperlink?
			bool titleIsLink = (this.AllowTitleExpandCollapse == true);
			// Are images being used for the action hyperlink?
			bool useImages = (this.CollapseImageUrl != "") &&
				(this.ExpandImageUrl != "") && 
				(this.HideActionExpandCollapse == false);
			// Sets the hidden input value to the correct collapsed state.
			_currentState.Value = (this.Collapsed == true) ? "true" : "false";
			// Sets the appropriate tooltip for the title link, either
			// collapsed or expanded., if the control is collapsible.
			if( (titleIsLink == true) && (this.Collapsible == true) )
			{
				_titleLink.ToolTip = (this.Collapsed == true) ?
					this.ExpandText : this.CollapseText;
			}
			// Sets the title link URL.
			if( (this.AllowTitleExpandCollapse == true) &&
				(this.Collapsible == true) )
			{
				_titleLink.NavigateUrl = PanelUrl(useImages, titleIsLink);
			}
			// Is the action link visible?
			_actionLink.Visible = (this.HideActionExpandCollapse == false) ?
				true : false;
			// Sets the appropriate tooltip for the action link, either
			// collapsed or expanded.
			_actionLink.ToolTip = (this.Collapsed == true) ?
				this.ExpandText : this.CollapseText;
			// Sets the appropriate tooltip for the action link, if the
			// control is collapsible.
			if(this.Collapsible == true)
			{
				_actionLink.NavigateUrl = PanelUrl(useImages, titleIsLink);
				_actionLink.Text = (this.Collapsed == false) ?
					this.CollapseText : this.ExpandText;
			}
			else
			{
				_actionLink.Text = "";
				_actionLink.Visible = false;
			}
			// If the action links are images, sets the correct
			// image URLs of the collapsed/expanded images.
			if(useImages == true)
			{
				_actionLink.ImageUrl = (this.Collapsed == false) ?
					this.CollapseImageUrl : this.ExpandImageUrl;
			}
			// Sets the user content row element visible or invisible
			// depending on whether the control is expaned or collapsed.
			_contentRow.Style.Add("display", (this.Collapsed == false) ?
				"" : "none");
		}
		/// <summary>
		/// Helper property to return the correct control id for the
		/// header panel object.
		/// </summary>
		private string HeaderPanelId
		{
			get { return this.ID + "_HeaderPanel"; }
		}
		/// <summary>
		/// Helper property to return the correct control id for the
		/// hidden input object that holds the control collapsed state
		/// string.
		/// </summary>
		private string CollapsedStateId
		{
			get { return this.ID + "_CurrentState"; }
		}
		/// <summary>
		/// Creates the control sub-components of the
		/// control.
		/// </summary>
		/// <remarks>
		/// Sets the user <see cref="_contentCell"/> container
		/// object render delegate as the
		/// <see cref="RenderPanelContent"/> method using the
		/// <see cref="Control.SetRenderMethodDelegate">
		/// SetRenderMethodDelegate</see> method. This allows
		/// the content (this panel control) to be rendered as a 
		/// child object of the <see cref="_contentCell"/> container.
		/// </remarks>
		private void CreateControlComponents()
		{
			// Create the header panel and
			// set its state to this control state.
			_headerPanel = new Panel();
			_headerPanel.ID = this.HeaderPanelId;
			_headerPanel.Visible = this.Visible;
			_headerPanel.Width = this.Width;
			_headerPanel.Height = this.Height;
			foreach(string key in this.Attributes.Keys)
			{
				string val = this.Attributes[key];
				_headerPanel.Attributes.Add(key, val);
			}
			_headerPanel.ToolTip = this.ToolTip;
			_headerPanel.ApplyStyle(this.ControlStyle);
			// Ensures that if a border is required,
			// the content panel (this) does not get a 
			// second border.
			this.BorderStyle = BorderStyle.NotSet;
			// Create the hidden input and set the
			// control collapsed state value.
			_currentState = new HtmlInputHidden();
			_currentState.ID = this.CollapsedStateId;
			_currentState.Value = this.Collapsed ? "true" : "false";
			// Create the control table container.
			_controlTable = new HtmlTable();
			_controlTable.ID = this.ID + "_Table";
			_controlTable.Width = "100%";
			_controlTable.CellSpacing = 0;
			_controlTable.CellPadding = 2;
			_controlTable.Visible = true;
			// Create the header container row.
			_headerRow = new HtmlTableRow();
			_headerRow.ID = this.ID + "_HeaderRow";
			_headerRow.Visible = true;
			// Create the title cell.
			_titleCell = new HtmlTableCell();
			_titleCell.ID = this.ID + "_TitleCell";
			_titleCell.Align = "left";
			_titleCell.Width = "90%";
			_titleCell.Visible = true;
			// Set the style of the cell to the TitleStyle.
			ApplyStyle(_titleCell, this.TitleStyle);
			// Create the title link.
			_titleLink = new HyperLink();
			_titleLink.ID = this.ID + "_TitleLink";
			_titleLink.Text = this.TitleText;
			// Set the style of the link to the TitleStyle.
			_titleLink.ApplyStyle(this.TitleStyle);
			_titleLink.Visible = true;
			// Create the action cell.
			_actionCell = new HtmlTableCell();
			_actionCell.ID = this.ID + "_ActionCell";
			_actionCell.Align = "right";
			_actionCell.Width = "10%";
			_actionCell.Visible = true;
			_actionCell.NoWrap = true;
			// Set the style of the cell to the TitleStyle.
			ApplyStyle(_actionCell, this.TitleStyle);
			// Create the action link.
			_actionLink = new HyperLink();
			_actionLink.ID = this.ID + "_actionLink";
			// Set the style of the link to the TitleStyle.
			_actionLink.ApplyStyle(this.TitleStyle);
			// Create the row container for this control.
			_contentRow = new HtmlTableRow();
			_contentRow.ID = this.ID + "_ContentRow";
			_contentRow.Visible = true;
			// Create the cell container for this control.
			_contentCell = new HtmlTableCell();
			_contentCell.ID = this.ID + "_ContentCell";
			_contentCell.ColSpan = 2;
			_contentCell.Align = "left";
			_contentCell.Visible = true;
			// Set the render method for this control panel.
			// This allows us to render this control (user content panel)
			// as a child of the content cell.
			_contentCell.SetRenderMethodDelegate(
				new RenderMethod(RenderPanelContent));
		}
		/// <summary>
		/// Delegate member called when the <see cref="_contentCell"/>
		/// is to be rendered.
		/// </summary>
		/// <param name="Writer">HTML writer object to render to.</param>
		/// <param name="Ctl">Control to render.</param>
		/// <remarks>
		/// This control (DataPanel) contents are rendered using the
		/// <see cref="WebControl.RenderContents">RenderContents</see>
		/// method. This method renders all the child controls added to 
		/// the DataPanel control during design time.
		/// </remarks>
		private void RenderPanelContent(HtmlTextWriter Writer, Control Ctl) 
		{
			this.RenderContents(Writer);
		}
		/// <summary>
		/// Sets the parent/child control relationships for all child
		/// controls in this composite control.
		/// </summary>
		private void SetControlHierarchy()
		{
			_controlTable.Rows.Add(_headerRow);
			_headerRow.Cells.Add(_titleCell);
			_titleCell.Controls.Add(_titleLink);
			_headerRow.Cells.Add(_actionCell);
			_actionCell.Controls.Add(_actionLink);
			_controlTable.Rows.Add(_contentRow);
			_contentRow.Cells.Add(_contentCell);
			_headerPanel.Controls.Add(_controlTable);
			_headerPanel.Controls.Add(_currentState);
		}
		/// <summary>
		/// Overrides the <see cref="Control.CreateChildControls"/> method.
		/// </summary>
		protected override void CreateChildControls()
		{
			this.ClearChildViewState();
			this.CreateControlComponents();
			this.SetControlHierarchy();
			this.UpdateControlsState();
		}
		/// <summary>
		/// Overrides the <see cref="Control.OnPreRender"/> method.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> object that contains 
		/// the event data.</param>
		protected override void OnPreRender(System.EventArgs e)
		{
			base.OnPreRender(e);
			RegisterControlScript();
		}
		/// <summary>
		/// Overrides the <see cref="Control.LoadViewState"/> method.
		/// </summary>
		/// <param name="savedState">An <see cref="Object"/>
		/// that represents the control
		/// state to be restored.</param>
		/// <remarks>
		/// Loads the <see cref="TitleStyle"/> state in addition
		/// to the base class states.
		/// </remarks>
		protected override void LoadViewState(object savedState)
		{
			if(savedState != null) 
			{
				object[] state = (object[])savedState;
				if( (state.Length > 0) && (state[0] != null) )
					base.LoadViewState(state[0]);
				if( (state.Length > 1) && (state[1] != null) )
					((IStateManager)_titleStyle).LoadViewState(state[1]);
			}
		}
		/// <summary>
		/// Overrides the <see cref="Control.SaveViewState"/>
		/// method.
		/// </summary>
		/// <returns>
		/// Returns the server control's current view state.
		/// If there is no view state associated with the control,
		/// this method returns a null reference (Nothing in Visual Basic).
		/// </returns>
		/// <remarks>
		/// Saves the <see cref="TitleStyle"/> state in addition
		/// to the base class states.
		/// </remarks>
		protected override object SaveViewState()
		{
			object baseState = base.SaveViewState();
			object titleStyle = (_titleStyle != null) ? 
				((IStateManager)_titleStyle).SaveViewState() : null;
			object[] state = new object[2]{ baseState, titleStyle };
			return state;
		}
		/// <summary>
		/// Overrides the <see cref="Control.OnInit"/> method.
		/// </summary>
		/// <param name="e">The 
		/// <see cref="System.EventArgs">EventArgs</see> object 
		/// that contains the event data. 
		/// </param>
		/// <remarks>
		/// The control registers for postback data in this method.
		/// </remarks>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if(this.Page != null)
			{
				this.Page.RegisterRequiresPostBack(this);
			}
		}
		/// <summary>
		/// Sends server control content to a provided 
		/// <see cref="HtmlTextWriter"/> object, which writes the 
		/// content to be rendered on the client.
		/// </summary>
		/// <param name="writer">
		/// The HtmlTextWriter object that receives the server 
		/// control content.</param>
		/// <remarks>
		/// The control creates an outer panel object that is rendered
		/// in this method.
		/// </remarks>
		protected override void Render(HtmlTextWriter writer)
		{
			_headerPanel.ID = this.ID;
			_headerPanel.RenderControl(writer);
			_headerPanel.ID = this.HeaderPanelId;
		}
		#endregion Implementation
		#region IPostBackDataHandler Members
		/// <summary>
		/// This method is required for the
		/// <see cref="IPostBackDataHandler"/> implementation.
		/// </summary>
		/// <remarks>
		/// This method is not implemented as no
		/// postback event is required.
		/// </remarks>
		public void RaisePostDataChangedEvent()
		{
			// No implementation required.
		}
		/// <summary>
		/// Processes post back data for this control.
		/// </summary>
		/// <param name="postDataKey">
		/// The key identifier for the control.</param>
		/// <param name="postCollection">
		/// The collection of all incoming name values.
		/// </param>
		/// <returns>
		/// True if the server control's state changes as a 
		/// result of the post back; otherwise false.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The control sets the <see cref="Collapsed"/>
		/// property from the value obtained from the
		/// hidden input field used to determine the
		/// collapsed state and set by script on the client.
		/// </para>
		/// <para>
		/// This method always returns false, as no event
		/// notification is required.
		/// </para>
		/// </remarks>
		public bool LoadPostData(string postDataKey,
			NameValueCollection postCollection)
		{
			string collapsedState = postCollection[this.CollapsedStateId];
			if(collapsedState != null)
			{
				this.Collapsed = (collapsedState == "false") ? false : true;
			}
			return false;
		}
		#endregion
	}
	#endregion DataPanel
}