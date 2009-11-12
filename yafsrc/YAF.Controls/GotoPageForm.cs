using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace YAF.Controls
{
  /// <summary>
  /// The goto page forum event args.
  /// </summary>
  public class GotoPageForumEventArgs : EventArgs
  {
    /// <summary>
    /// The _goto page.
    /// </summary>
    private int _gotoPage;

    /// <summary>
    /// Initializes a new instance of the <see cref="GotoPageForumEventArgs"/> class.
    /// </summary>
    /// <param name="gotoPage">
    /// The goto page.
    /// </param>
    public GotoPageForumEventArgs(int gotoPage)
      : base()
    {
      GotoPage = gotoPage;
    }

    /// <summary>
    /// Gets or sets GotoPage.
    /// </summary>
    public int GotoPage
    {
      get
      {
        return this._gotoPage;
      }

      set
      {
        this._gotoPage = value;
      }
    }
  }

  /// <summary>
  /// The goto page form.
  /// </summary>
  public class GotoPageForm : BaseControl
  {
    /// <summary>
    /// The _div inner.
    /// </summary>
    private HtmlGenericControl _divInner = new HtmlGenericControl();

    /// <summary>
    /// The _goto button.
    /// </summary>
    private Button _gotoButton = new Button();

    /// <summary>
    /// The _goto page value.
    /// </summary>
    private int _gotoPageValue;

    /// <summary>
    /// The _goto text box.
    /// </summary>
    private TextBox _gotoTextBox = new TextBox();

    /// <summary>
    /// The _header text.
    /// </summary>
    private Label _headerText = new Label();

    /// <summary>
    /// The _main panel.
    /// </summary>
    private Panel _mainPanel = new Panel();

    /// <summary>
    /// Initializes a new instance of the <see cref="GotoPageForm"/> class.
    /// </summary>
    public GotoPageForm()
      : base()
    {
    }

    /// <summary>
    /// Gets MainPanel.
    /// </summary>
    public Panel MainPanel
    {
      get
      {
        return this._mainPanel;
      }
    }

    /// <summary>
    /// Gets InnerDiv.
    /// </summary>
    public HtmlGenericControl InnerDiv
    {
      get
      {
        return this._divInner;
      }
    }

    /// <summary>
    /// Gets or sets GotoPageValue.
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
    /// Gets GotoTextBoxClientID.
    /// </summary>
    public string GotoTextBoxClientID
    {
      get
      {
        return this._gotoTextBox.ClientID;
      }
    }

    /// <summary>
    /// Gets GotoButtonClientID.
    /// </summary>
    public string GotoButtonClientID
    {
      get
      {
        return this._gotoButton.ClientID;
      }
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

      BuildForm();
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
      if (!String.IsNullOrEmpty(PageContext.Localization.TransPage))
      {
        this._headerText.Text = PageContext.Localization.GetText("COMMON", "GOTOPAGE_HEADER");
        this._gotoButton.Text = PageContext.Localization.GetText("COMMON", "GO");
      }
      else
      {
        // non-localized for admin pages
        this._headerText.Text = "Goto Page...";
        this._gotoButton.Text = "Go";
      }
    }

    /// <summary>
    /// The build form.
    /// </summary>
    protected void BuildForm()
    {
      Controls.Add(this._mainPanel);

      this._mainPanel.CssClass = "gotoBase";
      this._mainPanel.ID = GetExtendedID("gotoBase");

      var divHeader = new HtmlGenericControl("div");

      divHeader.Attributes.Add("class", "gotoHeader");
      divHeader.ID = GetExtendedID("divHeader");

      this._mainPanel.Controls.Add(divHeader);

      this._headerText.ID = GetExtendedID("headerText");

      divHeader.Controls.Add(this._headerText);

      this._divInner.Attributes.Add("class", "gotoInner");
      this._divInner.ID = GetExtendedID("gotoInner");

      this._mainPanel.Controls.Add(this._divInner);

      this._gotoButton.ID = GetExtendedID("GotoButton");
      this._gotoButton.Style.Add(HtmlTextWriterStyle.Width, "30px");
      this._gotoButton.CausesValidation = false;
      this._gotoButton.UseSubmitBehavior = false;
      this._gotoButton.Click += new EventHandler(GotoButtonClick);

      // text box...
      this._gotoTextBox.ID = GetExtendedID("GotoTextBox");
      this._gotoTextBox.Style.Add(HtmlTextWriterStyle.Width, "30px");

      this._divInner.Controls.Add(this._gotoTextBox);
      this._divInner.Controls.Add(this._gotoButton);

      PageContext.PageElements.RegisterJsBlockStartup(
        String.Format(@"GotoPageFormKeyUp_{0}", ClientID), 
        String.Format(
          @"Sys.Application.add_load(function() {{ jQuery('#{0}').bind('keydown', function(e) {{ if(e.keyCode == 13) {{ jQuery('#{1}').click(); return false; }} }}); }});", 
          this._gotoTextBox.ClientID, 
          this._gotoButton.ClientID));

      // add enter key support...
      // _gotoTextBox.Attributes.Add( "onkeydown",
      // String.Format(
      // @"if( ( event.which || event.keyCode ) && (event.which == 13 || event.keyCode == 13) ) {{ jQuery('#{0}').click(); return false; }} return true;",
      // _gotoButton.ClientID ) );
      // document.getElementById('" +
      // _gotoButton.ClientID + "').click();return false;}} else {return true}; ") );
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      writer.WriteLine(String.Format(@"<div id=""{0}"" style=""display:none"" class=""gotoPageForm"">", ClientID));

      base.Render(writer);

      writer.WriteLine("</div>");
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
      if (GotoPageClick != null)
      {
        // attempt to parse the page value...
        if (int.TryParse(this._gotoTextBox.Text.Trim(), out this._gotoPageValue))
        {
          // valid, fire the event...
          GotoPageClick(this, new GotoPageForumEventArgs(GotoPageValue));
        }
      }

      // clear the old value...
      this._gotoTextBox.Text = string.Empty;
    }

    /// <summary>
    /// The goto page click.
    /// </summary>
    public event EventHandler<GotoPageForumEventArgs> GotoPageClick;
  }
}