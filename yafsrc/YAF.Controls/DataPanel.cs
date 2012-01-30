/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Drawing;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Utils;
  using YAF.Types;

  #endregion

  #region DataPanel Class

  /// <summary>
  /// Web control which provides a panel control as a 
  ///   collapsible/expandable control container. The control
  ///   provides a panel title and action link, either/both 
  ///   may provide the expand/collapse action as a hot-link.
  /// </summary>
  /// <remarks>
  /// The control is used by registering the library in the
  ///   Visual Studio Toolbox, and then dragging it onto the
  ///   ASP.NET form.  For the control to be truly
  ///   expandable/collapsible, the style tag 'POSITION: absolute'
  ///   should be removed from the HTML control definition, or
  ///   else the control should be place in another control
  ///   such as a table.
  /// </remarks>
  [ToolboxData("<{0}:DataPanel runat=server></{0}:DataPanel>")]
  public class DataPanel : Panel, IPostBackDataHandler
  {
    #region Constants and Fields

    /// <summary>
    ///   The constant string "AllowTitleExpandCollapse".
    /// </summary>
    /// <remarks>
    ///   This value is the key for the property
    ///   state parameter in the <see cref = "Control.ViewState" />
    ///   object.
    /// </remarks>
    private const string _allowTitleExpandCollapse = "AllowTitleExpandCollapse";

    /// <summary>
    ///   The constant string "CollapseImageUrl".
    /// </summary>
    /// <remarks>
    ///   This value is the key for the property
    ///   state parameter in the 
    ///   <see cref = "Control.ViewState">ViewState</see>
    ///   object.
    /// </remarks>
    private const string _collapseImageUrl = "CollapseImageUrl";

    /// <summary>
    ///   The constant string "CollapseText".
    /// </summary>
    /// <remarks>
    ///   This value is the key for the property
    ///   state parameter in the 
    ///   <see cref = "Control.ViewState">ViewState</see>
    ///   object.
    /// </remarks>
    private const string _collapseText = "CollapseText";

    /// <summary>
    ///   The constant string "Collapsed".
    /// </summary>
    /// <remarks>
    ///   This value is the key for the property
    ///   state parameter in the 
    ///   <see cref = "Control.ViewState">ViewState</see>
    ///   object.
    /// </remarks>
    private const string _collapsed = "Collapsed";

    /// <summary>
    ///   The constant string "Collapsible".
    /// </summary>
    /// <remarks>
    ///   This value is the key for the property
    ///   state parameter in the 
    ///   <see cref = "Control.ViewState">ViewState</see>
    ///   object.
    /// </remarks>
    private const string _collapsible = "Collapsible";

    /// <summary>
    ///   Name of the script block to control the collapsing/expanding
    ///   of the control.
    /// </summary>
    private const string _dataPanelScript = "DataPanelJs";

    /// <summary>
    ///   The constant string "ExpandImageUrl".
    /// </summary>
    /// <remarks>
    ///   This value is the key for the property
    ///   state parameter in the 
    ///   <see cref = "Control.ViewState">ViewState</see>
    ///   object.
    /// </remarks>
    private const string _expandImageUrl = "ExpandImageUrl";

    /// <summary>
    ///   The constant string "ExpandText".
    /// </summary>
    /// <remarks>
    ///   This value is the key for the property
    ///   state parameter in the 
    ///   <see cref = "Control.ViewState">ViewState</see>
    ///   object.
    /// </remarks>
    private const string _expandText = "ExpandText";

    /// <summary>
    ///   The constant string "TitleText".
    /// </summary>
    /// <remarks>
    ///   This value is the key for the property
    ///   state parameter in the 
    ///   <see cref = "Control.ViewState">ViewState</see>
    ///   object.
    /// </remarks>
    private const string _titleText = "TitleText";

    /// <summary>
    ///   The cell element that contains the expand/collapse action link or
    ///   image for the control.
    /// </summary>
    private HtmlTableCell _actionCell;

    /// <summary>
    ///   The action hyperlink element of the control.
    /// </summary>
    /// <remarks>
    ///   This link is active only if the
    ///   <see cref = "HideActionExpandCollapse" /> is false.
    /// </remarks>
    private HyperLink _actionLink;

    /// <summary>
    ///   The cell element for the user content panel.
    /// </summary>
    private HtmlTableCell _contentCell;

    /// <summary>
    ///   The row element for the user content controls.
    /// </summary>
    private HtmlTableRow _contentRow;

    /// <summary>
    ///   The layout table element for the control.
    /// </summary>
    private HtmlTable _controlTable;

    /// <summary>
    ///   Hidden input field which is set by the client script to
    ///   indicate if the control has been expanded or collapsed by the user.
    /// </summary>
    /// <remarks>
    ///   If the control is collapsed, the value of this field is "true" (string).
    ///   If the control is expanded, the value of this field is "false" (string).
    /// </remarks>
    private HtmlInputHidden _currentState;

    /// <summary>
    ///   The header panel element that is contained by the the
    ///   header row element.
    /// </summary>
    private Panel _headerPanel;

    /// <summary>
    ///   The header row element for the control.
    /// </summary>
    private HtmlTableRow _headerRow;

    /// <summary>
    ///   The constant string "HideActionExpandCollapse".
    /// </summary>
    /// <remarks>
    ///   This value is the key for the property
    ///   state parameter in the 
    ///   <see cref = "Control.ViewState">ViewState</see>
    ///   object.
    /// </remarks>
    private string _hideActionExpandCollapse = "HideActionExpandCollapse";

    /// <summary>
    ///   The header title cell element for the control.
    /// </summary>
    private HtmlTableCell _titleCell;

    /// <summary>
    ///   The title hyperlink element of the control.
    /// </summary>
    /// <remarks>
    ///   This link is active only if the 
    ///   <see cref = "AllowTitleExpandCollapse" /> property is true.
    /// </remarks>
    private HyperLink _titleLink;

    /// <summary>
    ///   The member used for the <see cref = "TitleStyle" />
    ///   property.
    /// </summary>
    private Style _titleStyle = new Style();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "DataPanel" /> class. 
    ///   Default Constructor.
    /// </summary>
    /// <remarks>
    ///   If the <see cref = "WebControl.Width">Width</see> of the control is not set,
    ///   the value of '100%' is set as default.
    /// </remarks>
    public DataPanel()
    {
      if (base.Width.IsEmpty)
      {
        this.Width = Unit.Parse("100%");
      }
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Set to true to allow clicking the panel
    ///   header title to expand/collapse the content panel.
    /// </summary>
    /// <remarks>
    ///   The <see cref = "Control.ViewState">ViewState</see> 
    ///   object is used to save the
    ///   value of this property between postbacks.
    /// </remarks>
    [Description("Set to true to allow clicking the panel" + "header title to expand/collapse the content panel.")]
    [Category("Appearance")]
    [DefaultValue(false)]
    public bool AllowTitleExpandCollapse
    {
      get
      {
        if (this.ViewState[_allowTitleExpandCollapse] == null)
        {
          return false;
        }

        return Convert.ToBoolean(this.ViewState[_allowTitleExpandCollapse]);
      }

      set
      {
        this.ViewState[_allowTitleExpandCollapse] = value;
      }
    }

    /// <summary>
    ///   Url of the image to use for the collapse-action link.
    /// </summary>
    /// <remarks>
    ///   The <see cref = "Control.ViewState">ViewState</see> 
    ///   object is used to save the
    ///   value of this property between postbacks.
    /// </remarks>
    [NotNull, Description("Url of the image to use for the collapse-" + "action link.")]
    [Category("Appearance")]
    [DefaultValue("")]
    public string CollapseImageUrl
    {
      get
      {
        if (this.ViewState[_collapseImageUrl] == null)
        {
          return string.Empty;
        }

        return this.ViewState[_collapseImageUrl].ToString();
      }

      set
      {
        this.ViewState[_collapseImageUrl] = value;
      }
    }

    /// <summary>
    ///   Text to use for the collapse-action link.
    /// </summary>
    /// <remarks>
    ///   The <see cref = "Control.ViewState">ViewState</see> 
    ///   object is used to save the
    ///   value of this property between postbacks.
    /// </remarks>
    [NotNull, Description("Text to use for the collapse-action link.")]
    [Category("Appearance")]
    [DefaultValue("Collapse")]
    public string CollapseText
    {
      get
      {
        if (this.ViewState[_collapseText] == null)
        {
          return "Collapse";
        }

        return this.ViewState[_collapseText].ToString();
      }

      set
      {
        this.ViewState[_collapseText] = value;
      }
    }

    /// <summary>
    ///   Set to true to collapse the content panel, 
    ///   else false to expand.
    /// </summary>
    /// <remarks>
    ///   The <see cref = "Control.ViewState">ViewState</see>
    ///   object is used to save the
    ///   value of this property between postbacks.
    /// </remarks>
    [Description("Set to true to collapse the content panel," + " else false to expand.")]
    [Category("Appearance")]
    [DefaultValue(true)]
    public bool Collapsed
    {
      get
      {
        if (this.ViewState[_collapsed] == null)
        {
          return true;
        }

        return Convert.ToBoolean(this.ViewState[_collapsed]);
      }

      set
      {
        this.ViewState[_collapsed] = value;
      }
    }

    /// <summary>
    ///   Set to true to enable the user content panel to
    ///   collapse, else false to disable.
    /// </summary>
    /// <remarks>
    ///   The <see cref = "Control.ViewState">ViewState</see> 
    ///   object is used to save the
    ///   value of this property between postbacks.
    /// </remarks>
    [Description("Set to true to enable the content panel to" + " collapse, else false to disable.")]
    [Category("Appearance")]
    [DefaultValue(true)]
    public bool Collapsible
    {
      get
      {
        if (this.ViewState[_collapsible] == null)
        {
          return true;
        }

        return Convert.ToBoolean(this.ViewState[_collapsible]);
      }

      set
      {
        this.ViewState[_collapsible] = value;
      }
    }

    /// <summary>
    ///   Url of the image to use for the expand-action link.
    /// </summary>
    /// <remarks>
    ///   The <see cref = "Control.ViewState">ViewState</see> 
    ///   object is used to save the
    ///   value of this property between postbacks.
    /// </remarks>
    [NotNull, Description("Url of the image to use for the expand-action link.")]
    [Category("Appearance")]
    [DefaultValue("")]
    public string ExpandImageUrl
    {
      get
      {
        if (this.ViewState[_expandImageUrl] == null)
        {
          return string.Empty;
        }

        return this.ViewState[_expandImageUrl].ToString();
      }

      set
      {
        this.ViewState[_expandImageUrl] = value;
      }
    }

    /// <summary>
    ///   Text to use for the expand-action link.
    /// </summary>
    /// <remarks>
    ///   The <see cref = "Control.ViewState">ViewState</see> 
    ///   object is used to save the
    ///   value of this property between postbacks.
    /// </remarks>
    [NotNull, Description("Text to use for the expand-action link.")]
    [Category("Appearance")]
    [DefaultValue("Expand")]
    public string ExpandText
    {
      get
      {
        if (this.ViewState[_expandText] == null)
        {
          return "Expand";
        }

        return this.ViewState[_expandText].ToString();
      }

      set
      {
        this.ViewState[_expandText] = value;
      }
    }

    /// <summary>
    ///   Set to true to hide the text/image action link which expands
    ///   or collapses the content panel.
    /// </summary>
    /// <remarks>
    ///   The <see cref = "Control.ViewState">ViewState</see> 
    ///   object is used to save the
    ///   value of this property between postbacks.
    /// </remarks>
    [Description("Set to true to hide the text/image action link which" + " expands or collapses the content panel.")]
    [Category("Appearance")]
    [DefaultValue(false)]
    public bool HideActionExpandCollapse
    {
      get
      {
        if (this.ViewState[this._hideActionExpandCollapse] == null)
        {
          return false;
        }

        return Convert.ToBoolean(this.ViewState[this._hideActionExpandCollapse]);
      }

      set
      {
        this.ViewState[this._hideActionExpandCollapse] = value;
      }
    }

    /// <summary>
    ///   Display style of the control title header.
    /// </summary>
    /// <remarks>
    ///   The <see cref = "SaveViewState" /> override is used to save
    ///   the value of this property between postbacks, while
    ///   the <see cref = "LoadViewState" /> override is used to restore
    ///   the value.
    /// </remarks>
    [NotNull, Description("Display style of the control title header.")]
    [Category("Style")]
    [DefaultValue(typeof(Style))]
    [PersistenceMode(PersistenceMode.Attribute)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Style TitleStyle
    {
      get
      {
        if (this._titleStyle == null)
        {
          this._titleStyle = new Style();
        }

        if (this.IsTrackingViewState)
        {
          ((IStateManager)this._titleStyle).TrackViewState();

          // Force the complete style to be saved in viewstate
          // every time.
          this._titleStyle.BackColor = this._titleStyle.BackColor;
          this._titleStyle.BorderColor = this._titleStyle.BorderColor;
          this._titleStyle.BorderStyle = this._titleStyle.BorderStyle;
          this._titleStyle.BorderWidth = this._titleStyle.BorderWidth;
          this._titleStyle.CssClass = this._titleStyle.CssClass;
          this._titleStyle.Font.Bold = this._titleStyle.Font.Bold;
          this._titleStyle.Font.Italic = this._titleStyle.Font.Italic;
          this._titleStyle.Font.Name = this._titleStyle.Font.Name;
          this._titleStyle.Font.Names = this._titleStyle.Font.Names;
          this._titleStyle.Font.Overline = this._titleStyle.Font.Overline;
          this._titleStyle.Font.Size = this._titleStyle.Font.Size;
          this._titleStyle.Font.Strikeout = this._titleStyle.Font.Strikeout;
          this._titleStyle.Font.Underline = this._titleStyle.Font.Underline;
          this._titleStyle.ForeColor = this._titleStyle.ForeColor;
          this._titleStyle.Height = this._titleStyle.Height;
          this._titleStyle.Width = this._titleStyle.Width;
        }

        return this._titleStyle;
      }
    }

    /// <summary>
    ///   Text that will appear on the control title header.
    /// </summary>
    /// <remarks>
    ///   The <see cref = "Control.ViewState">ViewState</see> 
    ///   object is used to save the
    ///   value of this property between postbacks.
    /// </remarks>
    [NotNull, Description("Text that will appear on the control title header.")]
    [Category("Appearance")]
    [DefaultValue("")]
    public string TitleText
    {
      get
      {
        if (this.ViewState[_titleText] == null)
        {
          return string.Empty;
        }

        return this.ViewState[_titleText].ToString();
      }

      set
      {
        this.ViewState[_titleText] = value;
      }
    }

    /// <summary>
    ///   Helper property to return the correct control id for the
    ///   hidden input object that holds the control collapsed state
    ///   string.
    /// </summary>
    private string CollapsedStateId
    {
      get
      {
        return this.ID + "_CurrentState";
      }
    }

    /// <summary>
    ///   Helper property to return the correct control id for the
    ///   header panel object.
    /// </summary>
    private string HeaderPanelId
    {
      get
      {
        return this.ID + "_HeaderPanel";
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IPostBackDataHandler

    /// <summary>
    /// Processes post back data for this control.
    /// </summary>
    /// <param name="postDataKey">
    /// The key identifier for the control.
    /// </param>
    /// <param name="postCollection">
    /// The collection of all incoming name values.
    /// </param>
    /// <returns>
    /// True if the server control's state changes as a 
    ///   result of the post back; otherwise false.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The control sets the <see cref="Collapsed"/>
    ///     property from the value obtained from the
    ///     hidden input field used to determine the
    ///     collapsed state and set by script on the client.
    ///   </para>
    /// <para>
    /// This method always returns false, as no event
    ///     notification is required.
    ///   </para>
    /// </remarks>
    public bool LoadPostData([NotNull] string postDataKey, [NotNull] NameValueCollection postCollection)
    {
      string collapsedState = postCollection[this.CollapsedStateId];
      if (collapsedState != null)
      {
        this.Collapsed = (collapsedState == "false") ? false : true;
      }

      return false;
    }

    /// <summary>
    /// This method is required for the
    ///   <see cref="IPostBackDataHandler"/> implementation.
    /// </summary>
    /// <remarks>
    /// This method is not implemented as no
    ///   postback event is required.
    /// </remarks>
    public void RaisePostDataChangedEvent()
    {
      // No implementation required.
    }

    #endregion

    #endregion

    #region Methods

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
    /// Overrides the <see cref="Control.LoadViewState"/> method.
    /// </summary>
    /// <param name="savedState">
    /// An <see cref="Object"/>
    ///   that represents the control
    ///   state to be restored.
    /// </param>
    /// <remarks>
    /// Loads the <see cref="TitleStyle"/> state in addition
    ///   to the base class states.
    /// </remarks>
    protected override void LoadViewState([NotNull] object savedState)
    {
      if (savedState != null)
      {
        var state = (object[])savedState;
        if ((state.Length > 0) && (state[0] != null))
        {
          base.LoadViewState(state[0]);
        }

        if ((state.Length > 1) && (state[1] != null))
        {
          ((IStateManager)this._titleStyle).LoadViewState(state[1]);
        }
      }
    }

    /// <summary>
    /// Overrides the <see cref="Control.OnInit"/> method.
    /// </summary>
    /// <param name="e">
    /// The 
    ///   <see cref="System.EventArgs">EventArgs</see> object 
    ///   that contains the event data. 
    /// </param>
    /// <remarks>
    /// The control registers for postback data in this method.
    /// </remarks>
    protected override void OnInit([NotNull] EventArgs e)
    {
      base.OnInit(e);
      if (this.Page != null)
      {
        this.Page.RegisterRequiresPostBack(this);
      }
    }

    /// <summary>
    /// Overrides the <see cref="Control.OnPreRender"/> method.
    /// </summary>
    /// <param name="e">
    /// An <see cref="EventArgs"/> object that contains 
    ///   the event data.
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      base.OnPreRender(e);
      this.RegisterControlScript();
    }

    /// <summary>
    /// Sends server control content to a provided 
    ///   <see cref="HtmlTextWriter"/> object, which writes the 
    ///   content to be rendered on the client.
    /// </summary>
    /// <param name="writer">
    /// The HtmlTextWriter object that receives the server 
    ///   control content.
    /// </param>
    /// <remarks>
    /// The control creates an outer panel object that is rendered
    ///   in this method.
    /// </remarks>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
      this._headerPanel.ID = this.ID;
      this._headerPanel.RenderControl(writer);
      this._headerPanel.ID = this.HeaderPanelId;
    }

    /// <summary>
    /// Overrides the <see cref="Control.SaveViewState"/>
    ///   method.
    /// </summary>
    /// <returns>
    /// Returns the server control's current view state.
    ///   If there is no view state associated with the control,
    ///   this method returns a null reference (Nothing in Visual Basic).
    /// </returns>
    /// <remarks>
    /// Saves the <see cref="TitleStyle"/> state in addition
    ///   to the base class states.
    /// </remarks>
    protected override object SaveViewState()
    {
      object baseState = base.SaveViewState();
      object titleStyle = (this._titleStyle != null) ? ((IStateManager)this._titleStyle).SaveViewState() : null;
      var state = new[] { baseState, titleStyle };
      return state;
    }

    /// <summary>
    /// Applies a <see cref="Style"/> object to an
    ///   <see cref="HtmlTableCell"/> object.
    /// </summary>
    /// <param name="cell">
    /// The cell.
    /// </param>
    /// <param name="controlStyle">
    /// The control Style.
    /// </param>
    /// The
    /// <see cref="Style"/>
    /// properties are converted
    /// to the
    /// <see cref="HtmlTableCell"/>
    /// object
    /// <see cref="CssStyleCollection"/>
    /// attributes.
    private void ApplyStyle([NotNull] HtmlTableCell cell, [NotNull] Style controlStyle)
    {
      string fontNames = string.Empty;
      string textDecorations = string.Empty;

      // Background colour.
      if (controlStyle.BackColor.IsEmpty == false)
      {
        cell.Style.Add("background-color", this.HtmlColorString(controlStyle.BackColor));
      }

      // Border colour.
      if (controlStyle.BorderColor.IsEmpty == false)
      {
        cell.Style.Add("border-color", this.HtmlColorString(controlStyle.BorderColor));
      }

      // Border style.
      if ((controlStyle.BorderStyle.ToString() != string.Empty) &&
          (controlStyle.BorderStyle.ToString().ToLower() != "notset"))
      {
        cell.Style.Add("border-style", controlStyle.BorderStyle.ToString());
      }

      // Border width.
      if (controlStyle.BorderWidth.IsEmpty == false)
      {
        cell.Style.Add("border-width", controlStyle.BorderWidth.ToString());
      }

      // Class.
      if (controlStyle.CssClass != string.Empty)
      {
        cell.Attributes.Add("class", controlStyle.CssClass);
      }

      // Font bold.
      if (controlStyle.Font.Bold)
      {
        cell.Style.Add("font-weight", "bold");
      }

      // Font italic.
      if (controlStyle.Font.Italic)
      {
        cell.Style.Add("font-style", "italic");
      }

      // Font name.
      if (controlStyle.Font.Name != string.Empty)
      {
        fontNames = controlStyle.Font.Name;
        cell.Style.Add("font-family", fontNames);
      }

      // Font names.
      if (controlStyle.Font.Names.Length > 0)
      {
        foreach (string s in controlStyle.Font.Names)
        {
          if (fontNames.IndexOf(s) == -1)
          {
            fontNames += "," + s;
          }
        }
      }

      if (fontNames != string.Empty)
      {
        cell.Style.Add("font-names", fontNames);
      }

      // Font size.
      if (controlStyle.Font.Size.IsEmpty == false)
      {
        cell.Style.Add("font-size", controlStyle.Font.Size.ToString());
      }

      // Font overline.
      if (controlStyle.Font.Overline)
      {
        if (textDecorations != string.Empty)
        {
          textDecorations += " ";
        }

        textDecorations += "overline";
      }

      // Font strikeout.
      if (controlStyle.Font.Strikeout)
      {
        if (textDecorations != string.Empty)
        {
          textDecorations += " ";
        }

        textDecorations += "line-through";
      }

      // Font underline.
      if (controlStyle.Font.Underline)
      {
        if (textDecorations != string.Empty)
        {
          textDecorations += " ";
        }

        textDecorations += "underline";
      }

      // Font decorations.
      if (textDecorations != string.Empty)
      {
        cell.Style.Add("text-decoration", textDecorations);
      }

      // Font fore colour.
      if (controlStyle.ForeColor.IsEmpty == false)
      {
        cell.Style.Add("color", this.HtmlColorString(controlStyle.ForeColor));
      }

      // Height.
      if (controlStyle.Height.IsEmpty == false)
      {
        cell.Style.Add("height", controlStyle.Height.ToString());
      }

      // Width.
      if (controlStyle.Width.IsEmpty == false)
      {
        cell.Style.Add("width", controlStyle.Height.ToString());
      }
    }

    /// <summary>
    /// Creates the control sub-components of the
    ///   control.
    /// </summary>
    /// <remarks>
    /// Sets the user <see cref="_contentCell"/> container
    ///   object render delegate as the
    ///   <see cref="RenderPanelContent"/> method using the
    ///   <see cref="Control.SetRenderMethodDelegate">
    ///     SetRenderMethodDelegate</see> method. This allows
    ///   the content (this panel control) to be rendered as a 
    ///   child object of the <see cref="_contentCell"/> container.
    /// </remarks>
    private void CreateControlComponents()
    {
      // Create the header panel and
      // set its state to this control state.
      this._headerPanel = new Panel();
      this._headerPanel.ID = this.HeaderPanelId;
      this._headerPanel.Visible = this.Visible;
      this._headerPanel.Width = this.Width;
      this._headerPanel.Height = this.Height;
      foreach (string key in this.Attributes.Keys)
      {
        string val = this.Attributes[key];
        this._headerPanel.Attributes.Add(key, val);
      }

      this._headerPanel.ToolTip = this.ToolTip;
      this._headerPanel.ApplyStyle(this.ControlStyle);

      // Ensures that if a border is required,
      // the content panel (this) does not get a 
      // second border.
      this.BorderStyle = BorderStyle.NotSet;

      // Create the hidden input and set the
      // control collapsed state value.
      this._currentState = new HtmlInputHidden();
      this._currentState.ID = this.CollapsedStateId;
      this._currentState.Value = this.Collapsed ? "true" : "false";

      // Create the control table container.
      this._controlTable = new HtmlTable();
      this._controlTable.ID = this.ID + "_Table";
      this._controlTable.Width = "100%";
      this._controlTable.CellSpacing = 0;
      this._controlTable.CellPadding = 2;
      this._controlTable.Visible = true;

      // Create the header container row.
      this._headerRow = new HtmlTableRow();
      this._headerRow.ID = this.ID + "_HeaderRow";
      this._headerRow.Visible = true;

      // Create the title cell.
      this._titleCell = new HtmlTableCell();
      this._titleCell.ID = this.ID + "_TitleCell";
      this._titleCell.Align = "left";
      this._titleCell.Width = "90%";
      this._titleCell.Visible = true;

      // Set the style of the cell to the TitleStyle.
      this.ApplyStyle(this._titleCell, this.TitleStyle);

      // Create the title link.
      this._titleLink = new HyperLink();
      this._titleLink.ID = this.ID + "_TitleLink";
      this._titleLink.Text = this.TitleText;

      // Set the style of the link to the TitleStyle.
      this._titleLink.ApplyStyle(this.TitleStyle);
      this._titleLink.Visible = true;

      // Create the action cell.
      this._actionCell = new HtmlTableCell();
      this._actionCell.ID = this.ID + "_ActionCell";
      this._actionCell.Align = "right";
      this._actionCell.Width = "10%";
      this._actionCell.Visible = true;
      this._actionCell.NoWrap = true;

      // Set the style of the cell to the TitleStyle.
      this.ApplyStyle(this._actionCell, this.TitleStyle);

      // Create the action link.
      this._actionLink = new HyperLink();
      this._actionLink.ID = this.ID + "_ActionLink";

      // Set the style of the link to the TitleStyle.
      this._actionLink.ApplyStyle(this.TitleStyle);

      // Create the row container for this control.
      this._contentRow = new HtmlTableRow();
      this._contentRow.ID = this.ID + "_ContentRow";
      this._contentRow.Visible = true;

      // Create the cell container for this control.
      this._contentCell = new HtmlTableCell();
      this._contentCell.ID = this.ID + "_ContentCell";
      this._contentCell.ColSpan = 2;
      this._contentCell.Align = "left";
      this._contentCell.Visible = true;

      // Set the render method for this control panel.
      // This allows us to render this control (user content panel)
      // as a child of the content cell.
      this._contentCell.SetRenderMethodDelegate(this.RenderPanelContent);
    }

    /// <summary>
    /// Converts a <see cref="Color"/> object to an HTML
    ///   compliant string.
    /// </summary>
    /// <param name="styleColor">
    /// The style Color.
    /// </param>
    /// <returns>
    /// The HTML colour string.
    /// </returns>
    private string HtmlColorString(Color styleColor)
    {
      // Format ARGB
      return "#" + (styleColor.ToArgb() & 0x00FFFFFF).ToString("X6");
    }

    /// <summary>
    /// Creates the expand/collapse hyper-links for the control.
    /// </summary>
    /// <param name="useImages">
    /// The use Images.
    /// </param>
    /// <param name="titleIsLink">
    /// The title Is Link.
    /// </param>
    /// <returns>
    /// The hyper-link as a string.
    /// </returns>
    private string PanelUrl(bool useImages, bool titleIsLink)
    {
      string titleLink = titleIsLink ? this.ID + "_TitleLink" : string.Empty;
      if (useImages == false)
      {
        return "javascript:DataPanel_ExpandCollapse('" + this.ID + "_ContentRow','" + titleLink + "','" + this.ID +
               "_ActionLink','" + this.ID + "_CurrentState','" + this.CollapseText + "','" + this.ExpandText + "');";
      }
      else
      {
        return "javascript:DataPanel_ExpandCollapseImage('" + this.ID + "_ContentRow','" + titleLink + "','" + this.ID +
               "_ActionLink','" + this.ID + "_CurrentState','" + this.ExpandImageUrl + "','" + this.CollapseImageUrl +
               "','" + this.CollapseText + "','" + this.ExpandText + "');";
      }
    }

    /// <summary>
    /// Method to extract the script block text from the control
    ///   resources and register it as a client script block.
    /// </summary>
    private void RegisterControlScript()
    {
      ScriptManager.RegisterClientScriptInclude(
        this, this.GetType(), _dataPanelScript, YafForumInfo.GetURLToResource("js/DataPanel.js"));
    }

    /// <summary>
    /// Delegate member called when the <see cref="_contentCell"/>
    ///   is to be rendered.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="ctl">
    /// The ctl.
    /// </param>
    /// <remarks>
    /// This control (DataPanel) contents are rendered using the
    ///   <see cref="WebControl.RenderContents">RenderContents</see>
    ///   method. This method renders all the child controls added to 
    ///   the DataPanel control during design time.
    /// </remarks>
    private void RenderPanelContent([NotNull] HtmlTextWriter writer, [NotNull] Control ctl)
    {
      this.RenderContents(writer);
    }

    /// <summary>
    /// Sets the parent/child control relationships for all child
    ///   controls in this composite control.
    /// </summary>
    private void SetControlHierarchy()
    {
      this._controlTable.Rows.Add(this._headerRow);
      this._headerRow.Cells.Add(this._titleCell);
      this._titleCell.Controls.Add(this._titleLink);
      this._headerRow.Cells.Add(this._actionCell);
      this._actionCell.Controls.Add(this._actionLink);
      this._controlTable.Rows.Add(this._contentRow);
      this._contentRow.Cells.Add(this._contentCell);
      this._headerPanel.Controls.Add(this._controlTable);
      this._headerPanel.Controls.Add(this._currentState);
    }

    /// <summary>
    /// Updates the control appearance state based on the
    ///   various properties of the control.
    /// </summary>
    private void UpdateControlsState()
    {
      // Is the title a hyperlink?
      bool titleIsLink = this.AllowTitleExpandCollapse;

      // Are images being used for the action hyperlink?
      bool useImages = (this.CollapseImageUrl != string.Empty) && (this.ExpandImageUrl != string.Empty) &&
                       (this.HideActionExpandCollapse == false);

      // Sets the hidden input value to the correct collapsed state.
      this._currentState.Value = this.Collapsed ? "true" : "false";

      // Sets the appropriate tooltip for the title link, either
      // collapsed or expanded., if the control is collapsible.
      if (titleIsLink && this.Collapsible)
      {
        this._titleLink.ToolTip = this.Collapsed ? this.ExpandText : this.CollapseText;
      }

      // Sets the title link URL.
      if (this.AllowTitleExpandCollapse && this.Collapsible)
      {
        this._titleLink.NavigateUrl = this.PanelUrl(useImages, titleIsLink);
      }

      // Is the action link visible?
      this._actionLink.Visible = (this.HideActionExpandCollapse == false) ? true : false;

      // Sets the appropriate tooltip for the action link, either
      // collapsed or expanded.
      this._actionLink.ToolTip = this.Collapsed ? this.ExpandText : this.CollapseText;

      // Sets the appropriate tooltip for the action link, if the
      // control is collapsible.
      if (this.Collapsible)
      {
        this._actionLink.NavigateUrl = this.PanelUrl(useImages, titleIsLink);
        this._actionLink.Text = (this.Collapsed == false) ? this.CollapseText : this.ExpandText;
      }
      else
      {
        this._actionLink.Text = string.Empty;
        this._actionLink.Visible = false;
      }

      // If the action links are images, sets the correct
      // image URLs of the collapsed/expanded images.
      if (useImages)
      {
        this._actionLink.ImageUrl = (this.Collapsed == false) ? this.CollapseImageUrl : this.ExpandImageUrl;
      }

      // Sets the user content row element visible or invisible
      // depending on whether the control is expaned or collapsed.
      this._contentRow.Style.Add("display", (this.Collapsed == false) ? string.Empty : "none");
    }

    #endregion
  }

  #endregion DataPanel
}