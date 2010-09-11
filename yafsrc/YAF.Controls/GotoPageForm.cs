namespace YAF.Controls
{
  #region Using

  using System;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The goto page forum event args.
  /// </summary>
  public class GotoPageForumEventArgs : EventArgs
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="GotoPageForumEventArgs"/> class.
    /// </summary>
    /// <param name="gotoPage">
    /// The goto page.
    /// </param>
    public GotoPageForumEventArgs(int gotoPage)
    {
      this.GotoPage = gotoPage;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets GotoPage.
    /// </summary>
    public int GotoPage { get; set; }

    #endregion
  }

  /// <summary>
  /// The goto page form.
  /// </summary>
  public class GotoPageForm : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _div inner.
    /// </summary>
    private readonly HtmlGenericControl _divInner = new HtmlGenericControl();

    /// <summary>
    ///   The _goto button.
    /// </summary>
    private readonly Button _gotoButton = new Button();

    /// <summary>
    ///   The _goto text box.
    /// </summary>
    private readonly TextBox _gotoTextBox = new TextBox();

    /// <summary>
    ///   The _header text.
    /// </summary>
    private readonly Label _headerText = new Label();

    /// <summary>
    ///   The _main panel.
    /// </summary>
    private readonly Panel _mainPanel = new Panel();

    /// <summary>
    ///   The _goto page value.
    /// </summary>
    private int _gotoPageValue;

    #endregion

    #region Events

    /// <summary>
    ///   The goto page click.
    /// </summary>
    public event EventHandler<GotoPageForumEventArgs> GotoPageClick;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets GotoButtonClientID.
    /// </summary>
    public string GotoButtonClientID
    {
      get
      {
        return this._gotoButton.ClientID;
      }
    }

    /// <summary>
    ///   Gets or sets GotoPageValue.
    /// </summary>
    public int GotoPageValue
    {
      get
      {
        return this._gotoPageValue;
      }

      set
      {
        this._gotoPageValue = value;
      }
    }

    /// <summary>
    ///   Gets GotoTextBoxClientID.
    /// </summary>
    public string GotoTextBoxClientID
    {
      get
      {
        return this._gotoTextBox.ClientID;
      }
    }

    /// <summary>
    ///   Gets InnerDiv.
    /// </summary>
    public HtmlGenericControl InnerDiv
    {
      get
      {
        return this._divInner;
      }
    }

    /// <summary>
    ///   Gets MainPanel.
    /// </summary>
    public Panel MainPanel
    {
      get
      {
        return this._mainPanel;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The build form.
    /// </summary>
    protected void BuildForm()
    {
      this.Controls.Add(this._mainPanel);

      this._mainPanel.CssClass = "gotoBase";
      this._mainPanel.ID = this.GetExtendedID("gotoBase");

      var divHeader = new HtmlGenericControl("div");

      divHeader.Attributes.Add("class", "gotoHeader");
      divHeader.ID = this.GetExtendedID("divHeader");

      this._mainPanel.Controls.Add(divHeader);

      this._headerText.ID = this.GetExtendedID("headerText");

      divHeader.Controls.Add(this._headerText);

      this._divInner.Attributes.Add("class", "gotoInner");
      this._divInner.ID = this.GetExtendedID("gotoInner");

      this._mainPanel.Controls.Add(this._divInner);

      this._gotoButton.ID = this.GetExtendedID("GotoButton");
      this._gotoButton.Style.Add(HtmlTextWriterStyle.Width, "30px");
      this._gotoButton.CausesValidation = false;
      this._gotoButton.UseSubmitBehavior = false;
      this._gotoButton.Click += this.GotoButtonClick;

      // text box...
      this._gotoTextBox.ID = this.GetExtendedID("GotoTextBox");
      this._gotoTextBox.Style.Add(HtmlTextWriterStyle.Width, "30px");

      this._divInner.Controls.Add(this._gotoTextBox);
      this._divInner.Controls.Add(this._gotoButton);

      this.PageContext.PageElements.RegisterJsBlockStartup(
        @"GotoPageFormKeyUp_{0}".FormatWith(this.ClientID), 
        @"Sys.Application.add_load(function() {{ jQuery('#{0}').bind('keydown', function(e) {{ if(e.keyCode == 13) {{ jQuery('#{1}').click(); return false; }} }}); }});"
          .FormatWith(this._gotoTextBox.ClientID, this._gotoButton.ClientID));

      // add enter key support...
      // _gotoTextBox.Attributes.Add( "onkeydown",
      // String.Format(
      // @"if( ( event.which || event.keyCode ) && (event.which == 13 || event.keyCode == 13) ) {{ jQuery('#{0}').click(); return false; }} return true;",
      // _gotoButton.ClientID ) );
      // document.getElementById('" +
      // _gotoButton.ClientID + "').click();return false;}} else {return true}; ") );
    }

    /// <summary>
    /// The goto button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void GotoButtonClick(object sender, EventArgs e)
    {
      if (this.GotoPageClick != null)
      {
        // attempt to parse the page value...
        if (int.TryParse(this._gotoTextBox.Text.Trim(), out this._gotoPageValue))
        {
          // valid, fire the event...
          this.GotoPageClick(this, new GotoPageForumEventArgs(this.GotoPageValue));
        }
      }

      // clear the old value...
      this._gotoTextBox.Text = string.Empty;
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);

      this.BuildForm();
    }

    /// <summary>
    /// The on load.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      // localization has to be done in here so as to not attempt
      // to localize before the class has been created
      if (this.PageContext.Localization.TransPage.IsSet())
      {
        this._headerText.Text = this.PageContext.Localization.GetText("COMMON", "GOTOPAGE_HEADER");
        this._gotoButton.Text = this.PageContext.Localization.GetText("COMMON", "GO");
      }
      else
      {
        // non-localized for admin pages
        this._headerText.Text = "Goto Page...";
        this._gotoButton.Text = "Go";
      }
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      writer.WriteLine(@"<div id=""{0}"" style=""display:none"" class=""gotoPageForm"">".FormatWith(this.ClientID));

      base.Render(writer);

      writer.WriteLine("</div>");
    }

    #endregion
  }
}